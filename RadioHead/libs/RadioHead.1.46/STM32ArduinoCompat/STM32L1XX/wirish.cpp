// ArduinoCompat/wirish.cpp
//
// Arduino-like API for STM32F4 Discovery and similar
// using STM32F4xx_DSP_StdPeriph_Lib_V1.3.0

#include <wirish.h>

// Describes all the STM32 things we need to know about a digital IO pin to
// make it input or output or to configure as an interrupt
typedef struct
{
	uint32_t		ahbperiph;
	GPIO_TypeDef*	port;
	uint16_t		pin;
	uint8_t			extiPortSource;
	uint8_t			extiPinSource;
} GPIOPin;

// These describe the registers and bits for each digital IO pin to allow us to 
// provide Arduino-like pin addressing, digitalRead etc.
// Indexed by pin number
const GPIOPin pins[] = 
{
	{ RCC_AHBPeriph_GPIOA, GPIOA, GPIO_Pin_0,	EXTI_PortSourceGPIOA, EXTI_PinSource0	}, // 0 = PA0
	{ RCC_AHBPeriph_GPIOA, GPIOA, GPIO_Pin_1,	EXTI_PortSourceGPIOA, EXTI_PinSource1	},
	{ RCC_AHBPeriph_GPIOA, GPIOA, GPIO_Pin_2,	EXTI_PortSourceGPIOA, EXTI_PinSource2	},
	{ RCC_AHBPeriph_GPIOA, GPIOA, GPIO_Pin_3,	EXTI_PortSourceGPIOA, EXTI_PinSource3	},
	{ RCC_AHBPeriph_GPIOA, GPIOA, GPIO_Pin_4,	EXTI_PortSourceGPIOA, EXTI_PinSource4	},
	{ RCC_AHBPeriph_GPIOA, GPIOA, GPIO_Pin_5,	EXTI_PortSourceGPIOA, EXTI_PinSource5	},
	{ RCC_AHBPeriph_GPIOA, GPIOA, GPIO_Pin_6,	EXTI_PortSourceGPIOA, EXTI_PinSource6	},
	{ RCC_AHBPeriph_GPIOA, GPIOA, GPIO_Pin_7,	EXTI_PortSourceGPIOA, EXTI_PinSource7	},
	{ RCC_AHBPeriph_GPIOA, GPIOA, GPIO_Pin_8,	EXTI_PortSourceGPIOA, EXTI_PinSource8	},
	{ RCC_AHBPeriph_GPIOA, GPIOA, GPIO_Pin_9,	EXTI_PortSourceGPIOA, EXTI_PinSource9	},
	{ RCC_AHBPeriph_GPIOA, GPIOA, GPIO_Pin_10,	EXTI_PortSourceGPIOA, EXTI_PinSource10	},
	{ RCC_AHBPeriph_GPIOA, GPIOA, GPIO_Pin_11,	EXTI_PortSourceGPIOA, EXTI_PinSource11	},
	{ RCC_AHBPeriph_GPIOA, GPIOA, GPIO_Pin_12,	EXTI_PortSourceGPIOA, EXTI_PinSource12	},
	{ RCC_AHBPeriph_GPIOA, GPIOA, GPIO_Pin_13,	EXTI_PortSourceGPIOA, EXTI_PinSource13	},
	{ RCC_AHBPeriph_GPIOA, GPIOA, GPIO_Pin_14,	EXTI_PortSourceGPIOA, EXTI_PinSource14	},
	{ RCC_AHBPeriph_GPIOA, GPIOA, GPIO_Pin_15,	EXTI_PortSourceGPIOA, EXTI_PinSource15	}, // 15 = PA15

	{ RCC_AHBPeriph_GPIOB, GPIOB, GPIO_Pin_0,	EXTI_PortSourceGPIOB, EXTI_PinSource0	}, // 16 = PB0
	{ RCC_AHBPeriph_GPIOB, GPIOB, GPIO_Pin_1,	EXTI_PortSourceGPIOB, EXTI_PinSource1	},
	{ RCC_AHBPeriph_GPIOB, GPIOB, GPIO_Pin_2,	EXTI_PortSourceGPIOB, EXTI_PinSource2	},
	{ RCC_AHBPeriph_GPIOB, GPIOB, GPIO_Pin_3,	EXTI_PortSourceGPIOB, EXTI_PinSource3	},
	{ RCC_AHBPeriph_GPIOB, GPIOB, GPIO_Pin_4,	EXTI_PortSourceGPIOB, EXTI_PinSource4	},
	{ RCC_AHBPeriph_GPIOB, GPIOB, GPIO_Pin_5,	EXTI_PortSourceGPIOB, EXTI_PinSource5	},
	{ RCC_AHBPeriph_GPIOB, GPIOB, GPIO_Pin_6,	EXTI_PortSourceGPIOB, EXTI_PinSource6	},
	{ RCC_AHBPeriph_GPIOB, GPIOB, GPIO_Pin_7,	EXTI_PortSourceGPIOB, EXTI_PinSource7	},
	{ RCC_AHBPeriph_GPIOB, GPIOB, GPIO_Pin_8,	EXTI_PortSourceGPIOB, EXTI_PinSource8	},
	{ RCC_AHBPeriph_GPIOB, GPIOB, GPIO_Pin_9,	EXTI_PortSourceGPIOB, EXTI_PinSource9	},
	{ RCC_AHBPeriph_GPIOB, GPIOB, GPIO_Pin_10,	EXTI_PortSourceGPIOB, EXTI_PinSource10	},
	{ RCC_AHBPeriph_GPIOB, GPIOB, GPIO_Pin_11,	EXTI_PortSourceGPIOB, EXTI_PinSource11	},
	{ RCC_AHBPeriph_GPIOB, GPIOB, GPIO_Pin_12,	EXTI_PortSourceGPIOB, EXTI_PinSource12	},
	{ RCC_AHBPeriph_GPIOB, GPIOB, GPIO_Pin_13,	EXTI_PortSourceGPIOB, EXTI_PinSource13	},
	{ RCC_AHBPeriph_GPIOB, GPIOB, GPIO_Pin_14,	EXTI_PortSourceGPIOB, EXTI_PinSource14	},
	{ RCC_AHBPeriph_GPIOB, GPIOB, GPIO_Pin_15,	EXTI_PortSourceGPIOB, EXTI_PinSource15	}, // 31 = PB15

	{ RCC_AHBPeriph_GPIOC, GPIOC, GPIO_Pin_0,	EXTI_PortSourceGPIOC, EXTI_PinSource0	}, // 32 = PC0
	{ RCC_AHBPeriph_GPIOC, GPIOC, GPIO_Pin_1,	EXTI_PortSourceGPIOC, EXTI_PinSource1	},
	{ RCC_AHBPeriph_GPIOC, GPIOC, GPIO_Pin_2,	EXTI_PortSourceGPIOC, EXTI_PinSource2	},
	{ RCC_AHBPeriph_GPIOC, GPIOC, GPIO_Pin_3,	EXTI_PortSourceGPIOC, EXTI_PinSource3	},
	{ RCC_AHBPeriph_GPIOC, GPIOC, GPIO_Pin_4,	EXTI_PortSourceGPIOC, EXTI_PinSource4	},
	{ RCC_AHBPeriph_GPIOC, GPIOC, GPIO_Pin_5,	EXTI_PortSourceGPIOC, EXTI_PinSource5	},
	{ RCC_AHBPeriph_GPIOC, GPIOC, GPIO_Pin_6,	EXTI_PortSourceGPIOC, EXTI_PinSource6	},
	{ RCC_AHBPeriph_GPIOC, GPIOC, GPIO_Pin_7,	EXTI_PortSourceGPIOC, EXTI_PinSource7	},
	{ RCC_AHBPeriph_GPIOC, GPIOC, GPIO_Pin_8,	EXTI_PortSourceGPIOC, EXTI_PinSource8	},
	{ RCC_AHBPeriph_GPIOC, GPIOC, GPIO_Pin_9,	EXTI_PortSourceGPIOC, EXTI_PinSource9	},
	{ RCC_AHBPeriph_GPIOC, GPIOC, GPIO_Pin_10,	EXTI_PortSourceGPIOC, EXTI_PinSource10	},
	{ RCC_AHBPeriph_GPIOC, GPIOC, GPIO_Pin_11,	EXTI_PortSourceGPIOC, EXTI_PinSource11	},
	{ RCC_AHBPeriph_GPIOC, GPIOC, GPIO_Pin_12,	EXTI_PortSourceGPIOC, EXTI_PinSource12	},
	{ RCC_AHBPeriph_GPIOC, GPIOC, GPIO_Pin_13,	EXTI_PortSourceGPIOC, EXTI_PinSource13	},
	{ RCC_AHBPeriph_GPIOC, GPIOC, GPIO_Pin_14,	EXTI_PortSourceGPIOC, EXTI_PinSource14	},
	{ RCC_AHBPeriph_GPIOC, GPIOC, GPIO_Pin_15,	EXTI_PortSourceGPIOC, EXTI_PinSource15	}	// 47 = PC15
};
#define NUM_PINS (sizeof(pins) / sizeof(GPIOPin))

