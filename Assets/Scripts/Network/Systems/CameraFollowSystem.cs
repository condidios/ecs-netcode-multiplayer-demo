using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Network.Systems
{
    public partial class CameraFollowSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var camera = Camera.main;

            // Ensure camera is following the local player's cube
            foreach (var (trans, owner) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<GhostOwnerIsLocal>>())
            {
                var cubePos = trans.ValueRO.Position;
                camera.transform.position = cubePos + new float3(0, 1.5f, 0); // Adjust height as needed
                camera.transform.rotation = trans.ValueRO.Rotation;
            }
        }
    }
}