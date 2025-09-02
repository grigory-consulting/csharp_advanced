static IEnumerable<string> ExtractQuotes(string html)
{
    var doc = new HtmlDocument();
    doc.LoadHtml(html);

    return doc.DocumentNode // extraction of quotes
        .SelectNodes("//span[@class='text']")
        ?.Select(n => HtmlEntity.DeEntitize(n.InnerText.Trim())) // 
         ?? Enumerable.Empty<string>();
}
