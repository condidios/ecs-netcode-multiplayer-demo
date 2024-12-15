using Unity.Entities;
using UnityEngine;

namespace Network.Components
{
    public class GroundAuthoring : MonoBehaviour
    {
        class Baking : Baker<GroundAuthoring>
        {
            public override void Bake(GroundAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<GroundComponent>(entity);
                Debug.Log("GroundBaker executed for entity: " + entity);
            }
        }
    }
}