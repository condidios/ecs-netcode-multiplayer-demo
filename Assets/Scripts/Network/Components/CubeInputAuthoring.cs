using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

namespace Network.Components
{
    public struct CubeInput : IInputComponentData
    {
        public int Horizontal;
        public int Vertical;
        public float MouseDeltaX; 
        public float MouseDeltaY;
        public InputEvent IsFiring; // Event for switching to mode 2
        public float FireRate;
        public InputEvent JumpEvent;
        public InputEvent ShootingMode1;
        public InputEvent ShootingMode2;
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
        // Input values
        bool left = Input.GetKey(KeyCode.A);
        bool right = Input.GetKey(KeyCode.D);
        bool down = Input.GetKey(KeyCode.S);
        bool up = Input.GetKey(KeyCode.W);
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);

        bool isMode1Pressed = Input.GetKeyDown("1");
        bool isMode2Pressed = Input.GetKeyDown("2");

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float fireRate = 5f;

        bool isFiring = Input.GetMouseButton(0);

        foreach (var (playerInput, transform,shootMode) in SystemAPI.Query<RefRW<CubeInput>, RefRO<LocalTransform>, RefRW<PlayerFireTimer>>().WithAll<GhostOwnerIsLocal>())
        {
            var currentInput = default(CubeInput);

            // Update movement inputs
            currentInput.Horizontal = 0; // Reset to avoid stale input
            currentInput.Vertical = 0;

            if (left) currentInput.Horizontal -= 1;
            if (right) currentInput.Horizontal += 1;
            if (down) currentInput.Vertical -= 1;
            if (up) currentInput.Vertical += 1;

            // Update fire rate and mouse deltas
            currentInput.FireRate = fireRate;
            currentInput.MouseDeltaX = -mouseX;
            currentInput.MouseDeltaY = -mouseY;

            // Handle events
            if (jumpPressed)
            {
                currentInput.JumpEvent.Set();
            }

            if (isFiring)
            {
                currentInput.IsFiring.Set();
            }

            // Trigger InputEvent for shooting mode switching
            if (isMode1Pressed)
            {
                currentInput.ShootingMode1.Set();
            }
            else if (isMode2Pressed)
            {
                currentInput.ShootingMode2.Set();
            }

            // Write updated input back to the component
            playerInput.ValueRW = currentInput;
        }
    }
}



}