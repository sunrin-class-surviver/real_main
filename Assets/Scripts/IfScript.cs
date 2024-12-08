using System.Collections;
using UnityEngine;

public class IfScript : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float screenLeftX = -10f;
    public float screenRightX = 10f;
    public float screenTopY = 10f;
    public float screenBottomY = -10f;
    public float cBulletInterval = 0.05f;
    private int excludedSectionX;

    void Start()
    {
        excludedSectionX = Random.Range(0, 3); // 0: 좌, 1: 중, 2: 우
        Debug.Log("제외된 섹션: " + excludedSectionX);
        StartCoroutine(SpawnCShapedBulletsInSections());

        // 2초 뒤에 StopBulletGeneration 호출
        StartCoroutine(StopBulletGenerationAfterDelay(2f));
    }

    // 총알 생성 코루틴 (2초 동안만 총알을 생성)
    public IEnumerator SpawnCShapedBulletsInSections()
    {
        float elapsedTime = 0f; // 경과 시간 추적

        while (elapsedTime < 2f) // 2초 동안만 생성
        {
            if (excludedSectionX != 0)
                SpawnCShapedBulletInRegion(screenLeftX + 2f, -1f); 
            if (excludedSectionX != 1)
                SpawnCShapedBulletInRegion(0f, 0f); 
            if (excludedSectionX != 2)
                SpawnCShapedBulletInRegion(screenRightX - 2f, 1f); 

            elapsedTime += cBulletInterval; // 시간 증가
            yield return new WaitForSeconds(cBulletInterval); // 일정 간격으로 생성
        }

        // 2초 후 더 이상 총알을 생성하지 않음
        StopBulletGeneration();
    }

    // C자 형태로 총알을 생성하는 함수
    void SpawnCShapedBulletInRegion(float xPos, float direction)
    {
        for (int i = 0; i < 10; i++)
        {
            float spawnY = screenTopY + i * 2f; // 화면 상단부터 총알 생성
            // 각 섹션 간 충분한 간격 확보
            float spawnX = Mathf.Lerp(xPos - 3f, xPos + 3f, i / 9f); 
            Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);

            Debug.Log($"총알 생성 위치: {spawnPosition}");

            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
            // 총알에 "Bullet" 태그 부여 (인스펙터에서 미리 설정 가능)
            bullet.tag = "Bullet";
            StartCoroutine(MoveBulletDown(bullet, 30f));
        }
    }

    // 총알이 아래로 내려가는 함수
    IEnumerator MoveBulletDown(GameObject bullet, float fallSpeed)
    {
        while (bullet.transform.position.y > screenBottomY)
        {
            bullet.transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            yield return null;
        }
        Destroy(bullet);
    }

    // 총알 생성을 멈추는 함수
    public void StopBulletGeneration()
    {
        Debug.Log("모든 총알 생성을 중지합니다!");

        // "Bullet" 태그를 가진 모든 오브젝트를 찾아서 삭제
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (var bullet in bullets)
        {
            Destroy(bullet);
        }

        // 추가적으로 다른 멈춤 처리 필요 시 구현
    }

    // 2초 후 StopBulletGeneration 호출
    private IEnumerator StopBulletGenerationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StopBulletGeneration();
    }
}
