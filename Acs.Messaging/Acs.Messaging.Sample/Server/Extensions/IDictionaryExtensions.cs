namespace Acs.Messaging.Sample.Server.Extensions;

public static class IDictionaryExtensions
{
    public static TValue? Get<TKey, TValue>(this IDictionary<TKey, TValue>? dictionary, TKey key, TValue? defaultValue = default)
    {
        if (dictionary != null && dictionary.TryGetValue(key, out TValue? value))
            return value;
        else
            return defaultValue;
    }

    public static object? Get(this IDictionary<string, object>? dictionary, string key, object? defaultValue = null)
    {
        return dictionary?.Get<string, object>(key, defaultValue) ?? defaultValue;
    }
}