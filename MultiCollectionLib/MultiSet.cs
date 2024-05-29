using MultiSetLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSetLib
{
    public class MultiSet<T> : IMultiSet<T>
    {
        private readonly Dictionary<T, int> _elements;
        private readonly IEqualityComparer<T> _comparer;

        public MultiSet() : this(EqualityComparer<T>.Default) { }

        public MultiSet(IEqualityComparer<T> comparer)
        {
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _elements = new Dictionary<T, int>(_comparer);
        }

        public MultiSet(IEnumerable<T> sequence) : this(sequence, EqualityComparer<T>.Default) { }

        public MultiSet(IEnumerable<T> sequence, IEqualityComparer<T> comparer) : this(comparer)
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));
            foreach (var item in sequence)
            {
                Add(item);
            }
        }

        public int Count => _elements.Values.Sum();

        public bool IsReadOnly => false;

        public bool IsEmpty => _elements.Count == 0;

        public IEqualityComparer<T> Comparer => _comparer;

        public int this[T item] => _elements.TryGetValue(item, out var count) ? count : 0;

        public void Add(T item)
        {
            Add(item, 1);
        }

        public MultiSet<T> Add(T item, int numberOfItems = 1)
        {
            if (IsReadOnly) throw new NotSupportedException("Multiset is read-only");
            if (numberOfItems <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfItems), "Number of items must be positive");

            if (_elements.TryGetValue(item, out var count))
            {
                _elements[item] = count + numberOfItems;
            }
            else
            {
                _elements[item] = numberOfItems;
            }

            return this;
        }

        public bool Remove(T item)
        {
            return Remove(item, 1) != this;
        }

        public MultiSet<T> Remove(T item, int numberOfItems = 1)
        {
            if (IsReadOnly) throw new NotSupportedException("Multiset is read-only");
            if (numberOfItems <= 0) throw new ArgumentOutOfRangeException(nameof(numberOfItems), "Number of items must be positive");

            if (_elements.TryGetValue(item, out var count))
            {
                if (numberOfItems >= count)
                {
                    _elements.Remove(item);
                }
                else
                {
                    _elements[item] = count - numberOfItems;
                }
            }

            return this;
        }

        public MultiSet<T> RemoveAll(T item)
        {
            if (IsReadOnly) throw new NotSupportedException("Multiset is read-only");
            _elements.Remove(item);
            return this;
        }

        public void Clear()
        {
            if (IsReadOnly) throw new NotSupportedException("Multiset is read-only");
            _elements.Clear();
        }

        public bool Contains(T item)
        {
            return _elements.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex > array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count) throw new ArgumentException("The array does not have enough space to copy the elements.");

            foreach (var kvp in _elements)
            {
                for (int i = 0; i < kvp.Value; i++)
                {
                    array[arrayIndex++] = kvp.Key;
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var kvp in _elements)
            {
                for (int i = 0; i < kvp.Value; i++)
                {
                    yield return kvp.Key;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public MultiSet<T> UnionWith(IEnumerable<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            foreach (var item in other)
            {
                Add(item);
            }
            return this;
        }

        public MultiSet<T> IntersectWith(IEnumerable<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            var otherMultiSet = new MultiSet<T>(other, _comparer);
            var keysToRemove = _elements.Keys.Where(key => !otherMultiSet.Contains(key)).ToList();

            foreach (var key in keysToRemove)
            {
                _elements.Remove(key);
            }

            foreach (var key in _elements.Keys.ToList())
            {
                _elements[key] = Math.Min(_elements[key], otherMultiSet[key]);
                if (_elements[key] == 0)
                {
                    _elements.Remove(key);
                }
            }

            return this;
        }

        public MultiSet<T> ExceptWith(IEnumerable<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            foreach (var item in other)
            {
                Remove(item);
            }

            return this;
        }

        public MultiSet<T> SymmetricExceptWith(IEnumerable<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            var otherMultiSet = new MultiSet<T>(other, _comparer);

            foreach (var item in otherMultiSet)
            {
                if (Contains(item))
                {
                    int minCount = Math.Min(this[item], otherMultiSet[item]);
                    Remove(item, minCount);
                    otherMultiSet.Remove(item, minCount);
                }
            }

            foreach (var item in otherMultiSet)
            {
                Add(item);
            }

            return this;
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            var otherMultiSet = new MultiSet<T>(other, _comparer);

            foreach (var kvp in _elements)
            {
                if (kvp.Value > otherMultiSet[kvp.Key])
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            var otherMultiSet = new MultiSet<T>(other);
            bool isProper = false;

            foreach (var kvp in _elements)
            {
                if (kvp.Value > otherMultiSet[kvp.Key])
                {
                    return false;
                }
                if (kvp.Value < otherMultiSet[kvp.Key])
                {
                    isProper = true;
                }
            }

            return isProper && otherMultiSet.Count > Count;
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            var otherMultiSet = new MultiSet<T>(other, _comparer);

            foreach (var item in otherMultiSet)
            {
                if (!Contains(item))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            var otherMultiSet = new MultiSet<T>(other, _comparer);
            bool isProper = false;

            foreach (var item in otherMultiSet)
            {
                if (!Contains(item))
                {
                    return false;
                }
                if (otherMultiSet[item] < this[item])
                {
                    isProper = true;
                }
            }

            return isProper && Count > otherMultiSet.Count;
        }


        public bool Overlaps(IEnumerable<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            var otherMultiSet = new MultiSet<T>(other, _comparer);

            foreach (var kvp in _elements)
            {
                if (otherMultiSet.Contains(kvp.Key))
                {
                    return true;
                }
            }

            return false;
        }

        public bool MultiSetEquals(IEnumerable<T> other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));

            var otherMultiSet = new MultiSet<T>(other, _comparer);

            if (Count != otherMultiSet.Count)
            {
                return false;
            }

            foreach (var kvp in _elements)
            {
                if (kvp.Value != otherMultiSet[kvp.Key])
                {
                    return false;
                }
            }

            return true;
        }

        public static IMultiSet<T> operator +(MultiSet<T> first, MultiSet<T> second)
        {
            if (first == null || second == null) throw new ArgumentNullException();

            var result = new MultiSet<T>(first._comparer);

            foreach (var item in first)
            {
                result.Add(item, first[item]);
            }

            foreach (var item in second)
            {
                result.Add(item, second[item]);
            }

            return result;
        }

        public static IMultiSet<T> operator -(MultiSet<T> first, MultiSet<T> second)
        {
            if (first == null || second == null) throw new ArgumentNullException();

            var result = new MultiSet<T>(first._comparer);

            foreach (var item in first)
            {
                result.Add(item, first[item]);
            }

            foreach (var item in second)
            {
                result.Remove(item, second[item]);
            }

            return result;
        }

        public static IMultiSet<T> operator *(MultiSet<T> first, MultiSet<T> second)
        {
            if (first == null || second == null) throw new ArgumentNullException();

            var result = new MultiSet<T>(first._comparer);

            foreach (var item in first)
            {
                if (second.Contains(item))
                {
                    result.Add(item, Math.Min(first[item], second[item]));
                }
            }

            return result;
        }

        public IReadOnlySet<T> AsSet()
        {
            return new HashSet<T>(_elements.Keys, _comparer);
        }

        public IReadOnlyDictionary<T, int> AsDictionary()
        {
            return new Dictionary<T, int>(_elements, _comparer);
        }
    }
}
