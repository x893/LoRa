#ifndef __RFM95W_BB_H__
#define __RFM95W_BB_H__

#ifdef __cplusplus
 extern "C" {
#endif

#include "stm32l1xx.h"

typedef enum 
{
	LED1 = 0,
	LED2 = 1,
	LED3 = 2
} Led_TypeDef;

typedef enum 
{
	COM1 = 0
} COM_TypeDef;	

/** 
	* @brief	Define for RFM95W_BB board	
	*/ 
#if !defined (USE_RFM95W_BB)
	#define USE_RFM95W_BB
#endif

/** @addtogroup RFM95W_BB_LOW_LEVEL_LED
	* @{
	*/
#define LEDn				3

#define LED1_PIN			GPIO_Pin_11
#define LED1_GPIO_PORT		GPIOB
#define LED1_GPIO_CLK		RCC_AHBPeriph_GPIOB
	
#define LED2_PIN			GPIO_Pin_10
#define LED2_GPIO_PORT		GPIOB
#define LED2_GPIO_CLK		RCC_AHBPeriph_GPIOB
	
#define LED3_PIN			GPIO_Pin_3
#define LED3_GPIO_PORT		GPIOA
#define LED3_GPIO_CLK		RCC_AHBPeriph_GPIOA
	
/**
 * @brief Definition for COM port1, connected to USART1
 */ 
#define EVAL_COM1					USART1
#define RF95W_COM_CLK				RCC_APB2Periph_USART1
#define RF95W_COM_PIN_AF			GPIO_AF_USART1
#define USART_IRQHandler			USART1_IRQHandler

#define RF95W_COM_TX_PIN			GPIO_Pin_9
#define RF95W_COM_TX_GPIO_PORT		GPIOA
#define RF95W_COM_TX_GPIO_CLK		RCC_AHBPeriph_GPIOA
#define RF95W_COM_TX_SOURCE			GPIO_PinSource9

#define RF95W_COM_RX_PIN			GPIO_Pin_10
#define RF95W_COM_RX_GPIO_PORT		GPIOA
#define RF95W_COM_RX_GPIO_CLK		RCC_AHBPeriph_GPIOA
#define RF95W_COM_RX_SOURCE			GPIO_PinSource10

#define RF95W_COM_IRQn				USART1_IRQn

/**
  * @brief	RFM95W SPI Interface
  */	
#define RF95W_SPI					SPI1
#define RF95W_SPI_CLK				RCC_APB2Periph_SPI1
#define RF95W_PIN_AF				GPIO_AF_SPI1

#define RF95W_SCK_PIN				GPIO_Pin_3				/* PB.3 */
#define RF95W_SCK_GPIO_PORT			GPIOB
#define RF95W_SCK_GPIO_CLK			RCC_AHBPeriph_GPIOB
#define RF95W_SCK_SOURCE			GPIO_PinSource3

#define RF95W_MISO_PIN				GPIO_Pin_4				/* PB.4 */
#define RF95W_MISO_GPIO_PORT		GPIOB
#define RF95W_MISO_GPIO_CLK			RCC_AHBPeriph_GPIOB
#define RF95W_MISO_SOURCE			GPIO_PinSource4

#define RF95W_MOSI_PIN				GPIO_Pin_5				/* PB.5 */
#define RF95W_MOSI_GPIO_PORT		GPIOB
#define RF95W_MOSI_GPIO_CLK			RCC_AHBPeriph_GPIOB
#define RF95W_MOSI_SOURCE			GPIO_PinSource5

#define RF95W_CS_PIN				GPIO_Pin_15				/* PA.15 */
#define RF95W_CS_GPIO_PORT			GPIOA
#define RF95W_CS_GPIO_CLK			RCC_AHBPeriph_GPIOA

#define STM32L15_USB_CONNECT		SYSCFG_USBPuCmd(ENABLE)
#define STM32L15_USB_DISCONNECT		SYSCFG_USBPuCmd(DISABLE)

#define RF95W_DIO0_PIN				GPIO_Pin_6				/* PB.06 */
#define RF95W_DIO0_GPIO_PORT		GPIOB
#define RF95W_DIO0_GPIO_CLK			RCC_AHBPeriph_GPIOB
#define RF95W_DIO0_EXTI_LINE		EXTI_Line6
#define RF95W_DIO0_EXTI_PIN_SOURCE	EXTI_PinSource6
#define RF95W_DIO0_EXTI_PORT_SOURCE	EXTI_PortSourceGPIOB
#define RF95W_DIO0_EXTI_IRQn		EXTI9_5_IRQn 
	
void STM_EVAL_LEDInit(Led_TypeDef Led);
void STM_EVAL_LEDOn(Led_TypeDef Led);
void STM_EVAL_LEDOff(Led_TypeDef Led);
void STM_EVAL_LEDToggle(Led_TypeDef Led);

void STM_EVAL_COMInit(COM_TypeDef COM, USART_InitTypeDef* USART_InitStruct); 

void RF95W_LowLevel_DeInit(void);
void RF95W_LowLevel_Init(void); 
	
#ifdef __cplusplus
}
#endif

#endif
