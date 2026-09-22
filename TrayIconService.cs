using System.Drawing;
using System.Windows.Forms;
using DesktopPet.Behavior;

namespace DesktopPet;

public sealed class TrayIconService : IDisposable
{
    private readonly NotifyIcon _notifyIcon;
    private readonly ToolStripMenuItem _normalModeItem;
    private readonly ToolStripMenuItem _stayModeItem;
    private readonly ToolStripMenuItem _focusModeItem;

    public TrayIconService(PetMode initialMode, Action<PetMode> onModeSelected, Action onExitRequested)
    {
        _normalModeItem = CreateModeItem("일반 모드", PetMode.Normal, onModeSelected);
        _stayModeItem = CreateModeItem("여기서 쉬기", PetMode.Stay, onModeSelected);
        _focusModeItem = CreateModeItem("집중 모드", PetMode.Focus, onModeSelected);

        var exitItem = new ToolStripMenuItem("종료");
        exitItem.Click += (_, _) => onExitRequested();

        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add(_normalModeItem);
        contextMenu.Items.Add(_stayModeItem);
        contextMenu.Items.Add(_focusModeItem);
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add(exitItem);

        SetCheckedMode(initialMode);

        _notifyIcon = new NotifyIcon
        {
            Icon = SystemIcons.Application,
            Visible = true,
            Text = "DesktopPet",
            ContextMenuStrip = contextMenu
        };
    }

    private ToolStripMenuItem CreateModeItem(string text, PetMode mode, Action<PetMode> onModeSelected)
    {
        var item = new ToolStripMenuItem(text);
        item.Click += (_, _) =>
        {
            SetCheckedMode(mode);
            onModeSelected(mode);
        };
        return item;
    }

    private void SetCheckedMode(PetMode mode)
    {
        _normalModeItem.Checked = mode == PetMode.Normal;
        _stayModeItem.Checked = mode == PetMode.Stay;
        _focusModeItem.Checked = mode == PetMode.Focus;
    }

    public void Dispose()
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
    }
}
