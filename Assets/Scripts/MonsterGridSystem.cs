using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct MonsterGridSystem : ISystem {
    private Entity _gridEntity;
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<MonsterValue>();

        _gridEntity = state.EntityManager.CreateEntity();
        var gridData = new MapGridData {
            GripMap = new NativeParallelMultiHashMap<Unity.Mathematics.int2, MonsterValue>(100, Allocator.Persistent),
        };
        state.EntityManager.AddComponentData(_gridEntity, gridData);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {
        var gridData = SystemAPI.GetComponent<MapGridData>(_gridEntity);
        gridData.GripMap.Dispose();
        state.EntityManager.DestroyEntity(_gridEntity);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var gridData = SystemAPI.GetComponent<MapGridData>( _gridEntity);
        gridData.GripMap.Clear();

        foreach(var monsterValue in SystemAPI.Query<RefRW<MonsterValue>>()) {
            int2 gridPosition = new int2((int)monsterValue.ValueRO.Position.x, (int)monsterValue.ValueRO.Position.z);
            gridData.GripMap.Add(gridPosition, monsterValue.ValueRO);
        }
    }
}
