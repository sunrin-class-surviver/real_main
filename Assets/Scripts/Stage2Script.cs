using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage2Script : MonoBehaviour
{
    // 총알 관리 변수
    private List<GameObject> activeBullets = new List<GameObject>();

    // 기본 총알 프리팹
    public GameObject bulletPrefab;

    // 특수 총알 프리팹 배열
    public GameObject[] specialBullets;

    // 총알 생성 간격 및 속도
    public float basicBulletInterval = 1f;
    public float basicBulletFallSpeedMin = 1f;
    public float basicBulletFallSpeedMax = 20f;
    public float specialBulletInterval = 1f;
    public float specialBulletFallSpeedMin = 2f;
    public float specialBulletFallSpeedMax = 10f;

    // 화면 경계
    public float screenLeftX = -10f;
    public float screenRightX = 10f;
    public float screenTopY = 10f;
    public float screenBottomY = -10f;

    // 총알 배치 관련 변수
    public float horizontalSpacing = 2f;
    public int maxBulletsPerRow = 6;

    // 총알 매니저
    private ForScript triangleBulletManager;
    private WhileScript cShapeBulletManager;
    private IfScript continuousBulletManager;

    // 총알 생성 상태
    private bool isSpawningBasicBullets = false;
    private bool isSpawningSpecialBullets = false;
    private bool isSpecialBulletFunctionRunning = false; // 특수 기능 실행 중 여부

    void Start()
    {
        // 기본 총알 및 특수 총알 생성 시작 여부 설정
        isSpawningBasicBullets = true;
        isSpawningSpecialBullets = true;

        if (isSpawningBasicBullets)
        {
            StartCoroutine(SpawnBasicBullets());
        }

        // 필요에 따라 총알 생성 루프 시작
        if (isSpawningSpecialBullets)
        {
            StartCoroutine(SpawnSpecialBullets());
        }

        StartCoroutine(LoadNextSceneAfterDelay(5f));
    }
    IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 콘솔에 메시지 출력
        Debug.Log("Stage3로 전환합니다.");

        // 씬 전환
        SceneManager.LoadScene("Stage3"); // "Stage2" 씬으로 전환
    }

    // 총알 충돌 처리
    public void HandleBulletCollision(string bulletType)
    {
        Debug.Log("Bullet collision detected: " + bulletType); // 로그 추가

        // 총알 삭제 및 특수 기능 시작
        if (!isSpecialBulletFunctionRunning)  // 특수 기능이 실행 중이 아니면 실행
        {
            StartCoroutine(DeleteBulletsAndStartFunction(bulletType));
        }
    }

    private IEnumerator DeleteBulletsAndStartFunction(string bulletType)
    {
        // 특수 기능 실행 중이라면 추가 총알 생성 막기
        isSpecialBulletFunctionRunning = true;

        // 모든 총알 삭제
        foreach (GameObject bullet in activeBullets)
        {
            if (bullet != null)
            {
                Destroy(bullet);
            }
        }

        // 2초 대기
        yield return new WaitForSeconds(2f);

        // 특수 총알에 맞은 총알 타입에 따라 특수 기능 시작
        switch (bulletType)
        {
            case "for":
                StartTriangleBulletPattern();
                break;
            case "while":
                StartCShapeBulletPattern();
                break;
            case "break":
                // Break 총알이 충돌하면 플레이어를 1초 멈추기
                Player player = FindObjectOfType<Player>();  // Player 스크립트 참조
                if (player != null)
                {
                    player.FreezePlayerForSeconds(8f);  // 1초 동안 플레이어를 멈추기
                }
                break;
            case "if":
                StartContinuousBulletPattern();
                break;

            default:
                Debug.Log("Unknown bullet type: " + bulletType);
                break;
        }

        // 특수 기능 실행 후 5초 대기
        yield return new WaitForSeconds(5f);

        // 특수 총알 다시 생성 시작
        isSpecialBulletFunctionRunning = false;

        // break 총알 패턴이 끝나면 기본 총알과 특수 총알 생성 재개
        StartCoroutine(ResumeBulletSpawningAfterDelay(2f)); // 2초 후 재개
    }

    // break 총알 패턴 종료 후, 기본 총알과 특수 총알 생성을 재개
    private IEnumerator ResumeBulletSpawningAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 기본 총알 및 특수 총알 생성 다시 시작
        isSpawningBasicBullets = true;
        isSpawningSpecialBullets = true;

        StartCoroutine(SpawnBasicBullets());
        StartCoroutine(SpawnSpecialBullets());
    }

    // For 총알 패턴 시작
    private void StartTriangleBulletPattern()
    {
        if (triangleBulletManager == null)
        {
            GameObject triangleBulletManagerObject = new GameObject("TriangleBulletManager");
            triangleBulletManager = triangleBulletManagerObject.AddComponent<ForScript>();
            triangleBulletManager.bulletPrefab = bulletPrefab;
        }

        Debug.Log("For bullet logic executed");
        StartCoroutine(triangleBulletManager.SpawnTriangleBulletPattern());
    }

    // While 총알 패턴 시작
    private void StartCShapeBulletPattern()
    {
        if (cShapeBulletManager == null)
        {
            GameObject cShapeBulletManagerObject = new GameObject("CShapeBulletManager");
            cShapeBulletManager = cShapeBulletManagerObject.AddComponent<WhileScript>();
            cShapeBulletManager.bulletPrefab = bulletPrefab;
        }

        Debug.Log("While bullet logic executed");
        StartCoroutine(cShapeBulletManager.SpawnCShapedBulletsInSections());
    }

    // Break 총알 패턴 시작
    private void StartContinuousBulletPattern()
    {
        if (continuousBulletManager == null)
        {
            GameObject continuousBulletManagerObject = new GameObject("ContinuousBulletManager");
            continuousBulletManager = continuousBulletManagerObject.AddComponent<IfScript>();
            continuousBulletManager.bulletPrefab = bulletPrefab;
        }

        Debug.Log("Break bullet logic executed");
        StartCoroutine(continuousBulletManager.SpawnCShapedBulletsInSections());

        // 2초 뒤에 멈추도록 처리
        StartCoroutine(StopContinuousBulletPatternAfterDelay(2f));
    }

    // 2초 뒤에 계속되는 총알 패턴을 멈추는 함수
    private IEnumerator StopContinuousBulletPatternAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 총알 생성 멈추기
        if (continuousBulletManager != null)
        {
            continuousBulletManager.StopBulletGeneration();
        }

        // 특수 기능을 멈추고, 더 이상 생성하지 않도록 설정
        StopSpawningSpecialBullets();
    }

    // 특별 총알 생성
    IEnumerator SpawnSpecialBullets()
    {
        while (isSpawningSpecialBullets)
        {
            if (isSpecialBulletFunctionRunning) // 특수 기능이 실행 중이면 생성하지 않음
            {
                yield return null; // 대기
                continue; // 특수 총알 생성 루프를 다시 시작
            }

            int numberOfBullets = Random.Range(1, 6);

            for (int i = 0; i < numberOfBullets; i++)
            {
                int bulletType = Random.Range(0, specialBullets.Length);
                float randomX = Random.Range(screenLeftX, screenRightX);
                float spawnY = Camera.main.orthographicSize;
                Vector3 spawnPosition = new Vector3(randomX, spawnY, 0f);

                // 위치가 겹치지 않도록 확인
                if (!IsPositionOccupied(spawnPosition))
                {
                    // 총알을 겹치지 않는 위치에 생성
                    GameObject bulletPrefab = specialBullets[bulletType];
                    GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
                    bullet.tag = bulletPrefab.tag;
                    activeBullets.Add(bullet);

                    // 총알 떨어지는 속도 설정
                    float randomFallSpeed = Random.Range(specialBulletFallSpeedMin, specialBulletFallSpeedMax);
                    StartCoroutine(MoveBulletDown(bullet, randomFallSpeed));
                }
                else
                {
                    // 겹치는 위치에 총알이 있어 다시 생성 시도
                    i--;  // 한번 더 시도하도록 하여 겹치지 않는 위치로 총알이 생성되도록 함
                }
            }

            // 총알 생성 간격 설정
            yield return new WaitForSeconds(specialBulletInterval);
        }
    }

    // 기본 총알 생성
    IEnumerator SpawnBasicBullets()
    {
        while (true)  // 무한 루프, 총알을 계속 생성하게 만듬
        {
            if (isSpecialBulletFunctionRunning) // 특수 기능이 실행 중이면 기본 총알 생성 멈춤
            {
                yield return null; // 대기
                continue; // 기본 총알 생성 루프를 다시 시작
            }

            float screenWidth = (screenRightX - screenLeftX); // 화면 너비
            int totalBullets = Mathf.FloorToInt(screenWidth / horizontalSpacing) + 5; // 총알이 들어갈 수 있는 개수

            for (int i = 0; i < totalBullets; i++)
            {
                float randomX = Random.Range(screenLeftX, screenRightX);
                float randomY = Camera.main.orthographicSize;
                Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);

                // 위치가 겹치지 않도록 확인
                if (!IsPositionOccupied(spawnPosition))
                {
                    GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
                    bullet.tag = "Bullet_Stage2";  // 기본 총알 태그 설정
                    activeBullets.Add(bullet);

                    // 총알 속도 설정
                    float randomFallSpeed = Random.Range(basicBulletFallSpeedMin, basicBulletFallSpeedMax);
                    StartCoroutine(MoveBulletDown(bullet, randomFallSpeed));
                }
            }

            yield return new WaitForSeconds(basicBulletInterval);
        }
    }

    // 총알이 화면을 벗어나지 않도록 이동
    IEnumerator MoveBulletDown(GameObject bullet, float fallSpeed)
    {
        while (bullet.transform.position.y > screenBottomY)
        {
            bullet.transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
            yield return null;
        }

        Destroy(bullet);  // 화면을 벗어난 총알은 삭제
    }

    // 위치가 겹치지 않는지 확인
    private bool IsPositionOccupied(Vector3 position)
    {
        foreach (GameObject bullet in activeBullets)
        {
            if (bullet != null && bullet.transform.position == position)
            {
                return true;
            }
        }
        return false;
    }

    // 기본 총알 및 특수 총알 생성 멈추기
    private void StopSpawningBasicBullets()
    {
        isSpawningBasicBullets = false;
    }

    private void StopSpawningSpecialBullets()
    {
        isSpawningSpecialBullets = false;
    }
}
