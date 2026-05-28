namespace iris_v2;

using System;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using Windows.UI;
using Windows.UI.Composition;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

static internal class Program
{
  /// <summary>
  ///  The main entry point for the application.
  /// </summary>
  [STAThread]
  static void Main()
  {
    DispatcherQueueHelper.InitDispatcherQueue();
    // HostingWindow.RegisterWindowClass();

    ApplicationConfiguration.Initialize();
    Application.SetColorMode(SystemColorMode.System);

    var host = new HostingForm();

    host.AddWidget(new TestWidget());

    SetupTray();
    Application.Run(host);
  }

  static void SetupTray()
  {
    var tray = new NotifyIcon
    {
      Visible = true,
      Icon = SystemIcons.Application,
      Text = "yo mama"
    };

    var contextMenuStrip = new ContextMenuStrip
    {
      Opacity = 0.9
    };
    contextMenuStrip.Items.Add("Exit", null, (_, _) =>
    {
      Application.Exit();
    });
    tray.ContextMenuStrip = contextMenuStrip;
  }
}