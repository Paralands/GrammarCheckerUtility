using System;
using System.Collections.Generic;
using System.Linq;

namespace GrammarChecker
{
    /// <summary>
    /// Trie for saving words data.
    /// </summary>
    /// <typeparam name="T">Type of data that will be saved in nodes.</typeparam>
    public class Trie<T>
    {
        /// <summary>
        /// The root node.
        /// </summary>
        public Node<T> Root { get; set; }
        /// <summary>
        /// Count of words.
        /// </summary>
        public int Count { get; private set; }
        /// <summary>
        /// Creates a trie.
        /// </summary>
        /// <param name="root">A root node.</param>
        public Trie(Node<T> root)
        {
            if (!root.Equals(null))
            {
                Root = root;
                Count = 0;
            }
            else throw new ArgumentNullException("Argument is null");
        }
        /// <summary>
        /// Adding a key.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="data">Data.</param>
        public void Add(string key, T data = default(T))
        {
            if (!string.IsNullOrEmpty(key))
            {
                AddNode(key, data, Root);
            }
            else throw new ArgumentNullException("Argument is null");
        }
        /// <summary>
        /// Adding list with default data.
        /// </summary>
        /// <param name="list">List of keys.</param>
        public void AddRange(IEnumerable<string> list)
        {
            foreach (string item in list)
            {
                Add(item);
            }
        }
        /// <summary>
        /// Removing a key.
        /// </summary>
        /// <param name="key">Key.</param>
        public void Remove(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                RemoveNode(key, Root, null);
            }
            else throw new ArgumentNullException("Argument is null");
        }
        /// <summary>
        /// Removing keys.
        /// </summary>
        /// <param name="list">List of keys.</param>
        public void RemoveRange(IEnumerable<string> list)
        {
            foreach (string item in list)
            {
                Remove(item);
            }
        }
        /// <summary>
        /// Searches for a key.
        /// </summary>
        /// <param name="key">A key.</param>
        /// <param name="value">Value of the last node.</param>
        /// <returns>True if nodes with this key exists.</returns>
        public bool TrySearch(string key, out T value)
        {
            return SearchNode(key, Root, out value);
        }
        /// <summary>
        /// Searches for a key.
        /// </summary>
        /// <param name="key">A key.</param>
        /// <returns>True if nodes with this key exists.</returns>
        public bool TrySearch(string key)
        {
            return SearchNode(key, Root, out T value);
        }
        /// <summary>
        /// Searches for a key.
        /// </summary>
        /// <param name="key">A key.</param>
        /// <param name="node">Node, which was the last node before the error happened in case that key was not found, else the last node.</param>
        /// <returns>True if nodes with this key exists.</returns>
        public bool TrySearch(string key, out Node<T> node)
        {
            node = SearchNodeUntilError(key, Root);
            if (node != null && node.Prefix.Equals(key))
            {
                return true;
            }
            else return false;
        }
        /// <summary>
        /// Searches for keys and their data.
        /// </summary>
        /// <param name="list">List of keys.</param>
        /// <param name="data">Data.</param>
        /// <returns>Dictionary keys-bool (true if the key was found, false if not) and dictionary keys-data.</returns>
        public Dictionary<string, bool> TrySearchRange(IEnumerable<string> list, out Dictionary<string, T> data)
        {
            data = new Dictionary<string, T>();

            var answer = new Dictionary<string, bool>();
            foreach (string item in list)
            {
                var search = TrySearch(item, out T value);
                answer.Add(item, search);
                data.Add(item, value);
            }
            return answer;
        }
        public IEnumerable<string> TrySearchOptions(string key, int amount)
        {
            if (key == null)
            {
                throw new ArgumentNullException("Argument word cannot be null.");
            }
            if (TrySearch(key, out T value))
            {
                return null;
            }

            var list = new List<string>();
            var allNodes = new List<Node<T>>();

            var lastNode = SearchNodeUntilError(key, Root);
            var endOfKey = key.Substring(lastNode.Prefix.Length); //The part of the key, that was not found in the previous method.
            allNodes.Add(lastNode);

            int i = 1;
            while (list.Count <= amount && i <= endOfKey.Length)
            {
                foreach (var node in allNodes)
                {
                    foreach (var subnode in node.GetSubnodes())
                    {
                        if (SearchNode(endOfKey.Substring(i), subnode, out Node<T> nodeToReturn))
                        {
                            list.Add(nodeToReturn.Prefix);
                            if (list.Count > amount)
                                break;
                        }
                    }
                    if (list.Count > amount)
                        break;
                }

                foreach (var item in allNodes.ToList())
                {
                    allNodes.AddRange(item.GetSubnodes());
                    allNodes.Remove(item);
                }
            }

            return list;
        }
        private void RemoveNode(string key, Node<T> node, List<Node<T>> remove)
        {
            List<Node<T>> to_remove;
            if (remove != null)
            {
                to_remove = new List<Node<T>>(remove);
            }
            else
            {
                to_remove = new List<Node<T>>();
            }

            var symbol = key[0];
            var subnode = node.TryFind(symbol);

            if (subnode is null)
            {
                return;
            }

            if (subnode.IsWord && key.Length <= 1)
            {
                subnode.IsWord = false;
                Count--;
                if (subnode.SubNodes.Count == 0)
                {
                    foreach (Node<T> item in to_remove)
                    {
                        if (item.SubNodes.Count > 1)
                        {
                            item.SubNodes.Remove(symbol);
                        }
                        else
                        {
                            item.SubNodes.Clear();
                        }
                    }
                }
                return;
            }

            if (subnode.SubNodes.Count > 1)
            {
                to_remove.Clear();
            }
            to_remove.Add(subnode);
            RemoveNode(key.Substring(1), subnode, to_remove);
        }
        private void RemoveNode(Node<T> nodeToRemove, Node<T> node)
        {
            SearchNode(new String(nodeToRemove.Prefix.Take(nodeToRemove.Prefix.Length - 1).ToArray()), node, out Node<T> nodeToReturn);
            if (nodeToReturn != null)
            {
                nodeToReturn.SubNodes.Remove(nodeToRemove.Symbol);
            }
        }
        private void AddNode(string key, T data, Node<T> node)
        {
            if (string.IsNullOrEmpty(key))
            {
                node.Data = data;
                if (!node.IsWord)
                {
                    node.IsWord = true;
                    Count++;
                }
            }
            else
            {
                var subnode = node.TryFind(key[0]);
                if (subnode != null)
                {
                    AddNode(key.Substring(1), data, subnode);
                }
                else
                {
                    var newNode = new Node<T>(key[0], data, node.Prefix + key[0].ToString());
                    node.SubNodes.Add(key[0], newNode);
                    AddNode(key.Substring(1), data, newNode);
                }
            }
        }
        private void AddNode(Node<T> newNode, Node<T> node)
        {
            if (newNode == null || node == null)
            {
                throw new ArgumentNullException("newNode and node arguments cannot be null");
            }

            var subnodes = node.GetSubnodes();
            var symbolSubnode = subnodes.FirstOrDefault(x => x.Symbol.Equals(newNode.Symbol));

            if (symbolSubnode == default(Node<T>))
            {
                node.SubNodes.Add(newNode.Symbol, newNode);
                return;
            }
            else
            {
                symbolSubnode.IsWord = (newNode.IsWord || symbolSubnode.IsWord);
                foreach (var item in newNode.GetSubnodes())
                {
                    AddNode(item, symbolSubnode);
                }
            }
        }
        /// <summary>
        /// Searches for a node beginning from a different node than root.
        /// </summary>
        /// <param name="key">A key.</param>
        /// <param name="node">A node from which the search starts.</param>
        /// <param name="nodeToReturn">A last node; if method returns false, nodeToReturn is null.</param>
        /// <returns>True, if the node is found and it is the end of a word.</returns>
        public bool SearchNode(string key, Node<T> node, out Node<T> nodeToReturn)
        {
            nodeToReturn = node;
            if (string.IsNullOrEmpty(key))
            {
                return node.IsWord;
            }
            else
            {
                var symbol = key[0];
                var subnode = node.TryFind(symbol);
                nodeToReturn = subnode;
                if (subnode != null)
                {
                    return SearchNode(key.Substring(1), subnode, out nodeToReturn);
                }
            }
            nodeToReturn = null;
            return false;
        }
        /// <summary>
        /// Searches for a node.
        /// </summary>
        /// <param name="key">A key.</param>

