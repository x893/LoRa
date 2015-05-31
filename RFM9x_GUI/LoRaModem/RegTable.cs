using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LoRaModem
{
	public class RegTable : Form
	{
		public RegTable()
		{
			InitializeComponent();
		}

		#region InitializeComponent
		private IContainer components;
		public TextBox tbRegPllHf;
		private Label label81;
		private Label label72;
		public TextBox tbRegPaDac;
		public TextBox tbRegTcxo;
		public TextBox tbRegModemConfig3;
		private Label label71;
		private Label label69;
		private Label label70;
		private Label label80;
		private Label label82;
		private Label label79;
		public TextBox tbRegPreambleLsb;
		public TextBox tbRegPreambleMsb;
		public TextBox tbRegSymbTimeoutLsb;
		private Label label74;
		private Label label73;
		private Label label66;
		private Label label46;
		private Label label45;
		private Label label44;
		public TextBox tbRegIrqFlagsMask;
		private Label label24;
		private Label label15;
		private Label label42;
		public TextBox tbRegModemConfig1;
		public TextBox tbRegModemConfig2;
		private Label label64;
		private Label label65;
		private Label label43;
		private Label label2;
		private Label label57;
		private Label label56;
		public TextBox tbRegLna;
		public TextBox tbRegOcp;
		public TextBox tbRegPaRamp;
		public TextBox tbRegPaConfig;
		public TextBox tbRegFrLsb;
		public TextBox tbRegFrMid;
		public TextBox tbRegFrMsb;
		public TextBox tbRegOpMode;
		private Label label55;
		private Label label39;
		private Label label9;
		private Label label32;
		private Label label37;
		private Label label36;
		private Label label35;
		private Label label34;
		private Label label33;
		private Label label31;
		private Label label30;
		private Label label8;
		private Label label7;
		private Label label6;
		private Label label5;
		private Label label4;
		private Label label3;
		private Label label29;
		private Label label10;
		private TableLayoutPanel tableLayoutPanel1;

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
				this.components.Dispose();
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = (IContainer)new Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegTable));
			this.tbRegPllHf = new System.Windows.Forms.TextBox();
			this.label81 = new System.Windows.Forms.Label();
			this.label72 = new System.Windows.Forms.Label();
			this.tbRegPaDac = new System.Windows.Forms.TextBox();
			this.tbRegTcxo = new System.Windows.Forms.TextBox();
			this.tbRegModemConfig3 = new System.Windows.Forms.TextBox();
			this.label71 = new System.Windows.Forms.Label();
			this.label69 = new System.Windows.Forms.Label();
			this.label70 = new System.Windows.Forms.Label();
			this.label80 = new System.Windows.Forms.Label();
			this.label82 = new System.Windows.Forms.Label();
			this.label79 = new System.Windows.Forms.Label();
			this.tbRegPreambleLsb = new System.Windows.Forms.TextBox();
			this.tbRegPreambleMsb = new System.Windows.Forms.TextBox();
			this.tbRegSymbTimeoutLsb = new System.Windows.Forms.TextBox();
			this.label74 = new System.Windows.Forms.Label();
			this.label73 = new System.Windows.Forms.Label();
			this.label66 = new System.Windows.Forms.Label();
			this.label46 = new System.Windows.Forms.Label();
			this.label45 = new System.Windows.Forms.Label();
			this.label44 = new System.Windows.Forms.Label();
			this.tbRegIrqFlagsMask = new System.Windows.Forms.TextBox();
			this.label24 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.label42 = new System.Windows.Forms.Label();
			this.tbRegModemConfig1 = new System.Windows.Forms.TextBox();
			this.tbRegModemConfig2 = new System.Windows.Forms.TextBox();
			this.label64 = new System.Windows.Forms.Label();
			this.label65 = new System.Windows.Forms.Label();
			this.label43 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label57 = new System.Windows.Forms.Label();
			this.label56 = new System.Windows.Forms.Label();
			this.tbRegLna = new System.Windows.Forms.TextBox();
			this.tbRegOcp = new System.Windows.Forms.TextBox();
			this.tbRegPaRamp = new System.Windows.Forms.TextBox();
			this.tbRegPaConfig = new System.Windows.Forms.TextBox();
			this.tbRegFrLsb = new System.Windows.Forms.TextBox();
			this.tbRegFrMid = new System.Windows.Forms.TextBox();
			this.tbRegFrMsb = new System.Windows.Forms.TextBox();
			this.tbRegOpMode = new System.Windows.Forms.TextBox();
			this.label55 = new System.Windows.Forms.Label();
			this.label39 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label32 = new System.Windows.Forms.Label();
			this.label37 = new System.Windows.Forms.Label();
			this.label36 = new System.Windows.Forms.Label();
			this.label35 = new System.Windows.Forms.Label();
			this.label34 = new System.Windows.Forms.Label();
			this.label33 = new System.Windows.Forms.Label();
			this.label31 = new System.Windows.Forms.Label();
			this.label30 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label29 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tbRegPllHf
			// 
			this.tbRegPllHf.Location = new System.Drawing.Point(575, 264);
			this.tbRegPllHf.Name = "tbRegPllHf";
			this.tbRegPllHf.ReadOnly = true;
			this.tbRegPllHf.Size = new System.Drawing.Size(76, 22);
			this.tbRegPllHf.TabIndex = 11;
			this.tbRegPllHf.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label81
			// 
			this.label81.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label81.AutoSize = true;
			this.label81.Location = new System.Drawing.Point(517, 270);
			this.label81.Name = "label81";
			this.label81.Size = new System.Drawing.Size(31, 15);
			this.label81.TabIndex = 0;
			this.label81.Text = "0x70";
			// 
			// label72
			// 
			this.label72.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label72.AutoSize = true;
			this.label72.Location = new System.Drawing.Point(391, 270);
			this.label72.Name = "label72";
			this.label72.Size = new System.Drawing.Size(49, 15);
			this.label72.TabIndex = 10;
			this.label72.Text = "RegPllHf";
			// 
			// tbRegPaDac
			// 
			this.tbRegPaDac.Location = new System.Drawing.Point(575, 235);
			this.tbRegPaDac.Name = "tbRegPaDac";
			this.tbRegPaDac.ReadOnly = true;
			this.tbRegPaDac.Size = new System.Drawing.Size(76, 22);
			this.tbRegPaDac.TabIndex = 11;
			this.tbRegPaDac.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// tbRegTcxo
			// 
			this.tbRegTcxo.Location = new System.Drawing.Point(575, 206);
			this.tbRegTcxo.Name = "tbRegTcxo";
			this.tbRegTcxo.ReadOnly = true;
			this.tbRegTcxo.Size = new System.Drawing.Size(76, 22);
			this.tbRegTcxo.TabIndex = 11;
			this.tbRegTcxo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// tbRegModemConfig3
			// 
			this.tbRegModemConfig3.Location = new System.Drawing.Point(575, 177);
			this.tbRegModemConfig3.Name = "tbRegModemConfig3";
			this.tbRegModemConfig3.ReadOnly = true;
			this.tbRegModemConfig3.Size = new System.Drawing.Size(76, 22);
			this.tbRegModemConfig3.TabIndex = 11;
			this.tbRegModemConfig3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label71
			// 
			this.label71.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label71.AutoSize = true;
			this.label71.Location = new System.Drawing.Point(386, 239);
			this.label71.Name = "label71";
			this.label71.Size = new System.Drawing.Size(60, 15);
			this.label71.TabIndex = 10;
			this.label71.Text = "RegPaDac";
			// 
			// label69
			// 
			this.label69.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label69.AutoSize = true;
			this.label69.Location = new System.Drawing.Point(388, 210);
			this.label69.Name = "label69";
			this.label69.Size = new System.Drawing.Size(55, 15);
			this.label69.TabIndex = 10;
			this.label69.Text = "RegTcxo ";
			// 
			// label70
			// 
			this.label70.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label70.AutoSize = true;
			this.label70.Location = new System.Drawing.Point(365, 181);
			this.label70.Name = "label70";
			this.label70.Size = new System.Drawing.Size(102, 15);
			this.label70.TabIndex = 10;
			this.label70.Text = "RegModemConfig3 ";
			// 
			// label80
			// 
			this.label80.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label80.AutoSize = true;
			this.label80.Location = new System.Drawing.Point(516, 239);
			this.label80.Name = "label80";
			this.label80.Size = new System.Drawing.Size(33, 15);
			this.label80.TabIndex = 0;
			this.label80.Text = "0x4D";
			// 
			// label82
			// 
			this.label82.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label82.AutoSize = true;
			this.label82.Location = new System.Drawing.Point(517, 210);
			this.label82.Name = "label82";
			this.label82.Size = new System.Drawing.Size(32, 15);
			this.label82.TabIndex = 0;
			this.label82.Text = "0x4B";
			// 
			// label79
			// 
			this.label79.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label79.AutoSize = true;
			this.label79.Location = new System.Drawing.Point(517, 181);
			this.label79.Name = "label79";
			this.label79.Size = new System.Drawing.Size(31, 15);
			this.label79.TabIndex = 0;
			this.label79.Text = "0x26";
			// 
			// tbRegPreambleLsb
			// 
			this.tbRegPreambleLsb.Location = new System.Drawing.Point(575, 148);
			this.tbRegPreambleLsb.Name = "tbRegPreambleLsb";
			this.tbRegPreambleLsb.ReadOnly = true;
			this.tbRegPreambleLsb.Size = new System.Drawing.Size(76, 22);
			this.tbRegPreambleLsb.TabIndex = 11;
			this.tbRegPreambleLsb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// tbRegPreambleMsb
			// 
			this.tbRegPreambleMsb.Location = new System.Drawing.Point(575, 119);
			this.tbRegPreambleMsb.Name = "tbRegPreambleMsb";
			this.tbRegPreambleMsb.ReadOnly = true;
			this.tbRegPreambleMsb.Size = new System.Drawing.Size(76, 22);
			this.tbRegPreambleMsb.TabIndex = 11;
			this.tbRegPreambleMsb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// tbRegSymbTimeoutLsb
			// 
			this.tbRegSymbTimeoutLsb.Location = new System.Drawing.Point(575, 90);
			this.tbRegSymbTimeoutLsb.Name = "tbRegSymbTimeoutLsb";
			this.tbRegSymbTimeoutLsb.ReadOnly = true;
			this.tbRegSymbTimeoutLsb.Size = new System.Drawing.Size(76, 22);
			this.tbRegSymbTimeoutLsb.TabIndex = 11;
			this.tbRegSymbTimeoutLsb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label74
			// 
			this.label74.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label74.AutoSize = true;
			this.label74.Location = new System.Drawing.Point(517, 152);
			this.label74.Name = "label74";
			this.label74.Size = new System.Drawing.Size(31, 15);
			this.label74.TabIndex = 0;
			this.label74.Text = "0x21";
			// 
			// label73
			// 
			this.label73.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label73.AutoSize = true;
			this.label73.Location = new System.Drawing.Point(517, 123);
			this.label73.Name = "label73";
			this.label73.Size = new System.Drawing.Size(31, 15);
			this.label73.TabIndex = 0;
			this.label73.Text = "0x20";
			// 
			// label66
			// 
			this.label66.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label66.AutoSize = true;
			this.label66.Location = new System.Drawing.Point(517, 94);
			this.label66.Name = "label66";
			this.label66.Size = new System.Drawing.Size(32, 15);
			this.label66.TabIndex = 0;
			this.label66.Text = "0x1F";
			// 
			// label46
			// 
			this.label46.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label46.AutoSize = true;
			this.label46.Location = new System.Drawing.Point(371, 152);
			this.label46.Name = "label46";
			this.label46.Size = new System.Drawing.Size(90, 15);
			this.label46.TabIndex = 10;
			this.label46.Text = "RegPreambleLsb";
			// 
			// label45
			// 
			this.label45.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label45.AutoSize = true;
			this.label45.Location = new System.Drawing.Point(369, 123);
			this.label45.Name = "label45";
			this.label45.Size = new System.Drawing.Size(93, 15);
			this.label45.TabIndex = 10;
			this.label45.Text = "RegPreambleMsb";
			// 
			// label44
			// 
			this.label44.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label44.AutoSize = true;
			this.label44.Location = new System.Drawing.Point(361, 94);
			this.label44.Name = "label44";
			this.label44.Size = new System.Drawing.Size(110, 15);
			this.label44.TabIndex = 10;
			this.label44.Text = "RegSymbTimeoutLsb";
			// 
			// tbRegIrqFlagsMask
			// 
			this.tbRegIrqFlagsMask.Location = new System.Drawing.Point(237, 264);
			this.tbRegIrqFlagsMask.Name = "tbRegIrqFlagsMask";
			this.tbRegIrqFlagsMask.ReadOnly = true;
			this.tbRegIrqFlagsMask.Size = new System.Drawing.Size(72, 22);
			this.tbRegIrqFlagsMask.TabIndex = 11;
			this.tbRegIrqFlagsMask.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label24
			// 
			this.label24.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label24.AutoSize = true;
			this.label24.Location = new System.Drawing.Point(179, 270);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(31, 15);
			this.label24.TabIndex = 0;
			this.label24.Text = "0x11";
			// 
			// label15
			// 
			this.label15.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(29, 270);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(97, 15);
			this.label15.TabIndex = 0;
			this.label15.Text = " RegIrqFlagsMask";
			// 
			// label42
			// 
			this.label42.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label42.AutoSize = true;
			this.label42.Location = new System.Drawing.Point(366, 36);
			this.label42.Name = "label42";
			this.label42.Size = new System.Drawing.Size(99, 15);
			this.label42.TabIndex = 10;
			this.label42.Text = "RegModemConfig1";
			// 
			// tbRegModemConfig1
			// 
			this.tbRegModemConfig1.Location = new System.Drawing.Point(575, 32);
			this.tbRegModemConfig1.Name = "tbRegModemConfig1";
			this.tbRegModemConfig1.ReadOnly = true;
			this.tbRegModemConfig1.Size = new System.Drawing.Size(76, 22);
			this.tbRegModemConfig1.TabIndex = 11;
			this.tbRegModemConfig1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// tbRegModemConfig2
			// 
			this.tbRegModemConfig2.Location = new System.Drawing.Point(575, 61);
			this.tbRegModemConfig2.Name = "tbRegModemConfig2";
			this.tbRegModemConfig2.ReadOnly = true;
			this.tbRegModemConfig2.Size = new System.Drawing.Size(76, 22);
			this.tbRegModemConfig2.TabIndex = 11;
			this.tbRegModemConfig2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label64
			// 
			this.label64.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label64.AutoSize = true;
			this.label64.Location = new System.Drawing.Point(516, 36);
			this.label64.Name = "label64";
			this.label64.Size = new System.Drawing.Size(33, 15);
			this.label64.TabIndex = 0;
			this.label64.Text = "0x1D";
			// 
			// label65
			// 
			this.label65.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label65.AutoSize = true;
			this.label65.Location = new System.Drawing.Point(517, 65);
			this.label65.Name = "label65";
			this.label65.Size = new System.Drawing.Size(32, 15);
			this.label65.TabIndex = 0;
			this.label65.Text = "0x1E";
			// 
			// label43
			// 
			this.label43.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label43.AutoSize = true;
			this.label43.Location = new System.Drawing.Point(366, 65);
			this.label43.Name = "label43";
			this.label43.Size = new System.Drawing.Size(99, 15);
			this.label43.TabIndex = 10;
			this.label43.Text = "RegModemConfig2";
			// 
			// label2
			// 
			this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(43, 36);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(69, 15);
			this.label2.TabIndex = 5;
			this.label2.Text = "RegOpMode";
			// 
			// label57
			// 
			this.label57.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label57.AutoSize = true;
			this.label57.Font = new System.Drawing.Font("Arial Unicode MS", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label57.Location = new System.Drawing.Point(594, 6);
			this.label57.Name = "label57";
			this.label57.Size = new System.Drawing.Size(38, 16);
			this.label57.TabIndex = 0;
			this.label57.Text = "Data";
			// 
			// label56
			// 
			this.label56.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label56.AutoSize = true;
			this.label56.Font = new System.Drawing.Font("Arial Unicode MS", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label56.Location = new System.Drawing.Point(254, 6);
			this.label56.Name = "label56";
			this.label56.Size = new System.Drawing.Size(38, 16);
			this.label56.TabIndex = 0;
			this.label56.Text = "Data";
			// 
			// tbRegLna
			// 
			this.tbRegLna.Location = new System.Drawing.Point(237, 235);
			this.tbRegLna.Name = "tbRegLna";
			this.tbRegLna.ReadOnly = true;
			this.tbRegLna.Size = new System.Drawing.Size(72, 22);
			this.tbRegLna.TabIndex = 11;
			this.tbRegLna.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// tbRegOcp
			// 
			this.tbRegOcp.Location = new System.Drawing.Point(237, 206);
			this.tbRegOcp.Name = "tbRegOcp";
			this.tbRegOcp.ReadOnly = true;
			this.tbRegOcp.Size = new System.Drawing.Size(72, 22);
			this.tbRegOcp.TabIndex = 11;
			this.tbRegOcp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// tbRegPaRamp
			// 
			this.tbRegPaRamp.Location = new System.Drawing.Point(237, 177);
			this.tbRegPaRamp.Name = "tbRegPaRamp";
			this.tbRegPaRamp.ReadOnly = true;
			this.tbRegPaRamp.Size = new System.Drawing.Size(72, 22);
			this.tbRegPaRamp.TabIndex = 11;
			this.tbRegPaRamp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// tbRegPaConfig
			// 
			this.tbRegPaConfig.Location = new System.Drawing.Point(237, 148);
			this.tbRegPaConfig.Name = "tbRegPaConfig";
			this.tbRegPaConfig.ReadOnly = true;
			this.tbRegPaConfig.Size = new System.Drawing.Size(72, 22);
			this.tbRegPaConfig.TabIndex = 11;
			this.tbRegPaConfig.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// tbRegFrLsb
			// 
			this.tbRegFrLsb.Location = new System.Drawing.Point(237, 119);
			this.tbRegFrLsb.Name = "tbRegFrLsb";
			this.tbRegFrLsb.ReadOnly = true;
			this.tbRegFrLsb.Size = new System.Drawing.Size(72, 22);
			this.tbRegFrLsb.TabIndex = 11;
			this.tbRegFrLsb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// tbRegFrMid
			// 
			this.tbRegFrMid.Location = new System.Drawing.Point(237, 90);
			this.tbRegFrMid.Name = "tbRegFrMid";
			this.tbRegFrMid.ReadOnly = true;
			this.tbRegFrMid.Size = new System.Drawing.Size(72, 22);
			this.tbRegFrMid.TabIndex = 11;
			this.tbRegFrMid.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// tbRegFrMsb
			// 
			this.tbRegFrMsb.Location = new System.Drawing.Point(237, 61);
			this.tbRegFrMsb.Name = "tbRegFrMsb";
			this.tbRegFrMsb.ReadOnly = true;
			this.tbRegFrMsb.Size = new System.Drawing.Size(72, 22);
			this.tbRegFrMsb.TabIndex = 11;
			this.tbRegFrMsb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// tbRegOpMode
			// 
			this.tbRegOpMode.Location = new System.Drawing.Point(237, 32);
			this.tbRegOpMode.Name = "tbRegOpMode";
			this.tbRegOpMode.ReadOnly = true;
			this.tbRegOpMode.Size = new System.Drawing.Size(72, 22);
			this.tbRegOpMode.TabIndex = 11;
			this.tbRegOpMode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label55
			// 
			this.label55.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label55.AutoSize = true;
			this.label55.Font = new System.Drawing.Font("Arial Unicode MS", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label55.Location = new System.Drawing.Point(503, 6);
			this.label55.Name = "label55";
			this.label55.Size = new System.Drawing.Size(60, 16);
			this.label55.TabIndex = 0;
			this.label55.Text = "Address";
			// 
			// label39
			// 
			this.label39.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label39.AutoSize = true;
			this.label39.Font = new System.Drawing.Font("Arial Unicode MS", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label39.Location = new System.Drawing.Point(381, 6);
			this.label39.Name = "label39";
			this.label39.Size = new System.Drawing.Size(69, 16);
			this.label39.TabIndex = 9;
			this.label39.Text = "Regsister";
			// 
			// label9
			// 
			this.label9.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label9.AutoSize = true;
			this.label9.Font = new System.Drawing.Font("Arial Unicode MS", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label9.Location = new System.Drawing.Point(43, 6);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(69, 16);
			this.label9.TabIndex = 0;
			this.label9.Text = "Regsister";
			// 
			// label32
			// 
			this.label32.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label32.AutoSize = true;
			this.label32.Font = new System.Drawing.Font("Arial Unicode MS", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label32.Location = new System.Drawing.Point(165, 6);
			this.label32.Name = "label32";
			this.label32.Size = new System.Drawing.Size(60, 16);
			this.label32.TabIndex = 0;
			this.label32.Text = "Address";
			// 
			// label37
			// 
			this.label37.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label37.AutoSize = true;
			this.label37.Location = new System.Drawing.Point(179, 36);
			this.label37.Name = "label37";
			this.label37.Size = new System.Drawing.Size(31, 15);
			this.label37.TabIndex = 0;
			this.label37.Text = "0x01";
			// 
			// label36
			// 
			this.label36.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label36.AutoSize = true;
			this.label36.Location = new System.Drawing.Point(179, 65);
			this.label36.Name = "label36";
			this.label36.Size = new System.Drawing.Size(31, 15);
			this.label36.TabIndex = 0;
			this.label36.Text = "0x06";
			// 
			// label35
			// 
			this.label35.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label35.AutoSize = true;
			this.label35.Location = new System.Drawing.Point(179, 94);
			this.label35.Name = "label35";
			this.label35.Size = new System.Drawing.Size(31, 15);
			this.label35.TabIndex = 0;
			this.label35.Text = "0x07";
			// 
			// label34
			// 
			this.label34.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label34.AutoSize = true;
			this.label34.Location = new System.Drawing.Point(179, 123);
			this.label34.Name = "label34";
			this.label34.Size = new System.Drawing.Size(31, 15);
			this.label34.TabIndex = 0;
			this.label34.Text = "0x08";
			// 
			// label33
			// 
			this.label33.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label33.AutoSize = true;
			this.label33.Location = new System.Drawing.Point(179, 152);
			this.label33.Name = "label33";
			this.label33.Size = new System.Drawing.Size(31, 15);
			this.label33.TabIndex = 0;
			this.label33.Text = "0x09";
			// 
			// label31
			// 
			this.label31.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label31.AutoSize = true;
			this.label31.Location = new System.Drawing.Point(179, 181);
			this.label31.Name = "label31";
			this.label31.Size = new System.Drawing.Size(32, 15);
			this.label31.TabIndex = 0;
			this.label31.Text = "0x0A";
			// 
			// label30
			// 
			this.label30.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label30.AutoSize = true;
			this.label30.Location = new System.Drawing.Point(179, 210);
			this.label30.Name = "label30";
			this.label30.Size = new System.Drawing.Size(32, 15);
			this.label30.TabIndex = 0;
			this.label30.Text = "0x0B";
			// 
			// label8
			// 
			this.label8.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(54, 210);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(48, 15);
			this.label8.TabIndex = 3;
			this.label8.Text = "RegOcp";
			// 
			// label7
			// 
			this.label7.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(44, 181);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(68, 15);
			this.label7.TabIndex = 4;
			this.label7.Text = "RegPaRamp";
			// 
			// label6
			// 
			this.label6.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(42, 152);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(71, 15);
			this.label6.TabIndex = 1;
			this.label6.Text = "RegPaConfig";
			// 
			// label5
			// 
			this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(50, 123);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(56, 15);
			this.label5.TabIndex = 2;
			this.label5.Text = "RegFrLsb";
			// 
			// label4
			// 
			this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(50, 94);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(55, 15);
			this.label4.TabIndex = 7;
			this.label4.Text = "RegFrMid";
			// 
			// label3
			// 
			this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(48, 65);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(59, 15);
			this.label3.TabIndex = 8;
			this.label3.Text = "RegFrMsb";
			// 
			// label29
			// 
			this.label29.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label29.AutoSize = true;
			this.label29.Location = new System.Drawing.Point(178, 239);
			this.label29.Name = "label29";
			this.label29.Size = new System.Drawing.Size(33, 15);
			this.label29.TabIndex = 0;
			this.label29.Text = "0x0C";
			// 
			// label10
			// 
			this.label10.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(55, 239);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(45, 15);
			this.label10.TabIndex = 0;
			this.label10.Text = "RegLna";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 7;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Controls.Add(this.label32, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.label9, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.label39, 4, 0);
			this.tableLayoutPanel1.Controls.Add(this.label55, 5, 0);
			this.tableLayoutPanel1.Controls.Add(this.label56, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this.label57, 6, 0);
			this.tableLayoutPanel1.Controls.Add(this.label42, 4, 1);
			this.tableLayoutPanel1.Controls.Add(this.label43, 4, 2);
			this.tableLayoutPanel1.Controls.Add(this.label44, 4, 3);
			this.tableLayoutPanel1.Controls.Add(this.label45, 4, 4);
			this.tableLayoutPanel1.Controls.Add(this.label46, 4, 5);
			this.tableLayoutPanel1.Controls.Add(this.label70, 4, 6);
			this.tableLayoutPanel1.Controls.Add(this.label69, 4, 7);
			this.tableLayoutPanel1.Controls.Add(this.label71, 4, 8);
			this.tableLayoutPanel1.Controls.Add(this.label72, 4, 9);
			this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.label37, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.tbRegOpMode, 2, 1);
			this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.label36, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.tbRegFrMsb, 2, 2);
			this.tableLayoutPanel1.Controls.Add(this.tbRegFrMid, 2, 3);
			this.tableLayoutPanel1.Controls.Add(this.tbRegFrLsb, 2, 4);
			this.tableLayoutPanel1.Controls.Add(this.tbRegPaConfig, 2, 5);
			this.tableLayoutPanel1.Controls.Add(this.tbRegPaRamp, 2, 6);
			this.tableLayoutPanel1.Controls.Add(this.tbRegOcp, 2, 7);
			this.tableLayoutPanel1.Controls.Add(this.tbRegLna, 2, 8);
			this.tableLayoutPanel1.Controls.Add(this.tbRegIrqFlagsMask, 2, 9);
			this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.label5, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this.label6, 0, 5);
			this.tableLayoutPanel1.Controls.Add(this.label7, 0, 6);
			this.tableLayoutPanel1.Controls.Add(this.label8, 0, 7);
			this.tableLayoutPanel1.Controls.Add(this.label10, 0, 8);
			this.tableLayoutPanel1.Controls.Add(this.label15, 0, 9);
			this.tableLayoutPanel1.Controls.Add(this.label35, 1, 3);
			this.tableLayoutPanel1.Controls.Add(this.label34, 1, 4);
			this.tableLayoutPanel1.Controls.Add(this.label33, 1, 5);
			this.tableLayoutPanel1.Controls.Add(this.label31, 1, 6);
			this.tableLayoutPanel1.Controls.Add(this.label30, 1, 7);
			this.tableLayoutPanel1.Controls.Add(this.label29, 1, 8);
			this.tableLayoutPanel1.Controls.Add(this.label24, 1, 9);
			this.tableLayoutPanel1.Controls.Add(this.label64, 5, 1);
			this.tableLayoutPanel1.Controls.Add(this.tbRegModemConfig1, 6, 1);
			this.tableLayoutPanel1.Controls.Add(this.tbRegModemConfig2, 6, 2);
			this.tableLayoutPanel1.Controls.Add(this.tbRegSymbTimeoutLsb, 6, 3);
			this.tableLayoutPanel1.Controls.Add(this.tbRegPreambleMsb, 6, 4);
			this.tableLayoutPanel1.Controls.Add(this.tbRegPreambleLsb, 6, 5);
			this.tableLayoutPanel1.Controls.Add(this.tbRegModemConfig3, 6, 6);
			this.tableLayoutPanel1.Controls.Add(this.tbRegTcxo, 6, 7);
			this.tableLayoutPanel1.Controls.Add(this.tbRegPaDac, 6, 8);
			this.tableLayoutPanel1.Controls.Add(this.tbRegPllHf, 6, 9);
			this.tableLayoutPanel1.Controls.Add(this.label65, 5, 2);
			this.tableLayoutPanel1.Controls.Add(this.label66, 5, 3);
			this.tableLayoutPanel1.Controls.Add(this.label73, 5, 4);
			this.tableLayoutPanel1.Controls.Add(this.label74, 5, 5);
			this.tableLayoutPanel1.Controls.Add(this.label79, 5, 6);
			this.tableLayoutPanel1.Controls.Add(this.label82, 5, 7);
			this.tableLayoutPanel1.Controls.Add(this.label80, 5, 8);
			this.tableLayoutPanel1.Controls.Add(this.label81, 5, 9);
			this.tableLayoutPanel1.Font = new System.Drawing.Font("Arial Unicode MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 10;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(654, 294);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// RegTable
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(656, 297);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "RegTable";
			this.Text = "RegTable";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion
	}
}
