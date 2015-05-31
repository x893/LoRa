using System;
using System.Management;

namespace SemtechLib.Usb
{
	internal class UsbDeviceEvent
	{
		private string deviceId = "";
		private ManagementEventWatcher creationEventWatcher;
		private ManagementEventWatcher deletionEventWatcher;

		public event EventHandler Attached;

		public event EventHandler Detached;

		public UsbDeviceEvent()
		{
			this.creationEventWatcher = (ManagementEventWatcher)null;
			ManagementOperationObserver operationObserver = new ManagementOperationObserver();
			ManagementScope scope = new ManagementScope("root\\CIMV2");
			scope.Options.EnablePrivileges = true;
			try
			{
				WqlEventQuery wqlEventQuery = new WqlEventQuery();
				wqlEventQuery.EventClassName = "__InstanceCreationEvent";
				wqlEventQuery.WithinInterval = new TimeSpan(0, 0, 3);
				wqlEventQuery.Condition = "TargetInstance ISA 'Win32_USBControllerDevice'";
				Console.WriteLine(wqlEventQuery.QueryString);
				this.creationEventWatcher = new ManagementEventWatcher(scope, (EventQuery)wqlEventQuery);
				this.creationEventWatcher.EventArrived += new EventArrivedEventHandler(this.creationEventWatcher_EventArrived);
				this.creationEventWatcher.Start();
				wqlEventQuery.EventClassName = "__InstanceDeletionEvent";
				wqlEventQuery.WithinInterval = new TimeSpan(0, 0, 3);
				wqlEventQuery.Condition = "TargetInstance ISA 'Win32_USBControllerdevice'";
				Console.WriteLine(wqlEventQuery.QueryString);
				this.deletionEventWatcher = new ManagementEventWatcher(scope, (EventQuery)wqlEventQuery);
				this.deletionEventWatcher.EventArrived += new EventArrivedEventHandler(this.deletionEventWatcher_EventArrived);
				this.deletionEventWatcher.Start();
			}
			catch
			{
				this.Dispose();
			}
		}

		public UsbDeviceEvent(string deviceId)
			: this()
		{
			this.deviceId = deviceId;
		}

		private void OnAttached()
		{
			if (this.Attached == null)
				return;
			this.Attached((object)this, EventArgs.Empty);
		}

		private void OnDetached()
		{
			if (this.Detached == null)
				return;
			this.Detached((object)this, EventArgs.Empty);
		}

		public void Dispose()
		{
			if (this.creationEventWatcher != null)
				this.creationEventWatcher.Stop();
			if (this.deletionEventWatcher != null)
				this.deletionEventWatcher.Stop();
			this.creationEventWatcher = (ManagementEventWatcher)null;
			this.deletionEventWatcher = (ManagementEventWatcher)null;
		}

		private void creationEventWatcher_EventArrived(object sender, EventArrivedEventArgs e)
		{
			foreach (PropertyData propertyData1 in e.NewEvent.Properties)
			{
				ManagementBaseObject managementBaseObject;
				if ((managementBaseObject = propertyData1.Value as ManagementBaseObject) != null)
				{
					foreach (PropertyData propertyData2 in managementBaseObject.Properties)
					{
						string str = propertyData2.Value as string;
						if (str != null && str.Replace("\\", "").Contains(this.deviceId.Replace("\\", "")))
							this.OnAttached();
					}
				}
			}
		}

		private void deletionEventWatcher_EventArrived(object sender, EventArrivedEventArgs e)
		{
			foreach (PropertyData propertyData1 in e.NewEvent.Properties)
			{
				ManagementBaseObject managementBaseObject;
				if ((managementBaseObject = propertyData1.Value as ManagementBaseObject) != null)
				{
					foreach (PropertyData propertyData2 in managementBaseObject.Properties)
					{
						string str = propertyData2.Value as string;
						if (str != null && str.Replace("\\", "").Contains(this.deviceId.Replace("\\", "")))
							this.OnDetached();
					}
				}
			}
		}
	}
}