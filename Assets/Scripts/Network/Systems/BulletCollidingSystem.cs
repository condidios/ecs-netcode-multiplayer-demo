using System.ComponentModel;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

namespace Network.Systems
{
    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial class BulletCollisionSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEcbSystem;

        protected override void OnCreate()
        {
            _endSimulationEcbSystem = World.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _endSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();

            var playerHealthLookup = GetComponentLookup<PlayerHealth>(isReadOnly: false);
            var bulletLookup = GetComponentLookup<Bullet>(isReadOnly: true);

            Dependency = new BulletTriggerJob
            {
                Ecb = ecb,
                PlayerHealthLookup = playerHealthLookup,
                BulletLookup = bulletLookup
            }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), Dependency);

            _endSimulationEcbSystem.AddJobHandleForProducer(Dependency);
        }

        [BurstCompile]
        struct BulletTriggerJob : ITriggerEventsJob
        {
            public EntityCommandBuffer.ParallelWriter Ecb;
            public ComponentLookup<PlayerHealth> PlayerHealthLookup;
            [Unity.Collections.ReadOnly] public ComponentLookup<Bullet> BulletLookup;

            public void Execute(TriggerEvent triggerEvent)
            {
                Debug.Log("Trigger Event Detected");
                bool entityAIsBullet = BulletLookup.HasComponent(triggerEvent.EntityA);
                bool entityBIsBullet = BulletLookup.HasComponent(triggerEvent.EntityB);

                bool entityAIsPlayer = PlayerHealthLookup.HasComponent(triggerEvent.EntityA);
                bool entityBIsPlayer = PlayerHealthLookup.HasComponent(triggerEvent.EntityB);

                if (entityAIsBullet && entityBIsPlayer)
                {
                    HandleCollision(triggerEvent.EntityA, triggerEvent.EntityB);
                }
                else if (entityBIsBullet && entityAIsPlayer)
                {
                    HandleCollision(triggerEvent.EntityB, triggerEvent.EntityA);
                }
            }

            private void HandleCollision(Entity bulletEntity, Entity playerEntity)
            {
                if (PlayerHealthLookup.HasComponent(playerEntity))
                {
                    var health = PlayerHealthLookup[playerEntity];
                    health.CurrentHealth -= 10; // Example damage
                    PlayerHealthLookup[playerEntity] = health;

                    if (health.CurrentHealth <= 0)
                    {
                        Ecb.DestroyEntity(0, playerEntity);
                    }
                }
                Ecb.DestroyEntity(0, bulletEntity); // Remove bullet after collision
            }
        }
    }
}
