using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginTwo.Classes
{
    public class Comparison
    {
        public static bool CompareDictionaries<TKey, TValue>(
            Dictionary<TKey,TValue> dict1, 
            Dictionary<TKey,TValue> dict2
        ){
            if (dict1 == dict2) return true;
            if ((dict1 == null) || (dict2 == null)) return false;
            if (dict1.Count != dict2.Count) return false;

            EqualityComparer<TValue> valueComparer = EqualityComparer<TValue>.Default;

            foreach(KeyValuePair<TKey,TValue> kvp in dict1)
            {
                TValue value;
                if (!dict2.TryGetValue(kvp.Key, out value)) return false;
                if (!valueComparer.Equals(kvp.Value, value)) return false;
            }

            return true;
        }
    }
}
