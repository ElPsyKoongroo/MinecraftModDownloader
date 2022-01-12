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

    static int downloadProgressPerc = 0;

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

    public static async Task download(String url, string fileName, IProgress<int> progress){

        HttpResponseMessage chunkResponse;
        byte[] chunkContent;

        long chunk = 102400L;

        byte[] chunks = { };

        long totalSize = await GetSize(url);

        long totalChunks = (totalSize / chunk); // 100/100 

        long remanent = (totalSize % chunk);

        if (remanent != 0) totalChunks++;

        for (int i = 0; i < totalChunks; i++)
        {
            HttpRequestMessage msg = new();
            msg.RequestUri = new Uri(url);
            long actualChunk = (i == totalChunks - 1) ? remanent : chunk;
            msg.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(i * chunk, i * chunk + actualChunk - 1);
            chunkResponse = await client.SendAsync(msg);
            chunkContent = await chunkResponse.Content.ReadAsByteArrayAsync();
            chunks = chunks.Concat(chunkContent).ToArray();
            progress.Report((int)((chunk*(i)+actualChunk)*100 / totalSize)); 
        }
        await File.WriteAllBytesAsync($"{MainWindow.downloadPath}/{fileName}", chunks);
    }

    public static async Task<long> GetSize(string url)
    {
        long size;
        HttpRequestMessage msg = new();
        msg.RequestUri = new Uri(url);
        msg.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(0, 0);
        var response = await client.SendAsync(msg);
        string contentRange = response.Content.Headers.GetValues(@"Content-Range").Single();
        string lengthString = System.Text.RegularExpressions.Regex.Match(contentRange, @"(?<=^bytes\s[0-9]+\-[0-9]+/)[0-9]+$").Value;
        size = long.Parse(lengthString);
        return size;
    }
}

