using Microsoft.Win32.SafeHandles;
using SemtechLib.Devices.Common;
using SemtechLib.Devices.Common.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Interop;

namespace SemtechLib.Devices.Common.Hid
{
	public class HidDevice : IDisposable, INotifyPropertyChanged
	{
		public event EventHandler Opened;
		public event EventHandler Closed;
		public event PropertyChangedEventHandler PropertyChanged;

		private object syncObject = new object();
		private string Name = string.Empty;
		private string deviceID = string.Empty;
		private Guid guid = new Guid(0x4D1E55B2, 0xF16F, 0x11CF, (byte)136, (byte)203, (byte)0, (byte)17, (byte)17, (byte)0, (byte)0, (byte)48);
		private string manufacturer = string.Empty;
		private string product = string.Empty;
		private UsbDetector usbDetector;
		private List<HidDevice.DeviceData> devicesList;
		private SafeFileHandle deviceStream;
		private bool isOpen;
		private int vendorId;
		private int productId;
		private FileStream fileStream;

		public HidDevice(int vendorId, int productId, string product)
		{
			this.vendorId = vendorId;
			this.productId = productId;
			this.product = product;

			deviceID = string.Format(CultureInfo.InvariantCulture,
				"vid_{0:x4}&pid_{1:x4}",
				vendorId,
				productId
			);
			usbDetector = new UsbDetector();
			usbDetector.StateChanged += new DeviceStateEventHandler(usbDetector_StateChanged);
		}

		public bool IsOpen
		{
			get
			{
				return isOpen;
			}
			private set
			{
				isOpen = value;
				OnPropertyChanged("IsOpen");
			}
		}

		public int VendorId
		{
			get { return vendorId; }
			private set { vendorId = value; }
		}

		public int ProductId
		{
			get { return productId; }
			private set { productId = value; }
		}

		public string Manufacturer
		{
			get { return manufacturer; }
			private set { manufacturer = value; }
		}

		public string Product
		{
			get { return product; }
			private set { product = value; }
		}

		private void OnOpened()
		{
			if (Opened != null)
				Opened(this, EventArgs.Empty);
		}

		private void OnClosed()
		{
			if (Closed != null)
				Closed(this, EventArgs.Empty);
		}

		private static List<HidDevice.DeviceData> PopulateDeviceList(int vendorId, int productId)
		{
			SafeFileHandle safeFileHandle = (SafeFileHandle)null;
			List<HidDevice.DeviceData> list = new List<HidDevice.DeviceData>();
			Guid gHid;
			SemtechLib.Devices.Common.NativeMethods.HidD_GetHidGuid(out gHid);
			IntPtr classDevs = SemtechLib.Devices.Common.NativeMethods.SetupDiGetClassDevs(ref gHid, IntPtr.Zero, IntPtr.Zero, 18);
			string vid_pid = string.Format((IFormatProvider)CultureInfo.InvariantCulture,
				"vid_{0:x4}&pid_{1:x4}",
				vendorId,
				productId
				);
			try
			{
				SemtechLib.Devices.Common.NativeMethods.SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new SemtechLib.Devices.Common.NativeMethods.SP_DEVICE_INTERFACE_DATA();
				deviceInterfaceData.cbSize = Marshal.SizeOf(deviceInterfaceData);
				int deviceIndex = 0;
				while (SemtechLib.Devices.Common.NativeMethods.SetupDiEnumDeviceInterfaces(classDevs, IntPtr.Zero, ref gHid, deviceIndex++, ref deviceInterfaceData))
				{	// Scan device path for VIS/PID
					string devicePath = HidDevice.GetDevicePath(classDevs, ref deviceInterfaceData);
					if (devicePath.IndexOf(vid_pid) >= 0)
					{
						if (safeFileHandle != null)
							safeFileHandle.Close();
						safeFileHandle = SemtechLib.Devices.Common.NativeMethods.CreateFile(devicePath, -1073741824, 0, IntPtr.Zero, 3, 0, IntPtr.Zero);
						if (Marshal.GetLastWin32Error() == 0)
						{
							StringBuilder lpBuffer = new StringBuilder();
							if (SemtechLib.Devices.Common.NativeMethods.HidD_GetManufacturerString(safeFileHandle, lpBuffer, 255))
							{
								StringBuilder lpbuffer = new StringBuilder();
								if (SemtechLib.Devices.Common.NativeMethods.HidD_GetProductString(safeFileHandle, lpbuffer, 255))
									list.Add(new HidDevice.DeviceData(devicePath, lpBuffer.ToString(), lpbuffer.ToString()));
							}
						}
					}
				}
				return list;
			}
			finally
			{
				if (safeFileHandle != null)
					safeFileHandle.Close();
				SemtechLib.Devices.Common.NativeMethods.SetupDiDestroyDeviceInfoList(classDevs);
			}
		}

