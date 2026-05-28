using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using Microsoft.Graphics.Canvas.Effects;
using Windows.Graphics.DirectX;
using Windows.UI.Composition;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using WinRT;
// using Microsoft.

class HostingWindow : IDisposable
{
  private readonly Compositor compositor = new();
  private readonly HWND hwnd;
  internal readonly SpriteVisual root;

  internal HostingWindow()
  {
    var size = Screen.PrimaryScreen!.Bounds.Size;
    hwnd = SetupCompositionWindow(size) ?? throw new Exception("idk how to use this");
    var target = compositor.CreateDesktopWindowTarget(hwnd, true);
    root = compositor.CreateSpriteVisual();
    target.Root = root;

    InitContent();
  }

  private void InitContent()
  {
    // root.Brush = compositor.CreateColorBrush(Windows.UI.Color.FromArgb(0x1f, 0xff, 0, 0));
    var container = CreateFilter();

    root.Children.InsertAtTop(container);
  }

  private ContainerVisual CreateFilter()
  {
    var container = compositor.CreateContainerVisual();
    var b = CreateBlurVisual();
    var o = CreateExposureVisual(-0.50f);

    container.Children.InsertAtTop(o);
    container.Children.InsertAtTop(b);

    var offset = new Vector3(40, 40, 0);
    container.Offset = offset;

    var animation = compositor.CreateSpringVector3Animation();
    animation.DampingRatio = 1;
    animation.InitialValue = offset;
    animation.FinalValue = offset * 5;
    container.StartAnimation("Offset", animation);

    return container;
  }


  internal void Reset()
  {
    root.Children.RemoveAll();
    InitContent();
  }

  internal void Show()
  {
    PInvoke.ShowWindow(hwnd, SHOW_WINDOW_CMD.SW_SHOW);
  }

  private Visual CreateVisual()
  {
    var clip = compositor.CreateShapeVisual();

    var triangle = compositor.CreateSpriteShape();
    var geometry = compositor.CreateRoundedRectangleGeometry();
    geometry.Size = new(50, 50);
    triangle.Geometry = geometry;

    clip.Shapes.Add(triangle);

    var sprite = compositor.CreateSpriteVisual();

    var brush = compositor.CreateColorBrush();
    brush.Color = Windows.UI.Color.FromArgb(0xFF, 0, 0, 0xFF);
    sprite.Brush = brush;
    sprite.Size = new Vector2(100, 100);

    return sprite;
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

    var shadow = compositor.CreateDropShadow();
    shadow.BlurRadius = 67f;
    shadow.Color = Windows.UI.Color.FromArgb(0x50, 0, 0, 0);
    sprite.Shadow = shadow;

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


  private const string className = "IrisV2WindowClass";
  internal static void RegisterWindowClass()
  {
    unsafe
    {
      var hInst = PInvoke.GetModuleHandle(null);

      fixed (char* pClassName = className)
      {
        var wndClass = new WNDCLASSW
        {
          lpfnWndProc = WndProc, // Tie our click/close handler here
          hInstance = (HINSTANCE)hInst.DangerousGetHandle(),
          lpszClassName = pClassName,
          hCursor = PInvoke.LoadCursor(default, PInvoke.IDC_ARROW),
        };

        if (PInvoke.RegisterClass(wndClass) == 0)
        {
          Debug.WriteLine("Failed to register window class.");
        }
      }
    }
  }


  static HWND? SetupCompositionWindow(Size size, string title = "Widget")
  {
    unsafe
    {
      var hInst = PInvoke.GetModuleHandle(null);

      var hwnd = PInvoke.CreateWindowEx(
        WINDOW_EX_STYLE.WS_EX_NOREDIRECTIONBITMAP | WINDOW_EX_STYLE.WS_EX_TOOLWINDOW,
        className,
        title,
        WINDOW_STYLE.WS_OVERLAPPEDWINDOW | WINDOW_STYLE.WS_POPUP,
        0,
        0,
        size.Width,
        size.Height,
        HWND.Null,
        null,
        hInst,
        null
      );

      // force update -> so that there wont be any decoration
      PInvoke.SetWindowPos(
        hwnd,
        default, // Ignore Z-order
        0, 0, 0, 0, // Ignore X, Y, Width, Height
        SET_WINDOW_POS_FLAGS.SWP_FRAMECHANGED |
        SET_WINDOW_POS_FLAGS.SWP_NOMOVE |
        SET_WINDOW_POS_FLAGS.SWP_NOSIZE |
        SET_WINDOW_POS_FLAGS.SWP_NOZORDER |
        SET_WINDOW_POS_FLAGS.SWP_NOOWNERZORDER
      );
      return hwnd;
    }
  }


  private static LRESULT WndProc(HWND hwnd, uint msg, WPARAM wParam, LPARAM lParam)
  {
    if (msg == PInvoke.WM_NCCALCSIZE)
    {
      if (wParam.Value != 0) // wParam is TRUE when Windows wants to calculate the client area
      {
        // Returning 0 tells Windows to expand the client area to cover the entire window,
        // removing the standard title bar and borders while keeping the drop shadow.
        return (LRESULT)0;
      }
    }
    return PInvoke.DefWindowProc(hwnd, msg, wParam, lParam);
  }

  public void Dispose()
  {
    throw new NotImplementedException();
  }
}