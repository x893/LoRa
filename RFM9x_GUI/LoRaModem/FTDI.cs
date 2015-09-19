using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace LoRaModem
{
	public class FTDI
	{
		private object m_lock = new object();
		private IContainer components;
		private SerialPort m_comm;

		public FTDI()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			components = (IContainer)new Container();
			m_comm = new SerialPort(components);
		}

		public bool Open()
		{
			try
			{
				byte[] buffer = new byte[1] { (byte)FTDI.SPI_CMD.SPI_QUERY };
				foreach (string port in SerialPort.GetPortNames())
				{
					m_comm.PortName = port;
					if (!m_comm.IsOpen)
					{
						m_comm.BaudRate = 115200;
						m_comm.Parity = Parity.None;
						m_comm.Open();
						m_comm.Write(buffer, 0, 1);
						Thread.Sleep(100);
						if (m_comm.ReadExisting().Contains("HopeRF LoRa"))
							return true;
						m_comm.Close();
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
			if (m_comm.PortName == null || !m_comm.IsOpen)
				return false;
			m_comm.Close();
			return true;
		}

		public bool SendByte(byte address, byte data)
		{
			lock (m_lock)
			{
				byte[] outData = new byte[3] { (byte)FTDI.SPI_CMD.SPI_WRITE, address, data };
				m_comm.DiscardOutBuffer();
				m_comm.Write(outData, 0, 3);
				int delay;
				for (delay = 0; delay < 5; ++delay)
				{
					Thread.Sleep(3);
					if (m_comm.BytesToWrite == 0)
						break;
				}
				return delay < 5;
			}
		}

		public bool SendBytes(byte address, byte[] data, int length)
		{
			lock (m_lock)
			{
				byte[] outData = new byte[(int)length + 2];
				outData[0] = (byte)FTDI.SPI_CMD.SPI_BURST_WRITE;
				outData[1] = address;
				for (int idx = 2; idx < length + 2; ++idx)
					outData[idx] = data[idx - 2];

				m_comm.DiscardOutBuffer();
				m_comm.Write(outData, 0, length + 2);
				int delay;
				for (delay = 0; delay < length; ++delay)
				{
					Thread.Sleep(5);
					if (m_comm.BytesToWrite == 0)
						break;
				}
				if (delay >= length)
					return false;
				m_comm.ReadTimeout += length + 100;
				FTDI.SPI_CMD ack;
				try
				{
					ack = (FTDI.SPI_CMD)(m_comm.ReadByte() & 0xFF);
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
			lock (m_lock)
			{
				try
				{
					byte[] outData = new byte[3] { (byte)FTDI.SPI_CMD.SPI_READ, address, 0xFF };
					m_comm.DiscardOutBuffer();
					m_comm.DiscardInBuffer();
					m_comm.Write(outData, 0, 3);
					int delay;
					for (delay = 0; delay < 5; ++delay)
					{
						Thread.Sleep(3);
						if (m_comm.BytesToWrite == 0)
							break;
					}
					if (delay >= 5)
						return false;
					m_comm.ReadTimeout += 10;
					data = (byte)(m_comm.ReadByte() & 0xFF);
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
			lock (m_lock)
			{
				try
				{
					byte[] outData = new byte[3] { (byte)FTDI.SPI_CMD.SPI_BURST_READ, address, (byte)length };
					m_comm.DiscardInBuffer();
					m_comm.DiscardOutBuffer();
					m_comm.Write(outData, 0, 3);
					int delay;
					for (delay = 0; delay < 5; ++delay)
					{
						Thread.Sleep(3);
						if (m_comm.BytesToWrite == 0)
							break;
					}
					if (delay >= 5)
						return false;
					Thread.Sleep(length + 10);
					m_comm.ReadTimeout += 20;
					m_comm.Read(data, 0, length);
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
			m_comm.DiscardOutBuffer();
			m_comm.Write(outData, 0, 1);
			int delay;
			for (delay = 0; delay < 5; ++delay)
			{
				Thread.Sleep(3);
				if (m_comm.BytesToWrite == 0)
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
			m_comm.DiscardOutBuffer();
			m_comm.DiscardInBuffer();
			m_comm.Write(buffer, 0, 1);
			m_comm.ReadTimeout += 30;
			FTDI.SPI_CMD ack = FTDI.SPI_CMD.SPI_NACK;
			try
			{
				ack = (FTDI.SPI_CMD)(m_comm.ReadByte() & 0xFF);
			}
			catch
			{
				MessageBox.Show("Connection fails, please try again!");
			}
			return (ack == FTDI.SPI_CMD.SPI_ACK);
		}

		public bool LReset()
		{
			byte[] buffer = new byte[] { (byte)FTDI.SPI_CMD.SPI_LRESET };
			m_comm.DiscardOutBuffer();
			m_comm.DiscardInBuffer();
			m_comm.Write(buffer, 0, 1);

			m_comm.ReadTimeout += 30;
			FTDI.SPI_CMD ack = FTDI.SPI_CMD.SPI_NACK;
			try
			{
				ack = (FTDI.SPI_CMD)(m_comm.ReadByte() & 0xFF);
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
				m_comm.DiscardInBuffer();
				m_comm.DiscardOutBuffer();
				m_comm.Write(buffer, 0, 1);
				m_comm.ReadTimeout += 100;
				return ((m_comm.ReadByte() & 0xFF) == (int)FTDI.LCD_CMD.LCD_ACK);
			}
			catch { }
			return false;
		}

		public bool LCDFull()
		{
			try
			{
				byte[] buffer = new byte[1] { (byte)FTDI.LCD_CMD.LCD_FULL };
				m_comm.DiscardInBuffer();
				m_comm.DiscardOutBuffer();
				m_comm.Write(buffer, 0, 1);
				m_comm.ReadTimeout += 100;
				return ((m_comm.ReadByte() & 0xFF) == (int)FTDI.LCD_CMD.LCD_ACK);
			}
			catch { }
			return false;
		}

		public bool LCDDisplay()
		{
			try
			{
				byte[] buffer = new byte[1] { (byte)FTDI.LCD_CMD.LCD_SHOW };
				m_comm.DiscardInBuffer();
				m_comm.DiscardOutBuffer();
				m_comm.Write(buffer, 0, 1);
				m_comm.ReadTimeout += 100;
				return ((m_comm.ReadByte() & 0xFF) == (int)FTDI.LCD_CMD.LCD_ACK);
			}
			catch { }
			return false;
		}

		private bool SendByte(byte[] data)
		{
			m_comm.DiscardOutBuffer();
			m_comm.Write(data, 0, data.Length);
			int delay;
			for (delay = 0; delay < 5; ++delay)
			{
				Thread.Sleep(3);
				if (m_comm.BytesToWrite == 0)
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
			m_comm.Write(new byte[1] { (byte)FTDI.HAL_CMD.HAL_DIO2_IN }, 0, 1);
		}

		public void RfDataOut()
		{
			m_comm.Write(new byte[1] { (byte)FTDI.HAL_CMD.HAL_DIO2_OUT }, 0, 1);
		}

		public void RfDataHigh()
		{
			m_comm.Write(new byte[1] { (byte)FTDI.HAL_CMD.HAL_DIO2_H }, 0, 1);
		}

		public void RfDataLow()
		{
			m_comm.Write(new byte[1] { (byte)FTDI.HAL_CMD.HAL_DIO2_L }, 0, 1);
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
