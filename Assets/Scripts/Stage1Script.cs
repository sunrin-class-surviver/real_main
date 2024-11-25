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
        StartCoroutine(GenerateBottomLine()); // 가로 패턴으로 전환
    }



    IEnumerator GenerateBottomLine()
    {
        currentBullets.Clear();

        // 화면 아래쪽 시작 위치 계산 (정확히 1줄로 생성)
        Vector3 bottomPosition = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        bottomPosition.y = -4.5f; // Y 좌표를 고정 (화면 바닥 기준으로 원하는 값)
        bottomPosition.z = 0f;    // Z축 고정

        for (int i = 0; i < 18; i++)
        {
            GameObject bullet = bulletPool.GetObject();

            // 가로로 1줄로 생성되도록 설정
            bullet.transform.position = new Vector3(
                bottomPosition.x + (i * 1.0f), // X는 일정 간격으로 증가
                bottomPosition.y,              // Y는 고정된 값
                0f
            );

            // 총알 타입 설정 (0, 1 반복)
            int type = i % 2;
            bullet.GetComponent<Bullet>().SetType(type);

            currentBullets.Add(bullet);

            // 다음 총알 생성까지 대기
            yield return new WaitForSeconds(moveInterval);
        }

        // 총알 이동 시작 (위로 이동)
        foreach (GameObject bullet in currentBullets)
        {
            StartCoroutine(MoveBulletUp(bullet));
        }

        yield return new WaitForSeconds(2f);

        // 다음 패턴 호출
        StartCoroutine(GenerateVerticalLeftLine());
    }


    IEnumerator MoveBulletUp(GameObject bullet)
    {
        // 랜덤 속도 설정
        float randomSpeed = Random.Range(3f, 7f); // 최소 속도를 3f로 설정

        // 목표 위치 설정 (X 좌표는 그대로, Y는 화면 위쪽 끝으로 설정)
        float targetY = 10f; // 화면 위쪽 끝 (원하는 Y 값)
        Vector3 targetPosition = new Vector3(bullet.transform.position.x, targetY, bullet.transform.position.z);

        while (bullet.transform.position.y < targetY) // Y가 목표 값에 도달할 때까지 이동
        {
            // 목표 위치로 이동
            bullet.transform.position = Vector3.MoveTowards(
                bullet.transform.position,
                targetPosition,
                randomSpeed * Time.deltaTime // 랜덤 속도로 이동
            );

            yield return null; // 매 프레임마다 이동
        }

        // 화면 밖으로 나가면 제거
        Destroy(bullet);
    }

    IEnumerator GenerateVerticalLeftLine()
    {
        currentBullets.Clear();

        // 세로로 총알을 일정 간격으로 생성
        float verticalSpacing = 0.8f; // 총알 간 간격
        int totalBullets = 13;       // 생성할 총알 개수

        // 화면의 왼쪽 끝 위치 계산
        Vector3 leftEdgeWorld = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
        leftEdgeWorld.z = 0f; // Z축은 항상 0으로 고정
        float margin = 1f; // 화면 왼쪽 여백
        leftEdgeWorld.x += margin; // 여백만큼 오른쪽으로 이동

        for (int i = 0; i < totalBullets; i++)
        {
            GameObject bullet = bulletPool.GetObject();

            // 일정한 간격으로 세로로 배치 (왼쪽 끝에서 아래로 배치)
            bullet.transform.position = new Vector3(
                leftEdgeWorld.x,
                leftEdgeWorld.y - (i * verticalSpacing),
                0f
            );

            // 0, 1 타입 반복 배치
            int type = i % 2;
            bullet.GetComponent<Bullet>().SetType(type);

            currentBullets.Add(bullet);

            yield return new WaitForSeconds(moveInterval); // 간격 생성
        }

        // 총알 이동 시작 (랜덤 속도로 오른쪽으로 이동)
        foreach (GameObject bullet in currentBullets)
        {
            StartCoroutine(MoveBulletRightWithRandomSpeed(bullet)); // 총알이 오른쪽으로 이동하도록 수정
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(GenerateHorizontalLine()); // 가로 패턴으로 전환
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

    IEnumerator MoveBulletRightWithRandomSpeed(GameObject bullet)
    {
        // 랜덤 속도 설정
        float randomSpeed = Random.Range(3f, 7f); // 최소 속도를 3f로 설정

        // 오른쪽 끝(10f)까지 이동하도록 타겟 위치 설정
        Vector3 targetPosition = new Vector3(10f, bullet.transform.position.y, bullet.transform.position.z);

        while (bullet.transform.position.x < 10f)
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

        // 화면 밖으로 나가면 제거 또는 비활성화
        //bullet.SetActive(false); // 오브젝트 풀을 사용할 경우
        Destroy(bullet); // 오브젝트 풀을 사용하지 않는 경우
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