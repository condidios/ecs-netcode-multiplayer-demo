using Network.Components;
using Unity.Physics;
using Unity.Physics.Extensions; // For AddImpulse
using Unity.NetCode;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Network.Systems
{
    [UpdateInGroup(typeof(PredictedFixedStepSimulationSystemGroup))]
    public partial struct JumpSystem : ISystem
    {
        private const float JumpImpulse = 10f;

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (input, physicsVelocity, physicsBody, localTransform) in SystemAPI.Query<RefRO<CubeInput>, RefRW<PhysicsVelocity>, RefRW<PhysicsMass>, RefRO<LocalTransform>>()
                         .WithAll<Simulate>())
            {
                var cubeInput = input.ValueRO;

                // Check if JumpEvent is triggered and entity is on the ground
                if (cubeInput.JumpEvent.IsSet && localTransform.ValueRO.Position.y <= 0.01f)
                {
                    // Apply upward impulse
                    physicsVelocity.ValueRW.Linear += new float3(0, JumpImpulse * physicsBody.ValueRW.InverseMass, 0);
                }
            }
        }
    }
}