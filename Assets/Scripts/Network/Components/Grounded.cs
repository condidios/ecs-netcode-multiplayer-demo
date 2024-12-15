using Unity.Entities;

namespace Network.Components
{
    public struct Grounded : IComponentData
    {
        public bool IsGrounded;
    }
}