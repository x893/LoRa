// Decompiled with JetBrains decompiler
// Type: SemtechLib.Devices.SX1276.UI.Controls.PacketHandlerView
// Assembly: SemtechLib.Devices.SX1276.UI, Version=2.0.0.0, Culture=neutral, PublicKeyToken=a3cee8af388a4f11
// MVID: 2B98C92B-3345-4D34-A253-90690D8C71AF
// Assembly location: C:\Tools\Semtech\SX1276SKA\SemtechLib.Devices.SX1276.UI.dll

using SemtechLib.Controls;
using SemtechLib.Controls.HexBoxCtrl;
using SemtechLib.Devices.SX1276.Enumerations;
using SemtechLib.Devices.SX1276.Events;
using SemtechLib.Devices.SX1276.General;
using SemtechLib.General.Events;
using SemtechLib.General.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace SemtechLib.Devices.SX1276.UI.Controls
{
  public class PacketHandlerView : UserControl, INotifyDocumentationChanged
  {
    private MaskValidationType syncWord = new MaskValidationType("69-81-7E-96");
    private MaskValidationType aesWord = new MaskValidationType("00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00");
    private OperatingModeEnum mode = OperatingModeEnum.Stdby;
    private bool bitSync = true;
    private byte[] syncValue = new byte[4]
    {
      (byte) 105,
      (byte) 129,
      (byte) 126,
      (byte) 150
    };
    private IContainer components;
    private Label label1;
    private NumericUpDownEx nudPreambleSize;
    private Label label2;
    private Label label3;
    private Panel pnlSync;
    private RadioButton rBtnSyncOn;
    private RadioButton rBtnSyncOff;
    private Label label4;
    private Panel pnlFifoFillCondition;
    private RadioButton rBtnFifoFillAlways;
    private RadioButton rBtnFifoFillSyncAddress;
    private Label label9;
    private MaskedTextBox tBoxSyncValue;
    private Label label10;
    private Panel pnlPacketFormat;
    private RadioButton rBtnPacketFormatVariable;
    private RadioButton rBtnPacketFormatFixed;
    private Label label11;
    private NumericUpDownEx nudPayloadLength;
    private Label lblPayloadLength;
    private Label label12;
    private Label label17;
    private Panel pnlAddressInPayload;
    private Label label18;
    private Panel pnlAddressFiltering;
    private RadioButton rBtnAddressFilteringOff;
    private RadioButton rBtnAddressFilteringNode;
    private RadioButton rBtnAddressFilteringNodeBroadcast;
    private Label label19;
    private NumericUpDownEx nudNodeAddress;
    private Label lblNodeAddress;
    private Label label20;
    private NumericUpDownEx nudBroadcastAddress;
    private Label lblBroadcastAddress;
    private Label label21;
    private Panel pnlDcFree;
    private RadioButton rBtnDcFreeOff;
    private RadioButton rBtnDcFreeManchester;
    private RadioButton rBtnDcFreeWhitening;
    private Label label22;
    private Panel pnlCrcCalculation;
    private RadioButton rBtnCrcOn;
    private RadioButton rBtnCrcOff;
    private Label label23;
    private Panel pnlCrcAutoClear;
    private RadioButton rBtnCrcAutoClearOn;
    private RadioButton rBtnCrcAutoClearOff;
    private Label label26;
    private Panel pnlTxStart;
    private RadioButton rBtnTxStartFifoLevel;
    private RadioButton rBtnTxStartFifoNotEmpty;
    private Label label27;
    private NumericUpDownEx nudFifoThreshold;
    private GroupBoxEx gBoxPacket;
    private TableLayoutPanel tblPacket;
    private Label label29;
    private Label lblPacketPreamble;
    private Label label30;
    private Label lblPacketSyncValue;
    private Label label31;
    private Label lblPacketLength;
    private Label label32;
    private Panel pnlPacketAddr;
    private Label lblPacketAddr;
    private Label label33;
    private Label lblPayload;
    private Label label34;
    private Panel pnlPacketCrc;
    private Label lblPacketCrc;
    private Led ledPacketCrc;
    private PayloadImg imgPacketMessage;
    private GroupBoxEx gBoxMessage;
    private TableLayoutPanel tblPayloadMessage;
    private Label label35;
    private Label label36;
    private HexBox hexBoxPayload;
    private GroupBoxEx gBoxControl;
    private CheckBox cBtnPacketHandlerStartStop;
    private Label lblPacketsNb;
    private TextBox tBoxPacketsNb;
    private Label lblPacketsRepeatValue;
    private TextBox tBoxPacketsRepeatValue;
    private ErrorProvider errorProvider;
    private TableLayoutPanel tableLayoutPanel1;
    private Panel pnlPayloadLength;
    private TableLayoutPanel tableLayoutPanel2;
    private Panel pnlBroadcastAddress;
    private Panel pnlNodeAddress;
    private GroupBoxEx gBoxDeviceStatus;
    private Label lblOperatingMode;
    private Label label37;
    private Label lblBitSynchroniser;
    private Label lblDataMode;
    private Label label38;
    private Label label39;
    private RadioButton rBtnNodeAddressInPayloadNo;
    private RadioButton rBtnNodeAddressInPayloadYes;
    private CheckBox cBtnLog;
    private Label label13;
    private Label label5;
    private NumericUpDownEx nudSyncSize;
    private Label label6;
    private Panel panel1;
    private RadioButton rBtnPreamblePolarity55;
    private RadioButton rBtnPreamblePolarityAA;
    private Label label7;
    private Panel panel2;
    private RadioButton rBtnCrcCcitt;
    private RadioButton rBtnCrcIbm;
    private Panel panel3;
    private RadioButton rBtnIoHomeOff;
    private RadioButton rBtnIoHomeOn;
    private Panel panel4;
    private RadioButton rBtnIoHomePwrFrameOff;
    private RadioButton rBtnIoHomePwrFrameOn;
    private Panel panel5;
    private RadioButton rBtnBeaconOff;
    private RadioButton rBtnBeaconOn;
    private Label label8;
    private Label label14;
    private Label label15;
    private Label label16;
    private ComboBox cBoxDataMode;
    private Label label24;
    private Button btnFillFifo;
    private Label label25;
    private ComboBox cBoxAutoRestartRxMode;
    private bool inHexPayloadDataChanged;
    private Decimal bitRate;
    private byte[] message;
    private ushort crc;

    public OperatingModeEnum Mode
    {
      get
      {
        return this.mode;
      }
      set
      {
        this.mode = value;
        this.UpdateControls();
      }
    }

    public Decimal Bitrate
    {
      get
      {
        return this.bitRate;
      }
      set
      {
        if (!(this.bitRate != value))
          return;
        this.bitRate = value;
      }
    }

    public bool BitSyncOn
    {
      get
      {
        return this.bitSync;
      }
      set
      {
        this.bitSync = value;
        if (this.bitSync)
          this.lblBitSynchroniser.Text = "ON";
        else
          this.lblBitSynchroniser.Text = "OFF";
      }
    }

    public DataModeEnum DataMode
    {
      get
      {
        return (DataModeEnum) this.cBoxDataMode.SelectedIndex;
      }
      set
      {
        this.cBoxDataMode.SelectedIndex = (int) value;
        this.UpdateControls();
      }
    }

    public int PreambleSize
    {
      get
      {
        return (int) this.nudPreambleSize.Value;
      }
      set
      {
        this.nudPreambleSize.Value = (Decimal) value;
        switch (value)
        {
          case 0:
            this.lblPacketPreamble.Text = "";
            break;
          case 1:
            this.lblPacketPreamble.Text = "55";
            break;
          case 2:
            this.lblPacketPreamble.Text = "55-55";
            break;
          case 3:
            this.lblPacketPreamble.Text = "55-55-55";
            break;
          case 4:
            this.lblPacketPreamble.Text = "55-55-55-55";
            break;
          case 5:
            this.lblPacketPreamble.Text = "55-55-55-55-55";
            break;
          default:
            this.lblPacketPreamble.Text = "55-55-55-55-...-55";
            break;
        }
        if (this.nudPreambleSize.Value < new Decimal(2))
        {
          this.nudPreambleSize.BackColor = ControlPaint.LightLight(Color.Red);
          this.errorProvider.SetError((Control) this.nudPreambleSize, "Preamble size must be greater than 12 bits!");
        }
        else
        {
          this.nudPreambleSize.BackColor = SystemColors.Window;
          this.errorProvider.SetError((Control) this.nudPreambleSize, "");
        }
      }
    }

    public AutoRestartRxEnum AutoRestartRxOn
    {
      get
      {
        return (AutoRestartRxEnum) this.cBoxAutoRestartRxMode.SelectedIndex;
      }
      set
      {
        this.cBoxAutoRestartRxMode.SelectedIndex = (int) value;
      }
    }

    public PreamblePolarityEnum PreamblePolarity
    {
      get
      {
        return this.rBtnPreamblePolarity55.Checked ? PreamblePolarityEnum.POLARITY_55 : PreamblePolarityEnum.POLARITY_AA;
      }
      set
      {
        this.rBtnPreamblePolarity55.CheckedChanged -= new EventHandler(this.rBtnPreamblePolarity_CheckedChanged);
        this.rBtnPreamblePolarityAA.CheckedChanged -= new EventHandler(this.rBtnPreamblePolarity_CheckedChanged);
        if (value == PreamblePolarityEnum.POLARITY_55)
        {
          this.rBtnPreamblePolarity55.Checked = true;
          this.rBtnPreamblePolarityAA.Checked = false;
        }
        else
        {
          this.rBtnPreamblePolarity55.Checked = false;
          this.rBtnPreamblePolarityAA.Checked = true;
        }
        this.rBtnPreamblePolarity55.CheckedChanged += new EventHandler(this.rBtnPreamblePolarity_CheckedChanged);
        this.rBtnPreamblePolarityAA.CheckedChanged += new EventHandler(this.rBtnPreamblePolarity_CheckedChanged);
      }
    }

    public bool SyncOn
    {
      get
      {
        return this.rBtnSyncOn.Checked;
      }
      set
      {
        this.rBtnSyncOn.Checked = value;
        this.rBtnSyncOff.Checked = !value;
        this.nudSyncSize.Enabled = value;
        this.tBoxSyncValue.Enabled = value;
        this.lblPacketSyncValue.Visible = value;
      }
    }

    public FifoFillConditionEnum FifoFillCondition
    {
      get
      {
        return !this.rBtnFifoFillSyncAddress.Checked ? FifoFillConditionEnum.Allways : FifoFillConditionEnum.OnSyncAddressIrq;
      }
      set
      {
        if (value == FifoFillConditionEnum.OnSyncAddressIrq)
          this.rBtnFifoFillSyncAddress.Checked = true;
        else
          this.rBtnFifoFillAlways.Checked = true;
      }
    }

    public byte SyncSize
    {
      get
      {
        return (byte) this.nudSyncSize.Value;
      }
      set
      {
        try
        {
          this.nudSyncSize.Value = (Decimal) value;
          string text = this.tBoxSyncValue.Text;
          switch ((byte) this.nudSyncSize.Value)
          {
            case (byte) 1:
              this.tBoxSyncValue.Mask = "&&";
              break;
            case (byte) 2:
              this.tBoxSyncValue.Mask = "&&-&&";
              break;
            case (byte) 3:
              this.tBoxSyncValue.Mask = "&&-&&-&&";
              break;
            case (byte) 4:
              this.tBoxSyncValue.Mask = "&&-&&-&&-&&";
              break;
            case (byte) 5:
              this.tBoxSyncValue.Mask = "&&-&&-&&-&&-&&";
              break;
            case (byte) 6:
              this.tBoxSyncValue.Mask = "&&-&&-&&-&&-&&-&&";
              break;
            case (byte) 7:
              this.tBoxSyncValue.Mask = "&&-&&-&&-&&-&&-&&-&&";
              break;
            case (byte) 8:
              this.tBoxSyncValue.Mask = "&&-&&-&&-&&-&&-&&-&&-&&";
              break;
            default:
              throw new Exception("Wrong sync word size!");
          }
          this.tBoxSyncValue.Text = text;
        }
        catch (Exception ex)
        {
          this.OnError((byte) 1, ex.Message);
        }
      }
    }

    public byte[] SyncValue
    {
      get
      {
        return this.syncValue;
      }
      set
      {
        this.syncValue = value;
        try
        {
          this.tBoxSyncValue.TextChanged -= new EventHandler(this.tBoxSyncValue_TextChanged);
          this.tBoxSyncValue.MaskInputRejected -= new MaskInputRejectedEventHandler(this.tBoxSyncValue_MaskInputRejected);
          this.syncWord.ArrayValue = this.syncValue;
          this.lblPacketSyncValue.Text = this.tBoxSyncValue.Text = this.syncWord.StringValue;
        }
        catch (Exception ex)
        {
          this.OnError((byte) 1, ex.Message);
        }
        finally
        {
          this.tBoxSyncValue.TextChanged += new EventHandler(this.tBoxSyncValue_TextChanged);
          this.tBoxSyncValue.MaskInputRejected += new MaskInputRejectedEventHandler(this.tBoxSyncValue_MaskInputRejected);
        }
      }
    }

    public PacketFormatEnum PacketFormat
    {
      get
      {
        return this.rBtnPacketFormatVariable.Checked ? PacketFormatEnum.Variable : PacketFormatEnum.Fixed;
      }
      set
      {
        if (this.Mode == OperatingModeEnum.Tx)
          this.nudPayloadLength.Enabled = false;
        else if (this.Mode == OperatingModeEnum.Rx)
          this.nudPayloadLength.Enabled = true;
        else
          this.nudPayloadLength.Enabled = false;
        if (value == PacketFormatEnum.Variable)
        {
          this.lblPacketLength.Visible = true;
          this.rBtnPacketFormatVariable.Checked = true;
        }
        else
        {
          this.lblPacketLength.Visible = false;
          this.rBtnPacketFormatFixed.Checked = true;
        }
      }
    }

    public DcFreeEnum DcFree
    {
      get
      {
        if (this.rBtnDcFreeOff.Checked)
          return DcFreeEnum.OFF;
        if (this.rBtnDcFreeManchester.Checked)
          return DcFreeEnum.Manchester;
        return this.rBtnDcFreeWhitening.Checked ? DcFreeEnum.Whitening : DcFreeEnum.OFF;
      }
      set
      {
        if (value == DcFreeEnum.Manchester)
          this.rBtnDcFreeManchester.Checked = true;
        else if (value == DcFreeEnum.Whitening)
          this.rBtnDcFreeWhitening.Checked = true;
        else
          this.rBtnDcFreeOff.Checked = true;
      }
    }

    public bool CrcOn
    {
      get
      {
        return this.rBtnCrcOn.Checked;
      }
      set
      {
        this.lblPacketCrc.Visible = value;
        this.rBtnCrcOn.Checked = value;
        this.rBtnCrcOff.Checked = !value;
      }
    }

    public bool CrcAutoClearOff
    {
      get
      {
        return this.rBtnCrcAutoClearOff.Checked;
      }
      set
      {
        this.rBtnCrcAutoClearOn.Checked = !value;
        this.rBtnCrcAutoClearOff.Checked = value;
      }
    }

    public AddressFilteringEnum AddressFiltering
    {
      get
      {
        if (this.rBtnAddressFilteringOff.Checked)
          return AddressFilteringEnum.OFF;
        if (this.rBtnAddressFilteringNode.Checked)
          return AddressFilteringEnum.Node;
        return this.rBtnAddressFilteringNodeBroadcast.Checked ? AddressFilteringEnum.NodeBroadcast : AddressFilteringEnum.Reserved;
      }
      set
      {
        if (value == AddressFilteringEnum.Node)
        {
          this.rBtnAddressFilteringNode.Checked = true;
          this.lblPacketAddr.Visible = true;
          this.nudNodeAddress.Enabled = true;
          this.lblNodeAddress.Enabled = true;
          this.nudBroadcastAddress.Enabled = false;
          this.lblBroadcastAddress.Enabled = false;
        }
        else if (value == AddressFilteringEnum.NodeBroadcast)
        {
          this.rBtnAddressFilteringNodeBroadcast.Checked = true;
          this.lblPacketAddr.Visible = true;
          this.nudNodeAddress.Enabled = true;
          this.lblNodeAddress.Enabled = true;
          this.nudBroadcastAddress.Enabled = true;
          this.lblBroadcastAddress.Enabled = true;
        }
        else if (value == AddressFilteringEnum.Reserved)
        {
          this.rBtnAddressFilteringNode.Checked = false;
          this.rBtnAddressFilteringNodeBroadcast.Checked = false;
          this.rBtnAddressFilteringOff.Checked = false;
          this.lblPacketAddr.Visible = false;
          this.nudNodeAddress.Enabled = false;
          this.lblNodeAddress.Enabled = false;
          this.nudBroadcastAddress.Enabled = false;
          this.lblBroadcastAddress.Enabled = false;
        }
        else
        {
          this.rBtnAddressFilteringOff.Checked = true;
          this.lblPacketAddr.Visible = false;
          this.nudNodeAddress.Enabled = false;
          this.lblNodeAddress.Enabled = false;
          this.nudBroadcastAddress.Enabled = false;
          this.lblBroadcastAddress.Enabled = false;
        }
      }
    }

    public short PayloadLength
    {
      get
      {
        return (short) this.nudPayloadLength.Value;
      }
      set
      {
        this.nudPayloadLength.Value = (Decimal) value;
        this.lblPayloadLength.Text = "0x" + value.ToString("X02");
      }
    }

    public byte NodeAddress
    {
      get
      {
        return (byte) this.nudNodeAddress.Value;
      }
      set
      {
        this.nudNodeAddress.Value = (Decimal) value;
        this.lblPacketAddr.Text = value.ToString("X02");
        this.lblNodeAddress.Text = "0x" + value.ToString("X02");
      }
    }

    public byte NodeAddressRx
    {
      get
      {
        return (byte) 0;
      }
      set
      {
        this.lblPacketAddr.Text = value.ToString("X02");
      }
    }

    public byte BroadcastAddress
    {
      get
      {
        return (byte) this.nudBroadcastAddress.Value;
      }
      set
      {
        this.nudBroadcastAddress.Value = (Decimal) value;
        this.lblBroadcastAddress.Text = "0x" + value.ToString("X02");
      }
    }

    public bool TxStartCondition
    {
      get
      {
        return this.rBtnTxStartFifoNotEmpty.Checked;
      }
      set
      {
        this.rBtnTxStartFifoNotEmpty.Checked = value;
        this.rBtnTxStartFifoLevel.Checked = !value;
      }
    }

    public byte FifoThreshold
    {
      get
      {
        return (byte) this.nudFifoThreshold.Value;
      }
      set
      {
        this.nudFifoThreshold.Value = (Decimal) value;
      }
    }

    public bool CrcIbmOn
    {
      get
      {
        return this.rBtnCrcIbm.Checked;
      }
      set
      {
        this.rBtnCrcIbm.CheckedChanged -= new EventHandler(this.rBtnCrcIbm_CheckedChanged);
        this.rBtnCrcCcitt.CheckedChanged -= new EventHandler(this.rBtnCrcIbm_CheckedChanged);
        if (value)
        {
          this.rBtnCrcIbm.Checked = true;
          this.rBtnCrcCcitt.Checked = false;
        }
        else
        {
          this.rBtnCrcIbm.Checked = false;
          this.rBtnCrcCcitt.Checked = true;
        }
        this.rBtnCrcIbm.CheckedChanged += new EventHandler(this.rBtnCrcIbm_CheckedChanged);
        this.rBtnCrcCcitt.CheckedChanged += new EventHandler(this.rBtnCrcIbm_CheckedChanged);
      }
    }

    public bool IoHomeOn
    {
      get
      {
        return this.rBtnIoHomeOn.Checked;
      }
      set
      {
        this.rBtnIoHomeOn.CheckedChanged -= new EventHandler(this.rBtnIoHomeOn_CheckedChanged);
        this.rBtnIoHomeOff.CheckedChanged -= new EventHandler(this.rBtnIoHomeOn_CheckedChanged);
        if (value)
        {
          this.rBtnIoHomeOn.Checked = true;
          this.rBtnIoHomeOff.Checked = false;
        }
        else
        {
          this.rBtnIoHomeOn.Checked = false;
          this.rBtnIoHomeOff.Checked = true;
        }
        this.rBtnIoHomeOn.CheckedChanged += new EventHandler(this.rBtnIoHomeOn_CheckedChanged);
        this.rBtnIoHomeOff.CheckedChanged += new EventHandler(this.rBtnIoHomeOn_CheckedChanged);
      }
    }

    public bool IoHomePwrFrameOn
    {
      get
      {
        return this.rBtnIoHomePwrFrameOn.Checked;
      }
      set
      {
        this.rBtnIoHomePwrFrameOn.CheckedChanged -= new EventHandler(this.rBtnIoHomePwrFrameOn_CheckedChanged);
        this.rBtnIoHomePwrFrameOff.CheckedChanged -= new EventHandler(this.rBtnIoHomePwrFrameOn_CheckedChanged);
        if (value)
        {
          this.rBtnIoHomePwrFrameOn.Checked = true;
          this.rBtnIoHomePwrFrameOff.Checked = false;
        }
        else
        {
          this.rBtnIoHomePwrFrameOn.Checked = false;
          this.rBtnIoHomePwrFrameOff.Checked = true;
        }
        this.rBtnIoHomePwrFrameOn.CheckedChanged += new EventHandler(this.rBtnIoHomePwrFrameOn_CheckedChanged);
        this.rBtnIoHomePwrFrameOff.CheckedChanged += new EventHandler(this.rBtnIoHomePwrFrameOn_CheckedChanged);
      }
    }

    public bool BeaconOn
    {
      get
      {
        return this.rBtnBeaconOn.Checked;
      }
      set
      {
        this.rBtnBeaconOn.CheckedChanged -= new EventHandler(this.rBtnBeaconOn_CheckedChanged);
        this.rBtnBeaconOff.CheckedChanged -= new EventHandler(this.rBtnBeaconOn_CheckedChanged);
        if (value)
        {
          this.rBtnBeaconOn.Checked = true;
          this.rBtnBeaconOff.Checked = false;
        }
        else
        {
          this.rBtnBeaconOn.Checked = false;
          this.rBtnBeaconOff.Checked = true;
        }
        this.rBtnBeaconOn.CheckedChanged += new EventHandler(this.rBtnBeaconOn_CheckedChanged);
        this.rBtnBeaconOff.CheckedChanged += new EventHandler(this.rBtnBeaconOn_CheckedChanged);
      }
    }

    public int MessageLength
    {
      get
      {
        return Convert.ToInt32(this.lblPacketLength.Text, 16);
      }
      set
      {
        this.lblPacketLength.Text = value.ToString("X02");
      }
    }

    public byte[] Message
    {
      get
      {
        return this.message;
      }
      set
      {
        this.message = value;
        DynamicByteProvider dynamicByteProvider = this.hexBoxPayload.ByteProvider as DynamicByteProvider;
        dynamicByteProvider.Bytes.Clear();
        dynamicByteProvider.Bytes.AddRange(value);
        this.hexBoxPayload.ByteProvider.ApplyChanges();
        this.hexBoxPayload.Invalidate();
      }
    }

    public ushort Crc
    {
      get
      {
        return this.crc;
      }
      set
      {
        this.crc = value;
        this.lblPacketCrc.Text = ((int) value >> 8 & (int) byte.MaxValue).ToString("X02") + "-" + ((int) value & (int) byte.MaxValue).ToString("X02");
      }
    }

    public bool StartStop
    {
      get
      {
        return this.cBtnPacketHandlerStartStop.Checked;
      }
      set
      {
        this.cBtnPacketHandlerStartStop.Checked = value;
      }
    }

    public int PacketNumber
    {
      get
      {
        return Convert.ToInt32(this.tBoxPacketsNb.Text);
      }
      set
      {
        this.tBoxPacketsNb.Text = value.ToString();
      }
    }

    public int MaxPacketNumber
    {
      get
      {
        return Convert.ToInt32(this.tBoxPacketsRepeatValue.Text);
      }
      set
      {
        this.tBoxPacketsRepeatValue.Text = value.ToString();
      }
    }

    public bool LogEnabled
    {
      get
      {
        return this.cBtnLog.Checked;
      }
      set
      {
        this.cBtnLog.Checked = value;
      }
    }

    public event ErrorEventHandler Error;

    public event DataModeEventHandler DataModeChanged;

    public event Int32EventHandler PreambleSizeChanged;

    public event AutoRestartRxEventHandler AutoRestartRxChanged;

    public event PreamblePolarityEventHandler PreamblePolarityChanged;

    public event BooleanEventHandler SyncOnChanged;

    public event FifoFillConditionEventHandler FifoFillConditionChanged;

    public event ByteEventHandler SyncSizeChanged;

    public event ByteArrayEventHandler SyncValueChanged;

    public event PacketFormatEventHandler PacketFormatChanged;

    public event DcFreeEventHandler DcFreeChanged;

    public event BooleanEventHandler CrcOnChanged;

    public event BooleanEventHandler CrcAutoClearOffChanged;

    public event AddressFilteringEventHandler AddressFilteringChanged;

    public event Int16EventHandler PayloadLengthChanged;

    public event ByteEventHandler NodeAddressChanged;

    public event ByteEventHandler BroadcastAddressChanged;

    public event BooleanEventHandler TxStartConditionChanged;

    public event ByteEventHandler FifoThresholdChanged;

    public event Int32EventHandler MessageLengthChanged;

    public event ByteArrayEventHandler MessageChanged;

    public event BooleanEventHandler StartStopChanged;

    public event Int32EventHandler MaxPacketNumberChanged;

    public event BooleanEventHandler PacketHandlerLogEnableChanged;

    public event BooleanEventHandler CrcIbmChanged;

    public event BooleanEventHandler IoHomeOnChanged;

    public event BooleanEventHandler IoHomePwrFrameOnChanged;

    public event BooleanEventHandler BeaconOnChanged;

    public event EventHandler FillFifoChanged;

    public event DocumentationChangedEventHandler DocumentationChanged;

    public PacketHandlerView()
    {
      this.InitializeComponent();
      this.tBoxSyncValue.TextChanged -= new EventHandler(this.tBoxSyncValue_TextChanged);
      this.tBoxSyncValue.MaskInputRejected -= new MaskInputRejectedEventHandler(this.tBoxSyncValue_MaskInputRejected);
      this.tBoxSyncValue.ValidatingType = typeof (MaskValidationType);
      this.tBoxSyncValue.Mask = "&&-&&-&&-&&";
      this.tBoxSyncValue.TextChanged += new EventHandler(this.tBoxSyncValue_TextChanged);
      this.tBoxSyncValue.MaskInputRejected += new MaskInputRejectedEventHandler(this.tBoxSyncValue_MaskInputRejected);
      this.message = new byte[0];
      this.hexBoxPayload.ByteProvider = (IByteProvider) new DynamicByteProvider(new byte[this.Message.Length]);
      this.hexBoxPayload.ByteProvider.Changed += new EventHandler(this.hexBoxPayload_DataChanged);
      this.hexBoxPayload.ByteProvider.ApplyChanges();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new Container();
      this.errorProvider = new ErrorProvider(this.components);
      this.tBoxSyncValue = new MaskedTextBox();
      this.nudPreambleSize = new NumericUpDownEx();
      this.label12 = new Label();
      this.label1 = new Label();
      this.label2 = new Label();
      this.label18 = new Label();
      this.label11 = new Label();
      this.label20 = new Label();
      this.label10 = new Label();
      this.label21 = new Label();
      this.label19 = new Label();
      this.label27 = new Label();
      this.label26 = new Label();
      this.pnlDcFree = new Panel();
      this.rBtnDcFreeWhitening = new RadioButton();
      this.rBtnDcFreeManchester = new RadioButton();
      this.rBtnDcFreeOff = new RadioButton();
      this.pnlAddressInPayload = new Panel();
      this.rBtnNodeAddressInPayloadNo = new RadioButton();
      this.rBtnNodeAddressInPayloadYes = new RadioButton();
      this.label17 = new Label();
      this.pnlFifoFillCondition = new Panel();
      this.rBtnFifoFillAlways = new RadioButton();
      this.rBtnFifoFillSyncAddress = new RadioButton();
      this.label4 = new Label();
      this.pnlSync = new Panel();
      this.rBtnSyncOff = new RadioButton();
      this.rBtnSyncOn = new RadioButton();
      this.label3 = new Label();
      this.label9 = new Label();
      this.pnlCrcAutoClear = new Panel();
      this.rBtnCrcAutoClearOff = new RadioButton();
      this.rBtnCrcAutoClearOn = new RadioButton();
      this.label23 = new Label();
      this.pnlCrcCalculation = new Panel();
      this.rBtnCrcOff = new RadioButton();
      this.rBtnCrcOn = new RadioButton();
      this.label22 = new Label();
      this.pnlTxStart = new Panel();
      this.rBtnTxStartFifoNotEmpty = new RadioButton();
      this.rBtnTxStartFifoLevel = new RadioButton();
      this.pnlAddressFiltering = new Panel();
      this.rBtnAddressFilteringNodeBroadcast = new RadioButton();
      this.rBtnAddressFilteringNode = new RadioButton();
      this.rBtnAddressFilteringOff = new RadioButton();
      this.lblNodeAddress = new Label();
      this.lblPayloadLength = new Label();
      this.lblBroadcastAddress = new Label();
      this.pnlPacketFormat = new Panel();
      this.rBtnPacketFormatFixed = new RadioButton();
      this.rBtnPacketFormatVariable = new RadioButton();
      this.tableLayoutPanel1 = new TableLayoutPanel();
      this.pnlPayloadLength = new Panel();
      this.nudPayloadLength = new NumericUpDownEx();
      this.label5 = new Label();
      this.nudSyncSize = new NumericUpDownEx();
      this.label6 = new Label();
      this.panel1 = new Panel();
      this.rBtnPreamblePolarity55 = new RadioButton();
      this.rBtnPreamblePolarityAA = new RadioButton();
      this.label7 = new Label();
      this.cBoxDataMode = new ComboBox();
      this.label24 = new Label();
      this.label25 = new Label();
      this.cBoxAutoRestartRxMode = new ComboBox();
      this.pnlNodeAddress = new Panel();
      this.nudNodeAddress = new NumericUpDownEx();
      this.pnlBroadcastAddress = new Panel();
      this.nudBroadcastAddress = new NumericUpDownEx();
      this.tableLayoutPanel2 = new TableLayoutPanel();
      this.nudFifoThreshold = new NumericUpDownEx();
      this.label13 = new Label();
      this.panel2 = new Panel();
      this.rBtnCrcCcitt = new RadioButton();
      this.rBtnCrcIbm = new RadioButton();
      this.panel3 = new Panel();
      this.rBtnIoHomeOff = new RadioButton();
      this.rBtnIoHomeOn = new RadioButton();
      this.panel4 = new Panel();
      this.rBtnIoHomePwrFrameOff = new RadioButton();
      this.rBtnIoHomePwrFrameOn = new RadioButton();
      this.panel5 = new Panel();
      this.rBtnBeaconOff = new RadioButton();
      this.rBtnBeaconOn = new RadioButton();
      this.label8 = new Label();
      this.label14 = new Label();
      this.label15 = new Label();
      this.label16 = new Label();
      this.gBoxDeviceStatus = new GroupBoxEx();
      this.lblOperatingMode = new Label();
      this.label37 = new Label();
      this.lblBitSynchroniser = new Label();
      this.lblDataMode = new Label();
      this.label38 = new Label();
      this.label39 = new Label();
      this.gBoxControl = new GroupBoxEx();
      this.btnFillFifo = new Button();
      this.tBoxPacketsNb = new TextBox();
      this.cBtnLog = new CheckBox();
      this.cBtnPacketHandlerStartStop = new CheckBox();
      this.lblPacketsNb = new Label();
      this.tBoxPacketsRepeatValue = new TextBox();
      this.lblPacketsRepeatValue = new Label();
      this.gBoxPacket = new GroupBoxEx();
      this.imgPacketMessage = new PayloadImg();
      this.gBoxMessage = new GroupBoxEx();
      this.tblPayloadMessage = new TableLayoutPanel();
      this.hexBoxPayload = new HexBox();
      this.label36 = new Label();
      this.label35 = new Label();
      this.tblPacket = new TableLayoutPanel();
      this.label29 = new Label();
      this.label30 = new Label();
      this.label31 = new Label();
      this.label32 = new Label();
      this.label33 = new Label();
      this.label34 = new Label();
      this.lblPacketPreamble = new Label();
      this.lblPayload = new Label();
      this.pnlPacketCrc = new Panel();
      this.ledPacketCrc = new Led();
      this.lblPacketCrc = new Label();
      this.pnlPacketAddr = new Panel();
      this.lblPacketAddr = new Label();
      this.lblPacketLength = new Label();
      this.lblPacketSyncValue = new Label();
      ((ISupportInitialize) this.errorProvider).BeginInit();
      this.nudPreambleSize.BeginInit();
      this.pnlDcFree.SuspendLayout();
      this.pnlAddressInPayload.SuspendLayout();
      this.pnlFifoFillCondition.SuspendLayout();
      this.pnlSync.SuspendLayout();
      this.pnlCrcAutoClear.SuspendLayout();
      this.pnlCrcCalculation.SuspendLayout();
      this.pnlTxStart.SuspendLayout();
      this.pnlAddressFiltering.SuspendLayout();
      this.pnlPacketFormat.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      this.pnlPayloadLength.SuspendLayout();
      this.nudPayloadLength.BeginInit();
      this.nudSyncSize.BeginInit();
      this.panel1.SuspendLayout();
      this.pnlNodeAddress.SuspendLayout();
      this.nudNodeAddress.BeginInit();
      this.pnlBroadcastAddress.SuspendLayout();
      this.nudBroadcastAddress.BeginInit();
      this.tableLayoutPanel2.SuspendLayout();
      this.nudFifoThreshold.BeginInit();
      this.panel2.SuspendLayout();
      this.panel3.SuspendLayout();
      this.panel4.SuspendLayout();
      this.panel5.SuspendLayout();
      this.gBoxDeviceStatus.SuspendLayout();
      this.gBoxControl.SuspendLayout();
      this.gBoxPacket.SuspendLayout();
      this.gBoxMessage.SuspendLayout();
      this.tblPayloadMessage.SuspendLayout();
      this.tblPacket.SuspendLayout();
      this.pnlPacketCrc.SuspendLayout();
      this.pnlPacketAddr.SuspendLayout();
      this.SuspendLayout();
      this.errorProvider.ContainerControl = (ContainerControl) this;
      this.tBoxSyncValue.Anchor = AnchorStyles.Left;
      this.errorProvider.SetIconPadding((Control) this.tBoxSyncValue, 6);
      this.tBoxSyncValue.InsertKeyMode = InsertKeyMode.Overwrite;
      this.tBoxSyncValue.Location = new Point(163, 172);
      this.tBoxSyncValue.Margin = new Padding(3, 2, 3, 2);
      this.tBoxSyncValue.Mask = "&&-&&-&&-&&-&&-&&-&&-&&";
      this.tBoxSyncValue.Name = "tBoxSyncValue";
      this.tBoxSyncValue.Size = new Size(143, 20);
      this.tBoxSyncValue.TabIndex = 14;
      this.tBoxSyncValue.Text = "AAAAAAAAAAAAAAAA";
      this.tBoxSyncValue.MaskInputRejected += new MaskInputRejectedEventHandler(this.tBoxSyncValue_MaskInputRejected);
      this.tBoxSyncValue.TypeValidationCompleted += new TypeValidationEventHandler(this.tBoxSyncValue_TypeValidationCompleted);
      this.tBoxSyncValue.TextChanged += new EventHandler(this.tBoxSyncValue_TextChanged);
      this.tBoxSyncValue.KeyDown += new KeyEventHandler(this.tBoxSyncValue_KeyDown);
      this.tBoxSyncValue.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.tBoxSyncValue.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.tBoxSyncValue.Validated += new EventHandler(this.tBox_Validated);
      this.nudPreambleSize.Anchor = AnchorStyles.Left;
      this.errorProvider.SetIconPadding((Control) this.nudPreambleSize, 6);
      this.nudPreambleSize.Location = new Point(163, 27);
      this.nudPreambleSize.Margin = new Padding(3, 2, 3, 2);
      NumericUpDownEx numericUpDownEx1 = this.nudPreambleSize;
      int[] bits1 = new int[4];
      bits1[0] = (int) ushort.MaxValue;
      Decimal num1 = new Decimal(bits1);
      numericUpDownEx1.Maximum = num1;
      this.nudPreambleSize.Name = "nudPreambleSize";
      this.nudPreambleSize.Size = new Size(59, 20);
      this.nudPreambleSize.TabIndex = 1;
      NumericUpDownEx numericUpDownEx2 = this.nudPreambleSize;
      int[] bits2 = new int[4];
      bits2[0] = 3;
      Decimal num2 = new Decimal(bits2);
      numericUpDownEx2.Value = num2;
      this.nudPreambleSize.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.nudPreambleSize.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.nudPreambleSize.ValueChanged += new EventHandler(this.nudPreambleSize_ValueChanged);
      this.label12.Anchor = AnchorStyles.None;
      this.label12.AutoSize = true;
      this.label12.Location = new Point(356, 223);
      this.label12.Name = "label12";
      this.label12.Size = new Size(32, 13);
      this.label12.TabIndex = 19;
      this.label12.Text = "bytes";
      this.label12.TextAlign = ContentAlignment.MiddleLeft;
      this.label1.Anchor = AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label1.Location = new Point(3, 30);
      this.label1.Name = "label1";
      this.label1.Size = new Size(75, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Preamble size:";
      this.label1.TextAlign = ContentAlignment.MiddleLeft;
      this.label2.Anchor = AnchorStyles.None;
      this.label2.AutoSize = true;
      this.label2.Location = new Point(356, 30);
      this.label2.Name = "label2";
      this.label2.Size = new Size(32, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "bytes";
      this.label2.TextAlign = ContentAlignment.MiddleLeft;
      this.label18.Anchor = AnchorStyles.Left;
      this.label18.AutoSize = true;
      this.label18.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label18.Location = new Point(3, 29);
      this.label18.Name = "label18";
      this.label18.Size = new Size(116, 13);
      this.label18.TabIndex = 2;
      this.label18.Text = "Address based filtering:";
      this.label18.TextAlign = ContentAlignment.MiddleLeft;
      this.label11.Anchor = AnchorStyles.Left;
      this.label11.AutoSize = true;
      this.label11.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label11.Location = new Point(3, 223);
      this.label11.Name = "label11";
      this.label11.Size = new Size(80, 13);
      this.label11.TabIndex = 17;
      this.label11.Text = "Payload length:";
      this.label11.TextAlign = ContentAlignment.MiddleLeft;
      this.label20.Anchor = AnchorStyles.Left;
      this.label20.AutoSize = true;
      this.label20.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label20.Location = new Point(3, 77);
      this.label20.Name = "label20";
      this.label20.Size = new Size(98, 13);
      this.label20.TabIndex = 5;
      this.label20.Text = "Broadcast address:";
      this.label20.TextAlign = ContentAlignment.MiddleLeft;
      this.label10.Anchor = AnchorStyles.Left;
      this.label10.AutoSize = true;
      this.label10.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label10.Location = new Point(3, 199);
      this.label10.Name = "label10";
      this.label10.Size = new Size(76, 13);
      this.label10.TabIndex = 15;
      this.label10.Text = "Packet format:";
      this.label10.TextAlign = ContentAlignment.MiddleLeft;
      this.label21.Anchor = AnchorStyles.Left;
      this.label21.AutoSize = true;
      this.label21.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label21.Location = new Point(3, 101);
      this.label21.Name = "label21";
      this.label21.Size = new Size(46, 13);
      this.label21.TabIndex = 6;
      this.label21.Text = "DC-free:";
      this.label21.TextAlign = ContentAlignment.MiddleLeft;
      this.label19.Anchor = AnchorStyles.Left;
      this.label19.AutoSize = true;
      this.label19.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label19.Location = new Point(3, 53);
      this.label19.Name = "label19";
      this.label19.Size = new Size(76, 13);
      this.label19.TabIndex = 4;
      this.label19.Text = "Node address:";
      this.label19.TextAlign = ContentAlignment.MiddleLeft;
      this.label27.Anchor = AnchorStyles.Left;
      this.label27.AutoSize = true;
      this.label27.Location = new Point(3, 222);
      this.label27.Name = "label27";
      this.label27.Size = new Size(83, 13);
      this.label27.TabIndex = 18;
      this.label27.Text = "FIFO Threshold:";
      this.label27.TextAlign = ContentAlignment.MiddleLeft;
      this.label26.Anchor = AnchorStyles.Left;
      this.label26.AutoSize = true;
      this.label26.Location = new Point(3, 198);
      this.label26.Name = "label26";
      this.label26.Size = new Size(91, 13);
      this.label26.TabIndex = 16;
      this.label26.Text = "Tx start condition:";
      this.label26.TextAlign = ContentAlignment.MiddleLeft;
      this.pnlDcFree.Anchor = AnchorStyles.Left;
      this.pnlDcFree.AutoSize = true;
      this.pnlDcFree.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.pnlDcFree.Controls.Add((Control) this.rBtnDcFreeWhitening);
      this.pnlDcFree.Controls.Add((Control) this.rBtnDcFreeManchester);
      this.pnlDcFree.Controls.Add((Control) this.rBtnDcFreeOff);
      this.pnlDcFree.Location = new Point(129, 98);
      this.pnlDcFree.Margin = new Padding(3, 2, 3, 2);
      this.pnlDcFree.Name = "pnlDcFree";
      this.pnlDcFree.Size = new Size(217, 20);
      this.pnlDcFree.TabIndex = 7;
      this.pnlDcFree.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.pnlDcFree.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnDcFreeWhitening.AutoSize = true;
      this.rBtnDcFreeWhitening.Location = new Point(141, 3);
      this.rBtnDcFreeWhitening.Margin = new Padding(3, 0, 3, 0);
      this.rBtnDcFreeWhitening.Name = "rBtnDcFreeWhitening";
      this.rBtnDcFreeWhitening.Size = new Size(73, 17);
      this.rBtnDcFreeWhitening.TabIndex = 2;
      this.rBtnDcFreeWhitening.Text = "Whitening";
      this.rBtnDcFreeWhitening.UseVisualStyleBackColor = true;
      this.rBtnDcFreeWhitening.CheckedChanged += new EventHandler(this.rBtnDcFreeWhitening_CheckedChanged);
      this.rBtnDcFreeWhitening.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnDcFreeWhitening.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnDcFreeManchester.AutoSize = true;
      this.rBtnDcFreeManchester.Location = new Point(54, 3);
      this.rBtnDcFreeManchester.Margin = new Padding(3, 0, 3, 0);
      this.rBtnDcFreeManchester.Name = "rBtnDcFreeManchester";
      this.rBtnDcFreeManchester.Size = new Size(81, 17);
      this.rBtnDcFreeManchester.TabIndex = 1;
      this.rBtnDcFreeManchester.Text = "Manchester";
      this.rBtnDcFreeManchester.UseVisualStyleBackColor = true;
      this.rBtnDcFreeManchester.CheckedChanged += new EventHandler(this.rBtnDcFreeManchester_CheckedChanged);
      this.rBtnDcFreeManchester.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnDcFreeManchester.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnDcFreeOff.AutoSize = true;
      this.rBtnDcFreeOff.Checked = true;
      this.rBtnDcFreeOff.Location = new Point(3, 3);
      this.rBtnDcFreeOff.Margin = new Padding(3, 0, 3, 0);
      this.rBtnDcFreeOff.Name = "rBtnDcFreeOff";
      this.rBtnDcFreeOff.Size = new Size(45, 17);
      this.rBtnDcFreeOff.TabIndex = 0;
      this.rBtnDcFreeOff.TabStop = true;
      this.rBtnDcFreeOff.Text = "OFF";
      this.rBtnDcFreeOff.UseVisualStyleBackColor = true;
      this.rBtnDcFreeOff.CheckedChanged += new EventHandler(this.rBtnDcFreeOff_CheckedChanged);
      this.rBtnDcFreeOff.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnDcFreeOff.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.pnlAddressInPayload.Anchor = AnchorStyles.Left;
      this.pnlAddressInPayload.AutoSize = true;
      this.pnlAddressInPayload.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.pnlAddressInPayload.Controls.Add((Control) this.rBtnNodeAddressInPayloadNo);
      this.pnlAddressInPayload.Controls.Add((Control) this.rBtnNodeAddressInPayloadYes);
      this.pnlAddressInPayload.Location = new Point(129, 2);
      this.pnlAddressInPayload.Margin = new Padding(3, 2, 3, 2);
      this.pnlAddressInPayload.Name = "pnlAddressInPayload";
      this.pnlAddressInPayload.Size = new Size(98, 20);
      this.pnlAddressInPayload.TabIndex = 1;
      this.pnlAddressInPayload.Visible = false;
      this.pnlAddressInPayload.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.pnlAddressInPayload.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnNodeAddressInPayloadNo.AutoSize = true;
      this.rBtnNodeAddressInPayloadNo.Location = new Point(54, 3);
      this.rBtnNodeAddressInPayloadNo.Margin = new Padding(3, 0, 3, 0);
      this.rBtnNodeAddressInPayloadNo.Name = "rBtnNodeAddressInPayloadNo";
      this.rBtnNodeAddressInPayloadNo.Size = new Size(41, 17);
      this.rBtnNodeAddressInPayloadNo.TabIndex = 1;
      this.rBtnNodeAddressInPayloadNo.Text = "NO";
      this.rBtnNodeAddressInPayloadNo.UseVisualStyleBackColor = true;
      this.rBtnNodeAddressInPayloadNo.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnNodeAddressInPayloadNo.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnNodeAddressInPayloadYes.AutoSize = true;
      this.rBtnNodeAddressInPayloadYes.Checked = true;
      this.rBtnNodeAddressInPayloadYes.Location = new Point(3, 3);
      this.rBtnNodeAddressInPayloadYes.Margin = new Padding(3, 0, 3, 0);
      this.rBtnNodeAddressInPayloadYes.Name = "rBtnNodeAddressInPayloadYes";
      this.rBtnNodeAddressInPayloadYes.Size = new Size(46, 17);
      this.rBtnNodeAddressInPayloadYes.TabIndex = 0;
      this.rBtnNodeAddressInPayloadYes.TabStop = true;
      this.rBtnNodeAddressInPayloadYes.Text = "YES";
      this.rBtnNodeAddressInPayloadYes.UseVisualStyleBackColor = true;
      this.rBtnNodeAddressInPayloadYes.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnNodeAddressInPayloadYes.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.label17.Anchor = AnchorStyles.Left;
      this.label17.AutoSize = true;
      this.label17.Location = new Point(3, 5);
      this.label17.Name = "label17";
      this.label17.Size = new Size(120, 13);
      this.label17.TabIndex = 0;
      this.label17.Text = "Add address in payload:";
      this.label17.TextAlign = ContentAlignment.MiddleLeft;
      this.label17.Visible = false;
      this.pnlFifoFillCondition.Anchor = AnchorStyles.Left;
      this.pnlFifoFillCondition.AutoSize = true;
      this.pnlFifoFillCondition.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.pnlFifoFillCondition.Controls.Add((Control) this.rBtnFifoFillAlways);
      this.pnlFifoFillCondition.Controls.Add((Control) this.rBtnFifoFillSyncAddress);
      this.pnlFifoFillCondition.Location = new Point(163, 124);
      this.pnlFifoFillCondition.Margin = new Padding(3, 2, 3, 2);
      this.pnlFifoFillCondition.Name = "pnlFifoFillCondition";
      this.pnlFifoFillCondition.Size = new Size(159, 20);
      this.pnlFifoFillCondition.TabIndex = 6;
      this.pnlFifoFillCondition.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.pnlFifoFillCondition.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnFifoFillAlways.AutoSize = true;
      this.rBtnFifoFillAlways.Location = new Point(98, 3);
      this.rBtnFifoFillAlways.Margin = new Padding(3, 0, 3, 0);
      this.rBtnFifoFillAlways.Name = "rBtnFifoFillAlways";
      this.rBtnFifoFillAlways.Size = new Size(58, 17);
      this.rBtnFifoFillAlways.TabIndex = 1;
      this.rBtnFifoFillAlways.Text = "Always";
      this.rBtnFifoFillAlways.UseVisualStyleBackColor = true;
      this.rBtnFifoFillAlways.CheckedChanged += new EventHandler(this.rBtnFifoFill_CheckedChanged);
      this.rBtnFifoFillAlways.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnFifoFillAlways.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnFifoFillSyncAddress.AutoSize = true;
      this.rBtnFifoFillSyncAddress.Checked = true;
      this.rBtnFifoFillSyncAddress.Location = new Point(3, 3);
      this.rBtnFifoFillSyncAddress.Margin = new Padding(3, 0, 3, 0);
      this.rBtnFifoFillSyncAddress.Name = "rBtnFifoFillSyncAddress";
      this.rBtnFifoFillSyncAddress.Size = new Size(89, 17);
      this.rBtnFifoFillSyncAddress.TabIndex = 0;
      this.rBtnFifoFillSyncAddress.TabStop = true;
      this.rBtnFifoFillSyncAddress.Text = "Sync address";
      this.rBtnFifoFillSyncAddress.UseVisualStyleBackColor = true;
      this.rBtnFifoFillSyncAddress.CheckedChanged += new EventHandler(this.rBtnFifoFill_CheckedChanged);
      this.rBtnFifoFillSyncAddress.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnFifoFillSyncAddress.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.label4.Anchor = AnchorStyles.Left;
      this.label4.AutoSize = true;
      this.label4.Location = new Point(3, (int) sbyte.MaxValue);
      this.label4.Name = "label4";
      this.label4.Size = new Size(91, 13);
      this.label4.TabIndex = 5;
      this.label4.Text = "FIFO fill condition:";
      this.label4.TextAlign = ContentAlignment.MiddleLeft;
      this.pnlSync.Anchor = AnchorStyles.Left;
      this.pnlSync.AutoSize = true;
      this.pnlSync.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.pnlSync.Controls.Add((Control) this.rBtnSyncOff);
      this.pnlSync.Controls.Add((Control) this.rBtnSyncOn);
      this.pnlSync.Location = new Point(163, 100);
      this.pnlSync.Margin = new Padding(3, 2, 3, 2);
      this.pnlSync.Name = "pnlSync";
      this.pnlSync.Size = new Size(98, 20);
      this.pnlSync.TabIndex = 4;
      this.pnlSync.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.pnlSync.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnSyncOff.AutoSize = true;
      this.rBtnSyncOff.Location = new Point(50, 3);
      this.rBtnSyncOff.Margin = new Padding(3, 0, 3, 0);
      this.rBtnSyncOff.Name = "rBtnSyncOff";
      this.rBtnSyncOff.Size = new Size(45, 17);
      this.rBtnSyncOff.TabIndex = 1;
      this.rBtnSyncOff.Text = "OFF";
      this.rBtnSyncOff.UseVisualStyleBackColor = true;
      this.rBtnSyncOff.CheckedChanged += new EventHandler(this.rBtnSyncOn_CheckedChanged);
      this.rBtnSyncOff.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnSyncOff.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnSyncOn.AutoSize = true;
      this.rBtnSyncOn.Checked = true;
      this.rBtnSyncOn.Location = new Point(3, 3);
      this.rBtnSyncOn.Margin = new Padding(3, 0, 3, 0);
      this.rBtnSyncOn.Name = "rBtnSyncOn";
      this.rBtnSyncOn.Size = new Size(41, 17);
      this.rBtnSyncOn.TabIndex = 0;
      this.rBtnSyncOn.TabStop = true;
      this.rBtnSyncOn.Text = "ON";
      this.rBtnSyncOn.UseVisualStyleBackColor = true;
      this.rBtnSyncOn.CheckedChanged += new EventHandler(this.rBtnSyncOn_CheckedChanged);
      this.rBtnSyncOn.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnSyncOn.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.label3.Anchor = AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.label3.Location = new Point(3, 103);
      this.label3.Name = "label3";
      this.label3.Size = new Size(60, 13);
      this.label3.TabIndex = 3;
      this.label3.Text = "Sync word:";
      this.label3.TextAlign = ContentAlignment.MiddleLeft;
      this.label9.Anchor = AnchorStyles.Left;
      this.label9.AutoSize = true;
      this.label9.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label9.Location = new Point(3, 175);
      this.label9.Name = "label9";
      this.label9.Size = new Size(89, 13);
      this.label9.TabIndex = 13;
      this.label9.Text = "Sync word value:";
      this.label9.TextAlign = ContentAlignment.MiddleLeft;
      this.pnlCrcAutoClear.Anchor = AnchorStyles.Left;
      this.pnlCrcAutoClear.AutoSize = true;
      this.pnlCrcAutoClear.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.pnlCrcAutoClear.Controls.Add((Control) this.rBtnCrcAutoClearOff);
      this.pnlCrcAutoClear.Controls.Add((Control) this.rBtnCrcAutoClearOn);
      this.pnlCrcAutoClear.Location = new Point(129, 146);
      this.pnlCrcAutoClear.Margin = new Padding(3, 2, 3, 2);
      this.pnlCrcAutoClear.Name = "pnlCrcAutoClear";
      this.pnlCrcAutoClear.Size = new Size(102, 20);
      this.pnlCrcAutoClear.TabIndex = 11;
      this.pnlCrcAutoClear.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.pnlCrcAutoClear.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnCrcAutoClearOff.AutoSize = true;
      this.rBtnCrcAutoClearOff.Location = new Point(54, 3);
      this.rBtnCrcAutoClearOff.Margin = new Padding(3, 0, 3, 0);
      this.rBtnCrcAutoClearOff.Name = "rBtnCrcAutoClearOff";
      this.rBtnCrcAutoClearOff.Size = new Size(45, 17);
      this.rBtnCrcAutoClearOff.TabIndex = 1;
      this.rBtnCrcAutoClearOff.Text = "OFF";
      this.rBtnCrcAutoClearOff.UseVisualStyleBackColor = true;
      this.rBtnCrcAutoClearOff.CheckedChanged += new EventHandler(this.rBtnCrcAutoClearOff_CheckedChanged);
      this.rBtnCrcAutoClearOff.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnCrcAutoClearOff.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnCrcAutoClearOn.AutoSize = true;
      this.rBtnCrcAutoClearOn.Checked = true;
      this.rBtnCrcAutoClearOn.Location = new Point(3, 3);
      this.rBtnCrcAutoClearOn.Margin = new Padding(3, 0, 3, 0);
      this.rBtnCrcAutoClearOn.Name = "rBtnCrcAutoClearOn";
      this.rBtnCrcAutoClearOn.Size = new Size(41, 17);
      this.rBtnCrcAutoClearOn.TabIndex = 0;
      this.rBtnCrcAutoClearOn.TabStop = true;
      this.rBtnCrcAutoClearOn.Text = "ON";
      this.rBtnCrcAutoClearOn.UseVisualStyleBackColor = true;
      this.rBtnCrcAutoClearOn.CheckedChanged += new EventHandler(this.rBtnCrcAutoClearOn_CheckedChanged);
      this.rBtnCrcAutoClearOn.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnCrcAutoClearOn.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.label23.Anchor = AnchorStyles.Left;
      this.label23.AutoSize = true;
      this.label23.Location = new Point(3, 149);
      this.label23.Name = "label23";
      this.label23.Size = new Size(82, 13);
      this.label23.TabIndex = 10;
      this.label23.Text = "CRC auto clear:";
      this.label23.TextAlign = ContentAlignment.MiddleLeft;
      this.pnlCrcCalculation.Anchor = AnchorStyles.Left;
      this.pnlCrcCalculation.AutoSize = true;
      this.pnlCrcCalculation.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.pnlCrcCalculation.Controls.Add((Control) this.rBtnCrcOff);
      this.pnlCrcCalculation.Controls.Add((Control) this.rBtnCrcOn);
      this.pnlCrcCalculation.Location = new Point(129, 122);
      this.pnlCrcCalculation.Margin = new Padding(3, 2, 3, 2);
      this.pnlCrcCalculation.Name = "pnlCrcCalculation";
      this.pnlCrcCalculation.Size = new Size(102, 20);
      this.pnlCrcCalculation.TabIndex = 9;
      this.pnlCrcCalculation.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.pnlCrcCalculation.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnCrcOff.AutoSize = true;
      this.rBtnCrcOff.Location = new Point(54, 3);
      this.rBtnCrcOff.Margin = new Padding(3, 0, 3, 0);
      this.rBtnCrcOff.Name = "rBtnCrcOff";
      this.rBtnCrcOff.Size = new Size(45, 17);
      this.rBtnCrcOff.TabIndex = 1;
      this.rBtnCrcOff.Text = "OFF";
      this.rBtnCrcOff.UseVisualStyleBackColor = true;
      this.rBtnCrcOff.CheckedChanged += new EventHandler(this.rBtnCrcOff_CheckedChanged);
      this.rBtnCrcOff.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnCrcOff.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnCrcOn.AutoSize = true;
      this.rBtnCrcOn.Checked = true;
      this.rBtnCrcOn.Location = new Point(3, 3);
      this.rBtnCrcOn.Margin = new Padding(3, 0, 3, 0);
      this.rBtnCrcOn.Name = "rBtnCrcOn";
      this.rBtnCrcOn.Size = new Size(41, 17);
      this.rBtnCrcOn.TabIndex = 0;
      this.rBtnCrcOn.TabStop = true;
      this.rBtnCrcOn.Text = "ON";
      this.rBtnCrcOn.UseVisualStyleBackColor = true;
      this.rBtnCrcOn.CheckedChanged += new EventHandler(this.rBtnCrcOn_CheckedChanged);
      this.rBtnCrcOn.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnCrcOn.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.label22.Anchor = AnchorStyles.Left;
      this.label22.AutoSize = true;
      this.label22.Location = new Point(3, 125);
      this.label22.Name = "label22";
      this.label22.Size = new Size(86, 13);
      this.label22.TabIndex = 8;
      this.label22.Text = "CRC calculation:";
      this.label22.TextAlign = ContentAlignment.MiddleLeft;
      this.pnlTxStart.Anchor = AnchorStyles.Left;
      this.pnlTxStart.AutoSize = true;
      this.pnlTxStart.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.pnlTxStart.Controls.Add((Control) this.rBtnTxStartFifoNotEmpty);
      this.pnlTxStart.Controls.Add((Control) this.rBtnTxStartFifoLevel);
      this.pnlTxStart.Location = new Point(129, 195);
      this.pnlTxStart.Margin = new Padding(3, 3, 3, 2);
      this.pnlTxStart.Name = "pnlTxStart";
      this.pnlTxStart.Size = new Size(168, 20);
      this.pnlTxStart.TabIndex = 17;
      this.pnlTxStart.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.pnlTxStart.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnTxStartFifoNotEmpty.AutoSize = true;
      this.rBtnTxStartFifoNotEmpty.Checked = true;
      this.rBtnTxStartFifoNotEmpty.Location = new Point(77, 3);
      this.rBtnTxStartFifoNotEmpty.Margin = new Padding(3, 0, 3, 0);
      this.rBtnTxStartFifoNotEmpty.Name = "rBtnTxStartFifoNotEmpty";
      this.rBtnTxStartFifoNotEmpty.Size = new Size(88, 17);
      this.rBtnTxStartFifoNotEmpty.TabIndex = 1;
      this.rBtnTxStartFifoNotEmpty.TabStop = true;
      this.rBtnTxStartFifoNotEmpty.Text = "FifoNotEmpty";
      this.rBtnTxStartFifoNotEmpty.UseVisualStyleBackColor = true;
      this.rBtnTxStartFifoNotEmpty.CheckedChanged += new EventHandler(this.rBtnTxStartFifoNotEmpty_CheckedChanged);
      this.rBtnTxStartFifoNotEmpty.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnTxStartFifoNotEmpty.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnTxStartFifoLevel.AutoSize = true;
      this.rBtnTxStartFifoLevel.Location = new Point(3, 3);
      this.rBtnTxStartFifoLevel.Margin = new Padding(3, 0, 3, 0);
      this.rBtnTxStartFifoLevel.Name = "rBtnTxStartFifoLevel";
      this.rBtnTxStartFifoLevel.Size = new Size(68, 17);
      this.rBtnTxStartFifoLevel.TabIndex = 0;
      this.rBtnTxStartFifoLevel.Text = "FifoLevel";
      this.rBtnTxStartFifoLevel.UseVisualStyleBackColor = true;
      this.rBtnTxStartFifoLevel.CheckedChanged += new EventHandler(this.rBtnTxStartFifoLevel_CheckedChanged);
      this.rBtnTxStartFifoLevel.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnTxStartFifoLevel.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.pnlAddressFiltering.Anchor = AnchorStyles.Left;
      this.pnlAddressFiltering.AutoSize = true;
      this.pnlAddressFiltering.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.pnlAddressFiltering.Controls.Add((Control) this.rBtnAddressFilteringNodeBroadcast);
      this.pnlAddressFiltering.Controls.Add((Control) this.rBtnAddressFilteringNode);
      this.pnlAddressFiltering.Controls.Add((Control) this.rBtnAddressFilteringOff);
      this.pnlAddressFiltering.Location = new Point(129, 26);
      this.pnlAddressFiltering.Margin = new Padding(3, 2, 3, 2);
      this.pnlAddressFiltering.Name = "pnlAddressFiltering";
      this.pnlAddressFiltering.Size = new Size(228, 20);
      this.pnlAddressFiltering.TabIndex = 3;
      this.pnlAddressFiltering.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.pnlAddressFiltering.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnAddressFilteringNodeBroadcast.AutoSize = true;
      this.rBtnAddressFilteringNodeBroadcast.Location = new Point(111, 3);
      this.rBtnAddressFilteringNodeBroadcast.Margin = new Padding(3, 0, 3, 0);
      this.rBtnAddressFilteringNodeBroadcast.Name = "rBtnAddressFilteringNodeBroadcast";
      this.rBtnAddressFilteringNodeBroadcast.Size = new Size(114, 17);
      this.rBtnAddressFilteringNodeBroadcast.TabIndex = 2;
      this.rBtnAddressFilteringNodeBroadcast.Text = "Node or Broadcast";
      this.rBtnAddressFilteringNodeBroadcast.UseVisualStyleBackColor = true;
      this.rBtnAddressFilteringNodeBroadcast.CheckedChanged += new EventHandler(this.rBtnAddressFilteringNodeBroadcast_CheckedChanged);
      this.rBtnAddressFilteringNodeBroadcast.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnAddressFilteringNodeBroadcast.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnAddressFilteringNode.AutoSize = true;
      this.rBtnAddressFilteringNode.Location = new Point(54, 3);
      this.rBtnAddressFilteringNode.Margin = new Padding(3, 0, 3, 0);
      this.rBtnAddressFilteringNode.Name = "rBtnAddressFilteringNode";
      this.rBtnAddressFilteringNode.Size = new Size(51, 17);
      this.rBtnAddressFilteringNode.TabIndex = 1;
      this.rBtnAddressFilteringNode.Text = "Node";
      this.rBtnAddressFilteringNode.UseVisualStyleBackColor = true;
      this.rBtnAddressFilteringNode.CheckedChanged += new EventHandler(this.rBtnAddressFilteringNode_CheckedChanged);
      this.rBtnAddressFilteringNode.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnAddressFilteringNode.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnAddressFilteringOff.AutoSize = true;
      this.rBtnAddressFilteringOff.Checked = true;
      this.rBtnAddressFilteringOff.Location = new Point(3, 3);
      this.rBtnAddressFilteringOff.Margin = new Padding(3, 0, 3, 0);
      this.rBtnAddressFilteringOff.Name = "rBtnAddressFilteringOff";
      this.rBtnAddressFilteringOff.Size = new Size(45, 17);
      this.rBtnAddressFilteringOff.TabIndex = 0;
      this.rBtnAddressFilteringOff.TabStop = true;
      this.rBtnAddressFilteringOff.Text = "OFF";
      this.rBtnAddressFilteringOff.UseVisualStyleBackColor = true;
      this.rBtnAddressFilteringOff.CheckedChanged += new EventHandler(this.rBtnAddressFilteringOff_CheckedChanged);
      this.rBtnAddressFilteringOff.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnAddressFilteringOff.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.lblNodeAddress.BorderStyle = BorderStyle.Fixed3D;
      this.lblNodeAddress.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.lblNodeAddress.Location = new Point(65, 0);
      this.lblNodeAddress.Name = "lblNodeAddress";
      this.lblNodeAddress.Size = new Size(59, 20);
      this.lblNodeAddress.TabIndex = 1;
      this.lblNodeAddress.Text = "0x00";
      this.lblNodeAddress.TextAlign = ContentAlignment.MiddleCenter;
      this.lblNodeAddress.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.lblNodeAddress.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.lblPayloadLength.BorderStyle = BorderStyle.Fixed3D;
      this.lblPayloadLength.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.lblPayloadLength.Location = new Point(65, 0);
      this.lblPayloadLength.Name = "lblPayloadLength";
      this.lblPayloadLength.Size = new Size(59, 20);
      this.lblPayloadLength.TabIndex = 1;
      this.lblPayloadLength.Text = "0x00";
      this.lblPayloadLength.TextAlign = ContentAlignment.MiddleCenter;
      this.lblPayloadLength.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.lblPayloadLength.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.lblBroadcastAddress.BorderStyle = BorderStyle.Fixed3D;
      this.lblBroadcastAddress.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.lblBroadcastAddress.Location = new Point(65, 0);
      this.lblBroadcastAddress.Name = "lblBroadcastAddress";
      this.lblBroadcastAddress.Size = new Size(59, 20);
      this.lblBroadcastAddress.TabIndex = 1;
      this.lblBroadcastAddress.Text = "0x00";
      this.lblBroadcastAddress.TextAlign = ContentAlignment.MiddleCenter;
      this.lblBroadcastAddress.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.lblBroadcastAddress.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.pnlPacketFormat.Anchor = AnchorStyles.Left;
      this.pnlPacketFormat.AutoSize = true;
      this.pnlPacketFormat.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.pnlPacketFormat.Controls.Add((Control) this.rBtnPacketFormatFixed);
      this.pnlPacketFormat.Controls.Add((Control) this.rBtnPacketFormatVariable);
      this.pnlPacketFormat.Location = new Point(163, 196);
      this.pnlPacketFormat.Margin = new Padding(3, 2, 3, 2);
      this.pnlPacketFormat.Name = "pnlPacketFormat";
      this.pnlPacketFormat.Size = new Size(125, 20);
      this.pnlPacketFormat.TabIndex = 16;
      this.pnlPacketFormat.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.pnlPacketFormat.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnPacketFormatFixed.AutoSize = true;
      this.rBtnPacketFormatFixed.Location = new Point(72, 3);
      this.rBtnPacketFormatFixed.Margin = new Padding(3, 0, 3, 0);
      this.rBtnPacketFormatFixed.Name = "rBtnPacketFormatFixed";
      this.rBtnPacketFormatFixed.Size = new Size(50, 17);
      this.rBtnPacketFormatFixed.TabIndex = 1;
      this.rBtnPacketFormatFixed.Text = "Fixed";
      this.rBtnPacketFormatFixed.UseVisualStyleBackColor = true;
      this.rBtnPacketFormatFixed.CheckedChanged += new EventHandler(this.rBtnPacketFormat_CheckedChanged);
      this.rBtnPacketFormatFixed.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnPacketFormatFixed.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnPacketFormatVariable.AutoSize = true;
      this.rBtnPacketFormatVariable.Checked = true;
      this.rBtnPacketFormatVariable.Location = new Point(3, 3);
      this.rBtnPacketFormatVariable.Margin = new Padding(3, 0, 3, 0);
      this.rBtnPacketFormatVariable.Name = "rBtnPacketFormatVariable";
      this.rBtnPacketFormatVariable.Size = new Size(63, 17);
      this.rBtnPacketFormatVariable.TabIndex = 0;
      this.rBtnPacketFormatVariable.TabStop = true;
      this.rBtnPacketFormatVariable.Text = "Variable";
      this.rBtnPacketFormatVariable.UseVisualStyleBackColor = true;
      this.rBtnPacketFormatVariable.CheckedChanged += new EventHandler(this.rBtnPacketFormat_CheckedChanged);
      this.rBtnPacketFormatVariable.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnPacketFormatVariable.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.tableLayoutPanel1.AutoSize = true;
      this.tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.tableLayoutPanel1.ColumnCount = 3;
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
      this.tableLayoutPanel1.Controls.Add((Control) this.pnlPayloadLength, 1, 9);
      this.tableLayoutPanel1.Controls.Add((Control) this.label1, 0, 1);
      this.tableLayoutPanel1.Controls.Add((Control) this.pnlPacketFormat, 1, 8);
      this.tableLayoutPanel1.Controls.Add((Control) this.label3, 0, 4);
      this.tableLayoutPanel1.Controls.Add((Control) this.label4, 0, 5);
      this.tableLayoutPanel1.Controls.Add((Control) this.label5, 0, 6);
      this.tableLayoutPanel1.Controls.Add((Control) this.label9, 0, 7);
      this.tableLayoutPanel1.Controls.Add((Control) this.label10, 0, 8);
      this.tableLayoutPanel1.Controls.Add((Control) this.label11, 0, 9);
      this.tableLayoutPanel1.Controls.Add((Control) this.pnlFifoFillCondition, 1, 5);
      this.tableLayoutPanel1.Controls.Add((Control) this.pnlSync, 1, 4);
      this.tableLayoutPanel1.Controls.Add((Control) this.tBoxSyncValue, 1, 7);
      this.tableLayoutPanel1.Controls.Add((Control) this.label12, 2, 9);
      this.tableLayoutPanel1.Controls.Add((Control) this.nudPreambleSize, 1, 1);
      this.tableLayoutPanel1.Controls.Add((Control) this.label2, 2, 1);
      this.tableLayoutPanel1.Controls.Add((Control) this.nudSyncSize, 1, 6);
      this.tableLayoutPanel1.Controls.Add((Control) this.label6, 2, 6);
      this.tableLayoutPanel1.Controls.Add((Control) this.panel1, 1, 3);
      this.tableLayoutPanel1.Controls.Add((Control) this.label7, 0, 3);
      this.tableLayoutPanel1.Controls.Add((Control) this.cBoxDataMode, 1, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label24, 0, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label25, 0, 2);
      this.tableLayoutPanel1.Controls.Add((Control) this.cBoxAutoRestartRxMode, 1, 2);
      this.tableLayoutPanel1.Location = new Point(18, 3);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 10;
      this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel1.Size = new Size(391, 242);
      this.tableLayoutPanel1.TabIndex = 0;
      this.pnlPayloadLength.Anchor = AnchorStyles.Left;
      this.pnlPayloadLength.AutoSize = true;
      this.pnlPayloadLength.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.pnlPayloadLength.Controls.Add((Control) this.lblPayloadLength);
      this.pnlPayloadLength.Controls.Add((Control) this.nudPayloadLength);
      this.pnlPayloadLength.Location = new Point(163, 220);
      this.pnlPayloadLength.Margin = new Padding(3, 2, 3, 2);
      this.pnlPayloadLength.Name = "pnlPayloadLength";
      this.pnlPayloadLength.Size = new Size((int) sbyte.MaxValue, 20);
      this.pnlPayloadLength.TabIndex = 18;
      this.pnlPayloadLength.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.pnlPayloadLength.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.nudPayloadLength.Location = new Point(3, 0);
      this.nudPayloadLength.Margin = new Padding(3, 0, 3, 0);
      NumericUpDownEx numericUpDownEx3 = this.nudPayloadLength;
      int[] bits3 = new int[4];
      bits3[0] = 66;
      Decimal num3 = new Decimal(bits3);
      numericUpDownEx3.Maximum = num3;
      this.nudPayloadLength.Name = "nudPayloadLength";
      this.nudPayloadLength.Size = new Size(59, 20);
      this.nudPayloadLength.TabIndex = 0;
      NumericUpDownEx numericUpDownEx4 = this.nudPayloadLength;
      int[] bits4 = new int[4];
      bits4[0] = 66;
      Decimal num4 = new Decimal(bits4);
      numericUpDownEx4.Value = num4;
      this.nudPayloadLength.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.nudPayloadLength.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.nudPayloadLength.ValueChanged += new EventHandler(this.nudPayloadLength_ValueChanged);
      this.label5.Anchor = AnchorStyles.Left;
      this.label5.AutoSize = true;
      this.label5.BackColor = Color.Transparent;
      this.label5.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label5.Location = new Point(3, 151);
      this.label5.Name = "label5";
      this.label5.Size = new Size(81, 13);
      this.label5.TabIndex = 7;
      this.label5.Text = "Sync word size:";
      this.label5.TextAlign = ContentAlignment.MiddleLeft;
      this.nudSyncSize.Anchor = AnchorStyles.Left;
      this.nudSyncSize.Location = new Point(163, 148);
      this.nudSyncSize.Margin = new Padding(3, 2, 3, 2);
      NumericUpDownEx numericUpDownEx5 = this.nudSyncSize;
      int[] bits5 = new int[4];
      bits5[0] = 8;
      Decimal num5 = new Decimal(bits5);
      numericUpDownEx5.Maximum = num5;
      NumericUpDownEx numericUpDownEx6 = this.nudSyncSize;
      int[] bits6 = new int[4];
      bits6[0] = 1;
      Decimal num6 = new Decimal(bits6);
      numericUpDownEx6.Minimum = num6;
      this.nudSyncSize.Name = "nudSyncSize";
      this.nudSyncSize.Size = new Size(59, 20);
      this.nudSyncSize.TabIndex = 8;
      NumericUpDownEx numericUpDownEx7 = this.nudSyncSize;
      int[] bits7 = new int[4];
      bits7[0] = 4;
      Decimal num7 = new Decimal(bits7);
      numericUpDownEx7.Value = num7;
      this.nudSyncSize.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.nudSyncSize.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.nudSyncSize.ValueChanged += new EventHandler(this.nudSyncSize_ValueChanged);
      this.label6.Anchor = AnchorStyles.None;
      this.label6.AutoSize = true;
      this.label6.Location = new Point(356, 151);
      this.label6.Name = "label6";
      this.label6.Size = new Size(32, 13);
      this.label6.TabIndex = 9;
      this.label6.Text = "bytes";
      this.label6.TextAlign = ContentAlignment.MiddleLeft;
      this.panel1.Anchor = AnchorStyles.Left;
      this.panel1.AutoSize = true;
      this.panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.panel1.Controls.Add((Control) this.rBtnPreamblePolarity55);
      this.panel1.Controls.Add((Control) this.rBtnPreamblePolarityAA);
      this.panel1.Location = new Point(163, 76);
      this.panel1.Margin = new Padding(3, 2, 3, 2);
      this.panel1.Name = "panel1";
      this.panel1.Size = new Size(110, 20);
      this.panel1.TabIndex = 4;
      this.panel1.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.panel1.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnPreamblePolarity55.AutoSize = true;
      this.rBtnPreamblePolarity55.Location = new Point(59, 1);
      this.rBtnPreamblePolarity55.Margin = new Padding(3, 0, 3, 0);
      this.rBtnPreamblePolarity55.Name = "rBtnPreamblePolarity55";
      this.rBtnPreamblePolarity55.Size = new Size(48, 17);
      this.rBtnPreamblePolarity55.TabIndex = 1;
      this.rBtnPreamblePolarity55.Text = "0x55";
      this.rBtnPreamblePolarity55.UseVisualStyleBackColor = true;
      this.rBtnPreamblePolarity55.CheckedChanged += new EventHandler(this.rBtnPreamblePolarity_CheckedChanged);
      this.rBtnPreamblePolarity55.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnPreamblePolarity55.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnPreamblePolarityAA.AutoSize = true;
      this.rBtnPreamblePolarityAA.Checked = true;
      this.rBtnPreamblePolarityAA.Location = new Point(3, 3);
      this.rBtnPreamblePolarityAA.Margin = new Padding(3, 0, 3, 0);
      this.rBtnPreamblePolarityAA.Name = "rBtnPreamblePolarityAA";
      this.rBtnPreamblePolarityAA.Size = new Size(50, 17);
      this.rBtnPreamblePolarityAA.TabIndex = 0;
      this.rBtnPreamblePolarityAA.TabStop = true;
      this.rBtnPreamblePolarityAA.Text = "0xAA";
      this.rBtnPreamblePolarityAA.UseVisualStyleBackColor = true;
      this.rBtnPreamblePolarityAA.CheckedChanged += new EventHandler(this.rBtnPreamblePolarity_CheckedChanged);
      this.rBtnPreamblePolarityAA.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnPreamblePolarityAA.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.label7.Anchor = AnchorStyles.Left;
      this.label7.AutoSize = true;
      this.label7.Location = new Point(3, 79);
      this.label7.Name = "label7";
      this.label7.Size = new Size(90, 13);
      this.label7.TabIndex = 3;
      this.label7.Text = "Preamble polarity:";
      this.label7.TextAlign = ContentAlignment.MiddleLeft;
      this.cBoxDataMode.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cBoxDataMode.FormattingEnabled = true;
      this.cBoxDataMode.Items.AddRange(new object[2]
      {
        (object) "Continuous",
        (object) "Packet"
      });
      this.cBoxDataMode.Location = new Point(163, 2);
      this.cBoxDataMode.Margin = new Padding(3, 2, 3, 2);
      this.cBoxDataMode.Name = "cBoxDataMode";
      this.cBoxDataMode.Size = new Size(121, 21);
      this.cBoxDataMode.TabIndex = 20;
      this.cBoxDataMode.SelectedIndexChanged += new EventHandler(this.cBoxDataMode_SelectedIndexChanged);
      this.label24.Anchor = AnchorStyles.Left;
      this.label24.AutoSize = true;
      this.label24.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label24.Location = new Point(3, 6);
      this.label24.Name = "label24";
      this.label24.Size = new Size(62, 13);
      this.label24.TabIndex = 0;
      this.label24.Text = "Data mode:";
      this.label24.TextAlign = ContentAlignment.MiddleLeft;
      this.label25.Anchor = AnchorStyles.Left;
      this.label25.AutoSize = true;
      this.label25.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label25.Location = new Point(3, 55);
      this.label25.Name = "label25";
      this.label25.Size = new Size(109, 13);
      this.label25.TabIndex = 0;
      this.label25.Text = "Auto restart Rx mode:";
      this.label25.TextAlign = ContentAlignment.MiddleLeft;
      this.cBoxAutoRestartRxMode.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cBoxAutoRestartRxMode.FormattingEnabled = true;
      this.cBoxAutoRestartRxMode.Items.AddRange(new object[3]
      {
        (object) "OFF",
        (object) "ON, without waiting PLL to re-lock",
        (object) "ON, wait for PLL to lock"
      });
      this.cBoxAutoRestartRxMode.Location = new Point(163, 51);
      this.cBoxAutoRestartRxMode.Margin = new Padding(3, 2, 3, 2);
      this.cBoxAutoRestartRxMode.Name = "cBoxAutoRestartRxMode";
      this.cBoxAutoRestartRxMode.Size = new Size(187, 21);
      this.cBoxAutoRestartRxMode.TabIndex = 20;
      this.cBoxAutoRestartRxMode.SelectedIndexChanged += new EventHandler(this.cBoxAutoRestartRxMode_SelectedIndexChanged);
      this.pnlNodeAddress.Anchor = AnchorStyles.Left;
      this.pnlNodeAddress.AutoSize = true;
      this.pnlNodeAddress.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.pnlNodeAddress.Controls.Add((Control) this.nudNodeAddress);
      this.pnlNodeAddress.Controls.Add((Control) this.lblNodeAddress);
      this.pnlNodeAddress.Location = new Point(129, 50);
      this.pnlNodeAddress.Margin = new Padding(3, 2, 3, 2);
      this.pnlNodeAddress.Name = "pnlNodeAddress";
      this.pnlNodeAddress.Size = new Size((int) sbyte.MaxValue, 20);
      this.pnlNodeAddress.TabIndex = 59;
      this.pnlNodeAddress.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.pnlNodeAddress.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.nudNodeAddress.Location = new Point(0, 0);
      this.nudNodeAddress.Margin = new Padding(3, 0, 3, 0);
      NumericUpDownEx numericUpDownEx8 = this.nudNodeAddress;
      int[] bits8 = new int[4];
      bits8[0] = (int) byte.MaxValue;
      Decimal num8 = new Decimal(bits8);
      numericUpDownEx8.Maximum = num8;
      this.nudNodeAddress.Name = "nudNodeAddress";
      this.nudNodeAddress.Size = new Size(59, 20);
      this.nudNodeAddress.TabIndex = 0;
      this.nudNodeAddress.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.nudNodeAddress.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.nudNodeAddress.ValueChanged += new EventHandler(this.nudNodeAddress_ValueChanged);
      this.pnlBroadcastAddress.Anchor = AnchorStyles.Left;
      this.pnlBroadcastAddress.AutoSize = true;
      this.pnlBroadcastAddress.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.pnlBroadcastAddress.Controls.Add((Control) this.nudBroadcastAddress);
      this.pnlBroadcastAddress.Controls.Add((Control) this.lblBroadcastAddress);
      this.pnlBroadcastAddress.Location = new Point(129, 74);
      this.pnlBroadcastAddress.Margin = new Padding(3, 2, 3, 2);
      this.pnlBroadcastAddress.Name = "pnlBroadcastAddress";
      this.pnlBroadcastAddress.Size = new Size((int) sbyte.MaxValue, 20);
      this.pnlBroadcastAddress.TabIndex = 60;
      this.pnlBroadcastAddress.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.pnlBroadcastAddress.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.nudBroadcastAddress.Location = new Point(0, 0);
      this.nudBroadcastAddress.Margin = new Padding(3, 0, 3, 0);
      NumericUpDownEx numericUpDownEx9 = this.nudBroadcastAddress;
      int[] bits9 = new int[4];
      bits9[0] = (int) byte.MaxValue;
      Decimal num9 = new Decimal(bits9);
      numericUpDownEx9.Maximum = num9;
      this.nudBroadcastAddress.Name = "nudBroadcastAddress";
      this.nudBroadcastAddress.Size = new Size(59, 20);
      this.nudBroadcastAddress.TabIndex = 0;
      this.nudBroadcastAddress.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.nudBroadcastAddress.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.nudBroadcastAddress.ValueChanged += new EventHandler(this.nudBroadcastAddress_ValueChanged);
      this.tableLayoutPanel2.AutoSize = true;
      this.tableLayoutPanel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.tableLayoutPanel2.ColumnCount = 3;
      this.tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
      this.tableLayoutPanel2.Controls.Add((Control) this.label17, 0, 0);
      this.tableLayoutPanel2.Controls.Add((Control) this.pnlBroadcastAddress, 1, 3);
      this.tableLayoutPanel2.Controls.Add((Control) this.pnlTxStart, 1, 8);
      this.tableLayoutPanel2.Controls.Add((Control) this.label18, 0, 1);
      this.tableLayoutPanel2.Controls.Add((Control) this.nudFifoThreshold, 1, 9);
      this.tableLayoutPanel2.Controls.Add((Control) this.pnlCrcAutoClear, 1, 6);
      this.tableLayoutPanel2.Controls.Add((Control) this.pnlNodeAddress, 1, 2);
      this.tableLayoutPanel2.Controls.Add((Control) this.pnlCrcCalculation, 1, 5);
      this.tableLayoutPanel2.Controls.Add((Control) this.label19, 0, 2);
      this.tableLayoutPanel2.Controls.Add((Control) this.pnlDcFree, 1, 4);
      this.tableLayoutPanel2.Controls.Add((Control) this.label20, 0, 3);
      this.tableLayoutPanel2.Controls.Add((Control) this.pnlAddressFiltering, 1, 1);
      this.tableLayoutPanel2.Controls.Add((Control) this.label21, 0, 4);
      this.tableLayoutPanel2.Controls.Add((Control) this.label22, 0, 5);
      this.tableLayoutPanel2.Controls.Add((Control) this.label23, 0, 6);
      this.tableLayoutPanel2.Controls.Add((Control) this.label26, 0, 8);
      this.tableLayoutPanel2.Controls.Add((Control) this.label27, 0, 9);
      this.tableLayoutPanel2.Controls.Add((Control) this.pnlAddressInPayload, 1, 0);
      this.tableLayoutPanel2.Controls.Add((Control) this.label13, 2, 1);
      this.tableLayoutPanel2.Controls.Add((Control) this.panel2, 1, 7);
      this.tableLayoutPanel2.Controls.Add((Control) this.panel3, 1, 10);
      this.tableLayoutPanel2.Controls.Add((Control) this.panel4, 1, 11);
      this.tableLayoutPanel2.Controls.Add((Control) this.panel5, 1, 12);
      this.tableLayoutPanel2.Controls.Add((Control) this.label8, 0, 7);
      this.tableLayoutPanel2.Controls.Add((Control) this.label14, 0, 10);
      this.tableLayoutPanel2.Controls.Add((Control) this.label15, 0, 11);
      this.tableLayoutPanel2.Controls.Add((Control) this.label16, 0, 12);
      this.tableLayoutPanel2.Location = new Point(415, 3);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 13;
      this.tableLayoutPanel2.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel2.Size = new Size(366, 313);
      this.tableLayoutPanel2.TabIndex = 1;
      this.nudFifoThreshold.Anchor = AnchorStyles.Left;
      this.nudFifoThreshold.Location = new Point(129, 219);
      this.nudFifoThreshold.Margin = new Padding(3, 2, 3, 2);
      NumericUpDownEx numericUpDownEx10 = this.nudFifoThreshold;
      int[] bits10 = new int[4];
      bits10[0] = 128;
      Decimal num10 = new Decimal(bits10);
      numericUpDownEx10.Maximum = num10;
      this.nudFifoThreshold.Name = "nudFifoThreshold";
      this.nudFifoThreshold.Size = new Size(59, 20);
      this.nudFifoThreshold.TabIndex = 19;
      this.nudFifoThreshold.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.nudFifoThreshold.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.nudFifoThreshold.ValueChanged += new EventHandler(this.nudFifoThreshold_ValueChanged);
      this.label13.Anchor = AnchorStyles.None;
      this.label13.AutoSize = true;
      this.label13.Location = new Point(363, 29);
      this.label13.Name = "label13";
      this.label13.Size = new Size(0, 13);
      this.label13.TabIndex = 22;
      this.label13.TextAlign = ContentAlignment.MiddleLeft;
      this.panel2.Anchor = AnchorStyles.Left;
      this.panel2.AutoSize = true;
      this.panel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.panel2.Controls.Add((Control) this.rBtnCrcCcitt);
      this.panel2.Controls.Add((Control) this.rBtnCrcIbm);
      this.panel2.Location = new Point(129, 170);
      this.panel2.Margin = new Padding(3, 2, 3, 2);
      this.panel2.Name = "panel2";
      this.panel2.Size = new Size(112, 20);
      this.panel2.TabIndex = 11;
      this.panel2.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.panel2.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnCrcCcitt.AutoSize = true;
      this.rBtnCrcCcitt.Location = new Point(53, 3);
      this.rBtnCrcCcitt.Margin = new Padding(3, 0, 3, 0);
      this.rBtnCrcCcitt.Name = "rBtnCrcCcitt";
      this.rBtnCrcCcitt.Size = new Size(56, 17);
      this.rBtnCrcCcitt.TabIndex = 1;
      this.rBtnCrcCcitt.Text = "CCITT";
      this.rBtnCrcCcitt.UseVisualStyleBackColor = true;
      this.rBtnCrcCcitt.CheckedChanged += new EventHandler(this.rBtnCrcIbm_CheckedChanged);
      this.rBtnCrcCcitt.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnCrcCcitt.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnCrcIbm.AutoSize = true;
      this.rBtnCrcIbm.Checked = true;
      this.rBtnCrcIbm.Location = new Point(3, 3);
      this.rBtnCrcIbm.Margin = new Padding(3, 0, 3, 0);
      this.rBtnCrcIbm.Name = "rBtnCrcIbm";
      this.rBtnCrcIbm.Size = new Size(44, 17);
      this.rBtnCrcIbm.TabIndex = 0;
      this.rBtnCrcIbm.TabStop = true;
      this.rBtnCrcIbm.Text = "IBM";
      this.rBtnCrcIbm.UseVisualStyleBackColor = true;
      this.rBtnCrcIbm.CheckedChanged += new EventHandler(this.rBtnCrcIbm_CheckedChanged);
      this.rBtnCrcIbm.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnCrcIbm.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.panel3.Anchor = AnchorStyles.Left;
      this.panel3.AutoSize = true;
      this.panel3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.panel3.Controls.Add((Control) this.rBtnIoHomeOff);
      this.panel3.Controls.Add((Control) this.rBtnIoHomeOn);
      this.panel3.Location = new Point(129, 243);
      this.panel3.Margin = new Padding(3, 2, 3, 2);
      this.panel3.Name = "panel3";
      this.panel3.Size = new Size(102, 20);
      this.panel3.TabIndex = 11;
      this.panel3.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.panel3.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnIoHomeOff.AutoSize = true;
      this.rBtnIoHomeOff.Location = new Point(54, 3);
      this.rBtnIoHomeOff.Margin = new Padding(3, 0, 3, 0);
      this.rBtnIoHomeOff.Name = "rBtnIoHomeOff";
      this.rBtnIoHomeOff.Size = new Size(45, 17);
      this.rBtnIoHomeOff.TabIndex = 1;
      this.rBtnIoHomeOff.Text = "OFF";
      this.rBtnIoHomeOff.UseVisualStyleBackColor = true;
      this.rBtnIoHomeOff.CheckedChanged += new EventHandler(this.rBtnIoHomeOn_CheckedChanged);
      this.rBtnIoHomeOff.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnIoHomeOff.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnIoHomeOn.AutoSize = true;
      this.rBtnIoHomeOn.Checked = true;
      this.rBtnIoHomeOn.Location = new Point(3, 3);
      this.rBtnIoHomeOn.Margin = new Padding(3, 0, 3, 0);
      this.rBtnIoHomeOn.Name = "rBtnIoHomeOn";
      this.rBtnIoHomeOn.Size = new Size(41, 17);
      this.rBtnIoHomeOn.TabIndex = 0;
      this.rBtnIoHomeOn.TabStop = true;
      this.rBtnIoHomeOn.Text = "ON";
      this.rBtnIoHomeOn.UseVisualStyleBackColor = true;
      this.rBtnIoHomeOn.CheckedChanged += new EventHandler(this.rBtnIoHomeOn_CheckedChanged);
      this.rBtnIoHomeOn.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnIoHomeOn.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.panel4.Anchor = AnchorStyles.Left;
      this.panel4.AutoSize = true;
      this.panel4.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.panel4.Controls.Add((Control) this.rBtnIoHomePwrFrameOff);
      this.panel4.Controls.Add((Control) this.rBtnIoHomePwrFrameOn);
      this.panel4.Location = new Point(129, 267);
      this.panel4.Margin = new Padding(3, 2, 3, 2);
      this.panel4.Name = "panel4";
      this.panel4.Size = new Size(102, 20);
      this.panel4.TabIndex = 11;
      this.panel4.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.panel4.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnIoHomePwrFrameOff.AutoSize = true;
      this.rBtnIoHomePwrFrameOff.Location = new Point(54, 3);
      this.rBtnIoHomePwrFrameOff.Margin = new Padding(3, 0, 3, 0);
      this.rBtnIoHomePwrFrameOff.Name = "rBtnIoHomePwrFrameOff";
      this.rBtnIoHomePwrFrameOff.Size = new Size(45, 17);
      this.rBtnIoHomePwrFrameOff.TabIndex = 1;
      this.rBtnIoHomePwrFrameOff.Text = "OFF";
      this.rBtnIoHomePwrFrameOff.UseVisualStyleBackColor = true;
      this.rBtnIoHomePwrFrameOff.CheckedChanged += new EventHandler(this.rBtnIoHomePwrFrameOn_CheckedChanged);
      this.rBtnIoHomePwrFrameOff.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnIoHomePwrFrameOff.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnIoHomePwrFrameOn.AutoSize = true;
      this.rBtnIoHomePwrFrameOn.Checked = true;
      this.rBtnIoHomePwrFrameOn.Location = new Point(3, 3);
      this.rBtnIoHomePwrFrameOn.Margin = new Padding(3, 0, 3, 0);
      this.rBtnIoHomePwrFrameOn.Name = "rBtnIoHomePwrFrameOn";
      this.rBtnIoHomePwrFrameOn.Size = new Size(41, 17);
      this.rBtnIoHomePwrFrameOn.TabIndex = 0;
      this.rBtnIoHomePwrFrameOn.TabStop = true;
      this.rBtnIoHomePwrFrameOn.Text = "ON";
      this.rBtnIoHomePwrFrameOn.UseVisualStyleBackColor = true;
      this.rBtnIoHomePwrFrameOn.CheckedChanged += new EventHandler(this.rBtnIoHomePwrFrameOn_CheckedChanged);
      this.rBtnIoHomePwrFrameOn.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnIoHomePwrFrameOn.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.panel5.Anchor = AnchorStyles.Left;
      this.panel5.AutoSize = true;
      this.panel5.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.panel5.Controls.Add((Control) this.rBtnBeaconOff);
      this.panel5.Controls.Add((Control) this.rBtnBeaconOn);
      this.panel5.Location = new Point(129, 291);
      this.panel5.Margin = new Padding(3, 2, 3, 2);
      this.panel5.Name = "panel5";
      this.panel5.Size = new Size(102, 20);
      this.panel5.TabIndex = 11;
      this.panel5.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.panel5.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnBeaconOff.AutoSize = true;
      this.rBtnBeaconOff.Location = new Point(54, 3);
      this.rBtnBeaconOff.Margin = new Padding(3, 0, 3, 0);
      this.rBtnBeaconOff.Name = "rBtnBeaconOff";
      this.rBtnBeaconOff.Size = new Size(45, 17);
      this.rBtnBeaconOff.TabIndex = 1;
      this.rBtnBeaconOff.Text = "OFF";
      this.rBtnBeaconOff.UseVisualStyleBackColor = true;
      this.rBtnBeaconOff.CheckedChanged += new EventHandler(this.rBtnBeaconOn_CheckedChanged);
      this.rBtnBeaconOff.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnBeaconOff.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.rBtnBeaconOn.AutoSize = true;
      this.rBtnBeaconOn.Checked = true;
      this.rBtnBeaconOn.Location = new Point(3, 3);
      this.rBtnBeaconOn.Margin = new Padding(3, 0, 3, 0);
      this.rBtnBeaconOn.Name = "rBtnBeaconOn";
      this.rBtnBeaconOn.Size = new Size(41, 17);
      this.rBtnBeaconOn.TabIndex = 0;
      this.rBtnBeaconOn.TabStop = true;
      this.rBtnBeaconOn.Text = "ON";
      this.rBtnBeaconOn.UseVisualStyleBackColor = true;
      this.rBtnBeaconOn.CheckedChanged += new EventHandler(this.rBtnBeaconOn_CheckedChanged);
      this.rBtnBeaconOn.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.rBtnBeaconOn.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.label8.Anchor = AnchorStyles.Left;
      this.label8.AutoSize = true;
      this.label8.Location = new Point(3, 173);
      this.label8.Name = "label8";
      this.label8.Size = new Size(74, 13);
      this.label8.TabIndex = 10;
      this.label8.Text = "CRC polynom:";
      this.label8.TextAlign = ContentAlignment.MiddleLeft;
      this.label14.Anchor = AnchorStyles.Left;
      this.label14.AutoSize = true;
      this.label14.Location = new Point(3, 246);
      this.label14.Name = "label14";
      this.label14.Size = new Size(52, 13);
      this.label14.TabIndex = 10;
      this.label14.Text = "IO Home:";
      this.label14.TextAlign = ContentAlignment.MiddleLeft;
      this.label15.Anchor = AnchorStyles.Left;
      this.label15.AutoSize = true;
      this.label15.Location = new Point(3, 270);
      this.label15.Name = "label15";
      this.label15.Size = new Size(114, 13);
      this.label15.TabIndex = 10;
      this.label15.Text = "IO Home Power frame:";
      this.label15.TextAlign = ContentAlignment.MiddleLeft;
      this.label16.Anchor = AnchorStyles.Left;
      this.label16.AutoSize = true;
      this.label16.Location = new Point(3, 294);
      this.label16.Name = "label16";
      this.label16.Size = new Size(47, 13);
      this.label16.TabIndex = 10;
      this.label16.Text = "Beacon:";
      this.label16.TextAlign = ContentAlignment.MiddleLeft;
      this.gBoxDeviceStatus.Controls.Add((Control) this.lblOperatingMode);
      this.gBoxDeviceStatus.Controls.Add((Control) this.label37);
      this.gBoxDeviceStatus.Controls.Add((Control) this.lblBitSynchroniser);
      this.gBoxDeviceStatus.Controls.Add((Control) this.lblDataMode);
      this.gBoxDeviceStatus.Controls.Add((Control) this.label38);
      this.gBoxDeviceStatus.Controls.Add((Control) this.label39);
      this.gBoxDeviceStatus.Location = new Point(565, 317);
      this.gBoxDeviceStatus.Name = "gBoxDeviceStatus";
      this.gBoxDeviceStatus.Size = new Size(231, 77);
      this.gBoxDeviceStatus.TabIndex = 3;
      this.gBoxDeviceStatus.TabStop = false;
      this.gBoxDeviceStatus.Text = "Device status";
      this.gBoxDeviceStatus.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.gBoxDeviceStatus.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.lblOperatingMode.AutoSize = true;
      this.lblOperatingMode.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.lblOperatingMode.Location = new Point(146, 58);
      this.lblOperatingMode.Margin = new Padding(3);
      this.lblOperatingMode.Name = "lblOperatingMode";
      this.lblOperatingMode.Size = new Size(39, 13);
      this.lblOperatingMode.TabIndex = 5;
      this.lblOperatingMode.Text = "Sleep";
      this.lblOperatingMode.TextAlign = ContentAlignment.MiddleLeft;
      this.label37.AutoSize = true;
      this.label37.Location = new Point(3, 58);
      this.label37.Margin = new Padding(3);
      this.label37.Name = "label37";
      this.label37.Size = new Size(85, 13);
      this.label37.TabIndex = 4;
      this.label37.Text = "Operating mode:";
      this.label37.TextAlign = ContentAlignment.MiddleLeft;
      this.lblBitSynchroniser.AutoSize = true;
      this.lblBitSynchroniser.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.lblBitSynchroniser.Location = new Point(146, 20);
      this.lblBitSynchroniser.Margin = new Padding(3);
      this.lblBitSynchroniser.Name = "lblBitSynchroniser";
      this.lblBitSynchroniser.Size = new Size(25, 13);
      this.lblBitSynchroniser.TabIndex = 1;
      this.lblBitSynchroniser.Text = "ON";
      this.lblBitSynchroniser.TextAlign = ContentAlignment.MiddleLeft;
      this.lblDataMode.AutoSize = true;
      this.lblDataMode.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.lblDataMode.Location = new Point(146, 39);
      this.lblDataMode.Margin = new Padding(3);
      this.lblDataMode.Name = "lblDataMode";
      this.lblDataMode.Size = new Size(47, 13);
      this.lblDataMode.TabIndex = 3;
      this.lblDataMode.Text = "Packet";
      this.lblDataMode.TextAlign = ContentAlignment.MiddleLeft;
      this.label38.AutoSize = true;
      this.label38.Location = new Point(3, 20);
      this.label38.Margin = new Padding(3);
      this.label38.Name = "label38";
      this.label38.Size = new Size(86, 13);
      this.label38.TabIndex = 0;
      this.label38.Text = "Bit Synchronizer:";
      this.label38.TextAlign = ContentAlignment.MiddleLeft;
      this.label39.AutoSize = true;
      this.label39.Location = new Point(3, 39);
      this.label39.Margin = new Padding(3);
      this.label39.Name = "label39";
      this.label39.Size = new Size(62, 13);
      this.label39.TabIndex = 2;
      this.label39.Text = "Data mode:";
      this.label39.TextAlign = ContentAlignment.MiddleLeft;
      this.gBoxControl.Controls.Add((Control) this.btnFillFifo);
      this.gBoxControl.Controls.Add((Control) this.tBoxPacketsNb);
      this.gBoxControl.Controls.Add((Control) this.cBtnLog);
      this.gBoxControl.Controls.Add((Control) this.cBtnPacketHandlerStartStop);
      this.gBoxControl.Controls.Add((Control) this.lblPacketsNb);
      this.gBoxControl.Controls.Add((Control) this.tBoxPacketsRepeatValue);
      this.gBoxControl.Controls.Add((Control) this.lblPacketsRepeatValue);
      this.gBoxControl.Location = new Point(565, 394);
      this.gBoxControl.Name = "gBoxControl";
      this.gBoxControl.Size = new Size(231, 96);
      this.gBoxControl.TabIndex = 4;
      this.gBoxControl.TabStop = false;
      this.gBoxControl.Text = "Control";
      this.gBoxControl.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.gBoxControl.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.btnFillFifo.Location = new Point(168, 19);
      this.btnFillFifo.Name = "btnFillFifo";
      this.btnFillFifo.Size = new Size(57, 23);
      this.btnFillFifo.TabIndex = 5;
      this.btnFillFifo.Text = "Fill FIFO";
      this.btnFillFifo.UseVisualStyleBackColor = true;
      this.btnFillFifo.Click += new EventHandler(this.btnFillFifo_Click);
      this.tBoxPacketsNb.Location = new Point(149, 48);
      this.tBoxPacketsNb.Name = "tBoxPacketsNb";
      this.tBoxPacketsNb.ReadOnly = true;
      this.tBoxPacketsNb.Size = new Size(79, 20);
      this.tBoxPacketsNb.TabIndex = 2;
      this.tBoxPacketsNb.Text = "0";
      this.tBoxPacketsNb.TextAlign = HorizontalAlignment.Right;
      this.cBtnLog.Appearance = Appearance.Button;
      this.cBtnLog.Location = new Point(87, 19);
      this.cBtnLog.Name = "cBtnLog";
      this.cBtnLog.Size = new Size(75, 23);
      this.cBtnLog.TabIndex = 0;
      this.cBtnLog.Text = "Log";
      this.cBtnLog.TextAlign = ContentAlignment.MiddleCenter;
      this.cBtnLog.UseVisualStyleBackColor = true;
      this.cBtnLog.CheckedChanged += new EventHandler(this.cBtnLog_CheckedChanged);
      this.cBtnPacketHandlerStartStop.Appearance = Appearance.Button;
      this.cBtnPacketHandlerStartStop.Location = new Point(6, 19);
      this.cBtnPacketHandlerStartStop.Name = "cBtnPacketHandlerStartStop";
      this.cBtnPacketHandlerStartStop.Size = new Size(75, 23);
      this.cBtnPacketHandlerStartStop.TabIndex = 0;
      this.cBtnPacketHandlerStartStop.Text = "Start";
      this.cBtnPacketHandlerStartStop.TextAlign = ContentAlignment.MiddleCenter;
      this.cBtnPacketHandlerStartStop.UseVisualStyleBackColor = true;
      this.cBtnPacketHandlerStartStop.CheckedChanged += new EventHandler(this.cBtnPacketHandlerStartStop_CheckedChanged);
      this.lblPacketsNb.AutoSize = true;
      this.lblPacketsNb.Location = new Point(3, 51);
      this.lblPacketsNb.Name = "lblPacketsNb";
      this.lblPacketsNb.Size = new Size(64, 13);
      this.lblPacketsNb.TabIndex = 1;
      this.lblPacketsNb.Text = "Tx Packets:";
      this.lblPacketsNb.TextAlign = ContentAlignment.MiddleLeft;
      this.tBoxPacketsRepeatValue.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.tBoxPacketsRepeatValue.Location = new Point(149, 70);
      this.tBoxPacketsRepeatValue.Name = "tBoxPacketsRepeatValue";
      this.tBoxPacketsRepeatValue.Size = new Size(79, 20);
      this.tBoxPacketsRepeatValue.TabIndex = 4;
      this.tBoxPacketsRepeatValue.Text = "0";
      this.tBoxPacketsRepeatValue.TextAlign = HorizontalAlignment.Right;
      this.lblPacketsRepeatValue.AutoSize = true;
      this.lblPacketsRepeatValue.Location = new Point(3, 73);
      this.lblPacketsRepeatValue.Name = "lblPacketsRepeatValue";
      this.lblPacketsRepeatValue.Size = new Size(74, 13);
      this.lblPacketsRepeatValue.TabIndex = 3;
      this.lblPacketsRepeatValue.Text = "Repeat value:";
      this.lblPacketsRepeatValue.TextAlign = ContentAlignment.MiddleLeft;
      this.gBoxPacket.Controls.Add((Control) this.imgPacketMessage);
      this.gBoxPacket.Controls.Add((Control) this.gBoxMessage);
      this.gBoxPacket.Controls.Add((Control) this.tblPacket);
      this.gBoxPacket.Location = new Point(3, 317);
      this.gBoxPacket.Margin = new Padding(3, 1, 3, 1);
      this.gBoxPacket.Name = "gBoxPacket";
      this.gBoxPacket.Size = new Size(557, 172);
      this.gBoxPacket.TabIndex = 2;
      this.gBoxPacket.TabStop = false;
      this.gBoxPacket.Text = "Packet";
      this.gBoxPacket.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.gBoxPacket.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.imgPacketMessage.BackColor = Color.Transparent;
      this.imgPacketMessage.Location = new Point(5, 61);
      this.imgPacketMessage.Margin = new Padding(0);
      this.imgPacketMessage.Name = "imgPacketMessage";
      this.imgPacketMessage.Size = new Size(547, 5);
      this.imgPacketMessage.TabIndex = 1;
      this.imgPacketMessage.Text = "payloadImg1";
      this.gBoxMessage.Controls.Add((Control) this.tblPayloadMessage);
      this.gBoxMessage.Location = new Point(6, 67);
      this.gBoxMessage.Margin = new Padding(1);
      this.gBoxMessage.Name = "gBoxMessage";
      this.gBoxMessage.Size = new Size(547, 101);
      this.gBoxMessage.TabIndex = 2;
      this.gBoxMessage.TabStop = false;
      this.gBoxMessage.Text = "Message";
      this.gBoxMessage.MouseEnter += new EventHandler(this.control_MouseEnter);
      this.gBoxMessage.MouseLeave += new EventHandler(this.control_MouseLeave);
      this.tblPayloadMessage.AutoSize = true;
      this.tblPayloadMessage.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.tblPayloadMessage.ColumnCount = 2;
      this.tblPayloadMessage.ColumnStyles.Add(new ColumnStyle());
      this.tblPayloadMessage.ColumnStyles.Add(new ColumnStyle());
      this.tblPayloadMessage.Controls.Add((Control) this.hexBoxPayload, 0, 1);
      this.tblPayloadMessage.Controls.Add((Control) this.label36, 1, 0);
      this.tblPayloadMessage.Controls.Add((Control) this.label35, 0, 0);
      this.tblPayloadMessage.Location = new Point(20, 19);
      this.tblPayloadMessage.Name = "tblPayloadMessage";
      this.tblPayloadMessage.RowCount = 2;
      this.tblPayloadMessage.RowStyles.Add(new RowStyle());
      this.tblPayloadMessage.RowStyles.Add(new RowStyle());
      this.tblPayloadMessage.Size = new Size(507, 79);
      this.tblPayloadMessage.TabIndex = 0;
      this.hexBoxPayload.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.tblPayloadMessage.SetColumnSpan((Control) this.hexBoxPayload, 2);
      this.hexBoxPayload.Font = new Font("Courier New", 8.25f);
      this.hexBoxPayload.LineInfoDigits = (byte) 2;
      this.hexBoxPayload.LineInfoForeColor = Color.Empty;
      this.hexBoxPayload.Location = new Point(3, 16);
      this.hexBoxPayload.Name = "hexBoxPayload";
      this.hexBoxPayload.ShadowSelectionColor = Color.FromArgb(100, 60, 188, (int) byte.MaxValue);
      this.hexBoxPayload.Size = new Size(501, 60);
      this.hexBoxPayload.StringViewVisible = true;
      this.hexBoxPayload.TabIndex = 2;
      this.hexBoxPayload.UseFixedBytesPerLine = true;
      this.hexBoxPayload.VScrollBarVisible = true;
      this.label36.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.label36.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.label36.Location = new Point(329, 0);
      this.label36.Name = "label36";
      this.label36.Size = new Size(175, 13);
      this.label36.TabIndex = 1;
      this.label36.Text = "ASCII";
      this.label36.TextAlign = ContentAlignment.MiddleCenter;
      this.label35.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.label35.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.label35.Location = new Point(3, 0);
      this.label35.Name = "label35";
      this.label35.Size = new Size(320, 13);
      this.label35.TabIndex = 0;
      this.label35.Text = "HEXADECIMAL";
      this.label35.TextAlign = ContentAlignment.MiddleCenter;
      this.tblPacket.AutoSize = true;
      this.tblPacket.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.tblPacket.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
      this.tblPacket.ColumnCount = 6;
      this.tblPacket.ColumnStyles.Add(new ColumnStyle());
      this.tblPacket.ColumnStyles.Add(new ColumnStyle());
      this.tblPacket.ColumnStyles.Add(new ColumnStyle());
      this.tblPacket.ColumnStyles.Add(new ColumnStyle());
      this.tblPacket.ColumnStyles.Add(new ColumnStyle());
      this.tblPacket.ColumnStyles.Add(new ColumnStyle());
      this.tblPacket.Controls.Add((Control) this.label29, 0, 0);
      this.tblPacket.Controls.Add((Control) this.label30, 1, 0);
      this.tblPacket.Controls.Add((Control) this.label31, 2, 0);
      this.tblPacket.Controls.Add((Control) this.label32, 3, 0);
      this.tblPacket.Controls.Add((Control) this.label33, 4, 0);
      this.tblPacket.Controls.Add((Control) this.label34, 5, 0);
      this.tblPacket.Controls.Add((Control) this.lblPacketPreamble, 0, 1);
      this.tblPacket.Controls.Add((Control) this.lblPayload, 4, 1);
      this.tblPacket.Controls.Add((Control) this.pnlPacketCrc, 5, 1);
      this.tblPacket.Controls.Add((Control) this.pnlPacketAddr, 3, 1);
      this.tblPacket.Controls.Add((Control) this.lblPacketLength, 2, 1);
      this.tblPacket.Controls.Add((Control) this.lblPacketSyncValue, 1, 1);
      this.tblPacket.Location = new Point(5, 17);
      this.tblPacket.Margin = new Padding(1);
      this.tblPacket.Name = "tblPacket";
      this.tblPacket.RowCount = 2;
      this.tblPacket.RowStyles.Add(new RowStyle());
      this.tblPacket.RowStyles.Add(new RowStyle());
      this.tblPacket.Size = new Size(547, 43);
      this.tblPacket.TabIndex = 0;
      this.label29.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.label29.Location = new Point(1, 1);
      this.label29.Margin = new Padding(0);
      this.label29.Name = "label29";
      this.label29.Size = new Size(103, 20);
      this.label29.TabIndex = 0;
      this.label29.Text = "Preamble";
      this.label29.TextAlign = ContentAlignment.MiddleCenter;
      this.label30.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.label30.Location = new Point(108, 1);
      this.label30.Margin = new Padding(0);
      this.label30.Name = "label30";
      this.label30.Size = new Size(152, 20);
      this.label30.TabIndex = 1;
      this.label30.Text = "Sync";
      this.label30.TextAlign = ContentAlignment.MiddleCenter;
      this.label31.BackColor = Color.LightGray;
      this.label31.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.label31.Location = new Point(261, 1);
      this.label31.Margin = new Padding(0);
      this.label31.Name = "label31";
      this.label31.Size = new Size(59, 20);
      this.label31.TabIndex = 2;
      this.label31.Text = "Length";
      this.label31.TextAlign = ContentAlignment.MiddleCenter;
      this.label32.BackColor = Color.LightGray;
      this.label32.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.label32.Location = new Point(321, 1);
      this.label32.Margin = new Padding(0);
      this.label32.Name = "label32";
      this.label32.Size = new Size(87, 20);
      this.label32.TabIndex = 3;
      this.label32.Text = "Node Address";
      this.label32.TextAlign = ContentAlignment.MiddleCenter;
      this.label33.BackColor = Color.LightGray;
      this.label33.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.label33.ForeColor = SystemColors.WindowText;
      this.label33.Location = new Point(409, 1);
      this.label33.Margin = new Padding(0);
      this.label33.Name = "label33";
      this.label33.Size = new Size(85, 20);
      this.label33.TabIndex = 4;
      this.label33.Text = "Message";
      this.label33.TextAlign = ContentAlignment.MiddleCenter;
      this.label34.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.label34.Location = new Point(495, 1);
      this.label34.Margin = new Padding(0);
      this.label34.Name = "label34";
      this.label34.Size = new Size(51, 20);
      this.label34.TabIndex = 5;
      this.label34.Text = "CRC";
      this.label34.TextAlign = ContentAlignment.MiddleCenter;
      this.lblPacketPreamble.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.lblPacketPreamble.Location = new Point(1, 22);
      this.lblPacketPreamble.Margin = new Padding(0);
      this.lblPacketPreamble.Name = "lblPacketPreamble";
      this.lblPacketPreamble.Size = new Size(106, 20);
      this.lblPacketPreamble.TabIndex = 6;
      this.lblPacketPreamble.Text = "55-55-55-55-...-55";
      this.lblPacketPreamble.TextAlign = ContentAlignment.MiddleCenter;
      this.lblPayload.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.lblPayload.Location = new Point(409, 22);
      this.lblPayload.Margin = new Padding(0);
      this.lblPayload.Name = "lblPayload";
      this.lblPayload.Size = new Size(85, 20);
      this.lblPayload.TabIndex = 9;
      this.lblPayload.TextAlign = ContentAlignment.MiddleCenter;
      this.pnlPacketCrc.Controls.Add((Control) this.ledPacketCrc);
      this.pnlPacketCrc.Controls.Add((Control) this.lblPacketCrc);
      this.pnlPacketCrc.Location = new Point(495, 22);
      this.pnlPacketCrc.Margin = new Padding(0);
      this.pnlPacketCrc.Name = "pnlPacketCrc";
      this.pnlPacketCrc.Size = new Size(51, 20);
      this.pnlPacketCrc.TabIndex = 18;
      this.ledPacketCrc.BackColor = Color.Transparent;
      this.ledPacketCrc.LedColor = Color.Green;
      this.ledPacketCrc.LedSize = new Size(11, 11);
      this.ledPacketCrc.Location = new Point(17, 3);
      this.ledPacketCrc.Name = "ledPacketCrc";
      this.ledPacketCrc.Size = new Size(15, 15);
      this.ledPacketCrc.TabIndex = 1;
      this.ledPacketCrc.Text = "CRC";
      this.ledPacketCrc.Visible = false;
      this.lblPacketCrc.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.lblPacketCrc.Location = new Point(0, 0);
      this.lblPacketCrc.Margin = new Padding(0);
      this.lblPacketCrc.Name = "lblPacketCrc";
      this.lblPacketCrc.Size = new Size(51, 20);
      this.lblPacketCrc.TabIndex = 0;
      this.lblPacketCrc.Text = "XX-XX";
      this.lblPacketCrc.TextAlign = ContentAlignment.MiddleCenter;
      this.pnlPacketAddr.Controls.Add((Control) this.lblPacketAddr);
      this.pnlPacketAddr.Location = new Point(321, 22);
      this.pnlPacketAddr.Margin = new Padding(0);
      this.pnlPacketAddr.Name = "pnlPacketAddr";
      this.pnlPacketAddr.Size = new Size(87, 20);
      this.pnlPacketAddr.TabIndex = 11;
      this.lblPacketAddr.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.lblPacketAddr.Location = new Point(0, 0);
      this.lblPacketAddr.Margin = new Padding(0);
      this.lblPacketAddr.Name = "lblPacketAddr";
      this.lblPacketAddr.Size = new Size(87, 20);
      this.lblPacketAddr.TabIndex = 0;
      this.lblPacketAddr.Text = "00";
      this.lblPacketAddr.TextAlign = ContentAlignment.MiddleCenter;
      this.lblPacketLength.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.lblPacketLength.Location = new Point(261, 22);
      this.lblPacketLength.Margin = new Padding(0);
      this.lblPacketLength.Name = "lblPacketLength";
      this.lblPacketLength.Size = new Size(59, 20);
      this.lblPacketLength.TabIndex = 8;
      this.lblPacketLength.Text = "00";
      this.lblPacketLength.TextAlign = ContentAlignment.MiddleCenter;
      this.lblPacketSyncValue.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.lblPacketSyncValue.Location = new Point(108, 22);
      this.lblPacketSyncValue.Margin = new Padding(0);
      this.lblPacketSyncValue.Name = "lblPacketSyncValue";
      this.lblPacketSyncValue.Size = new Size(152, 20);
      this.lblPacketSyncValue.TabIndex = 7;
      this.lblPacketSyncValue.Text = "AA-AA-AA-AA-AA-AA-AA-AA";
      this.lblPacketSyncValue.TextAlign = ContentAlignment.MiddleCenter;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.gBoxDeviceStatus);
      this.Controls.Add((Control) this.tableLayoutPanel2);
      this.Controls.Add((Control) this.tableLayoutPanel1);
      this.Controls.Add((Control) this.gBoxControl);
      this.Controls.Add((Control) this.gBoxPacket);
      this.Name = "PacketHandlerView";
      this.Size = new Size(799, 493);
      ((ISupportInitialize) this.errorProvider).EndInit();
      this.nudPreambleSize.EndInit();
      this.pnlDcFree.ResumeLayout(false);
      this.pnlDcFree.PerformLayout();
      this.pnlAddressInPayload.ResumeLayout(false);
      this.pnlAddressInPayload.PerformLayout();
      this.pnlFifoFillCondition.ResumeLayout(false);
      this.pnlFifoFillCondition.PerformLayout();
      this.pnlSync.ResumeLayout(false);
      this.pnlSync.PerformLayout();
      this.pnlCrcAutoClear.ResumeLayout(false);
      this.pnlCrcAutoClear.PerformLayout();
      this.pnlCrcCalculation.ResumeLayout(false);
      this.pnlCrcCalculation.PerformLayout();
      this.pnlTxStart.ResumeLayout(false);
      this.pnlTxStart.PerformLayout();
      this.pnlAddressFiltering.ResumeLayout(false);
      this.pnlAddressFiltering.PerformLayout();
      this.pnlPacketFormat.ResumeLayout(false);
      this.pnlPacketFormat.PerformLayout();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.pnlPayloadLength.ResumeLayout(false);
      this.nudPayloadLength.EndInit();
      this.nudSyncSize.EndInit();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.pnlNodeAddress.ResumeLayout(false);
      this.nudNodeAddress.EndInit();
      this.pnlBroadcastAddress.ResumeLayout(false);
      this.nudBroadcastAddress.EndInit();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      this.nudFifoThreshold.EndInit();
      this.panel2.ResumeLayout(false);
      this.panel2.PerformLayout();
      this.panel3.ResumeLayout(false);
      this.panel3.PerformLayout();
      this.panel4.ResumeLayout(false);
      this.panel4.PerformLayout();
      this.panel5.ResumeLayout(false);
      this.panel5.PerformLayout();
      this.gBoxDeviceStatus.ResumeLayout(false);
      this.gBoxDeviceStatus.PerformLayout();
      this.gBoxControl.ResumeLayout(false);
      this.gBoxControl.PerformLayout();
      this.gBoxPacket.ResumeLayout(false);
      this.gBoxPacket.PerformLayout();
      this.gBoxMessage.ResumeLayout(false);
      this.gBoxMessage.PerformLayout();
      this.tblPayloadMessage.ResumeLayout(false);
      this.tblPacket.ResumeLayout(false);
      this.pnlPacketCrc.ResumeLayout(false);
      this.pnlPacketAddr.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    private void UpdateControls()
    {
      if (this.DataMode == DataModeEnum.Packet)
      {
        this.lblDataMode.Text = "Packet";
        if (this.mode == OperatingModeEnum.Tx || this.mode == OperatingModeEnum.Rx)
        {
          this.cBtnPacketHandlerStartStop.Enabled = true;
          this.tBoxPacketsNb.Enabled = true;
          if (!this.cBtnPacketHandlerStartStop.Checked)
            this.tBoxPacketsRepeatValue.Enabled = true;
        }
      }
      else
      {
        this.lblDataMode.Text = "Continuous";
        this.cBtnPacketHandlerStartStop.Enabled = false;
        this.tBoxPacketsNb.Enabled = false;
        this.tBoxPacketsRepeatValue.Enabled = false;
      }
      switch (this.Mode)
      {
        case OperatingModeEnum.Sleep:
          this.lblOperatingMode.Text = "Sleep";
          this.nudPayloadLength.Enabled = true;
          this.rBtnNodeAddressInPayloadYes.Enabled = false;
          this.rBtnNodeAddressInPayloadNo.Enabled = false;
          this.lblPacketsNb.Visible = false;
          this.tBoxPacketsNb.Visible = false;
          this.lblPacketsRepeatValue.Visible = false;
          this.tBoxPacketsRepeatValue.Visible = false;
          this.btnFillFifo.Enabled = true;
          break;
        case OperatingModeEnum.Stdby:
          this.lblOperatingMode.Text = "Standby";
          this.nudPayloadLength.Enabled = true;
          this.rBtnNodeAddressInPayloadYes.Enabled = false;
          this.rBtnNodeAddressInPayloadNo.Enabled = false;
          this.lblPacketsNb.Visible = false;
          this.tBoxPacketsNb.Visible = false;
          this.lblPacketsRepeatValue.Visible = false;
          this.tBoxPacketsRepeatValue.Visible = false;
          this.btnFillFifo.Enabled = true;
          break;
        case OperatingModeEnum.FsTx:
          this.lblOperatingMode.Text = "Synthesize Tx";
          this.nudPayloadLength.Enabled = false;
          this.rBtnNodeAddressInPayloadYes.Enabled = false;
          this.rBtnNodeAddressInPayloadNo.Enabled = false;
          this.lblPacketsNb.Visible = false;
          this.tBoxPacketsNb.Visible = false;
          this.lblPacketsRepeatValue.Visible = false;
          this.tBoxPacketsRepeatValue.Visible = false;
          this.btnFillFifo.Enabled = true;
          break;
        case OperatingModeEnum.Tx:
          this.lblOperatingMode.Text = "Transmitter";
          this.nudPayloadLength.Enabled = false;
          this.rBtnNodeAddressInPayloadYes.Enabled = true;
          this.rBtnNodeAddressInPayloadNo.Enabled = true;
          this.lblPacketsNb.Text = "Tx Packets:";
          this.lblPacketsNb.Visible = true;
          this.tBoxPacketsNb.Visible = true;
          this.lblPacketsRepeatValue.Visible = true;
          this.tBoxPacketsRepeatValue.Visible = true;
          this.btnFillFifo.Enabled = false;
          break;
        case OperatingModeEnum.FsRx:
          this.lblOperatingMode.Text = "Synthesize Rx";
          this.nudPayloadLength.Enabled = true;
          this.rBtnNodeAddressInPayloadYes.Enabled = false;
          this.rBtnNodeAddressInPayloadNo.Enabled = false;
          this.lblPacketsNb.Visible = false;
          this.tBoxPacketsNb.Visible = false;
          this.lblPacketsRepeatValue.Visible = false;
          this.tBoxPacketsRepeatValue.Visible = false;
          this.btnFillFifo.Enabled = true;
          break;
        case OperatingModeEnum.Rx:
          this.lblOperatingMode.Text = "Receiver";
          this.nudPayloadLength.Enabled = true;
          this.rBtnNodeAddressInPayloadYes.Enabled = false;
          this.rBtnNodeAddressInPayloadNo.Enabled = false;
          this.lblPacketsNb.Text = "Rx packets:";
          this.lblPacketsNb.Visible = true;
          this.tBoxPacketsNb.Visible = true;
          this.lblPacketsRepeatValue.Visible = false;
          this.tBoxPacketsRepeatValue.Visible = false;
          this.btnFillFifo.Enabled = false;
          break;
      }
    }

    private void OnError(byte status, string message)
    {
      if (this.Error == null)
        return;
      this.Error((object) this, new ErrorEventArgs(status, message));
    }

    private void OnDataModeChanged(DataModeEnum value)
    {
      if (this.DataModeChanged == null)
        return;
      this.DataModeChanged((object) this, new DataModeEventArg(value));
    }

    private void OnPreambleSizeChanged(int value)
    {
      if (this.PreambleSizeChanged == null)
        return;
      this.PreambleSizeChanged((object) this, new Int32EventArg(value));
    }

    private void OnAutoRestartRxChanged(AutoRestartRxEnum value)
    {
      if (this.AutoRestartRxChanged == null)
        return;
      this.AutoRestartRxChanged((object) this, new AutoRestartRxEventArg(value));
    }

    private void OnPreamblePolarityChanged(PreamblePolarityEnum value)
    {
      if (this.PreamblePolarityChanged == null)
        return;
      this.PreamblePolarityChanged((object) this, new PreamblePolarityEventArg(value));
    }

    private void OnFifoFillConditionChanged(FifoFillConditionEnum value)
    {
      if (this.FifoFillConditionChanged == null)
        return;
      this.FifoFillConditionChanged((object) this, new FifoFillConditionEventArg(value));
    }

    private void OnSyncOnChanged(bool value)
    {
      if (this.SyncOnChanged == null)
        return;
      this.SyncOnChanged((object) this, new BooleanEventArg(value));
    }

    private void OnSyncSizeChanged(byte value)
    {
      if (this.SyncSizeChanged == null)
        return;
      this.SyncSizeChanged((object) this, new ByteEventArg(value));
    }

    private void OnSyncValueChanged(byte[] value)
    {
      if (this.SyncValueChanged == null)
        return;
      this.SyncValueChanged((object) this, new ByteArrayEventArg(value));
    }

    private void OnPacketFormatChanged(PacketFormatEnum value)
    {
      if (this.PacketFormatChanged == null)
        return;
      this.PacketFormatChanged((object) this, new PacketFormatEventArg(value));
    }

    private void OnDcFreeChanged(DcFreeEnum value)
    {
      if (this.DcFreeChanged == null)
        return;
      this.DcFreeChanged((object) this, new DcFreeEventArg(value));
    }

    private void OnCrcOnChanged(bool value)
    {
      if (this.CrcOnChanged == null)
        return;
      this.CrcOnChanged((object) this, new BooleanEventArg(value));
    }

    private void OnCrcAutoClearOffChanged(bool value)
    {
      if (this.CrcAutoClearOffChanged == null)
        return;
      this.CrcAutoClearOffChanged((object) this, new BooleanEventArg(value));
    }

    private void OnAddressFilteringChanged(AddressFilteringEnum value)
    {
      if (this.AddressFilteringChanged == null)
        return;
      this.AddressFilteringChanged((object) this, new AddressFilteringEventArg(value));
    }

    private void OnPayloadLengthChanged(short value)
    {
      if (this.PayloadLengthChanged == null)
        return;
      this.PayloadLengthChanged((object) this, new Int16EventArg(value));
    }

    private void OnNodeAddressChanged(byte value)
    {
      if (this.NodeAddressChanged == null)
        return;
      this.NodeAddressChanged((object) this, new ByteEventArg(value));
    }

    private void OnBroadcastAddressChanged(byte value)
    {
      if (this.BroadcastAddressChanged == null)
        return;
      this.BroadcastAddressChanged((object) this, new ByteEventArg(value));
    }

    private void OnTxStartConditionChanged(bool value)
    {
      if (this.TxStartConditionChanged == null)
        return;
      this.TxStartConditionChanged((object) this, new BooleanEventArg(value));
    }

    private void OnFifoThresholdChanged(byte value)
    {
      if (this.FifoThresholdChanged == null)
        return;
      this.FifoThresholdChanged((object) this, new ByteEventArg(value));
    }

    private void OnMessageLengthChanged(int value)
    {
      if (this.MessageLengthChanged == null)
        return;
      this.MessageLengthChanged((object) this, new Int32EventArg(value));
    }

    private void OnMessageChanged(byte[] value)
    {
      if (this.MessageChanged == null)
        return;
      this.MessageChanged((object) this, new ByteArrayEventArg(value));
    }

    private void OnStartStopChanged(bool value)
    {
      if (this.StartStopChanged == null)
        return;
      this.StartStopChanged((object) this, new BooleanEventArg(value));
    }

    private void OnMaxPacketNumberChanged(int value)
    {
      if (this.MaxPacketNumberChanged == null)
        return;
      this.MaxPacketNumberChanged((object) this, new Int32EventArg(value));
    }

    private void OnPacketHandlerLogEnableChanged(bool value)
    {
      if (this.PacketHandlerLogEnableChanged == null)
        return;
      this.PacketHandlerLogEnableChanged((object) this, new BooleanEventArg(value));
    }

    private void OnCrcIbmChanged(bool value)
    {
      if (this.CrcIbmChanged == null)
        return;
      this.CrcIbmChanged((object) this, new BooleanEventArg(value));
    }

    private void OnIoHomeOnChanged(bool value)
    {
      if (this.IoHomeOnChanged == null)
        return;
      this.IoHomeOnChanged((object) this, new BooleanEventArg(value));
    }

    private void OnIoHomePwrFrameOnChanged(bool value)
    {
      if (this.IoHomePwrFrameOnChanged == null)
        return;
      this.IoHomePwrFrameOnChanged((object) this, new BooleanEventArg(value));
    }

    private void OnBeaconOnChanged(bool value)
    {
      if (this.BeaconOnChanged == null)
        return;
      this.BeaconOnChanged((object) this, new BooleanEventArg(value));
    }

    private void OnFillFifoChanged()
    {
      if (this.FillFifoChanged == null)
        return;
      this.FillFifoChanged((object) this, EventArgs.Empty);
    }

    public void UpdateSyncValueLimits(LimitCheckStatusEnum status, string message)
    {
      switch (status)
      {
        case LimitCheckStatusEnum.OK:
          this.tBoxSyncValue.BackColor = SystemColors.Window;
          break;
        case LimitCheckStatusEnum.OUT_OF_RANGE:
          this.tBoxSyncValue.BackColor = ControlPaint.LightLight(Color.Orange);
          break;
        case LimitCheckStatusEnum.ERROR:
          this.tBoxSyncValue.BackColor = ControlPaint.LightLight(Color.Red);
          break;
      }
      this.errorProvider.SetError((Control) this.tBoxSyncValue, message);
    }

    private void cBoxDataMode_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.DataMode = (DataModeEnum) this.cBoxDataMode.SelectedIndex;
      this.OnDataModeChanged(this.DataMode);
    }

    private void nudPreambleSize_ValueChanged(object sender, EventArgs e)
    {
      this.PreambleSize = (int) this.nudPreambleSize.Value;
      this.OnPreambleSizeChanged(this.PreambleSize);
    }

    private void cBoxAutoRestartRxMode_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.OnAutoRestartRxChanged(this.AutoRestartRxOn);
    }

    private void rBtnPreamblePolarity_CheckedChanged(object sender, EventArgs e)
    {
      this.OnPreamblePolarityChanged(this.PreamblePolarity);
    }

    private void rBtnSyncOn_CheckedChanged(object sender, EventArgs e)
    {
      this.SyncOn = this.rBtnSyncOn.Checked;
      this.OnSyncOnChanged(this.SyncOn);
    }

    private void rBtnFifoFill_CheckedChanged(object sender, EventArgs e)
    {
      this.FifoFillCondition = this.rBtnFifoFillSyncAddress.Checked ? FifoFillConditionEnum.OnSyncAddressIrq : FifoFillConditionEnum.Allways;
      this.OnFifoFillConditionChanged(this.FifoFillCondition);
    }

    private void nudSyncSize_ValueChanged(object sender, EventArgs e)
    {
      this.SyncSize = (byte) this.nudSyncSize.Value;
      this.OnSyncSizeChanged(this.SyncSize);
    }

    private void tBoxSyncValue_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
    {
      this.OnError((byte) 1, "Input rejected at position: " + e.Position.ToString((IFormatProvider) CultureInfo.CurrentCulture));
    }

    private void tBoxSyncValue_TypeValidationCompleted(object sender, TypeValidationEventArgs e)
    {
      try
      {
        if (e.IsValidInput)
        {
          if (!(e.ReturnValue is MaskValidationType))
            return;
          this.SyncValue = (e.ReturnValue as MaskValidationType).ArrayValue;
        }
        else
        {
          this.SyncValue = MaskValidationType.InvalidMask.ArrayValue;
          throw new Exception("Wrong Sync word entered.  Message: " + e.Message);
        }
      }
      catch (Exception ex)
      {
        this.OnError((byte) 1, ex.Message);
      }
    }

    private void tBoxSyncValue_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Shift || e.Control || Uri.IsHexDigit((char) e.KeyData) || (e.KeyData >= Keys.NumPad0 && e.KeyData <= Keys.NumPad9 || (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)) || (e.KeyData == Keys.Left || e.KeyData == Keys.Right))
      {
        this.OnError((byte) 0, "-");
      }
      else
      {
        e.Handled = true;
        e.SuppressKeyPress = true;
      }
    }

    private void tBoxSyncValue_TextChanged(object sender, EventArgs e)
    {
      this.OnError((byte) 0, "-");
      this.syncWord.StringValue = this.tBoxSyncValue.Text;
      this.syncValue = this.syncWord.ArrayValue;
      this.lblPacketSyncValue.Text = this.syncWord.StringValue;
    }

    private void rBtnPacketFormat_CheckedChanged(object sender, EventArgs e)
    {
      this.PacketFormat = !this.rBtnPacketFormatVariable.Checked ? PacketFormatEnum.Fixed : PacketFormatEnum.Variable;
      this.OnPacketFormatChanged(this.PacketFormat);
    }

    private void nudPayloadLength_ValueChanged(object sender, EventArgs e)
    {
      this.PayloadLength = (short) (byte) this.nudPayloadLength.Value;
      this.OnPayloadLengthChanged(this.PayloadLength);
    }

    private void rBtnAddressFilteringOff_CheckedChanged(object sender, EventArgs e)
    {
      if (!this.rBtnAddressFilteringOff.Checked)
        return;
      this.AddressFiltering = AddressFilteringEnum.OFF;
      this.OnAddressFilteringChanged(this.AddressFiltering);
    }

    private void rBtnAddressFilteringNode_CheckedChanged(object sender, EventArgs e)
    {
      if (!this.rBtnAddressFilteringNode.Checked)
        return;
      this.AddressFiltering = AddressFilteringEnum.Node;
      this.OnAddressFilteringChanged(this.AddressFiltering);
    }

    private void rBtnAddressFilteringNodeBroadcast_CheckedChanged(object sender, EventArgs e)
    {
      if (!this.rBtnAddressFilteringNodeBroadcast.Checked)
        return;
      this.AddressFiltering = AddressFilteringEnum.NodeBroadcast;
      this.OnAddressFilteringChanged(this.AddressFiltering);
    }

    private void nudNodeAddress_ValueChanged(object sender, EventArgs e)
    {
      this.NodeAddress = (byte) this.nudNodeAddress.Value;
      this.OnNodeAddressChanged(this.NodeAddress);
    }

    private void nudBroadcastAddress_ValueChanged(object sender, EventArgs e)
    {
      this.BroadcastAddress = (byte) this.nudBroadcastAddress.Value;
      this.OnBroadcastAddressChanged(this.BroadcastAddress);
    }

    private void rBtnDcFreeOff_CheckedChanged(object sender, EventArgs e)
    {
      if (!this.rBtnDcFreeOff.Checked)
        return;
      this.DcFree = DcFreeEnum.OFF;
      this.OnDcFreeChanged(this.DcFree);
    }

    private void rBtnDcFreeManchester_CheckedChanged(object sender, EventArgs e)
    {
      if (!this.rBtnDcFreeManchester.Checked)
        return;
      this.DcFree = DcFreeEnum.Manchester;
      this.OnDcFreeChanged(this.DcFree);
    }

    private void rBtnDcFreeWhitening_CheckedChanged(object sender, EventArgs e)
    {
      if (!this.rBtnDcFreeWhitening.Checked)
        return;
      this.DcFree = DcFreeEnum.Whitening;
      this.OnDcFreeChanged(this.DcFree);
    }

    private void rBtnCrcOn_CheckedChanged(object sender, EventArgs e)
    {
      if (!this.rBtnCrcOn.Checked)
        return;
      this.CrcOn = this.rBtnCrcOn.Checked;
      this.OnCrcOnChanged(this.CrcOn);
    }

    private void rBtnCrcOff_CheckedChanged(object sender, EventArgs e)
    {
      if (!this.rBtnCrcOff.Checked)
        return;
      this.CrcOn = this.rBtnCrcOn.Checked;
      this.OnCrcOnChanged(this.CrcOn);
    }

    private void rBtnCrcAutoClearOn_CheckedChanged(object sender, EventArgs e)
    {
      if (!this.rBtnCrcAutoClearOn.Checked)
        return;
      this.CrcAutoClearOff = this.rBtnCrcAutoClearOff.Checked;
      this.OnCrcAutoClearOffChanged(this.CrcAutoClearOff);
    }

    private void rBtnCrcAutoClearOff_CheckedChanged(object sender, EventArgs e)
    {
      if (!this.rBtnCrcAutoClearOff.Checked)
        return;
      this.CrcAutoClearOff = this.rBtnCrcAutoClearOff.Checked;
      this.OnCrcAutoClearOffChanged(this.CrcAutoClearOff);
    }

    private void rBtnCrcIbm_CheckedChanged(object sender, EventArgs e)
    {
      this.OnCrcIbmChanged(this.CrcIbmOn);
    }

    private void tBox_Validated(object sender, EventArgs e)
    {
      if (sender == this.tBoxSyncValue)
      {
        this.tBoxSyncValue.Text = this.tBoxSyncValue.Text.ToUpper();
        this.lblPacketSyncValue.Text = this.tBoxSyncValue.Text;
        this.OnSyncValueChanged(this.SyncValue);
      }
      else
      {
        TextBox textBox = (TextBox) sender;
        if (!(textBox.Text != "") || textBox.Text.StartsWith("0x", true, (CultureInfo) null))
          return;
        textBox.Text = "0x" + Convert.ToByte(textBox.Text, 16).ToString("X02");
      }
    }

    private void rBtnTxStartFifoLevel_CheckedChanged(object sender, EventArgs e)
    {
      if (!this.rBtnTxStartFifoLevel.Checked)
        return;
      this.TxStartCondition = !this.rBtnTxStartFifoLevel.Checked;
      this.OnTxStartConditionChanged(this.TxStartCondition);
    }

    private void rBtnTxStartFifoNotEmpty_CheckedChanged(object sender, EventArgs e)
    {
      if (!this.rBtnTxStartFifoNotEmpty.Checked)
        return;
      this.TxStartCondition = !this.rBtnTxStartFifoLevel.Checked;
      this.OnTxStartConditionChanged(this.TxStartCondition);
    }

    private void nudFifoThreshold_ValueChanged(object sender, EventArgs e)
    {
      this.FifoThreshold = (byte) this.nudFifoThreshold.Value;
      this.OnFifoThresholdChanged(this.FifoThreshold);
    }

    private void rBtnIoHomeOn_CheckedChanged(object sender, EventArgs e)
    {
      this.OnIoHomeOnChanged(this.IoHomeOn);
    }

    private void rBtnIoHomePwrFrameOn_CheckedChanged(object sender, EventArgs e)
    {
      this.OnIoHomePwrFrameOnChanged(this.IoHomePwrFrameOn);
    }

    private void rBtnBeaconOn_CheckedChanged(object sender, EventArgs e)
    {
      this.OnBeaconOnChanged(this.BeaconOn);
    }

    private void hexBoxPayload_DataChanged(object sender, EventArgs e)
    {
      if (this.inHexPayloadDataChanged)
        return;
      this.inHexPayloadDataChanged = true;
      if (this.hexBoxPayload.ByteProvider.Length > 64L)
      {
        this.hexBoxPayload.ByteProvider.DeleteBytes(64L, this.hexBoxPayload.ByteProvider.Length - 64L);
        this.hexBoxPayload.SelectionStart = 64L;
        this.hexBoxPayload.SelectionLength = 0L;
      }
      Array.Resize<byte>(ref this.message, (int) this.hexBoxPayload.ByteProvider.Length);
      for (int index = 0; index < this.message.Length; ++index)
        this.message[index] = this.hexBoxPayload.ByteProvider.ReadByte((long) index);
      this.OnMessageChanged(this.Message);
      this.inHexPayloadDataChanged = false;
    }

    private void cBtnPacketHandlerStartStop_CheckedChanged(object sender, EventArgs e)
    {
      if (this.cBtnPacketHandlerStartStop.Checked)
        this.cBtnPacketHandlerStartStop.Text = "Stop";
      else
        this.cBtnPacketHandlerStartStop.Text = "Start";
      this.tableLayoutPanel1.Enabled = !this.cBtnPacketHandlerStartStop.Checked;
      this.tableLayoutPanel2.Enabled = !this.cBtnPacketHandlerStartStop.Checked;
      this.gBoxPacket.Enabled = !this.cBtnPacketHandlerStartStop.Checked;
      this.tBoxPacketsRepeatValue.Enabled = !this.cBtnPacketHandlerStartStop.Checked;
      try
      {
        this.MaxPacketNumber = Convert.ToInt32(this.tBoxPacketsRepeatValue.Text);
      }
      catch
      {
        this.MaxPacketNumber = 0;
        this.tBoxPacketsRepeatValue.Text = "0";
        this.OnError((byte) 1, "Wrong max packet value! Value has been reseted.");
      }
      this.OnMaxPacketNumberChanged(this.MaxPacketNumber);
      this.OnStartStopChanged(this.cBtnPacketHandlerStartStop.Checked);
    }

    private void cBtnLog_CheckedChanged(object sender, EventArgs e)
    {
      this.OnPacketHandlerLogEnableChanged(this.cBtnLog.Checked);
    }

    private void btnFillFifo_Click(object sender, EventArgs e)
    {
      this.OnFillFifoChanged();
    }

    private void control_MouseEnter(object sender, EventArgs e)
    {
      if (sender == this.nudPreambleSize)
        this.OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Preamble size"));
      else if (sender == this.pnlSync || sender == this.rBtnSyncOn || sender == this.rBtnSyncOff)
        this.OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Sync word"));
      else if (sender == this.pnlFifoFillCondition || sender == this.rBtnFifoFillSyncAddress || sender == this.rBtnFifoFillAlways)
        this.OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Fifo fill condition"));
      else if (sender == this.nudSyncSize)
        this.OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Sync word size"));
      else if (sender == this.tBoxSyncValue)
        this.OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Sync word value"));
      else if (sender == this.pnlPacketFormat || sender == this.rBtnPacketFormatFixed || sender == this.rBtnPacketFormatVariable)
        this.OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Packet format"));
      else if (sender == this.pnlPayloadLength || sender == this.nudPayloadLength || sender == this.lblPayloadLength)
        this.OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Payload length"));
      else if (sender == this.pnlAddressInPayload || sender == this.rBtnNodeAddressInPayloadYes || sender == this.rBtnNodeAddressInPayloadNo)
        this.OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Address in payload"));
      else if (sender == this.pnlAddressFiltering || sender == this.rBtnAddressFilteringOff || (sender == this.rBtnAddressFilteringNode || sender == this.rBtnAddressFilteringNodeBroadcast))
        this.OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Address filtering"));
      else if (sender == this.pnlNodeAddress || sender == this.nudNodeAddress || sender == this.lblNodeAddress)
        this.OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Node address"));
      else if (sender == this.pnlBroadcastAddress || sender == this.nudBroadcastAddress || sender == this.lblBroadcastAddress)
        this.OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Broadcast address"));
      else if (sender == this.pnlDcFree || sender == this.rBtnDcFreeOff || (sender == this.rBtnDcFreeManchester || sender == this.rBtnDcFreeWhitening))
        this.OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Dc free"));
      else if (sender == this.pnlCrcCalculation || sender == this.rBtnCrcOn || sender == this.rBtnCrcOff)
        this.OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Crc calculation"));
      else if (sender == this.pnlCrcAutoClear || sender == this.rBtnCrcAutoClearOn || sender == this.rBtnCrcAutoClearOff)
        this.OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Crc auto clear"));
      else if (sender == this.pnlTxStart || sender == this.rBtnTxStartFifoLevel || sender == this.rBtnTxStartFifoNotEmpty)
        this.OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Tx start"));
      else if (sender == this.nudFifoThreshold)
        this.OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Fifo threshold"));
      else if (sender == this.gBoxControl)
        this.OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Control"));
      else if (sender == this.gBoxPacket)
        this.OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Packet"));
      else if (sender == this.gBoxMessage)
      {
        this.OnDocumentationChanged(new DocumentationChangedEventArgs("Packet handler", "Message"));
      }
      else
      {
        if (sender != this.gBoxDeviceStatus)
          return;
        this.OnDocumentationChanged(new DocumentationChangedEventArgs("Common", "Device status"));
      }
    }

    private void control_MouseLeave(object sender, EventArgs e)
    {
      this.OnDocumentationChanged(new DocumentationChangedEventArgs(".", "Overview"));
    }

    private void OnDocumentationChanged(DocumentationChangedEventArgs e)
    {
      if (this.DocumentationChanged == null)
        return;
      this.DocumentationChanged((object) this, e);
    }
  }
}
