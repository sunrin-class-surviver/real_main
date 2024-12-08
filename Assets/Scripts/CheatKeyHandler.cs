using UnityEngine;
using UnityEngine.SceneManagement;

public static class CheatKeyHandler
{
    /// <summary>
    /// 치트키 입력을 확인하고, 해당하는 씬으로 전환합니다.
    /// 이 함수를 각 스크립트의 Update() 메서드에서 호출해야 합니다.
    /// </summary>
    public static void CheckCheatKeys()
    {
        // 키 '0'부터 '9'까지 확인
        for (int i = 0; i <= 10; i++)
        {
            // '0'부터 '9'까지는 Alpha0부터 Alpha9까지
            // '10'번째 키는 F1으로 대체
            if (i <= 9)
            {
                KeyCode key = KeyCode.Alpha0 + i; // KeyCode.Alpha0, KeyCode.Alpha1, ..., KeyCode.Alpha9
                if (Input.GetKeyDown(key))
                {
                    LoadSceneByKey(i);
                }
            }
            else if (i == 10)
            {
                // 키 '10'은 F1 키로 대체
                if (Input.GetKeyDown(KeyCode.F1))
                {
                    LoadSceneByKey(i);
                }
            }
        }
    }

    /// <summary>
    /// 키 번호에 따라 씬을 로드합니다.
    /// </summary>
    /// <param name="keyNumber">키 번호 (0-10)</param>
    private static void LoadSceneByKey(int keyNumber)
    {
        // Time.timeScale을 1로 설정하여 시간 흐름을 정상화
        Time.timeScale = 1f;

        string sceneName = GetSceneNameByKey(keyNumber);
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning($"Cheat key {keyNumber} pressed, but no scene is mapped.");
            return;
        }

        Debug.Log($"Cheat Key {keyNumber} pressed. Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// 키 번호에 해당하는 씬 이름을 반환합니다.
    /// </summary>
    /// <param name="keyNumber">키 번호 (0-10)</param>
    /// <returns>씬 이름</returns>
    private static string GetSceneNameByKey(int keyNumber)
    {
        switch (keyNumber)
        {
            case 0:
                return "Beginning";
            case 1:
                return "Stage1Connection";
            case 2:
                return "Stage1Loading";
            case 3:
                return "Stage1";
            case 4:
                return "Stage2Connection";
            case 5:
                return "Stage2Loading";
            case 6:
                return "Stage2";
            case 7:
                return "Stage3Connection";
            case 8:
                return "Stage3Loading";
            case 9:
                return "Stage3";
            case 10:
                return "Ending";
            default:
                return string.Empty;
        }
    }
}
