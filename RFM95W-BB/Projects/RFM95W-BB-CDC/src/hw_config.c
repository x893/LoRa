/**
 ******************************************************************************
 * @file	hw_config.c
 * @author	MCD Application Team
 * @version V4.0.0
 * @date	21-January-2013
 * @brief	Hardware Configuration & Setup
 ******************************************************************************
 */

#include "hw_config.h"

#include "usb_lib.h"
#include "usb_prop.h"
#include "usb_desc.h"
#include "usb_pwr.h"

uint8_t	USB_Rx_Buffer [USB_RX_DATA_SIZE];
uint8_t	USB_Tx_Buffer [USB_TX_DATA_SIZE];
uint8_t	USART_Rx_Buffer [USART_RX_DATA_SIZE];
uint8_t	USART_Tx_Buffer [USART_TX_DATA_SIZE];

RingBuffer_t USB_Rx		= {	USB_Rx_Buffer, sizeof(USB_Rx_Buffer), 0, 0, 0	};
RingBuffer_t USB_Tx		= {	USB_Tx_Buffer, sizeof(USB_Tx_Buffer), 0, 0, 0	};

RingBuffer_t USART_Rx	= {	USART_Rx_Buffer, sizeof(USART_Rx_Buffer), 0, 0, 0	};
RingBuffer_t USART_Tx	= {	USART_Tx_Buffer, sizeof(USART_Tx_Buffer), 0, 0, 0	};

/******************************************************************************
 * @brief	Configures Main system clocks & power
 */
void USB_Config(void)
{
	EXTI_InitTypeDef EXTI_InitStructure;
	NVIC_InitTypeDef NVIC_InitStructure;

#if !defined(STM32L1XX_MD) && !defined(STM32L1XX_HD) && !defined(STM32L1XX_MD_PLUS)
	GPIO_InitTypeDef GPIO_InitStructure;
#endif /* STM32L1XX_MD && STM32L1XX_XD */	

#if defined(USB_USE_EXTERNAL_PULLUP)
	GPIO_InitTypeDef	GPIO_InitStructure;
#endif /* USB_USE_EXTERNAL_PULLUP */ 

#if defined(STM32L1XX_MD)		\
||	defined(STM32L1XX_HD)		\
||	defined(STM32L1XX_MD_PLUS)	\
||	defined(STM32F37X)			\
||	defined(STM32F30X)
	/* Enable the SYSCFG module clock */
	RCC_APB2PeriphClockCmd(RCC_APB2Periph_SYSCFG, ENABLE);

#endif /* STM32L1XX_XD */
	 
#if !defined(STM32L1XX_MD)		\
&&	!defined(STM32L1XX_HD)		\
&&	!defined(STM32L1XX_MD_PLUS)	\
&&	!defined(STM32F37X)			\
&&	!defined(STM32F30X)

	/* Enable USB_DISCONNECT GPIO clock */
	RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIO_DISCONNECT, ENABLE);

	/* Configure USB pull-up pin */
	GPIO_InitStructure.GPIO_Pin = USB_DISCONNECT_PIN;
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_OD;
	GPIO_Init(USB_DISCONNECT, &GPIO_InitStructure);

#endif /* STM32L1XX_MD && STM32L1XX_XD */
	 
#if defined(USB_USE_EXTERNAL_PULLUP)

	/* Enable the USB disconnect GPIO clock */
	RCC_AHBPeriphClockCmd(RCC_AHBPeriph_GPIO_DISCONNECT, ENABLE);

	/* USB_DISCONNECT used as USB pull-up */
	GPIO_InitStructure.GPIO_Pin = USB_DISCONNECT_PIN;
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT;
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_2MHz;
	GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL;
	GPIO_Init(USB_DISCONNECT, &GPIO_InitStructure);	

#endif /* USB_USE_EXTERNAL_PULLUP */ 