		private static string GetDevicePath(IntPtr DeviceInfoTable, ref SemtechLib.Devices.Common.NativeMethods.SP_DEVICE_INTERFACE_DATA InterfaceDataStructure)
		{
			int RequiredSize = 0;

			SemtechLib.Devices.Common.NativeMethods.SP_DEVICE_INTERFACE_DETAIL_DATA interfaceDetailData = new SemtechLib.Devices.Common.NativeMethods.SP_DEVICE_INTERFACE_DETAIL_DATA();
			interfaceDetailData.cbSize = Marshal.SizeOf(interfaceDetailData);
			SemtechLib.Devices.Common.NativeMethods.SetupDiGetDeviceInterfaceDetail(DeviceInfoTable, ref InterfaceDataStructure, IntPtr.Zero, 0, ref RequiredSize, IntPtr.Zero);
			IntPtr hDevice = Marshal.AllocHGlobal(RequiredSize);
			interfaceDetailData.cbSize = IntPtr.Size != 8 ? 4 + Marshal.SystemDefaultCharSize : 8;
			Marshal.StructureToPtr(interfaceDetailData, hDevice, false);
			if (SemtechLib.Devices.Common.NativeMethods.SetupDiGetDeviceInterfaceDetail(DeviceInfoTable, ref InterfaceDataStructure, hDevice, RequiredSize, IntPtr.Zero, IntPtr.Zero))
			{
				string devicePath = Marshal.PtrToStringUni(new IntPtr(hDevice.ToInt32() + 4));
				Marshal.FreeHGlobal(hDevice);
				return devicePath;
			}
			Marshal.FreeHGlobal(hDevice);
			return string.Empty;
		}

		private void OnAttached(string name)
		{
			devicesList = HidDevice.PopulateDeviceList(vendorId, productId);
			foreach (HidDevice.DeviceData deviceData in devicesList)
			{
				if (!IsOpen
					&& (string.IsNullOrEmpty(name) || name.ToLower() == deviceData.Name)
					&& product == deviceData.Product)
				{
					deviceStream = SemtechLib.Devices.Common.NativeMethods.CreateFile(deviceData.Name, -1073741824, 3, IntPtr.Zero, 3, 0, IntPtr.Zero);
					if (Marshal.GetLastWin32Error() == 0)
					{
						fileStream = new FileStream(deviceStream, FileAccess.ReadWrite, 65, false);
						Name = deviceData.Name;
						IsOpen = true;
						OnOpened();
						break;
					}
				}
			}
		}

		private void OnUnattached(string name)
		{
			if (!IsOpen || name.ToLower() != Name)
				return;
			if (fileStream != null)
			{
				fileStream.Close();
				Console.WriteLine("HidDevice: filestream closed");
			}
			if (deviceStream != null && !deviceStream.IsInvalid)
			{
				deviceStream.Close();
				Console.WriteLine("HidDevice: devicestream closed");
			}
			Name = string.Empty;
			IsOpen = false;
			OnClosed();
		}

		public bool Open()
		{
			OnAttached(string.Empty);
			return IsOpen;
		}

		public bool Close()
		{
			OnUnattached(Name);
			return !IsOpen;
		}

