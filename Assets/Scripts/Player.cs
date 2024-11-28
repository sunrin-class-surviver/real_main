using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    public float moveSpeed = 7f;  // 이동 속도

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();  // Rigidbody2D 컴포넌트를 가져옴
    }

    private void FixedUpdate()
    {
        // 입력 값 받아오기
        float moveX = Input.GetAxisRaw("Horizontal");  // A/D 또는 Left/Right 화살표
        float moveY = Input.GetAxisRaw("Vertical");    // W/S 또는 Up/Down 화살표

        // 방향 벡터 계산
        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;

        // 이동 속도 적용
        rb.velocity = moveDirection * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D o)
    {
        if (o.gameObject.CompareTag("Bullet"))
        {
            print("Game Over");
        }
    }
}
