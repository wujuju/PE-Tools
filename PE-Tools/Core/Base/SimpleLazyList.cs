// dnlib: See LICENSE.txt for more info

using System;
using System.Diagnostics;
using System.Threading;

/// <summary>
/// A readonly list that gets initialized lazily
/// </summary>
/// <typeparam name="T">Any class type</typeparam>
[DebuggerDisplay("Count = {Length}")]
public class SimpleLazyList<T> where T : class
{
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    readonly T[] elements;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly Func<uint, T> readElementByRID;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly uint length;

    /// <summary>
    /// Gets the length of this list
    /// </summary>
    public uint Length => length;

    /// <summary>
    /// Access the list
    /// </summary>
    /// <param name="index">Index</param>
    /// <returns>The element or <c>null</c> if <paramref name="index"/> is invalid</returns>
    public T this[uint index]
    {
        get
        {
            if (index >= length)
                return null;
            if (elements[index] is null)
                Interlocked.CompareExchange(ref elements[index], readElementByRID(index + 1), null);
            return elements[index];
        }
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="length">Length of the list</param>
    /// <param name="readElementByRID">Delegate instance that lazily reads an element. It might
    /// be called more than once for each <c>rid</c> in rare cases. It must never return
    /// <c>null</c>.</param>
    public SimpleLazyList(uint length, Func<uint, T> readElementByRID)
    {
        this.length = length;
        this.readElementByRID = readElementByRID;
        elements = new T[length];
        for (uint i = 0; i < length; i++)
        {
            elements[i] = readElementByRID(i + 1);
        }
    }
}