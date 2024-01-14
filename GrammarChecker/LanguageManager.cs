using System.Collections.Generic;

namespace GrammarChecker
{
    public abstract class LanguageManager<T>
        where T : Data
    {
        public static Dictionary<string, Language<T>> Languages { get; }

        public static Language<T> Russian { get => new Language<T>("Russian", "Languages\\ru-ru.txt"); }
        public static Language<T> English { get => new Language<T>("English", "Languages\\en-us.txt"); }
        /// <summary>
        /// Current Language. English by default.
        /// </summary>
        public static Language<T> CurrentLanguage = English;

        public static void AddLanguage(string languageName, string filePath)
        {
            Languages.Add(languageName, new Language<T>(languageName, filePath));
        }

        public static Language<T> GetLanguage(string languageName)
        {
            if (Languages.TryGetValue(languageName, out Language<T> value))
            {
                return value;
            }
            else return null;
        }
    }

    public abstract class LanguageManager : LanguageManager<Data>
    {

    }
}