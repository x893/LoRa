#include <RadioHead.h>
#include <USBSerial.h>

#include <usb_core.h>

 extern "C" {
	void USB_Init(void);
	void USB_Send_Data(const char * src, uint16_t length);
	void USB_Send(const char * src);
	bool USB_Send_Byte(uint8_t data);
}
 
void USBSerial::begin(uint32_t baud)
{
	USB_Init();
}

void USBSerial::write(uint8_t ch)
{
	if (UsbDeviceInfo.bDeviceState == CONFIGURED)
	{
		USB_Send_Byte(ch);
	}
}

#ifdef BOARD_HAVE_SERIALUSB
USBSerial SerialUSB;
#endif
