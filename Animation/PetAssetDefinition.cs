using System.Windows.Media.Imaging;

namespace DesktopPet.Animation;

public sealed record PetClipDefinition(string File, int FrameCount, int FrameDurationMs, bool Loop);

public sealed record PetCharacterDefinition(
    int SchemaVersion,
    string Id,
    int FrameWidthPx,
    int FrameHeightPx,
    int AnchorXPx,
    int AnchorYPx,
    string DefaultFacing,
    bool AllowMirror,
    IReadOnlyDictionary<string, PetClipDefinition> Clips);

public sealed record PetSkin(PetCharacterDefinition Definition, IReadOnlyDictionary<string, BitmapSource> ClipImages);
