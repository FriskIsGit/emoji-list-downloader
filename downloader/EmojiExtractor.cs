using System.Diagnostics;
using WebScrapper.scrapper;

namespace emoji_list_downloader.downloader; 

public class EmojiExtractor {
    string EMOJIS_WIKI = "https://emojis.wiki/all-emojis";

    private readonly ExtendedHttpClient client = new(new HttpClientHandler { AllowAutoRedirect = true }) {
        Timeout = TimeSpan.FromSeconds(60)
    };

    public List<Emoji> scrapeWikiEmojis() {
        var emojis = new List<Emoji>();
        SimpleResponse response = client.get(EMOJIS_WIKI);
        HtmlDoc doc = new HtmlDoc(response.content);

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        
        List<Tag> emojiAnchors = doc.FindAllFrom("a", 0, 
            Compare.Exact("class", "w-full"), Compare.KeyAndValuePrefix("href", "/"));
        foreach (Tag anchor in emojiAnchors) {
            List<Tag> spans = doc.ExtractTags(anchor, "span");
            if (spans.Count < 2) {
                Console.WriteLine("Not enough spans, actual: " + spans.Count);
                continue;
            }

            string emojiChar = doc.ExtractText(spans[0]);
            string emojiName = doc.ExtractText(spans[1]).Trim();
            Console.WriteLine(emojiChar + " " + emojiName + " length=" + emojiChar.Length);
                
            Emoji emoji = new Emoji(emojiChar, emojiName);
            emojis.Add(emoji);
        }
        Console.WriteLine(stopwatch.ElapsedMilliseconds + "ms elapsed while scraping " + response.content.Length + " chars of HTML");
        
        return emojis;
    }
}

public class Emoji {
    public string character;
    public string name;

    public Emoji(string character, string name) {
        this.character = character;
        this.name = name;
    }
}