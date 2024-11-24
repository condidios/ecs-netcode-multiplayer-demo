using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Network.Components
{
    public struct CubeInput : IInputComponentData
    {
        public int Horizontal;
        public int Vertical;
        public float MouseDeltaX; 
        public float MouseDeltaY;
        public bool IsFiring;
    }

    [DisallowMultipleComponent]
    public class CubeInputAuthoring : MonoBehaviour
    {
        class Baking : Baker<CubeInputAuthoring >
        {
            public override void Bake(CubeInputAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<CubeInput>(entity);
            }
        }
    }

    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial struct SampleCubeInput : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GhostOwnerIsLocal>();
        }

        public void OnUpdate(ref SystemState state)
        {
            bool left = Input.GetKey(KeyCode.A);
            bool right = Input.GetKey(KeyCode.D);
            bool down = Input.GetKey(KeyCode.S);
            bool up = Input.GetKey(KeyCode.W);

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            bool isFiring = Input.GetMouseButtonDown(0);

            using (var ecb = new EntityCommandBuffer(Allocator.Temp))
            {
                foreach (var (playerInput, transform, entity) in SystemAPI.Query<RefRW<CubeInput>, RefRO<LocalTransform>>().WithAll<GhostOwnerIsLocal>().WithEntityAccess())
                {
                    playerInput.ValueRW = default;
                    if (left) playerInput.ValueRW.Horizontal -= 1;
                    if (right) playerInput.ValueRW.Horizontal += 1;
                    if (down) playerInput.ValueRW.Vertical -= 1;
                    if (up) playerInput.ValueRW.Vertical += 1;

                    playerInput.ValueRW.MouseDeltaX = -mouseX;
                    playerInput.ValueRW.MouseDeltaY = -mouseY;
                    playerInput.ValueRW.IsFiring = isFiring;

                    if (isFiring)
                    {
                        var rpc = new FireBulletRpc
                        {
                            Position = transform.ValueRO.Position,
                            Rotation = transform.ValueRO.Rotation
                        };

                        var rpcEntity = ecb.CreateEntity();
                        ecb.AddComponent(rpcEntity, rpc);
                        ecb.AddComponent<SendRpcCommandRequest>(rpcEntity);
                    }
                }
                ecb.Playback(state.EntityManager);
            }
        }

    }


}