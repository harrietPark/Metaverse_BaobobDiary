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
        public float fadeSpeed = 1.0f;
        bool isFadingIn = false;
        bool isFadingOut = false;

        public void PlayFadeIn()
        {
            if (bubbleMat != null)
            {
                Debug.LogError("Bubble material is not assigned");
                return;
            }

            //Initialise the material properties
            bubbleMat.SetFloat("_Fade", 0);
            bubbleMat.SetFloat("_AmbientOcclusion", 0);
            Color color = bubbleMat.GetColor("_Color");
            color.a = 0;
            bubbleMat.SetColor("_Color", color);

            StartCoroutine(FadeIn());
        }

        private IEnumerator FadeIn()
        {
            isFadingIn = true;

            while (isFadingIn)
            {
                float currFade = bubbleMat.GetFloat("_Fade");
                currFade += Time.deltaTime * fadeSpeed;
                if (currFade >= 1.0f)
                {
                    currFade = 1.0f;
                    isFadingIn = false;
                }

                bubbleMat.SetFloat("_Fade", currFade);
                bubbleMat.SetFloat("_AmbientOcclusion", Mathf.Lerp(0, 2.75f, currFade));

                Color color = bubbleMat.GetColor("_Color");
                color.a = currFade;
                bubbleMat.SetColor("_Color", color);

                yield return null;
            }
        }
    }

}
