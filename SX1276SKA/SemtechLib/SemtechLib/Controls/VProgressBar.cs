using System.Windows.Forms;

namespace SemtechLib.Controls
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
