namespace DesktopPet;

public sealed class PetWanderMovement
{
    private const double SpeedDipPerSecond = 60.0;

    public (double Left, bool HitBoundary) GetNextLeft(double currentLeft, int directionX, double windowWidth, double elapsedSeconds, double screenLeft, double screenRight)
    {
        double nextLeft = currentLeft + (directionX * SpeedDipPerSecond * elapsedSeconds);
        bool hitBoundary = false;

        if (nextLeft <= screenLeft)
        {
            nextLeft = screenLeft;
            hitBoundary = true;
        }
        else if (nextLeft + windowWidth >= screenRight)
        {
            nextLeft = screenRight - windowWidth;
            hitBoundary = true;
        }

        return (nextLeft, hitBoundary);
    }
}
