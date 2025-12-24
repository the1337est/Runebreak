using UnityEngine;

[System.Serializable]
public struct ShopOddsProfile
{
    [Tooltip("Apply these odds starting from this wave number")]
    public int MinWave; 

    // Weights (do not need to add up to 100, we will normalize them)
    public float CommonWeight;
    public float UncommonWeight;
    public float RareWeight;
    public float EpicWeight;
    public float LegendaryWeight;
}
