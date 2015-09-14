/**
 ******************************************************************************
 * @file	usb_prop.c
 * @author	MCD Application Team
 * @version V4.0.0
 * @date	21-January-2013
 * @brief	All processing related to Virtual Com Port Demo
 ******************************************************************************
 */
#include "usb_lib.h"
#include "usb_conf.h"
#include "usb_prop.h"
#include "usb_desc.h"
#include "usb_pwr.h"
#include "hw_config.h"

LINE_CODING linecoding = {
	115200, /* baud rate*/
	0x00,	/* stop bits-1*/
	0x00,	/* parity - none*/
	0x08	/* no. of bits 8*/
	};

DEVICE Device_Table = {
	EP_NUM,
	1
};

DEVICE_PROP Device_Property = {
	64, /* MAX PACKET SIZE */
	CDC_Init,
	CDC_Reset,
	CDC_Status_In,
	CDC_Status_Out,
	CDC_Data_Setup,
	CDC_NoData_Setup,
	CDC_Get_Interface_Setting,
	CDC_GetDeviceDescriptor,
	CDC_GetConfigDescriptor,
	CDC_GetStringDescriptor,
	NULL
};

USER_STANDARD_REQUESTS User_Standard_Requests = {
	CDC_GetConfiguration,
	CDC_SetConfiguration,
	CDC_GetInterface,
	CDC_SetInterface,
	CDC_GetStatus,
	CDC_ClearFeature,
	CDC_SetEndPointFeature,
	CDC_SetDeviceFeature,
	CDC_SetDeviceAddress
};

ONE_DESCRIPTOR Device_Descriptor = {
	(uint8_t *)CDC_DeviceDescriptor,
	VIRTUAL_COM_PORT_SIZ_DEVICE_DESC
};

ONE_DESCRIPTOR Config_Descriptor = {
	(uint8_t *)CDC_ConfigDescriptor,
	VIRTUAL_COM_PORT_SIZ_CONFIG_DESC
};

ONE_DESCRIPTOR String_Descriptor[] = {
	{ (uint8_t *)CDC_StringLangID,	VIRTUAL_COM_PORT_SIZ_STRING_LANGID	},
	{ (uint8_t *)CDC_StringVendor,	VIRTUAL_COM_PORT_SIZ_STRING_VENDOR	},
	{ (uint8_t *)CDC_StringProduct,VIRTUAL_COM_PORT_SIZ_STRING_PRODUCT	},
	{ (uint8_t *)CDC_StringSerial,	VIRTUAL_COM_PORT_SIZ_STRING_SERIAL	}
};

/******************************************************************************
 * @brief	CDC init routine.
 */
void CDC_Init(void)
{
	Get_SerialNum();		/* Update the serial number string descriptor with the data from the unique ID */
	pInformation->Current_Configuration = 0;

	USB_Config();

	PowerOn();				/* Connect the device */
	USB_SIL_Init();			/* Perform basic device initialization operations */

	pInformation->bDeviceState = UNCONNECTED;
}

/******************************************************************************
 * @brief	CDC reset routine
 */
void CDC_Reset(void)
{
	/* Set CDC DEVICE as not configured */
	pInformation->Current_Configuration = 0;

	/* Current Feature initialization */
	pInformation->Current_Feature = CDC_ConfigDescriptor[7];

	/* Set CDC DEVICE with the default Interface */
	pInformation->Current_Interface = 0;

	SetBTABLE(BTABLE_ADDRESS);

	/* Initialize Endpoint 0 */
	SetEPType(ENDP0, EP_CONTROL);
	SetEPTxStatus(ENDP0, EP_TX_STALL);
	SetEPRxAddr(ENDP0, ENDP0_RXADDR);
	SetEPTxAddr(ENDP0, ENDP0_TXADDR);
	Clear_Status_Out(ENDP0);
	SetEPRxCount(ENDP0, Device_Property.MaxPacketSize);
	SetEPRxValid(ENDP0);

	/* Initialize Endpoint 1 */
	SetEPType(ENDP1, EP_BULK);
	SetEPTxAddr(ENDP1, ENDP1_TXADDR);
	SetEPTxStatus(ENDP1, EP_TX_NAK);
	SetEPRxStatus(ENDP1, EP_RX_DIS);

	/* Initialize Endpoint 2 */
	SetEPType(ENDP2, EP_INTERRUPT);
	SetEPTxAddr(ENDP2, ENDP2_TXADDR);
	SetEPRxStatus(ENDP2, EP_RX_DIS);
	SetEPTxStatus(ENDP2, EP_TX_NAK);

	/* Initialize Endpoint 3 */
	SetEPType(ENDP3, EP_BULK);
	SetEPRxAddr(ENDP3, ENDP3_RXADDR);
	SetEPRxCount(ENDP3, VIRTUAL_COM_PORT_DATA_SIZE);
	SetEPRxStatus(ENDP3, EP_RX_VALID);
	SetEPTxStatus(ENDP3, EP_TX_DIS);

	/* Set this device to response on default address */
	SetDeviceAddress(0);

	pInformation->bDeviceState = ATTACHED;
}

