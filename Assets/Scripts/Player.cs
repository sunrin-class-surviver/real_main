using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    public float moveSpeed = 7f;  // 이동 속도

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();  // Rigidbody2D 컴포넌트를 가져옴
    }

    private void Update()
    {
        // 입력 값 받아오기 (화살표 키나 WASD 키 사용)
        float moveX = Input.GetAxisRaw("Horizontal");  // A/D 또는 Left/Right 화살표
        float moveY = Input.GetAxisRaw("Vertical");    // W/S 또는 Up/Down 화살표

        // 입력에 따라 이동 벡터 계산
        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;  // 방향 벡터

        // 물리 기반으로 이동
        rb.velocity = moveDirection * moveSpeed;  // 속도 적용
    }
}
