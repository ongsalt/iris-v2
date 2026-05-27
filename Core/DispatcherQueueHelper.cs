using System.Runtime.InteropServices;

class DispatcherQueueHelper
{
  [StructLayout(LayoutKind.Sequential)]
  private struct DispatcherQueueOptions
  {
    public int dwSize;
    public int threadType;
    public int apartmentType;
  }

  private const int DQTYPE_THREAD_CURRENT = 2;
  private const int DQA_APARTMENT_NONE = 0;

  [DllImport("coremessaging.dll")]
  private static extern int CreateDispatcherQueueController(
      [In] DispatcherQueueOptions options,
      [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object dispatcherQueueController);


  [STAThread]
  public static void InitDispatcherQueue()
  {
    // 1. Initialize the WinRT DispatcherQueue for the current UI thread
    object? dispatcherQueueController = null;
    var options = new DispatcherQueueOptions
    {
      dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions)),
      threadType = DQTYPE_THREAD_CURRENT,
      apartmentType = DQA_APARTMENT_NONE
    };

#pragma warning disable CS8601 // Possible null reference assignment.
    int hr = CreateDispatcherQueueController(options, ref dispatcherQueueController);
#pragma warning restore CS8601 // Possible null reference assignment.
    if (hr != 0)
    {
      throw new COMException("Failed to create DispatcherQueueController.", hr);
    }
  }
}