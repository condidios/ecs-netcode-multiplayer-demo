using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Bullet : IComponentData
{
    public float3 Direction;   // Direction of the bullet's movement
    public float Speed;        // Speed of the bullet
    public float TimeLeft;     // Time before the bullet is destroyed
}
public struct BulletSpawned : IComponentData
{
    // Marker to indicate that a bullet has been spawned
}