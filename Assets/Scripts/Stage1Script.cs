using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class Stage1Script : MonoBehaviour
{
    public GameObject bulletPrefab;
    public TextMeshProUGUI timerText;
    public Transform spawnPoint;
    public float moveInterval = 0.1f;
    private Vector3 horizontalStartPosition;
    private List<GameObject> currentBullets = new List<GameObject>();

    public AudioClip initClip;
    public AudioClip battleClip;
    public AudioClip dieClip; // die 오디오 클립 추가

    public AudioClip audioClip;


    void Start()
    {
        AudioHelper.Initialize(initClip, battleClip, dieClip, audioClip);

        // 'battle' 오디오 재생
        AudioHelper.PlayBattleAudio();
        horizontalStartPosition = spawnPoint.position;
        StartCoroutine(GenerateHorizontalLine());
        StartCoroutine(LoadNextSceneAfterDelay(40f));
    }
     IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        float remainingTime = delay;

        while (remainingTime > 0)
        {
            // 타이머 텍스트 업데이트
            if (timerText != null)
            {
                timerText.text = $"{remainingTime:F1}"; // 소수점 1자리까지 표시
            }

            remainingTime -= Time.deltaTime;

             CheatKeyHandler.CheckCheatKeys();
            yield return null; // 프레임마다 실행
        }

        // 타이머 종료 시 Stage2로 전환
        Debug.Log("Stage2Connection으로 전환합니다.");
        SceneManager.LoadScene("Stage2Connection");

        
    }

    

    IEnumerator GenerateHorizontalLine()
    {
        currentBullets.Clear();

        for (int i = 0; i < 18; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.position = horizontalStartPosition + Vector3.right * i;
            bullet.GetComponent<Bullet>().SetType(i % 2);

            currentBullets.Add(bullet);
            yield return new WaitForSeconds(moveInterval);
        }

        foreach (GameObject bullet in currentBullets)
        {
            // 각 총알에 랜덤 지연 시간 추가
            float randomDelay = Random.Range(0.0f, 1.0f);
            StartCoroutine(DropBulletDown(bullet, randomDelay));
        }

        yield return new WaitForSeconds(2f);
        StartCoroutine(GenerateVerticalLine());
    }

    IEnumerator DropBulletDown(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb == null) rb = bullet.AddComponent<Rigidbody2D>();

        rb.gravityScale = 1f;
        rb.velocity = new Vector2(0f, -5f);

        while (true)
        {
            if (bullet.transform.position.y < -10f)
            {
                Destroy(bullet);
                yield break;
            }

            yield return null;
        }
    }

    IEnumerator GenerateVerticalLine()
    {
        currentBullets.Clear();

        float verticalSpacing = 0.8f;
        int totalBullets = 13;
        Vector3 rightEdgeWorld = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        rightEdgeWorld.z = 0f;

        for (int i = 0; i < totalBullets; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.position = new Vector3(
                rightEdgeWorld.x - 1f,
                rightEdgeWorld.y - (i * verticalSpacing),
                0f
            );

            bullet.GetComponent<Bullet>().SetType(i % 2);

            currentBullets.Add(bullet);
            yield return new WaitForSeconds(moveInterval);
        }

        foreach (GameObject bullet in currentBullets)
        {
            StartCoroutine(MoveBulletLeft(bullet));
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(GenerateBottomLine());
    }

    IEnumerator GenerateBottomLine()
    {
        currentBullets.Clear();

        Vector3 bottomPosition = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        bottomPosition.y = -4.5f;
        bottomPosition.z = 0f;

        for (int i = 0; i < 18; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.position = new Vector3(
                bottomPosition.x + (i * 1.0f),
                bottomPosition.y,
                0f
            );

            bullet.GetComponent<Bullet>().SetType(i % 2);

            currentBullets.Add(bullet);
            yield return new WaitForSeconds(moveInterval);
        }

        foreach (GameObject bullet in currentBullets)
        {
            StartCoroutine(MoveBulletUp(bullet));
        }

        yield return new WaitForSeconds(2f);
        StartCoroutine(GenerateVerticalLeftLine());
    }

    IEnumerator MoveBulletUp(GameObject bullet)
    {
        float randomSpeed = Random.Range(3f, 7f);
        Vector3 targetPosition = new Vector3(bullet.transform.position.x, 10f, bullet.transform.position.z);

        while (bullet.transform.position.y < 10f)
        {
            bullet.transform.position = Vector3.MoveTowards(
                bullet.transform.position,
                targetPosition,
                randomSpeed * Time.deltaTime
            );

            yield return null;
        }

        Destroy(bullet);
    }

    IEnumerator GenerateVerticalLeftLine()
    {
        currentBullets.Clear();

        float verticalSpacing = 0.8f;
        int totalBullets = 13;

        Vector3 leftEdgeWorld = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
        leftEdgeWorld.z = 0f;

        for (int i = 0; i < totalBullets; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.position = new Vector3(
                leftEdgeWorld.x + 1f,
                leftEdgeWorld.y - (i * verticalSpacing),
                0f
            );

            bullet.GetComponent<Bullet>().SetType(i % 2);

            currentBullets.Add(bullet);
            yield return new WaitForSeconds(moveInterval);
        }

        foreach (GameObject bullet in currentBullets)
        {
            StartCoroutine(MoveBulletRight(bullet));
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(GenerateHorizontalLine());
    }

    IEnumerator MoveBulletLeft(GameObject bullet)
    {
        float randomSpeed = Random.Range(3f, 7f);

        while (bullet.transform.position.x > -10f)
        {
            bullet.transform.position += Vector3.left * randomSpeed * Time.deltaTime;
            yield return null;
        }

        Destroy(bullet);
    }

    IEnumerator MoveBulletRight(GameObject bullet)
    {
        float randomSpeed = Random.Range(3f, 7f);

        while (bullet.transform.position.x < 10f)
        {
            bullet.transform.position += Vector3.right * randomSpeed * Time.deltaTime;
            yield return null;
        }

        Destroy(bullet);
    }
}
