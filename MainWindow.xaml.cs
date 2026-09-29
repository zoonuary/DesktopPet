using System.IO;
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

namespace DesktopPet;

public partial class MainWindow : Window
{
    private const double MaxElapsedSeconds = 0.1;

    private readonly PetWanderMovement _wander = new();
    private readonly PetBehaviorController _behavior = new();
    private readonly PetAnimationPlayer _animation = new();
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

        _wanderTimer = new DispatcherTimer(DispatcherPriority.Normal)
        {
            Interval = TimeSpan.FromMilliseconds(16)
        };
        _wanderTimer.Tick += WanderTimer_Tick;

        _trayIcon = new TrayIconService(
            _behavior.Mode,
            mode =>
            {
                _behavior.SetMode(mode);
                RefreshVisual();
            },
            () => System.Windows.Application.Current.Shutdown());

        Loaded += MainWindow_Loaded;
        Closed += MainWindow_Closed;
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
        Rect workArea = SystemParameters.WorkArea;
        if (_skin is not null)
        {
            double anchorRatio = (double)_skin.Definition.AnchorYPx / _skin.Definition.FrameHeightPx;
            Top = workArea.Bottom - (anchorRatio * Height);
        }
        else
        {
            Top = workArea.Bottom - Height;
        }

        _lastTick = DateTime.UtcNow;
        _wanderTimer.Start();
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

        PetStateId previousState = _behavior.State;
        _behavior.Tick(elapsedSeconds);

        if (_behavior.State == PetStateId.Walk)
        {
            int directionX = _behavior.Facing == PetFacing.Left ? -1 : 1;
            Rect workArea = SystemParameters.WorkArea;
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
        }
    }
}
