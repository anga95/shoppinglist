using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shoppinglist.Data.Models;

namespace shoppinglist.Models;

public class Items : IEnumerable<Item>
{
    private readonly List<Item> _list = new();

    public void Set(IEnumerable<Item> source)
    {
        _list.Clear();
        _list.AddRange(source);
        SortByGroups();
    }

    public void Add(Item item)
    {
        _list.Add(item);
        SortByGroups();
    }

    public bool RemoveById(int id)
    {
        var it = _list.FirstOrDefault(x => x.Id == id);
        if (it is null) return false;
        _list.Remove(it);
        SortByGroups();
        return true;
    }

    public void NotifyToggled(Item item)
    {
        // har bytt IsChecked – sortera om
        SortByGroups();
    }

    public IEnumerator<Item> GetEnumerator() => _list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void SortByGroups()
    {
        // Ocheckade överst, checkade underst.
        // Inom respektive grupp: stabil ordning per Id (eller Name), byt gärna om du vill.
        var ordered = _list
            .OrderBy(i => i.IsChecked)       // false först
            .ThenBy(i => i.Id)               // byt till Name om du vill
            .ToList();

        _list.Clear();
        _list.AddRange(ordered);
    }
}