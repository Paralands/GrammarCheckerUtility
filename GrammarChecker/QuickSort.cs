using System;
using System.Linq;

namespace GrammarChecker
{
    public static class QuickSort<T>
        where T : Data
    {
        private static void Swap(ref Node<T> x, ref Node<T> y)
        {
            var t = x;
            x = y;
            y = t;
        }

        private static int PartitionByWeight(Node<T>[] array, int minIndex, int maxIndex)
        {
            var pivot = minIndex - 1;
            for (var i = minIndex; i < maxIndex; i++)
            {
                if (array[i].Data.Weight < array[maxIndex].Data.Weight)
                {
                    pivot++;
                    Swap(ref array[pivot], ref array[i]);
                }
            }

            pivot++;
            Swap(ref array[pivot], ref array[maxIndex]);
            return pivot;
        }
        private static Node<T>[] SortByWeight(Node<T>[] array, int minIndex, int maxIndex)
        {
            if (minIndex >= maxIndex)
            {
                return array;
            }

            var pivotIndex = PartitionByWeight(array, minIndex, maxIndex);
            SortByWeight(array, minIndex, pivotIndex - 1);
            SortByWeight(array, pivotIndex + 1, maxIndex);

            return array;
        }
        private static int PartitionBySubnodes(Node<T>[] array, int minIndex, int maxIndex)
        {
            var pivot = minIndex - 1;
            for (var i = minIndex; i < maxIndex; i++)
            {
                if (array[i].SubNodes.Count < array[maxIndex].SubNodes.Count)
                {
                    pivot++;
                    Swap(ref array[pivot], ref array[i]);
                }
            }

            pivot++;
            Swap(ref array[pivot], ref array[maxIndex]);
            return pivot;
        }
        private static Node<T>[] SortBySubnodes(Node<T>[] array, int minIndex, int maxIndex)
        {
            if (minIndex >= maxIndex)
            {
                return array;
            }

            var pivotIndex = PartitionBySubnodes(array, minIndex, maxIndex);
            SortBySubnodes(array, minIndex, pivotIndex - 1);
            SortBySubnodes(array, pivotIndex + 1, maxIndex);

            return array;
        }

        public static Node<T>[] Sort(Node<T>[] array, SortRegime sortRegime)
        {
            var clearArray = array.ToList();
            clearArray.RemoveAll(x => x == null);
            if (clearArray.Count == 1)
                return clearArray.ToArray();
            if (sortRegime == SortRegime.BySubnodes)
            {
                return SortBySubnodes(clearArray.ToArray(), 0, clearArray.Count - 1).Reverse().ToArray();
            }
            return SortByWeight(clearArray.ToArray(), 0, clearArray.Count - 1).Reverse().ToArray();
        }
    }

    public enum SortRegime
    {
        ByWeight,
        BySubnodes
    }
}