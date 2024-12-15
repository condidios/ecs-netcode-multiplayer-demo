using Network.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using UnityEditor.SceneManagement;
using UnityEngine;

public struct Cube : IComponentData
{
}

[DisallowMultipleComponent]
public class CubeAuthoring : MonoBehaviour
{
    class Baker : Baker<CubeAuthoring>
    {
        public override void Bake(CubeAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<NetworkStreamConnection>(entity);
            AddComponent<PlayerHealth>(entity ,new PlayerHealth
            {
                CurrentHealth = 100
            });
            AddComponent<Grounded>(entity);
            AddComponent<AimComponent>(entity);
            AddComponent<PlayerTagComponent>(entity);
            AddComponent<PlayerFireTimer>(entity);
            AddComponent<JumpVelocity>(entity);
            AddComponent<Cube>(entity);
            
        }
    }
}