using Unity.Entities;
using UnityEngine;

public class BulletPrefabSingletonAuthoring : MonoBehaviour
{
    public GameObject BulletPrefab; // Assign the bullet prefab here

    class Baker : Baker<BulletPrefabSingletonAuthoring>
    {
        public override void Bake(BulletPrefabSingletonAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            // Convert BulletPrefab GameObject to an Entity
            var bulletEntityPrefab = GetEntity(authoring.BulletPrefab, TransformUsageFlags.Dynamic);
            
            // Add the BulletPrefabSingleton component to the singleton entity
            AddComponent(entity, new BulletPrefabSingleton
            {
                BulletPrefab = bulletEntityPrefab
            });
        }
    }
}

public partial struct BulletPrefabSingleton : IComponentData
{
    public Entity BulletPrefab;
}