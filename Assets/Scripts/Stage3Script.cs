using System.Collections;
using UnityEngine;

public class Stage3Manager : MonoBehaviour
{
    public GameObject[] images; // 이미지 배열
    public GameObject[] blackBars; // BlackBar 배열
    public GameObject player; // 플레이어 오브젝트
    public GameObject gameOverPanel; // 게임 오버 패널
    private bool isGameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        ShowRandomImageAndBlackBar();
    }

    // 이미지와 검정색 영역을 랜덤으로 보여주는 함수
    private void ShowRandomImageAndBlackBar()
    {
        StartCoroutine(ShowImageAndBlackBarCoroutine());
    }

    // 코루틴을 이용해 이미지와 blackBar를 순차적으로 처리
    private IEnumerator ShowImageAndBlackBarCoroutine()
    {
        while (!isGameOver) // 게임 오버 상태일 때까지 반복
        {
            // 랜덤 인덱스 생성
            int randomIndex = Random.Range(0, images.Length);

            // 해당 인덱스의 이미지와 blackBar 선택
            GameObject selectedImage = images[randomIndex];
            GameObject selectedBlackBar = blackBars[randomIndex];

            // 이미지 보이게 하기
            selectedImage.SetActive(true);

            // 이미지를 화면 중앙에 위치시키기 (가정: RectTransform 사용)
            RectTransform imageRectTransform = selectedImage.GetComponent<RectTransform>();
            imageRectTransform.anchoredPosition = Vector2.zero; // 중앙에 배치

            // 이미지가 2초 동안 보이게 하기
            yield return new WaitForSeconds(2f);

            // 이미지가 끝나면 해당 blackBar를 보이게 하기
            selectedImage.SetActive(false);
            selectedBlackBar.SetActive(true);

            // blackBar가 2초 동안 보이게 하기
            yield return new WaitForSeconds(2f);

            // blackBar가 끝난 후 해당 blackBar 비활성화
            selectedBlackBar.SetActive(false);
        }

        // 게임 오버 체크
        if (isGameOver)
        {
            ShowGameOverPanel();
        }
    }

    // 게임 오버 패널을 표시하는 메서드
    private void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0; // 게임 멈추기
        }
        else
        {
            Debug.LogError("Game Over Panel is not assigned!");
        }
    }

    // 검정색 영역과 충돌했을 때 게임 오버 처리
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BlackBar") && !isGameOver)
        {
            isGameOver = true;
        }
    }
}