        /// <param name="node">A node from which the search starts.</param>
        /// <param name="value">A data of the last node.</param>
        /// <returns>True, if the node is found and it is the end of a word.</returns>
        public bool SearchNode(string key, Node<T> node, out T value)
        {
            value = default(T);
            if (string.IsNullOrEmpty(key))
            {
                if (node.IsWord)
                {
                    value = node.Data;
                    return true;
                }
                else return false;
            }
            else
            {
                var symbol = key[0];
                var subnode = node.TryFind(symbol);
                if (subnode != null)
                {
                    return SearchNode(key.Substring(1), subnode, out value);
                }
            }
            return false;
        }
        /// <summary>
        /// Searches for the key until the error.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="node"></param>
        /// <returns>Node, that caused an error.</returns>
        private Node<T> SearchNodeUntilError(string key, Node<T> node)
        {
            Node<T> currentNode = node;
            if (string.IsNullOrEmpty(key))
            {
                if (node.IsWord)
                {
                    return currentNode;
                }
            }
            else
            {
                var symbol = key[0];
                var subnode = node.TryFind(symbol);
                if (subnode != null)
                {
                    return SearchNodeUntilError(key.Substring(1), subnode);
                }
            }
            return currentNode;
        }
        private void UpdatePrefix(Node<T> newNodeBefore, Node<T> nodeToChange)
        {
            var prefix = nodeToChange.Prefix;
            nodeToChange.Prefix = prefix.Substring(prefix.Length - 2) + newNodeBefore.Symbol + nodeToChange.Symbol;
        }
        public bool TryReplaceNode(Node<T> nodeToReplace, Node<T> newNode, bool copySubnodes = true)
        {
            if (nodeToReplace == null  || newNode == null  || nodeToReplace.Symbol != newNode.Symbol)
                return false;

            var key = nodeToReplace.Prefix;
            TrySearch(key.Remove(key.Length - 1), out Node<T> nodeBefore);

            if (nodeBefore == null)
                return false;

            nodeBefore.SubNodes.Remove(key[key.Length - 1]);
            nodeBefore.SubNodes.Add(newNode.Prefix[newNode.Prefix.Length - 1], newNode);
            newNode.Prefix = key.Substring(key.Length - 1) + newNode.Symbol;

            if (copySubnodes)
            {
                foreach (var subnode in nodeToReplace.SubNodes)
                {
                    if (!newNode.SubNodes.ContainsKey(subnode.Key))
                    {
                        newNode.SubNodes.Add(subnode.Key, subnode.Value);
                        UpdatePrefix(newNode, subnode.Value);
                    }
                }
                //TODO: Change all prefixes of subnodes of subnodes   
            }
            return true;
        }
        /// <summary>
        /// Adds a trie.
        /// </summary>
        /// <param name="trie">A trie that will be added to this trie.</param>
        public void AddTrie(Trie<T> trie)
        {
            if (trie.Count != 0)
            {
                foreach (var item in trie.Root.GetSubnodes())
                {
                    AddNode(item, Root);
                }
            }
            else throw new ArgumentNullException("Trie has to contain more than 1 root-node");
        }
        /// <summary>
        /// Removes a trie.
        /// </summary>
        /// <param name="trie">A trie that will be removed to this trie.</param>
        public void RemoveTrie(Trie<T> trie)
        {
            if (trie.Count != 0)
            {
                foreach (var item in trie.Root.GetSubnodes())
                {
                    RemoveNode(item, Root);
                }
            }
            else throw new ArgumentNullException("Trie has to contain more than 1 root-node");
        }
        public override string ToString()
        {
            return base.ToString() + Count;
        }
    }
}