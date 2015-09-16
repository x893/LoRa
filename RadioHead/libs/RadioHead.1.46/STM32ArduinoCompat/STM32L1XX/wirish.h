// ArduinoCompat/wirish.h

#ifndef _wirish_h
#define _wirish_h

#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include <Radiohead.h>

#include <HardwareSPI.h>
#include <HardwareSerial.h>
#include <USBSerial.h>

#define PROGMEM
#define memcpy_P memcpy
typedef enum WiringPin {
	PA0 = 0x00, PA1, PA2, PA3, PA4, PA5, PA6, PA7, PA8, PA9, PA10, PA11, PA12, PA13, PA14, PA15,
	PB0 = 0x10, PB1, PB2, PB3, PB4, PB5, PB6, PB7, PB8, PB9, PB10, PB11, PB12, PB13, PB14, PB15,
	PC0 = 0x20, PC1, PC2, PC3, PC4, PC5, PC6, PC7, PC8, PC9, PC10, PC11, PC12, PC13, PC14, PC15,
	PIN_UNUSED = 0xFF
} WiringPin;
typedef enum WiringPinMode {
	OUTPUT, /**< Basic digital output: when the pin is HIGH, the
				voltage is held at +3.3v (Vcc) and when it is LOW, it
				is pulled down to ground. */

	OUTPUT_OPEN_DRAIN, /**< In open drain mode, the pin indicates
							"low" by accepting current flow to ground
							and "high" by providing increased
							impedance. An example use would be to
							connect a pin to a bus line (which is pulled
							up to a positive voltage by a separate
							supply through a large resistor). When the
							pin is high, not much current flows through
							to ground and the line stays at positive
							voltage; when the pin is low, the bus
							"drains" to ground with a small amount of
							current constantly flowing through the large
							resistor from the external supply. In this
							mode, no current is ever actually sourced
							from the pin. */

	INPUT, /**< Basic digital input. The pin voltage is sampled; when
				it is closer to 3.3v (Vcc) the pin status is high, and
				when it is closer to 0v (ground) it is low. If no
				external circuit is pulling the pin voltage to high or
				low, it will tend to randomly oscillate and be very
				sensitive to noise (e.g., a breath of air across the pin
				might cause the state to flip). */

	INPUT_ANALOG, /**< This is a special mode for when the pin will be
					 used for analog (not digital) reads.	Enables ADC
					 conversion to be performed on the voltage at the
					 pin. */

	INPUT_PULLUP, /**< The state of the pin in this mode is reported
					 the same way as with INPUT, but the pin voltage
					 is gently "pulled up" towards +3.3v. This means
					 the state will be high unless an external device
					 is specifically pulling the pin down to ground,
					 in which case the "gentle" pull up will not
					 affect the state of the input. */

	INPUT_PULLDOWN, /**< The state of the pin in this mode is reported
						the same way as with INPUT, but the pin voltage
						is gently "pulled down" towards 0v. This means
						the state will be low unless an external device
						is specifically pulling the pin up to 3.3v, in
						which case the "gentle" pull down will not
						affect the state of the input. */

	INPUT_FLOATING, /**< Synonym for INPUT. */

	PWM, /**< This is a special mode for when the pin will be used for
			PWM output (a special case of digital output). */

	PWM_OPEN_DRAIN, /**< Like PWM, except that instead of alternating
						cycles of LOW and HIGH, the voltage on the pin
						consists of alternating cycles of LOW and
						floating (disconnected). */
} WiringPinMode;

void pinMode(uint8_t pin, WiringPinMode mode);
void digitalWrite(uint8_t pin, uint8_t val);
uint8_t digitalRead(uint8_t pin);
void attachInterrupt(uint8_t, void (*)(void), int mode);

#ifdef __cplusplus
extern "C" {
#endif
	extern uint32_t millis(void);
	extern void delay(uint32_t ms);
#ifdef __cplusplus
}
#endif

int32_t random(int32_t to);
int32_t random(int32_t from, int32_t to);

#define HIGH		0x1
#define LOW			0x0

#define LSBFIRST	0
#define MSBFIRST	1

#define CHANGE		1
#define FALLING		2
#define RISING		3

#endif