#if defined(STM32F37X) || defined(STM32F30X)
	
	/* Enable the USB disconnect GPIO clock */
	RCC_AHBPeriphClockCmd(RCC_AHBPeriph_GPIO_DISCONNECT, ENABLE);
	
	/*Set PA11,12 as IN - USB_DM,DP*/
	RCC_AHBPeriphClockCmd(RCC_AHBPeriph_GPIOA, ENABLE);
	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_11 | GPIO_Pin_12;
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF;
	GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL;
	GPIO_Init(GPIOA, &GPIO_InitStructure);

	/* SET PA11,12 for USB: USB_DM,DP*/
	GPIO_PinAFConfig(GPIOA, GPIO_PinSource11, GPIO_AF_14);
	GPIO_PinAFConfig(GPIOA, GPIO_PinSource12, GPIO_AF_14);

	/* USB_DISCONNECT used as USB pull-up */
	GPIO_InitStructure.GPIO_Pin = USB_DISCONNECT_PIN;
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_2MHz;
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT;
	GPIO_InitStructure.GPIO_OType = GPIO_OType_OD;
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL;
	GPIO_Init(USB_DISCONNECT, &GPIO_InitStructure);

#endif /* STM32F37X && STM32F30X */

	/* Configure the EXTI line 18 connected internally to the USB IP */
	EXTI_ClearITPendingBit(EXTI_Line18);
	EXTI_InitStructure.EXTI_Line = EXTI_Line18;
	EXTI_InitStructure.EXTI_Trigger = EXTI_Trigger_Rising;
	EXTI_InitStructure.EXTI_LineCmd = ENABLE;
	EXTI_Init(&EXTI_InitStructure);

#if defined(STM32L1XX_MD) || defined(STM32L1XX_HD) || defined(STM32L1XX_MD_PLUS) 
	/* Enable USB clock */
	RCC_APB1PeriphClockCmd(RCC_APB1Periph_USB, ENABLE);
	
#else 
	/* Select USBCLK source */
	RCC_USBCLKConfig(RCC_USBCLKSource_PLLCLK_1Div5);

	/* Enable the USB clock */
	RCC_APB1PeriphClockCmd(RCC_APB1Periph_USB, ENABLE);
#endif /* STM32L1XX_MD */

	/* 2 bit for pre-emption priority, 2 bits for subpriority */
	NVIC_PriorityGroupConfig(NVIC_PriorityGroup_2);	

	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 2;
	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 0;

#if defined(STM32L1XX_MD) || defined(STM32L1XX_HD)|| defined(STM32L1XX_MD_PLUS)

	NVIC_InitStructure.NVIC_IRQChannel = USB_LP_IRQn;
	NVIC_Init(&NVIC_InitStructure);
	
	/* Enable the USB Wake-up interrupt */
	NVIC_InitStructure.NVIC_IRQChannel = USB_FS_WKUP_IRQn;
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 0;
	NVIC_Init(&NVIC_InitStructure);
	
#elif defined(STM32F37X)

	/* Enable the USB interrupt */
	NVIC_InitStructure.NVIC_IRQChannel = USB_LP_IRQn;
	NVIC_Init(&NVIC_InitStructure);

	/* Enable the USB Wake-up interrupt */
	NVIC_InitStructure.NVIC_IRQChannel = USBWakeUp_IRQn;
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 0;
	NVIC_Init(&NVIC_InitStructure);
	
#else

	NVIC_InitStructure.NVIC_IRQChannel = USB_LP_CAN1_RX0_IRQn;
	NVIC_Init(&NVIC_InitStructure);
	
	/* Enable the USB Wake-up interrupt */
	NVIC_InitStructure.NVIC_IRQChannel = USBWakeUp_IRQn;
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 0;
	NVIC_Init(&NVIC_InitStructure);
#endif /* STM32L1XX_XD */

}

/******************************************************************************
 * @brief	Power-off system clocks and power while entering suspend mode
 */
void Enter_LowPowerMode(void)
{	/* Set the device state to suspend */
	pInformation->bDeviceState = SUSPENDED;
}

/******************************************************************************
 * @brief	Restores system clocks and power while exiting suspend mode
 */
void Leave_LowPowerMode(void)
{
	DEVICE_INFO *pInfo = &UsbDeviceInfo;

	/* Set the device state to the correct state */
	if (pInfo->Current_Configuration != 0)
		pInformation->bDeviceState = CONFIGURED;
	else
		pInformation->bDeviceState = ATTACHED;

	SystemInit();	/* Enable SystemCoreClock */
}

