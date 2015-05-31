using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SemtechLib.Devices.Common
{
	public sealed class NativeMethods
	{
		public const int DIGCF_PRESENT = 2;
		public const int DIGCF_DEVICEINTERFACE = 16;
		public const int FILE_ATTRIBUTE_NORMAL = 128;
		public const int FILE_FLAG_OVERLAPPED = 1073741824;
		public const int INVALID_HANDLE_VALUE = -1;
		public const int GENERIC_READ = -2147483648;
		public const int GENERIC_WRITE = 1073741824;
		public const int CREATE_NEW = 1;
		public const int CREATE_ALWAYS = 2;
		public const int OPEN_EXISTING = 3;
		public const int FILE_SHARE_READ = 1;
		public const int FILE_SHARE_WRITE = 2;
		public const int WM_DEVICECHANGE = 537;
		public const int DBT_DEVICEARRIVAL = 32768;
		public const int DBT_DEVICEQUERYREMOVE = 32769;
		public const int DBT_DEVICEQUERYREMOVEFAILED = 32770;
		public const int DBT_DEVICEREMOVEPENDING = 32771;
		public const int DBT_DEVICEREMOVECOMPLETE = 32772;
		public const int DBT_DEVICETYPESPECIFIC = 32773;
		public const int DBT_CUSTOMEVENT = 32774;
		public const int DBT_DEVNODES_CHANGED = 7;
		public const int DBT_QUERYCHANGECONFIG = 23;
		public const int DBT_CONFIGCHANGED = 24;
		public const int DBT_CONFIGCHANGECANCELED = 25;
		public const int DBT_USERDEFINED = 65535;
		public const int DBT_DEVTYP_DEVICEINTERFACE = 5;
		public const int DEVICE_NOTIFY_WINDOW_HANDLE = 0;
		public const int DEVICE_NOTIFY_SERVICE_HANDLE = 1;
		public const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;
		public const int ERROR_SUCCESS = 0;
		public const int ERROR_NO_MORE_ITEMS = 259;
		public const int SPDRP_DEVICEDESC = 0;
		public const int SPDRP_HARDWAREID = 1;
		public const int SPDRP_FRIENDLYNAME = 12;

		private NativeMethods()
		{
		}

		[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, int Flags);

		[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, IntPtr DeviceInfoData, ref Guid InterfaceClassGuid, int MemberIndex, ref NativeMethods.SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

		[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

		[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, int MemberIndex, ref NativeMethods.SP_DEVINFO_DATA DeviceInterfaceData);

		[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetupDiGetDeviceRegistryProperty(IntPtr DeviceInfoSet, ref NativeMethods.SP_DEVINFO_DATA DeviceInfoData, int Property, ref int PropertyRegDataType, IntPtr PropertyBuffer, int PropertyBufferSize, ref int RequiredSize);

		[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref NativeMethods.SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize, ref int RequiredSize, IntPtr DeviceInfoData);

		[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref NativeMethods.SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize, IntPtr RequiredSize, IntPtr DeviceInfoData);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, int Flags);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UnregisterDeviceNotification(IntPtr handle);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode, IntPtr lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool WriteFile(SafeFileHandle hFile, byte[] lpBuffer, int nNumberOfBytesToWrite, ref int lpNumberOfBytesWritten, IntPtr lpOverlapped);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ReadFile(SafeFileHandle hFile, IntPtr lpBuffer, int nNumberOfBytesToRead, ref int lpNumberOfBytesRead, IntPtr lpOverlapped);

		[DllImport("hid.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern void HidD_GetHidGuid(out Guid gHid);

		[DllImport("hid.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool HidD_GetAttributes(SafeFileHandle HidDeviceObject, ref NativeMethods.HIDD_ATTRIBUTES Attributes);

		[DllImport("hid.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool HidD_SetOutputReport(IntPtr HidDeviceObject, byte[] lpReportBuffer, int ReportBufferLength);

		[DllImport("hid.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool HidD_GetProductString(SafeFileHandle hidDeviceObject, StringBuilder lpbuffer, int bufferLength);

		[DllImport("hid.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool HidD_GetManufacturerString(SafeFileHandle HidDeviceObject, StringBuilder lpBuffer, int BufferLength);

		public struct SP_DEVICE_INTERFACE_DATA
		{
			public int cbSize;
			public Guid InterfaceClassGuid;
			public int Flags;
			public IntPtr Reserved;
		}

		public struct SP_DEVICE_INTERFACE_DETAIL_DATA
		{
			public int cbSize;
			public char[] DevicePath;
		}

		public struct SP_DEVINFO_DATA
		{
			public int cbSize;
			public Guid ClassGuid;
			public int DevInst;
			public IntPtr Reserved;
		}

		public struct DEV_BROADCAST_HDR
		{
			public int dbch_size;
			public int dbch_devicetype;
			public int dbch_reserved;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct DEV_BROADCAST_DEVICEINTERFACE
		{
			public int dbcc_size;
			public int dbcc_devicetype;
			public int dbcc_reserved;
			public Guid dbcc_classguid;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
			public char[] dbcc_name;
		}

		public struct HIDD_ATTRIBUTES
		{
			public int Size;
			public short VendorID;
			public short ProductID;
			public short VersionNumber;
		}
	}
}
