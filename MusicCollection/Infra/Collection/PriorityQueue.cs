//**********************************************************
//* PriorityQueue                                          *
//* Copyright (c) Julian M Bucknall 2004                   *
//* All rights reserved.                                   *
//* This code can be used in your applications, providing  *
//*    that this copyright comment box remains as-is       *
//**********************************************************
//* .NET priority queue class (heap algorithm)             *
//**********************************************************
//Adapted by David Desmaisons : generic + IComparer + initial size


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MusicCollection.ToolBox.Maths;

namespace MusicCollection.Infra.Collection
{
    public class PriorityQueue<T> : IPriorityQueue<T>
    {
        private int _Count;
        private int _Capacity;
        private int _Version;
        private IComparer<T> _ItemComparer;
        private T[] _Heap;

        public IComparer<T> ItemComparer
        {
            get { return _ItemComparer; }
        }

        public PriorityQueue(IComparer<T> iItemComparer = null, int iInitialCapacity = 15)
        {
            double? CurrentMinimalCapacity = new SecondDegreeSolver(a: 1, b: 1, c: -iInitialCapacity * 2).GetMaxSolution();

            if (!CurrentMinimalCapacity.HasValue)
                throw new ArgumentException("iInitialCapacity");

            int Almost = (int)Math.Truncate(CurrentMinimalCapacity.Value);
            int Adjust = (CurrentMinimalCapacity.Value % 1 == 0) ? Almost : Almost + 1;

            _Capacity = ((Adjust) * (Adjust + 1)) / 2; // 15 is equal to 4 complete levels
            _Heap = new T[_Capacity];
            _ItemComparer = iItemComparer ?? Comparer<T>.Default;
        }

        public T Dequeue()
        {
            if (_Count == 0)
                throw new InvalidOperationException();

            T result = _Heap[0];
            _Count--;
            trickleDown(0, _Heap[_Count]);
            _Heap[_Count] = default(T);
            _Version++;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Peek()
        {
            if (_Count == 0)
                throw new InvalidOperationException();

            return _Heap[0];
        }

        public void Enqueue(T item)
        {
            if (_Count == _Capacity)
                growHeap();
            _Count++;
            bubbleUp(_Count - 1, item);
            _Version++;
        }

        private void bubbleUp(int index, T he)
        {
            int parent = getParent(index);
            // note: (index > 0) means there is a parent

            while ((index > 0) && (_ItemComparer.Compare(_Heap[parent], he) < 0))
            {
                _Heap[index] = _Heap[parent];
                index = parent;
                parent = getParent(index);
            }
            _Heap[index] = he;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int getLeftChild(int index)
        {
            return (index * 2) + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int getParent(int index)
        {
            return (index - 1) / 2;
        }

        private void growHeap()
        {
            _Capacity = (_Capacity * 2) + 1;
            T[] newHeap = new T[_Capacity];
            System.Array.Copy(_Heap, 0, newHeap, 0, _Count);
            _Heap = newHeap;
        }

        private void trickleDown(int index, T he)
        {
            int child = getLeftChild(index);
            while (child < _Count)
            {
                if (((child + 1) < _Count) &&
                    (_ItemComparer.Compare(_Heap[child], _Heap[child + 1]) < 0))
                {
                    child++;
                }
                _Heap[index] = _Heap[child];
                index = child;
                child = getLeftChild(index);
            }
            bubbleUp(index, he);
        }

        #region IEnumerable<T> implementation

        public IEnumerator GetEnumerator()
        {
            return new PriorityQueueEnumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new PriorityQueueEnumerator(this);
        }

        #endregion

        public int Count
        {
            get { return _Count; }
        }

        public void CopyTo(Array array, int index)
        {
            System.Array.Copy(_Heap, 0, array, index, _Count);
        }

        #region Priority Queue enumerator

        private class PriorityQueueEnumerator : IEnumerator<T>
        {
            private int index;
            private PriorityQueue<T> pq;
            private int version;

            public PriorityQueueEnumerator(PriorityQueue<T> pq)
            {
                this.pq = pq;
                Reset();
            }

            private void checkVersion()
            {
                if (version != pq._Version)
                    throw new InvalidOperationException();
            }

            #region IEnumerator<T> Members

            public void Reset()
            {
                index = -1;
                version = pq._Version;
            }

            public object Current
            {
                get
                {
                    checkVersion();
                    return pq._Heap[index];
                }
            }

            public bool MoveNext()
            {
                checkVersion();
                if (index + 1 == pq._Count)
                    return false;
                index++;
                return true;
            }

            T IEnumerator<T>.Current
            {
                get
                {
                    checkVersion();
                    return pq._Heap[index];
                }
            }

            public void Dispose()
            {
            }

            #endregion
        }

        #endregion

    }
}
