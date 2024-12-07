using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator; // Animator 컴포넌트
    public float moveSpeed = 7f; // 이동 속도
    private bool canMove = true; // 플레이어가 움직일 수 있는지 여부
    
    public Stage2Script stage2Script;
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
                // 애니메이션을 멈추고 Idle 상태로 설정
                animator.SetBool("IsMoving", false); // Idle 상태로 변경
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    animator.Play("Idle", 0, 0f); // Idle 상태로 애니메이션 재생
                }
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
        string bulletTag = o.tag;

        if (bulletTag == "Bullet") // 기본 총알과의 충돌
        {
            Debug.Log("Game Over: Hit by Bullet!");
            ShowGameOverPanel();
        }
        else if (bulletTag == "for" || bulletTag == "while" || bulletTag == "if" || bulletTag == "break" || bulletTag == "return")
        {
            Debug.Log($"Bullet collision detected: {bulletTag}");
            if (stage2Script != null)
            {
                stage2Script.HandleBulletCollision(bulletTag);
            }
            Destroy(o.gameObject); // 특수 총알 파괴
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

    // Update 메서드 추가: Game Over 패널이 활성화된 상태에서 Enter 키 입력을 감지
    private void Update()
    {
        if (gameoverPanel != null && gameoverPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                RestartGame();
            }
        }
    }

    // 게임을 다시 시작하는 함수: "Beginning" 씬으로 로드
    private void RestartGame()
    {
        Time.timeScale = 1; // 시간 스케일 복구
        SceneManager.LoadScene("Beginning"); // "Beginning" 씬으로 로드
    }
}
