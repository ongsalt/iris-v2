using Windows.UI.Composition;
using Windows.UI.Composition.Desktop;
using Windows.Win32;
using Windows.Win32.Foundation;
using WinRT;

internal static class CompositorExtensions
{
    internal static DesktopWindowTarget CreateDesktopWindowTarget(this Compositor compositor, HWND hwnd, bool isTopmost)
    {
        var desktopInterop = compositor.As<ICompositorDesktopInterop>();
        desktopInterop.CreateDesktopWindowTarget(hwnd, isTopmost, out IntPtr targetPtr);
        
        // Marshal the pointer back to the WinRT object
        return MarshalInterface<DesktopWindowTarget>.FromAbi(targetPtr);
    }
}
