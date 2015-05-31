using System;

namespace MyCSLib.General
{
	public class EngineeringNotation
	{
		public static string ToString(double d, ref string unit)
		{
			double d1 = Math.Log10(Math.Abs(d));
			if (Math.Abs(d) >= 1.0)
			{
				switch ((int)Math.Floor(d1))
				{
					case 0:
					case 1:
					case 2:
						return d.ToString("F3");
					case 3:
					case 4:
					case 5:
						unit = "k" + unit;
						return (d / 1000.0).ToString("F3");
					case 6:
					case 7:
					case 8:
						unit = "M" + unit;
						return (d / 1000000.0).ToString("F3");
					case 9:
					case 10:
					case 11:
						unit = "G" + unit;
						return (d / 1000000000.0).ToString("F3");
					case 12:
					case 13:
					case 14:
						unit = "T" + unit;
						return (d / 1000000000000.0).ToString("F3");
					case 15:
					case 16:
					case 17:
						unit = "P" + unit;
						return (d / 1E+15).ToString("F3");
					case 18:
					case 19:
					case 20:
						unit = "E" + unit;
						return (d / 1E+18).ToString("F3");
					case 21:
					case 22:
					case 23:
						unit = "Z" + unit;
						return (d / 1E+21).ToString("F3");
					default:
						unit = "Y" + unit;
						return (d / 1E+24).ToString("F3");
				}
			}
			else
			{
				if (Math.Abs(d) <= 0.0)
					return "0.000";
				switch ((int)Math.Floor(d1))
				{
					case -21:
					case -20:
					case -19:
						unit = "z" + unit;
						return (d * 1E+15).ToString("F3");
					case -18:
					case -17:
					case -16:
						unit = "a" + unit;
						return (d * 1E+15).ToString("F3");
					case -15:
					case -14:
					case -13:
						unit = "f" + unit;
						return (d * 1E+15).ToString("F3");
					case -12:
					case -11:
					case -10:
						unit = "p" + unit;
						return (d * 1000000000000.0).ToString("F3");
					case -9:
					case -8:
					case -7:
						unit = "n" + unit;
						return (d * 1000000000.0).ToString("F3");
					case -6:
					case -5:
					case -4:
						unit = "μ" + unit;
						return (d * 1000000.0).ToString("F3");
					case -3:
					case -2:
					case -1:
						unit = "m" + unit;
						return (d * 1000.0).ToString("F3");
					default:
						unit = "y" + unit;
						return (d * 1E+15).ToString("F3");
				}
			}
		}
	}
}
