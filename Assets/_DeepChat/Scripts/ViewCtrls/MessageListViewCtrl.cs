using System.Collections.Generic;
using System.Threading;
using _DeepChat.Scripts.Common;
using _DeepChat.Scripts.Data;
using _DeepChat.Scripts.Logic;
using _DeepChat.Scripts.Systems;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class MessageListViewCtrl : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform listRoot;

        [Header("消息框")] [SerializeField] private GameObject playerMessagePrefab;
        [SerializeField] private GameObject playerBusyMessagePrefab;
        [SerializeField] private GameObject npcMessagePrefab;
        [SerializeField] private GameObject ratingMessagePrefab;
        [SerializeField] private GameObject waitingMessagePrefab;

        [Header("匹配")] [SerializeField] private GameObject perfectMatchIndicatorPrefab;
        [SerializeField] private GameObject goodMismatchIndicatorPrefab;
        [SerializeField] private GameObject badMismatchIndicatorPrefab;

        [Header("宽度匹配计算")] [SerializeField] private LayoutGroup targetReference;
        [SerializeField] private float targetWidthCorrection;

        [Header("时间控制")] [SerializeField] private float playerMessageDelayDuration = 0.25f;
        [SerializeField] private float npcMessagePrepareDuration = 2.0f;
        [SerializeField] private float matchDelayDuration = 0.5f;
        [SerializeField] private float matchJoinDuration = 1.0f;
        [SerializeField] private float matchIndicateDuration = 1.0f;
        [SerializeField] private float matchSeparateDuration = 1.0f;

        [Header("Config")] [SerializeField] private bool clearOnAwake = true;

        [SerializeField, ReadOnly] private List<TextMessageViewCtrl> messages;
        private GameObject _waitingMessage;

        private void Awake()
        {
            if (clearOnAwake)
                Clear();
        }

        private void OnDestroy()
        {
            DOTween.Kill(gameObject.GetInstanceID());
        }

        public async Awaitable AsyncAppendPlayerMessage(CancellationToken token, string message)
        {
            await Awaitable.WaitForSecondsAsync(playerMessageDelayDuration, token);
            var go = Instantiate(playerMessagePrefab, listRoot);
            var messageViewCtrl = go.GetComponent<TextMessageViewCtrl>();
            messageViewCtrl.SetContent(message);
            messages.Add(messageViewCtrl);
            AudioManager.PlaySfx(SfxKey.SendMessage);
            await AsyncScrollToBottom(token);
        }

        public async Awaitable AsyncAppendPlayerBusyMessage(CancellationToken token)
        {
            Instantiate(playerBusyMessagePrefab, listRoot);
            AudioManager.PlaySfx(SfxKey.SendMessage);
            await AsyncScrollToBottom(token);
        }

        public async Awaitable AsyncAppendNpcMessage(CancellationToken token, string message)
        {
            if (_waitingMessage)
            {
                _waitingMessage.transform.SetAsLastSibling();
                _waitingMessage.SetActive(true);
            }
            else
                _waitingMessage = Instantiate(waitingMessagePrefab, listRoot);

            await Awaitable.WaitForSecondsAsync(npcMessagePrepareDuration, token);
            _waitingMessage.SetActive(false);

            var go = Instantiate(npcMessagePrefab, listRoot);
            var messageViewCtrl = go.GetComponent<TextMessageViewCtrl>();
            messageViewCtrl.SetContent(message);
            messages.Add(messageViewCtrl);
            AudioManager.PlaySfx(SfxKey.ReceiveMessage);
            await AsyncScrollToBottom(token);
        }

        public async Awaitable AsyncMatchMessagePair(
            CancellationToken token, WidthMatchResultType widthMatchResult, bool enableTransparency)
        {
            var m1 = messages[^2];
            var m2 = messages[^1];
            var indicatorPrefab = GetMismatchIndicatorPrefab(widthMatchResult);
            var indicator = InstantiateMismatchIndicator(indicatorPrefab, m1, m2);
            var sfxKey = GetMismatchIndicateSfxKey(widthMatchResult);

            var m1Clone = Instantiate(m1.gameObject, listRoot, true).GetComponent<TextMessageViewCtrl>();
            var m2Clone = Instantiate(m2.gameObject, listRoot, true).GetComponent<TextMessageViewCtrl>();
            m1Clone.GetCanvasGroup().alpha = 0.0f;
            m2Clone.GetCanvasGroup().alpha = 0.0f;
            await AsyncPlayMatchPerformance(token, m1, m2, indicator, enableTransparency, sfxKey);
            Destroy(m1Clone.gameObject);
            Destroy(m2Clone.gameObject);
        }

        private GameObject GetMismatchIndicatorPrefab(WidthMatchResultType widthMatchResult)
        {
            switch (widthMatchResult)
            {
                case WidthMatchResultType.Perfect:
                    return perfectMatchIndicatorPrefab;
                case WidthMatchResultType.Good:
                    return goodMismatchIndicatorPrefab;
                case WidthMatchResultType.Bad:
                case WidthMatchResultType.Terrible:
                default:
                    return badMismatchIndicatorPrefab;
            }
        }

        private SfxKey GetMismatchIndicateSfxKey(WidthMatchResultType widthMatchResult)
        {
            switch (widthMatchResult)
            {
                case WidthMatchResultType.Perfect:
                    return SfxKey.MatchPerfect;
                case WidthMatchResultType.Good:
                    return SfxKey.MatchGood;
                case WidthMatchResultType.Bad:
                    return SfxKey.MatchBad;
                case WidthMatchResultType.Terrible:
                default:
                    return SfxKey.MatchTerrible;
            }
        }

        private MismatchIndicatorViewCtrl InstantiateMismatchIndicator(GameObject prefab, TextMessageViewCtrl m1,
            TextMessageViewCtrl m2)
        {
            var m1TopRight = ConvertFromMessageLocalPositionToListLocalPosition(
                m1.GetContent(), m1.GetContent().rect.max, listRoot);
            var m2BottomLeft = ConvertFromMessageLocalPositionToListLocalPosition(
                m2.GetContent(), m2.GetContent().rect.min, listRoot);
            var center = (m1TopRight + m2BottomLeft) * 0.5f;
            var size = m1TopRight - m2BottomLeft;
            size = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
            var go = Instantiate(prefab, listRoot);
            var viewCtrl = go.GetComponent<MismatchIndicatorViewCtrl>();
            viewCtrl.ResizeRectTransform(center, size);
            return viewCtrl;
        }

        private async Awaitable AsyncPlayMatchPerformance(
            CancellationToken token,
            TextMessageViewCtrl m1, TextMessageViewCtrl m2, MismatchIndicatorViewCtrl indicator,
            bool makeM1SemiTransparent, SfxKey sfxKey)
        {
            m2.SetLayoutEnabled(false);
            m1.SetLayoutEnabled(false);

            var m1Transform = m1.GetComponent<RectTransform>();
            var m2Transform = m2.GetComponent<RectTransform>();
            var m1OriginalY = m1Transform.localPosition.y;
            var m2OriginalY = m2Transform.localPosition.y;

            if (makeM1SemiTransparent)
                m2.transform.SetSiblingIndex(m1.transform.GetSiblingIndex());
            indicator.GetCanvasGroup().alpha = 0.0f;

            var goID = gameObject.GetInstanceID();

            await Awaitable.WaitForSecondsAsync(matchDelayDuration, token);

            if (makeM1SemiTransparent)
                m1.GetCanvasGroup().DOFade(0.8f, matchJoinDuration).SetId(goID).Play();
            var targetY = (m1OriginalY + m2OriginalY) * 0.5f;
            m1Transform.DOLocalMoveY(targetY, matchJoinDuration).SetId(goID).Play();
            m2Transform.DOLocalMoveY(targetY, matchJoinDuration).SetId(goID).Play();
            await Awaitable.WaitForSecondsAsync(matchJoinDuration, token);

            indicator.GetCanvasGroup().DOFade(1.0f, matchIndicateDuration * 0.5f).SetId(goID).Play();
            AudioManager.PlaySfx(sfxKey);
            await Awaitable.WaitForSecondsAsync(matchIndicateDuration, token);

            if (makeM1SemiTransparent)
                m1.GetCanvasGroup().DOFade(1.0f, matchSeparateDuration).SetId(goID).Play();
            m1Transform.DOLocalMoveY(m1OriginalY, matchSeparateDuration).SetId(goID).Play();
            m2Transform.DOLocalMoveY(m2OriginalY, matchSeparateDuration).SetId(goID).Play();
            await Awaitable.WaitForSecondsAsync(matchSeparateDuration, token);

            if (makeM1SemiTransparent)
                m1.transform.SetSiblingIndex(m2.transform.GetSiblingIndex());

            m1.SetLayoutEnabled(true);
            m2.SetLayoutEnabled(true);
        }

        public async Awaitable AsyncAppendRating(CancellationToken token, Rating rating)
        {
            var go = Instantiate(ratingMessagePrefab, listRoot);
            var ratingMessageViewCtrl = go.GetComponent<RatingMessageViewCtrl>();
            ratingMessageViewCtrl.SetContent(rating);
            await AsyncScrollToBottom(token);
        }

        public async Awaitable AsyncScrollToBottom(CancellationToken token)
        {
            Canvas.ForceUpdateCanvases();
            await Awaitable.EndOfFrameAsync(token);
            scrollRect.normalizedPosition = new Vector2(0, 0);
        }

        public float GetDifferenceAgainstTarget()
        {
            var m1 = messages[^2];
            var m2 = messages[^1];
            var width = m1.GetContent().rect.width + m2.GetContent().rect.width;
            var targetWidth = (targetReference.transform as RectTransform)!.rect.width
                              - targetReference.padding.left
                              - targetReference.padding.right;
            targetWidth += targetWidthCorrection;
            var diff = width - targetWidth;
            return diff;
        }

        public Vector2 ConvertFromMessageLocalPositionToListLocalPosition(
            RectTransform messageTransform, Vector2 messageLocalPosition, RectTransform listTransform)
        {
            var worldPosition = messageTransform.TransformPoint(messageLocalPosition);
            var localPosition = listTransform.InverseTransformPoint(worldPosition);
            return localPosition;
        }

        [Button]
        public void Clear()
        {
            foreach (var message in messages)
                Destroy(message.gameObject);
            messages.Clear();

            for (var i = listRoot.childCount - 1; i >= 0; --i)
                Destroy(listRoot.GetChild(i).gameObject);
            _waitingMessage = null;
        }
    }
}