using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage.Streams;

namespace DesktopPet.Services;

// Windows 내장 OCR(Windows.Media.Ocr)을 사용한다. 무료·오프라인이지만
// 사용자 프로필에 해당 언어의 OCR 언어팩이 설치되어 있어야 인식된다.
public static class ScreenOcrService
{
    public static async Task<string?> RecognizeRegionAsync(CaptureRegionSettings region)
    {
        byte[] pngBytes = await Task.Run(() => CaptureRegionAsPng(region));

        using var winrtStream = new InMemoryRandomAccessStream();
        using (var writer = new DataWriter(winrtStream))
        {
            writer.WriteBytes(pngBytes);
            await writer.StoreAsync();
            await writer.FlushAsync();
            writer.DetachStream();
        }

        winrtStream.Seek(0);

        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(winrtStream);
        SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();

        OcrEngine? engine = OcrEngine.TryCreateFromUserProfileLanguages();
        if (engine is null)
        {
            return null;
        }

        OcrResult result = await engine.RecognizeAsync(softwareBitmap);
        return result.Text;
    }

    private static byte[] CaptureRegionAsPng(CaptureRegionSettings region)
    {
        using var bitmap = new Bitmap(region.Width, region.Height);
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            g.CopyFromScreen(region.X, region.Y, 0, 0, bitmap.Size);
        }

        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        return stream.ToArray();
    }
}
