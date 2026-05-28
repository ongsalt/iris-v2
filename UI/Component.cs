using Windows.UI.Composition;

// this is ass
abstract class Component : IDisposable
{
  protected readonly Compositor Compositor = CompositionCommon.Compositor;
  public readonly ContainerVisual Root = CompositionCommon.Compositor.CreateContainerVisual();
#if DEBUG
  private Action? deregister;
#endif

  internal Component()
  {
#if DEBUG
    deregister = HotReloader.Register(GetType(), () =>
    {
      Root.Children.RemoveAll();
      InitializeContent();
    });
#endif

    InitializeContent();
  }

  // internal void Mount()
  // {

  // }

  public void InsertAtTop(Component child)
  {
    Root.Children.InsertAtTop(child.Root);
  }

  abstract public void InitializeContent();

  public void Dispose()
  {
#if DEBUG
    deregister?.Invoke();
#endif
    Root.Parent?.Children.Remove(Root);
  }
}