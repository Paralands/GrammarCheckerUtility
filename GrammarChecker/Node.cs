using System;
using System.Collections.Generic;

namespace GrammarChecker
{
    public class Node<T>
    {
        public T Data { get; set; }
        public char Symbol { get; private set; }
        public bool IsWord { get; set; }
        public string Prefix { get; set; }

        public Dictionary<char, Node<T>> SubNodes { get; set; }

        /// <summary>
        /// Creating a Node.
        /// </summary>
        /// <param name="symbol">Symbol that node has.</param>
        /// <param name="data">Data in the node.</param>
        /// <param name="prefix">All the nodes that were before in the trie.</param>
        public Node(char symbol, T data, string prefix)
        {
            Symbol = symbol;
            Data = data;

            if (prefix != null)
            {
                Prefix = prefix;
            }
            else throw new ArgumentException("string prefix cannot be null or empty!");

            SubNodes = new Dictionary<char, Node<T>>();
        }
        public Node<T> TryFind(char symbol)
        {
            if (SubNodes.TryGetValue(symbol, out Node<T> value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }
        public IEnumerable<Node<T>> GetSubnodes()
        {
            foreach (var key in SubNodes.Keys)
            {
                SubNodes.TryGetValue(key, out Node<T> value);
                yield return value;
            }
        }
        public Node<T>[] GetArrayOfSubnodes()
        {
            var keys = SubNodes.Keys;
            var array = new Node<T>[keys.Count];

            int i = 0;
            foreach (var key in keys)
            {
                SubNodes.TryGetValue(key, out Node<T> node);
                array[i] = node;
            }

            return array;
        }

        public override bool Equals(object obj)
        {
            if (obj is Node<T> node)
            {
                return this.Symbol.Equals(node.Symbol);
            }
            else if (obj is null && this is null)
            {
                return true;
            }
            else return false;
        }
        public override string ToString()
        {
            return $"[{SubNodes.Count}] ({Prefix})";
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public static bool operator ==(Node<T> node1, Node<T> node2)
        {
            if (node1 is null && node2 is null)
            {
                return true;
            }
            else if (!(node1 is null) && !(node2 is null))
            {
                return node1.Symbol.Equals(node2.Symbol);
            }
            else return false;
        }
        public static bool operator !=(Node<T> node1, Node<T> node2)
        {
            if (node1 is null && node2 is null)
            {
                return false;
            }
            else if (!(node1 is null) && !(node2 is null))
            {
                return !node1.Symbol.Equals(node2.Symbol);
            }
            else return true;
        }
    }
}