using Network.Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Network.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct JumpSystem : ISystem
    {
        private const float JumpSpeed = 25f;
        private const float GroundLevel = 0f;

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (input, transform) in SystemAPI.Query<RefRO<CubeInput>, RefRW<LocalTransform>>())
            {
                var cubeInput = input.ValueRO;
                var position = transform.ValueRW.Position;

                // Check if the JumpEvent was triggered
                if (cubeInput.JumpEvent.IsSet)
                {
                    position.y += JumpSpeed * SystemAPI.Time.DeltaTime; // Apply jump impulse
                }

                // Simulate landing back on the ground
                if (position.y > GroundLevel)
                {
                    position.y -= 5f * SystemAPI.Time.DeltaTime; // Simulate gravity
                    if (position.y < GroundLevel)
                    {
                        position.y = GroundLevel; // Clamp to ground
                    }
                }

                // Write updated position back
                transform.ValueRW.Position = position;
            }
        }
    }

}