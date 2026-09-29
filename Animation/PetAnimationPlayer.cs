namespace DesktopPet.Animation;

public sealed class PetAnimationPlayer
{
    private string? _clipName;
    private PetClipDefinition? _clip;
    private double _elapsedMs;

    public int FrameIndex { get; private set; }

    public void SetClip(string clipName, PetClipDefinition clip)
    {
        if (_clipName == clipName)
        {
            return;
        }

        _clipName = clipName;
        _clip = clip;
        FrameIndex = 0;
        _elapsedMs = 0;
    }

    public void Tick(double elapsedMs)
    {
        if (_clip is null)
        {
            return;
        }

        _elapsedMs += elapsedMs;
        while (_elapsedMs >= _clip.FrameDurationMs)
        {
            _elapsedMs -= _clip.FrameDurationMs;
            FrameIndex++;
            if (FrameIndex >= _clip.FrameCount)
            {
                FrameIndex = _clip.Loop ? 0 : _clip.FrameCount - 1;
            }
        }
    }
}
