/*using MyGame.Network.RPCs;
using Unity.Entities;
using Unity.NetCode;
using Unity.Collections;
using Unity.Burst;

namespace MyGame.Network.Systems
{
    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct GoInGameServerSystem : ISystem
    {
        private ComponentLookup<NetworkId> networkIdFromEntity;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CubeSpawner>();
            networkIdFromEntity = state.GetComponentLookup<NetworkId>(true);
        }

        public void OnUpdate(ref SystemState state)
        {
            var prefab = SystemAPI.GetSingleton<CubeSpawner>().Cube;
            state.EntityManager.GetName(prefab, out var prefabName);

            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            networkIdFromEntity.Update(ref state);

            foreach (var (reqSrc, reqEntity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>>().WithAll<GoInGameRequest>().WithEntityAccess())
            {
                commandBuffer.AddComponent<NetworkStreamInGame>(reqSrc.ValueRO.SourceConnection);
                var networkId = networkIdFromEntity[reqSrc.ValueRO.SourceConnection];

                // Log the spawn event for the connected client
                UnityEngine.Debug.Log($"Server: Spawning a Ghost '{prefabName}' for client {networkId.Value}");

                var player = commandBuffer.Instantiate(prefab);
                commandBuffer.SetComponent(player, new GhostOwner { NetworkId = networkId.Value });
                commandBuffer.AppendToBuffer(reqSrc.ValueRO.SourceConnection, new LinkedEntityGroup { Value = player });
                commandBuffer.DestroyEntity(reqEntity);
            }
            commandBuffer.Playback(state.EntityManager);
        }
    }
}*/