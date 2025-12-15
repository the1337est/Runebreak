using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrade")]
public class UpgradeSO : ScriptableObject
{
    public string Name;
    public int BaseCost;
    public Rarity Rarity;
    public bool Stackable;
    public List<StatChange> Changes;
}
