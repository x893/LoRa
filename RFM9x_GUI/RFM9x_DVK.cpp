/*
RFM9x-DVK emulator for NUCLEO-F103RB
*/
#include <mbed.h>

RawSerial host(PC_10,PC_11);

SPI rfm(D11, D12, D13);
DigitalOut rfm_cs(D10);
DigitalInOut rfm_dio0(D9);

enum ChipSet
{
	RF96 = 18,
	RF92 = 34,
};
enum SPI_CMD
{
	SPI_NACK = 0,
	SPI_QUERY = 128,
	SPI_TxLED_ON = 129,
	SPI_LED_OFF = 130,
	SPI_BEEP_ON = 131,
	SPI_BEEP_OFF = 132,
	SPI_RxLED_ON = 133,
	SPI_HRESET = 248,
	SPI_LRESET = 249,
	SPI_ACK = 250,
	SPI_READ = 252,
	SPI_BURST_READ = 253,
	SPI_WRITE = 254,
	SPI_BURST_WRITE = 255,
};

enum LCD_CMD
{
	LCD_NACK = 0,
	LCD_CLEAR = 112,
	LCD_SHOW = 113,
	LCD_SETVALUE = 114,
	LCD_ANDVALUE = 115,
	LCD_ORVALUE = 116,
	LCD_ACK = 122,
	LCD_FULL = 127,
};

enum HAL_CMD
{
	HAL_DIO2_L = 96,
	HAL_DIO2_H = 97,
	HAL_DIO2_OUT = 99,
	HAL_DIO2_IN = 100,
};

int main()
{
	int cmd;
	int addr;
	int data;

	rfm_cs = 1;
	rfm.format(8, 0);
	rfm.frequency(1000000);
	rfm_dio0.input();
	rfm_dio0.mode(PullUp);

	host.format(8, SerialBase::None, 1);
	host.baud(115200);
	host.puts("\r\nRFM9X-DVK HopeRF LoRa (BUILD:[" __DATE__ "/" __TIME__ "])\r\n");

	while (1)
	{
		if (host.readable())
		{
			int cmd = host.getc();
			if (cmd == SPI_READ)
			{
				addr = host.getc();		// address
				data = host.getc();
// Only for emulation
				if (addr == 66)			// Chip version register
				{
					host.putc(RF96);
				}
				else if (addr == 1)		// RegOpMode
				{
					host.putc(0);
				}
				else
				{
					host.putc(0);
				}
			}
			else if (cmd == SPI_WRITE)
			{
				addr = host.getc();
				data = host.getc();
			}
			else if (cmd == SPI_LRESET)
			{
				host.putc(SPI_ACK);		// FSK Reset
			}
			else if (cmd == SPI_HRESET)
			{
				host.putc(SPI_ACK);		// LoRa Reset
			}
			else if (cmd == SPI_LED_OFF)
			{
			}
			else if (cmd == SPI_TxLED_ON)
			{
			}
			else if (cmd == SPI_RxLED_ON)
			{
			}
			else if (cmd == SPI_BEEP_ON)
			{
			}
			else if (cmd == SPI_BEEP_OFF)
			{
			}
			else if (cmd == SPI_BEEP_OFF)
			{
			}
			else if (cmd == HAL_DIO2_IN)
			{
			}
			else if (cmd == HAL_DIO2_OUT)
			{
			}
			else if (cmd == HAL_DIO2_H)
			{
			}
			else if (cmd == HAL_DIO2_L)
			{
			}
			else if (cmd == LCD_ANDVALUE)
			{
				addr = host.getc();
				data = host.getc();
			}
			else if (cmd == LCD_ORVALUE)
			{
				addr = host.getc();
				data = host.getc();
			}
			else if (cmd == LCD_SETVALUE)
			{
				addr = host.getc();
				data = host.getc();
			}
			else if (cmd == LCD_FULL)
			{
				host.putc(LCD_ACK);
			}
			else if (cmd == LCD_CLEAR)
			{
				host.putc(LCD_ACK);
			}
			else if (cmd == LCD_SHOW)
			{
				host.putc(LCD_ACK);
			}
			else if (cmd == SPI_QUERY)
			{
				host.puts("\r\nRFM9X-DVK HopeRF LoRa (BUILD:[" __DATE__ "/" __TIME__ "])\r\n");
			}
		}
	}
}
