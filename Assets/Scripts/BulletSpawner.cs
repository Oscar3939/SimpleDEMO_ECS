using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField]
    private float _spawnTime;
    [SerializeField]
    private float _angle = 60;

    private float _timer;

    private EntityManager _entityManager;

    private void Start() {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void Update() {
        _timer += Time.deltaTime;
        if (_timer > _spawnTime) {
            GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bullet.transform.SetParent(transform);
            bullet.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            bullet.transform.localPosition = Vector3.zero;
            bullet.GetComponent<Collider>().enabled = false;

            var entity = _entityManager.CreateEntity();
            var angle = Random.Range(-_angle, _angle);
            Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward;
            _entityManager.AddComponentData(entity, new BulletValue {
                Position = bullet.transform.position,
                Direction = direction,
                Radius = 0.8f,
                Speed = 15,
            });
            _entityManager.AddComponentData(entity, new BulletInstance {
                Transform = bullet.transform,
            });

            _timer -= _spawnTime;
        }
    }
}
