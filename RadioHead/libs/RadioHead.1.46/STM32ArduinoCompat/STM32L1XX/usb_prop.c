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

#if defined ( STM32L1XX_MD )
	#define	MCU_ID1		(0x1FF80050)
	#define	MCU_ID2		(0x1FF80054)
	#define	MCU_ID3		(0x1FF80064)
#else
	#error "Unsupported MCU"
#endif

void Get_SerialNum(void);

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
	NULL,
	VIRTUAL_COM_PORT_DATA_SIZE
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

	Device_Serial0 = *(uint32_t *)MCU_ID1 + *(uint32_t *)MCU_ID3;
	Device_Serial1 = *(uint32_t *)MCU_ID2;

	if (Device_Serial0 != 0)
	{
		IntToUnicode (Device_Serial0, &CDC_StringSerial[2] , 8);
		IntToUnicode (Device_Serial1, &CDC_StringSerial[18], 4);
	}
}

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
 * @param	Request Nb.
 * @retval	USB_UNSUPPORT or USB_SUCCESS.
 */
USB_RESULT CDC_Data_Setup(uint8_t RequestNo)
{
	uint8_t	*(*CopyRoutine)(uint16_t) = NULL;

	if (RequestNo == GET_LINE_CODING)
	{
		if (Type_Recipient == (CLASS_REQUEST | INTERFACE_RECIPIENT))
			CopyRoutine = CDC_GetLineCoding;
	}
	else
	if (RequestNo == SET_LINE_CODING)
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
