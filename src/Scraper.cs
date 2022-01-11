using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;

namespace webScrapWPF;
public class Scraper
{


    public static HttpClient client = new(new HttpClientHandler()
    {
        Proxy = null,
        UseCookies = false,
        UseProxy = false,
        UseDefaultCredentials = true,
        AllowAutoRedirect = true,
    });

    public static void setHeaders()
    {
        client.DefaultRequestHeaders.Add("Authorization", "Bearer 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");
    }

    public static async Task<String> get(String url)
    {
        HttpResponseMessage response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            return  (await response.Content.ReadAsStringAsync()).ReplaceLineEndings(" "); ;
        }
        return String.Empty;
    }

    public static async Task<bool> download(String url, string fileName){

        HttpResponseMessage response = await client.GetAsync(url);

        string path = $"{MainWindow.downloadPath}/{fileName}";

        if (response.IsSuccessStatusCode)
        {
            File.WriteAllBytes(path, await response.Content.ReadAsByteArrayAsync());
            return true;
        }
        return false;
    }
}

