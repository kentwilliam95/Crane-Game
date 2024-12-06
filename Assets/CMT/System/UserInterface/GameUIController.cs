using Core;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using System.Collections;
using TMPro;
using UnityEngine.UI;

namespace CMT
{
    public class GameUIController : MonoBehaviour
    {
        private Sequence _sequenceHideMenu;
        private Sequence _sequenceShowResult;
        private Sequence _sequenceHideGameResult;
        private Coroutine _coroutineDelay;
        private float _animationDuration = 0.25f;

        [SerializeField] private Canvas _canvas;

        [Header("Menu")]
        [SerializeField] private CanvasGroup _groupMenu;
        [SerializeField] private RectTransform _rectTransformText;
        [SerializeField] private RectTransform _rectTransformButton;

        [Header("Result")]
        [SerializeField] private CanvasGroup _groupResult;
        [SerializeField] private TextMeshProUGUI _textCongratulation;
        [SerializeField] private TextMeshProUGUI _textYouGot;
        [SerializeField] private TextMeshProUGUI _textResult;
        [SerializeField] private RectTransform _rectTransformImageResult;
        [SerializeField] private Image _imageResult;
        [SerializeField] private Button _buttonResult;
        [SerializeField] private ParticleSystem[] _particleConfetti;

        public UnityAction OnHideMenuComplete;
        public UnityAction OnHideGameResultComplete;

        private void OnDestroy()
        {
            _sequenceHideMenu?.Kill();
        }

        private void Start()
        {
            _canvas.worldCamera = CameraController.Instance.uiCamera;
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;

            _groupResult.interactable = false;
            _groupResult.blocksRaycasts = false;
            _groupResult.alpha = 0f;
            InitializeHideMenuAnimation();
        }

        private void InitializeHideMenuAnimation()
        {
            _sequenceHideMenu = DOTween.Sequence();
            _sequenceHideMenu.SetAutoKill(false);

            _sequenceHideMenu.Insert(0f, _rectTransformButton.DOAnchorPos(Vector3.zero - Vector3.up * _rectTransformButton.rect.height * 0.5f, 0.5f).SetEase(Ease.InBack));
            _sequenceHideMenu.Insert(0f, _rectTransformText.DOAnchorPos(Vector3.zero + Vector3.up * _rectTransformText.rect.height * 0.5f, 0.5f).SetEase(Ease.InBack));
            _sequenceHideMenu.InsertCallback(0.35f, () =>
            {
                _groupMenu.interactable = false;
                _groupMenu.blocksRaycasts = false;
            });
            _sequenceHideMenu.Pause();
        }

        public void HideMenu()
        {
            _sequenceHideMenu.Restart();
            if (_coroutineDelay != null)
                StopCoroutine(_coroutineDelay);

            _coroutineDelay = StartCoroutine(CoroutineDelay(_sequenceHideMenu.Duration(), OnHideMenuComplete));
        }

        public void ShowResult(Pickable item)
        {
            _textResult.text = item.ObjectName;
            _imageResult.sprite = item.Sprite;
            InitializeGameResultAnimation();
            _sequenceShowResult.Restart();
        }

        private void InitializeGameResultAnimation()
        {
            if (_sequenceShowResult != null)
                return;

            _sequenceShowResult = DOTween.Sequence();
            _sequenceShowResult.SetAutoKill(false);

            float totalAnimationDuration = 0f;

            _sequenceShowResult.Insert(totalAnimationDuration, _groupResult.DOFade(1f, 0.25f).From(0f));
            _sequenceShowResult.Insert(totalAnimationDuration, _textCongratulation.rectTransform.DOScale(1f, _animationDuration)
            .From(0).SetEase(Ease.OutBack));

            totalAnimationDuration += _animationDuration;

            _sequenceShowResult.Insert(totalAnimationDuration, _textYouGot.rectTransform.DOScale(1f, _animationDuration).From(0).SetEase(Ease.OutBack));
            totalAnimationDuration += _animationDuration;

            _sequenceShowResult.InsertCallback(totalAnimationDuration, () =>
            {
                for (int i = 0; i < _particleConfetti.Length; i++)
                {
                    _particleConfetti[i].Play();
                }
            });

            totalAnimationDuration += _animationDuration;
            _sequenceShowResult.Insert(totalAnimationDuration, _textResult.rectTransform.DOScale(1f, _animationDuration).From(0).SetEase(Ease.OutBack));
            totalAnimationDuration += _animationDuration;

            _sequenceShowResult.Insert(totalAnimationDuration, _rectTransformImageResult.DOScale(1f, 0.25f).From(0));
            totalAnimationDuration += _animationDuration;

            totalAnimationDuration += 2 * _animationDuration;
            _sequenceShowResult.Insert(totalAnimationDuration, _buttonResult.transform.DOScale(1f, _animationDuration).From(0));
            totalAnimationDuration += _animationDuration;

            _sequenceShowResult.InsertCallback(totalAnimationDuration, () =>
            {
                _groupResult.interactable = true;
                _groupResult.blocksRaycasts = true;
            });

            _sequenceShowResult.Pause();
        }

        public void BackToGame()
        {
            InitializeBackToGameAnimation();
            _sequenceHideGameResult.Restart();
        }

        private void InitializeBackToGameAnimation()
        {
            if (_sequenceHideGameResult != null)
                return;

            _sequenceHideGameResult = DOTween.Sequence();
            _sequenceHideGameResult.SetAutoKill(false);

            _sequenceHideGameResult.Insert(0f, _groupResult.DOFade(0f, _animationDuration).From(1f));
            _sequenceHideGameResult.InsertCallback(_animationDuration, () =>
            {
                _groupResult.interactable = false;
                _groupResult.blocksRaycasts = false;
                OnHideGameResultComplete?.Invoke();
            });
            _sequenceHideGameResult.Pause();
        }

        private IEnumerator CoroutineDelay(float duration, UnityAction onComplete)
        {
            duration = Mathf.Max(duration, 0f);
            yield return new WaitForSeconds(duration);
            onComplete?.Invoke();
        }
    }
}
