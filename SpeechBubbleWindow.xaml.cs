using System.Windows;

namespace DesktopPet;

public partial class SpeechBubbleWindow : Window
{
    public SpeechBubbleWindow()
    {
        InitializeComponent();
    }

    public void ShowText(string text, double anchorLeft, double anchorTop, double anchorWidth)
    {
        BubbleText.Text = text;

        if (!IsVisible)
        {
            Show();
        }

        UpdateLayout();

        Left = anchorLeft + (anchorWidth / 2.0) - (ActualWidth / 2.0);
        Top = anchorTop - ActualHeight - 8;
    }

    public void HideBubble()
    {
        if (IsVisible)
        {
            Hide();
        }
    }
}
