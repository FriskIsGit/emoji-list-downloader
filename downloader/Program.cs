using System.Text;

namespace emoji_list_downloader.downloader;

class Program {
    
    static void Main(string[] args) {
        if (args.Length > 0 && (args[0].StartsWith("-h") || args[0].StartsWith("--help"))) {
            Console.WriteLine("TODO: Help requested");
            return;
        }

        var extractor = new EmojiExtractor();
        List<Emoji> emojis = extractor.scrapeWikiEmojis();
        Console.WriteLine("Emoji count: " + emojis.Count);
        writeEmojisToJSFile(emojis);
    }

    // [emoji, unicode_name, aliases...]
    static void writeEmojisToJSFile(List<Emoji> emojis) {
        var code = new StringBuilder();
        code.Append("let emojis = [\n");
        foreach (Emoji emoji in emojis) {
            code.Append("[\"" + emoji.character + "\", \"" + emoji.name + "\"],\n");
        }

        code.Append("]\n");
        File.WriteAllText("emojis.js", code.ToString());
    }
}