using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using DesktopPet.Animation;
using DesktopPet.Behavior;
using DesktopPet.Services;

namespace DesktopPet;

public partial class MainWindow : Window
{
    private const double MaxElapsedSeconds = 0.1;

    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_TOOLWINDOW = 0x00000080;
    private const int WS_EX_APPWINDOW = 0x00040000;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    private readonly PetWanderMovement _wander = new();
    private readonly PetBehaviorController _behavior = new();
    private readonly PetAnimationPlayer _animation = new();
    private readonly PetSettingsStore _settingsStore = new();
    private readonly PetSettings _settings;
    private readonly DispatcherTimer _wanderTimer;
    private readonly TrayIconService _trayIcon;
    private PetSkin? _skin;
    private string? _currentClipName;
    private int _currentFrameIndex = -1;
    private DateTime _lastTick;
    private bool _isDragging;

    public MainWindow()
    {
        InitializeComponent();

        LoadSkin();

        _settings = _settingsStore.Load();
        _behavior.SetMode(ParseMode(_settings.Mode));

        _wanderTimer = new DispatcherTimer(DispatcherPriority.Normal)
        {
            Interval = TimeSpan.FromMilliseconds(16)
        };
        _wanderTimer.Tick += WanderTimer_Tick;

        _trayIcon = new TrayIconService(
            _behavior.Mode,
            ApplyModeSelection,
            ShowCaptureRegionOverlay,
            () => System.Windows.Application.Current.Shutdown());

        UpdateModeMenuChecks();
        RefreshVisual();

        SourceInitialized += MainWindow_SourceInitialized;
        Loaded += MainWindow_Loaded;
        Closed += MainWindow_Closed;
    }

    private void ApplyModeSelection(PetMode mode)
    {
        _behavior.SetMode(mode);
        RefreshVisual();
        UpdateModeMenuChecks();
        _trayIcon.SyncMode(mode);

        _settings.Mode = mode.ToString();
        _settingsStore.Save(_settings);
    }

    private static PetMode ParseMode(string mode) => Enum.TryParse(mode, out PetMode parsed) ? parsed : PetMode.Normal;

    private void UpdateModeMenuChecks()
    {
        NormalModeMenuItem.IsChecked = _behavior.Mode == PetMode.Normal;
        StayModeMenuItem.IsChecked = _behavior.Mode == PetMode.Stay;
        FocusModeMenuItem.IsChecked = _behavior.Mode == PetMode.Focus;
    }

    private void NormalModeMenuItem_Click(object sender, RoutedEventArgs e) => ApplyModeSelection(PetMode.Normal);

    private void StayModeMenuItem_Click(object sender, RoutedEventArgs e) => ApplyModeSelection(PetMode.Stay);

    private void FocusModeMenuItem_Click(object sender, RoutedEventArgs e) => ApplyModeSelection(PetMode.Focus);

    private void ExitMenuItem_Click(object sender, RoutedEventArgs e) => System.Windows.Application.Current.Shutdown();

    private void CaptureRegionMenuItem_Click(object sender, RoutedEventArgs e) => ShowCaptureRegionOverlay();

    private void ShowCaptureRegionOverlay()
    {
        var overlay = new CaptureRegionOverlayWindow(OnCaptureRegionSelected);
        overlay.Show();
    }

    private void OnCaptureRegionSelected(CaptureRegionSettings region)
    {
        _settings.CaptureRegion = region;
        _settingsStore.Save(_settings);
    }

    private void MainWindow_SourceInitialized(object? sender, EventArgs e)
    {
        IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
        int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
        exStyle |= WS_EX_TOOLWINDOW;
        exStyle &= ~WS_EX_APPWINDOW;
        SetWindowLong(hwnd, GWL_EXSTYLE, exStyle);
    }

    private void LoadSkin()
    {
        try
        {
            string skinDirectory = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "Pets", "default");
            _skin = PetAssetLoader.LoadSkin(skinDirectory);
            PetPlaceholder.Visibility = Visibility.Collapsed;
            PetImage.Visibility = Visibility.Visible;
        }
        catch (Exception ex) when (ex is PetAssetLoadException or IOException or JsonException)
        {
            System.Diagnostics.Debug.WriteLine($"Pet skin load failed, falling back to placeholder: {ex.Message}");
            _skin = null;
        }
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        _wanderTimer.Stop();
        _trayIcon.Dispose();
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (_settings.WindowLeft is double savedLeft && _settings.WindowTop is double savedTop && IsPositionOnScreen(savedLeft, savedTop))
        {
            Left = savedLeft;
            Top = savedTop;
        }
        else
        {
            Rect workArea = GetCurrentWorkArea();
            Left = workArea.Left + ((workArea.Width - Width) / 2.0);

            if (_skin is not null)
            {
                double anchorRatio = (double)_skin.Definition.AnchorYPx / _skin.Definition.FrameHeightPx;
                Top = workArea.Bottom - (anchorRatio * Height);
            }
            else
            {
                Top = workArea.Bottom - Height;
            }
        }

