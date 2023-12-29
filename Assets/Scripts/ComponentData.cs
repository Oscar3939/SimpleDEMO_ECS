using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

public struct MapGridData : IComponentData {
    public NativeParallelMultiHashMap<int2, MonsterValue> GripMap;
}

public struct MonsterValue : IComponentData {
    public float3 Position;
    public float Radius;
    public Entity Entity;
}

public class MonsterInstance : IComponentData, IDisposable {
    public GameObject GO;

    public void Dispose() {
        if (GO != null) {
            Object.Destroy(GO);
        }
    }
}

public struct BulletValue : IComponentData {
    public float3 Position;
    public float3 Direction;
    public float Radius;
    public float Speed;
    public float Timer;
}

public class BulletInstance : IComponentData, IDisposable {
    public Transform Transform;

    public void Dispose() {
        if (Transform != null) {
            Object.Destroy(Transform.gameObject);
        }
    }
}