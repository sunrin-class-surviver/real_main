using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1Script : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform spawnPoint;
    public float moveInterval = 0.1f;
    public float fallInterval = 0.5f;
    public float moveSpeedVertical = 1f;
    public float moveSpeedHorizental = 2f;
    public int bulletCount = 10;
    public float fallDistance = 10f;

    private Vector3 horizontalStartPosition;
    private Vector3 verticalStartPosition;
    private ObjectPool bulletPool;
    public float waitBeforeFall = 1.5f;
    private List<GameObject> currentBullets = new List<GameObject>();

    void Start()
    {
        horizontalStartPosition = spawnPoint.position;
        verticalStartPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        verticalStartPosition.z = 0;

        bulletPool = new ObjectPool(bulletPrefab, bulletCount * 2);
        StartCoroutine(GenerateHorizontalLine());
    }

    IEnumerator GenerateHorizontalLine()
    {
        currentBullets.Clear();

        for (int i = 0; i < 18; i++)
        {
            GameObject bullet = bulletPool.GetObject();
            bullet.transform.position = horizontalStartPosition + Vector3.right * i;
            int type = i % 2;
            bullet.GetComponent<Bullet>().SetType(type);

            currentBullets.Add(bullet);
            yield return new WaitForSeconds(moveInterval);
        }

        yield return new WaitForSeconds(waitBeforeFall);

        foreach (GameObject bullet in currentBullets)
        {
            float randomDelay = Random.Range(0f, 1f);
            StartCoroutine(DropBulletDown(bullet, randomDelay));
        }

        yield return new WaitForSeconds(2f);
        StartCoroutine(GenerateVerticalLine());
    }

    IEnumerator DropBulletDown(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay + fallInterval);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            rb = bullet.AddComponent<Rigidbody2D>();
        }

        rb.gravityScale = 1f;
        rb.velocity = new Vector2(0f, -5f);

        while (true)
        {
            if (bullet.transform.position.y < -10f)
            {
                Destroy(bullet); // 바닥 밖으로 벗어나면 제거
                yield break;
            }

            yield return null;
        }
    }

    IEnumerator GenerateVerticalLine()
    {
        currentBullets.Clear();

        // 세로로 총알을 일정 간격으로 생성
        float verticalSpacing = 0.8f; // 총알 간 간격
        int totalBullets = 13;       // 생성할 총알 개수

        // 화면의 오른쪽 끝 위치 계산
        Vector3 rightEdgeWorld = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        rightEdgeWorld.z = 0f; // Z축은 항상 0으로 고정
        float margin = 1f; // 화면 오른쪽 여백
        rightEdgeWorld.x -= margin; // 여백만큼 왼쪽으로 이동

        for (int i = 0; i < totalBullets; i++)
        {
            GameObject bullet = bulletPool.GetObject();

            // 일정한 간격으로 세로로 배치 (오른쪽 끝에서 아래로 배치)
            bullet.transform.position = new Vector3(
                rightEdgeWorld.x,
                rightEdgeWorld.y - (i * verticalSpacing),
                0f
            );

            // 0, 1 타입 반복 배치
            int type = i % 2;
            bullet.GetComponent<Bullet>().SetType(type);

            currentBullets.Add(bullet);

            yield return new WaitForSeconds(moveInterval); // 간격 생성
        }

        // 총알 이동 시작 (랜덤 속도로 왼쪽으로 이동)
        foreach (GameObject bullet in currentBullets)
        {
            StartCoroutine(MoveBulletLeftWithRandomSpeed(bullet));
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(GenerateHorizontalLine()); // 가로 패턴으로 전환
    }

    IEnumerator MoveBulletLeft(GameObject bullet)
    {
        while (true)
        {
            float randomY = Random.Range(-4f, 4f);
            bullet.transform.position = new Vector3(10f, randomY, bullet.transform.position.z);

            Vector3 targetPosition = bullet.transform.position + Vector3.left;

            while (bullet.transform.position.x > -10f)
            {
                bullet.transform.position = Vector3.MoveTowards(bullet.transform.position, targetPosition, moveSpeedHorizental * Time.deltaTime);

                // 왼쪽 화면 밖으로 벗어나면 제거
                if (bullet.transform.position.x < -10f)
                {
                    Destroy(bullet);
                    yield break;
                }

                yield return null;
            }
        }
    }

    IEnumerator GenerateLeftToRightBullets()
    {
        while (true)
        {
            GameObject bullet = bulletPool.GetObject();

            // 총알을 화면 오른쪽 위에서 시작하도록 설정
            float randomY = Random.Range(-4f, 4f); // Y축 랜덤 위치
            bullet.transform.position = new Vector3(10f, randomY, 0f);

            StartCoroutine(MoveBulletLeft(bullet));

            yield return new WaitForSeconds(moveInterval);
        }
    }

    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(0, list.Count);
            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    IEnumerator MoveBulletLeftWithRandomSpeed(GameObject bullet)
    {
        // 랜덤 속도 설정
        float randomSpeed = Random.Range(3f, 7f); // 최소 속도를 3f로 설정

        // 왼쪽 끝(-10f)까지 이동하도록 타겟 위치 설정
        Vector3 targetPosition = new Vector3(-10f, bullet.transform.position.y, bullet.transform.position.z);

        while (bullet.transform.position.x > -10f)
        {
            // 목표 위치로 이동
            bullet.transform.position = Vector3.MoveTowards(
                bullet.transform.position,
                targetPosition,
                randomSpeed * Time.deltaTime // 랜덤 속도로 이동
            );

            // 멈추지 않도록 매 프레임 확인
            yield return null;
        }

        // 화면 밖으로 나가면 제거
        Destroy(bullet);
    }
}

public class ObjectPool
{
    private GameObject bulletPrefab;
    private Queue<GameObject> bulletPool = new Queue<GameObject>();

    public ObjectPool(GameObject prefab, int initialSize)
    {
        bulletPrefab = prefab;
        for (int i = 0; i < initialSize; i++)
        {
            GameObject bullet = Object.Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Enqueue(bullet);
        }
    }

    public GameObject GetObject()
    {
        if (bulletPool.Count > 0)
        {
            GameObject bullet = bulletPool.Dequeue();
            bullet.SetActive(true);
            return bullet;
        }
        else
        {
            GameObject bullet = Object.Instantiate(bulletPrefab);
            return bullet;
        }
    }

    public void ReturnObject(GameObject bullet)
    {
        bullet.SetActive(false);
        bulletPool.Enqueue(bullet);
    }
}