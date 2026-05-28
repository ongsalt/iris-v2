using System.Collections.ObjectModel;
using System.Numerics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.UI.Xaml.Controls;
using SignalsDotnet;
using Windows.UI.Composition;

namespace iris_v2.UI.Components;

partial class TextWithFilter(string text, IEnumerable<CompositionBrush>? brushes = null) : Component
{
  public Signal<string> Contents = new(text);
  public ObservableCollection<CompositionBrush> Brushes = new(brushes ?? [CompositionCommon.Black]);
  public Signal<Vector2> Bounds = new(new Vector2(100, 100), c => c with
  {
    RaiseOnlyWhenChanged = false
  });
  public Signal<CanvasTextFormat> Format = new(new CanvasTextFormat
  {
    FontFamily = "Lora",
    FontSize = 67,
  }, c => c with
  {
    RaiseOnlyWhenChanged = false
  });

  // public DropShadow? Shadow;

  public override void InitializeContent()
  {
    var canvasDevice = CanvasDevice.GetSharedDevice();
    var pathGeometry = Signal.Computed(() =>
    {
      var layout = new CanvasTextLayout(canvasDevice, Contents.Value, Format.Value, Bounds.Value.X, Bounds.Value.Y);
      var geometry = CanvasGeometry.CreateText(layout);
      var pathGeometry = Compositor.CreatePathGeometry(new CompositionPath(geometry));
      return pathGeometry;
    });

    var shapeLayer = Compositor.CreateShapeVisual();

    var shape = Compositor.CreateSpriteShape();
    shape.FillBrush = CompositionCommon.Black;
    shapeLayer.Shapes.Add(shape);
    shapeLayer.BorderMode = CompositionBorderMode.Soft;

    var visualSurface = Compositor.CreateVisualSurface();
    visualSurface.SourceVisual = shapeLayer;

    var maskSurface = Compositor.CreateSurfaceBrush(visualSurface);

    var shadow = Compositor.CreateDropShadow();
    shadow.BlurRadius = 67f;
    shadow.Opacity = 0.3f;
    shadow.Mask = maskSurface;

    var shadowVisual = Compositor.CreateSpriteVisual();
    shadowVisual.Shadow = shadow;
    Root.Children.InsertAtTop(shadowVisual);

    // TODO lifetime????
    new Effect(() =>
    {
      shape.Geometry = pathGeometry.Value;
      Console.WriteLine($"pathGeometry.Value changed");
    });
    new Effect(() =>
    {
      shapeLayer.Size = Bounds.Value;
      visualSurface.SourceSize = Bounds.Value;
      shadowVisual.Size = Bounds.Value;
    });

    // TODO: nuke this for brushes' onchanged
    foreach (var brush in Brushes)
    {
      var visual = CompositionCommon.CreateEffectVisual(maskSurface, brush);
      var effect = new Effect(() =>
      {
        visual.Size = Bounds.Value;
      });
      // effect.Dispose();
      Root.Children.InsertAtTop(visual);
    }
  }
}