/******************************************************************************
 * @brief	Update the device state to configured.
 */
void CDC_SetConfiguration(void)
{
	DEVICE_INFO *pInfo = &UsbDeviceInfo;

	if (pInfo->Current_Configuration != 0)
	{	/* Device configured */
		pInformation->bDeviceState = CONFIGURED;
	}
}

/******************************************************************************
 * @brief	Update the device state to addressed.
 */
void CDC_SetDeviceAddress (void)
{
	pInformation->bDeviceState = ADDRESSED;
}

/******************************************************************************
 * @brief	CDC Status In Routine.
 */
void CDC_Status_In(void)
{
}

/******************************************************************************
 * @brief	CDC Status OUT Routine.
 */
void CDC_Status_Out(void)
{
}

/******************************************************************************
 * @brief	Handle the data class specific requests
 * @param	Input			: Request Nb.
 * @retval	USB_UNSUPPORT or USB_SUCCESS.
 */
USB_RESULT CDC_Data_Setup(uint8_t RequestNo)
{
	uint8_t	*(*CopyRoutine)(uint16_t);

	CopyRoutine = NULL;

	if (RequestNo == GET_LINE_CODING)
	{
		if (Type_Recipient == (CLASS_REQUEST | INTERFACE_RECIPIENT))
			CopyRoutine = CDC_GetLineCoding;
	}
	else if (RequestNo == SET_LINE_CODING)
	{
		if (Type_Recipient == (CLASS_REQUEST | INTERFACE_RECIPIENT))
			CopyRoutine = CDC_SetLineCoding;
	}

	if (CopyRoutine == NULL)
		return USB_UNSUPPORT;

	pInformation->Ctrl_Info.CopyData = CopyRoutine;
	pInformation->Ctrl_Info.Usb_wOffset = 0;
	(*CopyRoutine)(0);
	return USB_SUCCESS;
}

/******************************************************************************
 * @brief	Handle the no data class specific requests.
 * @param	Input : Request Nb.
 * @retval	USB_UNSUPPORT or USB_SUCCESS.
 */
USB_RESULT CDC_NoData_Setup(uint8_t RequestNo)
{
	if (Type_Recipient == (CLASS_REQUEST | INTERFACE_RECIPIENT))
	{
		if (RequestNo == SET_COMM_FEATURE)
			return USB_SUCCESS;
		else if (RequestNo == SET_CONTROL_LINE_STATE)
			return USB_SUCCESS;
	}
	return USB_UNSUPPORT;
}

/******************************************************************************
 * @brief	Gets the device descriptor.
 * @param	Length
 * @retval	The address of the device descriptor.
 */
uint8_t * CDC_GetDeviceDescriptor(uint16_t length)
{
	return Standard_GetDescriptorData(length, &Device_Descriptor);
}

/******************************************************************************
 * @brief	Get the configuration descriptor.
 * @param	Input : Length.
 * @retval	The address of the configuration descriptor.
 */
uint8_t * CDC_GetConfigDescriptor(uint16_t length)
{
	return Standard_GetDescriptorData(length, &Config_Descriptor);
}

/******************************************************************************
 * @brief	Gets the string descriptors according to the needed index
 * @param	Input : Length.
 * @retval	The address of the string descriptors.
 */
uint8_t * CDC_GetStringDescriptor(uint16_t length)
{
	uint8_t wValue0 = pInformation->USBwValue0;
	if (wValue0 > 4)
		return NULL;
	return Standard_GetDescriptorData(length, &String_Descriptor[wValue0]);
}

/******************************************************************************
 * @brief	Test the interface and the alternate setting according to the
 *			supported one.
 * @param	Interface : interface number.
 * @param	AlternateSetting : Alternate Setting number.
 * @retval	The address of the string descriptors.
 */
USB_RESULT CDC_Get_Interface_Setting(uint8_t Interface, uint8_t AlternateSetting)
{
	if (AlternateSetting > 0 || Interface > 1)
		return USB_UNSUPPORT;
	return USB_SUCCESS;
}

/******************************************************************************
 * @brief	Send the linecoding structure to the PC host.
 * @param	Length.
 * @retval	Linecoding structure base address.
 */
uint8_t * CDC_GetLineCoding(uint16_t length)
{
	if (length == 0)
	{
		pInformation->Ctrl_Info.Usb_wLength = sizeof(linecoding);
		return NULL;
	}
	return (uint8_t *)&linecoding;
}

/******************************************************************************
 * @brief	Set the linecoding structure fields.
 * @param	Length.
 * @retval	Linecoding structure base address.
 */
uint8_t * CDC_SetLineCoding(uint16_t length)
{
	if (length == 0)
	{
		pInformation->Ctrl_Info.Usb_wLength = sizeof(linecoding);
		return NULL;
	}
	return (uint8_t *)&linecoding;
}