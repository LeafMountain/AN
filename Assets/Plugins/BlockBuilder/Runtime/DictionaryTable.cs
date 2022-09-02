using System.Collections.Generic;

namespace BlockBuilder
{
    public class DictionaryTable<TK, TV> 
    {
        public Dictionary<(TK, TK), TV> data = new Dictionary<(TK, TK), TV>();
        
        public TV GetValue(TK key1, TK key2)
        {
            if (data.TryGetValue((key1, key2), out TV value) || data.TryGetValue((key2, key1), out value)) return value;
            return default;
        }

        public bool ContainsKey(TK key1, TK key2)
        {
            return data.ContainsKey((key1, key2)) || data.ContainsKey((key2, key1));
        }
        
        public void Add(TK key1, TK key2, TV value)
        {
            if(data.ContainsKey((key1, key2)))
                data.Add((key1, key2), value); 
            if (data.ContainsKey((key2, key1)))
                data.Add((key2, key1), value); 
        }
        
        public void Remove(TK key1, TK key2)
        {
            if(data.ContainsKey((key1, key2)))
                data.Remove((key1, key2)); 
            if (data.ContainsKey((key2, key1)))
                data.Remove((key2, key1)); 
        }

        public void Clear() => data.Clear();
   }
}