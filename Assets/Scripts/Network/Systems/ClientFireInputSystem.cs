using Network.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using Ray = UnityEngine.Ray;

[BurstCompile]
[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
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

        var entityManager = state.EntityManager; // Non-static EntityManager
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

        Camera mainCamera = Camera.main;

        foreach (var (input, transform, ghostOwner, fireTimer) in
                 SystemAPI.Query<RefRW<CubeInput>, RefRO<LocalTransform>, RefRW<GhostOwner>, RefRW<PlayerFireTimer>>()
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

            // Get aim direction
            float3 aimDirection = math.forward(transform.ValueRO.Rotation); // Default forward
            if (mainCamera != null)
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                aimDirection = math.normalize(ray.direction);
            }

            if (input.ValueRO.ShootingMode == 1) // Standard bullet firing
            {
                Debug.Log("ShootingMode1");
                var bulletEntity = commandBuffer.Instantiate(bulletPrefab);
                var bullet = new Bullet
                {
                    Direction = aimDirection,
                    Speed = 20f,
                    TimeLeft = 3f,
                };
                commandBuffer.SetComponent(bulletEntity, ghostOwner.ValueRO);
                commandBuffer.SetComponent(bulletEntity, bullet);

                float spawnOffset = 1f; // Adjust this value to control the spawn distance
                var initialPosition = transform.ValueRO.Position + aimDirection * spawnOffset;
                commandBuffer.SetComponent(bulletEntity, new LocalTransform
                {
                    Position = initialPosition,
                    Rotation = quaternion.LookRotationSafe(aimDirection, math.up()), // Rotate to face aim direction
                    Scale = 0.1f
                });
            }
            else if (input.ValueRO.ShootingMode == 2) // Raycast-based shooting
            {
                Debug.Log("RaycastAtış");
                float3 rayOrigin = transform.ValueRO.Position;
                RaycastInput raycastInput = new RaycastInput
                {
                    Start = rayOrigin,
                    End = rayOrigin + aimDirection * 100f, // Max range of 100 units
                    Filter = new CollisionFilter
                    {
                        BelongsTo = ~0u, // Collides with everything
                        CollidesWith = ~0u, // Collides with everything
                        GroupIndex = 0
                    }
                };

                if (physicsWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit hit))
                {
                    Entity hitEntity = physicsWorld.Bodies[hit.RigidBodyIndex].Entity;

                    if (entityManager.HasComponent<PlayerHealth>(hitEntity))
                    {
                        var health = entityManager.GetComponentData<PlayerHealth>(hitEntity);
                        health.CurrentHealth -= 10; // Example damage
                        entityManager.SetComponentData(hitEntity, health);

                        Debug.Log($"Hit entity {hitEntity} - New Health: {health.CurrentHealth}");
                    }
                }
            }
        }

        commandBuffer.Playback(state.EntityManager);
        commandBuffer.Dispose();
    }
}
