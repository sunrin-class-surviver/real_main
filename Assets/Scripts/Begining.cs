using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Beginning : MonoBehaviour
{
    // Dialogue 데이터 클래스
    [System.Serializable]
    public class Dialogue
    {
        public string dialogueText;
    }

    public List<Dialogue> dialogues; // 대화 시퀀스를 저장할 리스트

    // UI 요소
    
    public TextMeshProUGUI dialogueText;

    // 대화 진행 상태
    private int currentDialogueIndex = 0;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    public AudioClip initClip;
    public AudioClip battleClip;
    public AudioClip dieClip; // die 오디오 클립 추가
    public AudioClip audioClip;


    void Start()
    {
        // 대화 상자 비활성화 (초기 상태)
        dialogueText.gameObject.SetActive(false);
        // 대화 초기화
        StartCoroutine(InitializeDialogue());

         AudioHelper.Initialize(initClip, battleClip, dieClip, audioClip);

        // 'battle' 오디오 재생
        AudioHelper.PlayInitAudio();
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
                dialogueText.text = dialogues[currentDialogueIndex].dialogueText;
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

    void DisplayDialogue(Dialogue dialogue)
    {
        dialogueText.text = "";

        typingCoroutine = StartCoroutine(TypeSentence(dialogue.dialogueText));
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

    IEnumerator EndDialogueSequence()
    {
        // 모든 대화가 끝난 후 다음 스테이지로 전환하기 전에 잠시 대기
        yield return new WaitForSeconds(1f); // 원하는 대기 시간 설정

        // BeggingPlayer 스크립트의 LoadNextStage() 호출
        BeggingPlayer beggingPlayer = FindObjectOfType<BeggingPlayer>();
        if (beggingPlayer != null)
        {
           beggingPlayer.LoadNextSceneAfterDelay();
        }
        else
        {
            Debug.LogError("BeggingPlayer 스크립트를 찾을 수 없습니다.");
        }
    }
}
