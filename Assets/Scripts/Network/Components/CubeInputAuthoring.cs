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
        
        public float FireRate;
        public double LastFireTime; 
        
        public InputEvent JumpEvent;
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
            bool jumpPressed = Input.GetKeyDown(KeyCode.Space);

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            float fireRate = 3f;

            bool isFiring = Input.GetMouseButton(0);
            Debug.Log(isFiring);
            

            using (var ecb = new EntityCommandBuffer(Allocator.Temp))
            {
                foreach (var (playerInput, transform, entity) in SystemAPI.Query<RefRW<CubeInput>, RefRO<LocalTransform>>().WithAll<GhostOwnerIsLocal>().WithEntityAccess())
                {
                    var currentInput = playerInput.ValueRO;

                    // Update movement and firing inputs
                    currentInput.Horizontal = 0;
                    currentInput.Vertical = 0;
                    if (left) currentInput.Horizontal -= 1;
                    if (right) currentInput.Horizontal += 1;
                    if (down) currentInput.Vertical -= 1;
                    if (up) currentInput.Vertical += 1;

                    currentInput.FireRate = fireRate;
                    currentInput.MouseDeltaX = -mouseX;
                    currentInput.MouseDeltaY = -mouseY;
                    currentInput.IsFiring = isFiring;
                    
                    if (jumpPressed)
                    {
                        currentInput.JumpEvent.Set(); // Set jump event to true
                    }

                    // Write the updated input back
                    playerInput.ValueRW = currentInput;
                    
                }
                ecb.Playback(state.EntityManager);
            }
        }

    }


}