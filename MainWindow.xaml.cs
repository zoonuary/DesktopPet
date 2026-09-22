using System.Text;
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
using DesktopPet.Behavior;

namespace DesktopPet;

public partial class MainWindow : Window
{
    private const double MaxElapsedSeconds = 0.1;

    private readonly PetWanderMovement _wander = new();
    private readonly PetBehaviorController _behavior = new();
    private readonly DispatcherTimer _wanderTimer;
    private readonly TrayIconService _trayIcon;
    private DateTime _lastTick;
    private bool _isDragging;

    public MainWindow()
    {
        InitializeComponent();

        _wanderTimer = new DispatcherTimer(DispatcherPriority.Render)
        {
            Interval = TimeSpan.FromMilliseconds(16)
        };
        _wanderTimer.Tick += WanderTimer_Tick;

        _trayIcon = new TrayIconService(() => System.Windows.Application.Current.Shutdown());

        Loaded += MainWindow_Loaded;
        Closed += MainWindow_Closed;
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        _wanderTimer.Stop();
        _trayIcon.Dispose();
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        Top = SystemParameters.WorkArea.Bottom - Height;

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

        if (_behavior.State != previousState)
        {
            UpdatePlaceholderColor();
        }
    }

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
        UpdatePlaceholderColor();
        try
        {
            DragMove();
        }
        finally
        {
            _isDragging = false;
            _behavior.ExitDrag();
            UpdatePlaceholderColor();
        }
    }
}
