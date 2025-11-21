using Core;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _bullet;

    public void SpawnBullet(Vector2 direction)
    {
        GameObject bulletPrefab = PrefabPoolManager.Get(_bullet);
        if (bulletPrefab == null) return;

        bulletPrefab.transform.position = transform.position;
        bulletPrefab.SetActive(true);

        Bullet bullet = bulletPrefab.GetComponent<Bullet>();
        bullet.SetDirection(direction);
    }
}
