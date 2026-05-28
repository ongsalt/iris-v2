namespace iris_v2;
static internal class Program
{
  [STAThread]
  static void Main()
  {
    DispatcherQueueHelper.InitDispatcherQueue();

    ApplicationConfiguration.Initialize();
    Application.SetColorMode(SystemColorMode.System);

    var host = new HostingForm();
    host.AddComponent(new TestWidget());
    host.AddComponent(new TestWidget2());

    SetupTray();
    Application.Run(host);
  }

  static void SetupTray()
  {
    var tray = new NotifyIcon
    {
      Visible = true,
      Icon = SystemIcons.Application,
      Text = "Iris"
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