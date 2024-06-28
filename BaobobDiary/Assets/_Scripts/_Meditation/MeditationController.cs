using Oculus.Interaction;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Meta.XR.MRUtilityKit;
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

    [HideInInspector] public UnityEvent onOneTwoThreeFinished;

    private bool isCheckingGesture = false;
    private bool isGestureCorrect = false;
    private bool isButtonPressed = false;
    Animator tutorialHandAnimator;
    GameObject tutorialHandInstance;
    GameObject bubbleInstance;

    [SerializeField] ActiveStateGroup poseActivateStateGroup;
    [SerializeField] ActiveStateGroup gestureActivateStateGroup;

    [SerializeField] private OVRPassthroughLayer _passthroughLayer;

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
            StartCoroutine(AdjustLighting(-0.5f, 4f));
            //어서와. (pause) 지금부터 잠시 머리를 비워볼거야.(pause)
            //몸의 긴장을 풀고, 지금 이 순간에 정신을 집중해봐.
            PlayNarration(0, onNarration0Finished, false, null);
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
                Debug.Log($"Adjusting lighting with brightness: {newBrightness}");
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

    private void PlayNarration(int narrationIndex, UnityEvent onComplete, bool checkGesture, ActiveStateGroup activateStateGroup)
    {
        meditationNarrationAudioSource.clip = narrations[narrationIndex];
        meditationNarrationAudioSource.Play();
        leftHand.sharedMaterial = defaultMat;
        rightHand.sharedMaterial = defaultMat;
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
        Vector3 tutorialHandPose = playerTransform.position + playerTransform.forward * tutorialHandDistance + tutorialHandOffset;
        tutorialHandInstance = Instantiate(tutorialHand, tutorialHandPose, Quaternion.LookRotation(playerTransform.forward));
        tutorialHandAnimator = tutorialHandInstance.GetComponent<Animator>();
    }

    private void InstantiateBubble()
    {
        Vector3 bubblePose = playerTransform.position + playerTransform.forward * bubbleDistance + bubbleOffset;
        bubbleInstance = Instantiate(sphere, bubblePose, Quaternion.LookRotation(playerTransform.forward));
    }

    private void OnNarration0Finished()
    {
        //narration1 starts
        //눈 앞에 두 손 보이지? (손 wave)
        //두 손을 따라하며 천천히 같이 호흡을 해보자.
        PlayNarration(1, onNarration1Finished, false, null);
        //tutorial hand shows up in front of the player 
        InstantiateTutorialHand();
        tutorialHandAnimator.SetTrigger("wave");

        //ui shows below the tutorial hand ("동작을 따라하세요")
    }

    private void OnNarration1Finished()
    {
        //narration2 starts
        //자, 손을 동그랗게 벌리며 숨을 크게 들이 쉬어봐. 숨을 참아야 해
        PlayNarration(2, onNarration2Finished, true, poseActivateStateGroup);
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
        //그리고 그대로 숨을 끝까지 내뱉으면서 손을 앞으로 뻗어보자. 
        PlayNarration(3, onNarration3Finished, true, gestureActivateStateGroup);
        StartCoroutine(AnimationDelay(3.0f, "pushCircle"));
    }

    private void OnNarration3Finished()
    {
        //narration4 starts
        //잘했어 다시 한 번. 손에 집중해서 숨을 들이쉬자. 
        PlayNarration(4, onNarration4Finished, true, poseActivateStateGroup);
        StartCoroutine(AnimationDelay(0.0f, "thumbsUp"));
    }

    private void OnNarration4Finished()
    {
        //narration5 starts
        //잠시 머금고 내뱉어. 
        PlayNarration(5, onNarration5Finished, true, gestureActivateStateGroup);
        StartCoroutine(AnimationDelay(0.0f, "pushCircle"));
    }

    private void OnNarration5Finished()
    {
        //narration6 starts
        //좋아 마지막이야. 호흡이 오가는 그 길을 느껴봐. 
        PlayNarration(6, onNarration6Finished, true, poseActivateStateGroup);
        StartCoroutine(AnimationDelay(0.0f, "thumbsUp"));
    }

    private void OnNarration6Finished()
    {
        //narration7 starts
        //깊게 내뱉으며 손을 모아줘. 
        PlayNarration(7, onNarration7Finished, true, gestureActivateStateGroup);
        StartCoroutine(AnimationDelay(0.0f, "pushCircle"));
    }

    private void OnNarration7Finished()
    {
        StartCoroutine(ShowBubble(2.0f, 2.0f));
        StartCoroutine(AdjustLighting(0.0f, 4f));
    }

    IEnumerator ShowBubble(float delay1, float delay2)
    {
        Destroy(tutorialHandInstance);
        yield return new WaitForSeconds(delay1);
        InstantiateBubble();
        yield return new WaitForSeconds(delay2);
        PlayNarration(8, onNarration8Finished, false, null);
    }

    private void OnNarration8Finished()
    {

    }
}
