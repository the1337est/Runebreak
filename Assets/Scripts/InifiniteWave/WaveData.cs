using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New WaveData", menuName = "Wave Data")]
public class WaveData : ScriptableObject
{
    public int Time;
    
    [Header("Spawn Details")]
    public float SpawnInterval;
    public int MinSpawnCount;
    public int MaxSpawnCount;
    [Range(0f, 1f)] public float SpawnCountBias;
    
    [Header("Enemy Details")]
    public List<EnemyProbability> EnemyProbilities;
}

[Serializable]
public struct EnemyProbability
{
    public Enemy Enemy;
    [Range(0, 100)] public float Probability;
}
