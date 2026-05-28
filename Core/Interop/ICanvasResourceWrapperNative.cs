using System.Runtime.InteropServices;

[ComImport]
[Guid("5F10688D-EA55-4D55-A3B0-4DDB55C0C20A")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface ICanvasResourceWrapperNative
{
    void GetNativeResource(
        IntPtr device,
        float dpi,
        ref Guid iid,
        out IntPtr resource);
}
