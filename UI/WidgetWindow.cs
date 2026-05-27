using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Composition;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using WinRT;
// using Microsoft.

class WidgetWindow : IDisposable
{
  private readonly Compositor compositor = new();
  private readonly HWND hwnd;
  private readonly SpriteVisual root;

  internal WidgetWindow()
  {
    hwnd = SetupCompositionWindow() ?? throw new Exception("idk how to use this");
    var target = compositor.CreateDesktopWindowTarget(hwnd, true);
    root = compositor.CreateSpriteVisual();
    root.Brush = compositor.CreateColorBrush(Windows.UI.Color.FromArgb(0x1f, 0xff, 0, 0));
    root.Size = new(10000, 10000);
    target.Root = root;

    InitContent();
  }

  private void InitContent()
  {
    var r = CreateVisual();
    var b = CreateBlurVisual();
    b.Offset = new(50, 50, 0);
    root.Children.InsertAtTop(b);
    root.Children.InsertAtBottom(r);
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


  static HWND? SetupCompositionWindow(string title = "Widget")
  {
    unsafe
    {
      var hInst = PInvoke.GetModuleHandle(null);

      var hwnd = PInvoke.CreateWindowEx(
        WINDOW_EX_STYLE.WS_EX_NOREDIRECTIONBITMAP,
        className,
        title,
        WINDOW_STYLE.WS_OVERLAPPEDWINDOW,
        0,
        0,
        200,
        200,
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