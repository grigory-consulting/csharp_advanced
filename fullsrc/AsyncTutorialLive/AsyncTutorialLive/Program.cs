
using HtmlAgilityPack;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


class Program
{


static async Task Main()

{
    await ConcurrentDownload();
    await ScrapeQuotes();
        await MyTask();
}


static async Task ScrapeQuotes()
{
    string html = await DownloadAsync("http://quotes.toscrape.com/page/1/");
    IEnumerable<string> quotes = ExtractQuotes(html);

    foreach (string quote in quotes)
    {
        Console.WriteLine("* " + quote);
    }
    Console.WriteLine();
}

static IEnumerable<string> ExtractQuotes(string html)
{
    var doc = new HtmlDocument();
    doc.LoadHtml(html);

    return doc.DocumentNode // extraction of quotes
        .SelectNodes("//span[@class='text']")
        ?.Select(n => HtmlEntity.DeEntitize(n.InnerText.Trim())) // 
         ?? Enumerable.Empty<string>();
}

static async Task ConcurrentDownload()
{

    string[] urls = {
        "https://example.com",
        "https://dotnet.microsoft.com",
        "https://google.com",

        };

    Task<string>[] jobs = urls.Select(DownloadAsync).ToArray();

    try
    {
        string[] pages = await Task.WhenAll(jobs);

        for (int i = 0; i < urls.Length; i++)
        {
            Console.WriteLine($"{urls[i],-45} → {pages[i].Length,6} chars");
        }

    }
    catch (Exception ex)
    {
        Console.WriteLine($"At least one download failed: {ex.Message}");
    }

    Console.WriteLine();
}

static async Task<string> DownloadAsync(string url)
{
    using HttpClient client = new HttpClient();
    return await client.GetStringAsync(url);
}



static async Task<int> MyTask()
    {
        for (int i = 0; i<10; i++)
        {
            await SmallTask(i);
        }

        return 0; 
    }

 static async Task SmallTask(int i)
    {
        Console.WriteLine("Task" + i);
    }
}