		public void RegisterNotification(IntPtr handle)
		{
			RegisterNotification(handle, false);
		}

		public void RegisterNotification(IntPtr handle, bool isWpfApplication)
		{
			if (isWpfApplication)
			{
				HwndSource source = HwndSource.FromHwnd(handle);
				HwndSourceHook hook = new HwndSourceHook(NotificationHandler);
				source.AddHook(hook);
			}
			usbDetector.RegisterNotification(handle);
		}

		public void UnregisterNotification()
		{
			usbDetector.UnregisterNotification();
		}

		public void ProcessWinMessage(int msg, IntPtr wParam, IntPtr lParam)
		{
			usbDetector.ProcessWinMessage(msg, wParam, lParam);
		}

		public IntPtr NotificationHandler(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			return usbDetector.NotificationHandler(hwnd, msg, wParam, lParam);
		}

		private int ReadAsync(byte[] array, int offset, int count, int timeout)
		{
			ManualResetEvent manualResetEvent = new ManualResetEvent(false);
			IAsyncResult asyncResult = fileStream.BeginRead(array, offset, count, new AsyncCallback(HidDevice.OnReadCompletion), manualResetEvent);
			manualResetEvent.WaitOne(timeout, false);
			if (asyncResult.IsCompleted)
				return fileStream.EndRead(asyncResult);
			return 0;
		}

		private static void OnReadCompletion(IAsyncResult asyncResult)
		{
			(asyncResult.AsyncState as ManualResetEvent).Set();
		}

		private void WriteAsync(byte[] array, int offset, int count, int timeout)
		{
			ManualResetEvent manualResetEvent = new ManualResetEvent(false);
			IAsyncResult asyncResult = fileStream.BeginWrite(array, offset, count, new AsyncCallback(HidDevice.OnWriteCompletion), manualResetEvent);
			manualResetEvent.WaitOne(timeout, false);
			if (!asyncResult.IsCompleted)
				throw new Exception("Write timeout");
			fileStream.EndWrite(asyncResult);
		}

		private static void OnWriteCompletion(IAsyncResult asyncResult)
		{
			(asyncResult.AsyncState as ManualResetEvent).Set();
		}

		/// <summary>
		/// Request:	[0][Command][outData...]
		/// Response:	[0][Command][inData...]
		/// </summary>
		/// <param name="command"></param>
		/// <param name="outData"></param>
		/// <param name="inData"></param>
		/// <returns></returns>
		public bool TxRxCommand(byte command, byte[] outData, ref byte[] inData)
		{
			byte[] request = new byte[65];
			byte[] response = new byte[65];
			try
			{
				lock (syncObject)
				{
					if (IsOpen && (fileStream.CanWrite || fileStream.CanRead))
					{
						for (int idx = 0; idx < request.Length; ++idx)
							request[idx] = 0xFF;

						request[0] = 0;
						request[1] = command;
						if (outData != null)
							Array.Copy(outData, 0, request, 2, outData.Length);
						fileStream.Write(request, 0, request.Length);

						if (inData == null)
							return true;
						response[0] = 0;
						if (fileStream.Read(response, 0, response.Length) > 0 && response[1] == command)
						{
							Array.Copy(response, 2, inData, 0, inData.Length);
							return true;
						}
					}
					return false;
				}
			}
			catch (IOException ex)
			{
				Console.WriteLine(ex.Message);
				OnUnattached(Name);
				return false;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		private void usbDetector_StateChanged(object sender, DeviceStateEventArg e)
		{
			switch (e.State)
			{
				case DeviceState.Attached:
					OnAttached(e.Name);
					break;
				case DeviceState.Unattached:
					OnUnattached(e.Name);
					break;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
				return;
			usbDetector.UnregisterNotification();
		}

		private void OnPropertyChanged(string propName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		public struct DeviceData
		{
			public string Name;
			public string Manufacturer;
			public string Product;

			public DeviceData(string name, string manufacturer, string product)
			{
				Name = name;
				Manufacturer = manufacturer;
				Product = product;
			}
		}
	}
}
