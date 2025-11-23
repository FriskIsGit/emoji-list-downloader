using WebScrapper.scrapper;

namespace emoji_list_downloader.downloader; 

public class EmojiExtractor {
    string EMOJIS_WIKI = "https://emojis.wiki/all-emojis";
     const string SMILEYS_AND_EMOTION_CATEGORY = "smileys-and-emotion-category";
    const string PEOPLE_AND_BODY_CATEGORY = "people-and-body-category";
    const string ANIMALS_AND_NATURE_CATEGORY = "animals-and-nature-category";
    const string FOOD_AND_DRINK_CATEGORY = "food-and-drink-category";
    const string TRAVEL_AND_PLACES_CATEGORY = "travel-and-places-category";
    const string ACTIVITIES_CATEGORY = "activities-category";
    const string OBJECTS_CATEGORY = "objects-category";
    const string SYMBOLS_CATEGORY = "symbols-category";
    const string FLAGS_CATEGORY = "flags-category";
    const string COMPONENT_CATEGORY = "component-category";

    private static string[] CATEGORIES = {
        SMILEYS_AND_EMOTION_CATEGORY, PEOPLE_AND_BODY_CATEGORY, ANIMALS_AND_NATURE_CATEGORY, FOOD_AND_DRINK_CATEGORY,
        TRAVEL_AND_PLACES_CATEGORY, ACTIVITIES_CATEGORY, OBJECTS_CATEGORY, SYMBOLS_CATEGORY, FLAGS_CATEGORY, COMPONENT_CATEGORY
    };
    
    private readonly ExtendedHttpClient client = new(new HttpClientHandler { AllowAutoRedirect = true }) {
        Timeout = TimeSpan.FromSeconds(60)
    };

    public List<Emoji> scrapeWikiEmojis() {
        var emojis = new List<Emoji>();
        SimpleResponse response = client.get(EMOJIS_WIKI);
        HtmlDoc doc = new HtmlDoc(response.content);

        int index = 70000;
        foreach (string category in CATEGORIES) {
            Tag? categoryDiv = doc.FindFrom("div", index, Compare.Exact("id", category));
            if (categoryDiv == null) {
                Console.WriteLine("Category div " + category + " not found, skipping");
                continue;
            }

            List<Tag> emojiAnchors = doc.ExtractTags(categoryDiv, "a");
            foreach (Tag anchor in emojiAnchors) {
                if (!anchor.CompareAttributes(
                        Compare.Exact("class", "w-full"), 
                        Compare.KeyAndValuePrefix("href", "/"))) {
                    continue;
                }

                List<Tag> spans = doc.ExtractTags(anchor, "span");
                if (spans.Count < 2) {
                    Console.WriteLine("Not enough spans, actual: " + spans.Count);
                    continue;
                }

                string emojiChar = doc.ExtractText(spans[0]);
                string emojiName = doc.ExtractText(spans[1]);
                Console.WriteLine(emojiChar + " " + emojiName + " length=" + emojiChar.Length);
                
                Emoji emoji = new Emoji(emojiChar, emojiName);
                emojis.Add(emoji);
            }
            index = categoryDiv.EstimateEndOffset();
        }
        
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