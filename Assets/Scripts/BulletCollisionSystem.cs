using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateAfter(typeof(MonsterGridSystem))]
[BurstCompile]
public partial struct BulletCollisionSystem : ISystem {
    NativeArray<int2> _offsetGrids;
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<BulletValue>();
        _offsetGrids = new NativeArray<int2>(9, Allocator.Persistent);
        _offsetGrids[0] = new int2(0, 0);// 子彈本身所在的網格
        _offsetGrids[1] = new int2(0, 1);// 上
        _offsetGrids[2] = new int2(1, 1);// 右上
        _offsetGrids[3] = new int2(1, 0);// 右
        _offsetGrids[4] = new int2(1, -1);// 右下
        _offsetGrids[5] = new int2(0, -1);// 下
        _offsetGrids[6] = new int2(-1, -1);// 左下
        _offsetGrids[7] = new int2(-1, 0);// 左
        _offsetGrids[8] = new int2(-1, 1);// 左上
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) {
        _offsetGrids.Dispose();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var writer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
        var gridData = SystemAPI.GetSingleton<MapGridData>();
        var deltaTime = SystemAPI.Time.DeltaTime;
        new CollisionJob {
            Writer = writer,
            GridData = gridData,
            OffsetGrids = _offsetGrids,
            DeltaTime = deltaTime,
        }.ScheduleParallel();
    }

    [BurstCompile]
    partial struct CollisionJob : IJobEntity {
        public EntityCommandBuffer.ParallelWriter Writer;
        [ReadOnly] public float DeltaTime;
        [ReadOnly] public MapGridData GridData;
        [ReadOnly] public NativeArray<int2> OffsetGrids;

        [BurstCompile]
        public void Execute(ref BulletValue bullet, in Entity entity) {
            bullet.Timer += DeltaTime;
            bullet.Position += bullet.Direction * bullet.Speed * DeltaTime;
            //Debug.Log(bullet.Position);
            int2 gridIndex = new int2((int)bullet.Position.x, (int)bullet.Position.z);

            for (int i = 0; i < OffsetGrids.Length; i++) {
                int2 grid = gridIndex + OffsetGrids[i];
                if (GridData.GripMap.TryGetFirstValue(grid, out MonsterValue monsterValue, out var iterator)) {
                    do {
                        float distancesq = math.distancesq(monsterValue.Position, bullet.Position);
                        float totalRadius = (bullet.Radius + monsterValue.Radius);
                        if (distancesq < totalRadius * totalRadius) {
                            Writer.DestroyEntity(0, monsterValue.Entity);
                            Writer.DestroyEntity(1, entity);
                        }
                    }while(GridData.GripMap.TryGetNextValue(out monsterValue, ref iterator));
                }
            }

            if (bullet.Timer > 3) {
                Writer.DestroyEntity(2, entity);
            }
        }
    }
}
