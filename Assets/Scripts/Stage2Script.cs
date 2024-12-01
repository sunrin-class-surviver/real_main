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
    private bool[] rowSpawned; // 각 행에 대해 총알이 이미 생성되었는지 확인하는 배열

    void Start()
    {
        // 기본 총알 생성 시작
        // StartCoroutine(SpawnBasicBullets());

        // // 특별 총알 생성 시작
        // StartCoroutine(SpawnSpecialBullets());

        // 삼각형 총알 패턴을 계속 생성하고 내려가게 시작
        //rowSpawned = new bool[maxRows]; // 행마다 총알 생성 여부를 추적
        //StartCoroutine(SpawnTriangleBulletPattern());

        //StartCoroutine(SpawnCShapedBullets());

        sectionWidth = (screenRightX - screenLeftX) / 3f;  // 가로 3등분
        sectionHeight = (screenTopY - screenBottomY) / 3f; // 세로 3등분

        // 랜덤으로 제외할 구역을 설정
        excludedSectionX = Random.Range(0, 3);  // 0, 1, 2 중 하나

        StartCoroutine(SpawnCShapedBulletsInSections());
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

    // 삼각형 총알 패턴 계속 생성 (쭉 내려오는 형태)
    IEnumerator SpawnTriangleBulletPattern()
    {
        while (true) // 무한 반복으로 삼각형 패턴 생성
        {
            for (int row = 0; row < maxRows; row++)
            {
                if (rowSpawned[row]) continue; // 이미 생성된 행은 건너뛰기

                // 가로 간격을 점점 넓혀 가기 위해서
                float dynamicSpacing = horizontalSpacing + (row * 0.5f); // 행마다 간격을 넓힘

                // 한 줄에 생성할 총알의 개수 (행 번호에 비례)
                int bulletsInRow = row + 1;

                // 한 줄에 총알을 배치
                for (int i = 0; i < bulletsInRow; i++)
                {
                    // X 좌표는 화면 중앙에서 왼쪽과 오른쪽 3칸을 제외한 범위 내에서 랜덤으로 배치
                    float minX = screenLeftX + dynamicSpacing;
                    float maxX = screenRightX - dynamicSpacing;
                    float spawnX = Random.Range(minX, maxX);

                    // Y 좌표는 각 행의 Y 값으로 내려옴
                    float spawnY = Camera.main.orthographicSize + (row * verticalSpacing); 

                    Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);
                    GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
                    activeBullets.Add(bullet);

                    // 총알 낙하 속도 설정
                    float randomFallSpeed = Random.Range(basicBulletFallSpeedMin, basicBulletFallSpeedMax);
                    StartCoroutine(MoveBulletDown(bullet, randomFallSpeed));
                }

                // 삼각형 패턴의 한 행을 생성하고 간격을 조절
                rowSpawned[row] = true;
                yield return new WaitForSeconds(triangleBulletInterval);
            }

            // 삼각형 패턴을 한 번 다 끝내면 다시 처음으로
            rowSpawned = new bool[maxRows];
        }
    }

    // C자 총알을 3초간 계속 생성하는 코루틴
    IEnumerator SpawnCShapedBulletsInSections()
    {
        float startTime = Time.time; // C자 총알 생성 시작 시간

        while (Time.time - startTime < cBulletDuration) // 3초 동안 계속 생성
        {
            // 랜덤으로 제외할 구역을 설정
            excludedSectionX = Random.Range(0, 3);  // 0, 1, 2 중 하나

            // C자 총알 생성이 제외된 구역을 제외한 두 구역에만 C자 총알을 미친 듯이 생성
            if (excludedSectionX != 0)
                SpawnCShapedBulletInRegion(screenLeftX + 2f, -1f); // 왼쪽 구역에 C자 총알 생성
            if (excludedSectionX != 1)
                SpawnCShapedBulletInRegion(screenLeftX + sectionWidth, 0f); // 가운데 구역에 C자 총알 생성
            if (excludedSectionX != 2)
                SpawnCShapedBulletInRegion(screenRightX - 2f, 1f); // 오른쪽 구역에 C자 총알 생성

            // C자 총알의 생성 간격을 잠깐 기다림
            yield return new WaitForSeconds(cBulletInterval); // 생성 간격을 유지하며 계속 생성
        }
    }

    // C자 모양 총알 생성 (주어진 X 위치에서)
    void SpawnCShapedBulletInRegion(float xPos, float direction)
    {
        // C자 형태로 총알을 여러 개 배치하기 위해 Y 좌표는 점차 아래로 내려가도록 설정
        float step = 2f; // 총알 사이의 간격 설정 (점점 넓어지도록)

        for (int i = 0; i < 10; i++) // 10개씩 내려옴
        {
            float spawnY = Camera.main.orthographicSize + i * step; // 위에서부터 아래로 내려가도록 설정
            // X 좌표는 수평적으로 정렬되도록 (왼쪽과 오른쪽을 차지하는 영역을 꽉 채움)
            float spawnX = Mathf.Lerp(xPos - 3f, xPos + 3f, (i % 10) / 9f); // 좌우로 넓어지도록 배치

            Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);

            // 총알 생성
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
            activeBullets.Add(bullet);

            // 총알을 아래로 떨어지게 하기 위한 낙하 시작
            StartCoroutine(MoveBulletDown(bullet, cBulletFallSpeed));
        }
    }
}
