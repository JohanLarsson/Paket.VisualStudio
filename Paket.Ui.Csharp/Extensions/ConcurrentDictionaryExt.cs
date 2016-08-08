﻿namespace Paket.Ui.Csharp
{
    using System.Collections.Concurrent;

    internal static class ConcurrentDictionaryExt
    {
        internal static TValue GetValueOrDefault<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key)
        {
            if (key == null)
            {
                return default(TValue);
            }

            TValue value;
            return dict.TryGetValue(key, out value) ? value : default(TValue);
        }
    }
}
