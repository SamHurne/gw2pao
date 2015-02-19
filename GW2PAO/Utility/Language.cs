using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Utility
{
    public enum Language
    {
        English,
        Spanish,
        French,
        German
    }

    public static class LanguageExtensions
    {
        public static string ToFullName(this Language lang)
        {
            switch (lang)
            {
                case Language.English:
                    return "English";
                case Language.Spanish:
                    return "Español";
                case Language.French:
                    return "Français";
                case Language.German:
                    return "Deutsch";
                default:
                    return "???";
            }
        }

        public static Language FromFullName(string lang)
        {
            switch (lang)
            {
                case "English":
                    return Language.English;
                case "Español":
                    return Language.Spanish;
                case "Français":
                    return Language.French;
                case "Deutsch":
                    return Language.German;
                default:
                    return Language.English;
            }
        }

        public static string ToTwoLetterISOLanguageName(this Language lang)
        {
            switch (lang)
            {
                case Language.English:
                    return "en";
                case Language.Spanish:
                    return "es";
                case Language.French:
                    return "fr";
                case Language.German:
                    return "de";
                default:
                    return "en";
            }
        }

        public static Language FromTwoLetterISOLanguageName(string lang)
        {
            switch (lang)
            {
                case "en":
                    return Language.English;
                case "es":
                    return Language.Spanish;
                case "fr":
                    return Language.French;
                case "de":
                    return Language.German;
                default:
                    return Language.English;
            }
        }
    }
}
