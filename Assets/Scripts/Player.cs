using System.Collections;  // 이 부분 추가
using UnityEngine;
public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    public float moveSpeed = 7f;  // 이동 속도
    private bool canMove = true;  // 플레이어가 움직일 수 있는지 여부

    public GameObject gameoverPanel;  // Game Over Panel

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();  // Rigidbody2D 컴포넌트를 가져옴
    }

    private void FixedUpdate()
    {
        if (canMove)  // canMove가 true일 때만 이동 가능
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");
            Vector2 moveDirection = new Vector2(moveX, moveY).normalized;
            rb.velocity = moveDirection * moveSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;  // 움직이지 않게끔 속도 0으로 설정
        }
    }

    // 플레이어의 움직임을 멈추는 함수 (1초 동안)
    public void FreezePlayerForSeconds(float seconds)
    {
        StartCoroutine(FreezeForDuration(seconds));
    }

    private IEnumerator FreezeForDuration(float duration)
    {
        canMove = false;
        yield return new WaitForSeconds(duration);
        canMove = true;
    }

    // 충돌 처리: Bullet이나 BlackBar와 충돌하면 게임 오버
    private void OnTriggerEnter2D(Collider2D o)
    {
        if (o.CompareTag("Bullet"))
        {
            Debug.Log("Game Over: Hit by Bullet!");
            ShowGameOverPanel();
        }
        else if (o.CompareTag("BlackBar"))
        {
            Debug.Log("Game Over: Hit by Blackasdasdar!");
            ShowGameOverPanel();
        }
        else if (o.CompareTag("Bullet_Stage2"))
        {
            Debug.Log("Game Over: Hit by Blackasdasdar!");
            ShowGameOverPanel();
        }
    }

    private void ShowGameOverPanel()
    {
        if (gameoverPanel != null)
        {
            gameoverPanel.SetActive(true);  // Game Over 패널 활성화
            Time.timeScale = 0;            // 게임 멈추기
        }
        else
        {
            Debug.LogError("Game Over Panel is not assigned!");
        }   
    }
}