typedef struct
{
	uint32_t	extiLine;
	uint8_t		extiIrqn;
} IRQLine;

typedef void (*IRQHandler)(void);

// IRQ line data indexed by pin source number with its port
// and the programmable handler that will handle interrupts on that line
const IRQLine irqlines[] =
{
	{ EXTI_Line0,	EXTI0_IRQn		},
	{ EXTI_Line1,	EXTI1_IRQn		},
	{ EXTI_Line2,	EXTI2_IRQn		},
	{ EXTI_Line3,	EXTI3_IRQn		},
	{ EXTI_Line4,	EXTI4_IRQn		},
	{ EXTI_Line5,	EXTI9_5_IRQn	},
	{ EXTI_Line6,	EXTI9_5_IRQn	},
	{ EXTI_Line7,	EXTI9_5_IRQn	},
	{ EXTI_Line8,	EXTI9_5_IRQn	},
	{ EXTI_Line9,	EXTI9_5_IRQn	},
	{ EXTI_Line10, EXTI15_10_IRQn	},
	{ EXTI_Line11, EXTI15_10_IRQn	},
	{ EXTI_Line12, EXTI15_10_IRQn	},
	{ EXTI_Line13, EXTI15_10_IRQn	},
	{ EXTI_Line14, EXTI15_10_IRQn	},
	{ EXTI_Line15, EXTI15_10_IRQn	}
};
#define NUM_IRQ_LINES (sizeof(irqlines) / sizeof(IRQLine))

