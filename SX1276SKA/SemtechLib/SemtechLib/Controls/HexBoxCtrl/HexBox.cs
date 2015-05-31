using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace SemtechLib.Controls.HexBoxCtrl
{
	[ToolboxBitmap(typeof(HexBox), "HexBox.bmp")]
	public class HexBox : Control
	{
		private System.Windows.Forms.Timer _thumbTrackTimer = new System.Windows.Forms.Timer();
		private int _recBorderLeft = SystemInformation.Border3DSize.Width;
		private int _recBorderRight = SystemInformation.Border3DSize.Width;
		private int _recBorderTop = SystemInformation.Border3DSize.Height;
		private int _recBorderBottom = SystemInformation.Border3DSize.Height;
		private long _bytePos = -1L;
		private string _hexStringFormat = "X";
		private byte lineInfoDigits = (byte)2;
		private Color _backColorDisabled = Color.FromName("WhiteSmoke");
		private int _bytesPerLine = 16;
		private BorderStyle _borderStyle = BorderStyle.Fixed3D;
		private Color _lineInfoForeColor = Color.Empty;
		private Color _selectionBackColor = Color.Blue;
		private Color _selectionForeColor = Color.White;
		private bool _shadowSelectionVisible = true;
		private Color _shadowSelectionColor = Color.FromArgb(100, 60, 188, (int)byte.MaxValue);
		private const int THUMPTRACKDELAY = 50;
		private Rectangle _recContent;
		private Rectangle _recLineInfo;
		private Rectangle _recHex;
		private Rectangle _recStringView;
		private StringFormat _stringFormat;
		private SizeF _charSize;
		private int _iHexMaxHBytes;
		private int _iHexMaxVBytes;
		private int _iHexMaxBytes;
		private long _scrollVmin;
		private long _scrollVmax;
		private long _scrollVpos;
		private VScrollBar _vScrollBar;
		private long _thumbTrackPosition;
		private int _lastThumbtrack;
		private long _startByte;
		private long _endByte;
		private int _byteCharacterPos;
		private HexBox.IKeyInterpreter _keyInterpreter;
		private HexBox.EmptyKeyInterpreter _eki;
		private HexBox.KeyInterpreter _ki;
		private HexBox.StringKeyInterpreter _ski;
		private bool _caretVisible;
		private bool _abortFind;
		private long _findingPos;
		private bool _insertActive;
		private bool _readOnly;
		private bool _useFixedBytesPerLine;
		private bool _vScrollBarVisible;
		private IByteProvider _byteProvider;
		private bool _lineInfoVisible;
		private long _lineInfoOffset;
		private bool _stringViewVisible;
		private long _selectionLength;
		private long _currentLine;
		private int _currentPositionInLine;
		private BuiltInContextMenu _builtInContextMenu;
		private IByteCharConverter _byteCharConverter;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public long CurrentFindingPosition
		{
			get
			{
				return _findingPos;
			}
		}

		public byte LineInfoDigits
		{
			get
			{
				return lineInfoDigits;
			}
			set
			{
				lineInfoDigits = value;
			}
		}

		[DefaultValue(typeof(Color), "White")]
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}

		public override Font Font
		{
			get
			{
				return base.Font;
			}
			set
			{
				base.Font = value;
			}
		}

		[Browsable(false)]
		[Bindable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
			}
		}

		[Bindable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override RightToLeft RightToLeft
		{
			get
			{
				return base.RightToLeft;
			}
			set
			{
				base.RightToLeft = value;
			}
		}

		[DefaultValue(typeof(Color), "WhiteSmoke")]
		[Category("Appearance")]
		public Color BackColorDisabled
		{
			get
			{
				return _backColorDisabled;
			}
			set
			{
				_backColorDisabled = value;
			}
		}

		[Category("Hex")]
		[Description("Gets or sets if the count of bytes in one line is fix.")]
		[DefaultValue(false)]
		public bool ReadOnly
		{
			get
			{
				return _readOnly;
			}
			set
			{
				if (_readOnly == value)
					return;
				_readOnly = value;
				OnReadOnlyChanged(EventArgs.Empty);
				Invalidate();
			}
		}

		[DefaultValue(16)]
		[Category("Hex")]
		[Description("Gets or sets the maximum count of bytes in one line.")]
		public int BytesPerLine
		{
			get
			{
				return _bytesPerLine;
			}
			set
			{
				if (_bytesPerLine == value)
					return;
				_bytesPerLine = value;
				OnBytesPerLineChanged(EventArgs.Empty);
				UpdateRectanglePositioning();
				Invalidate();
			}
		}

		[Category("Hex")]
		[Description("Gets or sets if the count of bytes in one line is fix.")]
		[DefaultValue(false)]
		public bool UseFixedBytesPerLine
		{
			get
			{
				return _useFixedBytesPerLine;
			}
			set
			{
				if (_useFixedBytesPerLine == value)
					return;
				_useFixedBytesPerLine = value;
				OnUseFixedBytesPerLineChanged(EventArgs.Empty);
				UpdateRectanglePositioning();
				Invalidate();
			}
		}

		[DefaultValue(false)]
		[Description("Gets or sets the visibility of a vertical scroll bar.")]
		[Category("Hex")]
		public bool VScrollBarVisible
		{
			get
			{
				return _vScrollBarVisible;
			}
			set
			{
				if (_vScrollBarVisible == value)
					return;
				_vScrollBarVisible = value;
				if (_vScrollBarVisible)
					Controls.Add((Control)_vScrollBar);
				else
					Controls.Remove((Control)_vScrollBar);
				UpdateRectanglePositioning();
				UpdateScrollSize();
				OnVScrollBarVisibleChanged(EventArgs.Empty);
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IByteProvider ByteProvider
		{
			get
			{
				return _byteProvider;
			}
			set
			{
				if (_byteProvider == value)
					return;
				if (value == null)
					ActivateEmptyKeyInterpreter();
				else
					ActivateKeyInterpreter();
				if (_byteProvider != null)
					_byteProvider.LengthChanged -= new EventHandler(_byteProvider_LengthChanged);
				_byteProvider = value;
				if (_byteProvider != null)
					_byteProvider.LengthChanged += new EventHandler(_byteProvider_LengthChanged);
				OnByteProviderChanged(EventArgs.Empty);
				if (value == null)
				{
					_bytePos = -1L;
					_byteCharacterPos = 0;
					_selectionLength = 0L;
					DestroyCaret();
				}
				else
				{
					SetPosition(0L, 0);
					SetSelectionLength(0L);
					if (_caretVisible && Focused)
						UpdateCaret();
					else
						CreateCaret();
				}
				CheckCurrentLineChanged();
				CheckCurrentPositionInLineChanged();
				_scrollVpos = 0L;
				UpdateVisibilityBytes();
				UpdateRectanglePositioning();
				Invalidate();
			}
		}

		[DefaultValue(false)]
		[Description("Gets or sets the visibility of a line info.")]
		[Category("Hex")]
		public bool LineInfoVisible
		{
			get
			{
				return _lineInfoVisible;
			}
			set
			{
				if (_lineInfoVisible == value)
					return;
				_lineInfoVisible = value;
				OnLineInfoVisibleChanged(EventArgs.Empty);
				UpdateRectanglePositioning();
				Invalidate();
			}
		}

		[Description("Gets or sets the offset of the line info.")]
		[DefaultValue(0L)]
		[Category("Hex")]
		public long LineInfoOffset
		{
			get
			{
				return _lineInfoOffset;
			}
			set
			{
				if (_lineInfoOffset == value)
					return;
				_lineInfoOffset = value;
				Invalidate();
			}
		}

		[DefaultValue(typeof(BorderStyle), "Fixed3D")]
		[Description("Gets or sets the hex box´s border style.")]
		[Category("Hex")]
		public BorderStyle BorderStyle
		{
			get
			{
				return _borderStyle;
			}
			set
			{
				if (_borderStyle == value)
					return;
				_borderStyle = value;
				switch (_borderStyle)
				{
					case BorderStyle.None:
						_recBorderLeft = _recBorderTop = _recBorderRight = _recBorderBottom = 0;
						break;
					case BorderStyle.FixedSingle:
						_recBorderLeft = _recBorderTop = _recBorderRight = _recBorderBottom = 1;
						break;
					case BorderStyle.Fixed3D:
						_recBorderLeft = _recBorderRight = SystemInformation.Border3DSize.Width;
						_recBorderTop = _recBorderBottom = SystemInformation.Border3DSize.Height;
						break;
				}
				UpdateRectanglePositioning();
				OnBorderStyleChanged(EventArgs.Empty);
			}
		}

		[Description("Gets or sets the visibility of the string view.")]
		[DefaultValue(false)]
		[Category("Hex")]
		public bool StringViewVisible
		{
			get
			{
				return _stringViewVisible;
			}
			set
			{
				if (_stringViewVisible == value)
					return;
				_stringViewVisible = value;
				OnStringViewVisibleChanged(EventArgs.Empty);
				UpdateRectanglePositioning();
				Invalidate();
			}
		}

		[Category("Hex")]
		[DefaultValue(typeof(HexCasing), "Upper")]
		[Description("Gets or sets whether the HexBox control displays the hex characters in upper or lower case.")]
		public HexCasing HexCasing
		{
			get
			{
				return _hexStringFormat == "X" ? HexCasing.Upper : HexCasing.Lower;
			}
			set
			{
				string str = value != HexCasing.Upper ? "x" : "X";
				if (_hexStringFormat == str)
					return;
				_hexStringFormat = str;
				OnHexCasingChanged(EventArgs.Empty);
				Invalidate();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public long SelectionStart
		{
			get
			{
				return _bytePos;
			}
			set
			{
				SetPosition(value, 0);
				ScrollByteIntoView();
				Invalidate();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public long SelectionLength
		{
			get
			{
				return _selectionLength;
			}
			set
			{
				SetSelectionLength(value);
				ScrollByteIntoView();
				Invalidate();
			}
		}

		[Category("Hex")]
		[DefaultValue(typeof(Color), "Empty")]
		[Description("Gets or sets the line info color. When this property is null, then ForeColor property is used.")]
		public Color LineInfoForeColor
		{
			get
			{
				return _lineInfoForeColor;
			}
			set
			{
				_lineInfoForeColor = value;
				Invalidate();
			}
		}

		[Description("Gets or sets the background color for the selected bytes.")]
		[DefaultValue(typeof(Color), "Blue")]
		[Category("Hex")]
		public Color SelectionBackColor
		{
			get
			{
				return _selectionBackColor;
			}
			set
			{
				_selectionBackColor = value;
				Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "White")]
		[Description("Gets or sets the foreground color for the selected bytes.")]
		[Category("Hex")]
		public Color SelectionForeColor
		{
			get
			{
				return _selectionForeColor;
			}
			set
			{
				_selectionForeColor = value;
				Invalidate();
			}
		}

		[Category("Hex")]
		[DefaultValue(true)]
		[Description("Gets or sets the visibility of a shadow selection.")]
		public bool ShadowSelectionVisible
		{
			get
			{
				return _shadowSelectionVisible;
			}
			set
			{
				if (_shadowSelectionVisible == value)
					return;
				_shadowSelectionVisible = value;
				Invalidate();
			}
		}

		[Description("Gets or sets the color of the shadow selection.")]
		[Category("Hex")]
		public Color ShadowSelectionColor
		{
			get
			{
				return _shadowSelectionColor;
			}
			set
			{
				_shadowSelectionColor = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int HorizontalByteCount
		{
			get
			{
				return _iHexMaxHBytes;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public int VerticalByteCount
		{
			get
			{
				return _iHexMaxVBytes;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public long CurrentLine
		{
			get
			{
				return _currentLine;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public long CurrentPositionInLine
		{
			get
			{
				return (long)_currentPositionInLine;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public bool InsertActive
		{
			get
			{
				return _insertActive;
			}
			set
			{
				if (_insertActive == value)
					return;
				_insertActive = value;
				DestroyCaret();
				CreateCaret();
				OnInsertActiveChanged(EventArgs.Empty);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BuiltInContextMenu BuiltInContextMenu
		{
			get
			{
				return _builtInContextMenu;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IByteCharConverter ByteCharConverter
		{
			get
			{
				if (_byteCharConverter == null)
					_byteCharConverter = (IByteCharConverter)new DefaultByteCharConverter();
				return _byteCharConverter;
			}
			set
			{
				if (value == null || value == _byteCharConverter)
					return;
				_byteCharConverter = value;
				Invalidate();
			}
		}

		[Description("Occurs, when the value of InsertActive property has changed.")]
		public event EventHandler InsertActiveChanged;

		[Description("Occurs, when the value of ReadOnly property has changed.")]
		public event EventHandler ReadOnlyChanged;

		[Description("Occurs, when the value of ByteProvider property has changed.")]
		public event EventHandler ByteProviderChanged;

		[Description("Occurs, when the value of SelectionStart property has changed.")]
		public event EventHandler SelectionStartChanged;

		[Description("Occurs, when the value of SelectionLength property has changed.")]
		public event EventHandler SelectionLengthChanged;

		[Description("Occurs, when the value of LineInfoVisible property has changed.")]
		public event EventHandler LineInfoVisibleChanged;

		[Description("Occurs, when the value of StringViewVisible property has changed.")]
		public event EventHandler StringViewVisibleChanged;

		[Description("Occurs, when the value of BorderStyle property has changed.")]
		public event EventHandler BorderStyleChanged;

		[Description("Occurs, when the value of BytesPerLine property has changed.")]
		public event EventHandler BytesPerLineChanged;

		[Description("Occurs, when the value of UseFixedBytesPerLine property has changed.")]
		public event EventHandler UseFixedBytesPerLineChanged;

		[Description("Occurs, when the value of VScrollBarVisible property has changed.")]
		public event EventHandler VScrollBarVisibleChanged;

		[Description("Occurs, when the value of HexCasing property has changed.")]
		public event EventHandler HexCasingChanged;

		[Description("Occurs, when the value of HorizontalByteCount property has changed.")]
		public event EventHandler HorizontalByteCountChanged;

		[Description("Occurs, when the value of VerticalByteCount property has changed.")]
		public event EventHandler VerticalByteCountChanged;

		[Description("Occurs, when the value of CurrentLine property has changed.")]
		public event EventHandler CurrentLineChanged;

		[Description("Occurs, when the value of CurrentPositionInLine property has changed.")]
		public event EventHandler CurrentPositionInLineChanged;

		[Description("Occurs, when Copy method was invoked and ClipBoardData changed.")]
		public event EventHandler Copied;

		[Description("Occurs, when CopyHex method was invoked and ClipBoardData changed.")]
		public event EventHandler CopiedHex;

		public HexBox()
		{
			_vScrollBar = new VScrollBar();
			_vScrollBar.Scroll += new ScrollEventHandler(_vScrollBar_Scroll);
			_builtInContextMenu = new BuiltInContextMenu(this);
			BackColor = Color.White;
			Font = new Font("Courier New", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
			_stringFormat = new StringFormat(StringFormat.GenericTypographic);
			_stringFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
			ActivateEmptyKeyInterpreter();
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			_thumbTrackTimer.Interval = 50;
			_thumbTrackTimer.Tick += new EventHandler(PerformScrollThumbTrack);
		}

		private void _vScrollBar_Scroll(object sender, ScrollEventArgs e)
		{
			switch (e.Type)
			{
				case ScrollEventType.SmallDecrement:
					PerformScrollLineUp();
					break;
				case ScrollEventType.SmallIncrement:
					PerformScrollLineDown();
					break;
				case ScrollEventType.LargeDecrement:
					PerformScrollPageUp();
					break;
				case ScrollEventType.LargeIncrement:
					PerformScrollPageDown();
					break;
				case ScrollEventType.ThumbPosition:
					PerformScrollThumpPosition(FromScrollPos(e.NewValue));
					break;
				case ScrollEventType.ThumbTrack:
					if (_thumbTrackTimer.Enabled)
						_thumbTrackTimer.Enabled = false;
					int tickCount = Environment.TickCount;
					if (tickCount - _lastThumbtrack > 50)
					{
						PerformScrollThumbTrack((object)null, (EventArgs)null);
						_lastThumbtrack = tickCount;
						break;
					}
					_thumbTrackPosition = FromScrollPos(e.NewValue);
					_thumbTrackTimer.Enabled = true;
					break;
			}
			e.NewValue = ToScrollPos(_scrollVpos);
		}

		private void PerformScrollThumbTrack(object sender, EventArgs e)
		{
			_thumbTrackTimer.Enabled = false;
			PerformScrollThumpPosition(_thumbTrackPosition);
			_lastThumbtrack = Environment.TickCount;
		}

		public void UpdateScrollSize()
		{
			if (VScrollBarVisible && _byteProvider != null && (_byteProvider.Length > 0L && _iHexMaxHBytes != 0))
			{
				long val2 = Math.Max(0L, (long)Math.Ceiling((double)(_byteProvider.Length + 1L) / (double)_iHexMaxHBytes - (double)_iHexMaxVBytes));
				long val1 = _startByte / (long)_iHexMaxHBytes;
				if (val2 < _scrollVmax && _scrollVpos == _scrollVmax)
					PerformScrollLineUp();
				if (val2 == _scrollVmax && val1 == _scrollVpos)
					return;
				_scrollVmin = 0L;
				_scrollVmax = val2;
				_scrollVpos = Math.Min(val1, val2);
				UpdateVScroll();
			}
			else
			{
				if (!VScrollBarVisible)
					return;
				_scrollVmin = 0L;
				_scrollVmax = 0L;
				_scrollVpos = 0L;
				UpdateVScroll();
			}
		}

		private void UpdateVScroll()
		{
			int num = ToScrollMax(_scrollVmax);
			if (num > 0)
			{
				_vScrollBar.Minimum = 0;
				_vScrollBar.Maximum = num;
				_vScrollBar.Value = ToScrollPos(_scrollVpos);
				_vScrollBar.Enabled = true;
			}
			else
				_vScrollBar.Enabled = false;
		}

		private int ToScrollPos(long value)
		{
			int num1 = (int)ushort.MaxValue;
			if (_scrollVmax < (long)num1)
				return (int)value;
			double num2 = (double)value / (double)_scrollVmax * 100.0;
			return (int)Math.Min(_scrollVmax, (long)(int)Math.Max(_scrollVmin, (long)(int)Math.Floor((double)num1 / 100.0 * num2)));
		}

		private long FromScrollPos(int value)
		{
			int num = (int)ushort.MaxValue;
			if (_scrollVmax < (long)num)
				return (long)value;
			return (long)(int)Math.Floor((double)_scrollVmax / 100.0 * ((double)value / (double)num * 100.0));
		}

		private int ToScrollMax(long value)
		{
			long num = (long)ushort.MaxValue;
			if (value > num)
				return (int)num;
			return (int)value;
		}

		private void PerformScrollToLine(long pos)
		{
			if (pos < _scrollVmin || pos > _scrollVmax || pos == _scrollVpos)
				return;
			_scrollVpos = pos;
			UpdateVScroll();
			UpdateVisibilityBytes();
			UpdateCaret();
			Invalidate();
		}

		private void PerformScrollLines(int lines)
		{
			long pos;
			if (lines > 0)
			{
				pos = Math.Min(_scrollVmax, _scrollVpos + (long)lines);
			}
			else
			{
				if (lines >= 0)
					return;
				pos = Math.Max(_scrollVmin, _scrollVpos + (long)lines);
			}
			PerformScrollToLine(pos);
		}

		private void PerformScrollLineDown()
		{
			PerformScrollLines(1);
		}

		private void PerformScrollLineUp()
		{
			PerformScrollLines(-1);
		}

		private void PerformScrollPageDown()
		{
			PerformScrollLines(_iHexMaxVBytes);
		}

		private void PerformScrollPageUp()
		{
			PerformScrollLines(-_iHexMaxVBytes);
		}

		private void PerformScrollThumpPosition(long pos)
		{
			int num = _scrollVmax > (long)ushort.MaxValue ? 10 : 9;
			if (ToScrollPos(pos) == ToScrollMax(_scrollVmax) - num)
				pos = _scrollVmax;
			PerformScrollToLine(pos);
		}

		public void ScrollByteIntoView()
		{
			ScrollByteIntoView(_bytePos);
		}

		public void ScrollByteIntoView(long index)
		{
			if (_byteProvider == null || _keyInterpreter == null)
				return;
			if (index < _startByte)
			{
				PerformScrollThumpPosition((long)Math.Floor((double)index / (double)_iHexMaxHBytes));
			}
			else
			{
				if (index <= _endByte)
					return;
				PerformScrollThumpPosition((long)Math.Floor((double)index / (double)_iHexMaxHBytes) - (long)(_iHexMaxVBytes - 1));
			}
		}

		private void ReleaseSelection()
		{
			if (_selectionLength == 0L)
				return;
			_selectionLength = 0L;
			OnSelectionLengthChanged(EventArgs.Empty);
			if (!_caretVisible)
				CreateCaret();
			else
				UpdateCaret();
			Invalidate();
		}

		public bool CanSelectAll()
		{
			return Enabled && _byteProvider != null;
		}

		public void SelectAll()
		{
			if (ByteProvider == null)
				return;
			Select(0L, ByteProvider.Length);
		}

		public void Select(long start, long length)
		{
			if (ByteProvider == null || !Enabled)
				return;
			InternalSelect(start, length);
			ScrollByteIntoView();
		}

		private void InternalSelect(long start, long length)
		{
			long bytePos = start;
			long selectionLength = length;
			int byteCharacterPos = 0;
			if (selectionLength > 0L && _caretVisible)
				DestroyCaret();
			else if (selectionLength == 0L && !_caretVisible)
				CreateCaret();
			SetPosition(bytePos, byteCharacterPos);
			SetSelectionLength(selectionLength);
			UpdateCaret();
			Invalidate();
		}

		private void ActivateEmptyKeyInterpreter()
		{
			if (_eki == null)
				_eki = new HexBox.EmptyKeyInterpreter(this);
			if (_eki == _keyInterpreter)
				return;
			if (_keyInterpreter != null)
				_keyInterpreter.Deactivate();
			_keyInterpreter = (HexBox.IKeyInterpreter)_eki;
			_keyInterpreter.Activate();
		}

		private void ActivateKeyInterpreter()
		{
			if (_ki == null)
				_ki = new HexBox.KeyInterpreter(this);
			if (_ki == _keyInterpreter)
				return;
			if (_keyInterpreter != null)
				_keyInterpreter.Deactivate();
			_keyInterpreter = (HexBox.IKeyInterpreter)_ki;
			_keyInterpreter.Activate();
		}

		private void ActivateStringKeyInterpreter()
		{
			if (_ski == null)
				_ski = new HexBox.StringKeyInterpreter(this);
			if (_ski == _keyInterpreter)
				return;
			if (_keyInterpreter != null)
				_keyInterpreter.Deactivate();
			_keyInterpreter = (HexBox.IKeyInterpreter)_ski;
			_keyInterpreter.Activate();
		}

		private void CreateCaret()
		{
			if (_byteProvider == null || _keyInterpreter == null || (_caretVisible || !Focused))
				return;
			int nWidth = InsertActive ? 1 : (int)_charSize.Width;
			int nHeight = (int)_charSize.Height;
			NativeMethods.CreateCaret(Handle, IntPtr.Zero, nWidth, nHeight);
			UpdateCaret();
			NativeMethods.ShowCaret(Handle);
			_caretVisible = true;
		}

		private void UpdateCaret()
		{
			if (_byteProvider == null || _keyInterpreter == null)
				return;
			PointF caretPointF = _keyInterpreter.GetCaretPointF(_bytePos - _startByte);
			caretPointF.X += (float)_byteCharacterPos * _charSize.Width;
			NativeMethods.SetCaretPos((int)caretPointF.X, (int)caretPointF.Y);
		}

		private void DestroyCaret()
		{
			if (!_caretVisible)
				return;
			NativeMethods.DestroyCaret();
			_caretVisible = false;
		}

		private void SetCaretPosition(Point p)
		{
			if (_byteProvider == null || _keyInterpreter == null)
				return;
			long num1 = _bytePos;
			int num2 = _byteCharacterPos;
			if (_recHex.Contains(p))
			{
				BytePositionInfo bytePositionInfo = GetHexBytePositionInfo(p);
				SetPosition(bytePositionInfo.Index, bytePositionInfo.CharacterPosition);
				ActivateKeyInterpreter();
				UpdateCaret();
				Invalidate();
			}
			else
			{
				if (!_recStringView.Contains(p))
					return;
				BytePositionInfo bytePositionInfo = GetStringBytePositionInfo(p);
				SetPosition(bytePositionInfo.Index, bytePositionInfo.CharacterPosition);
				ActivateStringKeyInterpreter();
				UpdateCaret();
				Invalidate();
			}
		}

		private BytePositionInfo GetHexBytePositionInfo(Point p)
		{
			float num1 = (float)(p.X - _recHex.X) / _charSize.Width;
			float num2 = (float)(p.Y - _recHex.Y) / _charSize.Height;
			int num3 = (int)num1;
			long index = Math.Min(_byteProvider.Length, _startByte + (long)(_iHexMaxHBytes * ((int)num2 + 1) - _iHexMaxHBytes) + (long)(num3 / 3 + 1) - 1L);
			int characterPosition = num3 % 3;
			if (characterPosition > 1)
				characterPosition = 1;
			if (index == _byteProvider.Length)
				characterPosition = 0;
			if (index < 0L)
				return new BytePositionInfo(0L, 0);
			return new BytePositionInfo(index, characterPosition);
		}

		private BytePositionInfo GetStringBytePositionInfo(Point p)
		{
			float num = (float)(p.X - _recStringView.X) / _charSize.Width;
			long index = Math.Min(_byteProvider.Length, _startByte + (long)(_iHexMaxHBytes * ((int)((float)(p.Y - _recStringView.Y) / _charSize.Height) + 1) - _iHexMaxHBytes) + (long)((int)num + 1) - 1L);
			int characterPosition = 0;
			if (index < 0L)
				return new BytePositionInfo(0L, 0);
			return new BytePositionInfo(index, characterPosition);
		}

		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
		[SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
		public override bool PreProcessMessage(ref Message m)
		{
			switch (m.Msg)
			{
				case 256:
					return _keyInterpreter.PreProcessWmKeyDown(ref m);
				case 257:
					return _keyInterpreter.PreProcessWmKeyUp(ref m);
				case 258:
					return _keyInterpreter.PreProcessWmChar(ref m);
				default:
					return base.PreProcessMessage(ref m);
			}
		}

		private bool BasePreProcessMessage(ref Message m)
		{
			return base.PreProcessMessage(ref m);
		}

		public long Find(byte[] bytes, long startIndex)
		{
			int index1 = 0;
			int length = bytes.Length;
			_abortFind = false;
			for (long index2 = startIndex; index2 < _byteProvider.Length; ++index2)
			{
				if (_abortFind)
					return -2L;
				if (index2 % 1000L == 0L)
					Application.DoEvents();
				if ((int)_byteProvider.ReadByte(index2) != (int)bytes[index1])
				{
					index2 -= (long)index1;
					index1 = 0;
					_findingPos = index2;
				}
				else
				{
					++index1;
					if (index1 == length)
					{
						long start = index2 - (long)length + 1L;
						Select(start, (long)length);
						ScrollByteIntoView(_bytePos + _selectionLength);
						ScrollByteIntoView(_bytePos);
						return start;
					}
				}
			}
			return -1L;
		}

		public void AbortFind()
		{
			_abortFind = true;
		}

		private byte[] GetCopyData()
		{
			if (!CanCopy())
				return new byte[0];
			byte[] numArray = new byte[_selectionLength];
			int index1 = -1;
			for (long index2 = _bytePos; index2 < _bytePos + _selectionLength; ++index2)
			{
				++index1;
				numArray[index1] = _byteProvider.ReadByte(index2);
			}
			return numArray;
		}

		public void Copy()
		{
			if (!CanCopy())
				return;
			byte[] copyData = GetCopyData();
			DataObject dataObject = new DataObject();
			string @string = Encoding.ASCII.GetString(copyData, 0, copyData.Length);
			dataObject.SetData(typeof(string), (object)@string);
			MemoryStream memoryStream = new MemoryStream(copyData, 0, copyData.Length, false, true);
			dataObject.SetData("BinaryData", (object)memoryStream);
			Clipboard.SetDataObject((object)dataObject, true);
			UpdateCaret();
			ScrollByteIntoView();
			Invalidate();
			OnCopied(EventArgs.Empty);
		}

		public bool CanCopy()
		{
			return _selectionLength >= 1L && _byteProvider != null;
		}

		public void Cut()
		{
			if (!CanCut())
				return;
			Copy();
			_byteProvider.DeleteBytes(_bytePos, _selectionLength);
			_byteCharacterPos = 0;
			UpdateCaret();
			ScrollByteIntoView();
			ReleaseSelection();
			Invalidate();
			Refresh();
		}

		public bool CanCut()
		{
			return !ReadOnly && Enabled && (_byteProvider != null && _selectionLength >= 1L) && _byteProvider.SupportsDeleteBytes();
		}

		public void Paste()
		{
			if (!CanPaste())
				return;
			if (_selectionLength > 0L)
				_byteProvider.DeleteBytes(_bytePos, _selectionLength);
			IDataObject dataObject = Clipboard.GetDataObject();
			byte[] numArray;
			if (dataObject.GetDataPresent("BinaryData"))
			{
				MemoryStream memoryStream = (MemoryStream)dataObject.GetData("BinaryData");
				numArray = new byte[memoryStream.Length];
				memoryStream.Read(numArray, 0, numArray.Length);
			}
			else
			{
				if (!dataObject.GetDataPresent(typeof(string)))
					return;
				numArray = Encoding.ASCII.GetBytes((string)dataObject.GetData(typeof(string)));
			}
			_byteProvider.InsertBytes(_bytePos, numArray);
			SetPosition(_bytePos + (long)numArray.Length, 0);
			ReleaseSelection();
			ScrollByteIntoView();
			UpdateCaret();
			Invalidate();
		}

		public bool CanPaste()
		{
			if (ReadOnly || !Enabled || (_byteProvider == null || !_byteProvider.SupportsInsertBytes()) || !_byteProvider.SupportsDeleteBytes() && _selectionLength > 0L)
				return false;
			IDataObject dataObject = Clipboard.GetDataObject();
			return dataObject.GetDataPresent("BinaryData") || dataObject.GetDataPresent(typeof(string));
		}

		public bool CanPasteHex()
		{
			if (!CanPaste())
				return false;
			IDataObject dataObject = Clipboard.GetDataObject();
			if (dataObject.GetDataPresent(typeof(string)))
				return ConvertHexToBytes((string)dataObject.GetData(typeof(string))) != null;
			return false;
		}

		public void PasteHex()
		{
			if (!CanPaste())
				return;
			IDataObject dataObject = Clipboard.GetDataObject();
			if (!dataObject.GetDataPresent(typeof(string)))
				return;
			byte[] bs = ConvertHexToBytes((string)dataObject.GetData(typeof(string)));
			if (bs == null)
				return;
			if (_selectionLength > 0L)
				_byteProvider.DeleteBytes(_bytePos, _selectionLength);
			_byteProvider.InsertBytes(_bytePos, bs);
			SetPosition(_bytePos + (long)bs.Length, 0);
			ReleaseSelection();
			ScrollByteIntoView();
			UpdateCaret();
			Invalidate();
		}

		public void CopyHex()
		{
			if (!CanCopy())
				return;
			byte[] copyData = GetCopyData();
			DataObject dataObject = new DataObject();
			string str = ConvertBytesToHex(copyData);
			dataObject.SetData(typeof(string), (object)str);
			MemoryStream memoryStream = new MemoryStream(copyData, 0, copyData.Length, false, true);
			dataObject.SetData("BinaryData", (object)memoryStream);
			Clipboard.SetDataObject((object)dataObject, true);
			UpdateCaret();
			ScrollByteIntoView();
			Invalidate();
			OnCopiedHex(EventArgs.Empty);
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			switch (_borderStyle)
			{
				case BorderStyle.FixedSingle:
					e.Graphics.FillRectangle((Brush)new SolidBrush(BackColor), ClientRectangle);
					ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
					break;
				case BorderStyle.Fixed3D:
					if (TextBoxRenderer.IsSupported)
					{
						VisualStyleElement element = VisualStyleElement.TextBox.TextEdit.Normal;
						Color color = BackColor;
						if (Enabled)
						{
							if (ReadOnly)
								element = VisualStyleElement.TextBox.TextEdit.ReadOnly;
							else if (Focused)
								element = VisualStyleElement.TextBox.TextEdit.Focused;
						}
						else
						{
							element = VisualStyleElement.TextBox.TextEdit.Disabled;
							color = BackColorDisabled;
						}
						VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(element);
						visualStyleRenderer.DrawBackground((IDeviceContext)e.Graphics, ClientRectangle);
						Rectangle contentRectangle = visualStyleRenderer.GetBackgroundContentRectangle((IDeviceContext)e.Graphics, ClientRectangle);
						e.Graphics.FillRectangle((Brush)new SolidBrush(color), contentRectangle);
						break;
					}
					e.Graphics.FillRectangle((Brush)new SolidBrush(BackColor), ClientRectangle);
					ControlPaint.DrawBorder3D(e.Graphics, ClientRectangle, Border3DStyle.Sunken);
					break;
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (_byteProvider == null)
				return;
			Region region = new Region(ClientRectangle);
			region.Exclude(_recContent);
			e.Graphics.ExcludeClip(region);
			UpdateVisibilityBytes();
			if (_lineInfoVisible)
				PaintLineInfo(e.Graphics, _startByte, _endByte);
			if (!_stringViewVisible)
			{
				PaintHex(e.Graphics, _startByte, _endByte);
			}
			else
			{
				PaintHexAndStringView(e.Graphics, _startByte, _endByte);
				if (!_shadowSelectionVisible)
					return;
				PaintCurrentBytesSign(e.Graphics);
			}
		}

		private void PaintLineInfo(Graphics g, long startByte, long endByte)
		{
			endByte = Math.Min(_byteProvider.Length - 1L, endByte);
			Brush brush = (Brush)new SolidBrush(LineInfoForeColor != Color.Empty ? LineInfoForeColor : ForeColor);
			int num1 = GetGridBytePoint(endByte - startByte).Y + 1;
			for (int y = 0; y < num1; ++y)
			{
				long num2 = startByte + (long)(_iHexMaxHBytes * y) + _lineInfoOffset;
				PointF bytePointF = GetBytePointF(new Point(0, y));
				string str = num2.ToString(_hexStringFormat, (IFormatProvider)Thread.CurrentThread.CurrentCulture);
				string s = 8 - str.Length <= -1 ? new string('~', (int)lineInfoDigits) : new string('0', (int)lineInfoDigits - str.Length) + str;
				g.DrawString(s, Font, brush, new PointF((float)_recLineInfo.X, bytePointF.Y), _stringFormat);
			}
		}

		private void PaintHex(Graphics g, long startByte, long endByte)
		{
			Brush brush1 = (Brush)new SolidBrush(GetDefaultForeColor());
			Brush brush2 = (Brush)new SolidBrush(_selectionForeColor);
			Brush brushBack = (Brush)new SolidBrush(_selectionBackColor);
			int num1 = -1;
			long num2 = Math.Min(_byteProvider.Length - 1L, endByte + (long)_iHexMaxHBytes);
			bool flag = _keyInterpreter == null || _keyInterpreter.GetType() == typeof(HexBox.KeyInterpreter);
			for (long index = startByte; index < num2 + 1L; ++index)
			{
				++num1;
				Point gridBytePoint = GetGridBytePoint((long)num1);
				byte b = _byteProvider.ReadByte(index);
				if (index >= _bytePos && index <= _bytePos + _selectionLength - 1L && _selectionLength != 0L && flag)
					PaintHexStringSelected(g, b, brush2, brushBack, gridBytePoint);
				else
					PaintHexString(g, b, brush1, gridBytePoint);
			}
		}

		private void PaintHexString(Graphics g, byte b, Brush brush, Point gridPoint)
		{
			PointF bytePointF = GetBytePointF(gridPoint);
			string str = ConvertByteToHex(b);
			g.DrawString(str.Substring(0, 1), Font, brush, bytePointF, _stringFormat);
			bytePointF.X += _charSize.Width;
			g.DrawString(str.Substring(1, 1), Font, brush, bytePointF, _stringFormat);
		}

		private void PaintHexStringSelected(Graphics g, byte b, Brush brush, Brush brushBack, Point gridPoint)
		{
			string str = b.ToString(_hexStringFormat, (IFormatProvider)Thread.CurrentThread.CurrentCulture);
			if (str.Length == 1)
				str = "0" + str;
			PointF bytePointF = GetBytePointF(gridPoint);
			float width = gridPoint.X + 1 == _iHexMaxHBytes ? _charSize.Width * 2f : _charSize.Width * 3f;
			g.FillRectangle(brushBack, bytePointF.X, bytePointF.Y, width, _charSize.Height);
			g.DrawString(str.Substring(0, 1), Font, brush, bytePointF, _stringFormat);
			bytePointF.X += _charSize.Width;
			g.DrawString(str.Substring(1, 1), Font, brush, bytePointF, _stringFormat);
		}

		private void PaintHexAndStringView(Graphics g, long startByte, long endByte)
		{
			Brush brush1 = (Brush)new SolidBrush(GetDefaultForeColor());
			Brush brush2 = (Brush)new SolidBrush(_selectionForeColor);
			Brush brush3 = (Brush)new SolidBrush(_selectionBackColor);
			int num1 = -1;
			long num2 = Math.Min(_byteProvider.Length - 1L, endByte + (long)_iHexMaxHBytes);
			bool flag1 = _keyInterpreter == null || _keyInterpreter.GetType() == typeof(HexBox.KeyInterpreter);
			bool flag2 = _keyInterpreter != null && _keyInterpreter.GetType() == typeof(HexBox.StringKeyInterpreter);
			for (long index = startByte; index < num2 + 1L; ++index)
			{
				++num1;
				Point gridBytePoint = GetGridBytePoint((long)num1);
				PointF byteStringPointF = GetByteStringPointF(gridBytePoint);
				byte b = _byteProvider.ReadByte(index);
				bool flag3 = index >= _bytePos && index <= _bytePos + _selectionLength - 1L && _selectionLength != 0L;
				if (flag3 && flag1)
					PaintHexStringSelected(g, b, brush2, brush3, gridBytePoint);
				else
					PaintHexString(g, b, brush1, gridBytePoint);
				string s = new string(ByteCharConverter.ToChar(b), 1);
				if (flag3 && flag2)
				{
					g.FillRectangle(brush3, byteStringPointF.X, byteStringPointF.Y, _charSize.Width, _charSize.Height);
					g.DrawString(s, Font, brush2, byteStringPointF, _stringFormat);
				}
				else
					g.DrawString(s, Font, brush1, byteStringPointF, _stringFormat);
			}
		}

		private void PaintCurrentBytesSign(Graphics g)
		{
			if (_keyInterpreter == null || !Focused || (_bytePos == -1L || !Enabled))
				return;
			if (_keyInterpreter.GetType() == typeof(HexBox.KeyInterpreter))
			{
				if (_selectionLength == 0L)
				{
					PointF byteStringPointF = GetByteStringPointF(GetGridBytePoint(_bytePos - _startByte));
					Size size = new Size((int)_charSize.Width, (int)_charSize.Height);
					Rectangle rec = new Rectangle((int)byteStringPointF.X, (int)byteStringPointF.Y, size.Width, size.Height);
					if (!rec.IntersectsWith(_recStringView))
						return;
					rec.Intersect(_recStringView);
					PaintCurrentByteSign(g, rec);
				}
				else
				{
					int num1 = (int)((double)_recStringView.Width - (double)_charSize.Width);
					Point gridBytePoint1 = GetGridBytePoint(_bytePos - _startByte);
					PointF byteStringPointF1 = GetByteStringPointF(gridBytePoint1);
					Point gridBytePoint2 = GetGridBytePoint(_bytePos - _startByte + _selectionLength - 1L);
					PointF byteStringPointF2 = GetByteStringPointF(gridBytePoint2);
					int num2 = gridBytePoint2.Y - gridBytePoint1.Y;
					if (num2 == 0)
					{
						Rectangle rec = new Rectangle((int)byteStringPointF1.X, (int)byteStringPointF1.Y, (int)((double)byteStringPointF2.X - (double)byteStringPointF1.X + (double)_charSize.Width), (int)_charSize.Height);
						if (!rec.IntersectsWith(_recStringView))
							return;
						rec.Intersect(_recStringView);
						PaintCurrentByteSign(g, rec);
					}
					else
					{
						Rectangle rec1 = new Rectangle((int)byteStringPointF1.X, (int)byteStringPointF1.Y, (int)((double)(_recStringView.X + num1) - (double)byteStringPointF1.X + (double)_charSize.Width), (int)_charSize.Height);
						if (rec1.IntersectsWith(_recStringView))
						{
							rec1.Intersect(_recStringView);
							PaintCurrentByteSign(g, rec1);
						}
						if (num2 > 1)
						{
							Rectangle rec2 = new Rectangle(_recStringView.X, (int)((double)byteStringPointF1.Y + (double)_charSize.Height), _recStringView.Width, (int)((double)_charSize.Height * (double)(num2 - 1)));
							if (rec2.IntersectsWith(_recStringView))
							{
								rec2.Intersect(_recStringView);
								PaintCurrentByteSign(g, rec2);
							}
						}
						Rectangle rec3 = new Rectangle(_recStringView.X, (int)byteStringPointF2.Y, (int)((double)byteStringPointF2.X - (double)_recStringView.X + (double)_charSize.Width), (int)_charSize.Height);
						if (!rec3.IntersectsWith(_recStringView))
							return;
						rec3.Intersect(_recStringView);
						PaintCurrentByteSign(g, rec3);
					}
				}
			}
			else if (_selectionLength == 0L)
			{
				PointF bytePointF = GetBytePointF(GetGridBytePoint(_bytePos - _startByte));
				Size size = new Size((int)_charSize.Width * 2, (int)_charSize.Height);
				Rectangle rec = new Rectangle((int)bytePointF.X, (int)bytePointF.Y, size.Width, size.Height);
				PaintCurrentByteSign(g, rec);
			}
			else
			{
				int num1 = (int)((double)_recHex.Width - (double)_charSize.Width * 5.0);
				Point gridBytePoint1 = GetGridBytePoint(_bytePos - _startByte);
				PointF bytePointF1 = GetBytePointF(gridBytePoint1);
				Point gridBytePoint2 = GetGridBytePoint(_bytePos - _startByte + _selectionLength - 1L);
				PointF bytePointF2 = GetBytePointF(gridBytePoint2);
				int num2 = gridBytePoint2.Y - gridBytePoint1.Y;
				if (num2 == 0)
				{
					Rectangle rec = new Rectangle((int)bytePointF1.X, (int)bytePointF1.Y, (int)((double)bytePointF2.X - (double)bytePointF1.X + (double)_charSize.Width * 2.0), (int)_charSize.Height);
					if (!rec.IntersectsWith(_recHex))
						return;
					rec.Intersect(_recHex);
					PaintCurrentByteSign(g, rec);
				}
				else
				{
					Rectangle rec1 = new Rectangle((int)bytePointF1.X, (int)bytePointF1.Y, (int)((double)(_recHex.X + num1) - (double)bytePointF1.X + (double)_charSize.Width * 2.0), (int)_charSize.Height);
					if (rec1.IntersectsWith(_recHex))
					{
						rec1.Intersect(_recHex);
						PaintCurrentByteSign(g, rec1);
					}
					if (num2 > 1)
					{
						Rectangle rec2 = new Rectangle(_recHex.X, (int)((double)bytePointF1.Y + (double)_charSize.Height), (int)((double)num1 + (double)_charSize.Width * 2.0), (int)((double)_charSize.Height * (double)(num2 - 1)));
						if (rec2.IntersectsWith(_recHex))
						{
							rec2.Intersect(_recHex);
							PaintCurrentByteSign(g, rec2);
						}
					}
					Rectangle rec3 = new Rectangle(_recHex.X, (int)bytePointF2.Y, (int)((double)bytePointF2.X - (double)_recHex.X + (double)_charSize.Width * 2.0), (int)_charSize.Height);
					if (!rec3.IntersectsWith(_recHex))
						return;
					rec3.Intersect(_recHex);
					PaintCurrentByteSign(g, rec3);
				}
			}
		}

		private void PaintCurrentByteSign(Graphics g, Rectangle rec)
		{
			if (rec.Top < 0 || rec.Left < 0 || (rec.Width <= 0 || rec.Height <= 0))
				return;
			Bitmap bitmap = new Bitmap(rec.Width, rec.Height);
			Graphics.FromImage((Image)bitmap).FillRectangle((Brush)new SolidBrush(_shadowSelectionColor), 0, 0, rec.Width, rec.Height);
			g.CompositingQuality = CompositingQuality.GammaCorrected;
			g.DrawImage((Image)bitmap, rec.Left, rec.Top);
		}

		private Color GetDefaultForeColor()
		{
			if (Enabled)
				return ForeColor;
			return Color.Gray;
		}

		private void UpdateVisibilityBytes()
		{
			if (_byteProvider == null || _byteProvider.Length == 0L)
				return;
			_startByte = (_scrollVpos + 1L) * (long)_iHexMaxHBytes - (long)_iHexMaxHBytes;
			_endByte = Math.Min(_byteProvider.Length - 1L, _startByte + (long)_iHexMaxBytes);
		}

		private void UpdateRectanglePositioning()
		{
			SizeF sizeF = CreateGraphics().MeasureString("A", Font, 100, _stringFormat);
			_charSize = new SizeF((float)Math.Ceiling((double)sizeF.Width), (float)Math.Ceiling((double)sizeF.Height));
			_recContent = ClientRectangle;
			_recContent.X += _recBorderLeft;
			_recContent.Y += _recBorderTop;
			_recContent.Width -= _recBorderRight + _recBorderLeft;
			_recContent.Height -= _recBorderBottom + _recBorderTop;
			if (_vScrollBarVisible)
			{
				_recContent.Width -= _vScrollBar.Width;
				_vScrollBar.Left = _recContent.X + _recContent.Width;
				_vScrollBar.Top = _recContent.Y;
				_vScrollBar.Height = _recContent.Height;
			}
			int num1 = 4;
			if (_lineInfoVisible)
			{
				_recLineInfo = new Rectangle(_recContent.X + num1, _recContent.Y, (int)((double)_charSize.Width * (double)((int)lineInfoDigits + 2)), _recContent.Height);
			}
			else
			{
				_recLineInfo = Rectangle.Empty;
				_recLineInfo.X = num1;
			}
			_recHex = new Rectangle(_recLineInfo.X + _recLineInfo.Width, _recLineInfo.Y, _recContent.Width - _recLineInfo.Width, _recContent.Height);
			if (UseFixedBytesPerLine)
			{
				SetHorizontalByteCount(_bytesPerLine);
				_recHex.Width = (int)Math.Floor((double)_iHexMaxHBytes * (double)_charSize.Width * 3.0 + 2.0 * (double)_charSize.Width);
			}
			else
			{
				int num2 = (int)Math.Floor((double)_recHex.Width / (double)_charSize.Width);
				if (num2 > 1)
					SetHorizontalByteCount((int)Math.Floor((double)num2 / 3.0));
				else
					SetHorizontalByteCount(num2);
			}
			_recStringView = !_stringViewVisible ? Rectangle.Empty : new Rectangle(_recHex.X + _recHex.Width, _recHex.Y, (int)((double)_charSize.Width * (double)_iHexMaxHBytes), _recHex.Height);
			SetVerticalByteCount((int)Math.Floor((double)_recHex.Height / (double)_charSize.Height));
			_iHexMaxBytes = _iHexMaxHBytes * _iHexMaxVBytes;
			UpdateScrollSize();
		}

		private PointF GetBytePointF(long byteIndex)
		{
			return GetBytePointF(GetGridBytePoint(byteIndex));
		}

		private PointF GetBytePointF(Point gp)
		{
			return new PointF(3f * _charSize.Width * (float)gp.X + (float)_recHex.X, (float)(gp.Y + 1) * _charSize.Height - _charSize.Height + (float)_recHex.Y);
		}

		private PointF GetByteStringPointF(Point gp)
		{
			return new PointF(_charSize.Width * (float)gp.X + (float)_recStringView.X, (float)(gp.Y + 1) * _charSize.Height - _charSize.Height + (float)_recStringView.Y);
		}

		private Point GetGridBytePoint(long byteIndex)
		{
			int y = (int)Math.Floor((double)byteIndex / (double)_iHexMaxHBytes);
			return new Point((int)(byteIndex + (long)_iHexMaxHBytes - (long)(_iHexMaxHBytes * (y + 1))), y);
		}

		private string ConvertBytesToHex(byte[] data)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in data)
			{
				string str = ConvertByteToHex(b);
				stringBuilder.Append(str);
				stringBuilder.Append(" ");
			}
			if (stringBuilder.Length > 0)
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			return stringBuilder.ToString();
		}

		private string ConvertByteToHex(byte b)
		{
			string str = b.ToString(_hexStringFormat, (IFormatProvider)Thread.CurrentThread.CurrentCulture);
			if (str.Length == 1)
				str = "0" + str;
			return str;
		}

		private byte[] ConvertHexToBytes(string hex)
		{
			if (string.IsNullOrEmpty(hex))
				return (byte[])null;
			hex = hex.Trim();
			string[] strArray = hex.Split(' ');
			byte[] numArray = new byte[strArray.Length];
			for (int index = 0; index < strArray.Length; ++index)
			{
				byte b;
				if (!ConvertHexToByte(strArray[index], out b))
					return (byte[])null;
				numArray[index] = b;
			}
			return numArray;
		}

		private bool ConvertHexToByte(string hex, out byte b)
		{
			return byte.TryParse(hex, NumberStyles.HexNumber, (IFormatProvider)Thread.CurrentThread.CurrentCulture, out b);
		}

		private void SetPosition(long bytePos)
		{
			SetPosition(bytePos, _byteCharacterPos);
		}

		public void SetPosition(long bytePos, int byteCharacterPos)
		{
			if (_byteCharacterPos != byteCharacterPos)
				_byteCharacterPos = byteCharacterPos;
			if (bytePos == _bytePos)
				return;
			_bytePos = bytePos;
			CheckCurrentLineChanged();
			CheckCurrentPositionInLineChanged();
			OnSelectionStartChanged(EventArgs.Empty);
		}

		private void SetSelectionLength(long selectionLength)
		{
			if (selectionLength == _selectionLength)
				return;
			_selectionLength = selectionLength;
			OnSelectionLengthChanged(EventArgs.Empty);
		}

		private void SetHorizontalByteCount(int value)
		{
			if (_iHexMaxHBytes == value)
				return;
			_iHexMaxHBytes = value;
			OnHorizontalByteCountChanged(EventArgs.Empty);
		}

		private void SetVerticalByteCount(int value)
		{
			if (_iHexMaxVBytes == value)
				return;
			_iHexMaxVBytes = value;
			OnVerticalByteCountChanged(EventArgs.Empty);
		}

		private void CheckCurrentLineChanged()
		{
			long num = (long)Math.Floor((double)_bytePos / (double)_iHexMaxHBytes) + 1L;
			if (_byteProvider == null && _currentLine != 0L)
			{
				_currentLine = 0L;
				OnCurrentLineChanged(EventArgs.Empty);
			}
			else
			{
				if (num == _currentLine)
					return;
				_currentLine = num;
				OnCurrentLineChanged(EventArgs.Empty);
			}
		}

		private void CheckCurrentPositionInLineChanged()
		{
			int num = GetGridBytePoint(_bytePos).X + 1;
			if (_byteProvider == null && _currentPositionInLine != 0)
			{
				_currentPositionInLine = 0;
				OnCurrentPositionInLineChanged(EventArgs.Empty);
			}
			else
			{
				if (num == _currentPositionInLine)
					return;
				_currentPositionInLine = num;
				OnCurrentPositionInLineChanged(EventArgs.Empty);
			}
		}

		protected virtual void OnInsertActiveChanged(EventArgs e)
		{
			if (InsertActiveChanged == null)
				return;
			InsertActiveChanged((object)this, e);
		}

		protected virtual void OnReadOnlyChanged(EventArgs e)
		{
			if (ReadOnlyChanged == null)
				return;
			ReadOnlyChanged((object)this, e);
		}

		protected virtual void OnByteProviderChanged(EventArgs e)
		{
			if (ByteProviderChanged == null)
				return;
			ByteProviderChanged((object)this, e);
		}

		protected virtual void OnSelectionStartChanged(EventArgs e)
		{
			if (SelectionStartChanged == null)
				return;
			SelectionStartChanged((object)this, e);
		}

		protected virtual void OnSelectionLengthChanged(EventArgs e)
		{
			if (SelectionLengthChanged == null)
				return;
			SelectionLengthChanged((object)this, e);
		}

		protected virtual void OnLineInfoVisibleChanged(EventArgs e)
		{
			if (LineInfoVisibleChanged == null)
				return;
			LineInfoVisibleChanged((object)this, e);
		}

		protected virtual void OnStringViewVisibleChanged(EventArgs e)
		{
			if (StringViewVisibleChanged == null)
				return;
			StringViewVisibleChanged((object)this, e);
		}

		protected virtual void OnBorderStyleChanged(EventArgs e)
		{
			if (BorderStyleChanged == null)
				return;
			BorderStyleChanged((object)this, e);
		}

		protected virtual void OnUseFixedBytesPerLineChanged(EventArgs e)
		{
			if (UseFixedBytesPerLineChanged == null)
				return;
			UseFixedBytesPerLineChanged((object)this, e);
		}

		protected virtual void OnBytesPerLineChanged(EventArgs e)
		{
			if (BytesPerLineChanged == null)
				return;
			BytesPerLineChanged((object)this, e);
		}

		protected virtual void OnVScrollBarVisibleChanged(EventArgs e)
		{
			if (VScrollBarVisibleChanged == null)
				return;
			VScrollBarVisibleChanged((object)this, e);
		}

		protected virtual void OnHexCasingChanged(EventArgs e)
		{
			if (HexCasingChanged == null)
				return;
			HexCasingChanged((object)this, e);
		}

		protected virtual void OnHorizontalByteCountChanged(EventArgs e)
		{
			if (HorizontalByteCountChanged == null)
				return;
			HorizontalByteCountChanged((object)this, e);
		}

		protected virtual void OnVerticalByteCountChanged(EventArgs e)
		{
			if (VerticalByteCountChanged == null)
				return;
			VerticalByteCountChanged((object)this, e);
		}

		protected virtual void OnCurrentLineChanged(EventArgs e)
		{
			if (CurrentLineChanged == null)
				return;
			CurrentLineChanged((object)this, e);
		}

		protected virtual void OnCurrentPositionInLineChanged(EventArgs e)
		{
			if (CurrentPositionInLineChanged == null)
				return;
			CurrentPositionInLineChanged((object)this, e);
		}

		protected virtual void OnCopied(EventArgs e)
		{
			if (Copied == null)
				return;
			Copied((object)this, e);
		}

		protected virtual void OnCopiedHex(EventArgs e)
		{
			if (CopiedHex == null)
				return;
			CopiedHex((object)this, e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (!Focused)
				Focus();
			if (e.Button == MouseButtons.Left)
				SetCaretPosition(new Point(e.X, e.Y));
			base.OnMouseDown(e);
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			PerformScrollLines(-(e.Delta * SystemInformation.MouseWheelScrollLines / 120));
			base.OnMouseWheel(e);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			UpdateRectanglePositioning();
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			CreateCaret();
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			DestroyCaret();
		}

		private void _byteProvider_LengthChanged(object sender, EventArgs e)
		{
			UpdateScrollSize();
		}

		private interface IKeyInterpreter
		{
			void Activate();

			void Deactivate();

			bool PreProcessWmKeyUp(ref Message m);

			bool PreProcessWmChar(ref Message m);

			bool PreProcessWmKeyDown(ref Message m);

			PointF GetCaretPointF(long byteIndex);
		}

		private class EmptyKeyInterpreter : HexBox.IKeyInterpreter
		{
			private HexBox _hexBox;

			public EmptyKeyInterpreter(HexBox hexBox)
			{
				_hexBox = hexBox;
			}

			public void Activate()
			{
			}

			public void Deactivate()
			{
			}

			public bool PreProcessWmKeyUp(ref Message m)
			{
				return _hexBox.BasePreProcessMessage(ref m);
			}

			public bool PreProcessWmChar(ref Message m)
			{
				return _hexBox.BasePreProcessMessage(ref m);
			}

			public bool PreProcessWmKeyDown(ref Message m)
			{
				return _hexBox.BasePreProcessMessage(ref m);
			}

			public PointF GetCaretPointF(long byteIndex)
			{
				return new PointF();
			}
		}

		private class KeyInterpreter : HexBox.IKeyInterpreter
		{
			protected HexBox _hexBox;
			protected bool _shiftDown;
			private bool _mouseDown;
			private BytePositionInfo _bpiStart;
			private BytePositionInfo _bpi;
			private Dictionary<Keys, HexBox.KeyInterpreter.MessageDelegate> _messageHandlers;

			private Dictionary<Keys, HexBox.KeyInterpreter.MessageDelegate> MessageHandlers
			{
				get
				{
					if (_messageHandlers == null)
					{
						_messageHandlers = new Dictionary<Keys, HexBox.KeyInterpreter.MessageDelegate>();
						_messageHandlers.Add(Keys.Left, new HexBox.KeyInterpreter.MessageDelegate(PreProcessWmKeyDown_Left));
						_messageHandlers.Add(Keys.Up, new HexBox.KeyInterpreter.MessageDelegate(PreProcessWmKeyDown_Up));
						_messageHandlers.Add(Keys.Right, new HexBox.KeyInterpreter.MessageDelegate(PreProcessWmKeyDown_Right));
						_messageHandlers.Add(Keys.Down, new HexBox.KeyInterpreter.MessageDelegate(PreProcessWmKeyDown_Down));
						_messageHandlers.Add(Keys.Prior, new HexBox.KeyInterpreter.MessageDelegate(PreProcessWmKeyDown_PageUp));
						_messageHandlers.Add(Keys.Next | Keys.Shift, new HexBox.KeyInterpreter.MessageDelegate(PreProcessWmKeyDown_PageDown));
						_messageHandlers.Add(Keys.Left | Keys.Shift, new HexBox.KeyInterpreter.MessageDelegate(PreProcessWmKeyDown_ShiftLeft));
						_messageHandlers.Add(Keys.Up | Keys.Shift, new HexBox.KeyInterpreter.MessageDelegate(PreProcessWmKeyDown_ShiftUp));
						_messageHandlers.Add(Keys.Right | Keys.Shift, new HexBox.KeyInterpreter.MessageDelegate(PreProcessWmKeyDown_ShiftRight));
						_messageHandlers.Add(Keys.Down | Keys.Shift, new HexBox.KeyInterpreter.MessageDelegate(PreProcessWmKeyDown_ShiftDown));
						_messageHandlers.Add(Keys.Tab, new HexBox.KeyInterpreter.MessageDelegate(PreProcessWmKeyDown_Tab));
						_messageHandlers.Add(Keys.Back, new HexBox.KeyInterpreter.MessageDelegate(PreProcessWmKeyDown_Back));
						_messageHandlers.Add(Keys.Delete, new HexBox.KeyInterpreter.MessageDelegate(PreProcessWmKeyDown_Delete));
						_messageHandlers.Add(Keys.Home, new HexBox.KeyInterpreter.MessageDelegate(PreProcessWmKeyDown_Home));
						_messageHandlers.Add(Keys.End, new HexBox.KeyInterpreter.MessageDelegate(PreProcessWmKeyDown_End));
						_messageHandlers.Add(Keys.ShiftKey | Keys.Shift, new HexBox.KeyInterpreter.MessageDelegate(PreProcessWmKeyDown_ShiftShiftKey));
						_messageHandlers.Add(Keys.C | Keys.Control, new HexBox.KeyInterpreter.MessageDelegate(PreProcessWmKeyDown_ControlC));
						_messageHandlers.Add(Keys.X | Keys.Control, new HexBox.KeyInterpreter.MessageDelegate(PreProcessWmKeyDown_ControlX));
						_messageHandlers.Add(Keys.V | Keys.Control, new HexBox.KeyInterpreter.MessageDelegate(PreProcessWmKeyDown_ControlV));
					}
					return _messageHandlers;
				}
			}

			public KeyInterpreter(HexBox hexBox)
			{
				_hexBox = hexBox;
			}

			public virtual void Activate()
			{
				_hexBox.MouseDown += new MouseEventHandler(BeginMouseSelection);
				_hexBox.MouseMove += new MouseEventHandler(UpdateMouseSelection);
				_hexBox.MouseUp += new MouseEventHandler(EndMouseSelection);
			}

			public virtual void Deactivate()
			{
				_hexBox.MouseDown -= new MouseEventHandler(BeginMouseSelection);
				_hexBox.MouseMove -= new MouseEventHandler(UpdateMouseSelection);
				_hexBox.MouseUp -= new MouseEventHandler(EndMouseSelection);
			}

			private void BeginMouseSelection(object sender, MouseEventArgs e)
			{
				if (e.Button != MouseButtons.Left)
					return;
				_mouseDown = true;
				if (!_shiftDown)
				{
					_bpiStart = new BytePositionInfo(_hexBox._bytePos, _hexBox._byteCharacterPos);
					_hexBox.ReleaseSelection();
				}
				else
					UpdateMouseSelection((object)this, e);
			}

			private void UpdateMouseSelection(object sender, MouseEventArgs e)
			{
				if (!_mouseDown)
					return;
				_bpi = GetBytePositionInfo(new Point(e.X, e.Y));
				long index = _bpi.Index;
				long start;
				long length;
				if (index < _bpiStart.Index)
				{
					start = index;
					length = _bpiStart.Index - index;
				}
				else if (index > _bpiStart.Index)
				{
					start = _bpiStart.Index;
					length = index - start;
				}
				else
				{
					start = _hexBox._bytePos;
					length = 0L;
				}
				if (start != _hexBox._bytePos || length != _hexBox._selectionLength)
				{
					_hexBox.InternalSelect(start, length);
					_hexBox.ScrollByteIntoView(_bpi.Index);
				}
				long num1 = _hexBox._bytePos;
				long num2 = _hexBox._selectionLength;
				if (_bpiStart.Index <= num1)
				{
					long num3 = num2 + (long)_hexBox._iHexMaxHBytes;
					_hexBox.ScrollByteIntoView(num1 + num3);
				}
				else
				{
					long num3 = num2 - (long)_hexBox._iHexMaxHBytes;
					long num4;
					long num5;
					if (num3 < 0L)
					{
						num4 = _bpiStart.Index;
						num5 = -num3;
					}
					else
					{
						num4 = num1 + (long)_hexBox._iHexMaxHBytes;
						num5 = num3 - (long)_hexBox._iHexMaxHBytes;
					}
					_hexBox.ScrollByteIntoView();
				}
			}

			private void EndMouseSelection(object sender, MouseEventArgs e)
			{
				_mouseDown = false;
			}

			public virtual bool PreProcessWmKeyDown(ref Message m)
			{
				Keys index = (Keys)m.WParam.ToInt32() | Control.ModifierKeys;
				bool flag = MessageHandlers.ContainsKey(index);
				if (flag && RaiseKeyDown(index))
					return true;
				HexBox.KeyInterpreter.MessageDelegate messageDelegate;
				return (flag ? MessageHandlers[index] : (messageDelegate = new HexBox.KeyInterpreter.MessageDelegate(PreProcessWmKeyDown_Default)))(ref m);
			}

			protected bool PreProcessWmKeyDown_Default(ref Message m)
			{
				_hexBox.ScrollByteIntoView();
				return _hexBox.BasePreProcessMessage(ref m);
			}

			protected bool RaiseKeyDown(Keys keyData)
			{
				KeyEventArgs e = new KeyEventArgs(keyData);
				_hexBox.OnKeyDown(e);
				return e.Handled;
			}

			protected virtual bool PreProcessWmKeyDown_Left(ref Message m)
			{
				return PerformPosMoveLeft();
			}

			protected virtual bool PreProcessWmKeyDown_Up(ref Message m)
			{
				long num1 = _hexBox._bytePos;
				int num2 = _hexBox._byteCharacterPos;
				if (num1 != 0L || num2 != 0)
				{
					long bytePos = Math.Max(-1L, num1 - (long)_hexBox._iHexMaxHBytes);
					if (bytePos == -1L)
						return true;
					_hexBox.SetPosition(bytePos);
					if (bytePos < _hexBox._startByte)
						_hexBox.PerformScrollLineUp();
					_hexBox.UpdateCaret();
					_hexBox.Invalidate();
				}
				_hexBox.ScrollByteIntoView();
				_hexBox.ReleaseSelection();
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_Right(ref Message m)
			{
				return PerformPosMoveRight();
			}

			protected virtual bool PreProcessWmKeyDown_Down(ref Message m)
			{
				long num = _hexBox._bytePos;
				int byteCharacterPos = _hexBox._byteCharacterPos;
				if (num == _hexBox._byteProvider.Length && byteCharacterPos == 0)
					return true;
				long bytePos = Math.Min(_hexBox._byteProvider.Length, num + (long)_hexBox._iHexMaxHBytes);
				if (bytePos == _hexBox._byteProvider.Length)
					byteCharacterPos = 0;
				_hexBox.SetPosition(bytePos, byteCharacterPos);
				if (bytePos > _hexBox._endByte - 1L)
					_hexBox.PerformScrollLineDown();
				_hexBox.UpdateCaret();
				_hexBox.ScrollByteIntoView();
				_hexBox.ReleaseSelection();
				_hexBox.Invalidate();
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_PageUp(ref Message m)
			{
				long num1 = _hexBox._bytePos;
				int num2 = _hexBox._byteCharacterPos;
				if (num1 == 0L && num2 == 0)
					return true;
				long bytePos = Math.Max(0L, num1 - (long)_hexBox._iHexMaxBytes);
				if (bytePos == 0L)
					return true;
				_hexBox.SetPosition(bytePos);
				if (bytePos < _hexBox._startByte)
					_hexBox.PerformScrollPageUp();
				_hexBox.ReleaseSelection();
				_hexBox.UpdateCaret();
				_hexBox.Invalidate();
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_PageDown(ref Message m)
			{
				long num = _hexBox._bytePos;
				int byteCharacterPos = _hexBox._byteCharacterPos;
				if (num == _hexBox._byteProvider.Length && byteCharacterPos == 0)
					return true;
				long bytePos = Math.Min(_hexBox._byteProvider.Length, num + (long)_hexBox._iHexMaxBytes);
				if (bytePos == _hexBox._byteProvider.Length)
					byteCharacterPos = 0;
				_hexBox.SetPosition(bytePos, byteCharacterPos);
				if (bytePos > _hexBox._endByte - 1L)
					_hexBox.PerformScrollPageDown();
				_hexBox.ReleaseSelection();
				_hexBox.UpdateCaret();
				_hexBox.Invalidate();
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_ShiftLeft(ref Message m)
			{
				long start = _hexBox._bytePos;
				long num = _hexBox._selectionLength;
				if (start + num < 1L)
					return true;
				long length;
				if (start + num <= _bpiStart.Index)
				{
					if (start == 0L)
						return true;
					--start;
					length = num + 1L;
				}
				else
					length = Math.Max(0L, num - 1L);
				_hexBox.ScrollByteIntoView();
				_hexBox.InternalSelect(start, length);
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_ShiftUp(ref Message m)
			{
				long start = _hexBox._bytePos;
				long num1 = _hexBox._selectionLength;
				if (start - (long)_hexBox._iHexMaxHBytes < 0L && start <= _bpiStart.Index)
					return true;
				if (_bpiStart.Index >= start + num1)
				{
					_hexBox.InternalSelect(start - (long)_hexBox._iHexMaxHBytes, num1 + (long)_hexBox._iHexMaxHBytes);
					_hexBox.ScrollByteIntoView();
				}
				else
				{
					long num2 = num1 - (long)_hexBox._iHexMaxHBytes;
					if (num2 < 0L)
					{
						_hexBox.InternalSelect(_bpiStart.Index + num2, -num2);
						_hexBox.ScrollByteIntoView();
					}
					else
					{
						long length = num2 - (long)_hexBox._iHexMaxHBytes;
						_hexBox.InternalSelect(start, length);
						_hexBox.ScrollByteIntoView(start + length);
					}
				}
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_ShiftRight(ref Message m)
			{
				long start = _hexBox._bytePos;
				long num = _hexBox._selectionLength;
				if (start + num >= _hexBox._byteProvider.Length)
					return true;
				if (_bpiStart.Index <= start)
				{
					long length = num + 1L;
					_hexBox.InternalSelect(start, length);
					_hexBox.ScrollByteIntoView(start + length);
				}
				else
				{
					_hexBox.InternalSelect(start + 1L, Math.Max(0L, num - 1L));
					_hexBox.ScrollByteIntoView();
				}
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_ShiftDown(ref Message m)
			{
				long start1 = _hexBox._bytePos;
				long num = _hexBox._selectionLength;
				long length1 = _hexBox._byteProvider.Length;
				if (start1 + num + (long)_hexBox._iHexMaxHBytes > length1)
					return true;
				if (_bpiStart.Index <= start1)
				{
					long length2 = num + (long)_hexBox._iHexMaxHBytes;
					_hexBox.InternalSelect(start1, length2);
					_hexBox.ScrollByteIntoView(start1 + length2);
				}
				else
				{
					long length2 = num - (long)_hexBox._iHexMaxHBytes;
					long start2;
					if (length2 < 0L)
					{
						start2 = _bpiStart.Index;
						length2 = -length2;
					}
					else
						start2 = start1 + (long)_hexBox._iHexMaxHBytes;
					_hexBox.InternalSelect(start2, length2);
					_hexBox.ScrollByteIntoView();
				}
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_Tab(ref Message m)
			{
				if (_hexBox._stringViewVisible && _hexBox._keyInterpreter.GetType() == typeof(HexBox.KeyInterpreter))
				{
					_hexBox.ActivateStringKeyInterpreter();
					_hexBox.ScrollByteIntoView();
					_hexBox.ReleaseSelection();
					_hexBox.UpdateCaret();
					_hexBox.Invalidate();
					return true;
				}
				if (_hexBox.Parent == null)
					return true;
				_hexBox.Parent.SelectNextControl((Control)_hexBox, true, true, true, true);
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_ShiftTab(ref Message m)
			{
				if (_hexBox._keyInterpreter is HexBox.StringKeyInterpreter)
				{
					_shiftDown = false;
					_hexBox.ActivateKeyInterpreter();
					_hexBox.ScrollByteIntoView();
					_hexBox.ReleaseSelection();
					_hexBox.UpdateCaret();
					_hexBox.Invalidate();
					return true;
				}
				if (_hexBox.Parent == null)
					return true;
				_hexBox.Parent.SelectNextControl((Control)_hexBox, false, true, true, true);
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_Back(ref Message m)
			{
				if (!_hexBox._byteProvider.SupportsDeleteBytes() || _hexBox.ReadOnly)
					return true;
				long num1 = _hexBox._bytePos;
				long num2 = _hexBox._selectionLength;
				long val2 = _hexBox._byteCharacterPos != 0 || num2 != 0L ? num1 : num1 - 1L;
				if (val2 < 0L && num2 < 1L)
					return true;
				long length = num2 > 0L ? num2 : 1L;
				_hexBox._byteProvider.DeleteBytes(Math.Max(0L, val2), length);
				_hexBox.UpdateScrollSize();
				if (num2 == 0L)
					PerformPosMoveLeftByte();
				_hexBox.ReleaseSelection();
				_hexBox.Invalidate();
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_Delete(ref Message m)
			{
				if (!_hexBox._byteProvider.SupportsDeleteBytes() || _hexBox.ReadOnly)
					return true;
				long index = _hexBox._bytePos;
				long num = _hexBox._selectionLength;
				if (index >= _hexBox._byteProvider.Length)
					return true;
				long length = num > 0L ? num : 1L;
				_hexBox._byteProvider.DeleteBytes(index, length);
				_hexBox.UpdateScrollSize();
				_hexBox.ReleaseSelection();
				_hexBox.Invalidate();
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_Home(ref Message m)
			{
				long num1 = _hexBox._bytePos;
				int num2 = _hexBox._byteCharacterPos;
				if (num1 < 1L)
					return true;
				_hexBox.SetPosition(0L, 0);
				_hexBox.ScrollByteIntoView();
				_hexBox.UpdateCaret();
				_hexBox.ReleaseSelection();
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_End(ref Message m)
			{
				long num1 = _hexBox._bytePos;
				int num2 = _hexBox._byteCharacterPos;
				if (num1 >= _hexBox._byteProvider.Length - 1L)
					return true;
				_hexBox.SetPosition(_hexBox._byteProvider.Length, 0);
				_hexBox.ScrollByteIntoView();
				_hexBox.UpdateCaret();
				_hexBox.ReleaseSelection();
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_ShiftShiftKey(ref Message m)
			{
				if (_mouseDown || _shiftDown)
					return true;
				_shiftDown = true;
				if (_hexBox._selectionLength > 0L)
					return true;
				_bpiStart = new BytePositionInfo(_hexBox._bytePos, _hexBox._byteCharacterPos);
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_ControlC(ref Message m)
			{
				_hexBox.Copy();
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_ControlX(ref Message m)
			{
				_hexBox.Cut();
				return true;
			}

			protected virtual bool PreProcessWmKeyDown_ControlV(ref Message m)
			{
				_hexBox.Paste();
				return true;
			}

			public virtual bool PreProcessWmChar(ref Message m)
			{
				if (Control.ModifierKeys == Keys.Control)
					return _hexBox.BasePreProcessMessage(ref m);
				bool flag1 = _hexBox._byteProvider.SupportsWriteByte();
				bool flag2 = _hexBox._byteProvider.SupportsInsertBytes();
				bool flag3 = _hexBox._byteProvider.SupportsDeleteBytes();
				long num1 = _hexBox._bytePos;
				long length = _hexBox._selectionLength;
				int byteCharacterPos = _hexBox._byteCharacterPos;
				if (!flag1 && num1 != _hexBox._byteProvider.Length || !flag2 && num1 == _hexBox._byteProvider.Length)
					return _hexBox.BasePreProcessMessage(ref m);
				char ch = (char)m.WParam.ToInt32();
				if (!Uri.IsHexDigit(ch))
					return _hexBox.BasePreProcessMessage(ref m);
				if (RaiseKeyPress(ch) || _hexBox.ReadOnly)
					return true;
				bool flag4 = num1 == _hexBox._byteProvider.Length;
				if (!flag4 && flag2 && (_hexBox.InsertActive && byteCharacterPos == 0))
					flag4 = true;
				if (flag3 && flag2 && length > 0L)
				{
					_hexBox._byteProvider.DeleteBytes(num1, length);
					flag4 = true;
					byteCharacterPos = 0;
					_hexBox.SetPosition(num1, byteCharacterPos);
				}
				_hexBox.ReleaseSelection();
				string str1 = (!flag4 ? _hexBox._byteProvider.ReadByte(num1) : (byte)0).ToString("X", (IFormatProvider)Thread.CurrentThread.CurrentCulture);
				if (str1.Length == 1)
					str1 = "0" + str1;
				string str2 = ch.ToString();
				byte num2 = byte.Parse(byteCharacterPos != 0 ? str1.Substring(0, 1) + str2 : str2 + str1.Substring(1, 1), NumberStyles.AllowHexSpecifier, (IFormatProvider)Thread.CurrentThread.CurrentCulture);
				if (flag4)
					_hexBox._byteProvider.InsertBytes(num1, new byte[1]
          {
            num2
          });
				else
					_hexBox._byteProvider.WriteByte(num1, num2);
				PerformPosMoveRight();
				_hexBox.Invalidate();
				return true;
			}

			protected bool RaiseKeyPress(char keyChar)
			{
				KeyPressEventArgs e = new KeyPressEventArgs(keyChar);
				_hexBox.OnKeyPress(e);
				return e.Handled;
			}

			public virtual bool PreProcessWmKeyUp(ref Message m)
			{
				Keys keyData = (Keys)m.WParam.ToInt32() | Control.ModifierKeys;
				switch (keyData)
				{
					case Keys.ShiftKey:
					case Keys.Insert:
						if (RaiseKeyUp(keyData))
							return true;
						break;
				}
				switch (keyData)
				{
					case Keys.ShiftKey:
						_shiftDown = false;
						return true;
					case Keys.Insert:
						return PreProcessWmKeyUp_Insert(ref m);
					default:
						return _hexBox.BasePreProcessMessage(ref m);
				}
			}

			protected virtual bool PreProcessWmKeyUp_Insert(ref Message m)
			{
				_hexBox.InsertActive = !_hexBox.InsertActive;
				return true;
			}

			protected bool RaiseKeyUp(Keys keyData)
			{
				KeyEventArgs e = new KeyEventArgs(keyData);
				_hexBox.OnKeyUp(e);
				return e.Handled;
			}

			protected virtual bool PerformPosMoveLeft()
			{
				long bytePos = _hexBox._bytePos;
				long num1 = _hexBox._selectionLength;
				int num2 = _hexBox._byteCharacterPos;
				if (num1 != 0L)
				{
					int byteCharacterPos = 0;
					_hexBox.SetPosition(bytePos, byteCharacterPos);
					_hexBox.ReleaseSelection();
				}
				else
				{
					if (bytePos == 0L && num2 == 0)
						return true;
					int byteCharacterPos;
					if (num2 > 0)
					{
						byteCharacterPos = num2 - 1;
					}
					else
					{
						bytePos = Math.Max(0L, bytePos - 1L);
						byteCharacterPos = num2 + 1;
					}
					_hexBox.SetPosition(bytePos, byteCharacterPos);
					if (bytePos < _hexBox._startByte)
						_hexBox.PerformScrollLineUp();
					_hexBox.UpdateCaret();
					_hexBox.Invalidate();
				}
				_hexBox.ScrollByteIntoView();
				return true;
			}

			protected virtual bool PerformPosMoveRight()
			{
				long bytePos = _hexBox._bytePos;
				int num1 = _hexBox._byteCharacterPos;
				long num2 = _hexBox._selectionLength;
				if (num2 != 0L)
				{
					_hexBox.SetPosition(bytePos + num2, 0);
					_hexBox.ReleaseSelection();
				}
				else if (bytePos != _hexBox._byteProvider.Length || num1 != 0)
				{
					int byteCharacterPos;
					if (num1 > 0)
					{
						bytePos = Math.Min(_hexBox._byteProvider.Length, bytePos + 1L);
						byteCharacterPos = 0;
					}
					else
						byteCharacterPos = num1 + 1;
					_hexBox.SetPosition(bytePos, byteCharacterPos);
					if (bytePos > _hexBox._endByte - 1L)
						_hexBox.PerformScrollLineDown();
					_hexBox.UpdateCaret();
					_hexBox.Invalidate();
				}
				_hexBox.ScrollByteIntoView();
				return true;
			}

			protected virtual bool PerformPosMoveLeftByte()
			{
				long num1 = _hexBox._bytePos;
				int num2 = _hexBox._byteCharacterPos;
				if (num1 == 0L)
					return true;
				long bytePos = Math.Max(0L, num1 - 1L);
				int byteCharacterPos = 0;
				_hexBox.SetPosition(bytePos, byteCharacterPos);
				if (bytePos < _hexBox._startByte)
					_hexBox.PerformScrollLineUp();
				_hexBox.UpdateCaret();
				_hexBox.ScrollByteIntoView();
				_hexBox.Invalidate();
				return true;
			}

			protected virtual bool PerformPosMoveRightByte()
			{
				long num1 = _hexBox._bytePos;
				int num2 = _hexBox._byteCharacterPos;
				if (num1 == _hexBox._byteProvider.Length)
					return true;
				long bytePos = Math.Min(_hexBox._byteProvider.Length, num1 + 1L);
				int byteCharacterPos = 0;
				_hexBox.SetPosition(bytePos, byteCharacterPos);
				if (bytePos > _hexBox._endByte - 1L)
					_hexBox.PerformScrollLineDown();
				_hexBox.UpdateCaret();
				_hexBox.ScrollByteIntoView();
				_hexBox.Invalidate();
				return true;
			}

			public virtual PointF GetCaretPointF(long byteIndex)
			{
				return _hexBox.GetBytePointF(byteIndex);
			}

			protected virtual BytePositionInfo GetBytePositionInfo(Point p)
			{
				return _hexBox.GetHexBytePositionInfo(p);
			}

			private delegate bool MessageDelegate(ref Message m);
		}

		private class StringKeyInterpreter : HexBox.KeyInterpreter
		{
			public StringKeyInterpreter(HexBox hexBox)
				: base(hexBox)
			{
				_hexBox._byteCharacterPos = 0;
			}

			public override bool PreProcessWmKeyDown(ref Message m)
			{
				Keys keyData = (Keys)m.WParam.ToInt32() | Control.ModifierKeys;
				switch (keyData)
				{
					case Keys.Tab:
					case Keys.Tab | Keys.Shift:
						if (RaiseKeyDown(keyData))
							return true;
						break;
				}
				switch (keyData)
				{
					case Keys.Tab:
						return PreProcessWmKeyDown_Tab(ref m);
					case Keys.Tab | Keys.Shift:
						return PreProcessWmKeyDown_ShiftTab(ref m);
					default:
						return base.PreProcessWmKeyDown(ref m);
				}
			}

			protected override bool PreProcessWmKeyDown_Left(ref Message m)
			{
				return PerformPosMoveLeftByte();
			}

			protected override bool PreProcessWmKeyDown_Right(ref Message m)
			{
				return PerformPosMoveRightByte();
			}

			public override bool PreProcessWmChar(ref Message m)
			{
				if (Control.ModifierKeys == Keys.Control)
					return _hexBox.BasePreProcessMessage(ref m);
				bool flag1 = _hexBox._byteProvider.SupportsWriteByte();
				bool flag2 = _hexBox._byteProvider.SupportsInsertBytes();
				bool flag3 = _hexBox._byteProvider.SupportsDeleteBytes();
				long num1 = _hexBox._bytePos;
				long length = _hexBox._selectionLength;
				int num2 = _hexBox._byteCharacterPos;
				if (!flag1 && num1 != _hexBox._byteProvider.Length || !flag2 && num1 == _hexBox._byteProvider.Length)
					return _hexBox.BasePreProcessMessage(ref m);
				char ch = (char)m.WParam.ToInt32();
				if (RaiseKeyPress(ch) || _hexBox.ReadOnly)
					return true;
				bool flag4 = num1 == _hexBox._byteProvider.Length;
				if (!flag4 && flag2 && _hexBox.InsertActive)
					flag4 = true;
				if (flag3 && flag2 && length > 0L)
				{
					_hexBox._byteProvider.DeleteBytes(num1, length);
					flag4 = true;
					int byteCharacterPos = 0;
					_hexBox.SetPosition(num1, byteCharacterPos);
				}
				_hexBox.ReleaseSelection();
				byte num3 = _hexBox.ByteCharConverter.ToByte(ch);
				if (flag4)
					_hexBox._byteProvider.InsertBytes(num1, new byte[1]
          {
            num3
          });
				else
					_hexBox._byteProvider.WriteByte(num1, num3);
				PerformPosMoveRightByte();
				_hexBox.Invalidate();
				return true;
			}

			public override PointF GetCaretPointF(long byteIndex)
			{
				return _hexBox.GetByteStringPointF(_hexBox.GetGridBytePoint(byteIndex));
			}

			protected override BytePositionInfo GetBytePositionInfo(Point p)
			{
				return _hexBox.GetStringBytePositionInfo(p);
			}
		}
	}
}
