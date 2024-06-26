using Oculus.Interaction;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class MeditationController : MonoBehaviour
{
    [Tooltip("Audio")]
    [SerializeField] AudioSource meditationBGAudioSource;
    [SerializeField] AudioSource meditationNarrationAudioSource;
    [SerializeField] AudioClip[] narrations;
    [SerializeField] AudioClip oneTwoThree;

    [Tooltip("Assets")]
    [SerializeField] GameObject tutorialHand;
    [SerializeField] Animator tutorialHandAnimator;
    [SerializeField] GameObject oneTwoThreeUI;
    [SerializeField] GameObject sphere;

    [SerializeField] Transform playerTransform;

    [Tooltip("Events")]
    [HideInInspector] public UnityEvent onNarration0Finished;
    [HideInInspector] public UnityEvent onNarration1Finished;
    [HideInInspector] public UnityEvent onNarration2Finished;
    [HideInInspector] public UnityEvent onNarration3Finished;
    [HideInInspector] public UnityEvent onNarration4Finished;
    [HideInInspector] public UnityEvent onNarration5Finished;
    [HideInInspector] public UnityEvent onNarration6Finished;
    [HideInInspector] public UnityEvent onNarration7Finished;

    [HideInInspector] public UnityEvent onOneTwoThreeFinished;

    private bool isCheckingGesture = false;
    [SerializeField] ActiveStateGroup poseActivateStateGroup;
    [SerializeField] ActiveStateGroup gestureActivateStateGroup;

    private void Start()
    {
        tutorialHand.SetActive(false);
        oneTwoThreeUI.SetActive(false);
        oneTwoThreeUI.SetActive(false);
    }

    //bind this to the meditation start button
    public void MeditationStartButton()
    {
        PlayMeditationBGMusic();
        //TODO : make the environment darker

        //���. ���ݺ��� ��� �Ӹ��� ����� �ž�. ��, ���� ������ Ǯ��, ���� �� ������ ������ ������ ��.
        PlayNarration(0, onNarration0Finished, false, null);
    }

    private void PlayMeditationBGMusic()
    {
        meditationBGAudioSource.Play();
    }

    private void PlayNarration(int narrationIndex, UnityEvent onComplete, bool checkGesture, ActiveStateGroup activateStateGroup)
    {
        meditationNarrationAudioSource.clip = narrations[narrationIndex];
        meditationNarrationAudioSource.Play();
        StartCoroutine(WaitForNarrationToEnd(narrations[narrationIndex].length, onComplete, checkGesture, activateStateGroup));
    }

    IEnumerator WaitForNarrationToEnd(float duration, UnityEvent onComplete, bool checkGesture, ActiveStateGroup activateStateGroup)
    {
        yield return new WaitForSeconds(duration);
        if (checkGesture)
        {
            StartCoroutine(CheckGesture(onComplete, activateStateGroup));
        }
        else
        {
            onComplete.Invoke();
        }
    }
    private IEnumerator CheckGesture(UnityEvent onComplete, ActiveStateGroup activateStateGroup)
    {
        isCheckingGesture = true;

        while (isCheckingGesture)
        {
            if (activateStateGroup.Active)
            {
                isCheckingGesture = false;
                //TODO : tutorial hand glows? should do here or on the editor?
                //play one two three
                PlayOneTwoThree(onComplete);
            }
            yield return null;
        }
    }

    private void PlayOneTwoThree(UnityEvent onComplete)
    {
        meditationNarrationAudioSource.clip = oneTwoThree;
        meditationNarrationAudioSource.Play();
        oneTwoThreeUI.SetActive(true);
        //TODO : also have to play this ui's animation?
        StartCoroutine(WaitForOneTwoThreeToEnd(oneTwoThree.length, onComplete));
    }

    IEnumerator WaitForOneTwoThreeToEnd(float duration, UnityEvent onComplete)
    {
        yield return new WaitForSeconds(duration);
        oneTwoThreeUI.SetActive(false);
        onComplete.Invoke();
    }

    private void OnEnable()
    {
        onNarration0Finished.AddListener(OnNarration0Finished);
        onNarration1Finished.AddListener(OnNarration1Finished);
        onNarration2Finished.AddListener(OnNarration2Finished);
        onNarration3Finished.AddListener(OnNarration3Finished);
        onNarration4Finished.AddListener(OnNarration4Finished);
        onNarration5Finished.AddListener(OnNarration5Finished);
        onNarration6Finished.AddListener(OnNarration6Finished);
        onNarration7Finished.AddListener(OnNarration7Finished);
        
        //onOneTwoThreeFinished.AddListener(OnOneTwoThreeFinished);
    }

    private void OnDisable()
    {
        onNarration0Finished.RemoveListener(OnNarration0Finished);
        onNarration1Finished.RemoveListener(OnNarration1Finished);
        onNarration2Finished.RemoveListener(OnNarration2Finished);
        onNarration3Finished.RemoveListener(OnNarration3Finished);
        onNarration4Finished.RemoveListener(OnNarration4Finished);
        onNarration5Finished.RemoveListener(OnNarration5Finished);
        onNarration6Finished.RemoveListener(OnNarration6Finished);
        onNarration7Finished.RemoveListener(OnNarration7Finished);
        
        //onOneTwoThreeFinished.RemoveListener(OnOneTwoThreeFinished);
    }

    private void OnNarration0Finished()
    {
        //narration1 starts
        //�� �տ� ���̴� �� �� ������? �Ȱ��� �ڼ��� ���� ��, �� �� ������ ������ �� �������� �Ҿ�ִ´� �����ϰ� õõ�� ȣ���� �غ���.
        PlayNarration(1, onNarration1Finished, false, null);
        //tutorial hand shows up in front of the player 
        tutorialHand.SetActive(true);
        tutorialHandAnimator.SetTrigger("wave");
        //ui shows below the tutorial hand ("������ �����ϼ���")
    }

    private void OnNarration1Finished()
    {
        //narration2 starts
        //���� ũ�� ���̽��鼭 ���� ������. 
        PlayNarration(2, onNarration2Finished, true, poseActivateStateGroup);
        tutorialHandAnimator.SetTrigger("circlePose");

        //after narration2 ends, Check user does the right pose
        //if they did right,
        //hand glows
        //then play one two three
        //when playing one two three, it's UI pops up
    }

    private void OnNarration2Finished()
    {
        //narration3 starts
        //�׸��� �״�� ���� ������ �������鼭 ���� ������ �����. 
        PlayNarration(3, onNarration3Finished, true, gestureActivateStateGroup);
        tutorialHandAnimator.SetTrigger("pushCircle");
    }

    private void OnNarration3Finished()
    {
        //narration4 starts
        //���߾� �ٽ� �� ��. �տ� �����ؼ� ���� ���̽���. 
        PlayNarration(4, onNarration4Finished, true, poseActivateStateGroup);
        tutorialHandAnimator.SetTrigger("circlePose");
    }

    private void OnNarration4Finished()
    {
        //narration5 starts
        //��� �ӱݰ� �����. 
        PlayNarration(5, onNarration5Finished, true, gestureActivateStateGroup);
        tutorialHandAnimator.SetTrigger("pushCircle");
    }

    private void OnNarration5Finished()
    {
        //narration6 starts
        //���� �������̾�. ȣ���� ������ �� ���� ������. 
        PlayNarration(6, onNarration6Finished, true, poseActivateStateGroup);
        tutorialHandAnimator.SetTrigger("circlePose");
    }

    private void OnNarration6Finished()
    {
        //narration7 starts
        //��� �������� ���� �����. 
        PlayNarration(7, onNarration7Finished, true, gestureActivateStateGroup);
        tutorialHandAnimator.SetTrigger("pushCircle");
    }

    private void OnNarration7Finished()
    {
        //bubble ����
        Vector3 spherePos = playerTransform.position + playerTransform.forward * 2.0f;
        Instantiate(sphere, spherePos, Quaternion.identity);
        //particle effect
        //seed ����?
        
    }
}
