using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1Script : MonoBehaviour
{
    // 기존 변수들은 그대로 유지
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
    public float waitBeforeFall = 1.5f;     // 새로 추가: 모든 총알 생성 후 대기 시간
    private List<GameObject> currentBullets = new List<GameObject>(); // 새로 추가: 현재 생성된 총알들을 저장

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
        currentBullets.Clear(); // 새로 추가

        // 왼쪽에서 오른쪽으로 순서대로 생성
        for (int i = 0; i < 18; i++)
        {
            GameObject bullet = bulletPool.GetObject();
            bullet.transform.position = horizontalStartPosition + Vector3.right * i;
            int type = i % 2;
            bullet.GetComponent<Bullet>().SetType(type);
            
            currentBullets.Add(bullet); // 새로 추가: 생성된 총알을 리스트에 저장
            yield return new WaitForSeconds(moveInterval);
        }

        // 새로 추가: 모든 총알이 생성된 후 대기
        yield return new WaitForSeconds(waitBeforeFall);

        // 새로 추가: 모든 총알에 대해 랜덤 딜레이로 떨어지게 함
        foreach (GameObject bullet in currentBullets)
        {
            float randomDelay = Random.Range(0f, 1f);
            StartCoroutine(DropBulletDown(bullet, randomDelay));
        }

        // 다음 패턴으로 진행
        yield return new WaitForSeconds(2f);
        StartCoroutine(GenerateVerticalLine());
    }

    // 수정된 DropBulletDown 코루틴 
    IEnumerator DropBulletDown(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay + fallInterval);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            rb = bullet.AddComponent<Rigidbody2D>();
        }

        rb.gravityScale = 1f; // 기본 중력 효과 적용

        // 추가: 아래로 떨어지는 초기 속도 설정
        rb.velocity = new Vector2(0f, -5f); // X축 속도는 0, Y축으로 -5의 속도
    }

    ///////////////////////////////////////////////////////////////////////////////

    // 오른쪽 세로줄 생성
//  List<GameObject> currentBullets = new List<GameObject>();  // 현재 생성된 총알들을 저장할 리스트

IEnumerator GenerateVerticalLine()
{
    List<int> order = new List<int>();
    for (int i = 0; i < 10; i++) // 세로줄의 개수
    {
        order.Add(i); // 0부터 9까지 번호 추가
    }

    // 번호를 랜덤으로 섞음
    Shuffle(order);

    // 화면의 오른쪽 끝에서 세로줄의 시작 위치를 설정
    float screenWidth = Screen.width;  // 화면의 너비
    Vector3 rightEdgeScreen = new Vector3(screenWidth, Screen.height, 0);  // 오른쪽 끝 스크린 좌표
    Vector3 rightEdgeWorld = Camera.main.ScreenToWorldPoint(rightEdgeScreen); // 월드 좌표로 변환

    // 오른쪽 끝에서 10px 정도 마진을 두고 시작하도록 조정
    float margin = 1f;  // 1 유닛 정도 마진
    rightEdgeWorld.x -= margin;  // X 좌표에 마진을 추가

    // 세로줄을 위에서 아래로 생성
    for (int i = 0; i < order.Count; i++)
    {
        int index = order[i]; // 랜덤 순서에 따라 번호를 가져옴
        GameObject bullet = bulletPool.GetObject();
         
        // Bullet의 위치 설정
        bullet.transform.position = rightEdgeWorld + Vector3.down * i; // 화면 오른쪽 끝에서 아래로 내려가면서 위치 설정
        
        // Bullet의 스프라이트 타입 설정 (0 또는 1)
        int type = i % 2;
        bullet.GetComponent<Bullet>().SetType(type);

        // 현재 생성된 총알을 리스트에 추가
        currentBullets.Add(bullet);

        yield return new WaitForSeconds(moveInterval);
    }

    // 세로줄이 다 생성되면, 각 탄막들이 랜덤한 딜레이로 오른쪽으로 이동
    foreach (GameObject bullet in currentBullets)
    {
        // 이미 생성된 총알들을 이동시키기
        StartCoroutine(MoveBulletLeft(bullet));
    }

    // 세로줄이 다 생성되면 반복
    yield return new WaitForSeconds(1f);
    StartCoroutine(GenerateHorizontalLine());
}



    // 세로줄 탄막이 오른쪽으로 이동하는 코루틴
    IEnumerator MoveBulletLeft(GameObject bullet)
{
    yield return new WaitForSeconds(fallInterval);

    // Y축 값 유지, X축 값만 이동 (왼쪽 방향)
    Vector3 targetPosition = bullet.transform.position + Vector3.left * fallDistance;

    while (bullet.transform.position != targetPosition)
    {
        bullet.transform.position = Vector3.MoveTowards(bullet.transform.position, targetPosition, moveSpeedHorizental * Time.deltaTime);
        yield return null;
    }
}

    // 리스트를 랜덤으로 섞는 함수
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
}

public class ObjectPool
{
    private GameObject bulletPrefab;
    private Queue<GameObject> bulletPool = new Queue<GameObject>(); // 제네릭 Queue 사용

    public ObjectPool(GameObject prefab, int initialSize)
    {
        bulletPrefab = prefab;
        for (int i = 0; i < initialSize; i++)
        {
            GameObject bullet = Object.Instantiate(bulletPrefab);
            bullet.SetActive(false); // 초기 상태는 비활성화
            bulletPool.Enqueue(bullet);
        }
    }

    public GameObject GetObject()
    {
        if (bulletPool.Count > 0)
        {
            GameObject bullet = bulletPool.Dequeue();
            bullet.SetActive(true); // 객체를 활성화
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
