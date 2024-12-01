using System.Collections;
using UnityEngine;

public class ForScript : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject player;  // 플레이어 객체 받기
    public float horizontalSpacing = 2f;
    public float verticalSpacing = 2f;
    public int maxRows = 5;
    public float triangleBulletInterval = 0.1f;

    private bool[] rowSpawned;

    // 일정한 떨어지는 속도
    public float fixedFallSpeed = 5f;

    void Start()
    {
        rowSpawned = new bool[maxRows];
        StartCoroutine(SpawnTriangleBulletPattern());
    }

    public IEnumerator SpawnTriangleBulletPattern()
    {
        for (int row = 0; row < maxRows; row++)
        {
            int bulletsInRow = row + 1;  // 첫 번째 행은 1개, 두 번째 행은 2개, 세 번째 행은 3개...

            // 각 행에서 총알을 생성할 X 좌표 계산
            // X 좌표는 플레이어를 기준으로 설정
            for (int i = 0; i < bulletsInRow; i++)
            {
                // 플레이어의 X 위치를 기준으로 배치
                float playerX = player.transform.position.x;
                float spawnX = playerX + (i - (bulletsInRow / 2f)) * horizontalSpacing; // 플레이어를 기준으로 총알 배치
                float spawnY = Camera.main.orthographicSize + (row * verticalSpacing);  // Y 좌표는 행마다 올라가게
                Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);

                // 총알을 생성하고 아래로 떨어지게 만들기
                GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
                // 일정한 속도로 떨어짐
                StartCoroutine(MoveBulletDown(bullet, fixedFallSpeed));
            }

            yield return new WaitForSeconds(triangleBulletInterval);
        }
    }

    // 총알을 일정 속도로 떨어뜨리기
    IEnumerator MoveBulletDown(GameObject bullet, float fallSpeed)
    {
        while (bullet.transform.position.y > -10f)
        {
            bullet.transform.position += Vector3.down * fallSpeed * Time.deltaTime;  // 일정 속도로 떨어지도록
            yield return null;
        }
        Destroy(bullet);
    }
}