IRQHandler irqHandlers[NUM_IRQ_LINES];

// Functions we expect to find in the sketch
extern void setup();
extern void loop();

volatile uint32_t disable_irq_count = 0;

void SysTickConfig()
{
	/* Setup SysTick Timer for 1ms interrupts	*/
	if (SysTick_Config(SystemCoreClock / 1000))
	{	/* Capture error */
		while (1);
	}

	/* Configure the SysTick handler priority */
	NVIC_SetPriority(SysTick_IRQn, 0x0);
}

// These interrupt handlers have to be extern C else they dont get linked in to the interrupt vectors
extern "C"
{
	// Interrupt handlers for optional external GPIO interrupts
	static void EXTIx_IRQHandler(uint8_t irq)
	{
		if (EXTI_GetITStatus(irqlines[irq].extiLine) != RESET)
		{
			if (irqHandlers[irq])
				irqHandlers[irq]();
			EXTI_ClearITPendingBit(irqlines[irq].extiLine);
		}
	}

	void EXTI0_IRQHandler(void)
	{
		EXTIx_IRQHandler(0);
	}
	void EXTI1_IRQHandler(void)
	{
		EXTIx_IRQHandler(1);
	}
	void EXTI2_IRQHandler(void)
	{
		EXTIx_IRQHandler(2);
	}
	void EXTI3_IRQHandler(void)
	{
		EXTIx_IRQHandler(3);
	}
	void EXTI4_IRQHandler(void)
	{
		EXTIx_IRQHandler(4);
	}
	void EXTI9_5_IRQHandler(void)
	{
		EXTIx_IRQHandler(5);
		EXTIx_IRQHandler(6);
		EXTIx_IRQHandler(7);
		EXTIx_IRQHandler(8);
		EXTIx_IRQHandler(9);
	}
	void EXTI15_10_IRQHandler(void)
	{
		EXTIx_IRQHandler(10);
		EXTIx_IRQHandler(11);
		EXTIx_IRQHandler(12);
		EXTIx_IRQHandler(13);
		EXTIx_IRQHandler(14);
		EXTIx_IRQHandler(15);
	}
}	/*	extern "C"	*/

