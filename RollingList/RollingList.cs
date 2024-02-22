using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AG.Collections
{
    /// <summary>
    /// A list with limited capacity holding items of type <typeparamref name="T"/>.
    /// Adding further items will result in the list rolling over.
    /// </summary>
    /// <remarks>
    /// Implemented as a circular list using a <see cref="List{T}"/> internally.
    /// Insertions and Removals are not supported.
    /// Not thread-safe.
    /// </remarks>
    public class RollingList<T> : IList<T>
    {
        private List<T> _items;
        private int _size;
        private int _firstIndex;

        public int Count => this._items.Count;

        /// <summary>Internal list capacity</summary>
        public int Capacity
        {
            get => this._items.Capacity;
            set => this._items.Capacity = Math.Min(value, this._size);
        }

        /// <summary>Rolling list size</summary>
        public int Size
        {
            get => this._size;
            set
            {
                if (value == this._size) return;
                if (value > this._size)
                {
                    if (this._firstIndex > 0)
                    {
                        this._items = new List<T>(this);
                        this._firstIndex = 0;
                    }
                }
                else // value < this._size
                {
                    ThrowHelper.ThrowArgumentOutOfRangeExceptionIfLessThan(nameof(value), value, 0);
                    if (value < this.Count)
                    {
                        this._items = new List<T>(this.TakeLast(value));
                        this._firstIndex = 0;
                    }
                }
                this._size = value;
            }
        }

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionIfGreaterThanOrEqual(nameof(index), index, this.Count);
                ThrowHelper.ThrowArgumentOutOfRangeExceptionIfLessThan(nameof(index), index, 0);
                return this._items[this.GetRealIndex(index)];
            }
            set
            {
                ThrowHelper.ThrowArgumentOutOfRangeExceptionIfGreaterThanOrEqual(nameof(index), index, this.Count);
                ThrowHelper.ThrowArgumentOutOfRangeExceptionIfLessThan(nameof(index), index, 0);
                this._items[this.GetRealIndex(index)] = value;
            }
        }

        public RollingList(int size, int capacity)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionIfLessThan(nameof(size), size, 0);
            capacity = Math.Min(capacity, size);
            this._size = size;
            this._items = new List<T>(capacity);
        }

        public RollingList(int size)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionIfLessThan(nameof(size), size, 0);
            this._size = size;
            this._items = new();
        }

        public RollingList(IEnumerable<T> items, int size, int capacity)
        {
            if (items.TryGetNonEnumeratedCount(out var count) && count > capacity) capacity = count;
            capacity = Math.Min(capacity, size);
            this._size = size;
            this._items = new List<T>(capacity);
            this.AddRange(items);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetRealIndex(int index) => this._size > 0 ? (index + this._firstIndex) % this._size : 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetVirtualIndex(int index) => this._size > 0 ? (this._size + index - this._firstIndex) % this._size : 0;

        public void Add(T item)
        {
            if (this._size == 0) return;
            if (this._items.Count >= this._size)
            {
                this._items[this._firstIndex] = item;
                this._firstIndex = (this._firstIndex + 1) % this._size;
            }
            else
            {
                if (this._items.Count == this._items.Capacity)
                {
                    // Manual list capacity resize
                    var newCapacity = Math.Max(Math.Min(this._size, this._items.Capacity * 2), this._items.Capacity);
                    this._items.Capacity = newCapacity;
                }
                this._items.Add(item);
            }
            Debug.Assert(this._items.Count <= this._size);
        }

        public void AddRange(IEnumerable<T> items)
        {
            if (this._size == 0) return;
            foreach (var item in items) this.Add(item);
        }

        public void Clear()
        {
            this._items.Clear();
            this._firstIndex = 0;
        }

        public int IndexOf(T item)
        {
            var index = this._items.IndexOf(item);
            if (index == -1) return -1;
            return this.GetVirtualIndex(index);
        }

        public void Insert(int index, T item) => throw new NotSupportedException();

        public void RemoveAt(int index) => throw new NotSupportedException();

        public bool Contains(T item) => this._items.Contains(item);

        public void CopyTo(T[] array, int arrayIndex)
        {
            ThrowHelper.ThrowArgumentOutOfRangeExceptionIfLessThan(nameof(arrayIndex), arrayIndex, 0);
            if (array.Length - arrayIndex < this.Count) ThrowArgumentException("Not enough space");
            for (var index = 0; index < this.Count; index++)
            {
                array[arrayIndex++] = this[index];
            }
        }

        public bool Remove(T item) => throw new NotSupportedException();

        public IEnumerator<T> GetEnumerator()
        {
            for (var index = 0; index < this._items.Count; index++)
            {
                yield return this._items[this.GetRealIndex(index)];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        [DoesNotReturn]
        private static void ThrowArgumentException(string message) => throw new ArgumentException(message);
    }
}
