using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace QrMenu.Infrastructure.Wolvox;

public class WolvoxSdkOptions
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 3056;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;  // plain, MD5'e çevireceğiz
    public string DevCode { get; set; } = string.Empty;
    public string DevPass { get; set; } = string.Empty;
}

public class WolvoxSdkService
{
    private readonly WolvoxSdkOptions _opts;
    private readonly HttpClient _http;
    private string? _tPwd;

    public WolvoxSdkService(WolvoxSdkOptions opts, HttpClient http)
    {
        _opts = opts;
        _http = http;
    }

    // ----- HELPERS -----

    private string BaseUrl => $"http://{_opts.Host}:{_opts.Port}/getdata.html";

    private static string ToMd5(string input)
    {
        var bytes = MD5.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToUpper();
    }

    private static string ToBase64(string input)
        => Convert.ToBase64String(Encoding.UTF8.GetBytes(input));

    private static string FromBase64(string input)
        => Encoding.UTF8.GetString(Convert.FromBase64String(input));

    private async Task<string> GetRaw(string plainParams)
    {
        var encoded = ToBase64(plainParams);
        var url = $"{BaseUrl}?{encoded}";
        var response = await _http.GetStringAsync(url);
        return FromBase64(response.Trim());
    }

    // ----- LOGIN / LOGOUT -----

    public async Task<bool> LoginAsync()
    {
        var md5Pass = ToMd5(_opts.Password);
        var plain = $"command=wlogin&username={_opts.Username}&password={md5Pass}" +
                    $"&devCode={_opts.DevCode}&devPass={_opts.DevPass}&timeOut=60";

        var result = await GetRaw(plain);
        // Dönüş: "1&TEMP_PASSWORD" veya "0&Hata mesajı"
        var parts = result.Split('&', 2);
        if (parts[0] == "1")
        {
            _tPwd = parts[1];
            return true;
        }
        return false;
    }

    public async Task LogoutAsync()
    {
        if (_tPwd == null) return;
        await GetRaw($"command=wlogout&tpwd={_tPwd}");
        _tPwd = null;
    }

    // ----- VERİ OKUMA -----

    public async Task<XDocument> GetSirketListeAsync()
    {
        var plain = $"command=get_sirketliste&tPwd={_tPwd}";
        var xml = await GetRaw(plain);
        return XDocument.Parse(xml);
    }

    public async Task<XDocument> GetStokListeAsync(string sirketKodu, string calismaYili, string ekSart = "", string fieldList = "")
    {
        var plain = $"command=get_stoklist&tPwd={_tPwd}" +
                    $"&sirketKodu={sirketKodu}&calismaYili={calismaYili}" +
                    $"&ekSart={ekSart}&fieldList={fieldList}";
        var xml = await GetRaw(plain);
        return XDocument.Parse(xml);
    }

    // ----- KULLANIM KOLAYLAŞTIRICI -----

    // Login → işlem → logout döngüsü
    public async Task<T> ExecuteAsync<T>(Func<WolvoxSdkService, Task<T>> action)
    {
        var ok = await LoginAsync();
        if (!ok) throw new Exception("Wolvox SDK login başarısız.");
        try
        {
            return await action(this);
        }
        finally
        {
            await LogoutAsync();
        }
    }
}