        _lastTick = DateTime.UtcNow;
        _wanderTimer.Start();
    }

    // 정밀한 모니터별 DPI 변환 대신, 전체 가상 화면 범위 안에 있는지만 대략 검사한다.
    // 모니터가 사라졌거나 좌표가 화면 밖이면 false를 돌려줘 기본 배치로 복구시킨다.
    private bool IsPositionOnScreen(double left, double top)
    {
        DpiScale dpi = VisualTreeHelper.GetDpi(this);
        System.Drawing.Rectangle virtualScreen = System.Windows.Forms.SystemInformation.VirtualScreen;

        double vLeft = virtualScreen.Left / dpi.DpiScaleX;
        double vTop = virtualScreen.Top / dpi.DpiScaleY;
        double vRight = virtualScreen.Right / dpi.DpiScaleX;
        double vBottom = virtualScreen.Bottom / dpi.DpiScaleY;

        return left >= vLeft && left + Width <= vRight && top >= vTop && top + Height <= vBottom;
    }

    private void WanderTimer_Tick(object? sender, EventArgs e)
    {
        DateTime now = DateTime.UtcNow;
        double elapsedSeconds = Math.Min((now - _lastTick).TotalSeconds, MaxElapsedSeconds);
        _lastTick = now;

        if (_isDragging)
        {
            return;
        }

        Rect workArea = GetCurrentWorkArea();
        bool nearLeftEdge = Left <= workArea.Left;
        bool nearRightEdge = Left + Width >= workArea.Right;

        PetStateId previousState = _behavior.State;
        _behavior.Tick(elapsedSeconds, nearLeftEdge, nearRightEdge);

        if (_behavior.State == PetStateId.Walk)
        {
            int directionX = _behavior.Facing == PetFacing.Left ? -1 : 1;
            (double nextLeft, bool hitBoundary) = _wander.GetNextLeft(Left, directionX, Width, elapsedSeconds, workArea.Left, workArea.Right);
            Left = nextLeft;

            if (hitBoundary)
            {
                _behavior.NotifyBoundaryHit();
            }
        }

        if (_skin is not null)
        {
            UpdateAnimationFrame(elapsedSeconds * 1000.0);
        }
        else if (_behavior.State != previousState)
        {
            UpdatePlaceholderColor();
        }
    }

    private Rect GetCurrentWorkArea()
    {
        var interopHelper = new System.Windows.Interop.WindowInteropHelper(this);
        System.Windows.Forms.Screen? screen = interopHelper.Handle != IntPtr.Zero
            ? System.Windows.Forms.Screen.FromHandle(interopHelper.Handle)
            : System.Windows.Forms.Screen.PrimaryScreen;

        if (screen is null)
        {
            return SystemParameters.WorkArea;
        }

        DpiScale dpi = VisualTreeHelper.GetDpi(this);
        System.Drawing.Rectangle wa = screen.WorkingArea;

        return new Rect(
            wa.Left / dpi.DpiScaleX,
            wa.Top / dpi.DpiScaleY,
            wa.Width / dpi.DpiScaleX,
            wa.Height / dpi.DpiScaleY);
    }

    private void RefreshVisual()
    {
        if (_skin is not null)
        {
            UpdateAnimationFrame(0);
        }
        else
        {
            UpdatePlaceholderColor();
        }
    }

    private void UpdateAnimationFrame(double elapsedMs)
    {
        if (_skin is null)
        {
            return;
        }

        string clipName = ClipNameFor(_behavior.State);
        if (!_skin.Definition.Clips.TryGetValue(clipName, out PetClipDefinition? clip))
        {
            return;
        }

        _animation.SetClip(clipName, clip);
        _animation.Tick(elapsedMs);

        if (clipName != _currentClipName || _animation.FrameIndex != _currentFrameIndex)
        {
            _currentClipName = clipName;
            _currentFrameIndex = _animation.FrameIndex;

            BitmapSource sheet = _skin.ClipImages[clipName];
            int frameWidth = _skin.Definition.FrameWidthPx;
            int frameHeight = _skin.Definition.FrameHeightPx;
            var sourceRect = new Int32Rect(_currentFrameIndex * frameWidth, 0, frameWidth, frameHeight);
            PetImage.Source = new CroppedBitmap(sheet, sourceRect);
        }

        if (_skin.Definition.AllowMirror)
        {
            PetImageFlip.ScaleX = _behavior.Facing == PetFacing.Left ? -1 : 1;
        }
    }

    private static string ClipNameFor(PetStateId state) => state switch
    {
        PetStateId.Idle => "idle",
        PetStateId.Walk => "walk",
        PetStateId.Rest => "rest",
        PetStateId.Sleep => "sleep",
        PetStateId.React => "react",
        PetStateId.Drag => "drag",
        _ => "idle"
    };

    private void UpdatePlaceholderColor()
    {
        PetPlaceholder.Fill = _behavior.State switch
        {
            PetStateId.Idle => System.Windows.Media.Brushes.CornflowerBlue,
            PetStateId.Walk => System.Windows.Media.Brushes.LimeGreen,
            PetStateId.Rest => System.Windows.Media.Brushes.Orange,
            PetStateId.Sleep => System.Windows.Media.Brushes.Gray,
            PetStateId.Drag => System.Windows.Media.Brushes.MediumPurple,
            _ => System.Windows.Media.Brushes.CornflowerBlue
        };
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isDragging = true;
        _behavior.EnterDrag();
        RefreshVisual();
        try
        {
            DragMove();
        }
        finally
        {
            _isDragging = false;
            _behavior.ExitDrag();
            RefreshVisual();

            _settings.WindowLeft = Left;
            _settings.WindowTop = Top;
            _settingsStore.Save(_settings);
        }
    }
}
