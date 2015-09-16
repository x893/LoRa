#ifndef __USB_SERIAL_H__
#define __USB_SERIAL_H__

#include <Print.h>

class USBSerial : public Print
{
public:
    // TODO: move these from being inlined
    void begin(uint32_t baud);
};

#if BOARD_HAVE_SERIALUSB
	extern USBSerial SerialUSB;
	#define HAVE_CDCSERIAL
#endif

#endif
