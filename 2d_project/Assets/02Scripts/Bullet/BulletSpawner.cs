using Core;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _bullet;

    public void SpawnBullet(Vector2 direction, PlayerStatController stat)
    {
        GameObject bulletPrefab = PrefabPoolManager.Get(_bullet);
        if (bulletPrefab == null) return;

        bulletPrefab.transform.position = transform.position;

        if (bulletPrefab.TryGetComponent<Bullet>(out var bullet))
        {
            bullet.Init(direction, stat);
            bulletPrefab.SetActive(true);
        }
        else
        {
            Debug.LogError($"The prefab '{_bullet.name}' is missing the Bullet component.", gameObject);
            PrefabPoolManager.Return(bulletPrefab);
        }
    }
}
