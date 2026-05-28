using System.Runtime.InteropServices;

[ComImport]
[Guid("29E691FA-4567-4DCA-B319-D0F207EB6807")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface ICompositorDesktopInterop
{
    void CreateDesktopWindowTarget(
        IntPtr hwndTarget, 
        bool isTopmost, 
        out IntPtr test);
}
