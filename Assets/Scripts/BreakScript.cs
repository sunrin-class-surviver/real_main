using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BreakScript : MonoBehaviour
{
    private Player player;  // 플레이어 스크립트 참조

    void Start()
    {
        // 씬에서 Player 스크립트가 있는 객체를 찾습니다.
        player = FindObjectOfType<Player>();
    }

    // 플레이어를 1초 동안 멈추게 하는 함수
    public void FreezePlayerForOneSecond()
    {
        if (player != null)
        {
            player.FreezePlayerForSeconds(10f);  // 1초 동안 멈추게 설정
        }
        else
        {
            Debug.LogError("Player script not found!");
        }
    }
}
