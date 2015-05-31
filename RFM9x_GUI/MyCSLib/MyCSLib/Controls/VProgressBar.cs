using System.Windows.Forms;

namespace MyCSLib.Controls
{
	internal class VProgressBar : ProgressBar
	{
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				createParams.Style |= 4;
				return createParams;
			}
		}
	}
}
