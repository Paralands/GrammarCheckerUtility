using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace GrammarChecker
{
    /// <summary>
    /// Language<T>
    /// </summary>
    /// <typeparam name="T">Type of data that every word has</typeparam>
    public class Language<T>
        where T : Data
    {
        public Trie<T> trie { get; }
        public Trie<T> mistakes { get; }
        public string FilePath { get; private set; }
        public string LanguageName { get; set; }
        public int Count { get => trie.Count; }

        public Language(string languageName, Trie<T> trie)
        {
            if (!string.IsNullOrEmpty(languageName))
            {
                LanguageName = languageName;
            }
            else throw new ArgumentNullException("Argument languageName cannot be null!");

            if (trie != null)
            {
                this.trie = trie;
            }
            else throw new ArgumentNullException("Argument trie cannot be null!");
        }

        /// <summary>
        /// Creating a language.
        /// </summary>
        /// <param name="languageName">Language name in English.</param>
        /// <param name="filePath">Path to the .txt file with all the words this language has.</param>
        /// <param name="rootData">Data that root node contains. It is recommended to use for saving default information about a specific language.</param>
        public Language(string languageName, string filePath, T rootData = default(T))
        {
            if (!string.IsNullOrEmpty(languageName))
            {
                LanguageName = languageName;
            }
            else throw new ArgumentNullException("Argument languageName cannot be null!");

            if (!string.IsNullOrEmpty(filePath))
            {
                FilePath = filePath;
            }
            else throw new ArgumentNullException("Argument filePath cannot be null!");

            trie = new Trie<T>(new Node<T>(' ', rootData, ""));
            mistakes = new Trie<T>(new Node<T>(' ', rootData, ""));
            CompleteTheTrie();
        }
        /// <summary>
        /// Completes the trie with deafult data.
        /// </summary>
        private void CompleteTheTrie()
        {
            FileStream fstream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            using (var sr = new StreamReader(fstream, Encoding.ASCII))
            {
                while (!sr.EndOfStream)
                {
                    trie.Add(sr.ReadLine(), (T)new Data());
                }
            }
        }
        public void AddMistake(string word, IEnumerable<Node<T>> corrections, T data = default(T))
        {
            if (data == default(T))
            {
                data = (T)new Data();
            }
            if (mistakes.TrySearch(word, out Node<T> node) && node is Mistake<T> mistake)
            {
                mistake.AddCorrectionRange(corrections);
                mistake.Data.Weight++;
            }
            else
            {
                mistakes.Add(word, data);
                mistakes.TrySearch(word, out Node<T> node1);
                mistakes.TryReplaceNode(node, new Mistake<T>(node1, this, corrections));
                trie.Remove(word);
            }
        }
        public void AddMistake(string word, IEnumerable<string> corrections, T data = default(T))
        {
            if (data == default(T))
            {
                data = (T)new Data();
            }
            var list = new List<Node<T>>();
            foreach (var item in corrections)
            {
                var node = new Node<T>(item.Last(), (T)new Data(), item);
                node.IsWord = true;
                list.Add(node);
            }
            AddMistake(word, list, data);
        }
        public void AddMistake(string word, T data = default(T))
        {
            if (data == default(T))
            {
                data = (T)new Data();
            }
            mistakes.Add(word, data);
            mistakes.TrySearch(word, out Node<T> node);
            mistakes.TryReplaceNode(node, new Mistake<T>(node, this));
            trie.Remove(word);
        }
        public void AddRangeOfMistakes(IEnumerable<string> words)
        {
            foreach (var item in words)
            {
                AddMistake(item);
            }
        }
        public void RemoveMistake(string word)
        {
            mistakes.Remove(word);
        }

        /// <summary>
        /// Adding a word.
        /// </summary>
        /// <param name="word">Word to add.</param>
        /// <param name="data">Data that word contains.</param>
        public void AddWord(string word, T data = default(T))
        {
            trie.Add(word, data);
            RemoveMistake(word);
        }
        /// <summary>
        /// Adding words with default data.
        /// </summary>
        /// <param name="words">Words that has to be added.</param>
        public void AddRangeOfWords(IEnumerable<string> words)
        {
            trie.AddRange(words);
            mistakes.RemoveRange(words);
        }
        /// <summary>
        /// Adds all the words from the file to the trie.
        /// </summary>
        /// <param name="path">File path.</param>
        public void AddFile(string path)
        {
            Trie<T> words = new Trie<T>(new Node<T>(' ', default(T), ""));

            FileStream fstream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using (var sr = new StreamReader(fstream, Encoding.ASCII))
            {
                while (!sr.EndOfStream)
                {
                    string[] splitLine = sr.ReadLine().ToLower().Split(' ');
                    words.AddRange(splitLine);
                }
            }

            trie.AddTrie(words);
            mistakes.RemoveTrie(words);
        }
        /// <summary>
        /// Removing a word.
        /// </summary>
        /// <param name="word">Word to remove.</param>
        public void RemoveWord(string word)
        {
            trie.Remove(word);
        }
        /// <summary>
        /// Removing a list of words.
        /// </summary>
        /// <param name="words">Words to remove.</param>
        public void RemoveRangeOfWords(IEnumerable<string> words)
        {
            trie.RemoveRange(words);
        }
        /// <summary>
        /// Searching for a word.
        /// </summary>
        /// <param name="word">Word to search for,</param>
        /// <param name="data">Data that the word contains.</param>
        /// <returns>True if word exists; False is not.</returns>
        public bool SearchWord(string word, out T data)
        {
            var search = trie.TrySearch(word, out T value);
            data = value;
            return search;
        }
        /// <summary>
        /// Searching for a word.
        /// </summary>
        /// <param name="word">Word to search for,</param>
        /// <returns>True if word exists; False is not.</returns>
        public bool SearchWord(string word)
        {
            return SearchWord(word, out T data);
        }
        /// <summary>
        /// Searching for every word in a list.
        /// </summary>
        /// <param name="words">Words to search for.</param>
        /// <param name="data">Data of words.</param>
        /// <returns>True if word exists; False is not.</returns>
        public Dictionary<string, bool> SearchRangeOfWords(IEnumerable<string> words, out Dictionary<string, T> data)
        {
            var search = trie.TrySearchRange(words, out Dictionary<string, T> value);
            data = value;
            return search;
        }

        /// <summary>
        /// Searches for spell corrections of mistake in the word.
        /// </summary>
        /// <param name="word">A word that has a mistake in it.</param>
        /// <param name="amount">An amount of words in IEnumerable.</param>
        /// <returns>IEnumerable of spell corrections variants. Null if word spelling is correct.</returns>
        public IEnumerable<string> SearchSpellCorrections(string word, int amount, double percentage = 80)
        {
            if (word == null)
            {
                throw new ArgumentNullException("Word cannot be null");
            }
            if (trie.TrySearch(word, out Node<T> lastNode))
            {
                return null;
            }

            var corrections = new List<string>();

            if (mistakes.TrySearch(word, out Node<T> nodeMistake) && nodeMistake is Mistake<T> mistake)
            {
                corrections = mistake.GetCorrections(amount, SortRegime.ByWeight).Select(x => x.Prefix).ToList();
                if (corrections.Count == amount)
                {
                    return corrections;
                }
            }

            var allNodes = new List<Node<T>>();
            var endOfKey = word.Substring(lastNode.Prefix.Length); //The part of the key, that was not found in the previous method.
            allNodes.Add(lastNode);

            while (corrections.Count <= amount && corrections.Count <= trie.Count)
            {
                foreach (var node in QuickSort<T>.Sort(allNodes.ToArray<Node<T>>(), SortRegime.ByWeight))
                {
                    foreach (var subnode in QuickSort<T>.Sort(node.GetSubnodes().ToArray<Node<T>>(), SortRegime.ByWeight))
                    {
                        if (endOfKey.Length == 1
                            && !corrections.Contains(subnode.Prefix)
                            && subnode.IsWord)
                        {
                            corrections.Add(subnode.Prefix);
                        }
                        else if (trie.SearchNode(endOfKey.Substring(1), subnode, out Node<T> nodeToReturn)
                            && !corrections.Contains(nodeToReturn.Prefix)
                            && nodeToReturn.IsWord
                            && GrammarChecker.CompareWords(nodeToReturn.Prefix, word) >= percentage)
                        {
                            corrections.Add(nodeToReturn.Prefix);
                        }
                        if (corrections.Count > amount)
                            break;
                    }
                }

                foreach (var item in allNodes.ToList())
                {
                    allNodes.AddRange(item.GetSubnodes());
                    allNodes.Remove(item);
                }
            }

            if (nodeMistake is Mistake<T> mistake1)
            {
                mistake1.AddCorrectionRange(corrections);
                mistake1.Data.Weight++;
            }
            else
            {
                AddMistake(word, corrections);
            }

            return corrections;
        }
        public override string ToString()
        {
            return $"{LanguageName}";
        }
    }

    public class Language : Language<Data>
    {
        public Language(string languageName, string filePath) : base(languageName, filePath)
        {

        }

        public Language(string languageName, Trie<Data> trie) : base(languageName, trie)
        {

        }
    }
}