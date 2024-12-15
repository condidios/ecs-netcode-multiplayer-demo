using Network.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct AimSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (input, aim, transform) in SystemAPI.Query<RefRO<CubeInput>, RefRW<AimComponent>, RefRW<LocalTransform>>())
        {
            // Rotate the aim direction based on mouse input
            var aimComponent = aim.ValueRW;
            aimComponent.AimDirection = math.mul(quaternion.Euler(-input.ValueRO.MouseDeltaY, input.ValueRO.MouseDeltaX, 0f), math.forward());

            // Apply horizontal rotation to the cube
            var cubeTransform = transform.ValueRW;
            cubeTransform.Rotation = quaternion.Euler(0f, input.ValueRO.MouseDeltaX, 0f); // Horizontal rotation only

            aim.ValueRW = aimComponent;
        }
    }
}