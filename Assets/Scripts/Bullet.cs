using UnityEngine;

public class Bullet : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    // 두 개의 스프라이트 필드 추가
    public Sprite bullet0Sprite;  // Bullet0 스프라이트
    public Sprite bullet1Sprite;  // Bullet1 스프라이트

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // SetType 함수로 스프라이트 설정
    public void SetType(int type)
    {
        if (type == 0)
        {
            spriteRenderer.sprite = bullet0Sprite;  // Bullet0 스프라이트 설정
        }
        else if (type == 1)
        {
            spriteRenderer.sprite = bullet1Sprite;  // Bullet1 스프라이트 설정
        }
    }

    
}
