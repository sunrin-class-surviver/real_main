using System.Collections;
using UnityEngine;
public class IfScript : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float screenLeftX = -10f;
    public float screenRightX = 10f;
    public float screenTopY = 10f;
    public float screenBottomY = -10f;
    public float cBulletInterval = 0.05f;
    private int excludedSectionX;

    void Start()
    {
        excludedSectionX = Random.Range(0, 3);
        StartCoroutine(SpawnCShapedBulletsInSections());
    }

    public IEnumerator SpawnCShapedBulletsInSections()
    {
        while (true)
        {
            if (excludedSectionX != 0)
                SpawnCShapedBulletInRegion(screenLeftX + 2f, -1f); 
            if (excludedSectionX != 1)
                SpawnCShapedBulletInRegion(0f, 0f); 
            if (excludedSectionX != 2)
                SpawnCShapedBulletInRegion(screenRightX - 2f, 1f); 

            yield return new WaitForSeconds(cBulletInterval);
        }
    }

    void SpawnCShapedBulletInRegion(float xPos, float direction)
    {
        for (int i = 0; i < 10; i++)
        {
            float spawnY = Camera.main.orthographicSize + i * 2f;
            float spawnX = Mathf.Lerp(xPos - 3f, xPos + 3f, (i % 10) / 9f);
            Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);

            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
            StartCoroutine(MoveBulletDown(bullet, 30f));
        }
    }

    IEnumerator MoveBulletDown(GameObject bullet, float fallSpeed)
    {
        while (bullet.transform.position.y > -10f)
        {
            bullet.transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            yield return null;
        }
        Destroy(bullet);
    }

    public void StopBulletGeneration()
    {
        Debug.Log("Stopping all continuous bullets!");

        // 총알 생성 중지: 현재 활성화된 총알들을 비활성화하거나 제거
        foreach (var bullet in GetComponentsInChildren<Bullet>())
        {
            // 예시: 총알들을 비활성화하거나 제거
            Destroy(bullet.gameObject);
        }

        // 추가적으로 다른 멈춤 처리 필요 시 구현
    }
}