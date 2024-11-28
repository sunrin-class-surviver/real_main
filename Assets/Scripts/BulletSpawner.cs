using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject[] bulletPrefabs; // 총알 Prefab 배열
    public Transform[] spawnPoints;   // Spawn Point 배열
    public float spawnInterval = 2f;  // 발사 간격 (초)

    private void Start()
    {
        // 일정 간격으로 총알 발사
        InvokeRepeating(nameof(SpawnBullets), 0f, spawnInterval);
    }

    private void SpawnBullets()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            // 현재 Spawn Point에 해당하는 Prefab 가져오기
            GameObject bulletPrefab = bulletPrefabs[i % bulletPrefabs.Length];
            Transform spawnPoint = spawnPoints[i];

            // 총알 생성
            Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
