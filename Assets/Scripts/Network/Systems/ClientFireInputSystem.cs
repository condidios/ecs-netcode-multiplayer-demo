using Network.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
//[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct PredictedClientBulletFiringSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NetworkStreamConnection>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var currentTime = SystemAPI.Time.ElapsedTime;

        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        var bulletPrefabSingleton = SystemAPI.GetSingleton<BulletPrefabSingleton>();
        var bulletPrefab = bulletPrefabSingleton.BulletPrefab;
        foreach (var (input, transform, ghostOwner,fireTimer) in
                 SystemAPI.Query<RefRW<CubeInput>, RefRO<LocalTransform>, RefRW<GhostOwner>,RefRW<PlayerFireTimer>>()
                     .WithAll<Simulate>())
        {
            if (fireTimer.ValueRW.FireTimer > 0)
            {
                fireTimer.ValueRW.FireTimer -= SystemAPI.Time.DeltaTime;
                continue;
            }
            if (!input.ValueRO.IsFiring.IsSet) 
                continue;
            fireTimer.ValueRW.FireTimer = 1 / input.ValueRO.FireRate;
            var bulletEntity = commandBuffer.Instantiate(bulletPrefab);
            var bullet = new Bullet
            {
                Direction = math.forward((transform.ValueRO.Rotation)),
                Speed = 20f,
                TimeLeft = 3f,
            };
            commandBuffer.SetComponent(bulletEntity, ghostOwner.ValueRO);
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
        commandBuffer.Playback(state.EntityManager);
        commandBuffer.Dispose();
    }
}