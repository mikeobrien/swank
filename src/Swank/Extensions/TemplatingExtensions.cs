﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using MarkdownDeep;
using Nustache.Core;
using RazorEngine;
using RazorEngine.Templating;
using Swank.Web.Assets;

namespace Swank.Extensions
{
    public static class TemplatingExtensions
    {
        public static string RenderMustache<T>(this string template, T model)
        {
            return Render.StringToString(template, model);
        }

        public static void CompileRazor<T>(this string template)
        {
            var name = template.Hash();
            if (Engine.Razor.IsTemplateCached(name, typeof(T))) return;
            Engine.Razor.Compile(template, name, typeof(T));
        }

        public static string RenderRazor<T>(this string template, T model)
        {
            return Engine.Razor.Run(template.Hash(), typeof(T), model);
        }

        // Markdown

        public static string AddMarkdownExtension(this string filename)
        {
            return filename.EndsWith(MarkdownAsset.Extension) ? 
                filename : filename + MarkdownAsset.Extension;
        }

        public static string TransformMarkdownBlock(this string markdown)
        {
            return markdown.IsNullOrEmpty() ? markdown :
                new Markdown
                {
                    AutoHeadingIDs = true,
                    ExtraMode = true
                }.Transform(markdown
                    .RenderMarkdownFencedCodeBlocks()
                    .ConvertEmojisToHtml()).Trim();
        }

        public static string TransformMarkdownInline(this string markdown)
        {
            return markdown.TransformMarkdownBlock().UnwrapParagraphTags();
        }

        private static readonly Regex FragmentIdRegex = new Regex("\\s");

        public static string ToFragmentId(this string value)
        {
            return value.IsNullOrEmpty() ? value :
                FragmentIdRegex.Replace(value, "-").ToLower();
        }

        private static readonly Regex FencedCodeBlocksRegex =
            new Regex(@"^((`{3}\s*?)(\w+)?(\s*([\w\W]+?)\n*)\2)\n*",
                RegexOptions.Multiline);
    
        public static string RenderMarkdownFencedCodeBlocks(this string source)
        {
            return source.IsNullOrEmpty() ? source :
                FencedCodeBlocksRegex.Replace(source, x =>
                    $"<pre class=\"{x.Groups[3].Value}\"><code>" +
                    x.Groups[4].Value.Trim() + "</code></pre>");
        }

        // Html

        public static string UnwrapParagraphTags(this string text)
        {
            return string.IsNullOrEmpty(text) ? text :
                Regex.Replace(text, "(^<p>|</p>$)", "", RegexOptions.IgnoreCase);
        }

        public static string ConvertNbspHtmlEntityToSpaces(this string text)
        {
            return Regex.Replace(text, "&nbsp;", " ", RegexOptions.IgnoreCase);
        }

        public static string ConvertBrHtmlTagsToLineBreaks(this string text)
        {
            return Regex.Replace(text, "<br\\s?\\/?>", "\r\n", RegexOptions.IgnoreCase);
        }

