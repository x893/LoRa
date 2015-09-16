// rf95_reliable_datagram_client.pde
// -*- mode: C++ -*-
// Example sketch showing how to create a simple addressed, reliable messaging client
// with the RHReliableDatagram class, using the RH_RF95 driver to control a RF95 radio.
// It is designed to work with the other example rf95_reliable_datagram_server
// Tested with Anarduino MiniWirelessLoRa

#include <RHReliableDatagram.h>
#include <RH_RF95.h>
#include <SPI.h>

#define LED1	PB11
#define LED2	PB10
#define LED3	PA3

#define CLIENT_ADDRESS 1
#define SERVER_ADDRESS 2

// Singleton instance of the radio driver
//				NSS  DIO0     SPI       RESET POWER_ON
RH_RF95 driver(PA15, PB6, hardware_spi, PB15, PB9);

// Class to manage message delivery and receipt, using the driver declared above
RHReliableDatagram manager(driver, CLIENT_ADDRESS);

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

uint8_t data[] = "Hello World!";
// Dont put this on the stack:
uint8_t buf[RH_RF95_MAX_MESSAGE_LEN];

void loop()
{
	Serial.println("Sending to rf95_reliable_datagram_server");

	// Send a message to manager_server
	digitalWrite(LED1, HIGH);
	if (manager.sendtoWait(data, sizeof(data), SERVER_ADDRESS))
	{
		// Now wait for a reply from the server
		uint8_t len = sizeof(buf);
		uint8_t from;
		if (manager.recvfromAckTimeout(buf, &len, 2000, &from))
		{
			digitalWrite(LED1, LOW);
			digitalWrite(LED2, HIGH);
			Serial.print("got reply from : 0x");
			Serial.print(from, HEX);
			Serial.print(": ");
			Serial.println((char*)buf);
		}
		else
		{
			digitalWrite(LED1, LOW);
			digitalWrite(LED3, HIGH);
			Serial.println("No reply, is rf95_reliable_datagram_server running?");
		}
	}
	else
	{
		digitalWrite(LED1, LOW);
		digitalWrite(LED3, HIGH);
		Serial.println("sendtoWait failed");
	}
	delay(500);
	digitalWrite(LED1, LOW);
	digitalWrite(LED2, LOW);
	digitalWrite(LED3, LOW);
	delay(500);
}
