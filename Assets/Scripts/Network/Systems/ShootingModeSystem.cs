using Network.Components;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(SimulationSystemGroup))]

public partial struct ShootingModeSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (playerInput,playertag,entity) in SystemAPI.Query<RefRW<CubeInput>,RefRW<PlayerTagComponent>>().WithAll<GhostOwnerIsLocal>().WithEntityAccess())
        {
            var input = playerInput.ValueRW;

            // Check for mode-switching events
            if (input.ShootingMode1.IsSet)
            {
                playertag.ValueRW.ShootingMode = 1; // Clear the event after handling
            }

            if (input.ShootingMode2.IsSet)
            {
                playertag.ValueRW.ShootingMode = 2; // Clear the event after handling
            }

            // Write back updated input
            playerInput.ValueRW = input;
        }
    }
}