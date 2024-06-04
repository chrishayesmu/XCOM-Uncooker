using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCOM_Uncooker.Utils
{
    /// <summary>
    /// Very barebones implementation of a dictionary where each key can map to multiple values.
    /// </summary>
    public class MultiValueDictionary<K, V> : Dictionary<K, List<V>> where K : notnull
    {
        public MultiValueDictionary(): base() { }

        public MultiValueDictionary(int capacity): base(capacity) { }

        public int NumValues
        { 
            get
            {
                int count = 0;

                foreach (var valueList in Values)
                {
                    count += valueList.Count;
                }

                return count;
            }
        }

        public void Add(K key, V value)
        {
            if (TryGetValue(key, out List<V> values))
            {
                values.Add(value);
            }
            else
            {
                values = new List<V>() { value };
                Add(key, values);
            }
        }
    }
}
