#include "hw_config.h"
#include "usb_lib.h"
#include "usb_desc.h"
#include "usb_pwr.h"

#include <string.h>
#include <ctype.h>

#define STRLEN(m)				(sizeof(m) - 1)
#define HEARTBEAT_LED_START		2000
#define HEARTBEAT_LED_END		2100

typedef struct Config_s {
	bool EchoDisable;
} Config_t;

Config_t Config;

static char UsbCommand[256];
static char CdcCommand[256];

static const char m_Help[]			=	"\r\nAvailable commands:\r\n"
										"-------------------\r\n"
										"help       ?      this info\r\n"
										"echo on    EON    echo on\r\n"
										"echo off   EOFF   echo off\r\n"
										"power off  POFF   power off RF\r\n"
										"power on   PON    power on RF\r\n"
										"power off  POFF   power off RF\r\n"
										;
static const char cmd_Help[]		=	"help";
static const char cmd_Help2[]		=	"?";
static const char cmd_PowerOn[]		=	"power on";
static const char cmd_PON[]			=	"pon";
static const char cmd_PowerOff[]	=	"power off";
static const char cmd_POFF[]		=	"poff";
static const char cmd_EchoOn[]		=	"echo on";
static const char cmd_EON[]			=	"eon";
static const char cmd_EchoOff[]		=	"echo off";
static const char cmd_EOFF[]		=	"eoff";

typedef bool (* FnCommand)(bool, char *);

typedef struct CommandDef_s {
	FnCommand	Fn;
	const char * Command;
} __attribute__((packed)) CommandDef_t;

static bool CmdPowerOn	(bool isUsv, char * cmd);
static bool CmdPowerOff	(bool isUsv, char * cmd);
static bool CmdEchoOn	(bool isUsv, char * cmd);
static bool CmdEchoOff	(bool isUsv, char * cmd);
static bool CmdHelp		(bool isUsv, char * cmd);

const CommandDef_t Commands[] = {
	{	CmdPowerOn,		cmd_PowerOn,	},
	{	CmdPowerOn,		cmd_PON,		},
	{	CmdPowerOff,	cmd_PowerOff,	},
	{	CmdPowerOff,	cmd_POFF,		},
	{	CmdEchoOn,		cmd_EchoOn,		},
	{	CmdEchoOn,		cmd_EON,		},
	{	CmdEchoOff,		cmd_EchoOff,	},
	{	CmdEchoOff,		cmd_EOFF,		},
	{	CmdHelp,		cmd_Help,		},
	{	CmdHelp,		cmd_Help2,		},
	{	NULL,			NULL			}
};

void strUpper(char * src)
{
	while (*src != 0)
	{
		*src = tolower(*src);
		src++;
	}
}

static void MsgSend(bool isUsb, const char * msg)
{
	if (isUsb)
		USB_Send(msg);
	else
		USART_Send(msg);
}

static bool CmdEchoOn(bool isUsb, char * cmd)
{
	Config.EchoDisable = false;
	return true;
}

static bool CmdEchoOff(bool isUsb, char * cmd)
{
	Config.EchoDisable = true;
	return true;
}

static bool CmdPowerOn(bool isUsb, char * cmd)
{
	MsgSend(isUsb, "Power ON\r\n");
	return true;
}

static bool CmdPowerOff(bool isUsb, char * cmd)
{
	MsgSend(isUsb, "Power OFF\r\n");
	return true;
}

static bool CmdHelp(bool isUsb, char * cmd)
{
	MsgSend(isUsb, m_Help);
	return true;
}

/******************************************************************************
 * @brief	Main routine.
 */
bool ProcessCommand(bool isUsb, char * cmd)
{
	const CommandDef_t *cmds = Commands;

	STM_EVAL_LEDOn(LED2);
	strUpper(cmd);
	while (cmds->Command != NULL)
	{
		if (strcmp(cmd, cmds->Command) == 0)
		{
			if ((cmds->Fn)(isUsb, cmd))
			{
				MsgSend(isUsb, "OK\r\n");
				return true;
			}
			break;
		}
		cmds++;
	}
	MsgSend(isUsb, "ERROR\r\n");
	return false;
}

/******************************************************************************
 * @brief	Initialize IWDG
 */
