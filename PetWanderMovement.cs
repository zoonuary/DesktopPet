namespace DesktopPet;

public sealed class PetWanderMovement
{
    private const double SpeedPixelsPerSecond = 60.0;

    private int _directionX = 1;

    public double GetNextLeft(double currentLeft, double windowWidth, double elapsedSeconds, double screenLeft, double screenRight)
    {
        double nextLeft = currentLeft + (_directionX * SpeedPixelsPerSecond * elapsedSeconds);

        if (nextLeft <= screenLeft)
        {
            nextLeft = screenLeft;
            _directionX = 1;
        }
        else if (nextLeft + windowWidth >= screenRight)
        {
            nextLeft = screenRight - windowWidth;
            _directionX = -1;
        }

        return nextLeft;
    }
}
