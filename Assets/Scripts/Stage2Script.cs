using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2Script : MonoBehaviour
{
    // 총알 관리 변수
    private List<GameObject> activeBullets = new List<GameObject>();

    // 기본 총알 프리팹
    public GameObject bulletPrefab;
    // 특별 총알 프리팹 배열
    public GameObject[] specialBullets;


     public float basicBulletInterval = 1f; // 기본 총알 생성 간격
    public float basicBulletFallSpeedMin = 1f; // 기본 총알 최소 낙하 속도
    public float basicBulletFallSpeedMax = 20f; // 기본 총알 최대 낙하 속도

    // 특별 총알 생성 관련 변수
    public float specialBulletInterval = 1f; // 특별 총알 생성 간격
    public float specialBulletFallSpeedMin = 2f; // 특별 총알 최소 낙하 속도
    public float specialBulletFallSpeedMax = 10f; // 특별 총알 최대 낙하 속도

    // 화면 경계
    public float screenLeftX = -10f;
    public float screenRightX = 10f;
    public float screenTopY = 10f;
    public float screenBottomY = -10f;

    // 가로 간격 및 총알 배치 관련 변수
    public float horizontalSpacing = 2f;
    public int maxBulletsPerRow = 6;

    // 각 매니저 오브젝트
    public GameObject triangleBulletManagerObject;
    public GameObject cShapeBulletManagerObject;
    public GameObject continuousBulletManagerObject;

    // 각 총알 매니저
    private ForScript triangleBulletManager;
    private WhileScript cShapeBulletManager;
    private IfScript continuousBulletManager;

    // 화면 크기 계산
    private float sectionWidth;
    private float sectionHeight;

    // 제외할 구역 X 값 (0 ~ 2)
    private int excludedSectionX;

    void Start()
    {
        // 각 총알 관리 스크립트를 관리할 게임 오브젝트를 동적으로 생성
       // triangleBulletManagerObject = new GameObject("TriangleBulletManager");
       // cShapeBulletManagerObject = new GameObject("CShapeBulletManager");
       // continuousBulletManagerObject = new GameObject("ContinuousBulletManager");

        // 각 스크립트를 해당 오브젝트에 부착
        //triangleBulletManager = triangleBulletManagerObject.AddComponent<ForScript>();
        //cShapeBulletManager = cShapeBulletManagerObject.AddComponent<WhileScript>();
        //continuousBulletManager = continuousBulletManagerObject.AddComponent<IfScript>();

        // 각 매니저에 bulletPrefab 전달
        //triangleBulletManager.bulletPrefab = bulletPrefab;
        //cShapeBulletManager.bulletPrefab = bulletPrefab;
        //continuousBulletManager.bulletPrefab = bulletPrefab;

        // 화면 크기 비율을 기준으로 각 구역 계산
        sectionWidth = (screenRightX - screenLeftX) / 3f;  // 가로 3등분
        sectionHeight = (screenTopY - screenBottomY) / 3f; // 세로 3등분

        // 랜덤으로 제외할 구역을 설정
        excludedSectionX = Random.Range(0, 3);  // 0, 1, 2 중 하나

        // 각 매니저에서 기능을 시작하도록 호출
        //StartCoroutine(triangleBulletManager.SpawnTriangleBulletPattern());
        //StartCoroutine(cShapeBulletManager.SpawnCShapedBulletsInSections());
        //StartCoroutine(continuousBulletManager.SpawnCShapedBulletsInSections());

        StartCoroutine(SpawnBasicBullets());
        StartCoroutine(SpawnSpecialBullets());
    }

    // HandleBulletCollision 메서드로 충돌 상태 처리
    public void HandleBulletCollision(string bulletType)
    {
        switch (bulletType)
        {
            case "for":
                Debug.Log("Handling 'for' bullet collision");
                HandleForBulletCollision();
                break;
            case "while":
                Debug.Log("Handling 'while' bullet collision");
                HandleWhileBulletCollision();
                break;
            case "break":
                Debug.Log("Handling 'break' bullet collision");
                HandleBreakBulletCollision();
                break;
            default:
                Debug.Log("Unknown bullet type");
                break;
        }
    }

    // 각 총알에 맞는 처리 로직 (예시)
    private void HandleForBulletCollision()
    {
        // For bullet이 부딪혔을 때의 처리

        triangleBulletManagerObject = new GameObject("TriangleBulletManager");

        triangleBulletManager = triangleBulletManagerObject.AddComponent<ForScript>();

        triangleBulletManager.bulletPrefab = bulletPrefab;

        StartCoroutine(triangleBulletManager.SpawnTriangleBulletPattern());
        Debug.Log("For bullet logic executed");
        // 필요한 로직 추가
    }

    private void HandleWhileBulletCollision()
    {
        // While bullet이 부딪혔을 때의 처리

         cShapeBulletManagerObject = new GameObject("CShapeBulletManager");

          cShapeBulletManager = cShapeBulletManagerObject.AddComponent<WhileScript>();

          cShapeBulletManager.bulletPrefab = bulletPrefab;

           StartCoroutine(cShapeBulletManager.SpawnCShapedBulletsInSections());
        Debug.Log("While bullet logic executed");
        // 필요한 로직 추가
    }

    private void HandleBreakBulletCollision()
    {
        // Break bullet이 부딪혔을 때의 처리

        continuousBulletManagerObject = new GameObject("ContinuousBulletManager");

        continuousBulletManager = continuousBulletManagerObject.AddComponent<IfScript>();
        
        continuousBulletManager.bulletPrefab = bulletPrefab;

         sectionWidth = (screenRightX - screenLeftX) / 3f;  // 가로 3등분
        sectionHeight = (screenTopY - screenBottomY) / 3f; // 세로 3등분

        // 랜덤으로 제외할 구역을 설정
        excludedSectionX = Random.Range(0, 3);  // 0, 1, 2 중 하나

        StartCoroutine(continuousBulletManager.SpawnCShapedBulletsInSections());

        Debug.Log("Break bullet logic executed");
        // 필요한 로직 추가
    }

    // 기본 총알 생성
     IEnumerator SpawnBasicBullets()
    {
        while (true)  // 무한 루프, 총알을 계속 생성하게 만듬
        {
            float screenWidth = (screenRightX - screenLeftX); // 화면 너비
            int totalBullets = Mathf.FloorToInt(screenWidth / horizontalSpacing) + 10; // 총알이 들어갈 수 있는 개수

            for (int i = 0; i < totalBullets; i++)
            {
                float spawnX = Random.Range(screenLeftX, screenRightX);
                float spawnY = Camera.main.orthographicSize;
                Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);

                if (!IsPositionOccupied(spawnPosition))
                {
                    GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
                    bullet.tag = "Bullet_Stage2"; // 기본 총알에 "Bullet_Stage2" 태그 추가
                    activeBullets.Add(bullet);
                    float randomFallSpeed = Random.Range(basicBulletFallSpeedMin, basicBulletFallSpeedMax);
                    StartCoroutine(MoveBulletDown(bullet, randomFallSpeed));
                }

                float randomInterval = Random.Range(0.05f, 0.1f);
                yield return new WaitForSeconds(randomInterval);
            }

            yield return new WaitForSeconds(basicBulletInterval);
        }
    }

     IEnumerator SpawnSpecialBullets()
    {
        while (true)
        {
            int numberOfBullets = Random.Range(1, 6); // 1개에서 5개까지 랜덤 생성

            for (int i = 0; i < numberOfBullets; i++)
            {
                int bulletType = Random.Range(0, specialBullets.Length);
                float randomX = Random.Range(screenLeftX, screenRightX);
                float spawnY = Camera.main.orthographicSize;
                Vector3 spawnPosition = new Vector3(randomX, spawnY, 0f);

                GameObject bulletPrefab = specialBullets[bulletType];

                if (!IsPositionOccupied(spawnPosition))
                {
                    GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

                    // 총알의 태그 추가
                    bullet.tag = bulletPrefab.tag;

                    activeBullets.Add(bullet);
                    float randomFallSpeed = Random.Range(specialBulletFallSpeedMin, specialBulletFallSpeedMax);
                    StartCoroutine(MoveBulletDown(bullet, randomFallSpeed));
                }
            }

            yield return new WaitForSeconds(specialBulletInterval);
        }
    }

    // 총알 아래로 이동 (속도 적용)
    IEnumerator MoveBulletDown(GameObject bullet, float fallSpeed)
    {
        while (bullet != null && bullet.transform.position.y > screenBottomY)
        {
            bullet.transform.position += Vector3.down * fallSpeed * Time.deltaTime;

            Collider[] hits = Physics.OverlapSphere(bullet.transform.position, 0.5f);
            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    Destroy(hit.gameObject);  // 플레이어 파괴
                    Destroy(bullet);          // 총알 파괴
                    activeBullets.Remove(bullet);  // 리스트에서 총알 제거
                    yield break;
                }
            }

            yield return null;
        }

        if (bullet != null)
        {
            activeBullets.Remove(bullet);
            Destroy(bullet);
        }
    }

    // 특정 위치가 다른 총알들과 겹치는지 확인하는 함수
    bool IsPositionOccupied(Vector3 spawnPosition)
    {
        foreach (GameObject bullet in activeBullets)
        {
            if (Vector3.Distance(bullet.transform.position, spawnPosition) < horizontalSpacing)
            {
                return true; // 겹친다면 true 반환
            }
        }
        return false; // 겹치지 않으면 false 반환
    }

    public void ClearAllBullets()
    {
        foreach (var bullet in activeBullets)
        {
            Destroy(bullet);
        }
        activeBullets.Clear();
    }
}
