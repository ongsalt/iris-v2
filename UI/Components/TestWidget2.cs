using iris_v2.UI.Components;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Composition;

class TestWidget2 : Component
{
  public override void InitializeContent()
  {
    var textBrushes = CreateEffects();
    var text = new TextWithFilter("9:25", [textBrushes.Item1, textBrushes.Item2, textBrushes.Item2]);
    text.Bounds.Value = new(1000, 160);
    text.Root.Offset = new(156, 540, 0);
    text.Format.Mutate(it =>
    {
      it.FontSize = 130;
    });
    InsertAtTop(text);

    // Task.Run(async () =>
    // {
    //   for (int i = 0; i <= 10; i++)
    //   {
    //     await Task.Delay(1000);
    //     text.Contents.Value = $"{i}{i}:{i}{i}";
    //   }
    // });
  }

  private (CompositionBrush, CompositionBrush) CreateEffects()
  {
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
    
    var blendEffect = new BlendEffect()
    {
      Name = "Blend",
      Foreground = new CompositionEffectSourceParameter("bg"),
      Background = new CompositionEffectSourceParameter("fg"),
      Mode = BlendEffectMode.Overlay
    };

    var blendBrush = Compositor.CreateEffectFactory(blendEffect).CreateBrush();

    var blendColor = CompositionCommon.Black;
    blendBrush.SetSourceParameter("bg", Compositor.CreateBackdropBrush());
    blendBrush.SetSourceParameter("fg", blendColor);

    return (blurBrush, blendBrush);
  }
}