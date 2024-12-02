/*using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

namespace Network.Systems
{
    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial class FireBulletRpcSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<ReceiveRpcCommandRequest>();
        }

        protected override void OnUpdate()
        {
            var bulletPrefabSingleton = SystemAPI.GetSingleton<BulletPrefabSingleton>();
            var bulletPrefab = bulletPrefabSingleton.BulletPrefab;

            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

            // Use a foreach loop to process each entity with the specified components
            foreach (var (rpc, entity) in SystemAPI.Query<RefRO<FireBulletRpc>>().WithAll<ReceiveRpcCommandRequest>().WithEntityAccess())
            {
                var bulletEntity = commandBuffer.Instantiate(bulletPrefab);

                var bullet = new Bullet
                {
                    Direction = math.forward(rpc.ValueRO.Rotation),
                    Speed = 20f,
                    TimeLeft = 3f
                };

                commandBuffer.SetComponent(bulletEntity, bullet);

                commandBuffer.SetComponent(bulletEntity, new LocalTransform
                {
                    Position = rpc.ValueRO.Position,
                    Rotation = rpc.ValueRO.Rotation,
                    Scale = 0.1f
                });

                // Clean up the RPC entity
                commandBuffer.DestroyEntity(entity);
            }

            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
        }
    }
}*/