using System.Diagnostics;
using System.Numerics;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Composition;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
// using Microsoft.

class WidgetWindow
{
  private readonly Compositor compositor = new();

  internal WidgetWindow()
  {
    var hwnd = SetupCompositionWindow();
    if (hwnd == null)
    {
      return;
    }
    var target = compositor.CreateDesktopWindowTarget(hwnd.Value, true);
    var container = compositor.CreateContainerVisual();
    // container.Size = new Vector2(1000, 1000);
    target.Root = container;

    var r = CreateVisual();
    var b = CreateBlurVisual();
    b.Offset = new (50, 50, 0);
    container.Children.InsertAtTop(b);
    container.Children.InsertAtBottom(r);
  }
  private Visual CreateVisual()
  {
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

    // sprite.Brush = brush;
    sprite.Brush = _backdropBrush;
    sprite.Size = new Vector2(100, 100);

    return sprite;
  }


  static HWND? SetupCompositionWindow()
  {
    unsafe
    {
      var hInst = PInvoke.GetModuleHandle(null);
      var className = "IrisV2WindowClass";

      // 1. Define the Window Blueprint (Class)
      fixed (char* pClassName = className)
      {
        var wndClass = new WNDCLASSW
        {
          lpfnWndProc = WndProc, // Tie our click/close handler here
          hInstance = (HINSTANCE)hInst.DangerousGetHandle(),
          lpszClassName = pClassName,
          // Give it a default dark/gray window brush so we can physically see it
          // hbrBackground = (HBRUSH)PInvoke.GetStockObject(GET_STOCK_OBJECT_FLAGS.GRAY_BRUSH)
        };

        if (PInvoke.RegisterClass(wndClass) == 0)
        {
          Debug.WriteLine("Failed to register window class.");
          return null;
        }

        var hwnd = PInvoke.CreateWindowEx(
          WINDOW_EX_STYLE.WS_EX_NOREDIRECTIONBITMAP,
          className,
          "composition",
          WINDOW_STYLE.WS_VISIBLE | WINDOW_STYLE.WS_OVERLAPPEDWINDOW,
          0,
          0,
          600,
          600,
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


}