// Run the Arduino standard functions in the main loop
int main(int argc, char** argv)
{
	SystemCoreClockUpdate();

	DBGMCU_Config(DBGMCU_SLEEP | DBGMCU_STOP | DBGMCU_STANDBY, ENABLE);
	DBGMCU_APB1PeriphConfig(DBGMCU_IWDG_STOP | DBGMCU_RTC_STOP, ENABLE);

	SysTickConfig();
	//	Seed the random number generator
	//	srand(getpid() ^ (unsigned) time(NULL)/2);
	setup();
	while (1)
		loop();
}

void pinMode(uint8_t pin, WiringPinMode mode)
{
	if (pin > NUM_PINS)
		return;

	// Enable the GPIO clock
	RCC_AHBPeriphClockCmd(pins[pin].ahbperiph, ENABLE);
	GPIO_InitTypeDef GPIO_InitStructure;
	GPIO_InitStructure.GPIO_Pin = pins[pin].pin;
	GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL;
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN;
/*
	if (mode == INPUT || mode == INPUT_FLOATING)
	{
		GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN;
	}
	else
*/
	if (mode == INPUT_ANALOG)
		GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AN;
	else if (mode == INPUT_PULLUP)
		GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP;
	else if (mode == INPUT_PULLDOWN)
		GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_DOWN;
	else if (mode == OUTPUT)
		GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT;
	else if (mode == OUTPUT_OPEN_DRAIN)
	{
		GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT;
		GPIO_InitStructure.GPIO_OType = GPIO_OType_OD;
	}
	else
		return; // Unknown so far

	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_40MHz;
	GPIO_Init(pins[pin].port, &GPIO_InitStructure);
}

// This takes about 150ns on STM32F4 Discovery
void digitalWrite(uint8_t pin, uint8_t val)
{
	if (pin > NUM_PINS)
		return;
	if (val)
		GPIO_SetBits(pins[pin].port, pins[pin].pin);
	else
		GPIO_ResetBits(pins[pin].port, pins[pin].pin);
}

uint8_t digitalRead(uint8_t pin)
{
	if (pin > NUM_PINS)
		return 0;
	return GPIO_ReadInputDataBit(pins[pin].port, pins[pin].pin);
}

void attachInterrupt(uint8_t pin, void (*handler)(void), int mode)
{
	EXTI_InitTypeDef EXTI_InitStructure;
	NVIC_InitTypeDef NVIC_InitStructure;
	uint8_t extiPinSource = pins[pin].extiPinSource;

	RCC_APB2PeriphClockCmd(RCC_APB2Periph_SYSCFG, ENABLE);

	// Record the handler to call when the interrupt occurs
	irqHandlers[extiPinSource] = handler;

	/* Connect EXTI Line to GPIO Pin */
	SYSCFG_EXTILineConfig(pins[pin].extiPortSource, extiPinSource);

	/* Configure EXTI line */
	EXTI_InitStructure.EXTI_Line = irqlines[extiPinSource].extiLine;
	EXTI_InitStructure.EXTI_Mode = EXTI_Mode_Interrupt;

	if (mode == RISING)
		EXTI_InitStructure.EXTI_Trigger = EXTI_Trigger_Rising;	
	else if (mode == FALLING)
		EXTI_InitStructure.EXTI_Trigger = EXTI_Trigger_Falling;	
	else if (mode == CHANGE)
		EXTI_InitStructure.EXTI_Trigger = EXTI_Trigger_Rising_Falling;

	EXTI_InitStructure.EXTI_LineCmd = ENABLE;
	EXTI_Init(&EXTI_InitStructure);

	/* Enable and set EXTI Interrupt to the lowest priority */
	NVIC_InitStructure.NVIC_IRQChannel = irqlines[extiPinSource].extiIrqn;
	NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 0x0F;
	NVIC_InitStructure.NVIC_IRQChannelSubPriority = 0x0F;
	NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;

	NVIC_Init(&NVIC_InitStructure); 

	// The relevant EXTI?_IRQHandler
	// will now be called when the pin makes the selected transition
}

void delay(uint32_t ms)
{
	uint32_t start = millis();
	while ((millis() - start) < ms)
		;
}

int32_t random(int32_t from, int32_t to)
{
//!	return from + (RNG_GetRandomNumber() % (to - from));
	return (from + to) / 2;
}

int32_t random(int32_t to)
{
	return random(0, to);
}

extern "C"
{
	// These need to be in C land for correct linking
	void _init() {}
	void _fini() {}
}
