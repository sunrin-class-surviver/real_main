using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (gameoverPanel != null)
        {
            gameoverPanel.SetActive(false); // 게임 시작 시 Game Over 패널 비활성화
        }
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

    // 플레이어의 움직임을 멈추는 함수 (지정된 시간 동안)
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

            // 'die' 오디오 재생
            AudioHelper.PlayDieAudio();

            // 5초 후에 자동으로 게임을 다시 시작하도록 Coroutine 시작
            StartCoroutine(RestartAfterDelay(5f));
        }
        else
        {
            Debug.LogError("Game Over Panel is not assigned!");
        }
    }

    // 5초 후에 "Beginning" 씬으로 전환하는 Coroutine
    private IEnumerator RestartAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // Time.timeScale이 0이므로 real time 사용

        // 씬을 다시 시작하기 전에 게임 시간 복구
        Time.timeScale = 1;

        // 'Beginning' 씬으로 로드
        SceneManager.LoadScene("Beginning");

        // 'init' 오디오 재생
        AudioHelper.PlayInitAudio();
    }

    // Update 메서드 제거: Enter 키 관련 로직 삭제
}
