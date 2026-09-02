#pragma warning disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2CppReloaded.Gameplay;

internal class PriorityVar<T>
{
    private T _value;
    private float _currentPriority = int.MinValue;

    /// <summary>
    /// Compares a value with the specified priority. If the provided priority is higher
    /// than the current priority, the value is stored and the priority is updated.
    /// </summary>
    /// <param name="value">The value to add</param>
    /// <param name="priority">The priority level of the value. Higher values have higher priority.</param>
    internal void Compare(T value, float priority = 0)
    {
        if (_currentPriority < priority)
        {
            _currentPriority = priority;
            _value = value;
        }
    }

    /// <summary>
    /// Gets the currently stored value, which is the one with the highest priority seen so far.
    /// </summary>
    /// <returns>The value with the highest priority, or null if no value has been added.</returns>
    internal T Get()
    {
        return _value;
    }

    /// <summary>
    /// Attempts to get the currently stored value.
    /// </summary>
    /// <param name="value">
    /// When this method returns, contains the value with the highest priority if available;
    /// otherwise, null. This parameter is passed uninitialized.
    /// </param>
    /// <returns>true if a value has been added and is available; otherwise, false.</returns>
    internal bool TryGet(out T value)
    {
        if (_value != null)
        {
            value = _value;
            return true;
        }

        value = default;
        return false;
    }
}

    internal static class GetZombiez
    {
        internal static T[] GetItems<T>(this DataArray<T> data) where T : class, new()
        {
            var enumerator = data.m_itemLookup.Keys.GetEnumerator();
            var array = new T[data.m_itemLookup.Count];
            var count = 0;
            while (enumerator.MoveNext())
            {
                array[count] = enumerator.Current;
                count++;
            }
            enumerator.Dispose();
            return array;
        }

        internal static Zombie[] GetZombies(this Board board)
        {
            return board.m_zombies.GetItems();
        }
    }

