// RH_RF95.cpp
//
// Copyright (C) 2011 Mike McCauley
// $Id: RH_RF95.cpp,v 1.8 2015/08/12 23:18:51 mikem Exp $

#include "RH_RF95.h"
#include "wirish.h"

// Interrupt vectors for the 3 Arduino interrupt pins
// Each interrupt can be handled by a different instance of RH_RF95, allowing you to have
// 2 or more LORAs per Arduino
RH_RF95 * RH_RF95::_deviceForInterrupt[RH_RF95_NUM_INTERRUPTS] = {NULL, NULL, NULL};
uint8_t RH_RF95::_interruptCount = 0; // Index into _deviceForInterrupt for next device

// These are indexed by the values of ModemConfigChoice
// Stored in flash (program) memory to save SRAM
PROGMEM static const RH_RF95::ModemConfig MODEM_CONFIG_TABLE[] =
{
	//	1d,	 1e,	26
	{ 0x72,	0x74,	0x00}, // Bw125Cr45Sf128 (the chip default)
	{ 0x92,	0x74,	0x00}, // Bw500Cr45Sf128
	{ 0x48,	0x94,	0x00}, // Bw31_25Cr48Sf512
	{ 0x78,	0xc4,	0x00}, // Bw125Cr48Sf4096
};

RH_RF95::RH_RF95(
	uint8_t slaveSelectPin,
	uint8_t interruptPin,
	RHGenericSPI& spi,
	uint8_t resetPin,
	uint8_t powerPin
	)
	:
	RHSPIDriver(slaveSelectPin, spi),
	_rxBufValid(0),
	_resetPin(resetPin),
	_powerPin(powerPin)
{
	_interruptPin = interruptPin;
	_myInterruptIndex = 0xFF; // Not allocated yet
}

bool RH_RF95::init()
{
	bool result = false;

	if (_resetPin != PIN_UNUSED)
		pinMode(_resetPin, INPUT);

	if (_powerPin != PIN_UNUSED)
	{
		pinMode(_powerPin, OUTPUT);
		digitalWrite(_powerPin, HIGH);

		if (_resetPin != PIN_UNUSED)
			while (digitalRead(_resetPin) != HIGH)
				;
		delay(20);
	}

	if (!RHSPIDriver::init())
		return false;

	// Determine the interrupt number that corresponds to the interruptPin
	int interruptNumber = digitalPinToInterrupt(_interruptPin);
	if (interruptNumber == NOT_AN_INTERRUPT)
		return false;

	// Set sleep mode, so we can also set LORA mode:
	spiWrite(RH_RF95_REG_01_OP_MODE, RH_RF95_MODE_SLEEP | RH_RF95_LONG_RANGE_MODE);
	delay(10); // Wait for sleep mode to take over from say, CAD
	_revision = spiRead(RH_RF95_REG_42_VERSION);

	// Check we are in sleep mode, with LORA set
	if (spiRead(RH_RF95_REG_01_OP_MODE) == (RH_RF95_MODE_SLEEP | RH_RF95_LONG_RANGE_MODE))
	{
		// Add by Adrien van den Bossche <vandenbo@univ-tlse2.fr> for Teensy
		// ARM M4 requires the below. else pin interrupt doesn't work properly.
		// On all other platforms, its innocuous, belt and braces
		pinMode(_interruptPin, INPUT_PULLDOWN);

		// Set up interrupt handler
		// Since there are a limited number of interrupt glue functions isr*() available,
		// we can only support a limited number of devices simultaneously
		// ON some devices, notably most Arduinos, the interrupt pin passed in is actuallt the 
		// interrupt number. You have to figure out the interruptnumber-to-interruptpin mapping
		// yourself based on knwledge of what Arduino board you are running on.
		if (_myInterruptIndex == 0xFF)
		{
			// First run, no interrupt allocated yet
			if (_interruptCount < RH_RF95_NUM_INTERRUPTS)
				_myInterruptIndex = _interruptCount++;
			// else Too many devices, not enough interrupt vectors
		}
		if (_myInterruptIndex < RH_RF95_NUM_INTERRUPTS)
		{
			_deviceForInterrupt[_myInterruptIndex] = this;

			if (_myInterruptIndex == 0)
				attachInterrupt(interruptNumber, isr0, RISING);
			else if (_myInterruptIndex == 1)
				attachInterrupt(interruptNumber, isr1, RISING);
			else if (_myInterruptIndex == 2)
				attachInterrupt(interruptNumber, isr2, RISING);

			// Set up FIFO
			// We configure so that we can use the entire 256 byte FIFO for either receive
			// or transmit, but not both at the same time
			spiWrite(RH_RF95_REG_0E_FIFO_TX_BASE_ADDR, 0);
			spiWrite(RH_RF95_REG_0F_FIFO_RX_BASE_ADDR, 0);

			result = true;
		}
	}

	if (result)
	{
		// Packet format is preamble + explicit-header + payload + crc
		// Explicit Header Mode
		// payload is TO + FROM + ID + FLAGS + message data
		// RX mode is implmented with RXCONTINUOUS
		// max message data length is 255 - 4 = 251 octets

		setModeIdle();

		// Set up default configuration
		// No Sync Words in LORA mode.

		//	setModemConfig(Bw125Cr45Sf128);		// Radio default medium
		//	setModemConfig(Bw125Cr48Sf4096);	// slow and reliable?
		setModemConfig(Bw31_25Cr48Sf512);

		setPreambleLength(8);	// Default is 8
		setFrequency(434.0);	// An innocuous ISM frequency, same as RF22's
		setTxPower(23);			// Highest power
	}

	return result;
}

