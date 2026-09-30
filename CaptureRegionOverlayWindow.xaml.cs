using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DesktopPet.Services;

namespace DesktopPet;

public partial class CaptureRegionOverlayWindow : Window
{
    private readonly Action<CaptureRegionSettings> _onRegionSelected;
    private readonly int _originPixelX;
    private readonly int _originPixelY;
    private bool _isSelecting;
    private System.Drawing.Point _startPixel;

    public CaptureRegionOverlayWindow(Action<CaptureRegionSettings> onRegionSelected)
    {
        InitializeComponent();
        _onRegionSelected = onRegionSelected;

        System.Drawing.Rectangle virtualScreen = System.Windows.Forms.SystemInformation.VirtualScreen;
        _originPixelX = virtualScreen.Left;
        _originPixelY = virtualScreen.Top;

        Loaded += CaptureRegionOverlayWindow_Loaded;
    }

    private void CaptureRegionOverlayWindow_Loaded(object sender, RoutedEventArgs e)
    {
        System.Drawing.Rectangle virtualScreen = System.Windows.Forms.SystemInformation.VirtualScreen;
        DpiScale dpi = VisualTreeHelper.GetDpi(this);

        Left = virtualScreen.Left / dpi.DpiScaleX;
        Top = virtualScreen.Top / dpi.DpiScaleY;
        Width = virtualScreen.Width / dpi.DpiScaleX;
        Height = virtualScreen.Height / dpi.DpiScaleY;

        Focus();
    }

    private void Overlay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isSelecting = true;
        _startPixel = System.Windows.Forms.Cursor.Position;
        SelectionRectangle.Visibility = Visibility.Visible;
        UpdateSelectionVisual(_startPixel, _startPixel);
        CaptureMouse();
    }

    private void Overlay_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (!_isSelecting)
        {
            return;
        }

        UpdateSelectionVisual(_startPixel, System.Windows.Forms.Cursor.Position);
    }

    private void Overlay_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!_isSelecting)
        {
            return;
        }

        _isSelecting = false;
        ReleaseMouseCapture();

        System.Drawing.Point endPixel = System.Windows.Forms.Cursor.Position;
        int x = Math.Min(_startPixel.X, endPixel.X);
        int y = Math.Min(_startPixel.Y, endPixel.Y);
        int width = Math.Abs(endPixel.X - _startPixel.X);
        int height = Math.Abs(endPixel.Y - _startPixel.Y);

        Close();

        if (width >= 4 && height >= 4)
        {
            _onRegionSelected(new CaptureRegionSettings { X = x, Y = y, Width = width, Height = height });
        }
    }

    private void Overlay_MouseRightButtonDown(object sender, MouseButtonEventArgs e) => Close();

    private void Overlay_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }

    private void UpdateSelectionVisual(System.Drawing.Point startPixel, System.Drawing.Point currentPixel)
    {
        System.Windows.Point start = ToLocalPoint(startPixel);
        System.Windows.Point current = ToLocalPoint(currentPixel);

        double left = Math.Min(start.X, current.X);
        double top = Math.Min(start.Y, current.Y);

        Canvas.SetLeft(SelectionRectangle, left);
        Canvas.SetTop(SelectionRectangle, top);
        SelectionRectangle.Width = Math.Abs(current.X - start.X);
        SelectionRectangle.Height = Math.Abs(current.Y - start.Y);
    }

    private System.Windows.Point ToLocalPoint(System.Drawing.Point pixel)
    {
        DpiScale dpi = VisualTreeHelper.GetDpi(this);
        return new System.Windows.Point((pixel.X - _originPixelX) / dpi.DpiScaleX, (pixel.Y - _originPixelY) / dpi.DpiScaleY);
    }
}
