/*using Network.Components;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Network.Systems
{
    [UpdateInGroup(typeof(PredictedFixedStepSimulationSystemGroup))]
    public partial struct JumpSystem : ISystem
    {
        private const float GroundLevel = 0f;
        private const float Gravity = -9.81f; // Gravity acceleration (m/s²)
        private const float JumpImpulse = 10f; // Initial jump velocity (m/s)

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (input, transform, velocity) in SystemAPI.Query<RefRO<CubeInput>, RefRW<LocalTransform>, RefRW<JumpVelocity>>()
                         .WithAll<Simulate>())
            {
                var cubeInput = input.ValueRO;
                var position = transform.ValueRW.Position;
                var jumpVelocity = velocity.ValueRW;

                // Apply jump impulse if JumpEvent is triggered and the entity is on the ground
                if (cubeInput.JumpEvent.IsSet && Mathf.Abs(position.y - GroundLevel) < 0.01f)
                {
                    jumpVelocity.velocity = JumpImpulse; // Set upward velocity
                }

                // Apply gravity
                jumpVelocity.velocity += Gravity * SystemAPI.Time.fixedDeltaTime;

                // Update position based on velocity
                position.y += jumpVelocity.velocity * SystemAPI.Time.fixedDeltaTime;

                // Clamp position to the ground and reset velocity if landed
                if (position.y < GroundLevel)
                {
                    position.y = GroundLevel;
                    jumpVelocity.velocity = 0f; // Reset velocity
                }
                Debug.Log("Velocity :"+jumpVelocity.velocity);
                Debug.Log("Position"+position.y);

                // Write updated position and velocity back
                transform.ValueRW.Position = position;
                velocity.ValueRW = jumpVelocity;
            }
        }
    }
}*/