using System;
using System.Collections.Generic;
using System.Linq;

namespace GrammarChecker
{
    public class Mistake<T> : Node<T>
        where T : Data
    {
        private List<Node<T>> Corrections = new List<Node<T>>();
        public int MinimalCorrectionsAmount { get; set; }
        public Language<T> Language { get; set; }

        //If you add weight to the Node<T>, it will be added here too.
        public Mistake(char symbol, T data, string prefix, Language<T> language, IEnumerable<Node<T>> corrections, int minimalCorrectionsAmount = 5) : base(symbol, data, prefix)
        {
            if (minimalCorrectionsAmount > 0)
                MinimalCorrectionsAmount = minimalCorrectionsAmount;
            else throw new ArgumentException("Minimal Corrections Amount has to be more than null");

            if (language != null)
                Language = language;
            else throw new ArgumentNullException("Language cannot be null.");

            foreach (var item in corrections)
            {
                if (item.IsWord)
                    Corrections.Add(item);
                else throw new ArgumentException("Corrections has to be a last node of the word");
            }

            FindCorrections();
        }
        public Mistake(Node<T> node, Language<T> language, IEnumerable<Node<T>> corrections, int minimalCorrectionsAmount = 5) : base(node.Symbol, node.Data, node.Prefix)
        {
            if (minimalCorrectionsAmount > 0)
                MinimalCorrectionsAmount = minimalCorrectionsAmount;
            else throw new ArgumentException("Minimal Corrections Amount has to be more than null");

            if (language != null)
                Language = language;
            else throw new ArgumentNullException("Language cannot be null.");

            foreach (var item in corrections)
            {
                if (item.IsWord)
                    Corrections.Add(item);
                else throw new ArgumentException("Corrections has to be a last node of the word");
            }

            FindCorrections();
        }
        public Mistake(Node<T> node, Language<T> language, int minimalCorrectionsAmount = 5) : base(node.Symbol, node.Data, node.Prefix)
        {
            if (minimalCorrectionsAmount > 0)
                MinimalCorrectionsAmount = minimalCorrectionsAmount;
            else throw new ArgumentException("Minimal Corrections Amount has to be more than null");

            if (language != null)
                Language = language;
            else throw new ArgumentNullException("Language cannot be null.");

            FindCorrections();
        }
        public Mistake(char symbol, T data, string prefix, Language<T> language, int minimalCorrectionsAmount = 5) : base(symbol, data, prefix)
        {
            if (minimalCorrectionsAmount > 0)
                MinimalCorrectionsAmount = minimalCorrectionsAmount;
            else throw new ArgumentException("Minimal Corrections Amount has to be more than null");

            if (language != null)
                Language = language;
            else throw new ArgumentNullException("Language cannot be null.");

            FindCorrections();
        }

        public List<Node<T>> GetCorrections(int amount, SortRegime sortRegime)
        {
            return QuickSort<T>.Sort(Corrections.ToArray(), sortRegime).Take(amount).ToList();
        }
        public void AddCorrection(Node<T> correction)
        {
            if (correction.IsWord)
                Corrections.Add(correction);
        }
        public void AddCorrection(string correction)
        {
            var newNode = new Node<T>(correction[correction.Length - 1], Data, correction);
            newNode.IsWord = true;
            AddCorrection(newNode);
        }
        public void AddCorrectionRange(IEnumerable<Node<T>> corrections)
        {
            foreach (var item in corrections)
            {
                AddCorrection(item);
            }
        }
        public void AddCorrectionRange(IEnumerable<string> corrections)
        {
            foreach (var item in corrections)
            {
                AddCorrection(item);
            }
        }
        public void FindCorrections()
        {
            int i = 0;
            while (Corrections.Count <= MinimalCorrectionsAmount)
            {
                foreach (var correction in GrammarChecker.GetSpellCorrections(Prefix, Language, MinimalCorrectionsAmount + i))
                {
                    if (Corrections.FirstOrDefault(x => x.Prefix == correction) != default(Node<T>))
                    {
                        AddCorrection(correction);
                    }
                }
                i += MinimalCorrectionsAmount - Corrections.Count;
            }
        }
        public void AddWeight(string correction)
        {
            AddWeight(correction, 1);
        }
        public void AddWeight(string correction, int weight)
        {
            var node = Corrections.FirstOrDefault(x => x.Prefix == correction);
            if (node != default(Node<T>))
            {
                node.Data.Weight += weight;
                return;
            }
            else
            {
                var newNode = new Node<T>(correction.Last(), Data, correction);
                newNode.IsWord = true;
                AddCorrection(newNode);
            }
        }

        public override string ToString()
        {
            return $"Mistake: {Data} [{SubNodes.Count}] ({Prefix})";
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}