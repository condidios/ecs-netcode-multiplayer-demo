/*using Unity.Entities;
using Unity.NetCode;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerBulletFiringSystem : ISystem
{
    private ComponentLookup<LocalTransform> transformLookup;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BulletPrefabSingleton>();
        transformLookup = state.GetComponentLookup<LocalTransform>(true);
    }

    public void OnUpdate(ref SystemState state)
    {
        var bulletPrefab = SystemAPI.GetSingleton<BulletPrefabSingleton>().BulletPrefab;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        transformLookup.Update(ref state);

        foreach (var (request, reqSrc) in SystemAPI.Query<RefRO<BulletFiringRequest>, RefRO<ReceiveRpcCommandRequest>>())
        {
            // Ensure we have the position of the cube associated with this client connection
            if (transformLookup.HasComponent(reqSrc.ValueRO.SourceConnection))
            {
                float3 cubePosition = transformLookup[reqSrc.ValueRO.SourceConnection].Position;

                // Instantiate the bullet at the cube's position
                var bullet = ecb.Instantiate(bulletPrefab);

                // Calculate aim direction from the request's MouseDeltaX and MouseDeltaY
                float3 aimDirection = math.normalize(new float3(request.ValueRO.MouseDeltaX, request.ValueRO.MouseDeltaY, 1f));

                // Set the bullet's LocalTransform with initial position and orientation
                ecb.SetComponent(bullet, new LocalTransform
                {
                    Position = cubePosition,
                    Rotation = quaternion.LookRotationSafe(aimDirection, math.up()),  // Orienting the bullet in the aim direction
                    Scale = 0.1f
                });

                // Set the bullet's movement parameters like speed and direction
                ecb.SetComponent(bullet, new Bullet
                {
                    Direction = aimDirection,
                    Speed = 20f,
                    TimeLeft = 3f
                });

                UnityEngine.Debug.Log($"Bullet instantiated at position: {cubePosition} with direction: {aimDirection}");
            }

            // Destroy the request entity after processing
            //ecb.DestroyEntity(request.Entity);
        }

        ecb.Playback(state.EntityManager);
    }
}*/
