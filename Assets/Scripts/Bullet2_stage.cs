using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    // 충돌 처리
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어가 Bullet_Stage2와 충돌한 경우 처리
            Debug.Log("제발 충돌됨");
        }
        
        if (other.CompareTag("Bullet_Stage2"))
        {
            // Bullet_Stage2끼리 충돌한 경우 처리
            Debug.Log("Bullet_Stage2 hit something!");
            // Bullet_Stage2를 파괴하거나 다른 처리를 할 수 있음
            Destroy(other.gameObject); // Bullet_Stage2 파괴
        }
    }
}
