using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemColourSO", menuName = "SlimeSurvivor/ItemColourSO")]
public class ItemColorSO : ScriptableObject
{
    public List<ItemColorSet> Data;
}

[System.Serializable]
public class ItemColorSet
{
    public Rarity Rarity;
    public Color RarityColor;
    public Color BorderColor;
    public Color BackgroundColor;
}
