/**
 ******************************************************************************
 * @file    usb_endp.c
 * @author  MCD Application Team
 * @version V4.0.0
 * @date    21-January-2013
 * @brief   Endpoint routines
 ******************************************************************************
 */
#include "usb_lib.h"
#include "usb_desc.h"
#include "usb_mem.h"
#include "hw_config.h"
#include "usb_istr.h"
#include "usb_pwr.h"

/* Interval between sending IN packets in frame number (1 frame = 1ms) */
#define CDC_IN_FRAME_INTERVAL	2

extern RingBuffer_t USB_Tx;
uint8_t USB_Rx_Tmp_Buffer[VIRTUAL_COM_PORT_DATA_SIZE];

/******************************************************************************
 * @brief	EP1_IN_Callback
 */
void EP1_IN_Callback (void)
{
	uint16_t USB_Tx_ptr;
	uint16_t USB_Tx_length;
	RingBuffer_t * ring = &USB_Tx;

	if (pInformation->USB_Tx_Active)
	{
		if (ring->Length == 0)
			pInformation->USB_Tx_Active = false;
		else
		{
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
			UserToPMABufferCopy(&ring->Buffer[USB_Tx_ptr], ENDP1_TXADDR, USB_Tx_length);
			SetEPTxCount(ENDP1, USB_Tx_length);
			SetEPTxValid(ENDP1); 
		}
	}
}

/******************************************************************************
 * @brief	EP3_OUT_Callback
 */
void EP3_OUT_Callback(void)
{
	uint16_t USB_Rx_Cnt;

	/* Get the received data buffer and update the counter */
	USB_Rx_Cnt = USB_SIL_Read(EP3_OUT, USB_Rx_Tmp_Buffer);

	/*	USB data will be immediately processed,
		this allow next USB traffic being NAKed till the end of the USART Xfer
	*/
	USB_Receive_Data(USB_Rx_Tmp_Buffer, USB_Rx_Cnt);

	/* Enable the receive of data on EP3 */
	SetEPRxValid(ENDP3);
}


/******************************************************************************
 * @brief	SOF_Callback / INTR_SOFINTR_Callback
 */
void SOF_Callback(void)
{
	if (pInformation->bDeviceState == CONFIGURED)
	{
		if (pInformation->FrameCount++ == CDC_IN_FRAME_INTERVAL)
		{	/* Reset the frame counter */
			pInformation->FrameCount = 0;

			/* Check the data to be sent through IN pipe */
			Handle_USBAsynchXfer();
		}
	}
}

