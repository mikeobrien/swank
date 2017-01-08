using System;
using System.Linq;
using Swank.Extensions;

namespace DemoSite
{
    public class HipsterIpsum
    {
        private static readonly Random Random = new Random();

        public static string Generate(int sentences = 1, int paragraphs = 1)
        {
            return Enumerable.Range(1, paragraphs)
                .Select(y => Enumerable.Range(1, sentences)
                    .Select(x => Enumerable.Range(1, 10)
                        .Select(i => Dictionary[Random.Next(0, Dictionary.Length - 1)])
                        .Select(w => Random.Next(1, 10) > 7 ? $"**{w}**" : w).Join(" "))
                    .Select(x => Dictionary[Random.Next(0, Dictionary.Length - 27)].InitialCap() + $" {x}.")
                    .Join(" "))
                .Join("\r\n\r\n");
        }

        private static readonly string[] Dictionary = { "8-bit", "actually", "aesthetic", "affogato",
            "art", "artisan", "asymmetrical", "austin", "authentic", "axe", "bag", "banh", "banjo", "batch", "beard",
            "beer", "before", "belly", "bespoke", "bicycle", "biodiesel", "bird", "bitters", "blog", "blue", "booth",
            "bottle", "braid", "brooklyn", "brunch", "bun", "bushwick", "butcher", "cardigan", "carry", "celiac",
            "chambray", "chartreuse", "chia", "chic", "chicharrones", "chillwave", "chips", "church-key", "cleanse",
            "cliche", "coffee", "cold-pressed", "craft", "cray", "cred", "cronut", "deep",
            "denim", "direct", "disrupt", "distillery", "DIY", "dollar", "dreamcatcher", "drinking", "echo", "ennui",
            "ethical", "Etsy", "everyday", "fanny", "fap", "farm-to-table", "fashion", "fingerstache", "fixie", "flannel",
            "flexitarian", "food", "forage", "four", "franzen", "freegan", "fund", "gastropub", "gentrify", "gluten-free",
            "gochujang", "godard", "goth", "green", "hammock", "hashtag", "health", "heard", "heirloom", "hella",
            "helvetica", "hoodie", "humblebrag", "intelligentsia", "iPhone", "irony", "jean", "juice", "kale", "keffiyeh",
            "keytar", "kickstarter", "kinfolk", "kitsch", "knausgaard", "kogi", "kombucha", "leggings", "letterpress", "level",
            "listicle", "literally", "locavore", "lo-fi", "loko", "lomo",  "man", "marfa", "master", "meditation",
            "meggings", "meh", "messenger", "mi", "microdosing", "migas", "mixtape", "mlkshk", "moon", "mumblecore", "mustache",
            "narwhal", "neutra", "next", "normcore", "occupy", "offal", "organic",  "pabst", "paleo",
            "park", "party", "pBR&B", "photo", "pickled", "pinterest", "pitchfork", "plaid", "polaroid", "pop-up", "pork",
            "portland", "post-ironic", "pour-over", "poutine", "probably", "pug", "put", "quinoa", "ramps", "raw", "readymade",
            "retro", "rights", "roof", "salvia", "sartorial", "scenester", "schlitz", "seitan", "selfies", "selvage", "semiotics",
            "shabby", "shoreditch", "shorts", "single-origin", "skateboard", "slow-carb", "small", "sold", "squid", "sriracha",
            "street", "stumptown", "sustainable", "swag", "synth", "tacos", "tattooed", "taxidermy", "them", "they", "thundercats",
            "tilde", "toast", "tofu", "tote", "tousled", "trade", "truck", "truffaut", "trust", "try-hard", "tumblr", "twee",
            "typewriter", "ugh", "umami", "v", "vegan", "venmo", "VHS", "vice", "vinegar", "vinyl", "viral", "waistcoat",
            "wayfarers", "whatever", "williamsburg", "wolf", "XOXO", "YOLO", "you", "yr", "yuccie",
            ":smile:",":thumbsup:",":trollface:",":floppy_disk:",":key:",":lock:",":sound:",":mag_right:",":beer:",":shipit:",
            ":octocat:",":poop:",":thumbsdown:",":floppy_disk:",":key:",":lock:",":sound:",":mag_right:",":beer:",":shipit:",
            ":smile:",":thumbsup:",":trollface:", ":octocat:",":poop:",":thumbsdown:"};
    }
}