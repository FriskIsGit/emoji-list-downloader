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
    }
}