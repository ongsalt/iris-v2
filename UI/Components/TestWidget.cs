using System.Numerics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.UI.Text;
using Windows.UI.Composition;

using UIColor = Windows.UI.Color;

class TestWidget : Component
{
  // UIColor blendColor = UIColor.FromArgb(0xff, 0xff, 0xff, 0xff);
  UIColor blendColor = UIColor.FromArgb(0xff, 0, 0, 0);
  public override void InitializeContent()
  {
    CreateFilter();
  }

  private void CreateFilter()
  {
    var canvasDevice = CanvasDevice.GetSharedDevice();
    var layout = new CanvasTextLayout(canvasDevice, "text", new()
    {
      FontFamily = "Lora",
      FontSize = 100,
      // FontWeight = FontWeights.Bold,
    }, 600, 200);
    var geometry = CanvasGeometry.CreateText(layout);
    var pathGeometry = Compositor.CreatePathGeometry(new CompositionPath(geometry));

    var shapeLayer = Compositor.CreateShapeVisual();
    shapeLayer.Size = new(300, 300);
    var shape = Compositor.CreateSpriteShape();
    shape.Geometry = pathGeometry;
    shape.FillBrush = CompositionCommon.Black;
    shapeLayer.Shapes.Add(shape);
    shapeLayer.BorderMode = CompositionBorderMode.Soft;

    var visualSurface = Compositor.CreateVisualSurface();
    visualSurface.SourceVisual = shapeLayer;
    visualSurface.SourceSize = new Vector2(300, 300);

    var maskSurface = Compositor.CreateSurfaceBrush(visualSurface);
    
    var shadow = Compositor.CreateDropShadow();
    shadow.BlurRadius = 67f;
    // shadow.Color = UIColor.FromArgb(0xff, 0, 0, 0);
    shadow.Opacity = 0.3f;
    shadow.Mask = maskSurface;

    var shadowVisual = Compositor.CreateSpriteVisual();
    shadowVisual.Size = new Vector2(300, 300);
    shadowVisual.Shadow = shadow;

    var blurEffect = new GaussianBlurEffect()
    {
      Name = "Blur",
      BlurAmount = 15.0f,
      BorderMode = EffectBorderMode.Hard,
      Source = new CompositionEffectSourceParameter("source"),
      Optimization = EffectOptimization.Quality
    };

    var blurEffectFactory = Compositor.CreateEffectFactory(blurEffect);
    var blurBrush = blurEffectFactory.CreateBrush();
    blurBrush.SetSourceParameter("source", Compositor.CreateBackdropBrush());

    var blurVisual = CompositionCommon.CreateEffectVisual(maskSurface, blurBrush);
    blurVisual.Size = new Vector2(300, 300);

    var blendEffect = new BlendEffect()
    {
      Name = "Blend",
      Foreground = new CompositionEffectSourceParameter("bg"),
      Background = new CompositionEffectSourceParameter("fg"),
      Mode = BlendEffectMode.Overlay
    };

    var blendBrush = Compositor.CreateEffectFactory(blendEffect).CreateBrush();

    blendBrush.SetSourceParameter("bg", Compositor.CreateBackdropBrush());
    blendBrush.SetSourceParameter("fg", Compositor.CreateColorBrush(blendColor));

    var blendVisual = CompositionCommon.CreateEffectVisual(maskSurface, blendBrush);
    blendVisual.Size = new Vector2(300, 300);

    Root.Children.InsertAtTop(shadowVisual);
    Root.Children.InsertAtTop(blurVisual);
    Root.Children.InsertAtTop(blendVisual);
  }

}