using System;
using System.Drawing;

namespace Fusionbird.FusionToolkit.FusionTrackBar
{
  public class TrackBarDrawItemEventArgs : EventArgs
  {
    private Rectangle _bounds;
    private Graphics _graphics;
    private TrackBarItemState _state;

    public Rectangle Bounds
    {
      get
      {
        return this._bounds;
      }
    }

    public Graphics Graphics
    {
      get
      {
        return this._graphics;
      }
    }

    public TrackBarItemState State
    {
      get
      {
        return this._state;
      }
    }

    public TrackBarDrawItemEventArgs(Graphics graphics, Rectangle bounds, TrackBarItemState state)
    {
      this._graphics = graphics;
      this._bounds = bounds;
      this._state = state;
    }
  }
}
