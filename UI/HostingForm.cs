using System.Numerics;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Composition;
using Windows.UI.Composition.Desktop;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

class HostingForm : Form
{
  protected override CreateParams CreateParams
  {
    get
    {
      CreateParams cp = base.CreateParams;
      cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_NOREDIRECTIONBITMAP;
      cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_TRANSPARENT;
      return cp;
    }
  }

  private readonly DesktopWindowTarget target;
  internal readonly SpriteVisual root;
  private readonly Compositor compositor = CompositionCommon.Compositor;


  public HostingForm()
  {
    // TopMost = false;
    FormBorderStyle = FormBorderStyle.None;
    ShowInTaskbar = false;
    
    target = compositor.CreateDesktopWindowTarget(new HWND(Handle), true);
    root = compositor.CreateSpriteVisual();
    target.Root = root;

    SetupWindowGeometry();
  }

  public void AddComponent(Component widget)
  {
    root.Children.InsertAtTop(widget.Root);
  }


  void SetupWindowGeometry()
  {
    var screen = Screen.FromControl(this);
    Size = screen.Bounds.Size;
    Location = new Point(0, 0);
  }

  protected override void WndProc(ref Message m)
  {
    const int WM_NCHITTEST = 0x0084;
    const int HTTRANSPARENT = -1;

    // ignore mouse input
    if (m.Msg == WM_NCHITTEST)
    {
      m.Result = HTTRANSPARENT;
      return;
    }

    base.WndProc(ref m);
  }
}