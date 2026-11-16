using System;
using UnityEngine;

namespace WestRicochet.Gameplay.Balls
{
    [Serializable]
    public struct BallStats
    {
        [Min(0.01f)] public float Speed;
        [Min(0f)] public int Damage;
        [Min(0)] public int MaxBounces;
    }
}