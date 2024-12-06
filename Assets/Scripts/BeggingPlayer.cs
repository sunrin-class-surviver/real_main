using System.Collections;
using UnityEngine;

public class BeggingPlayer : MonoBehaviour
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

    private void FixedUpdate()
    {
        if (canMove)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");
            movement = new Vector2(moveX, moveY).normalized;

            rb.velocity = movement * moveSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        onMovement(); // 방향별 애니메이션 갱신
    }

    private void onMovement()
    {
        // 모든 방향 파라미터 초기화
        animator.SetBool("IsMovingRight", false);
        animator.SetBool("IsMovingLeft", false);
        animator.SetBool("IsMovingUp", false);
        animator.SetBool("IsMovingDown", false);
        animator.SetBool("IsRunning", false);

        // 이동 방향에 따라 파라미터 설정
        if (movement.x > 0)
        {
            animator.SetBool("IsMovingRight", true);
        }
        else if (movement.x < 0)
        {
            animator.SetBool("IsMovingLeft", true);
        }
        else if (movement.y > 0)
        {
            animator.SetBool("IsMovingUp", true);
        }
        else if (movement.y < 0)
        {
            animator.SetBool("IsMovingDown", true);
        }

        // 이동 중이면 IsRunning 활성화
        if (movement != Vector2.zero)
        {
            animator.SetBool("IsRunning", true);
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
}
