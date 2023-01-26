using System.Collections.Generic;
using UnityEngine;

/* It's a Dictionary that can be serialized by Unity */
[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{

    [SerializeField] private List<TKey> keys = new ();
    [SerializeField] private List<TValue> values = new ();

/// <summary>
/// It clears the keys and values lists, then adds the keys and values from the dictionary to the lists.
/// </summary>
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

/// <summary>
/// > This function is called after the dictionary is deserialized. It clears the dictionary and then
/// adds all the keys and values from the serialized lists
/// </summary>
    public void OnAfterDeserialize()
    {
        this.Clear();

        if (keys.Count != values.Count)
        {
            Debug.LogError("Tried to deserialize a SerializableDictionary, but the amount of keys ("
                + keys.Count + ") does not match the number of values (" + values.Count
                + ") which indicates that something went wrong");
        }

        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }
}