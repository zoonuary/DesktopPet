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

namespace DesktopPet;

public partial class MainWindow : Window
{
    private readonly PetWanderMovement _wander = new();
    private readonly DispatcherTimer _wanderTimer;
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

        Loaded += MainWindow_Loaded;
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
        double elapsedSeconds = (now - _lastTick).TotalSeconds;
        _lastTick = now;

        if (_isDragging)
        {
            return;
        }

        Rect workArea = SystemParameters.WorkArea;
        Left = _wander.GetNextLeft(Left, Width, elapsedSeconds, workArea.Left, workArea.Right);
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isDragging = true;
        try
        {
            DragMove();
        }
        finally
        {
            _isDragging = false;
        }
    }
}
