/**
  ******************************************************************************
  * @file    usb_sil.c
  * @author  MCD Application Team
  * @version V4.0.0
  * @date    28-August-2012
  * @brief   Simplified Interface Layer for Global Initialization and Endpoint
  *          Rea/Write operations.
  ******************************************************************************
  * @attention
  *
  * <h2><center>&copy; COPYRIGHT 2012 STMicroelectronics</center></h2>
  *
  * Licensed under MCD-ST Liberty SW License Agreement V2, (the "License");
  * You may not use this file except in compliance with the License.
  * You may obtain a copy of the License at:
  *
  *        http://www.st.com/software_license_agreement_liberty_v2
  *
  * Unless required by applicable law or agreed to in writing, software 
  * distributed under the License is distributed on an "AS IS" BASIS, 
  * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  * See the License for the specific language governing permissions and
  * limitations under the License.
  *
  ******************************************************************************
  */


/* Includes ------------------------------------------------------------------*/
#include "usb_lib.h"

/* Private typedef -----------------------------------------------------------*/
/* Private define ------------------------------------------------------------*/
/* Private macro -------------------------------------------------------------*/
/* Private variables ---------------------------------------------------------*/
/* Extern variables ----------------------------------------------------------*/
/* Private function prototypes -----------------------------------------------*/
/* Private functions ---------------------------------------------------------*/

/*******************************************************************************
* Function Name  : USB_SIL_Init
* Description    : Initialize the USB Device IP and the Endpoint 0.
* Input          : None.
* Output         : None.
* Return         : Status.
*******************************************************************************/
uint32_t USB_SIL_Init(void)
{
	/* USB interrupts initialization */
	/* clear pending interrupts */
	_SetISTR(0);
	wInterrupt_Mask = IMR_MSK;
	/* set interrupts mask */
	_SetCNTR(wInterrupt_Mask);
	return 0;
}

/******************************************************************************
 * @brief	Write a buffer of data to a selected endpoint.
 * @param	bEpAddr: The address of the non control endpoint.
 * @param	src: The pointer to the buffer of data to be written to the endpoint.
 * @param	length: Number of data to be written (in bytes).
 * @retval	0
 */
uint32_t USB_SIL_Write(uint8_t bEpAddr, uint8_t * src, uint32_t length)
{
	/* Use the memory interface function to write to the selected endpoint */
	UserToPMABufferCopy(src, GetEPTxAddr(bEpAddr & 0x7F), length);

	/* Update the data length in the control register */
	SetEPTxCount((bEpAddr & 0x7F), length);

	return 0;
}

/******************************************************************************
 * @brief	Write a buffer of data to a selected endpoint.
 * @param	bEpAddr: The address of the non control endpoint.
 * @param	dst: The pointer to which will be saved the received data buffer.
 * @retval	Number of received data (in Bytes).
 */
uint32_t USB_SIL_Read(uint8_t bEpAddr, uint8_t* dst)
{
	uint32_t length = 0;
	/* Get the number of received data on the selected Endpoint */
	length = GetEPRxCount(bEpAddr & 0x7F);

	/* Use the memory interface function to write to the selected endpoint */
	PMAToUserBufferCopy(dst, GetEPRxAddr(bEpAddr & 0x7F), length);

	/* Return the number of received data */
	return length;
}