        public static string RemoveWhitespace(this string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return text.Split(new[] { "\r\n", "\r", "\n" }, 
                    StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .Aggregate((a, i) => a + i);
        }

        public static string NormalizeLineBreaks(this string value)
        {
            return value.Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Replace("\n", "\r\n");
        }

        public static string ConvertEmojisToHtml(this string text)
        {
            return Macros.Aggregate(text, (t, m) => t.Replace(
                $":{m}:", $"<img class=\"emoji {m}\" />"));
        }

        private static readonly string[] Macros = { "-1", "+1", "8ball", "100", "1234",
            "a", "ab", "abc", "abcd", "accept", "aerial_tramway", "airplane",
            "alarm_clock", "alien", "ambulance", "anchor", "angel", "anger", "angry",
            "anguished", "ant", "apple", "aquarius", "aries", "arrow_backward",
            "arrow_double_down", "arrow_double_up", "arrow_down_small", "arrow_down",
            "arrow_forward", "arrow_heading_down", "arrow_heading_up", "arrow_left",
            "arrow_lower_left", "arrow_lower_right", "arrow_right_hook", "arrow_right",
            "arrow_up_down", "arrow_up_small", "arrow_up", "arrow_upper_left",
            "arrow_upper_right", "arrows_clockwise", "arrows_counterclockwise", "art",
            "articulated_lorry", "astonished", "atm", "b", "baby_bottle", "baby_chick",
            "baby_symbol", "baby", "back", "baggage_claim", "balloon", "ballot_box_with_check",
            "bamboo", "banana", "bangbang", "bank", "bar_chart", "barber", "baseball",
            "basketball", "bath", "bathtub", "battery", "bear", "bee", "beer", "beers",
            "beetle", "beginner", "bell", "bento", "bicyclist", "bike", "bikini", "bird",
            "birthday", "black_circle", "black_joker", "black_medium_small_square", "black_medium_square",
            "black_nib", "black_small_square", "black_square_button", "black_square", "blossom",
            "blowfish", "blue_book", "blue_car", "blue_heart", "blush", "boar", "boat", "bomb",
            "book", "bookmark_tabs", "bookmark", "books", "boom", "boot", "bouquet", "bow",
            "bowling", "bowtie", "boy", "bread", "bride_with_veil", "bridge_at_night", "briefcase",
            "broken_heart", "bug", "bulb", "bullettrain_front", "bullettrain_side", "bus", "busstop",
            "bust_in_silhouette", "busts_in_silhouette", "cactus", "cake", "calendar", "calling",
            "camel", "camera", "cancer", "candy", "capital_abcd", "capricorn", "car", "card_index",
            "carousel_horse", "cat", "cat2", "cd", "chart_with_downwards_trend", "chart_with_upwards_trend",
            "chart", "checkered_flag", "cherries", "cherry_blossom", "chestnut", "chicken",
            "children_crossing", "chocolate_bar", "christmas_tree", "church", "cinema", "circus_tent",
            "city_sunrise", "city_sunset", "cl", "clap", "clapper", "clipboard", "clock1", "clock2",
            "clock3", "clock4", "clock5", "clock6", "clock7", "clock8", "clock9", "clock10", "clock11",
            "clock12", "clock130", "clock230", "clock330", "clock430", "clock530", "clock630", "clock730",
            "clock830", "clock930", "clock1030", "clock1130", "clock1230", "closed_book",
            "closed_lock_with_key", "closed_umbrella", "cloud", "clubs", "cn", "cocktail", "coffee",
            "cold_sweat", "collision", "computer", "confetti_ball", "confounded", "confused",
            "congratulations", "construction_worker", "construction", "convenience_store", "cookie",
            "cool", "cop", "copyright", "corn", "couple_with_heart", "couple", "couplekiss", "cow",
            "cow2", "credit_card", "crescent_moon", "crocodile", "crossed_flags", "crown", "cry",
            "crying_cat_face", "crystal_ball", "cupid", "curly_loop", "currency_exchange", "curry",
            "custard", "customs", "cyclone", "dancer", "dancers", "dango", "dart", "dash", "date",
            "de", "deciduous_tree", "department_store", "diamond_shape_with_a_dot_inside", "diamonds",
            "disappointed_relieved", "disappointed", "dizzy_face", "dizzy", "do_not_litter", "dog", "dog2",
            "dollar", "dolls", "dolphin", "donut", "door", "doughnut", "dragon_face", "dragon", "dress",
            "dromedary_camel", "droplet", "dvd", "e-mail", "ear_of_rice", "ear", "earth_africa",
            "earth_americas", "earth_asia", "egg", "eggplant", "eight_pointed_black_star", "eight_spoked_asterisk",
            "eight", "electric_plug", "elephant", "email", "end", "envelope", "es", "euro", "european_castle",
            "european_post_office", "evergreen_tree", "exclamation", "expressionless", "eyeglasses", "eyes",
            "facepunch", "factory", "fallen_leaf", "family", "fast_forward", "fax", "fearful", "feelsgood"
                , "feet", "ferris_wheel", "file_folder", "finnadie", "fire_engine", "fire", "fireworks",
            "first_quarter_moon_with_face", "first_quarter_moon", "fish_cake", "fish", "fishing_pole_and_fish",
            "fist", "five", "flags", "flashlight", "floppy_disk", "flower_playing_cards", "flushed", "foggy",
            "football", "fork_and_knife", "fountain", "four_leaf_clover", "four", "fr", "free",
            "fried_shrimp", "fries", "frog", "frowning", "fu", "fuelpump", "full_moon_with_face", "full_moon",
            "game_die", "gb", "gem", "gemini", "ghost", "gift_heart", "gift", "girl", "globe_with_meridians",
            "goat", "goberserk", "godmode", "golf", "grapes", "green_apple", "green_book", "green_heart",
            "grey_exclamation", "grey_question", "grimacing", "grin", "grinning", "guardsman", "guitar",
            "gun", "haircut", "hamburger", "hammer", "hamster", "hand", "handbag", "hankey", "hash",
            "hatched_chick", "hatching_chick", "headphones", "hear_no_evil", "heart_decoration", "heart_eyes_cat",
            "heart_eyes", "heart", "heartbeat", "heartpulse", "hearts", "heavy_check_mark", "heavy_division_sign",
            "heavy_dollar_sign", "heavy_exclamation_mark", "heavy_minus_sign", "heavy_multiplication_x",
            "heavy_plus_sign", "helicopter", "herb", "hibiscus", "high_brightness", "high_heel",
            "hocho", "honey_pot", "honeybee", "horse_racing", "horse", "hospital", "hotel", "hotsprings",
            "hourglass_flowing_sand", "hourglass", "house_with_garden", "house", "hurtrealbad", "hushed",
            "ice_cream", "icecream", "id", "ideograph_advantage", "imp", "inbox_tray", "incoming_envelope",
            "information_desk_person", "information_source", "innocent", "interrobang", "iphone", "it",
            "izakaya_lantern", "jack_o_lantern", "japan", "japanese_castle", "japanese_goblin", "japanese_ogre",
            "jeans", "joy_cat", "joy", "jp", "key", "keycap_ten", "kimono", "kiss", "kissing_cat",
            "kissing_closed_eyes", "kissing_face", "kissing_heart", "kissing_smiling_eyes", "kissing", "koala",
            "koko", "kr", "large_blue_circle", "large_blue_diamond", "large_orange_diamond",
            "last_quarter_moon_with_face", "last_quarter_moon", "laughing", "leaves", "ledger", "left_luggage",
            "left_right_arrow", "leftwards_arrow_with_hook", "lemon", "leo", "leopard", "libra", "light_rail",
            "link", "lips", "lipstick", "lock_with_ink_pen", "lock", "lollipop", "loop", "loudspeaker",
            "love_hotel", "love_letter", "low_brightness", "m", "mag_right", "mag", "mahjong",
            "mailbox_closed", "mailbox_with_mail", "mailbox_with_no_mail", "mailbox", "man_with_gua_pi_mao",
            "man_with_turban", "man", "mans_shoe", "maple_leaf", "mask", "massage", "meat_on_bone", "mega",
            "melon", "memo", "mens", "metal", "metro", "microphone", "microscope", "milky_way", "minibus",
            "minidisc", "mobile_phone_off", "money_with_wings", "moneybag", "monkey_face", "monkey", "monorail",
            "mortar_board", "mount_fuji", "mountain_bicyclist", "mountain_cableway", "mountain_railway", "mouse",
            "mouse2", "movie_camera", "moyai", "muscle", "mushroom", "musical_keyboard", "musical_note",
            "musical_score", "mute", "nail_care", "name_badge", "neckbeard", "necktie",
            "negative_squared_cross_mark", "neutral_face", "new_moon_with_face", "new_moon", "new", "newspaper",
            "ng", "nine", "no_bell", "no_bicycles", "no_entry_sign", "no_entry", "no_good", "no_mobile_phones",
            "no_mouth", "no_pedestrians", "no_smoking", "non-potable_water", "nose",
            "notebook_with_decorative_cover", "notebook", "notes", "nut_and_bolt", "o", "o2", "ocean",
            "octocat", "octopus", "oden", "office", "ok_hand", "ok_woman", "ok", "older_man",
            "older_woman", "on", "oncoming_automobile", "oncoming_bus", "oncoming_police_car", "oncoming_taxi",
            "one", "open_file_folder", "open_hands", "open_mouth", "ophiuchus", "orange_book",
            "outbox_tray", "ox", "package", "page_facing_up", "page_with_curl", "pager", "palm_tree",
            "panda_face", "paperclip", "parking", "part_alternation_mark", "partly_sunny", "passport_control",
            "paw_prints", "peach", "pear", "pencil", "pencil2", "penguin", "pensive", "performing_arts",
            "persevere", "person_frowning", "person_with_blond_hair", "person_with_pouting_face", "phone",
            "pig_nose", "pig", "pig2", "pill", "pineapple", "pisces", "pizza", "plus1", "point_down",
            "point_left", "point_right", "point_up_2", "point_up", "police_car", "poodle", "poop",
            "post_office", "postal_horn", "postbox", "potable_water", "pouch", "poultry_leg",
            "pound", "pouting_cat", "pray", "princess", "punch", "purple_heart", "purse", "pushpin",
            "put_litter_in_its_place", "question", "rabbit", "rabbit2", "racehorse", "radio_button", "radio",
            "rage", "rage1", "rage2", "rage3", "rage4", "railway_car", "rainbow", "raised_hand",
            "raised_hands", "raising_hand", "ram", "ramen", "rat", "recycle", "red_car", "red_circle",
            "registered", "relaxed", "relieved", "repeat_one", "repeat", "restroom", "revolving_hearts",
            "rewind", "ribbon", "rice_ball", "rice_cracker", "rice_scene", "rice", "ring", "rocket",
            "roller_coaster", "rooster", "rose", "rotating_light", "round_pushpin", "rowboat", "ru",
            "rugby_football", "runner", "running_shirt_with_sash", "running", "sa", "sagittarius",
            "sailboat", "sake", "sandal", "santa", "satellite", "satisfied", "saxophone", "school_satchel",
            "school", "scissors", "scorpius", "scream_cat", "scream", "scroll", "seat", "secret",
            "see_no_evil", "seedling", "seven", "shaved_ice", "sheep", "shell", "ship", "shipit",
            "shirt", "shit", "shoe", "shower", "signal_strength", "simple_smile", "six_pointed_star",
            "six", "ski", "skull", "sleeping", "sleepy", "slot_machine", "small_blue_diamond",
            "small_orange_diamond", "small_red_triangle_down", "small_red_triangle", "smile_cat", "smile",
            "smiley_cat", "smiley", "smiling_imp", "smirk_cat", "smirk", "smoking", "snail", "snake",
            "snowboarder", "snowflake", "snowman", "sob", "soccer", "soon", "sos", "sound",
            "space_invader", "spades", "spaghetti", "sparkle", "sparkler", "sparkles", "sparkling_heart",
            "speak_no_evil", "speaker", "speech_balloon", "speedboat", "squirrel", "star", "star2",
            "stars", "station", "statue_of_liberty", "steam_locomotive", "stew", "straight_ruler",
            "strawberry", "stuck_out_tongue_closed_eyes", "stuck_out_tongue_winking_eye", "stuck_out_tongue",
            "sun_with_face", "sunflower", "sunglasses", "sunny", "sunrise_over_mountains", "sunrise",
            "surfer", "sushi", "suspect", "suspension_railway", "sweat_drops", "sweat_smile", "sweat",
            "sweet_potato", "swimmer", "symbols", "syringe", "tada", "tanabata_tree", "tangerine",
            "taurus", "taxi", "tea", "telephone_receiver", "telephone", "telescope", "tennis",
            "tent", "thought_balloon", "three", "thumbsdown", "thumbsup", "ticket", "tiger",
            "tiger2", "tired_face", "tm", "toilet", "tokyo_tower", "tomato", "tongue", "top",
            "tophat", "tractor", "traffic_light", "train", "train2", "tram", "triangular_flag_on_post",
            "triangular_ruler", "trident", "triumph", "trolleybus", "trollface", "trophy", "tropical_drink",
            "tropical_fish", "truck", "trumpet", "tshirt", "tulip", "turtle", "tv", "twisted_rightwards_arrows",
            "two_hearts", "two_men_holding_hands", "two_women_holding_hands", "two", "u6e80", "u7a7a",
            "u55b6", "u5272", "u5408", "u6307", "u6708", "u6709", "u7121", "u7533", "u7981", "uk",
            "umbrella", "unamused", "underage", "unlock", "up", "us", "v", "vertical_traffic_light",
            "vhs", "vibration_mode", "video_camera", "video_game", "violin", "virgo", "volcano", "vs",
            "walking", "waning_crescent_moon", "waning_gibbous_moon", "warning", "watch", "water_buffalo",
            "watermelon", "wave", "wavy_dash", "waxing_crescent_moon", "waxing_gibbous_moon", "wc", "weary",
            "wedding", "whale", "whale2", "wheelchair", "white_check_mark", "white_circle", "white_flower",
            "white_large_square", "white_medium_small_square", "white_medium_square", "white_small_square",
            "white_square_button", "wind_chime", "wine_glass", "wink", "wolf", "woman", "womans_clothes",
            "womans_hat", "womens", "worried", "wrench", "x", "yellow_heart", "yen", "yum", "zap",
            "zero", "zzz.png" };
    }
}
