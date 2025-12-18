using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBaseStats", menuName = "Stats/Stats SO")]
public class StatsSO : ScriptableObject
{
    [Serializable]
    public class StatEntry
    {
        [InspectorReadOnly]
        public StatType Stat;
        public float Value;
    }

    public string Name;
    public GameObject Prefab;
    
    [SerializeField] private List<StatEntry> _stats = new();
    public List<StatEntry> Stats => _stats;

    /// <summary>
    /// Editor-only function that ensures the list always matches the Enum definition.
    /// </summary>
    private void OnValidate()
    {
        var enumValues = Enum.GetValues(typeof(StatType)).Cast<StatType>().ToList();
        if (enumValues.Contains(StatType.None))
        {
            enumValues.Remove(StatType.None);
        }
        
        _stats ??= new List<StatEntry>();
        
        var newStatsList = new List<StatEntry>();

        foreach (var type in enumValues)
        {
            var existing = _stats.Find(x => x.Stat == type);

            newStatsList.Add(existing ?? new StatEntry { Stat = type, Value = 0 });
        }
        _stats = newStatsList;
    }

    /// <summary>
    /// Runtime helper to convert the list to a Dictionary for fast O(1) lookups.
    /// Call this when initializing your PlayerStats.
    /// </summary>
    public Dictionary<StatType, float> GetStatsDictionary()
    {
        var dict = new Dictionary<StatType, float>();
        foreach (var stat in _stats)
        {
            dict.TryAdd(stat.Stat, stat.Value);
        }
        return dict;
    }
}