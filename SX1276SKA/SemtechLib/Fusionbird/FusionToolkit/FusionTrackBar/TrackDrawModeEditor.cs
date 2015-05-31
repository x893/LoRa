using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Fusionbird.FusionToolkit.FusionTrackBar
{
  public class TrackDrawModeEditor : UITypeEditor
  {
    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    {
      TrackBarOwnerDrawParts barOwnerDrawParts = TrackBarOwnerDrawParts.None;
      if (!(value is TrackBarOwnerDrawParts) || provider == null)
        return value;
      IWindowsFormsEditorService formsEditorService = (IWindowsFormsEditorService) provider.GetService(typeof (IWindowsFormsEditorService));
      if (formsEditorService == null)
        return value;
      CheckedListBox checkedListBox = new CheckedListBox();
      checkedListBox.BorderStyle = BorderStyle.None;
      checkedListBox.CheckOnClick = true;
      checkedListBox.Items.Add((object) "Ticks", (((FusionTrackBar) context.Instance).OwnerDrawParts & TrackBarOwnerDrawParts.Ticks) == TrackBarOwnerDrawParts.Ticks);
      checkedListBox.Items.Add((object) "Thumb", (((FusionTrackBar) context.Instance).OwnerDrawParts & TrackBarOwnerDrawParts.Thumb) == TrackBarOwnerDrawParts.Thumb);
      checkedListBox.Items.Add((object) "Channel", (((FusionTrackBar) context.Instance).OwnerDrawParts & TrackBarOwnerDrawParts.Channel) == TrackBarOwnerDrawParts.Channel);
      formsEditorService.DropDownControl((Control) checkedListBox);
      foreach (object obj in checkedListBox.CheckedItems)
      {
        object objectValue = RuntimeHelpers.GetObjectValue(obj);
        barOwnerDrawParts |= (TrackBarOwnerDrawParts) Enum.Parse(typeof (TrackBarOwnerDrawParts), objectValue.ToString());
      }
      checkedListBox.Dispose();
      formsEditorService.CloseDropDown();
      return (object) barOwnerDrawParts;
    }

    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
    {
      return UITypeEditorEditStyle.DropDown;
    }
  }
}
