using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Stage3Connection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LoadNextSceneAfterDelay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LoadNextSceneAfterDelay()
    {
        // 타이머 종료 시 Stage2로 전환
        Debug.Log("Stage3으로 전환합니다.");
        SceneManager.LoadScene("Stage3");
    }
}
