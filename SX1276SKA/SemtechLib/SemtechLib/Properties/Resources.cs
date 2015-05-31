using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace SemtechLib.Properties
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[CompilerGenerated]
	[DebuggerNonUserCode]
	public class Resources
	{
		private static ResourceManager resourceMan;
		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals((object)Resources.resourceMan, (object)null))
					Resources.resourceMan = new ResourceManager("SemtechLib.Properties.Resources", typeof(Resources).Assembly);
				return Resources.resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		public static Bitmap About
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("About", Resources.resourceCulture);
			}
		}

		public static Bitmap Auto
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("Auto", Resources.resourceCulture);
			}
		}

		public static Bitmap AutoSelected
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("AutoSelected", Resources.resourceCulture);
			}
		}

		public static Bitmap Burn
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("Burn", Resources.resourceCulture);
			}
		}

		public static Bitmap Connected
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("Connected", Resources.resourceCulture);
			}
		}

		public static Bitmap CopyHS
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("CopyHS", Resources.resourceCulture);
			}
		}

		public static Bitmap CutHS
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("CutHS", Resources.resourceCulture);
			}
		}

		public static Bitmap Disconnected
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("Disconnected", Resources.resourceCulture);
			}
		}

		public static Bitmap ExpirationHS
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("ExpirationHS", Resources.resourceCulture);
			}
		}

		public static Bitmap graphhs
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("graphhs", Resources.resourceCulture);
			}
		}

		public static Bitmap Help
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("Help", Resources.resourceCulture);
			}
		}

		public static Bitmap Move
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("Move", Resources.resourceCulture);
			}
		}

		public static Bitmap MoveSelected
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("MoveSelected", Resources.resourceCulture);
			}
		}

		public static Bitmap openHS
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("openHS", Resources.resourceCulture);
			}
		}

		public static Bitmap PasteHS
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("PasteHS", Resources.resourceCulture);
			}
		}

		public static Bitmap Refresh
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("Refresh", Resources.resourceCulture);
			}
		}

		public static Bitmap RefreshDocViewHS
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("RefreshDocViewHS", Resources.resourceCulture);
			}
		}

		public static Bitmap SaveAllHS
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("SaveAllHS", Resources.resourceCulture);
			}
		}

		public static Bitmap saveHS
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("saveHS", Resources.resourceCulture);
			}
		}

		public static Icon semtech
		{
			get
			{
				return (Icon)Resources.ResourceManager.GetObject("semtech", Resources.resourceCulture);
			}
		}

		public static Bitmap semtech_16_16
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("semtech_16_16", Resources.resourceCulture);
			}
		}

		public static Bitmap Tune
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("Tune", Resources.resourceCulture);
			}
		}

		public static Bitmap ZoomIn
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("ZoomIn", Resources.resourceCulture);
			}
		}

		public static Bitmap ZoomInSelected
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("ZoomInSelected", Resources.resourceCulture);
			}
		}

		public static Bitmap ZoomOut
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("ZoomOut", Resources.resourceCulture);
			}
		}

		public static Bitmap ZoomOutSelected
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("ZoomOutSelected", Resources.resourceCulture);
			}
		}

		internal Resources()
		{
		}
	}
}
