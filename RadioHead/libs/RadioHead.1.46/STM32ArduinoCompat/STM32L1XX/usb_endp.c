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
#include "usb_istr.h"
#include "usb_pwr.h"

uint32_t millis(void);

/* Interval between sending IN packets in frame number (1 frame = 1ms) */
#define CDC_IN_FRAME_INTERVAL	2

typedef struct RingBuffer_s {
	uint8_t * Buffer;
	uint16_t Size;
	uint16_t InIndex;
	uint16_t OutIndex;
	uint16_t Length;
} RingBuffer_t;

#define USB_RX_DATA_SIZE	64
#define USB_TX_DATA_SIZE	256

uint8_t	USB_Rx_Buffer [USB_RX_DATA_SIZE];
uint8_t	USB_Tx_Buffer [USB_TX_DATA_SIZE];
RingBuffer_t USB_Rx		= {	USB_Rx_Buffer, sizeof(USB_Rx_Buffer), 0, 0, 0	};
RingBuffer_t USB_Tx		= {	USB_Tx_Buffer, sizeof(USB_Tx_Buffer), 0, 0, 0	};

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

void EnterSleepMode(void)
{
#if defined( STM32L1XX_MD )
	PWR_EnterSleepMode(PWR_Regulator_ON, PWR_SLEEPEntry_WFI);
#endif
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

