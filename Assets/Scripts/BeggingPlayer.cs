using System.Collections;
using UnityEngine;

public class BeggingPlayer : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator; // Animator 컴포넌트
    public float moveSpeed = 7f; // 이동 속도
    private bool canMove = true; // 플레이어가 움직일 수 있는지 여부

    public GameObject gameoverPanel; // Game Over Panel

<<<<<<< Updated upstream
=======
    private Vector2 movement;
    private Vector2 lastDirection = Vector2.down; // 초기 방향을 아래쪽으로 설정

>>>>>>> Stashed changes
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D 컴포넌트를 가져옴
        animator = GetComponent<Animator>(); // Animator 컴포넌트를 가져옴
    }

<<<<<<< Updated upstream
=======
    private void Start()
    {
        // 초기 애니메이션 파라미터 설정
        animator.SetBool("IsMoving", false);
        animator.SetFloat("X", lastDirection.x);
        animator.SetFloat("Y", lastDirection.y);

        Debug.Log("Start() called. Initial lastDirection: " + lastDirection + ", IsMoving: false");
    }

>>>>>>> Stashed changes
    private void FixedUpdate()
    {
        if (canMove) // canMove가 true일 때만 이동 가능
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");
            Vector2 moveDirection = new Vector2(moveX, moveY).normalized;
            rb.velocity = moveDirection * moveSpeed;

            UpdateAnimation(moveX, moveY); // 이동 방향에 따라 애니메이션 갱신
        }
        else
        {
            rb.velocity = Vector2.zero; // 움직이지 않게끔 속도 0으로 설정
        }
    }

    // 애니메이션 업데이트 함수
    private void UpdateAnimation(float moveX, float moveY)
    {
        if (animator != null)
        {
<<<<<<< Updated upstream
            animator.SetFloat("X", moveX);
            animator.SetFloat("Y", moveY);
            animator.SetBool("IsMoving", moveX != 0 || moveY != 0); // 움직이는지 여부
=======
            lastDirection = movement; // 마지막 이동 방향 업데이트

            // 모든 트리거 리셋
            ResetAllTriggers();

            // 대각선 이동 처리: 수평 이동을 우선으로 함
            if (movement.x != 0)
            {
                if (movement.x == -1 && movement.y==0)
                {
                    animator.SetTrigger("R"); // 오른쪽 이동 트리거
                }
                else if (movement.x == 1 && movement.y==0)
                {
                    animator.SetTrigger("L"); // 왼쪽 이동 트리거
                }
            }
            else
            {
                if (movement.y == 1 && movement.x==0)
                {
                    animator.SetTrigger("B"); // 위로 이동 트리거
                }
                else if (movement.y == -1 && movement.x== 0)
                {
                    animator.SetTrigger("F"); // 아래로 이동 트리거
                }
            }

            animator.SetBool("IsMoving", true); // 이동 상태 설정

            Debug.Log("Animator Parameters - X: " + movement.x + ", Y: " + movement.y + ", IsMoving: true");
>>>>>>> Stashed changes
        }
        else
        {
<<<<<<< Updated upstream
            Debug.LogError("Animator is not assigned!");
=======
            animator.SetBool("IsMoving", false); // Idle 상태
            animator.SetFloat("X", lastDirection.x); // 마지막 이동 방향 유지
            animator.SetFloat("Y", lastDirection.y); // 마지막 이동 방향 유지

            Debug.Log("Animator Parameters - X: " + lastDirection.x + ", Y: " + lastDirection.y + ", IsMoving: false");

            // IsMoving이 false일 때 Idle 상태로 전환
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                animator.Play("Idle", 0, 0f); // Idle 상태로 애니메이션 재생
                Debug.Log("Animator transitioned to Idle state.");
            }
>>>>>>> Stashed changes
        }

        // 현재 재생 중인 애니메이션 클립 로그 출력
        LogCurrentAnimation();
    }

    // 모든 트리거를 리셋하는 메소드
    private void ResetAllTriggers()
    {
        animator.ResetTrigger("R");
        animator.ResetTrigger("L");
        animator.ResetTrigger("F");
        animator.ResetTrigger("B");
        animator.ResetTrigger("I"); // 필요 시 추가 트리거도 리셋
    }

    // 현재 재생 중인 애니메이션 클립을 로그로 출력하는 메소드
    private void LogCurrentAnimation()
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0)
        {
            Debug.Log("Current Animation Clip: " + clipInfo[0].clip.name);
        }
        else
        {
            Debug.Log("No Animation Clip is currently playing.");
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
        Debug.Log("Player movement frozen for " + duration + " seconds.");
        yield return new WaitForSeconds(duration);
        canMove = true;
        Debug.Log("Player movement resumed.");
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
            Debug.Log("Game Over Panel activated. Time.timeScale set to 0.");
        }
        else
        {
            Debug.LogError("Game Over Panel is not assigned!");
        }
    }
}
