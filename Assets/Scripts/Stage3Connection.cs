using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class Stage3Connection : MonoBehaviour
{
       // Start is called before the first frame update
    public List<string> dialogues; // 각 대화는 "DialogueText" 형식의 문자열

    // UI 요소
    public TextMeshProUGUI dialogueText;

    // 대화 진행 상태
    private int currentDialogueIndex = 0;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

      public AudioClip initClip;
    public AudioClip battleClip;
    public AudioClip audioClip;
    public AudioClip dieClip; // die 오디오 클립 추가


    void Start()
    {

        AudioHelper.Initialize(initClip, battleClip, dieClip, audioClip);

        // 'battle' 오디오 재생
        AudioHelper.PlayInitAudio();
        // 대화 상자 비활성화 (초기 상태)
        dialogueText.gameObject.SetActive(false);

        // 대화 초기화
        StartCoroutine(InitializeDialogue());
    }

    void Update()
    {
        // 대화가 활성화된 상태에서 Enter 키를 누르면 다음 대사로 진행
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Return))
        {
            if (isTyping)
            {
                // 타이핑 중이면 남은 텍스트를 모두 표시
                StopCoroutine(typingCoroutine);
                dialogueText.text = GetCurrentDialogueText();
                isTyping = false;
            }
            else
            {
                DisplayNextDialogue();
            }
        }

         CheatKeyHandler.CheckCheatKeys();
    }

    IEnumerator InitializeDialogue()
    {
        // 대화 상자 활성화
        dialogueText.gameObject.SetActive(true);
        isDialogueActive = true;

        // 첫 번째 대화 표시
        DisplayDialogue(dialogues[currentDialogueIndex]);
        yield return null;
    }

    void DisplayNextDialogue()
    {
        currentDialogueIndex++;

        if (currentDialogueIndex < dialogues.Count)
        {
            DisplayDialogue(dialogues[currentDialogueIndex]);
        }
        else
        {
            // 모든 대화가 끝났을 때
            isDialogueActive = false;
            StartCoroutine(EndDialogueSequence());
        }
    }

    void DisplayDialogue(string dialogueLine)
    {
        dialogueText.text = "";

        typingCoroutine = StartCoroutine(TypeSentence(dialogueLine));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.02f); // 타이핑 속도 조절
        }
        isTyping = false;
    }

    string GetCurrentDialogueText()
    {
        return dialogues[currentDialogueIndex];
    }

    IEnumerator EndDialogueSequence()
    {
        // 모든 대화가 끝난 후 다음 스테이지로 전환하기 전에 잠시 대기
        yield return new WaitForSeconds(1f); // 원하는 대기 시간 설정

        // LoadNextSceneAfterDelay() 메서드 호출
        LoadNextSceneAfterDelay();
    }


    private void LoadNextSceneAfterDelay()
    {
        // 타이머 종료 시 Stage2로 전환
        Debug.Log("stage3로 전환합니다.");
        SceneManager.LoadScene("Stage3Loading");
    }
}
