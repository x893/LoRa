// ArduinoCompat/HardwareSerial.h
// STM32 implementation of Arduino compatible serial class

#ifndef _HardwareSerial_h
#define _HardwareSerial_h

#include <stdio.h>
#include <stm32l1xx.h>

#include <Print.h>

#ifndef ARDUINO_RINGBUFFER_SIZE
	#define ARDUINO_RINGBUFFER_SIZE 64
#endif

class RingBuffer
{
public:
	RingBuffer();
	bool	isEmpty();
	bool	isFull();
	bool	write(uint8_t ch);
	uint8_t read();

private:
	uint8_t _buffer[ARDUINO_RINGBUFFER_SIZE]; // In fact we can hold up to ARDUINO_RINGBUFFER_SIZE-1 bytes
	uint16_t _head;		// Index of next write
	uint16_t _tail;		// Index of next read
	uint32_t _overruns;	// Write attempted when buffer full
	uint32_t _underruns; // Read attempted when buffer empty
};

// Mostly compatible wuith Arduino HardwareSerial
// Theres just enough here to support RadioHead RH_Serial
class HardwareSerialNull : public Print
{
public:
	HardwareSerialNull(void) { }
	void begin(uint32_t baud) { }
	void end() { }
	virtual int available(void)		{ return 0; }
	virtual int read(void)			{ return 0; }
	virtual size_t write(uint8_t)	{ return 1; }
	inline size_t write(uint64_t n)	{ return 1; }
	inline size_t write(int64_t n)	{ return 1; }
	inline size_t write(uint32_t n)	{ return 1; }
	inline size_t write(int32_t n)	{ return 1; }
};

// Mostly compatible wuith Arduino HardwareSerial
// Theres just enough here to support RadioHead RH_Serial
class HardwareSerial : public Print
{
public:
	HardwareSerial(USART_TypeDef* usart);
	void begin(uint32_t baud);
	void end();
	virtual int available(void);
	virtual int read(void);
	virtual size_t write(uint8_t);
	inline size_t write(uint64_t n)	{ return write((uint8_t)n); }
	inline size_t write(int64_t n)	{ return write((uint8_t)n); }
	inline size_t write(uint32_t n)	{ return write((uint8_t)n); }
	inline size_t write(int32_t n)	{ return write((uint8_t)n); }

	// These need to be public so the IRQ handler can read and write to them:
	RingBuffer	 _rxRingBuffer;
	RingBuffer	 _txRingBuffer;

private:
	USART_TypeDef* _usart;
};

// Predefined serial ports are configured so:
// Serial		STM32 UART	RX pin	Tx Pin	Comments
// Serial1		USART1		PA10	 PA9		TX Conflicts with GREEN LED on Discovery
// Serial2		USART2		PA3		PA2
//
// All ports are idle HIGH, LSB first, 8 bits, No parity, 1 stop bit

#ifdef BOARD_HAVE_SERIAL1
	extern HardwareSerial Serial1;
	#define HAVE_HWSERIAL1
#endif
#ifdef BOARD_HAVE_SERIAL2
	extern HardwareSerial Serial2;
	#define HAVE_HWSERIAL2
#endif

#if ! defined ( BOARD_HAVE_SERIAL1 )	\
&&	! defined ( BOARD_HAVE_SERIAL2 )	\
&&	! defined ( BOARD_HAVE_SERIALUSB )
	extern HardwareSerialNull Serial;
#endif

#endif
