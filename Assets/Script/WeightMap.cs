using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WeightMap<T> : IEnumerable<KeyValuePair<T, float>>
{
    private readonly Dictionary<T, float> _map = new Dictionary<T, float>();

    public T this[float value]  => this[value, null];
    public T this[float value, IEnumerable<T> filter]
    {
        get
        {
            // casting filters to list for performance
            List<T> types = filter?.ToList();

            // if you only want one, don't need to do the rest ¯\_(ツ)_/¯
            if (types != null && types.Count == 1)
                return types[0];

            // if filters, apply, then order from lowest to highest weight
            IEnumerable<KeyValuePair<T, float>> map = (types == null || types.Count == 0 ?
                _map : _map.Where(m => types.Contains(m.Key)))
            .OrderBy(m => m.Value);

            foreach (var item in map)
                if (value < item.Value)
                    return item.Key;

            /* In case value didn't satisfy any weights,
             * defaults to lightest weight selected
             * if no matches, defaults to first in _map
            */ 
            return map.Count() > 0 ? map.Last().Key : _map.First().Key;
        }
    }

    #region IEnumerable Stuff
    public void Add(T value, float weight) => _map.Add(value, weight);
    public void Remove(T value) => _map.Remove(value);

    public IEnumerator<KeyValuePair<T, float>> GetEnumerator() =>
        ((IEnumerable<KeyValuePair<T, float>>)_map).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        ((IEnumerable)_map).GetEnumerator(); 
    #endregion
}