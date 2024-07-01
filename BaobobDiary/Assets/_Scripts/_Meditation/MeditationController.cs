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
using TMPro;
using UnityEngine.UI;

namespace meditation
{
    public class MeditationController : MonoBehaviour
    {
        [Tooltip("Audio")]
        [SerializeField] AudioSource meditationBGAudioSource;
        [SerializeField] AudioSource meditationNarrationAudioSource;
        [SerializeField] AudioClip[] narrations;
        [SerializeField] AudioClip oneTwoThree;

        [Tooltip("Assets")]
        [SerializeField] GameObject tutorialHand;
        [SerializeField] GameObject bubble;
        [SerializeField] Animator bubbleAnimator;
        [SerializeField] BubbleFadeHandler bubbleFadeHandler;
        [SerializeField] ParticleSystem bubbleParticle;

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

        GameObject _instructionText;
        GameObject _onetwothreeText;
        TMP_Text instructionTMPText;
        TMP_Text onetwothreeTMPText;

        GameObject pursingEffect;

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
                StartCoroutine(AdjustLighting(-0.5f, 4f));
                //어서와. (pause) 지금부터 잠시 머리를 비워볼거야.(pause)
                //몸의 긴장을 풀고, 지금 이 순간에 정신을 집중해봐.
                PlayNarration(0, onNarration0Finished, false, null, false);
            }

        }

        private void SetText(float delay, bool isInstructionText, string instructionText)
        {
            StartCoroutine(WaitDelayForText(delay, isInstructionText, instructionText));
        }

        private IEnumerator WaitDelayForText(float delay, bool isInstructionText, string instructionText)
        {
            yield return new WaitForSeconds(delay);

            if (isInstructionText)
            {
                StartCoroutine(FadeInText(instructionTMPText));
                instructionTMPText.text = instructionText;
                onetwothreeTMPText.text = "";
            }
            else
            {
                StartCoroutine(FadeInText(onetwothreeTMPText));
                instructionTMPText.text = "";
                StartCoroutine(ChangeOneTwoThreeText());
            }
        }

        private IEnumerator FadeInText(TMP_Text text)
        {
            Color color = text.color;
            color.a = 0;
            text.color = color;

            while (text.color.a < 1.0f)
            {
                color.a += Time.deltaTime / .5f;
                text.color = color;
                yield return null;
            }
        }

        private IEnumerator ChangeOneTwoThreeText()
        {
            onetwothreeTMPText.text = "하나";
            yield return new WaitForSeconds(2f);
            onetwothreeTMPText.text = "둘";
            yield return new WaitForSeconds(2f);
            onetwothreeTMPText.text = "셋";
            yield return new WaitForSeconds(2f);
        }

        private void DisableText()
        {
            instructionTMPText.text = "";
            onetwothreeTMPText.text = "";
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
            else if (!checkGesture)
            {
                onComplete.Invoke();
            }
            else if (checkGesture && passOneTwoThree)
            {
                StartCoroutine(WaitMore(3f, onComplete));
            }
        }

        private IEnumerator WaitMore(float additionalTime, UnityEvent onComplete)
        {
            float elapsedTime = 0.0f;

            while (elapsedTime < additionalTime)
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
            //다시 하라는 UI

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
            SetText(0.5f, false, null);
            StartCoroutine(WaitForOneTwoThreeToEnd(oneTwoThree.length, onComplete));
        }

        IEnumerator WaitForOneTwoThreeToEnd(float duration, UnityEvent onComplete)
        {
            yield return new WaitForSeconds(duration);
            DisableText();
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

            // Use more robust checking and logging to identify issues
            Transform panelTransform = tutorialHandInstance.transform.Find("UI/Canvas/Panel");

            if (panelTransform != null)
            {
                Transform instructionTextTransform = panelTransform.Find("instructionText");
                Transform onetwothreeTextTransform = panelTransform.Find("oneTwoThreeText");

                if (instructionTextTransform != null)
                {
                    _instructionText = instructionTextTransform.gameObject;
                    instructionTMPText = _instructionText.GetComponent<TMP_Text>();
                    if (instructionTMPText == null)
                    {
                        Debug.LogError("instructionTMPText component is missing.");
                    }
                }
                else
                {
                    Debug.LogError("instructionText object is missing.");
                }

                if (onetwothreeTextTransform != null)
                {
                    _onetwothreeText = onetwothreeTextTransform.gameObject;
                    onetwothreeTMPText = _onetwothreeText.GetComponent<TMP_Text>();
                    if (onetwothreeTMPText == null)
                    {
                        Debug.LogError("onetwothreeTMPText component is missing.");
                    }
                }
                else
                {
                    Debug.LogError("oneTwoThreeText object is missing.");
                }
            }
            else
            {
                Debug.LogError("Panel object is missing under UI/Canvas.");
            }
        }

        private void InstantiateBubble()
        {
            Vector3 bubblePose = playerTransform.position + playerTransform.forward * bubbleDistance + bubbleOffset;
            bubble.transform.position = bubblePose;
            bubble.transform.rotation = Quaternion.LookRotation(playerTransform.forward);
            bubble.SetActive(true);

            bubbleFadeHandler.PlayFadeIn();
            bubbleParticle.Play();

            bubbleAnimator.SetTrigger("startFloatingAnim");
        }

        private void OnNarration0Finished()
        {
            //narration1 starts
            //눈 앞에 두 손 보이지? (손 wave)
            //두 손을 따라하며 천천히 같이 호흡을 해보자.
            PlayNarration(1, onNarration1Finished, false, null, false);
            //tutorial hand shows up in front of the player 
            InstantiateTutorialHand();
            tutorialHandAnimator.SetTrigger("wave");
            SetText(4f, true, "호흡에 맞춰\r\n동작을 따라하세요");
        }

        private void OnNarration1Finished()
        {
            //narration2 starts
            //자, 손을 동그랗게 벌리며 숨을 크게 들이 쉬어봐. 숨을 참아야 해
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
            //그리고 그대로 숨을 끝까지 내뱉으면서 손을 앞으로 뻗어보자. 
            PlayNarration(3, onNarration3Finished, true, gestureActivateStateGroup, false);
            StartCoroutine(AnimationDelay(3.0f, "pushCircle"));
            SetText(3f, true, "호흡에 맞춰\r\n동작을 따라하세요");
        }

        private void OnNarration3Finished()
        {
            //narration4 starts
            //잘했어 이 동작을 몇 번 더 하면서 두 손 사이 공간에 너의 에너지를 불어넣을거야.
            //다시 한 번. 손에 집중해서 숨을 들이쉬자. 
            PlayNarration(4, onNarration4Finished, true, poseActivateStateGroup, false);
            StartCoroutine(AnimationDelay(0.0f, "thumbsUp"));
            SetText(10f, true, "호흡에 맞춰\r\n동작을 따라하세요");
        }

        private void OnNarration4Finished()
        {
            //narration5 starts
            //잠시 머금고 내뱉어. 
            PlayNarration(5, onNarration5Finished, true, gestureActivateStateGroup, false);
            StartCoroutine(AnimationDelay(0.0f, "pushCircle"));
            SetText(2f, true, "호흡에 맞춰\r\n동작을 따라하세요");
        }

        private void OnNarration5Finished()
        {
            //narration6 starts
            //좋아 마지막이야. 호흡이 오가는 그 길을 느껴봐. 
            PlayNarration(6, onNarration6Finished, true, poseActivateStateGroup, false);
            StartCoroutine(AnimationDelay(0.0f, "thumbsUp"));
            SetText(5f, true, "호흡에 맞춰\r\n동작을 따라하세요");
        }

        private void OnNarration6Finished()
        {
            //narration7 starts
            //깊게 내뱉으며 손을 모아줘. 
            PlayNarration(7, onNarration7Finished, true, gestureActivateStateGroup, false);
            StartCoroutine(AnimationDelay(0.0f, "pushCircle"));
            SetText(3f, true, "호흡에 맞춰\r\n동작을 따라하세요");
        }

        private void OnNarration7Finished()
        {
            DisableText();
            StartCoroutine(ShowBubble(1.0f, 2.0f));
            StartCoroutine(AdjustLighting(0.0f, 4f));
        }

        IEnumerator ShowBubble(float delay1, float delay2)
        {
            //Destroy(tutorialHandInstance);
            pursingEffect = tutorialHandInstance.transform.Find("PursingEffect").gameObject;
            pursingEffect.SetActive(false);
            tutorialHandInstance.SetActive(false);
            yield return new WaitForSeconds(delay1);
            InstantiateBubble();
            yield return new WaitForSeconds(delay2);
            bubbleParticle.Stop();
            PlayNarration(8, onNarration8Finished, false, null, false);

        }

        private void OnNarration8Finished()
        {
            //세번째 손가락과 엄지를 모아봐. 
            tutorialHandInstance.SetActive(true);
            PlayNarration(9, onNarration9Finished, true, scalePoseActiveStateGroup, true);
            StartCoroutine(AnimationDelay(0.0f, "scalePose"));
            SetText(0f, true, "동작을 따라하세요");
        }

        private void OnNarration9Finished()
        {
            //그리고 당겨봐. 당길수있는 최대한으로. 
            meditationNarrationAudioSource.clip = narrations[10];
            meditationNarrationAudioSource.Play();
            StartCoroutine(AnimationDelay(0.0f, "pullScale"));
            scaleInteractionVisual.maxScaledForThreeSecs.AddListener(OnMaxScaledForThreeSecs);
        }

        private void OnNarration10Finished()
        {
            //그리고 3초를 버텨. 하나 둘 셋 
            //PlayNarration(11, null, false, null, true);
        }

        private void OnMaxScaledForThreeSecs()
        {
            //scene transtiion
            DisableText();
            Debug.Log("scene transition happens ehere");
            // fadeIn.SetTrigger("triggerTransition");
            LoadScene("SJ_TestScene");

        }

        private void LoadScene(string _sceneName)
        {
            TransitionManager.Instance().Transition(_sceneName, transition, loadDelay);
        }
    }
}

