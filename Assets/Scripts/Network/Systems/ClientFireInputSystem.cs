using Network.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
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
        foreach (var (input, transform) in
                 SystemAPI.Query<RefRW<CubeInput>, RefRO<LocalTransform>>()
                     .WithAll<GhostOwnerIsLocal>())
        {
            if (input.ValueRO.IsFiring)
            {
                // Calculate fire rate and spawn bullets locally
                var fireInterval = 1f / input.ValueRO.FireRate;
                if (currentTime - input.ValueRO.LastFireTime >= fireInterval)
                {
                    input.ValueRW.LastFireTime = (float)currentTime;
                    var bulletEntity = commandBuffer.Instantiate(bulletPrefab);
                    var bullet = new Bullet
                    {
                        Direction = math.forward((transform.ValueRO.Rotation)),
                        Speed = 20f,
                        TimeLeft = 3f,
                        Owner = SystemAPI.GetSingletonEntity<NetworkStreamConnection>()
                    };
                    commandBuffer.SetComponent(bulletEntity, bullet);
                    commandBuffer.SetComponent(bulletEntity, new LocalTransform
                    {
                        Position = transform.ValueRO.Position,
                        Rotation = transform.ValueRO.Rotation,
                        Scale = 0.1f
                    });

                }
            }
        }
    }
}