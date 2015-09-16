// rf95_reliable_datagram_server.pde
// -*- mode: C++ -*-
// Example sketch showing how to create a simple addressed, reliable messaging server
// with the RHReliableDatagram class, using the RH_RF95 driver to control a RF95 radio.
// It is designed to work with the other example rf95_reliable_datagram_client
// Tested with Anarduino MiniWirelessLoRa

#include <RHReliableDatagram.h>
#include <RH_RF95.h>

#define LED1	PB11
#define LED2	PB10
#define LED3	PA3

#define CLIENT_ADDRESS 1
#define SERVER_ADDRESS 2

// Singleton instance of the radio driver
//				NSS  DIO0     SPI       RESET POWER_ON
RH_RF95 driver(PA15, PB6, hardware_spi, PB15, PB9);

// Class to manage message delivery and receipt, using the driver declared above
RHReliableDatagram manager(driver, SERVER_ADDRESS);

void setup() 
{
	pinMode(LED1, OUTPUT);
	digitalWrite(LED1, LOW);
	pinMode(LED2, OUTPUT);
	digitalWrite(LED2, LOW);
	pinMode(LED3, OUTPUT);
	digitalWrite(LED3, HIGH);

	Serial.begin(115200);
	if (!manager.init())
	{
		Serial.println("init failed");
		while (1)
			;
	}
	else
	{
		digitalWrite(LED3, LOW);
		digitalWrite(LED2, HIGH);
		Serial.print("Revision:");
		Serial.println(driver.revision(), HEX);
	}
	// Defaults after init are 434.0MHz, 13dBm, Bw = 125 kHz, Cr = 4/5, Sf = 128chips/symbol, CRC on
}

uint8_t data[] = "And hello back to you";
// Dont put this on the stack:
uint8_t buf[RH_RF95_MAX_MESSAGE_LEN];

void loop()
{
	if (manager.available())
	{
		// Wait for a message addressed to us from the client
		uint8_t len = sizeof(buf);
		uint8_t from;
		if (manager.recvfromAck(buf, &len, &from))
		{
			Serial.print("got request from : 0x");
			Serial.print(from, HEX);
			Serial.print(": ");
			Serial.println((char*)buf);

			// Send a reply back to the originator client
			if (!manager.sendtoWait(data, sizeof(data), from))
			{
				Serial.println("sendtoWait failed");
			}
		}
	}
}