uint8_t RH_RF95::revision(void)
{
	return _revision;
}

// C++ level interrupt handler for this instance
// LORA is unusual in that it has several interrupt lines, and not a single, combined one.
// On MiniWirelessLoRa, only one of the several interrupt lines (DI0) from the RFM95 is usefuly 
// connnected to the processor.
// We use this to get RxDone and TxDone interrupts
void RH_RF95::handleInterrupt()
{
	// Read the interrupt register
	uint8_t irq_flags = spiRead(RH_RF95_REG_12_IRQ_FLAGS);

	if (_mode == RHModeRx)
	{
		if (irq_flags & (RH_RF95_RX_TIMEOUT | RH_RF95_PAYLOAD_CRC_ERROR))
		{
			_rxBad++;
			// Stay in RX mode
		}
		else if (irq_flags & RH_RF95_RX_DONE)
		{
			// Have received a packet
			uint8_t len = spiRead(RH_RF95_REG_13_RX_NB_BYTES);

			// Reset the fifo read ptr to the beginning of the packet
			spiWrite(RH_RF95_REG_0D_FIFO_ADDR_PTR, spiRead(RH_RF95_REG_10_FIFO_RX_CURRENT_ADDR));
			spiBurstRead(RH_RF95_REG_00_FIFO, _buf, len);
			_bufLen = len;
			spiWrite(RH_RF95_REG_12_IRQ_FLAGS, 0xFF); // Clear all IRQ flags

			// Remember the RSSI of this packet
			// this is according to the doc, but is it really correct?
			// weakest receiveable signals are reported RSSI at about -66
			_lastRssi = spiRead(RH_RF95_REG_1A_PKT_RSSI_VALUE) - 137;

			// We have received a message.
			validateRxBuf();
			if (_rxBufValid)
				setModeIdle(); // Got one
			// else stay in RX mode
		}
	}
	else if (_mode == RHModeTx && (irq_flags & RH_RF95_TX_DONE))
	{
		_txGood++;
		setModeIdle();
	}
	spiWrite(RH_RF95_REG_12_IRQ_FLAGS, 0xFF); // Clear all IRQ flags
}

// These are low level functions that call the interrupt handler for the correct
// instance of RH_RF95.
// 3 interrupts allows us to have 3 different devices
void RH_RF95::isr0()
{
	if (_deviceForInterrupt[0])
		_deviceForInterrupt[0]->handleInterrupt();
}
void RH_RF95::isr1()
{
	if (_deviceForInterrupt[1])
		_deviceForInterrupt[1]->handleInterrupt();
}
void RH_RF95::isr2()
{
	if (_deviceForInterrupt[2])
		_deviceForInterrupt[2]->handleInterrupt();
}

// Check whether the latest received message is complete and uncorrupted
void RH_RF95::validateRxBuf()
{
	if (_bufLen < 4)
		return;	// Too short to be a real message

	// Extract the 4 headers
	_rxHeaderTo		= _buf[0];
	_rxHeaderFrom	= _buf[1];
	_rxHeaderId		= _buf[2];
	_rxHeaderFlags	= _buf[3];

	if (_promiscuous
	||	_rxHeaderTo == _thisAddress
	||	_rxHeaderTo == RH_BROADCAST_ADDRESS)
	{
		_rxGood++;
		_rxBufValid = true;
	}
}

bool RH_RF95::available()
{
	if (_mode == RHModeTx)
		return false;
	setModeRx();
	return _rxBufValid; // Will be set by the interrupt handler when a good message is received
}

void RH_RF95::clearRxBuf()
{
	ATOMIC_BLOCK_START;
	_rxBufValid = false;
	_bufLen = 0;
	ATOMIC_BLOCK_END;
}

