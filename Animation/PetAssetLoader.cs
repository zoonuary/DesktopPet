using System.IO;
using System.Text.Json;
using System.Windows.Media.Imaging;

namespace DesktopPet.Animation;

public sealed class PetAssetLoadException : Exception
{
    public PetAssetLoadException(string message) : base(message)
    {
    }
}

public static class PetAssetLoader
{
    private static readonly string[] RequiredClips = { "idle", "walk", "rest", "sleep", "react", "drag" };

    public static PetSkin LoadSkin(string skinDirectory)
    {
        PetCharacterDefinition definition = LoadDefinition(skinDirectory);
        var images = new Dictionary<string, BitmapSource>();

        foreach ((string name, PetClipDefinition clip) in definition.Clips)
        {
            string fullPath = Path.Combine(skinDirectory, clip.File);
            BitmapImage bitmap = LoadBitmap(fullPath);

            int expectedWidth = clip.FrameCount * definition.FrameWidthPx;
            if (bitmap.PixelWidth != expectedWidth || bitmap.PixelHeight != definition.FrameHeightPx)
            {
                throw new PetAssetLoadException(
                    $"Clip '{name}' size {bitmap.PixelWidth}x{bitmap.PixelHeight} does not match expected {expectedWidth}x{definition.FrameHeightPx}");
            }

            images[name] = bitmap;
        }

        return new PetSkin(definition, images);
    }

    private static PetCharacterDefinition LoadDefinition(string skinDirectory)
    {
        string jsonPath = Path.Combine(skinDirectory, "character.json");
        if (!File.Exists(jsonPath))
        {
            throw new PetAssetLoadException($"character.json not found: {jsonPath}");
        }

        using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(jsonPath));
        JsonElement root = doc.RootElement;

        int schemaVersion = root.GetProperty("schemaVersion").GetInt32();
        if (schemaVersion != 1)
        {
            throw new PetAssetLoadException($"Unsupported schemaVersion: {schemaVersion}");
        }

        string id = root.GetProperty("id").GetString()
            ?? throw new PetAssetLoadException("Missing id");

        JsonElement frameSize = root.GetProperty("frameSizePx");
        int frameWidth = frameSize.GetProperty("width").GetInt32();
        int frameHeight = frameSize.GetProperty("height").GetInt32();
        if (frameWidth <= 0 || frameHeight <= 0)
        {
            throw new PetAssetLoadException("frameSizePx must be positive");
        }

        JsonElement anchor = root.GetProperty("anchorPx");
        int anchorX = anchor.GetProperty("x").GetInt32();
        int anchorY = anchor.GetProperty("y").GetInt32();

        string defaultFacing = root.GetProperty("defaultFacing").GetString() ?? "right";
        bool allowMirror = root.TryGetProperty("allowMirror", out JsonElement mirrorEl) && mirrorEl.GetBoolean();

        string skinRoot = Path.GetFullPath(skinDirectory);
        var clips = new Dictionary<string, PetClipDefinition>();

        foreach (JsonProperty clipProp in root.GetProperty("clips").EnumerateObject())
        {
            JsonElement c = clipProp.Value;
            string file = c.GetProperty("file").GetString()
                ?? throw new PetAssetLoadException($"Clip '{clipProp.Name}' missing file");
            int frameCount = c.GetProperty("frameCount").GetInt32();
            int frameDurationMs = c.GetProperty("frameDurationMs").GetInt32();
            bool loop = c.GetProperty("loop").GetBoolean();

            if (frameCount <= 0 || frameDurationMs <= 0)
            {
                throw new PetAssetLoadException($"Clip '{clipProp.Name}' has invalid frameCount/frameDurationMs");
            }

            string fullPath = Path.GetFullPath(Path.Combine(skinDirectory, file));
            if (!fullPath.StartsWith(skinRoot, StringComparison.OrdinalIgnoreCase))
            {
                throw new PetAssetLoadException($"Clip '{clipProp.Name}' file escapes skin directory: {file}");
            }

            if (!File.Exists(fullPath))
            {
                throw new PetAssetLoadException($"Clip '{clipProp.Name}' file not found: {fullPath}");
            }

            clips[clipProp.Name] = new PetClipDefinition(file, frameCount, frameDurationMs, loop);
        }

        foreach (string required in RequiredClips)
        {
            if (!clips.ContainsKey(required))
            {
                throw new PetAssetLoadException($"Missing required clip: {required}");
            }
        }

        return new PetCharacterDefinition(schemaVersion, id, frameWidth, frameHeight, anchorX, anchorY, defaultFacing, allowMirror, clips);
    }

    private static BitmapImage LoadBitmap(string path)
    {
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.UriSource = new Uri(path, UriKind.Absolute);
        bitmap.EndInit();
        bitmap.Freeze();
        return bitmap;
    }
}