/******************************************************************************
 * @brief	Software Connection/Disconnection of USB Cable
 */
void USB_Cable_Config (FunctionalState NewState)
{
#if defined(STM32L1XX_MD) || defined (STM32L1XX_HD)|| (STM32L1XX_MD_PLUS)

	if (NewState != DISABLE)
		STM32L15_USB_CONNECT;
	else
		STM32L15_USB_DISCONNECT;

#else /* USE_STM3210B_EVAL or USE_STM3210E_EVAL */

	if (NewState != DISABLE)
		GPIO_ResetBits(USB_DISCONNECT, USB_DISCONNECT_PIN);
	else
		GPIO_SetBits(USB_DISCONNECT, USB_DISCONNECT_PIN);

#endif /* STM32L1XX_MD */
}

/******************************************************************************
 * @brief	Configure the EVAL_COM1 with default values.
 */
void USART_Config(void)
{
	USART_InitTypeDef USART_InitStructure;
	NVIC_InitTypeDef NVIC_InitStructure;

	/* EVAL_COM1 default configuration */
	/* EVAL_COM1 configured as follow:
			- BaudRate = 9600 baud	
			- Word Length = 8 Bits
			- One Stop Bit
			- Parity Odd
			- Hardware flow control disabled
			- Receive and transmit enabled
	*/
	USART_InitStructure.USART_BaudRate = 115200;
	USART_InitStructure.USART_WordLength = USART_WordLength_8b;
	USART_InitStructure.USART_StopBits = USART_StopBits_1;
	USART_InitStructure.USART_Parity = USART_Parity_Odd;
	USART_InitStructure.USART_HardwareFlowControl = USART_HardwareFlowControl_None;
	USART_InitStructure.USART_Mode = USART_Mode_Rx | USART_Mode_Tx;

	/* Configure and enable the USART */
	STM_EVAL_COMInit(COM1, &USART_InitStructure);

	/* Enable USART Interrupt */
	NVIC_InitStructure.NVIC_IRQChannel = RF95W_COM_IRQn;
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 0;
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 1;
	NVIC_Init(&NVIC_InitStructure);

	/* Enable the USART Receive interrupt */
	USART_ITConfig(EVAL_COM1, USART_IT_RXNE, ENABLE);
}

/******************************************************************************
 * @brief	Send the received data from UART 0 to USB
 */
bool RingBufferPut(RingBuffer_t * ring, uint8_t data)
{
	uint16_t next = ring->InIndex + 1;
	if (next == ring->Size)
		next = 0;

	if (next != ring->OutIndex)
	{
		int was_masked = __disable_irq();
		ring->Buffer[ring->InIndex] = data;
		ring->InIndex = next;
		if (!was_masked)
			__enable_irq();
		return true;
	}

	return false;
}

/******************************************************************************
 * @brief	Send the received data from UART 0 to USB
 */
bool RingBufferGet(RingBuffer_t * ring, uint8_t * data)
{
	if (ring->InIndex != ring->OutIndex)
	{
		int was_masked = __disable_irq();

		*data = ring->Buffer[ring->OutIndex];
		ring->OutIndex++;
		if (ring->OutIndex >= ring->Size)
			ring->OutIndex = 0;

		if (!was_masked)
			__enable_irq();
		return true;
	}
	return false;
}

/******************************************************************************
 * @brief	Send the received data from UART 0 to USB
 */
void USART_Receive_Data(void)
{
	RingBufferPut(&USART_Rx, USART_ReceiveData(EVAL_COM1));
}

/******************************************************************************
 * @brief	Send the received data from USB to the UART 0.
 * @param	Input		data_buffer: data address.
 * @param	Nb_bytes	number of bytes to send.
 */
void USB_Receive_Data(uint8_t* src, uint8_t length)
{
	// USB_Send_Data(src, length);
	while (length != 0)
	{
		--length;
		RingBufferPut(&USB_Rx, *src++);
	}
}

/******************************************************************************
 * @brief	Send the data to USB
 */
