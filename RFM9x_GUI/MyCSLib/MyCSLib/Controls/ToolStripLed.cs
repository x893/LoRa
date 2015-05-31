using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace MyCSLib.Controls
{
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
	public class ToolStripLed : ToolStripControlHost
	{
		public Led led
		{
			get
			{
				return this.Control as Led;
			}
		}

		public bool Checked
		{
			get
			{
				return this.led.Checked;
			}
			set
			{
				this.led.Checked = value;
			}
		}

		public Color LedColor
		{
			get
			{
				return this.led.LedColor;
			}
			set
			{
				this.led.LedColor = value;
			}
		}

		public ContentAlignment LedAlign
		{
			get
			{
				return this.led.LedAlign;
			}
			set
			{
				this.led.LedAlign = value;
			}
		}

		public Size LedSize
		{
			get
			{
				return this.led.LedSize;
			}
			set
			{
				this.led.LedSize = value;
			}
		}

		public ToolStripLed()
			: base((Control)new Led())
		{
		}
	}
}
