using System.Collections;
using UnityEngine;
using System;

namespace Core
{
    public class PanelFade : MonoBehaviour
    {
        private static PanelFade instance;
        public static PanelFade Instance => instance;

        private Coroutine fadeCoroutine;
        private float animationDuration = 0.25f;
        private bool isTweening;

        public Canvas canvas;
        public CanvasGroup canvasGroup;
        private void Awake()
        {
            instance = this;
        }

        public void Show(Action onComplete = null, int delayFrame = 0)
        {
            if (canvasGroup.alpha >= 1)
            {
                if (isTweening)
                    return;
                else
                {
                    onComplete?.Invoke();
                    return;
                }
            }

            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadeIenumerator(0f, 1f, animationDuration, delayFrame, (value) =>
            {
                canvasGroup.alpha = value;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }, onComplete));
        }

        public void Hide(Action onComplete = null, int delayFrame = 0)
        {
            if (canvasGroup.alpha <= 0)
            {
                if (isTweening)
                    return;
                else
                {
                    onComplete?.Invoke();
                    return;
                }
            }

            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadeIenumerator(1f, 0f, animationDuration, delayFrame, (value) =>
            {
                canvasGroup.alpha = value;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }, onComplete));
        }

        private IEnumerator FadeIenumerator(float from, float to, float time, int delayFrame, Action<float> onUpdate = null, Action onComplete = null)
        {
            for (int i = 0; i < delayFrame; i++)
            {
                yield return null;
            }

            float percentage = 0f;
            float speed = 1f / time;
            isTweening = true;

            for (float t = 0; t <= 1f;)
            {
                onUpdate?.Invoke(Mathf.Lerp(from, to, t));
                percentage += Time.unscaledDeltaTime * speed;
                t += Time.unscaledDeltaTime * speed;
                yield return null;
            }

            onUpdate?.Invoke(Mathf.Lerp(from, to, percentage));
            onComplete?.Invoke();
            isTweening = false;
        }
    }
}