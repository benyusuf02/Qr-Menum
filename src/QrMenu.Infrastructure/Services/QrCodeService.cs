using QRCoder;

namespace QrMenu.Infrastructure.Services;

public class QrCodeService
{
    private readonly string _webRootPath;

    public QrCodeService(string webRootPath)
    {
        _webRootPath = webRootPath;
    }

    public string Generate(string slug, string baseUrl)
    {
        var menuUrl = $"{baseUrl}/m/{slug}";

        var qrGenerator = new QRCodeGenerator();
        var qrData = qrGenerator.CreateQrCode(menuUrl, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrData);
        var pngBytes = qrCode.GetGraphic(10);

        // wwwroot/qr klasörüne kaydet
        var qrFolder = Path.Combine(_webRootPath, "qr");
        Directory.CreateDirectory(qrFolder);

        var fileName = $"{slug}.png";
        var filePath = Path.Combine(qrFolder, fileName);
        File.WriteAllBytes(filePath, pngBytes);

        return $"/qr/{fileName}";
    }
}