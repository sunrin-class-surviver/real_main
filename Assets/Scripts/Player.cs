using System.Collections;
using UnityEngine;

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

            Debug.Log("Movement: " + movement); // 디버깅 출력

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

            // IsMoving이 false일 때 모든 애니메이션을 멈추는 방법
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                animator.Play("Idle", 0, 0f); // Idle 상태로 애니메이션 재생
            }
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
