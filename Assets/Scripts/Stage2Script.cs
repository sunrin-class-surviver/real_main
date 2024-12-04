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
    public float basicBulletSpawnInterval = 0.1f; // 기본 총알 생성 간격 (초당 10개)
    public float basicBulletFallSpeedMin = 1f;
    public float basicBulletFallSpeedMax = 3f; // 속도 범위를 확장하여 가속 가능성 고려
    public float specialBulletInterval = 5f; // 특수 총알 생성 간격
    public float specialBulletFallSpeedMin = 1f;
    public float specialBulletFallSpeedMax = 2f;

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

    // 타이머 관련 변수
    private float remainingTime = 60f; // 초기 타이머 값
    private Coroutine timerCoroutine;

    // Coroutine 핸들
    private Coroutine generateBasicBulletsCoroutine;
    private Coroutine spawnSpecialBulletsCoroutine;

    void Start()
    {
        // 기본 총알 및 특수 총알 생성 시작 여부 설정
        isSpawningBasicBullets = true;
        isSpawningSpecialBullets = true;

        // 기본 총알 생성 Coroutine 시작
        if (isSpawningBasicBullets && generateBasicBulletsCoroutine == null)
        {
            generateBasicBulletsCoroutine = StartCoroutine(GenerateBasicBullets());
            Debug.Log("Started GenerateBasicBullets Coroutine in Start()");
        }

        // 특수 총알 생성 Coroutine 시작
        if (isSpawningSpecialBullets && spawnSpecialBulletsCoroutine == null)
        {
            spawnSpecialBulletsCoroutine = StartCoroutine(SpawnSpecialBullets());
            Debug.Log("Started SpawnSpecialBullets Coroutine in Start()");
        }

        // 타이머 Coroutine 시작
        timerCoroutine = StartCoroutine(LoadNextSceneAfterDelay());
        Debug.Log("Started LoadNextSceneAfterDelay Coroutine in Start()");
    }

    IEnumerator LoadNextSceneAfterDelay()
    {
        Debug.Log("LoadNextSceneAfterDelay Coroutine started.");

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

    // 타이머 재설정 함수 (40초로 강제 설정)
    public void ResetTimer()
    {
        remainingTime = 40f;
        Debug.Log("Timer has been reset to 40 seconds.");
    }

    // 총알 충돌 처리
    public void HandleBulletCollision(string bulletType)
    {
        Debug.Log("Bullet collision detected: " + bulletType); // 로그 추가

        // 총알 삭제 및 특수 기능 시작
        if (!isSpecialBulletFunctionRunning)  // 특수 기능이 실행 중이 아니면 실행
        {
            StartCoroutine(DeleteBulletsAndStartFunction(bulletType));
            Debug.Log($"Started DeleteBulletsAndStartFunction Coroutine for bulletType: {bulletType}");
        }
    }

    private IEnumerator DeleteBulletsAndStartFunction(string bulletType)
    {
        // 특수 기능 실행 중이라면 추가 총알 생성 막기
        isSpecialBulletFunctionRunning = true;
        Debug.Log("isSpecialBulletFunctionRunning set to true.");

        // 모든 총알 삭제 (태그가 "return"인 총알 제외)
        foreach (GameObject bullet in activeBullets)
        {
            if (bullet != null && !bullet.CompareTag("return"))
            {
                Destroy(bullet);
                Debug.Log($"Bullet destroyed: {bullet.name}");
            }
        }

        // "return" 태그를 가진 총알을 제외한 모든 총알 제거
        activeBullets.RemoveAll(bullet => bullet != null && !bullet.CompareTag("return"));
        Debug.Log("All activeBullets cleared except 'return' bullets.");

        // 2초 대기
       
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
                    player.FreezePlayerForSeconds(6f);  // 8초 동안 플레이어를 멈추기
                }
                break;
            case "if":
                StartContinuousBulletPattern();
                break;
            case "return":
                // 'return' 태그를 받은 경우, 다른 스크립트 호출하지 않고 타이머를 40초로 강제 설정
                ResetTimer(); // 40초로 타이머 재설정
                break;

            default:
                Debug.Log("Unknown bullet type: " + bulletType);
                break;
        }

        // 특수 기능 실행 후 5초 대기
        yield return new WaitForSeconds(5f);
        Debug.Log("Waited 5 seconds after special function.");

        // 특수 총알 다시 생성 시작
        isSpecialBulletFunctionRunning = false;
        Debug.Log("isSpecialBulletFunctionRunning set to false.");

        // 특수 패턴 종료 후, 기본 총알과 특수 총알 생성 재개
        StartCoroutine(ResumeBulletSpawningAfterDelay(0.1f)); // 2초 후 재개
        Debug.Log("Started ResumeBulletSpawningAfterDelay Coroutine.");
    }

    // 특수 패턴 종료 후, 기본 총알과 특수 총알 생성을 재개
    private IEnumerator ResumeBulletSpawningAfterDelay(float delay)
    {
        Debug.Log($"ResumeBulletSpawningAfterDelay Coroutine started with delay {delay} seconds.");
        yield return new WaitForSeconds(delay);

        // 기본 총알 및 특수 총알 생성 다시 시작
        isSpawningBasicBullets = true;
        isSpawningSpecialBullets = true;
        Debug.Log("isSpawningBasicBullets and isSpawningSpecialBullets set to true.");

        if (isSpawningBasicBullets && generateBasicBulletsCoroutine == null)
        {
            generateBasicBulletsCoroutine = StartCoroutine(GenerateBasicBullets());
            Debug.Log("Started GenerateBasicBullets Coroutine in ResumeBulletSpawningAfterDelay.");
        }

        if (isSpawningSpecialBullets && spawnSpecialBulletsCoroutine == null)
        {
            spawnSpecialBulletsCoroutine = StartCoroutine(SpawnSpecialBullets());
            Debug.Log("Started SpawnSpecialBullets Coroutine in ResumeBulletSpawningAfterDelay.");
        }
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
        Debug.Log($"StopContinuousBulletPatternAfterDelay Coroutine started with delay {delay} seconds.");
        yield return new WaitForSeconds(delay);

        // 총알 생성 멈추기
        if (continuousBulletManager != null)
        {
            continuousBulletManager.StopBulletGeneration();
            Debug.Log("continuousBulletManager.StopBulletGeneration() called.");
        }

        // 특수 기능을 멈추고, 더 이상 생성하지 않도록 설정
        StopSpawningSpecialBullets();
        Debug.Log("Stopped spawning special bullets in StopContinuousBulletPatternAfterDelay.");
    }

    // 기본 총알 생성 및 이동
    IEnumerator GenerateBasicBullets()
    {
        Debug.Log("GenerateBasicBullets started"); // 디버그 로그 추가

        while (isSpawningBasicBullets)
        {
            Debug.Log($"GenerateBasicBullets loop: isSpecialBulletFunctionRunning = {isSpecialBulletFunctionRunning}, activeBullets.Count = {activeBullets.Count}");

            if (isSpecialBulletFunctionRunning)
            {
                yield return null;
                continue;
            }

            // 기본 총알 생성
            float spawnX = Random.Range(screenLeftX, screenRightX);
            Vector3 spawnPosition = new Vector3(spawnX, screenTopY, 0f);

            // 기본 총알 인스턴스 생성
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
            bullet.tag = "Bullet"; // 기본 총알 태그 설정
            activeBullets.Add(bullet);
            Debug.Log($"Basic bullet spawned at {spawnPosition}"); // 디버그 로그 수정

            // 기본 총알 이동 Coroutine 시작
            float randomDelay = Random.Range(0.0f, 1.0f);
            float fallSpeed = Random.Range(basicBulletFallSpeedMin, basicBulletFallSpeedMax);
            StartCoroutine(DropBulletDown(bullet, randomDelay, fallSpeed));
            Debug.Log($"Started DropBulletDown for {bullet.name} with delay {randomDelay} and fallSpeed {fallSpeed}");

            // 다음 총알 생성 전에 basicBulletSpawnInterval 대기
            Debug.Log($"Waiting for basicBulletSpawnInterval: {basicBulletSpawnInterval} seconds.");
            yield return new WaitForSeconds(basicBulletSpawnInterval);
        }

        // Coroutine이 종료될 때 핸들 초기화
        Debug.Log("GenerateBasicBullets coroutine ended.");
        generateBasicBulletsCoroutine = null;
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
            if (activeBullets.Contains(bullet))
            {
                activeBullets.Remove(bullet);
                Debug.Log($"Removed bullet from activeBullets: {bullet.name}");
            }
            Destroy(bullet);
            Debug.Log($"Basic bullet destroyed: {bullet.name}");
        }
    }

    // 특수 총알 생성
    IEnumerator SpawnSpecialBullets()
    {
        Debug.Log("SpawnSpecialBullets started"); // 디버그 로그 추가

        while (isSpawningSpecialBullets)
        {
            Debug.Log($"SpawnSpecialBullets loop: isSpecialBulletFunctionRunning = {isSpecialBulletFunctionRunning}, activeBullets.Count = {activeBullets.Count}");

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
                Debug.Log($"Started DropSpecialBullet for {bullet.name} with delay {randomDelay} and fallSpeed {fallSpeed}");
            }

            yield return new WaitForSeconds(specialBulletInterval);
        }

        Debug.Log("SpawnSpecialBullets coroutine ended.");
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
            if (activeBullets.Contains(bullet))
            {
                activeBullets.Remove(bullet);
                Debug.Log($"Removed special bullet from activeBullets: {bullet.name}");
            }
            Destroy(bullet);
            Debug.Log($"Special bullet destroyed: {bullet.name}");
        }
    }

    // 기본 총알 및 특수 총알 생성 멈추기
    private void StopSpawningBasicBullets()
    {
        isSpawningBasicBullets = false;
        Debug.Log("Stopped spawning basic bullets");

        // Coroutine 핸들 중지
        if (generateBasicBulletsCoroutine != null)
        {
            StopCoroutine(generateBasicBulletsCoroutine);
            generateBasicBulletsCoroutine = null;
            Debug.Log("Stopped GenerateBasicBullets Coroutine.");
        }
    }

    private void StopSpawningSpecialBullets()
    {
        isSpawningSpecialBullets = false;
        Debug.Log("Stopped spawning special bullets");

        // Coroutine 핸들 중지
        if (spawnSpecialBulletsCoroutine != null)
        {
            StopCoroutine(spawnSpecialBulletsCoroutine);
            spawnSpecialBulletsCoroutine = null;
            Debug.Log("Stopped SpawnSpecialBullets Coroutine.");
        }
    }
}
