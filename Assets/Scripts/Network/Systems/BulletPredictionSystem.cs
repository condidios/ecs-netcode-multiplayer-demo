/*using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct BulletPredictionSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (bullet, transform) in SystemAPI.Query<RefRW<Bullet>, RefRW<LocalTransform>>())
        {
            // Predict the bullet's position based on its velocity
            transform.ValueRW.Position += bullet.ValueRO.Direction * bullet.ValueRO.Speed * SystemAPI.Time.DeltaTime;
        }
    }
}*/