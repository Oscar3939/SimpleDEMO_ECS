using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct BulletInstanceSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach(var (bulletValue, bulletInstance) in SystemAPI.Query<BulletValue, BulletInstance>()) {
            bulletInstance.Transform.position = bulletValue.Position;
            //Debug.Log($"direction {bulletValue.Direction} speed {bulletValue.Speed}");
        }
    }
}
