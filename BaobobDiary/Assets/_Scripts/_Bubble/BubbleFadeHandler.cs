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
    public class BubbleFadeHandler : MonoBehaviour
    {
        public Material bubbleMat;
        public float fadeDuration = 2f; // Total duration for the fade effect
        private bool isFading = false;

        [ContextMenu("Play Fade In")]
        public void PlayFadeIn()
        {
            if (bubbleMat == null)
            {
                Debug.LogError("Bubble material is not assigned");
                return;
            }

            // Initialize the material properties
            bubbleMat.SetFloat("_Fade", 0);
            bubbleMat.SetFloat("_AmbientOcclusion", 0);
            Color color = bubbleMat.GetColor("_Color");
            color.a = 0;
            bubbleMat.SetColor("_Color", color);

            StartCoroutine(FadeIn());
        }

        private IEnumerator FadeIn()
        {
            isFading = true;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / fadeDuration);
                float easeInOutCubic = t < 0.5f ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;

                bubbleMat.SetFloat("_Fade", easeInOutCubic);
                bubbleMat.SetFloat("_AmbientOcclusion", Mathf.Lerp(0, 2.75f, easeInOutCubic));

                Color color = bubbleMat.GetColor("_Color");
                color.a = easeInOutCubic;
                bubbleMat.SetColor("_Color", color);

                yield return null;
            }

            isFading = false;
        }

        [ContextMenu("Play Fade Out")]
        public void PlayFadeOut()
        {
            if (bubbleMat == null)
            {
                Debug.LogError("Bubble material is not assigned");
                return;
            }
            StartCoroutine(FadeOut());
        }

        private IEnumerator FadeOut()
        {
            isFading = true;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / fadeDuration);
                float easeInOutCubic = t < 0.5f ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;

                bubbleMat.SetFloat("_Fade", 1.0f - easeInOutCubic);
                bubbleMat.SetFloat("_AmbientOcclusion", Mathf.Lerp(2.75f, 0, easeInOutCubic));

                Color color = bubbleMat.GetColor("_Color");
                color.a = 1.0f - easeInOutCubic;
                bubbleMat.SetColor("_Color", color);

                yield return null;
            }

            isFading = false;
        }
    }
}
