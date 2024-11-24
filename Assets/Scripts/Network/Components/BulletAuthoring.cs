using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using UnityEngine;

public class BulletAuthoring : MonoBehaviour
{
    public float Speed = 20f;     // Bullet speed
    public float Lifetime = 3f;   // Bullet lifetime in seconds

    class Baker : Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            // Add the Bullet component with speed and lifetime
            AddComponent(entity, new Bullet
            {
                Speed = authoring.Speed,
                Direction = float3.zero,   // Set in the firing system
                TimeLeft = authoring.Lifetime
            });
            


            // Add LocalTransform to handle bullet position and rotation
            
            AddComponent<GhostAuthoringComponent>(entity);
            AddComponent(entity, new BulletSpawned());
            
        }
    }
}