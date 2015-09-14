/**
 ******************************************************************************
 * @file    stm32_it.c
 * @author  MCD Application Team
 * @version V4.0.0
 * @date    21-January-2013
 * @brief   Main Interrupt Service Routines.
 *          This file provides template for all exceptions handler and peripherals
 *          interrupt service routine.
 ******************************************************************************
 */

#include "hw_config.h"
#include "usb_lib.h"
#include "usb_istr.h"

/******************************************************************************
 * @brief	This function handles USB Low Priority interrupts requests.
 */
#if defined(STM32L1XX_MD) || defined(STM32L1XX_HD)|| defined(STM32L1XX_MD_PLUS)|| defined (STM32F37X)
void USB_LP_IRQHandler(void)
#else
void USB_LP_CAN1_RX0_IRQHandler(void)
#endif
{
	USB_Istr();
}

/******************************************************************************
 * @brief	This function handles EVAL_COM1 global interrupt request.
 */
void USART_IRQHandler(void)
{
	if (USART_GetITStatus(EVAL_COM1, USART_IT_RXNE) != RESET)
		USART_Receive_Data();

	/* If overrun condition occurs, clear the ORE flag and recover communication */
	if (USART_GetFlagStatus(EVAL_COM1, USART_FLAG_ORE) != RESET)
		(void)USART_ReceiveData(EVAL_COM1);
}

/******************************************************************************
 * @brief	This function handles USB WakeUp interrupt request.
 */

#if defined(STM32L1XX_MD) || defined(STM32L1XX_HD)|| defined(STM32L1XX_MD_PLUS)
void USB_FS_WKUP_IRQHandler(void)
#else
void USBWakeUp_IRQHandler(void)
#endif
{
	EXTI_ClearITPendingBit(EXTI_Line18);
}

/******************************************************************************
 * @brief	This function handles NMI exception.
 */
void NMI_Handler(void)
{
}

/******************************************************************************
 * @brief	This function handles Hard Fault exception.
 */
void HardFault_Handler(void)
{
	__disable_irq();
	STM_EVAL_LEDOn(LED3);
	/* Go to infinite loop when Hard Fault exception occurs */
	while (1)
	{ }
}

/******************************************************************************
 * @brief	This function handles Memory Manage exception.
 */
void MemManage_Handler(void) __attribute__((alias("HardFault_Handler")));

/******************************************************************************
 * @brief	This function handles Bus Fault exception.
 */
void BusFault_Handler(void) __attribute__((alias("HardFault_Handler")));

/******************************************************************************
 * @brief	This function handles Usage Fault exception.
 */
void UsageFault_Handler(void) __attribute__((alias("HardFault_Handler")));

/******************************************************************************
 * @brief	This function handles SVCall exception.
 */
void SVC_Handler(void)
{
}

/******************************************************************************
 * @brief	This function handles Debug Monitor exception.
 */
void DebugMon_Handler(void)
{
}

/******************************************************************************
 * @brief	This function handles PendSVC exception.
 */
void PendSV_Handler(void)
{
}

uint32_t sysTickCount = 0;
/******************************************************************************
 * @brief	This function handles SysTick Handler.
 */
void SysTick_Handler(void)
{
	sysTickCount++;
}

uint32_t millis(void)
{
	return sysTickCount;
}
