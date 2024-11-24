using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

public struct FireBulletRpc : IRpcCommand
{
    public float3 Position;
    public quaternion Rotation;
}