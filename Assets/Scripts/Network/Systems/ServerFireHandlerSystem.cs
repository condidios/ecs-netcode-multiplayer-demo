using Network.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

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
                var fireInterval = 1f / input.ValueRO.FireRate;
                if (currentTime - input.ValueRO.LastFireTime >= fireInterval)
                {
                    input.ValueRW.LastFireTime = (float)currentTime;

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