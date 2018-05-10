using System;
using System.Collections.Generic;

namespace Auxiliary
{
    /// <summary>
    /// Basically an improved list that provides access to the entire array, but provides methods for Push, Pop and Peek. However, stack pop operation is O(n). Use this structure only for small collections.
    /// </summary>
    /// <typeparam name="T">The type of elements on the stack.</typeparam>
    public class ImprovedStack<T>: List<T>
    {
        /// <summary>
        /// Returns the top of the stack, without removing it.
        /// </summary>
        public T Peek()
        {
            if (this.Count > 0) return this[this.Count - 1]; else return default(T);
        }
        /// <summary>
        /// Pushes the item to the top of the stack.
        /// </summary>
        /// <param name="t">Item to push on top of stack.</param>
        public void Push(T t)
        {
            this.Add(t);
        }

        /// <summary>
        /// Returns the item at the top of the stack and removes it, or returns null or throws an exception if the stack is empty.
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            if (this.Count > 0)
            {
                T t = this[this.Count - 1];
                this.Remove(t);
                return t;
            }
            else
            {
                throw new InvalidOperationException("The Improved Stack is empty, yet its Pop() method was called.");
            }
        }
    }
}
