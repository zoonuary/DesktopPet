using System.Drawing;
using System.Windows.Forms;

namespace DesktopPet;

public sealed class TrayIconService : IDisposable
{
    private readonly NotifyIcon _notifyIcon;

    public TrayIconService(Action onExitRequested)
    {
        var exitItem = new ToolStripMenuItem("종료");
        exitItem.Click += (_, _) => onExitRequested();

        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add(exitItem);

        _notifyIcon = new NotifyIcon
        {
            Icon = SystemIcons.Application,
            Visible = true,
            Text = "DesktopPet",
            ContextMenuStrip = contextMenu
        };
    }

    public void Dispose()
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
    }
}
