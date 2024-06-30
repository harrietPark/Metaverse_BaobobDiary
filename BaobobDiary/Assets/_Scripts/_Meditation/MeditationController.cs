using Oculus.Interaction;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Meta.XR.MRUtilityKit;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.SceneManagement;
using EasyTransition;

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
    [SerializeField] GameObject bubble;

    [SerializeField] Transform playerTransform;
    [SerializeField] float tutorialHandDistance;
    [SerializeField] float bubbleDistance;
    [SerializeField] Vector3 tutorialHandOffset;
    [SerializeField] Vector3 bubbleOffset;

    [SerializeField] SkinnedMeshRenderer leftHand;
    [SerializeField] SkinnedMeshRenderer rightHand;
    [SerializeField] Material defaultMat;
    [SerializeField] Material highlightMat;

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
    [HideInInspector] public UnityEvent onNarration9Finished;
    [HideInInspector] public UnityEvent onNarration10Finished;

    [HideInInspector] public UnityEvent onOneTwoThreeFinished;

    private bool isCheckingGesture = false;
    private bool isGestureCorrect = false;
    private bool isButtonPressed = false;
    Animator tutorialHandAnimator;
    GameObject tutorialHandInstance;

    [SerializeField] ActiveStateGroup poseActivateStateGroup;
    [SerializeField] ActiveStateGroup gestureActivateStateGroup;
    [SerializeField] ActiveStateGroup scalePoseActiveStateGroup;

    [SerializeField] private OVRPassthroughLayer _passthroughLayer;
    [SerializeField] ScaleInteractionVisual scaleInteractionVisual;

    public TransitionSettings transition;
    public float loadDelay;

    private void Start()
    {
        tutorialHandAnimator = tutorialHand.GetComponent<Animator>();
        bubble.SetActive(false);
    }

    //bind this to the meditation start button
    public void MeditationStartButton()
    {
        if (!isButtonPressed)
        {
            isButtonPressed = true;
            PlayMeditationBGMusic();
            //TODO : make the environment darker
            StartCoroutine(AdjustLighting(-0.5f, 4f));
            //���. (pause) ���ݺ��� ��� �Ӹ��� ������ž�.(pause)
            //���� ������ Ǯ��, ���� �� ������ ������ �����غ�.
            PlayNarration(0, onNarration0Finished, false, null, false);
        }
        
    }

    private IEnumerator AdjustLighting(float targetBrightness, float duration)
    {
        if (_passthroughLayer != null)
        {
            float startBrightness = _passthroughLayer.colorMapEditorBrightness;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float newBrightness = Mathf.Lerp(startBrightness, targetBrightness, elapsedTime / duration);
                _passthroughLayer.SetBrightnessContrastSaturation(newBrightness);
                yield return null;
            }

            // Ensure the final brightness is set
            _passthroughLayer.SetBrightnessContrastSaturation(targetBrightness);
        }
        else
        {
            Debug.LogError("Passthrough layer is not assigned.");
        }
    }

    private void PlayMeditationBGMusic()
    {
        meditationBGAudioSource.Play();
    }

    private void PlayNarration(int narrationIndex, UnityEvent onComplete, bool checkGesture, ActiveStateGroup activateStateGroup, bool passOneTwoThree)
    {
        meditationNarrationAudioSource.clip = narrations[narrationIndex];
        meditationNarrationAudioSource.Play();
        leftHand.sharedMaterial = defaultMat;
        rightHand.sharedMaterial = defaultMat;
        StartCoroutine(WaitForNarrationToEnd(narrations[narrationIndex].length, onComplete, checkGesture, activateStateGroup, passOneTwoThree));
    }

    IEnumerator WaitForNarrationToEnd(float duration, UnityEvent onComplete, bool checkGesture, ActiveStateGroup activateStateGroup, bool passOneTwoThree)
    {
        if (checkGesture)
        {
            StartCoroutine(CheckGesture(activateStateGroup));
        }

        yield return new WaitForSeconds(duration);

        if (checkGesture && !passOneTwoThree)
        {
            yield return StartCoroutine(WaitForAdditionalTime(3.0f, onComplete));
        }
        else if(!checkGesture)
        {
            onComplete.Invoke();
        }
        else if(checkGesture && passOneTwoThree)
        {
            StartCoroutine(WaitMore(3f, onComplete));
        }
    }

    private IEnumerator WaitMore(float additionalTime, UnityEvent onComplete)
    {
        float elapsedTime = 0.0f;

        while(elapsedTime < additionalTime)
        {
            if (isGestureCorrect)
            {
                isGestureCorrect = false;
                onComplete.Invoke();
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        while (true)
        {
            if (isGestureCorrect)
            {
                isGestureCorrect = false;
                onComplete.Invoke();
                yield break;
            }
            yield return null;
        }
        //�ٽ� �϶�� UI

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
                leftHand.sharedMaterial = highlightMat;
                rightHand.sharedMaterial = highlightMat;
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
        onNarration9Finished.AddListener(OnNarration9Finished);
        onNarration10Finished.AddListener(OnNarration10Finished);

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
        onNarration9Finished.RemoveListener(OnNarration9Finished);
        onNarration10Finished.RemoveListener(OnNarration10Finished);

        //onOneTwoThreeFinished.RemoveListener(OnOneTwoThreeFinished);
    }

    private void InstantiateTutorialHand()
    {
        Vector3 tutorialHandPose = playerTransform.position + playerTransform.forward * tutorialHandDistance + tutorialHandOffset;
        tutorialHandInstance = Instantiate(tutorialHand, tutorialHandPose, Quaternion.LookRotation(playerTransform.forward));
        tutorialHandAnimator = tutorialHandInstance.GetComponent<Animator>();
    }

    private void InstantiateBubble()
    {
        Vector3 bubblePose = playerTransform.position + playerTransform.forward * bubbleDistance + bubbleOffset;
        bubble.transform.position = bubblePose;
        bubble.transform.rotation = Quaternion.LookRotation(playerTransform.forward);
        bubble.SetActive(true);
    }

    private void OnNarration0Finished()
    {
        //narration1 starts
        //�� �տ� �� �� ������? (�� wave)
        //�� ���� �����ϸ� õõ�� ���� ȣ���� �غ���.
        PlayNarration(1, onNarration1Finished, false, null, false);
        //tutorial hand shows up in front of the player 
        InstantiateTutorialHand();
        tutorialHandAnimator.SetTrigger("wave");

        //ui shows below the tutorial hand ("������ �����ϼ���")
    }

    private void OnNarration1Finished()
    {
        //narration2 starts
        //��, ���� ���׶��� ������ ���� ũ�� ���� �����. ���� ���ƾ� ��
        PlayNarration(2, onNarration2Finished, true, poseActivateStateGroup, false);
        StartCoroutine(AnimationDelay(3.0f, "circlePose"));
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
        PlayNarration(3, onNarration3Finished, true, gestureActivateStateGroup, false);
        StartCoroutine(AnimationDelay(3.0f, "pushCircle"));
    }

    private void OnNarration3Finished()
    {
        //narration4 starts
        //���߾� �ٽ� �� ��. �տ� �����ؼ� ���� ���̽���. 
        PlayNarration(4, onNarration4Finished, true, poseActivateStateGroup, false);
        StartCoroutine(AnimationDelay(0.0f, "thumbsUp"));
    }

    private void OnNarration4Finished()
    {
        //narration5 starts
        //��� �ӱݰ� �����. 
        PlayNarration(5, onNarration5Finished, true, gestureActivateStateGroup, false);
        StartCoroutine(AnimationDelay(0.0f, "pushCircle"));
    }

    private void OnNarration5Finished()
    {
        //narration6 starts
        //���� �������̾�. ȣ���� ������ �� ���� ������. 
        PlayNarration(6, onNarration6Finished, true, poseActivateStateGroup, false);
        StartCoroutine(AnimationDelay(0.0f, "thumbsUp"));
    }

    private void OnNarration6Finished()
    {
        //narration7 starts
        //��� �������� ���� �����. 
        PlayNarration(7, onNarration7Finished, true, gestureActivateStateGroup, false);
        StartCoroutine(AnimationDelay(0.0f, "pushCircle"));
    }

    private void OnNarration7Finished()
    {
        StartCoroutine(ShowBubble(1.0f, 2.0f));
        StartCoroutine(AdjustLighting(0.0f, 4f));
    }

    IEnumerator ShowBubble(float delay1, float delay2)
    {
        Destroy(tutorialHandInstance);
        yield return new WaitForSeconds(delay1);
        InstantiateBubble();
        yield return new WaitForSeconds(delay2);
        PlayNarration(8, onNarration8Finished, false, null, false);
        
    }

    private void OnNarration8Finished()
    {
        //����° �հ����� ������ ��ƺ�. 
        PlayNarration(9, onNarration9Finished, true, scalePoseActiveStateGroup, true);
    }

    private void OnNarration9Finished()
    {
        //�׸��� ��ܺ�. �����ִ� �ִ�������. 
        meditationNarrationAudioSource.clip = narrations[10];
        meditationNarrationAudioSource.Play();
        scaleInteractionVisual.maxScaledForThreeSecs.AddListener(OnMaxScaledForThreeSecs);
    }

    private void OnNarration10Finished()
    {
        //�׸��� 3�ʸ� ����. �ϳ� �� �� 
        //PlayNarration(11, null, false, null, true);
    }

    private void OnMaxScaledForThreeSecs()
    {
        //scene transtiion
        Debug.Log("scene transition happens ehere");
        // fadeIn.SetTrigger("triggerTransition");
        LoadScene("SJ_TestScene");
    }

    private void LoadScene(string _sceneName)
    {
        TransitionManager.Instance().Transition(_sceneName, transition, loadDelay);
    }
}
