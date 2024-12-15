using Network.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

namespace Network.Systems
{
    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial class GroundCollisionSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEcbSystem;

        protected override void OnCreate()
        {
            _endSimulationEcbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
            RequireForUpdate<PhysicsWorldSingleton>();
        }

        protected override void OnUpdate()
        {
            var ecb = _endSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();

            var groundedLookup = GetComponentLookup<Grounded>(isReadOnly: false);
            var groundCollisionFilter = new CollisionFilter
            {
                BelongsTo = 1u << 3, // Ground layer
                CollidesWith = ~0u, // Collide with everything
                GroupIndex = 0
            };

            Dependency = new GroundDetectionJob
            {
                Ecb = ecb,
                GroundedLookup = groundedLookup,
                GroundCollisionFilter = groundCollisionFilter
            }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), Dependency);

            _endSimulationEcbSystem.AddJobHandleForProducer(Dependency);
        }

        [BurstCompile]
        struct GroundDetectionJob : ICollisionEventsJob
        {
            public EntityCommandBuffer.ParallelWriter Ecb;
            public ComponentLookup<Grounded> GroundedLookup;
            [ReadOnly] public CollisionFilter GroundCollisionFilter;

            public void Execute(CollisionEvent collisionEvent)
            {
                
                Entity entityA = collisionEvent.EntityA;
                Entity entityB = collisionEvent.EntityB;

                // Check if either entity collides with the ground
                bool entityAIsGrounded = GroundedLookup.HasComponent(collisionEvent.EntityA);
                bool entityBIsGrounded = GroundedLookup.HasComponent(collisionEvent.EntityB);

                if (entityAIsGrounded && GroundedLookup.HasComponent(entityB))
                {
                    SetGrounded(entityB, true);
                }
                else if (entityBIsGrounded && GroundedLookup.HasComponent(entityA))
                {
                    SetGrounded(entityA, true);
                }
            }


            private void SetGrounded(Entity entity, bool isGrounded)
            {
                if (GroundedLookup.HasComponent(entity))
                {
                    var grounded = GroundedLookup[entity];
                    grounded.IsGrounded = isGrounded;
                    GroundedLookup[entity] = grounded;
                }
            }
        }
    }
}
