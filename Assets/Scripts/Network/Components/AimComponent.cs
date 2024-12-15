using Unity.Entities;
using Unity.Mathematics;

namespace Network.Components
{
    public struct AimComponent : IComponentData
    {
        public float3 AimDirection;
        public quaternion AimRotation;
    }
}