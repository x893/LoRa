// rf95_reliable_datagram_client.pde
// -*- mode: C++ -*-
// Example sketch showing how to create a simple addressed, reliable messaging client
// with the RHReliableDatagram class, using the RH_RF95 driver to control a RF95 radio.
// It is designed to work with the other example rf95_reliable_datagram_server
// Tested with Anarduino MiniWirelessLoRa

#include "RHReliableDatagram.h"
#include "RH_RF95.h"

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
		Serial.print("Revision: 0x");
		Serial.println(driver.revision(), HEX);
	}
	delay(1000);
	digitalWrite(LED2, LOW);
	// Defaults after init are 434.0MHz, 23dBm, Bw = 31.25 kHz, Cr = 4/8, Sf = 512chips/symbol, CRC on
}

void i2a(char * dst, uint32_t n, uint8_t base)
{
	char buf[20];
	char *tmp = buf;
	char ch;

	if (n == 0)
		*tmp++ = 0;
	else
		while (n > 0)
		{
			*tmp++ = n % base;
			n /= base;
		}

	while (tmp != buf)
	{
		--tmp;
		ch = *tmp;
		*dst++ = (
				ch < 10 ?
				ch + '0' :
				ch + ('A' - 10)
				);
	}
	*dst = 0;
}

uint32_t package_count = 0;
char data[] = "SEQ: XXXXXXXX";
// Dont put this on the stack:
uint8_t buf[RH_RF95_MAX_MESSAGE_LEN];

void loop()
{
	// Send a message to manager_server
	i2a(data + 5, package_count, DEC);
	Serial.print("Sending ");
	Serial.print(data);

	digitalWrite(LED1, HIGH);

	if (manager.sendtoWait((uint8_t *)data, sizeof(data), SERVER_ADDRESS))
	{
		// Now wait for a reply from the server
		uint8_t len = sizeof(buf);
		uint8_t from;
		if (manager.recvfromAckTimeout(buf, &len, 2000, &from))
		{
			digitalWrite(LED1, LOW);
			digitalWrite(LED2, HIGH);
			Serial.print(" - Reply from : 0x");
			Serial.print(from, HEX);
			Serial.print(" : RSSI ");
			Serial.print(driver.lastRssi());
			Serial.print(" : ");
			Serial.println((char *)buf);
		}
		else
		{
			Serial.println(" - No reply");
			digitalWrite(LED1, LOW);
			digitalWrite(LED3, HIGH);
		}
	}
	else
	{
		Serial.println(" - Failed");
		digitalWrite(LED1, LOW);
		digitalWrite(LED3, HIGH);
	}
	delay(500);
	digitalWrite(LED1, LOW);
	digitalWrite(LED2, LOW);
	digitalWrite(LED3, LOW);
	delay(500);
	++package_count;
}
