using System.Numerics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Composition;
using Windows.UI.Composition;

class TestWidget : Widget
{
  public override void InitializeContent()
  {
    CreateFilter();
  }

  private void CreateFilter()
  {
    var b = CreateBlurVisual();
    var o = CreateExposureVisual(-0.50f);

    Root.Children.InsertAtTop(o);
    Root.Children.InsertAtTop(b);

    var offset = new Vector3(40, 40, 0);
    Root.Offset = offset;

    var animation = compositor.CreateSpringVector3Animation();
    animation.DampingRatio = 1;
    animation.InitialValue = offset;
    animation.FinalValue = offset * 5;
    Root.StartAnimation("Offset", animation);

    // var canvasDevice = d2d
    var canvasDevice = CanvasDevice.GetSharedDevice();    
    var polygon = CanvasGeometry.CreateCircle(canvasDevice, new(0, 0), 100);
    var pathGeometry = compositor.CreatePathGeometry(new CompositionPath(polygon));
    var clip = compositor.CreateGeometricClip();
    clip.Geometry = pathGeometry;
    Root.Clip = clip;


    var shadow = compositor.CreateDropShadow();
    shadow.BlurRadius = 68f;
    shadow.Color = Windows.UI.Color.FromArgb(0xa0, 0, 0, 0);
    Root.Shadow = shadow;
  }

  private Visual CreateBlurVisual()
  {
    var sprite = compositor.CreateSpriteVisual();
    GaussianBlurEffect blurEffect = new()
    {
      Name = "Blur",
      BlurAmount = 25.0f,
      BorderMode = EffectBorderMode.Hard,
      Source = new CompositionEffectSourceParameter("source")
    };

    CompositionEffectFactory blurEffectFactory = compositor.CreateEffectFactory(blurEffect);
    CompositionEffectBrush _backdropBrush = blurEffectFactory.CreateBrush();

    // Create a BackdropBrush and bind it to the EffectSourceParameter source.
    _backdropBrush.SetSourceParameter("source", compositor.CreateBackdropBrush());
    // _backdropBrush.SetSourceParameter("source", compositor.CreateHostBackdropBrush());

    // sprite.Brush = brush;
    sprite.Brush = _backdropBrush;
    sprite.Size = new Vector2(100, 100);

    return sprite;
  }

  private Visual CreateExposureVisual(float exposure)
  {
    var sprite = compositor.CreateSpriteVisual();
    ExposureEffect effect = new()
    {
      Name = "Blend",
      Source = new CompositionEffectSourceParameter("source"),
      Exposure = exposure
      // Foreground = new CompositionEffectSourceParameter("foreground"),

      // Mode = BlendEffectMode.Overlay
    };

    CompositionEffectFactory blurEffectFactory = compositor.CreateEffectFactory(effect);
    CompositionEffectBrush _backdropBrush = blurEffectFactory.CreateBrush();

    // Create a BackdropBrush and bind it to the EffectSourceParameter source.
    _backdropBrush.SetSourceParameter("source", compositor.CreateBackdropBrush());
    // _backdropBrush.SetSourceParameter("foreground", compositor.CreateColorBrush(Windows.UI.Color.FromArgb(0xff, 0xff, 0xff, 0xff)));

    // sprite.Brush = brush;
    sprite.Brush = _backdropBrush;
    sprite.Size = new Vector2(100, 100);

    return sprite;
  }

}