using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
public class KVPair<K,V>
{

    public K Key;
    public V Value;

    public override string ToString()
    {
        return "[KVPair]Key:" + Key + "_Value:" + Value;
    }
}
