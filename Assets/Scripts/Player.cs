using System.Collections;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator; // Animator 컴포넌트
    public float moveSpeed = 7f; // 이동 속도
    private bool canMove = true; // 플레이어가 움직일 수 있는지 여부

    public GameObject gameoverPanel; // Game Over Panel

    private Vector2 movement;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D 컴포넌트를 가져옴
        animator = GetComponent<Animator>(); // Animator 컴포넌트를 가져옴
    }

    private void Start()
    {
        animator.SetFloat("X", 0);
        animator.SetFloat("Y", -1); // 기본 방향을 Back으로 설정하려면 -1로 초기화
    }

private void FixedUpdate()
{
    if (canMove) // canMove가 true일 때만 이동 가능
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        movement = new Vector2(moveX, moveY).normalized;

        if (movement != Vector2.zero)
        {
            rb.velocity = movement * moveSpeed; // 이동 처리
        }
        else
        {
            rb.velocity = Vector2.zero; // 멈춤 처리
        }
    }
    else
    {
        rb.velocity = Vector2.zero; // 멈추기
    }

    onMovement(); // 이동 상태와 애니메이션 갱신
}

private void onMovement()
{
    if (movement.x != 0 || movement.y != 0) // 이동 중
    {
        animator.SetFloat("X", movement.x);
        animator.SetFloat("Y", movement.y);
        animator.SetBool("IsMoving", true); // 이동 상태
    }
    else // 멈춤
    {
        animator.SetBool("IsMoving", false); // Idle 상태
    }
}

    // 애니메이션 업데이트 함수
    private void UpdateAnimation(float X, float Y)
    {
        if (animator != null)
        {
            animator.SetFloat("X", X);
            animator.SetFloat("Y", Y);

            // 움직이는지 여부 확인 (움직이지 않으면 IsMoving은 false)
            bool isMoving = X != 0 || Y != 0;
            animator.SetBool("IsMoving", isMoving); // X, Y 모두 0일 때는 멈춤 상태

            // 방향에 맞춰서 Idle 상태로 전환
            if (!isMoving)
            {
                // Idle 상태로 돌아가기 위해 기본 값을 설정
                animator.SetFloat("X", 0);
                animator.SetFloat("Y", -1); // 기본 방향 (Back) 설정
            }
        }
        else
        {
            Debug.LogError("Animator is not assigned!");
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
            Debug.Log("Game Over: Hit by BlackBar!");
            ShowGameOverPanel();
        }
        else if (o.CompareTag("Bullet_Stage2"))
        {
            Debug.Log("Game Over: Hit by Bullet_Stage2!");
            ShowGameOverPanel();
        }
    }

    private void ShowGameOverPanel()
    {
        if (gameoverPanel != null)
        {
            gameoverPanel.SetActive(true); // Game Over 패널 활성화
            Time.timeScale = 0; // 게임 멈추기
        }
        else
        {
            Debug.LogError("Game Over Panel is not assigned!");
        }
    }
}
