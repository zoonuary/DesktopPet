using System.IO;
using System.Text.Json;

namespace DesktopPet.Services;

public sealed class PetSettings
{
    public int SchemaVersion { get; set; } = 1;
    public string CharacterId { get; set; } = "default";
    public double DisplaySizeDip { get; set; } = 160;
    public bool AlwaysOnTop { get; set; } = true;
    public string Mode { get; set; } = "Normal";
    public double? WindowLeft { get; set; }
    public double? WindowTop { get; set; }
    public CaptureRegionSettings? CaptureRegion { get; set; }
}

public sealed class CaptureRegionSettings
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

// 첫 CFG 구현. DESKTOP_PET_BEHAVIOR_SPEC.md 6장의 계약을 따르되,
// "변경을 모아 순차 저장"은 저장이 드문 이산 이벤트(드래그 종료, 모드 변경)에서만
// 일어나는 지금 규모에는 과할 것 같아 lock으로 동시 저장만 직렬화한다.
//
// 설치 프로그램 없이 exe를 폴더째 복사/삭제하는 포터블 배포라, 삭제 시 흔적이
// 안 남도록 %AppData%가 아니라 exe 옆에 저장한다. 나중에 Program Files처럼
// 쓰기 권한이 없는 위치에 설치하는 정식 설치 프로그램을 만들면 재검토해야 한다.
public sealed class PetSettingsStore
{
    private readonly string _filePath;
    private readonly object _saveLock = new();

    public PetSettingsStore()
    {
        _filePath = Path.Combine(AppContext.BaseDirectory, "settings.json");
    }

    public PetSettings Load()
    {
        if (!File.Exists(_filePath))
        {
            return new PetSettings();
        }

        try
        {
            string json = File.ReadAllText(_filePath);
            PetSettings? settings = JsonSerializer.Deserialize<PetSettings>(json);
            if (settings is null || settings.SchemaVersion != 1)
            {
                return new PetSettings();
            }

            return Sanitize(settings);
        }
        catch (Exception ex) when (ex is IOException or JsonException)
        {
            System.Diagnostics.Debug.WriteLine($"Settings load failed, using defaults: {ex.Message}");
            return new PetSettings();
        }
    }

    public void Save(PetSettings settings)
    {
        lock (_saveLock)
        {
            try
            {
                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                string tempPath = _filePath + ".tmp";
                File.WriteAllText(tempPath, json);
                File.Move(tempPath, _filePath, overwrite: true);
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Settings save failed: {ex.Message}");
            }
        }
    }

    private static PetSettings Sanitize(PetSettings settings)
    {
        if (settings.DisplaySizeDip is < 80 or > 240)
        {
            settings.DisplaySizeDip = 160;
        }

        if (settings.Mode is not ("Normal" or "Stay" or "Focus"))
        {
            settings.Mode = "Normal";
        }

        if (string.IsNullOrWhiteSpace(settings.CharacterId))
        {
            settings.CharacterId = "default";
        }

        return settings;
    }
}
