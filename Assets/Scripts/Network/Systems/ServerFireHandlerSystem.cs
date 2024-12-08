using Network.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerBulletSpawnSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BulletPrefabSingleton>();
        state.RequireForUpdate<NetworkStreamConnection>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var currentTime = SystemAPI.Time.ElapsedTime;

        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        var bulletPrefabSingleton = SystemAPI.GetSingleton<BulletPrefabSingleton>();
        var bulletPrefab = bulletPrefabSingleton.BulletPrefab;
        
        foreach (var (input, transform, entity) in
                 SystemAPI.Query<RefRW<CubeInput>, RefRO<LocalTransform>>()
                     .WithAll<GhostOwner>().WithEntityAccess())
        {
            if (input.ValueRO.IsFiring)
            {
                var elapsedTime = SystemAPI.Time.ElapsedTime;
                var timeSinceLastShot = elapsedTime - input.ValueRO.LastFireTime;
                var fireInterval = 1f / input.ValueRO.FireRate;

                if (timeSinceLastShot >= fireInterval)
                {
                    
                    input.ValueRW.LastFireTime = elapsedTime;

                    // Create the bullet entity
                    var bulletEntity = commandBuffer.Instantiate(bulletPrefab);
                    var bullet = new Bullet
                    {
                        Direction = math.forward((transform.ValueRO.Rotation)),
                        Speed = 20f,
                        TimeLeft = 3f,
                        Owner = entity
                    };
                    commandBuffer.SetComponent(bulletEntity, bullet);
                    float spawnOffset = 0.5f; // Adjust this value to control the spawn distance
                    var initialPosition = transform.ValueRO.Position + math.forward(transform.ValueRO.Rotation) * spawnOffset;
                    commandBuffer.SetComponent(bulletEntity, new LocalTransform
                    {
                        Position = initialPosition,
                        Rotation = transform.ValueRO.Rotation,
                        Scale = 0.1f
                    });
                }
            }
        }
        commandBuffer.Playback(state.EntityManager);
        commandBuffer.Dispose();
    }
}