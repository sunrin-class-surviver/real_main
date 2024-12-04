using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Stage2Script : MonoBehaviour
{
    // 총알 관리 변수
    private List<GameObject> activeBullets = new List<GameObject>();

    // 기본 총알 프리팹
    public GameObject bulletPrefab;

    // 특수 총알 프리팹 배열
    public GameObject[] specialBullets;

    // 총알 생성 간격 및 속도
    public float basicBulletInterval = 1f; // 기본 총알 세트 생성 간격
    public float basicBulletFallSpeedMin = 1f;
    public float basicBulletFallSpeedMax = 1f;
    public float specialBulletInterval = 1f; // 특수 총알 생성 간격
    public float specialBulletFallSpeedMin = 1f;
    public float specialBulletFallSpeedMax = 1f;

    // 화면 경계
    public float screenLeftX = -6f;
    public float screenRightX = 6f;
    public float screenTopY = 10f;
    public float screenBottomY = -10f;

    // 총알 매니저
    private ForScript triangleBulletManager;
    private WhileScript cShapeBulletManager;
    private IfScript continuousBulletManager;

    public TextMeshProUGUI timerText;

    // 총알 생성 상태
    private bool isSpawningBasicBullets = false;
    private bool isSpawningSpecialBullets = false;
    private bool isSpecialBulletFunctionRunning = false; // 특수 기능 실행 중 여부

    // 기본 총알 생성 관련 변수
    public int basicBulletsPerSet = 12; // 기본 총알 세트당 생성할 총알 수

    private List<GameObject> currentBasicBullets = new List<GameObject>();

    // 타이머 관련 변수
    private float remainingTime = 60f; // 초기 타이머 값
    private Coroutine timerCoroutine;

    void Start()
    {
        // 기본 총알 및 특수 총알 생성 시작 여부 설정
        isSpawningBasicBullets = true;
        isSpawningSpecialBullets = true;

        if (isSpawningBasicBullets)
        {
            StartCoroutine(GenerateBasicBullets());
        }

        // 필요에 따라 특수 총알 생성 루프 시작
        if (isSpawningSpecialBullets)
        {
            StartCoroutine(SpawnSpecialBullets());
        }

        // 타이머 코루틴 시작
        timerCoroutine = StartCoroutine(LoadNextSceneAfterDelay());
    }

    IEnumerator LoadNextSceneAfterDelay()
    {
        while (remainingTime > 0)
        {
            // 타이머 텍스트 업데이트
            if (timerText != null)
            {
                timerText.text = $"{remainingTime:F1}"; // 소수점 1자리까지 표시
            }

            remainingTime -= Time.deltaTime;
            yield return null; // 프레임마다 실행
        }

        // 타이머 종료 시 Stage3로 전환
        Debug.Log("Stage3로 전환합니다.");
        SceneManager.LoadScene("Stage3");
    }

    // 타이머 재설정 함수
    public void ResetTimer(float newTime)
    {
        remainingTime = newTime;
        Debug.Log($"Timer has been reset to {newTime} seconds.");

        // 타이머 코루틴을 재시작
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        timerCoroutine = StartCoroutine(LoadNextSceneAfterDelay());
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
                Debug.Log("Bullet destroyed");
            }
        }
        activeBullets.Clear();

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
                // Break 총알이 충돌하면 플레이어를 8초 멈추기
                Player player = FindObjectOfType<Player>();  // Player 스크립트 참조
                if (player != null)
                {
                    player.FreezePlayerForSeconds(8f);  // 8초 동안 플레이어를 멈추기
                }
                break;
            case "if":
                StartContinuousBulletPattern();
                break;
            case "return":
                // ReturnScript를 찾아서 타이머 재설정
                ReturnScript returnScript = FindObjectOfType<ReturnScript>();
                if(returnScript != null)
                {
                    returnScript.ResetTimer();
                }
                else
                {
                    Debug.LogError("ReturnScript not found in the scene.");
                }
                break;

            default:
                Debug.Log("Unknown bullet type: " + bulletType);
                break;
        }

        // 특수 기능 실행 후 5초 대기
        yield return new WaitForSeconds(5f);

        // 특수 총알 다시 생성 시작
        isSpecialBulletFunctionRunning = false;

        // 특수 패턴 종료 후, 기본 총알과 특수 총알 생성 재개
        StartCoroutine(ResumeBulletSpawningAfterDelay(2f)); // 2초 후 재개
    }

    // 특수 패턴 종료 후, 기본 총알과 특수 총알 생성을 재개
    private IEnumerator ResumeBulletSpawningAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 기본 총알 및 특수 총알 생성 다시 시작
        isSpawningBasicBullets = true;
        isSpawningSpecialBullets = true;

        StartCoroutine(GenerateBasicBullets());
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

    // 기본 총알 생성 및 이동
    IEnumerator GenerateBasicBullets()
    {
        Debug.Log("GenerateBasicBullets started"); // 디버그 로그 추가

        while (isSpawningBasicBullets)
        {
            if (isSpecialBulletFunctionRunning)
            {
                yield return null;
                continue;
            }

            // 기본 총알 세트 생성
            for (int i = 0; i < basicBulletsPerSet; i++) // 기본 총알 세트당 12개 생성
            {
                // 랜덤한 X 위치에서 생성 (-6 ~ +6)
                float spawnX = Random.Range(screenLeftX, screenRightX);
                Vector3 spawnPosition = new Vector3(spawnX, screenTopY, 0f);

                // 기본 총알 인스턴스 생성
                GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
                bullet.tag = "Bullet"; // 기본 총알 태그 설정
                // bullet.GetComponent<Bullet>().SetType(i % 2); // 필요시 타입 설정 (Bullet 스크립트 필요)
                activeBullets.Add(bullet);
                currentBasicBullets.Add(bullet);
                Debug.Log($"Basic bullet spawned at {spawnPosition}"); // 디버그 로그 수정
            }

            // 생성된 총알을 아래로 이동시키는 코루틴 시작
            foreach (GameObject bullet in currentBasicBullets)
            {
                float randomDelay = Random.Range(0.0f, 1.0f);
                float fallSpeed = Random.Range(basicBulletFallSpeedMin, basicBulletFallSpeedMax);
                StartCoroutine(DropBulletDown(bullet, randomDelay, fallSpeed));
            }

            currentBasicBullets.Clear();

            // 모든 총알이 파괴될 때까지 대기
            yield return new WaitUntil(() => activeBullets.Count == 0);

            // 다음 총알 세트 생성 전에 기본BulletInterval 대기
            yield return new WaitForSeconds(basicBulletInterval);
        }
    }

    IEnumerator DropBulletDown(GameObject bullet, float delay, float fallSpeed)
    {
        if (bullet == null) yield break;

        // 랜덤 지연 시간 대기
        yield return new WaitForSeconds(delay);

        Debug.Log($"Moving bullet {bullet.name} down with speed {fallSpeed}");

        // Transform.Translate를 사용하여 매 프레임마다 아래로 이동
        while (bullet != null && bullet.transform.position.y > screenBottomY)
        {
            bullet.transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
            yield return null;
        }

        if (bullet != null)
        {
            activeBullets.Remove(bullet);
            Destroy(bullet);
            Debug.Log("Basic bullet destroyed");
        }
    }

    // 특수 총알 생성
    IEnumerator SpawnSpecialBullets()
    {
        Debug.Log("SpawnSpecialBullets started"); // 디버그 로그 추가

        while (isSpawningSpecialBullets)
        {
            if (isSpecialBulletFunctionRunning)
            {
                yield return null;
                continue;
            }

            // 특수 총알 4개 생성
            for (int i = 0; i < 4; i++) // 특수 총알 개수 고정: 4개
            {
                // 랜덤한 X 위치에서 생성
                float spawnX = Random.Range(screenLeftX, screenRightX);
                Vector3 spawnPosition = new Vector3(spawnX, screenTopY, 0f);

                // 랜덤한 특수 총알 타입
                int bulletType = Random.Range(0, specialBullets.Length);
                GameObject bullet = Instantiate(specialBullets[bulletType], spawnPosition, Quaternion.identity);
                activeBullets.Add(bullet);
                Debug.Log($"Special bullet of type {bulletType} spawned at {spawnPosition}"); // 디버그 로그 유지

                // 랜덤 지연 시간과 속도 적용
                float randomDelay = Random.Range(0.0f, 2.0f); // 지연 시간 0.0초 ~ 2.0초
                float fallSpeed = Random.Range(specialBulletFallSpeedMin, specialBulletFallSpeedMax);
                StartCoroutine(DropSpecialBullet(bullet, randomDelay, fallSpeed));
            }

            yield return new WaitForSeconds(specialBulletInterval);
        }
    }

    IEnumerator DropSpecialBullet(GameObject bullet, float delay, float fallSpeed)
    {
        if (bullet == null) yield break;

        // 랜덤 지연 시간 대기
        yield return new WaitForSeconds(delay);

        Debug.Log($"Moving special bullet {bullet.name} down with speed {fallSpeed}");

        // Transform.Translate를 사용하여 매 프레임마다 아래로 이동
        while (bullet != null && bullet.transform.position.y > screenBottomY)
        {
            bullet.transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
            yield return null;
        }

        if (bullet != null)
        {
            activeBullets.Remove(bullet);
            Destroy(bullet);
            Debug.Log("Special bullet destroyed");
        }
    }

    // 기본 총알 및 특수 총알 생성 멈추기
    private void StopSpawningBasicBullets()
    {
        isSpawningBasicBullets = false;
        Debug.Log("Stopped spawning basic bullets");
    }

    private void StopSpawningSpecialBullets()
    {
        isSpawningSpecialBullets = false;
        Debug.Log("Stopped spawning special bullets");
    }
}
