using Oculus.Interaction;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using static UnityEngine.GraphicsBuffer;

public class MeditationController : MonoBehaviour
{
    [Tooltip("Audio")]
    [SerializeField] AudioSource meditationBGAudioSource;
    [SerializeField] AudioSource meditationNarrationAudioSource;
    [SerializeField] AudioClip[] narrations;
    [SerializeField] AudioClip oneTwoThree;

    [Tooltip("Assets")]
    [SerializeField] GameObject tutorialHand;
    [SerializeField] GameObject oneTwoThreeUI;
    [SerializeField] GameObject sphere;

    [SerializeField] Transform playerTransform;
    [SerializeField] float distance;
    [SerializeField] Vector3 offset;

    [Tooltip("Events")]
    [HideInInspector] public UnityEvent onNarration0Finished;
    [HideInInspector] public UnityEvent onNarration1Finished;
    [HideInInspector] public UnityEvent onNarration2Finished;
    [HideInInspector] public UnityEvent onNarration3Finished;
    [HideInInspector] public UnityEvent onNarration4Finished;
    [HideInInspector] public UnityEvent onNarration5Finished;
    [HideInInspector] public UnityEvent onNarration6Finished;
    [HideInInspector] public UnityEvent onNarration7Finished;

    [HideInInspector] public UnityEvent onNarration8Finished;

    [HideInInspector] public UnityEvent onOneTwoThreeFinished;

    private bool isCheckingGesture = false;
    private bool isGestureCorrect = false;
    private bool isButtonPressed = false;
    Animator tutorialHandAnimator;
    GameObject tutorialHandInstance;
    GameObject bubbleInstance;

    [SerializeField] ActiveStateGroup poseActivateStateGroup;
    [SerializeField] ActiveStateGroup gestureActivateStateGroup;

    private void Start()
    {
        tutorialHandAnimator = tutorialHand.GetComponent<Animator>();
    }

    //bind this to the meditation start button
    public void MeditationStartButton()
    {
        if (!isButtonPressed)
        {
            isButtonPressed = true;
            PlayMeditationBGMusic();
            //TODO : make the environment darker

            //���. (pause) ���ݺ��� ��� �Ӹ��� ������ž�.(pause)
            //���� ������ Ǯ��, ���� �� ������ ������ �����غ�.
            PlayNarration(0, onNarration0Finished, false, null);
        }
        
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
        if (checkGesture)
        {
            StartCoroutine(CheckGesture(activateStateGroup));
        }

        yield return new WaitForSeconds(duration);

        if (checkGesture)
        {
            yield return StartCoroutine(WaitForAdditionalTime(3.0f, onComplete));
        }
        else
        {
            onComplete.Invoke();
        }
    }

    private IEnumerator WaitForAdditionalTime(float additionalTime, UnityEvent onComplete)
    {
        float elapsedTime = 0.0f;
        bool hasTriggeredOneTwoThree = false;

        while (elapsedTime < additionalTime)
        {
            if (isGestureCorrect)
            {
                isGestureCorrect = false;
                hasTriggeredOneTwoThree = true;
                PlayOneTwoThree(onComplete);
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (!hasTriggeredOneTwoThree)
        {
            PlayOneTwoThree(onComplete);
        }
        
    }

    private IEnumerator CheckGesture(ActiveStateGroup activateStateGroup)
    {
        isCheckingGesture = true;

        while (isCheckingGesture)
        {
            if (activateStateGroup.Active)
            {
                isGestureCorrect = true;
                //TODO : tutorial hand glows? should do here or on the editor?
                //play one two three
                isCheckingGesture = false;
                yield break;
                
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

        onNarration8Finished.AddListener(OnNarration8Finished);

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

        onNarration8Finished.RemoveListener(OnNarration8Finished);

        //onOneTwoThreeFinished.RemoveListener(OnOneTwoThreeFinished);
    }

    private void InstantiateTutorialHand()
    {
        Vector3 tutorialHandPose = playerTransform.position + playerTransform.forward * distance + offset;
        tutorialHandInstance = Instantiate(tutorialHand, tutorialHandPose, Quaternion.LookRotation(playerTransform.forward));
        tutorialHandAnimator = tutorialHandInstance.GetComponent<Animator>();
    }

    private void InstantiateBubble()
    {
        Vector3 bubblePose = playerTransform.position + playerTransform.forward * distance + offset;
        bubbleInstance = Instantiate(sphere, bubblePose, Quaternion.LookRotation(playerTransform.forward));
    }

    private void OnNarration0Finished()
    {
        //narration1 starts
        //�� �տ� �� �� ������? (�� wave)
        //�� ���� �����ϸ� õõ�� ���� ȣ���� �غ���.
        PlayNarration(1, onNarration1Finished, false, null);
        //tutorial hand shows up in front of the player 
        InstantiateTutorialHand();
        tutorialHandAnimator.SetTrigger("wave");

        //ui shows below the tutorial hand ("������ �����ϼ���")
    }

    private void OnNarration1Finished()
    {
        //narration2 starts
        //��, ���� ���׶��� ������ ���� ũ�� ���� �����. ���� ���ƾ� ��
        PlayNarration(2, onNarration2Finished, true, poseActivateStateGroup);
        StartCoroutine(AnimationDelay(3.0f, "circlePose"));

        //after narration2 ends, Check user does the right pose
        //if they did right,
        //hand glows
        //then play one two three
        //when playing one two three, it's UI pops up
    }

    IEnumerator AnimationDelay(float delay, string stringTrigger)
    {
        yield return new WaitForSeconds(delay);
        tutorialHandAnimator.SetTrigger(stringTrigger);
    }

    private void OnNarration2Finished()
    {
        //narration3 starts
        //�׸��� �״�� ���� ������ �������鼭 ���� ������ �����. 
        PlayNarration(3, onNarration3Finished, true, gestureActivateStateGroup);
        StartCoroutine(AnimationDelay(3.0f, "pushCircle"));
    }

    private void OnNarration3Finished()
    {
        //narration4 starts
        //���߾� �ٽ� �� ��. �տ� �����ؼ� ���� ���̽���. 
        PlayNarration(4, onNarration4Finished, true, poseActivateStateGroup);
        StartCoroutine(AnimationDelay(0.0f, "thumbsUp"));
        //tutorialHandAnimator.SetTrigger("circlePose");
    }

    private void OnNarration4Finished()
    {
        //narration5 starts
        //��� �ӱݰ� �����. 
        PlayNarration(5, onNarration5Finished, true, gestureActivateStateGroup);
        StartCoroutine(AnimationDelay(0.0f, "pushCircle"));
    }

    private void OnNarration5Finished()
    {
        //narration6 starts
        //���� �������̾�. ȣ���� ������ �� ���� ������. 
        PlayNarration(6, onNarration6Finished, true, poseActivateStateGroup);
        StartCoroutine(AnimationDelay(0.0f, "thumbsUp"));
    }

    private void OnNarration6Finished()
    {
        //narration7 starts
        //��� �������� ���� �����. 
        PlayNarration(7, onNarration7Finished, true, gestureActivateStateGroup);
        StartCoroutine(AnimationDelay(0.0f, "pushCircle"));
    }

    private void OnNarration7Finished()
    {
        Destroy(tutorialHand);
        //bubble ����
        InstantiateBubble();
        //particle effect
        //seed ����?
        PlayNarration(8, onNarration8Finished, false, null);
    }

    private void OnNarration8Finished()
    {

    }
}
