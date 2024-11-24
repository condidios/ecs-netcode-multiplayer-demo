using Network.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using Unity.Burst;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[BurstCompile]
public partial struct CubeMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float speed = SystemAPI.Time.DeltaTime * 4;
        float rotationSpeed = 100f; // Adjust the rotation speed as needed
        float mouseSensitivity = 0.1f; // Adjust mouse sensitivity here

        foreach (var (input, trans) in SystemAPI.Query<RefRO<CubeInput>, RefRW<LocalTransform>>().WithAll<Simulate>())
        {
            // Mouse rotation
            float yaw = -input.ValueRO.MouseDeltaX * mouseSensitivity; // Horizontal rotation (yaw)
            float pitch = input.ValueRO.MouseDeltaY * mouseSensitivity; // Vertical rotation (pitch)

            // Apply horizontal rotation (yaw) to rotate the cube
            var yawRotation = quaternion.Euler(0, yaw * rotationSpeed * SystemAPI.Time.DeltaTime, 0);
            trans.ValueRW.Rotation = math.mul(trans.ValueRW.Rotation, yawRotation);

            // Movement based on the cube's forward direction
            var forward = math.forward(trans.ValueRO.Rotation);
            var right = math.cross(new float3(0, 1, 0), forward); // Cross product to get right vector

            var moveInput = new float2(input.ValueRO.Horizontal, input.ValueRO.Vertical);
            moveInput = math.normalizesafe(moveInput) * speed;

            // Move based on the forward and right directions
            trans.ValueRW.Position += (forward * moveInput.y) + (right * moveInput.x);
        }
    }
}