bool RH_RF95::recv(uint8_t* buf, uint8_t* len)
{
	if (!available())
		return false;
	if (buf && len)
	{
		ATOMIC_BLOCK_START;
		// Skip the 4 headers that are at the beginning of the rxBuf
		if (*len > _bufLen - RH_RF95_HEADER_LEN)
			*len = _bufLen - RH_RF95_HEADER_LEN;
		memcpy(buf, _buf + RH_RF95_HEADER_LEN, *len);
		ATOMIC_BLOCK_END;
	}
	clearRxBuf(); // This message accepted and cleared
	return true;
}

bool RH_RF95::send(const uint8_t* data, uint8_t len)
{
	if (len > RH_RF95_MAX_MESSAGE_LEN)
		return false;

	if (! waitPacketSent(2000))
		return false;	// Make sure we dont interrupt an outgoing message

    ATOMIC_BLOCK_START;

	setModeIdle();

	spiWrite(RH_RF95_REG_0D_FIFO_ADDR_PTR, 0);		// Position at the beginning of the FIFO

	spiWrite(RH_RF95_REG_00_FIFO, _txHeaderTo);		// The headers
	spiWrite(RH_RF95_REG_00_FIFO, _txHeaderFrom);
	spiWrite(RH_RF95_REG_00_FIFO, _txHeaderId);
	spiWrite(RH_RF95_REG_00_FIFO, _txHeaderFlags);

	spiBurstWrite(RH_RF95_REG_00_FIFO, data, len);	// The message data
	spiWrite(RH_RF95_REG_22_PAYLOAD_LENGTH, len + RH_RF95_HEADER_LEN);

	setModeTx();	// Start the transmitter
					// when Tx is done, interruptHandler will fire
					// and radio mode will return to STANDBY
    ATOMIC_BLOCK_END;

	return true;
}

bool RH_RF95::printRegisters()
{
	uint8_t registers[] = {
		RH_RF95_REG_01_OP_MODE,
		RH_RF95_REG_06_FRF_MSB,
		RH_RF95_REG_07_FRF_MID,
		RH_RF95_REG_08_FRF_LSB,
		RH_RF95_REG_09_PA_CONFIG,
		RH_RF95_REG_0A_PA_RAMP,
		RH_RF95_REG_0B_OCP,
		RH_RF95_REG_0C_LNA,
		RH_RF95_REG_0D_FIFO_ADDR_PTR,
		RH_RF95_REG_0E_FIFO_TX_BASE_ADDR,
		RH_RF95_REG_0F_FIFO_RX_BASE_ADDR,
		RH_RF95_REG_10_FIFO_RX_CURRENT_ADDR,
		RH_RF95_REG_11_IRQ_FLAGS_MASK,
		RH_RF95_REG_12_IRQ_FLAGS,
		RH_RF95_REG_13_RX_NB_BYTES,
		RH_RF95_REG_14_RX_HEADER_CNT_VALUE_MSB,
		RH_RF95_REG_15_RX_HEADER_CNT_VALUE_LSB,
		RH_RF95_REG_16_RX_PACKET_CNT_VALUE_MSB,
		RH_RF95_REG_17_RX_PACKET_CNT_VALUE_LSB,
		RH_RF95_REG_18_MODEM_STAT,
		RH_RF95_REG_19_PKT_SNR_VALUE,
		RH_RF95_REG_1A_PKT_RSSI_VALUE,
		RH_RF95_REG_1B_RSSI_VALUE,
		RH_RF95_REG_1C_HOP_CHANNEL,
		RH_RF95_REG_1D_MODEM_CONFIG1,
		RH_RF95_REG_1E_MODEM_CONFIG2,
		RH_RF95_REG_1F_SYMB_TIMEOUT_LSB,
		RH_RF95_REG_20_PREAMBLE_MSB,
		RH_RF95_REG_21_PREAMBLE_LSB,
		RH_RF95_REG_22_PAYLOAD_LENGTH,
		RH_RF95_REG_23_MAX_PAYLOAD_LENGTH,
		RH_RF95_REG_24_HOP_PERIOD,
		RH_RF95_REG_25_FIFO_RX_BYTE_ADDR,
		RH_RF95_REG_26_MODEM_CONFIG3,
		RH_RF95_REG_27_SYNC_CONFIG
	};

	uint8_t i;
	for (i = 0; i < sizeof(registers); i++)
	{
		Serial.print(registers[i], HEX);
		Serial.print(" : ");
		Serial.println(spiRead(registers[i]), HEX);
	}
	return true;
}

uint8_t RH_RF95::maxMessageLength()
{
	return RH_RF95_MAX_MESSAGE_LEN;
}

