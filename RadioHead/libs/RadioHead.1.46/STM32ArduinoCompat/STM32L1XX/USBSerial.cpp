#include <RadioHead.h>

void USBSerial::begin(uint32_t baud)
{
	
}

#ifdef BOARD_HAVE_SERIALUSB
USBSerial SerialUSB;
#endif
