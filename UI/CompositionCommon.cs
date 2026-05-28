using Windows.UI.Composition;

using UIColor = Windows.UI.Color;

class CompositionCommon
{
  public static readonly Compositor Compositor = new();
  public static readonly CompositionColorBrush Black = Compositor.CreateColorBrush(UIColor.FromArgb(0xff, 0, 0, 0));

  public static SpriteVisual CreateEffectVisual(CompositionSurfaceBrush? maskSurface, CompositionBrush effectBrush)
  {
    var sprite = Compositor.CreateSpriteVisual();

    if (maskSurface == null)
    {
      sprite.Brush = effectBrush;
    }
    else
    {
      var brush = Compositor.CreateMaskBrush();
      brush.Mask = maskSurface;
      brush.Source = effectBrush;
      sprite.Brush = brush;
    }
    sprite.BorderMode = CompositionBorderMode.Soft;
    return sprite;
  }

}