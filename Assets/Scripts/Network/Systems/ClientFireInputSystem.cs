/*using Network.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Network.Systems
{
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    [BurstCompile]
    public partial struct ClientFireInputSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkStreamConnection>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var input in SystemAPI.Query<RefRO<CubeInput>>().WithAll<GhostOwnerIsLocal>())
            {
                if (input.ValueRO.IsFiring)
                {
                    var fireRequest = ecb.CreateEntity();
                    ecb.AddComponent(fireRequest, new BulletFiringRequest
                    {
                        MouseDeltaX = input.ValueRO.MouseDeltaX,
                        MouseDeltaY = input.ValueRO.MouseDeltaY
                    });
                    ecb.AddComponent(fireRequest, new SendRpcCommandRequest
                    {
                        TargetConnection = SystemAPI.GetSingletonEntity<NetworkStreamConnection>()
                    });
                }
            }

            ecb.Playback(state.EntityManager);
        }
    }
}*/