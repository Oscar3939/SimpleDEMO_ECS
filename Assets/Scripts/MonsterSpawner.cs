using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField]
    private float _spawnTime;

    private float _timer;

    private EntityManager _entityManager;

    private void Start() {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void Update() {
        _timer += Time.deltaTime;
        if(_timer > _spawnTime) {
            GameObject monster = GameObject.CreatePrimitive(PrimitiveType.Cube);
            monster.transform.SetParent(transform);
            monster.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            monster.transform.localPosition = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f));
            monster.GetComponent<Collider>().enabled = false;

            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, new MonsterValue {
                Entity = entity,
                Position = monster.transform.position,
                Radius = 0.8f
            });
            _entityManager.AddComponentData(entity, new MonsterInstance {
                GO = monster,
            });

            _timer -= _spawnTime;
        }
    }
}
