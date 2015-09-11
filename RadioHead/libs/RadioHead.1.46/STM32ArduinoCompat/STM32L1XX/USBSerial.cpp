#include <RadioHead.h>

void USBSerial::begin(int baud)
{
	
}

#ifdef BOARD_HAVE_SERIALUSB
USBSerial SerialUSB;
#endif
