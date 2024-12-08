using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct BulletDestructionSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (flag, entity) in SystemAPI.Query<RefRO<DestroyFlag>>().WithEntityAccess())
        {
            ecb.DestroyEntity(entity); // Destroy entities with DestroyFlag
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}