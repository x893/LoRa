using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace LoRaModem
{
	public class FTDI
	{
		protected object syncThread = new object();
		private IContainer components;
		public SerialPort mycomm;
		public string PortName;

		public FTDI()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			components = (IContainer)new Container();
			mycomm = new SerialPort(components);
		}

		public bool Open()
		{
			try
			{
				foreach (string port in SerialPort.GetPortNames())
				{
					byte[] buffer = new byte[1] { (byte)FTDI.SPI_CMD.SPI_QUERY };
					mycomm.PortName = port;
					if (!mycomm.IsOpen)
					{
						mycomm.BaudRate = 115200;
						mycomm.Parity = Parity.None;
						mycomm.Open();
						mycomm.Write(buffer, 0, 1);
						Thread.Sleep(100);
						if (mycomm.ReadExisting().Contains("HopeRF LoRa"))
							return true;
						mycomm.Close();
					}
				}
				return false;
			}
			catch (Exception)
			{
				MessageBox.Show("Disconnected!");
				return false;
			}
		}

		public bool Close()
		{
			if (mycomm.PortName == null || !mycomm.IsOpen)
				return false;
			mycomm.Close();
			return true;
		}

		public bool SendByte(byte address, byte data)
		{
			lock (syncThread)
			{
				byte[] outData = new byte[3] { (byte)FTDI.SPI_CMD.SPI_WRITE, address, data };
				mycomm.DiscardOutBuffer();
				mycomm.Write(outData, 0, 3);
				byte local_1;
				for (local_1 = (byte)0; (int)local_1 < 5; ++local_1)
				{
					Thread.Sleep(3);
					if (mycomm.BytesToWrite == 0)
						break;
				}
				return (int)local_1 < 5;
			}
		}

		public bool SendBytes(byte address, byte[] data, int length)
		{
			lock (syncThread)
			{
				byte[] outData = new byte[(int)length + 2];
				outData[0] = (byte)FTDI.SPI_CMD.SPI_BURST_WRITE;
				outData[1] = address;
				for (int idx = 2; idx < length + 2; ++idx)
					outData[idx] = data[idx - 2];

				mycomm.DiscardOutBuffer();
				mycomm.Write(outData, 0, length + 2);
				int delay;
				for (delay = 0; delay < length; ++delay)
				{
					Thread.Sleep(5);
					if (mycomm.BytesToWrite == 0)
						break;
				}
				if (delay >= length)
					return false;
				mycomm.ReadTimeout += length + 100;
				FTDI.SPI_CMD ack;
				try
				{
					ack = (FTDI.SPI_CMD)(mycomm.ReadByte() & 0xFF);
				}
				catch
				{
					return false;
				}
				return (ack == FTDI.SPI_CMD.SPI_ACK);
			}
		}

		public bool ReadByte(byte address, ref byte data)
		{
			lock (syncThread)
			{
				try
				{
					byte[] outData = new byte[3] { (byte)FTDI.SPI_CMD.SPI_READ, address, 0xFF };
					mycomm.DiscardOutBuffer();
					mycomm.DiscardInBuffer();
					mycomm.Write(outData, 0, 3);
					int delay;
					for (delay = 0; delay < 5; ++delay)
					{
						Thread.Sleep(3);
						if (mycomm.BytesToWrite == 0)
							break;
					}
					if (delay >= 5)
						return false;
					mycomm.ReadTimeout += 10;
					data = (byte)(mycomm.ReadByte() & 0xFF);
					return true;
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		public bool ReadBytes(byte address, ref byte[] data, int length)
		{
			lock (syncThread)
			{
				try
				{
					byte[] outData = new byte[3] { (byte)FTDI.SPI_CMD.SPI_BURST_READ, address, (byte)length };
					mycomm.DiscardInBuffer();
					mycomm.DiscardOutBuffer();
					mycomm.Write(outData, 0, 3);
					int delay;
					for (delay = 0; delay < 5; ++delay)
					{
						Thread.Sleep(3);
						if (mycomm.BytesToWrite == 0)
							break;
					}
					if (delay >= 5)
						return false;
					Thread.Sleep(length + 10);
					mycomm.ReadTimeout += 20;
					mycomm.Read(data, 0, length);
					return true;
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		private bool SendByte(byte data)
		{
			byte[] outData = new byte[] { data };
			mycomm.DiscardOutBuffer();
			mycomm.Write(outData, 0, 1);
			int delay;
			for (delay = 0; delay < 5; ++delay)
			{
				Thread.Sleep(3);
				if (mycomm.BytesToWrite == 0)
					break;
			}
			return (delay < 5);
		}

		public bool BeepOn()
		{
			return SendByte((byte)FTDI.SPI_CMD.SPI_BEEP_ON);
		}

		public bool BeepOff()
		{
			return SendByte((byte)FTDI.SPI_CMD.SPI_BEEP_OFF);
		}

		public bool TxLedOn()
		{
			return SendByte((byte)FTDI.SPI_CMD.SPI_TxLED_ON);
		}

		public bool RxLedOn()
		{
			return SendByte((byte)FTDI.SPI_CMD.SPI_RxLED_ON);
		}

		public bool LedOff()
		{
			return SendByte((byte)FTDI.SPI_CMD.SPI_LED_OFF);
		}

		public bool HReset()
		{
			byte[] buffer = new byte[1];
			buffer[0] = (byte)FTDI.SPI_CMD.SPI_HRESET;
			mycomm.DiscardOutBuffer();
			mycomm.DiscardInBuffer();
			mycomm.Write(buffer, 0, 1);
			mycomm.ReadTimeout += 30;
			FTDI.SPI_CMD ack = FTDI.SPI_CMD.SPI_NACK;
			try
			{
				ack = (FTDI.SPI_CMD)(mycomm.ReadByte() & 0xFF);
			}
			catch
			{
				MessageBox.Show("Connection fails, please try again!");
			}
			return (ack == FTDI.SPI_CMD.SPI_ACK);
		}

		public bool LReset()
		{
			byte[] buffer = new byte[1];
			buffer[0] = (byte)FTDI.SPI_CMD.SPI_LRESET;
			mycomm.DiscardOutBuffer();
			mycomm.DiscardInBuffer();
			mycomm.Write(buffer, 0, 1);

			mycomm.ReadTimeout += 30;
			FTDI.SPI_CMD ack = FTDI.SPI_CMD.SPI_NACK;
			try
			{
				ack = (FTDI.SPI_CMD)(mycomm.ReadByte() & 0xFF);
			}
			catch
			{
				MessageBox.Show("Connection fails, please try again!");
			}
			return (ack == FTDI.SPI_CMD.SPI_ACK);
		}

		public bool LCDClear()
		{
			try
			{
				byte[] buffer = new byte[1] { (byte)FTDI.LCD_CMD.LCD_CLEAR };
				mycomm.DiscardInBuffer();
				mycomm.DiscardOutBuffer();
				mycomm.Write(buffer, 0, 1);
				mycomm.ReadTimeout += 100;
				return ((mycomm.ReadByte() & 0xFF) == (byte)FTDI.LCD_CMD.LCD_ACK);
			}
			catch { }
			return false;
		}

		public bool LCDFull()
		{
			try
			{
				byte[] buffer = new byte[1] { (byte)FTDI.LCD_CMD.LCD_FULL };
				mycomm.DiscardInBuffer();
				mycomm.DiscardOutBuffer();
				mycomm.Write(buffer, 0, 1);
				mycomm.ReadTimeout += 100;
				return ((mycomm.ReadByte() & 0xFF) == (byte)FTDI.LCD_CMD.LCD_ACK);
			}
			catch { }
			return false;
		}

		public bool LCDDisplay()
		{
			try
			{
				byte[] buffer = new byte[1] { (byte)FTDI.LCD_CMD.LCD_SHOW };
				mycomm.DiscardInBuffer();
				mycomm.DiscardOutBuffer();
				mycomm.Write(buffer, 0, 1);
				mycomm.ReadTimeout += 100;
				return ((mycomm.ReadByte() & 0xFF) == (byte)FTDI.LCD_CMD.LCD_ACK);
			}
			catch { }
			return false;
		}

		private bool SendByte(byte[] data)
		{
			mycomm.DiscardOutBuffer();
			mycomm.Write(data, 0, data.Length);
			int delay;
			for (delay = 0; delay < 5; ++delay)
			{
				Thread.Sleep(3);
				if (mycomm.BytesToWrite == 0)
					break;
			}
			return (delay < 5);
		}

		public bool LCDSetValue(byte address, byte value)
		{
			return SendByte(new byte[3] { (byte)FTDI.LCD_CMD.LCD_SETVALUE, address, value });
		}

		public bool LCDAndValue(byte address, byte value)
		{
			return SendByte(new byte[3] { (byte)FTDI.LCD_CMD.LCD_ANDVALUE, address, value });
		}

		public bool LCDOrValue(byte address, byte value)
		{
			return SendByte(new byte[3] { (byte)FTDI.LCD_CMD.LCD_ORVALUE, address, value });
		}

		public void RfDataIn()
		{
			mycomm.Write(new byte[1] { (byte)FTDI.HAL_CMD.HAL_DIO2_IN }, 0, 1);
		}

		public void RfDataOut()
		{
			mycomm.Write(new byte[1] { (byte)FTDI.HAL_CMD.HAL_DIO2_OUT }, 0, 1);
		}

		public void RfDataHigh()
		{
			mycomm.Write(new byte[1] { (byte)FTDI.HAL_CMD.HAL_DIO2_H }, 0, 1);
		}

		public void RfDataLow()
		{
			mycomm.Write(new byte[1] { (byte)FTDI.HAL_CMD.HAL_DIO2_L }, 0, 1);
		}

		private enum SPI_CMD
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
		}

		private enum LCD_CMD
		{
			LCD_NACK = 0,
			LCD_CLEAR = 112,
			LCD_SHOW = 113,
			LCD_SETVALUE = 114,
			LCD_ANDVALUE = 115,
			LCD_ORVALUE = 116,
			LCD_ACK = 122,
			LCD_FULL = 127,
		}

		private enum HAL_CMD
		{
			HAL_DIO2_L = 96,
			HAL_DIO2_H = 97,
			HAL_DIO2_OUT = 99,
			HAL_DIO2_IN = 100,
		}
	}
}