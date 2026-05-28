using System.Numerics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Composition;
using Microsoft.UI.Text;
using Windows.UI.Composition;
using Windows.UI.Text;

class TestWidget : Widget
{
  public override void InitializeContent()
  {
    CreateFilter();
  }

  private void CreateFilter()
  {
    var green = compositor.CreateColorBrush(Windows.UI.Color.FromArgb(0xff, 0, 0, 0));

    var offset = new Vector3(40, 40, 0);
    Root.Offset = offset * 0;
    var animation = compositor.CreateSpringVector3Animation();
    animation.DampingRatio = 1;
    animation.InitialValue = Vector3.Zero;
    animation.FinalValue = offset * 5;
    Root.StartAnimation("Offset", animation);

    // var canvasDevice = d2d
    var canvasDevice = CanvasDevice.GetSharedDevice();
    var layout = new CanvasTextLayout(canvasDevice, "u3y87ufehwjk", new()
    {
      FontFamily = "Segoe UI",
      FontSize = 100,
      FontWeight = FontWeights.Bold,
    }, 600, 200);
    var geometry = CanvasGeometry.CreateText(layout);

    var pathGeometry = compositor.CreatePathGeometry(new CompositionPath(geometry));

    var shapeLayer = compositor.CreateShapeVisual();
    shapeLayer.Size = new(300, 300);
    var shape = compositor.CreateSpriteShape();
    shape.Geometry = pathGeometry;
    shape.FillBrush = green;
    shapeLayer.Shapes.Add(shape);
    shapeLayer.BorderMode = CompositionBorderMode.Soft;

    var visualSurface = compositor.CreateVisualSurface();
    visualSurface.SourceVisual = shapeLayer;
    visualSurface.SourceSize = new Vector2(300, 300);
    visualSurface.SourceOffset = Vector2.Zero;

    var maskSurface = compositor.CreateSurfaceBrush(visualSurface);

    var mask = compositor.CreateMaskBrush();
    mask.Mask = maskSurface;
    mask.Source = green;
    // mask.

    var shadow = compositor.CreateDropShadow();
    shadow.BlurRadius = 67f;
    shadow.Color = Windows.UI.Color.FromArgb(0xff, 0, 0, 0);
    shadow.Opacity = 0.5f;
    shadow.Mask = maskSurface;
    Root.Shadow = shadow;

    var _maskVisual = compositor.CreateSpriteVisual();
    // _maskVisual.Brush = mask;
    _maskVisual.Size = new Vector2(300, 300);
    // _maskVisual.Offset = new Vector3(80, 80, 0);
    _maskVisual.Shadow = shadow;

    // _maskVisual.Children.InsertAtTop(shapeLayer);


    var blurEffect = new GaussianBlurEffect()
    {
      Name = "Blur",
      BlurAmount = 15.0f,
      BorderMode = EffectBorderMode.Hard,
      Source = new CompositionEffectSourceParameter("source"),
      Optimization = EffectOptimization.Quality
    };

    var blurEffectFactory = compositor.CreateEffectFactory(blurEffect);
    var blurBrush = blurEffectFactory.CreateBrush();
    blurBrush.SetSourceParameter("source", compositor.CreateBackdropBrush());

    var blurVisual = CreateEffectVisual(maskSurface, blurBrush);
    blurVisual.Size = new Vector2(300, 300);

    var blendEffect = new BlendEffect()
    {
      Name = "Blend",
      Foreground = new CompositionEffectSourceParameter("bg"),
      Background = new CompositionEffectSourceParameter("fg"),
      Mode = BlendEffectMode.Overlay
    };

    var blendBrush = compositor.CreateEffectFactory(blendEffect).CreateBrush();

    blendBrush.SetSourceParameter("bg", compositor.CreateBackdropBrush());
    blendBrush.SetSourceParameter("fg", compositor.CreateColorBrush(Windows.UI.Color.FromArgb(0xff, 0x00, 0x00, 0x00)));

    var blendVisual = CreateEffectVisual(maskSurface, blendBrush);
    blendVisual.Size = new Vector2(300, 300);

    Root.Children.InsertAtTop(_maskVisual);
    Root.Children.InsertAtTop(blurVisual);
    Root.Children.InsertAtTop(blendVisual);
  }

  private SpriteVisual CreateEffectVisual(CompositionSurfaceBrush? maskSurface, CompositionEffectBrush effectBrush)
  {
    var sprite = compositor.CreateSpriteVisual();

    if (maskSurface == null)
    {
      sprite.Brush = effectBrush;
    }
    else
    {
      var brush = compositor.CreateMaskBrush();
      brush.Mask = maskSurface;
      brush.Source = effectBrush;
      sprite.Brush = brush;
    }

    sprite.BorderMode = CompositionBorderMode.Soft;
    return sprite;
  }

}