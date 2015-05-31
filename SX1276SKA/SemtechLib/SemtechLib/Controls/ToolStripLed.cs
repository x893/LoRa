using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SemtechLib.Controls
{
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
	public class ToolStripLed : ToolStripControlHost
	{
		public Led led
		{
			get
			{
				return Control as Led;
			}
		}

		public bool Checked
		{
			get
			{
				return led.Checked;
			}
			set
			{
				led.Checked = value;
			}
		}

		public Color LedColor
		{
			get
			{
				return led.LedColor;
			}
			set
			{
				led.LedColor = value;
			}
		}

		public ContentAlignment LedAlign
		{
			get
			{
				return led.LedAlign;
			}
			set
			{
				led.LedAlign = value;
			}
		}

		public Size LedSize
		{
			get
			{
				return led.LedSize;
			}
			set
			{
				led.LedSize = value;
			}
		}

		public ToolStripLed()
			: base((Control)new Led())
		{
		}
	}
}
