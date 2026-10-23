using System;
using UnityEngine;

[Serializable]
public struct BallStats
{
    [Min(0.01f)] public float Speed;
    [Min(0f)] public int Damage;
    [Min(0)] public int MaxBounces;

    public BallStats(float speed, int damage, int maxBounces)
    {
        Speed = Mathf.Max(0.01f, speed);
        Damage = Mathf.Max(0, damage);
        MaxBounces = Mathf.Max(0, maxBounces);
    }
}