bool USB_Send_Byte(uint8_t data)
{
	uint32_t started = millis();
	while ( ! RingBufferPut(&USB_Tx, data))
	{
		EnterSleepMode();
		if ((millis() - started) > 20)
			return false;
	}
	return true;
}

void USB_Send_Data(const char * src, uint16_t length)
{
	while (length != 0)
	{
		--length;
		if ( ! USB_Send_Byte(*src++))
			break;
	}
}

void USB_Send(const char * src)
{
	while (*src != 0)
		if ( ! USB_Send_Byte(*src++))
			break;
}

/******************************************************************************
 * @brief	Send the data to USART
 */
bool USART_Send_Byte(uint8_t data)
{
	uint32_t started = millis();
	while ( ! RingBufferPut(&USART_Tx, data))
	{
		EnterSleepMode();
		if ((millis() - started) > 20)
			return false;
	}
	return true;
}

void USART_Send_Data(const char * src, uint16_t length)
{
	while (length != 0)
	{
		--length;
		if ( ! USART_Send_Byte(*src++))
			break;
	}
}

void USART_Send(const char * src)
{
	while (*src != 0)
		if ( ! USART_Send_Byte(*src++))
			break;
}

/******************************************************************************
 * @brief	Get data from USB
 */
bool USB_Get_Data(uint8_t * dst)
{
	return RingBufferGet(&USB_Rx, dst);
}

/******************************************************************************
 * @brief	Get data from USART
 */
bool USART_Get_Data(uint8_t * dst)
{
	return RingBufferGet(&USART_Rx, dst);
}

/******************************************************************************
 * @brief	Send data to USB.
 */
void Handle_USBAsynchXfer (void)
{
	uint16_t USB_Tx_ptr;
	uint16_t USB_Tx_length;
	RingBuffer_t * ring = &USB_Tx;
	
	if (pInformation->USB_Tx_Active == false && ring->OutIndex != ring->InIndex)
	{
		if (ring->OutIndex > ring->InIndex)	/* rollback */
			ring->Length = ring->Size - ring->OutIndex;
		else
			ring->Length = ring->InIndex - ring->OutIndex;

		if (ring->Length > VIRTUAL_COM_PORT_DATA_SIZE)
		{
			USB_Tx_ptr = ring->OutIndex;
			USB_Tx_length = VIRTUAL_COM_PORT_DATA_SIZE;

			ring->OutIndex += VIRTUAL_COM_PORT_DATA_SIZE;
			ring->Length -= VIRTUAL_COM_PORT_DATA_SIZE;
		}
		else
		{
			USB_Tx_ptr = ring->OutIndex;
			USB_Tx_length = ring->Length;

			ring->OutIndex += ring->Length;
			ring->Length = 0;
		}

		if (ring->OutIndex == ring->Size)
			ring->OutIndex = 0;
		
		pInformation->USB_Tx_Active = true;
		UserToPMABufferCopy(&ring->Buffer[USB_Tx_ptr], ENDP1_TXADDR, USB_Tx_length);
		SetEPTxCount(ENDP1, USB_Tx_length);
		SetEPTxValid(ENDP1);
	}
}

/******************************************************************************
 * @brief	Convert Hex 32Bits value into char.
 */
static void IntToUnicode (uint32_t value, uint8_t *dst, uint8_t len)
{
	uint8_t ch;

	while (len != 0)
	{
		--len;
		ch = (uint8_t)(value >> 28);
		if (ch < 0xA )
			ch += '0';
		else
			ch += ('A' - 10) ;
		*dst++ = ch;
		*dst++ = 0;
		value = value << 4;
	}
}

/******************************************************************************
 * @brief	Create the serial number string descriptor.
 */
void Get_SerialNum(void)
{
	uint32_t Device_Serial0, Device_Serial1;

	Device_Serial0 = *(uint32_t *)ID1 + *(uint32_t *)ID3;
	Device_Serial1 = *(uint32_t *)ID2;

	if (Device_Serial0 != 0)
	{
		IntToUnicode (Device_Serial0, &CDC_StringSerial[2] , 8);
		IntToUnicode (Device_Serial1, &CDC_StringSerial[18], 4);
	}
}
