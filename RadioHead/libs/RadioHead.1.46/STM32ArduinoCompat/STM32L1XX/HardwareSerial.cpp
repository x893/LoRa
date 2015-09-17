// ArduinoCompat/HardwareSerial.cpp
//
// Author: mikem@airspayce.com

#include "RadioHead.h"
#include "stm32l1xx_usart.h"

// Preinstantiated Serial objects
#ifdef BOARD_HAVE_SERIAL1
	HardwareSerial Serial1(USART1);
#endif
#ifdef BOARD_HAVE_SERIAL2
	HardwareSerial Serial2(USART2);
#endif

#if ! defined ( BOARD_HAVE_SERIAL1 )	\
&&	! defined ( BOARD_HAVE_SERIAL2 )	\
&&	! defined ( BOARD_HAVE_SERIALUSB )
	HardwareSerialNull SerialNull;
#endif

///////////////////////////////////////////////////////////////
// RingBuffer
///////////////////////////////////////////////////////////////

RingBuffer::RingBuffer()
	:	_head(0),
		_tail(0),
		_overruns(0),
		_underruns(0)
{
}

bool RingBuffer::isEmpty()
{
	return _head == _tail;
}	

bool RingBuffer::isFull()
{
	return ((_head + 1) % ARDUINO_RINGBUFFER_SIZE) == _tail;
}

bool RingBuffer::write(uint8_t ch)
{
	if (isFull())
	{
		_overruns++;
		return false;
	}
	_buffer[_head] = ch;
	if (++_head >= ARDUINO_RINGBUFFER_SIZE)
		_head = 0;
	return true;
}

uint8_t RingBuffer::read()
{
	if (isEmpty())
	{
		_underruns++;
		return 0; // What else can we do?
	}
	uint8_t ret = _buffer[_tail];
	if (++_tail >= ARDUINO_RINGBUFFER_SIZE)
		_tail = 0;
	return ret;
}

///////////////////////////////////////////////////////////////
// HardwareSerial
///////////////////////////////////////////////////////////////

// On STM32F4 Discovery, USART 1 is not very useful conflicts with the Green lED
HardwareSerial::HardwareSerial(USART_TypeDef* usart)
	: _usart(usart)
{
}

void HardwareSerial::begin(uint32_t baud)
{	
	USART_InitTypeDef	USART_InitStructure;
	GPIO_InitTypeDef	GPIO_InitStructure;

	// Common GPIO structure init:
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_40MHz;
	GPIO_InitStructure.GPIO_Mode	= GPIO_Mode_AF;
	GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;
	GPIO_InitStructure.GPIO_PuPd	= GPIO_PuPd_UP;

	USART_InitStructure.USART_BaudRate		= baud;

	// Only 8N1 is currently supported
	USART_InitStructure.USART_WordLength	= USART_WordLength_8b;	
	USART_InitStructure.USART_StopBits		= USART_StopBits_1;	
	USART_InitStructure.USART_Parity		= USART_Parity_No;
	USART_InitStructure.USART_Mode			= USART_Mode_Rx | USART_Mode_Tx;
	USART_InitStructure.USART_HardwareFlowControl = USART_HardwareFlowControl_None;

	RCC_AHBPeriphClockCmd(RCC_AHBPeriph_GPIOA, ENABLE);
	// Initialise the USART
	USART_Init(_usart, &USART_InitStructure);
	// Enable the RXNE interrupt
	USART_ITConfig(_usart, USART_IT_RXNE, ENABLE);

	// Different for each USART:
	if (_usart == USART1)
	{
		// Initialise the clocks for this USART and its RX, TX pins port
		RCC_APB2PeriphClockCmd(RCC_APB2Periph_USART1, ENABLE);

		GPIO_PinAFConfig(GPIOA, GPIO_PinSource9,  GPIO_AF_USART1);
		GPIO_PinAFConfig(GPIOA, GPIO_PinSource10, GPIO_AF_USART1);

		GPIO_InitStructure.GPIO_Pin = GPIO_Pin_9;
		GPIO_Init(GPIOA, &GPIO_InitStructure);
		GPIO_InitStructure.GPIO_Pin = GPIO_Pin_10;
		GPIO_Init(GPIOA, &GPIO_InitStructure);

		// Enable global interrupt
		NVIC_EnableIRQ(USART1_IRQn);
	}
	else if (_usart == USART2)
	{
		// Initialise the clocks for this USART and its RX, TX pins port
		RCC_APB1PeriphClockCmd(RCC_APB1Periph_USART2, ENABLE);

		GPIO_PinAFConfig(GPIOA, GPIO_PinSource2, GPIO_AF_USART2);
		GPIO_PinAFConfig(GPIOA, GPIO_PinSource3, GPIO_AF_USART2);

		GPIO_InitStructure.GPIO_Pin = GPIO_Pin_2;
		GPIO_Init(GPIOA, &GPIO_InitStructure);
		GPIO_InitStructure.GPIO_Pin = GPIO_Pin_3;
		GPIO_Init(GPIOA, &GPIO_InitStructure);

		// Enable global interrupt
		NVIC_EnableIRQ(USART2_IRQn);
	}

	USART_Cmd(_usart, ENABLE);
}

void HardwareSerial::end()
{
	USART_Cmd(_usart, DISABLE);
	USART_DeInit(_usart);
}

int HardwareSerial::available(void)
{
	return !_rxRingBuffer.isEmpty();
}

int HardwareSerial::read(void)
{
	return _rxRingBuffer.read();
}

void HardwareSerial::write(uint8_t ch)
{
	_txRingBuffer.write(ch);						// Queue it
	USART_ITConfig(_usart, USART_IT_TXE, ENABLE);	// Enable the TX interrupt
}

extern "C"
{
#ifdef HAVE_HWSERIAL1
void USART1_IRQHandler(void)
{
	if (USART_GetITStatus(USART1, USART_IT_RXNE) != RESET)
	{	// Newly received char, try to put it in our rx buffer
		Serial1._rxRingBuffer.write(USART_ReceiveData(USART1));
	}
	if (USART_GetITStatus(USART1, USART_IT_TXE) != RESET)
	{
		// Transmitter is empty, maybe send another char?
		if (Serial1._txRingBuffer.isEmpty())
			USART_ITConfig(USART1, USART_IT_TXE, DISABLE); // No more to send, disable the TX interrupt
		else
			USART_SendData(USART1, Serial1._txRingBuffer.read());
	}
}
#endif

#ifdef HAVE_HWSERIAL2
void USART2_IRQHandler(void)
{
	if (USART_GetITStatus(USART2, USART_IT_RXNE) != RESET)
	{	// Newly received char, try to put it in our rx buffer
		Serial2._rxRingBuffer.write(USART_ReceiveData(USART2));
	}
	if (USART_GetITStatus(USART2, USART_IT_TXE) != RESET)
	{
		// Transmitter is empty, maybe send another char?
		if (Serial2._txRingBuffer.isEmpty())
			USART_ITConfig(USART2, USART_IT_TXE, DISABLE); // No more to send, disable the TX interrupt
		else
			USART_SendData(USART2, Serial2._txRingBuffer.read());
	}
}
#endif
}	/* extern "C" */