void IWDG_Init(void)
{
	/* Enable the LSI oscillator ************************************************/
	RCC_LSICmd(ENABLE);

	/* Wait till LSI is ready */
	while (RCC_GetFlagStatus(RCC_FLAG_LSIRDY) == RESET)
	{ }

	IWDG_WriteAccessCmd(IWDG_WriteAccess_Enable);	/* Enable write access to IWDG_PR and IWDG_RLR registers */

											/* IWDG counter clock: LSI / 128 */
	IWDG_SetPrescaler(IWDG_Prescaler_128);	/* 1 / (LSI_VALUE = 37000) * 128 = 3.5 ms */
	IWDG_SetReload(1000);					/* 1 / (LSI_VALUE = 37000) * 128 * 1000 = 3.5 s */
	IWDG_ReloadCounter();					/* Reload IWDG counter */
	IWDG_Enable();			/* Enable IWDG (the LSI oscillator will be enabled by hardware) */
}

/******************************************************************************
 * @brief	Main routine.
 */
int main(void)
{
	uint32_t led_delay = 0;
	uint8_t ch;
	uint16_t UsbCommandIndex = 0;
	uint16_t CdcCommandIndex = 0;
	uint32_t iwdg_time;
	
	if (RCC_GetFlagStatus(RCC_FLAG_IWDGRST) != RESET)
	{	/* IWDGRST flag set */
		
	}
	RCC_ClearFlag();

	SystemCoreClockUpdate();
	DBGMCU_Config(DBGMCU_SLEEP | DBGMCU_STOP | DBGMCU_STANDBY, ENABLE);
	DBGMCU_APB1PeriphConfig(DBGMCU_IWDG_STOP | DBGMCU_RTC_STOP, ENABLE);

	STM_EVAL_LEDInit(LED1);
	STM_EVAL_LEDInit(LED2);
	STM_EVAL_LEDInit(LED3);
	STM_EVAL_LEDOn(LED3);

	IWDG_Init();

	if (SystemCoreClock < 8000000
	||	SysTick_Config(SystemCoreClock / 1000)
		)
	{
		while (1);
	}

	USART_Config();
	USB_Init();
 
	while (1)
	{
		if (UsbDeviceInfo.bDeviceState == CONFIGURED)
		{
			STM_EVAL_LEDOff(LED3);
			led_delay++;
			if (led_delay == HEARTBEAT_LED_END)
			{
				STM_EVAL_LEDOff(LED2);
				led_delay = 0;
			}
			else if (led_delay == HEARTBEAT_LED_START)
			{
				STM_EVAL_LEDOn(LED2);
			}

			while (USB_Get_Data(&ch))
			{
				if ( ! Config.EchoDisable)
					USB_Send_Byte(ch);

				if (ch == '\r')
				{
					if ( ! Config.EchoDisable)
						USB_Send_Byte('\n');
					ch = '\0';
				}
				if (UsbCommandIndex < (sizeof(UsbCommand) - 1))
					UsbCommand[UsbCommandIndex++] = ch;
				if (ch == 0)
				{
					ProcessCommand(true, UsbCommand);
					UsbCommandIndex = 0;
				}
			}
		}
		else
		{
			STM_EVAL_LEDOn(LED3);
			STM_EVAL_LEDOff(LED2);
			led_delay = 0;
		}
		
		while (USART_Get_Data(&ch))
		{
			if ( ! Config.EchoDisable)
				USB_Send_Byte(ch);

			if (ch == '\r')
			{
				if ( ! Config.EchoDisable)
					USB_Send_Byte('\n');
				ch = '\0';
			}
			if (CdcCommandIndex < (sizeof(CdcCommand) - 1))
				CdcCommand[CdcCommandIndex++] = ch;
			if (ch == 0)
			{
				ProcessCommand(false, CdcCommand);
				CdcCommandIndex = 0;
			}
		}
		EnterSleepMode();
		if ((millis() - iwdg_time) > 1000)
		{
			iwdg_time = millis();
			IWDG_ReloadCounter();	/* Reload IWDG counter */
		}
	}
}

void EnterSleepMode(void)
{
#if defined( STM32L1XX_MD )
	PWR_EnterSleepMode(PWR_Regulator_ON, PWR_SLEEPEntry_WFI);
#endif
}

#ifdef USE_FULL_ASSERT
/******************************************************************************
 * @brief	Reports the name of the source file and the source line number
 *			where the assert_param error has occurred.
 * @param	file: pointer to the source file name
 *			line: assert_param error line source number
 */
void assert_failed(uint8_t* file, uint32_t line)
{
	/* User can add his own implementation to report the file name and line number,
		ex: printf("Wrong parameters value: file %s on line %d\r\n", file, line) */

	/* Infinite loop */
	while (1)
	{}
}
#endif