bool RH_RF95::setFrequency(float centre)
{
	// Frf = FRF / FSTEP
	uint32_t frf = (centre * 1000000.0) / RH_RF95_FSTEP;

	ATOMIC_BLOCK_START;

	spiWrite(RH_RF95_REG_06_FRF_MSB, (frf >> 16) & 0xFF);
	spiWrite(RH_RF95_REG_07_FRF_MID, (frf >>  8) & 0xFF);
	spiWrite(RH_RF95_REG_08_FRF_LSB, (frf      ) & 0xFF);

	ATOMIC_BLOCK_END;

	return true;
}

//	Always call from ATOMIC_BLOCK or interrupt handle
void RH_RF95::setModeIdle()
{
	if (_mode != RHModeIdle)
	{
		_mode = RHModeIdle;
		spiWrite(RH_RF95_REG_01_OP_MODE, RH_RF95_MODE_STDBY);
	}
}

//	Always call from ATOMIC_BLOCK or interrupt handle
void RH_RF95::setModeTx()
{
	if (_mode != RHModeTx)
	{
		_mode = RHModeTx;
		spiWrite(RH_RF95_REG_40_DIO_MAPPING1, 0x40);		// Interrupt on TxDone
		spiWrite(RH_RF95_REG_01_OP_MODE, RH_RF95_MODE_TX);	// Set TX mode
	}
}

bool RH_RF95::sleep()
{
	if (_mode != RHModeSleep)
	{
		ATOMIC_BLOCK_START;
		_mode = RHModeSleep;
		spiWrite(RH_RF95_REG_01_OP_MODE, RH_RF95_MODE_SLEEP);
		ATOMIC_BLOCK_END;
	}
	return true;
}

void RH_RF95::setModeRx()
{
	if (_mode != RHModeRx)
	{
		ATOMIC_BLOCK_START;
		_mode = RHModeRx;
		spiWrite(RH_RF95_REG_40_DIO_MAPPING1, 0x00);	// Interrupt on RxDone
		spiWrite(RH_RF95_REG_01_OP_MODE, RH_RF95_MODE_RXCONTINUOUS);
		ATOMIC_BLOCK_END;
	}
}

void RH_RF95::setTxPower(int8_t power)
{
	if (power > 23)
		power = 23;
	if (power < 5)
		power = 5;

	ATOMIC_BLOCK_START;
	
	// For RH_RF95_PA_DAC_ENABLE, manual says '+20dBm on PA_BOOST when OutputPower=0xf'
	// RH_RF95_PA_DAC_ENABLE actually adds about 3dBm to all power levels. We will us it
	// for 21, 22 and 23dBm
	if (power > 20)
	{
		spiWrite(RH_RF95_REG_4D_PA_DAC, RH_RF95_PA_DAC_ENABLE);
		power -= 3;
	}
	else
		spiWrite(RH_RF95_REG_4D_PA_DAC, RH_RF95_PA_DAC_DISABLE);

	// RFM95/96/97/98 does not have RFO pins connected to anything. Only PA_BOOST
	// pin is connected, so must use PA_BOOST
	// Pout = 2 + OutputPower.
	// The documentation is pretty confusing on this topic: PaSelect says the max power is 20dBm,
	// but OutputPower claims it would be 17dBm.
	// My measurements show 20dBm is correct
	spiWrite(RH_RF95_REG_09_PA_CONFIG, RH_RF95_PA_SELECT | (power - 5));

	ATOMIC_BLOCK_END;
}

// Sets registers from a canned modem configuration structure
void RH_RF95::setModemRegisters(const ModemConfig* config)
{
	ATOMIC_BLOCK_START;

	spiWrite(RH_RF95_REG_1D_MODEM_CONFIG1,		config->reg_1d);
	spiWrite(RH_RF95_REG_1E_MODEM_CONFIG2,		config->reg_1e);
	spiWrite(RH_RF95_REG_26_MODEM_CONFIG3,		config->reg_26);

	ATOMIC_BLOCK_END;
}

// Set one of the canned FSK Modem configs
// Returns true if its a valid choice
bool RH_RF95::setModemConfig(ModemConfigChoice index)
{
	if ((signed int)index > (signed int)(sizeof(MODEM_CONFIG_TABLE) / sizeof(ModemConfig)))
		return false;

	ModemConfig cfg;
	memcpy_P(&cfg, &MODEM_CONFIG_TABLE[index], sizeof(RH_RF95::ModemConfig));

	setModemRegisters(&cfg);
	return true;
}

void RH_RF95::setPreambleLength(uint16_t bytes)
{
	ATOMIC_BLOCK_START;

	spiWrite(RH_RF95_REG_20_PREAMBLE_MSB, bytes >> 8);
	spiWrite(RH_RF95_REG_21_PREAMBLE_LSB, bytes & 0xFF);

	ATOMIC_BLOCK_END;
}
