using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnScript : MonoBehaviour
{
    private Stage2Script stage2Script;

    void Start()
    {
        stage2Script = FindObjectOfType<Stage2Script>();
        if(stage2Script == null)
        {
            Debug.LogError("Stage2Script not found in the scene.");
        }
    }

    // 타이머를 40초로 재설정하는 함수
    public void ResetTimer()
    {
        if(stage2Script != null)
        {
            stage2Script.ResetTimer(40f); // 40초로 타이머 재설정
            Debug.Log("Timer reset to 40 seconds via ReturnScript.");
        }
        else
        {
            Debug.LogError("Stage2Script reference is null in ReturnScript.");
        }
    }
}
