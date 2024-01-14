using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using System.Linq;

namespace GrammarChecker
{
    public static class GrammarChecker
    {
        /// <summary>
        /// Checks grammar of the text.
        /// </summary>
        /// <param name="path">Text.</param>
        /// <param name="mistakes">All the mistakes that were found.</param>
        /// <returns>True if there were no mistakes.</returns>
        public static bool Check(string text, out List<string> mistakes, bool ignoreUpperCase = true)
        {
            return Check(text, LanguageManager.CurrentLanguage, out mistakes, ignoreUpperCase);
        }
        /// <summary>
        /// Checks grammar of the text.
        /// </summary>
        /// <param name="path">Text.</param>
        /// <param name="language">Language of the text.</param>
        /// <param name="mistakes">All the mistakes that were found.</param>
        /// <returns>True if there were no mistakes.</returns>
        public static bool Check<T>(string text, Language<T> language, out List<string> mistakes, bool ignoreUpperCase = true) where T : Data
        {
            bool result = true;
            mistakes = new List<string>();
            string[] splitText = text.Split(' ');
            foreach (var item in splitText)
            {
                var itemCopy = ignoreUpperCase ? item.ToLower() : item;
                if (!language.SearchWord(itemCopy, out T data))
                {
                    mistakes.Add(item);
                    result = false;
                }
            }
            language.AddRangeOfMistakes(mistakes);
            return result;
        }
        /// <summary>
        /// Checks the text by the path.
        /// </summary>
        /// <param name="path">Path to the .txt file.</param>
        /// <param name="mistakes">All the mistakes that were found.</param>
        /// <param name="ignoreUpperCase">If true, all the upper-cases are ignored. True by default.</param>
        /// <returns>True if there were no mistakes.</returns>
        public static bool CheckPath(string path, out List<string> mistakes, bool ignoreUpperCase = true)
        {
            return CheckPath(path, LanguageManager.CurrentLanguage, out mistakes, ignoreUpperCase);
        }
        /// <summary>
        /// Checks the text by the path
        /// </summary>
        /// <param name="path">Path to the .txt file.</param>
        /// <param name="language"></param>
        /// <param name="mistakes">All the mistakes that were found.</param>
        /// <param name="ignoreUpperCase">If true, all the upper-cases are ignored. True by default.</param>
        /// <returns>True if there were no mistakes.</returns>.
        public static bool CheckPath<T>(string path, Language<T> language, out List<string> mistakes, bool ignoreUpperCase = true) where T : Data
        {
            FileStream fstream = new FileStream(path, FileMode.Open, FileAccess.Read);
            bool result = true;
            mistakes = new List<string>();
            using (var sr = new StreamReader(fstream, Encoding.ASCII))
            {
                while (!sr.EndOfStream)
                {
                    string[] splitLine = sr.ReadLine().Split(' ');
                    foreach (var item in splitLine)
                    {
                        var itemCopy = ignoreUpperCase ? item.ToLower() : item;
                        if (!language.SearchWord(itemCopy, out T data))
                        {
                            mistakes.Add(item);
                            result = false;
                        }
                    }
                }
            }
            language.AddRangeOfMistakes(mistakes);
            return result;
        }
        /// <summary>
        /// Clears double-spaces and clears/adds spaces from the text according to the grammar rules.
        /// </summary>
        /// <param name="text">A string text.</param>
        /// <returns>String with cleared spaces.</returns>
        public static string ClearSpaces(string text)
        {
            string newText = "";
        var characters = SpecialCharacters.Enumeration;
            for (int i = 1; i<text.Length-1; i++)
            {
                var symbol = text[i];
        var character = characters.SingleOrDefault(x => x.Character == text[i]);
        var next_character = characters.SingleOrDefault(x => x.Character == text[i + 1]);

                if (Char.IsDigit(symbol))
                {
                    newText += symbol;
                    continue;
                }
                
                if (symbol == ' ' && text[i + 1] == ' ')
                {
                    continue;
                }

if (character != default(SpecialCharacter) && !Char.IsDigit(text[i - 1]))
{
    switch (character.SpaceBefore)
    {
        case true:
            if (text[i - 1] != ' ')
                newText += ' ';
            break;
        case false:
            if (text[i - 1] == ' ')
                newText = newText.Remove(newText.Length - 1);
            break;
    }

    switch (character.SpaceAfter)
    {
        case true:
            newText += symbol;
            if (text[i + 1] != ' ')
            {
                if (next_character == default(SpecialCharacter))
                {
                    newText += " ";
                }
            }
            break;
        case false:
            newText += symbol;
            int k = i + 1;
            while (text[k] == ' ')
            {
                i++;
                k++;
            }
            continue;
    }
    continue;
}
newText += symbol;
            }

            newText = newText.Trim();
newText = text[0] + newText;
newText += text[text.Length - 1];

return newText.Trim();
        }
        /// <summary>
        /// Clears double-spaces and clears/adds spaces next to the specialCharacter according to the grammar rules.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="specialCharacter"></param>
        /// <returns>Cleared string/</returns>
        public static string ClearSpaces(string text, SpecialCharacter specialCharacter)
{
    string newText = "";
    var characters = SpecialCharacters.Enumeration;
    for (int i = 1; i < text.Length - 1; i++)
    {
        var symbol = text[i];
        var next_character = characters.SingleOrDefault(x => x.Character == text[i + 1]);

        if (Char.IsDigit(symbol))
        {
            newText += symbol;
            continue;
        }

        if (symbol == ' ' && text[i + 1] == ' ')
        {
            continue;
        }

        if (symbol == specialCharacter.Character)
        {
            switch (specialCharacter.SpaceBefore)
            {
                case true:
                    if (text[i - 1] != ' ')
                        newText += ' ';
                    break;
                case false:
                    if (text[i - 1] == ' ')
                        newText = newText.Remove(newText.Length - 1);
                    break;
            }
            switch (specialCharacter.SpaceAfter)
            {
                case true:
                    newText += symbol;
                    if (text[i + 1] != ' ')
                    {
                        if (next_character == default(SpecialCharacter))
                        {
                            newText += " ";
                        }
                    }
                    break;
                case false:
                    newText += symbol;
                    int k = i + 1;
                    while (text[k] == ' ')
                    {
                        i++;
                        k++;
                    }
                    continue;
            }
            continue;
        }
        newText += symbol;
    }

    newText = newText.Trim();
    newText = text[0] + newText;
    newText += text[text.Length - 1];

    return newText.Trim();
}
/// <summary>
/// Compares two words and returns percentage of success compare.
/// </summary>
/// <param name="word1">First word.</param>
/// <param name="word2">Second word.</param>
/// <returns>Double; 100 means 100% and 0 means 0%.</returns>
public static double CompareWords(string word1, string word2)
{
    while (word1.Length != word2.Length)
    {
        if (word1.Length > word2.Length)
        {
            word2 += " ";
        }
        else
        {
            word1 += " ";
        }
    }

    int correct_symbols = 0;
    for (int i = 0; i < word1.Length; i++)
    {
        if (word1[i].Equals(word2[i]))
        {
            correct_symbols++;
        }
    }

    return Math.Round(100 / (double)correct_symbols, 2);
}

//public static Dictionary<string, IEnumerable<string>> GetMistakes(string text)
//{

//}

public static IEnumerable<string> GetSpellCorrections<T>(string word, Language<T> language, int amount = 5) where T : Data
{
    if (word == null || language == null)
    {
        throw new ArgumentNullException("Arguments cannot be null.");
    }

    return language.SearchSpellCorrections(word, amount);
}
    }
}