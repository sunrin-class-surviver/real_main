using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2Script : MonoBehaviour
{
    public GameObject bulletPrefab; // 기본 총알 프리팹
    public GameObject[] specialBullets; // 특별 총알 프리팹 배열

    // 기본 총알 생성 관련 변수
    public float basicBulletInterval = 1f; // 기본 총알 생성 간격
    public float basicBulletFallSpeed = 1f; // 기본 총알 낙하 속도 (엄청 느리게)

    // 특별 총알 생성 관련 변수
    public float specialBulletInterval = 0.5f; // 특별 총알 생성 간격 (빠르게 생성)
    public int specialBulletBatchSize = 5; // 한 번에 생성할 특별 총알 개수
    public float specialBulletFallSpeedMultiplier = 1.5f; // 특별 총알 속도 배수 (그냥 조금만 빠르게)

    // 화면 경계
    public float screenLeftX = -8f; // 화면 좌측 경계
    public float screenRightX = 8f; // 화면 우측 경계
    public float screenBottomY = -10f; // 화면 하단 경계 (제거 위치)

    private List<GameObject> activeBullets = new List<GameObject>(); // 활성화된 총알 관리

    void Start()
    {
        // 기본 총알 생성 시작
        StartCoroutine(SpawnBasicBullets());

        // 특별 총알 생성 시작
        StartCoroutine(SpawnSpecialBullets());
    }

    IEnumerator SpawnBasicBullets()
    {
        while (true)
        {
            // 랜덤 X 좌표에서 기본 총알 생성
            float randomX = Random.Range(screenLeftX, screenRightX);

            // Y 좌표는 화면 상단
            float spawnY = Camera.main.orthographicSize;
            Vector3 spawnPosition = new Vector3(randomX, spawnY, 0f);

            // 기본 총알 생성
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

            // 활성화된 총알 리스트에 추가
            activeBullets.Add(bullet);

            // 기본 총알 이동 시작
            StartCoroutine(MoveBulletDown(bullet, basicBulletFallSpeed));

            // 다음 기본 총알 생성까지 대기
            yield return new WaitForSeconds(basicBulletInterval);
        }
    }

    IEnumerator SpawnSpecialBullets()
    {
        while (true)
        {
            for (int i = 0; i < specialBulletBatchSize; i++)
            {
                if (specialBullets.Length > 0)
                {
                    // 특별 총알 배열에서 랜덤 선택
                    GameObject specialBulletPrefab = specialBullets[Random.Range(0, specialBullets.Length)];

                    // 랜덤 X 좌표에서 특별 총알 생성
                    float randomX = Random.Range(screenLeftX, screenRightX);

                    // Y 좌표는 화면 상단
                    float spawnY = Camera.main.orthographicSize;
                    Vector3 spawnPosition = new Vector3(randomX, spawnY, 0f);

                    // 특별 총알 생성
                    GameObject bullet = Instantiate(specialBulletPrefab, spawnPosition, Quaternion.identity);

                    // 활성화된 총알 리스트에 추가
                    activeBullets.Add(bullet);

                    // 특별 총알 이동 시작
                    StartCoroutine(MoveBulletDown(bullet, basicBulletFallSpeed * specialBulletFallSpeedMultiplier));
                }
            }

            // 다음 특별 총알 생성까지 대기
            yield return new WaitForSeconds(specialBulletInterval);
        }
    }

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
}
