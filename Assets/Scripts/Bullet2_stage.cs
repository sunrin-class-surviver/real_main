using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    // Stage2Script 인스턴스
    public Stage2Script stage2Script;  // Stage2Script를 Inspector에서 할당
    // Start 메서드
    void Start()
    {
        if (stage2Script == null)
        {
            Debug.LogError("Stage2Script is not assigned!");
        }
    }

    // 총알이 충돌했을 때 호출되는 메서드
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("충돌한 태그: " + other.tag);  // 충돌한 태그 확인

        // 'Player'와 충돌했을 때
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Player hit by bullet!");
            StartCoroutine(HandleBulletCollision(other.gameObject));  // 총알 충돌 처리 코루틴 호출
        }
    }

    // 총알 충돌을 처리하는 코루틴
    public IEnumerator HandleBulletCollision(GameObject player)
    {
        string bulletTag = gameObject.tag;

        // 충돌한 총알의 태그에 따라 처리
        if (bulletTag == "for")
        {
            Debug.Log("For bullet hit the player!");
            stage2Script.HandleBulletCollision("for");  // Stage2Script에 상태 전달
        }
        else if (bulletTag == "while")
        {
            Debug.Log("While bullet hit the player!");
            stage2Script.HandleBulletCollision("while");  // Stage2Script에 상태 전달
        }
        else if (bulletTag == "break")
        {
            Debug.Log("Break bullet hit the player!");
            stage2Script.HandleBulletCollision("break");  // Stage2Script에 상태 전달
        }

        // 총알 파괴
        Destroy(gameObject);  // 총알 파괴
        yield return null;
    }
}
