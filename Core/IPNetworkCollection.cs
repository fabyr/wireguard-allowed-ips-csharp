using System.Collections;

namespace WireguardAllowedIPs.Core;

public class IPNetworkCollection<T> : ICollection<T> where T : IPNetwork
{
    public int Count => _list.Count;

    public bool IsReadOnly => false;

    private readonly List<T> _list = new();

    public void Add(T item)
    {
        if(!Contains(item))
        {
            
        }
    }

    public bool Remove(T item)
    {
        throw new NotImplementedException();
    }

    public bool Contains(T item)
    {
        return _list.Any(x => x.Contains(item));
    }

    public void Clear()
    {
        _list.Clear();
        _list.Add((T)IPNetwork.Any<T>());
    }
    
    public void CopyTo(T[] array, int arrayIndex)
    {
        _list.CopyTo(array, arrayIndex);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _list.GetEnumerator();
    }
    
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IList)_list).GetEnumerator();
    }
}