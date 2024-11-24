Unity NetCode Multiplayer FPS Project
This is a Unity project that implements a simple multiplayer First-Person Shooter (FPS) prototype using Unity's Entity Component System (ECS), NetCode, and Physics packages. The project showcases the basics of networked gameplay, including player movement, bullet firing, collision detection, and server-side validation.

Features
Multiplayer with Unity NetCode:

Supports authoritative server-side simulation.
RPCs (Remote Procedure Calls) are used for client-to-server communication.
Entity Component System (ECS):

Fully implemented in Unity ECS for high performance and scalability.
Avoids using GameObjects, relying on entities and components instead.
Bullet Firing Mechanic:

Bullets are instantiated on the server, synced to all clients.
Predictive simulation on the client side ensures smooth gameplay.
Bullets have a TimeLeft property to self-destruct after a set duration.
Collision Detection:

Uses Unity Physics for detecting bullet collisions with players or the environment.
Handles bullet destruction upon collision.
Burst Optimization:

Systems are Burst-compiled for high-performance simulations.
Cross-Platform:

Works on LAN and WAN setups.
Scalable architecture to extend for more features.
Technologies Used
Unity ECS: For high-performance entity-based architecture.
Unity NetCode: For implementing multiplayer networking.
Unity Physics: For collision detection and trigger events.
Unity Burst Compiler: For optimizing system performance.
Unity Transform System: For updating entity transformations (position, rotation, scale).
C# Jobs System: For multithreading simulation tasks.
How It Works
Core Components
Player Movement:

Handled via WASD keys and mouse input.
Client sends input to the server for validation.
Bullet Firing:

Clients send FireBulletRpc requests to the server.
Server spawns bullet entities and broadcasts updates to clients.
Collision Detection:

Detects when bullets hit players or objects.
Uses ITriggerEventsJob for efficient event handling.
Bullet Destruction:

Bullets are destroyed when they hit something or after TimeLeft expires.
Key Systems
FireBulletRpcSystem: Handles incoming RPC requests to fire bullets.
BulletMovementSystem: Moves bullets forward based on direction and speed.
BulletCollisionSystem: Detects and handles bullet collisions using Unity Physics.
Example Code
Bullet Component
public struct Bullet : IComponentData
{
    public float3 Direction;
    public float Speed;
    public float TimeLeft;
}
Bullet Movement System
[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct BulletMovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (bullet, trans, entity) in SystemAPI.Query<RefRW<Bullet>, RefRW<LocalTransform>>().WithEntityAccess())
        {
            trans.ValueRW.Position += bullet.ValueRO.Direction * bullet.ValueRO.Speed * deltaTime;
            bullet.ValueRW.TimeLeft -= deltaTime;

            if (bullet.ValueRW.TimeLeft <= 0f)
                ecb.DestroyEntity(entity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
Future Improvements
Add player health and respawn mechanics.
Implement environmental obstacles.
Add advanced collision layers for different entity types.
Improve client-side prediction for smoother gameplay.
License
This project is licensed under the MIT License. See the LICENSE file for details.

Acknowledgments
Thanks to the Unity ECS and NetCode teams for the great tools!
Inspired by other multiplayer FPS projects and the ECS community.
