using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;  // 이동 속도
    private Rigidbody2D rb;       // Rigidbody2D 컴포넌트

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();  // Rigidbody2D 컴포넌트를 가져옴
    }

    private void Update()
    {
        // 플레이어의 이동 입력
        float horizontal = Input.GetAxis("Horizontal");  // 좌우 입력 (A/D 또는 화살표)
        float vertical = Input.GetAxis("Vertical");      // 상하 입력 (W/S 또는 화살표)

        // 이동 벡터 계산
        Vector2 movement = new Vector2(horizontal, vertical).normalized;

        // Rigidbody2D에 이동 벡터를 적용하여 이동
        rb.velocity = movement * moveSpeed;
    }

    // 플레이어의 사망 처리 (예: "Game Over" 메시지)
     void OnDeath()
    {
        Debug.Log("Game Over");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌이 발생했는지 먼저 확인
        Debug.Log("Collision detected with " + collision.gameObject.name); // 충돌한 객체의 이름 출력

        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("Player hit by Bullet!"); // 총알과 충돌한 경우
            OnDeath();  // 사망 처리
            Destroy(gameObject);  // 플레이어 객체 삭제
        }
    }
}
