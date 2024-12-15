using Network.Components;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct ShootingModeSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (playerInput, entity) in SystemAPI.Query<RefRW<CubeInput>>().WithAll<GhostOwnerIsLocal>().WithEntityAccess())
        {
            var input = playerInput.ValueRW;

            // Check for mode-switching events
            if (input.SwitchToMode1.IsSet)
            {
                input.ShootingMode = 1;
                input.SwitchToMode1 = default; // Clear the event after handling
            }

            if (input.SwitchToMode2.IsSet)
            {
                input.ShootingMode = 2;
                input.SwitchToMode2 = default; // Clear the event after handling
            }

            // Write back updated input
            playerInput.ValueRW = input;
        }
    }
}