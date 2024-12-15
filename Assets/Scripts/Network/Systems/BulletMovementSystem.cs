using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct BulletMovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        
        // Create an EntityCommandBuffer to handle entity destruction
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (bullet, trans, entity) in SystemAPI.Query<RefRW<Bullet>, RefRW<LocalTransform>>().WithEntityAccess())
        {
            // Move the bullet forward
            trans.ValueRW.Position += bullet.ValueRO.Direction * bullet.ValueRO.Speed * deltaTime;


            // Reduce the bullet's time left
            bullet.ValueRW.TimeLeft -= deltaTime;

            // Destroy bullet if time left is zero or less
            if (bullet.ValueRW.TimeLeft <= 0f)
            {
                ecb.AddComponent<DestroyFlag>(entity);
            }
        }

        // Playback the command buffer to apply the changes
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}