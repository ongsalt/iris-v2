using Windows.UI.Composition;

abstract class Widget : IDisposable
{
  public SpriteVisual Root { get; private set; }
  protected Compositor compositor { get; private set; }

  internal void Mount(Compositor compositor)
  {
    this.compositor = compositor;
    Root = compositor.CreateSpriteVisual();

    Console.WriteLine($"registered {GetType()}");
    HotReloader.Register(GetType(), () =>
    {
      Console.WriteLine($"reloading {GetType()}");
      Root.Children.RemoveAll();
      InitializeContent();
    });
  }

  abstract public void InitializeContent();

  public void Dispose()
  {
    Root.Parent?.Children.Remove(Root);
  }
  
}