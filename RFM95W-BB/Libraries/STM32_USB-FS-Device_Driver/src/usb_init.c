#include "usb_lib.h"

/*  The number of current endpoint, it will be used to specify an endpoint */
 uint8_t	EPindex;
 
/*  Points to the DEVICE_INFO structure of current device */
/*  The purpose of this register is to speed up the execution */
DEVICE_INFO * pInformation;

/*  Points to the DEVICE_PROP structure of current device */
/*  The purpose of this register is to speed up the execution */
DEVICE_PROP *pProperty;

uint16_t	wInterrupt_Mask;
DEVICE_INFO	UsbDeviceInfo;
USER_STANDARD_REQUESTS  * pUser_Standard_Requests;

/*******************************************************************************
* Function Name  : USB_Init
* Description    : USB system initialization
* Input          : None.
* Output         : None.
* Return         : None.
*******************************************************************************/
void USB_Init(void)
{
	pInformation = &UsbDeviceInfo;
	pInformation->ControlState = IN_DATA;

	pUser_Standard_Requests = &User_Standard_Requests;
	pProperty = &Device_Property;

	pProperty->Init();	/* Initialize devices one by one */
}
