using Unity.Entities;

namespace Network.Components
{
    public struct FireInput : IComponentData
    {
        public bool IsFiring;         // Whether the player is firing
        public float FireRate;        // Shots per second
        public float LastFireTime;    // Time when the last bullet was fired
    }
}