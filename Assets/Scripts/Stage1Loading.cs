using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Stage1Loading : MonoBehaviour
{
    // 딜레이 시간을 초 단위로 설정
    [SerializeField]
    private float time = 5f;

    // Start is called before the first frame update
    void Start()
    {
        // LoadNextSceneAfterDelay 코루틴 시작
        StartCoroutine(LoadNextSceneAfterDelayCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        // 필요 시 업데이트 로직 추가
    }

    private IEnumerator LoadNextSceneAfterDelayCoroutine()
    {
        // 지정된 시간 동안 대기
        yield return new WaitForSeconds(time);
        
        // 씬 전환
        LoadNextSceneAfterDelay();
    }

    private void LoadNextSceneAfterDelay()
    {
        Debug.Log("stage1로 전환합니다.");
        SceneManager.LoadScene("Stage1");
    }
}
