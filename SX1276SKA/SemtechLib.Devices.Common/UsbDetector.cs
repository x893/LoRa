using SemtechLib.Devices.Common.Events;
using System;
using System.Runtime.InteropServices;

namespace SemtechLib.Devices.Common
{
	public class UsbDetector
	{
		private IntPtr handle = IntPtr.Zero;
		private Guid USBClassID = new Guid(1293833650U, (ushort)61807, (ushort)4559, (byte)136, (byte)203, (byte)0, (byte)17, (byte)17, (byte)0, (byte)0, (byte)48);

		public event DeviceStateEventHandler StateChanged;

		private void OnStateChanged(string name, DeviceState state)
		{
			if (this.StateChanged == null)
				return;
			this.StateChanged((object)this, new DeviceStateEventArg(name, state));
		}

		private string GetDeviceName(IntPtr lParam)
		{
			string str = string.Empty;
			NativeMethods.DEV_BROADCAST_HDR devBroadcastHdr = (NativeMethods.DEV_BROADCAST_HDR)Marshal.PtrToStructure(lParam, typeof(NativeMethods.DEV_BROADCAST_HDR));
			if (devBroadcastHdr.dbch_devicetype == 5)
			{
				int length = Convert.ToInt32((devBroadcastHdr.dbch_size - 32) / 2);
				NativeMethods.DEV_BROADCAST_DEVICEINTERFACE broadcastDeviceinterface;
				broadcastDeviceinterface.dbcc_name = new char[length + 1];
				str = new string(((NativeMethods.DEV_BROADCAST_DEVICEINTERFACE)Marshal.PtrToStructure(lParam, typeof(NativeMethods.DEV_BROADCAST_DEVICEINTERFACE))).dbcc_name, 0, length);
			}
			return str;
		}

		public IntPtr NotificationHandler(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
		{
			this.ProcessWinMessage(msg, wParam, lParam);
			return IntPtr.Zero;
		}

		public void ProcessWinMessage(int msg, IntPtr wParam, IntPtr lParam)
		{
			if (msg != 537)
				return;
			Console.WriteLine("UsbDetector\t:\tmsg = 0x{0} - wParam = 0x{1} - lParam = 0x{2}", (object)msg.ToString("X"), (object)wParam.ToInt32().ToString("X"), (object)lParam.ToInt32().ToString("X"));
			switch (wParam.ToInt32())
			{
				case 24:
					Console.WriteLine("UsbDetector\t:Device config changed");
					break;
				case 32768:
					Console.WriteLine("UsbDetector\t:Device arrival");
					Console.WriteLine("DEVICE INTERFACE: Arrived");
					this.OnStateChanged(this.GetDeviceName(lParam), DeviceState.Attached);
					break;
				case 32771:
					Console.WriteLine("UsbDetector\t:Device remove pending");
					break;
				case 32772:
					Console.WriteLine("UsbDetector\t:Device remove complete");
					Console.WriteLine("DEVICE INTERFACE: Removed");
					this.OnStateChanged(this.GetDeviceName(lParam), DeviceState.Unattached);
					break;
			}
		}

		public IntPtr RegisterNotification(IntPtr handle)
		{
			this.handle = handle;
			NativeMethods.DEV_BROADCAST_DEVICEINTERFACE broadcastDeviceinterface = new NativeMethods.DEV_BROADCAST_DEVICEINTERFACE();
			broadcastDeviceinterface.dbcc_devicetype = 5;
			broadcastDeviceinterface.dbcc_size = Marshal.SizeOf((object)broadcastDeviceinterface);
			broadcastDeviceinterface.dbcc_reserved = 0;
			broadcastDeviceinterface.dbcc_classguid = this.USBClassID;
			IntPtr num1 = IntPtr.Zero;
			IntPtr num2 = Marshal.AllocHGlobal(Marshal.SizeOf((object)broadcastDeviceinterface));
			Marshal.StructureToPtr((object)broadcastDeviceinterface, num2, false);
			IntPtr num3 = IntPtr.Zero;
			return NativeMethods.RegisterDeviceNotification(handle, num2, 0);
		}

		public bool UnregisterNotification()
		{
			return NativeMethods.UnregisterDeviceNotification(this.handle);
		}
	}
}
