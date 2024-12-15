using Network.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Network.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class CameraFollowSystem : SystemBase
    {
        private float3 _cameraOffset = new float3(0, 2f, -5f); // Adjustable offset
        private float _smoothSpeed = 5f; // Smooth movement speed
        private float _verticalAngle = 0f; // Angle for vertical aiming

        protected override void OnUpdate()
        {
            var camera = Camera.main;

            if (camera == null)
            {
                Debug.LogWarning("Main Camera not found!");
                return;
            }

            foreach (var (transform, cubeEntity) in SystemAPI.Query<RefRW<LocalTransform>>()
                         .WithAll<GhostOwnerIsLocal, PlayerTagComponent>().WithEntityAccess())
            {
                var cubeTransform = transform.ValueRW;

                // Get mouse input
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                // Rotate cube (yaw)
                quaternion horizontalRotation = quaternion.EulerXYZ(0, math.radians(mouseX * 2f), 0);
                cubeTransform.Rotation = math.mul(cubeTransform.Rotation, horizontalRotation);

                // Adjust camera pitch (vertical angle)
                _verticalAngle -= mouseY * 0.5f; // Invert Mouse Y for correct behavior
                _verticalAngle = math.clamp(_verticalAngle, -30f, 60f); // Restrict pitch

                // Compute camera position and rotation
                quaternion verticalRotation = quaternion.Euler(new float3(math.radians(_verticalAngle), 0, 0));
                float3 offset = math.mul(cubeTransform.Rotation, _cameraOffset); // Use cube's rotation for offset
                float3 desiredPosition = cubeTransform.Position + offset;

                camera.transform.position = math.lerp(camera.transform.position, desiredPosition, _smoothSpeed * SystemAPI.Time.DeltaTime);

                // Combine vertical camera rotation with cube's yaw
                float3 lookAtTarget = cubeTransform.Position + math.mul(cubeTransform.Rotation, new float3(0, 1, 10f));
                camera.transform.rotation = math.mul(cubeTransform.Rotation, verticalRotation);
            }
        }
    }
}
