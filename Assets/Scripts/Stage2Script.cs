using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2Script : MonoBehaviour
{
    public GameObject bulletPrefab; // 기본 총알 프리팹
    public GameObject[] specialBullets; // 특별 총알 프리팹 배열

    // 기본 총알 생성 관련 변수
    public float basicBulletInterval = 1f; // 기본 총알 생성 간격
    public float basicBulletFallSpeedMin = 1f; // 기본 총알 최소 낙하 속도
    public float basicBulletFallSpeedMax = 20f; // 기본 총알 최대 낙하 속도 (속도 범위 확대)

    // 특별 총알 생성 관련 변수
    public float specialBulletInterval = 1f; // 특별 총알 생성 간격
    public float specialBulletFallSpeedMin = 2f; // 특별 총알 최소 낙하 속도
    public float specialBulletFallSpeedMax = 10f; // 특별 총알 최대 낙하 속도

    // 화면 경계
    public float screenLeftX = -10f; // 화면 좌측 경계 (확장)
    public float screenRightX = 10f; // 화면 우측 경계 (확장)
    public float screenTopY = 10f;
    public float screenBottomY = -10f; // 화면 하단 경계 (제거 위치)

    // 가로 간격 및 총알 배치 관련 변수
    public float horizontalSpacing = 2f; // 각 총알들 사이의 고정된 가로 간격
    public int maxBulletsPerRow = 6; // 한 줄에 배치할 총알 개수

    // 삼각형 총알 패턴 관련 변수
    public int maxRows = 5; // 삼각형 패턴의 최대 행 수
    public float verticalSpacing = 2f; // 총알 간의 세로 간격


    public float cBulletFallSpeed = 30f;
    public float cBulletInterval = 0.05f; // C자 총알의 생성 간격
    public float cBulletDuration = 3f; // C자 총알이 생성될 시간 (3초 동안 계속 생성)
    public float triangleBulletInterval = 0.1f; // 삼각형 총알 생성 간격

    private float sectionWidth;
    private float sectionHeight;

    private int excludedSectionX;  // 제외할 구역 X (0 ~ 2)

    private List<GameObject> activeBullets = new List<GameObject>(); // 활성화된 총알 관리

    // 각 매니저를 연결하는 변수
    public GameObject triangleBulletManagerObject;
    public GameObject cShapeBulletManagerObject;
    public GameObject continuousBulletManagerObject;

    private ForScript triangleBulletManager;
    private WhileScript cShapeBulletManager;
    private IfScript continuousBulletManager;

    void Start()
    {
        // 각 총알 관리 스크립트를 관리할 게임 오브젝트를 동적으로 생성
        triangleBulletManagerObject = new GameObject("TriangleBulletManager");
        cShapeBulletManagerObject = new GameObject("CShapeBulletManager");
        continuousBulletManagerObject = new GameObject("ContinuousBulletManager");

        // 각 스크립트를 해당 오브젝트에 부착
        triangleBulletManager = triangleBulletManagerObject.AddComponent<ForScript>();
        //cShapeBulletManager = cShapeBulletManagerObject.AddComponent<WhileScript>(); // C자 총알
        //continuousBulletManager = continuousBulletManagerObject.AddComponent<IfScript>(); // 계속 떨어지는 총알

        // 각 매니저에 bulletPrefab 전달
        triangleBulletManager.bulletPrefab = bulletPrefab;  // ForScript에 bulletPrefab 전달
        cShapeBulletManager.bulletPrefab = bulletPrefab;    // WhileScript에 bulletPrefab 전달
        continuousBulletManager.bulletPrefab = bulletPrefab; // IfScript에 bulletPrefab 전달

        

        sectionWidth = (screenRightX - screenLeftX) / 3f;  // 가로 3등분
        sectionHeight = (screenTopY - screenBottomY) / 3f; // 세로 3등분

        // 랜덤으로 제외할 구역을 설정
        excludedSectionX = Random.Range(0, 3);  // 0, 1, 2 중 하나

        // 각 매니저에서 기능을 시작하도록 호출
        triangleBulletManager.StartCoroutine(triangleBulletManager.SpawnTriangleBulletPattern());
        //cShapeBulletManager.StartCoroutine(cShapeBulletManager.SpawnCShapedBulletsInSections());
        //continuousBulletManager.StartCoroutine(continuousBulletManager.SpawnCShapedBulletsInSections());

        // 기본 총알과 특별 총알 생성 시작
        //StartCoroutine(SpawnBasicBullets());
        //StartCoroutine(SpawnSpecialBullets());
    }


    // 기본 총알 생성
    IEnumerator SpawnBasicBullets()
    {
        while (true)
        {
            // 총알들을 가로로 배치 (화면 크기에 맞추어)
            float screenWidth = (screenRightX - screenLeftX); // 화면 너비
            int totalBullets = Mathf.FloorToInt(screenWidth / horizontalSpacing) + 10; // 총알이 들어갈 수 있는 개수

            for (int i = 0; i < totalBullets; i++)
            {
                // X 좌표는 화면 왼쪽에서 오른쪽까지 넘치도록 랜덤으로 설정
                float spawnX = Random.Range(screenLeftX, screenRightX);

                // Y 좌표는 화면 상단
                float spawnY = Camera.main.orthographicSize;
                Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);

                // 총알 생성, 겹치지 않게 위치 확인
                if (!IsPositionOccupied(spawnPosition))
                {
                    GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
                    activeBullets.Add(bullet);
                    float randomFallSpeed = Random.Range(basicBulletFallSpeedMin, basicBulletFallSpeedMax);
                    StartCoroutine(MoveBulletDown(bullet, randomFallSpeed));
                }

                // 각 총알의 생성 간격을 랜덤으로 설정하여 조금씩 시간 차 두기
                float randomInterval = Random.Range(0.05f, 0.1f);
                yield return new WaitForSeconds(randomInterval);
            }

            // 기본 총알 생성 간격
            yield return new WaitForSeconds(basicBulletInterval);
        }
    }

    // 특별 총알 생성
    IEnumerator SpawnSpecialBullets()
    {
        while (true)
        {
            // 한 번에 최대 5개의 특별 총알을 랜덤으로 생성
            int numberOfBullets = Random.Range(1, 6); // 1개에서 5개까지 랜덤 생성

            for (int i = 0; i < numberOfBullets; i++)
            {
                // 랜덤 타입 선택 (1 ~ 5)
                int bulletType = Random.Range(0, specialBullets.Length);

                // 랜덤 X 좌표에서 특별 총알 생성
                float randomX = Random.Range(screenLeftX, screenRightX);

                // Y 좌표는 화면 상단
                float spawnY = Camera.main.orthographicSize;
                Vector3 spawnPosition = new Vector3(randomX, spawnY, 0f);

                // 특별 총알 프리팹 배열에서 랜덤으로 총알 선택
                GameObject bulletPrefab = specialBullets[bulletType];

                // 총알 생성, 겹치지 않게 위치 확인
                if (!IsPositionOccupied(spawnPosition))
                {
                    GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
                    activeBullets.Add(bullet);
                    float randomFallSpeed = Random.Range(specialBulletFallSpeedMin, specialBulletFallSpeedMax);
                    StartCoroutine(MoveBulletDown(bullet, randomFallSpeed));
                }
            }

            // 특별 총알 생성 간격
            yield return new WaitForSeconds(specialBulletInterval);
        }
    }

    // 총알 아래로 이동 (속도 적용)
    IEnumerator MoveBulletDown(GameObject bullet, float fallSpeed)
    {
        // 속도 확인을 위한 콘솔 로그 출력
        Debug.Log("Bullet Fall Speed: " + fallSpeed);

        // 속도가 너무 빠르게 설정되지 않도록 20을 넘지 않게 제한
        if (fallSpeed > 20f)
        {
            fallSpeed = 20f;
        }

        // 일정 속도로 떨어지도록 처리
        while (bullet != null && bullet.transform.position.y > screenBottomY)
        {
            bullet.transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            yield return null;
        }

        // 화면 밖으로 벗어난 총알 제거
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
}
