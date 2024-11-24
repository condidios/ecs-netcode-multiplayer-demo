using Unity.Entities;
using Unity.NetCode;

namespace MyGame.Network
{
    public partial class NetworkManager : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<NetworkId>();
            RequireForUpdate<NetworkStreamInGame>();
        }

        protected override void OnUpdate()
        {
            // Initialization handled by specific server/client systems
        }
    }
}