using System.Collections.Generic;
using System.Threading;
using _DeepChat.Scripts.Logic;
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
        [SerializeField] private GameObject playerMessagePrefab;
        [SerializeField] private GameObject playerBusyMessagePrefab;
        [SerializeField] private GameObject npcMessagePrefab;
        [SerializeField] private GameObject mismatchIndicatorPrefab;

        [Header("宽度匹配计算")] [SerializeField] private LayoutGroup targetReference;
        [SerializeField] private float targetWidthCorrection;

        [Header("Config")] [SerializeField] private bool clearOnAwake = true;

        [SerializeField, ReadOnly] private List<MessageViewCtrl> messages;

        private void Awake()
        {
            if (clearOnAwake)
                Clear();
        }

        public async Awaitable AsyncAppendPlayerMessage(CancellationToken token, string message)
        {
            var go = Instantiate(playerMessagePrefab, listRoot);
            var messageViewCtrl = go.GetComponent<MessageViewCtrl>();
            messageViewCtrl.SetContent(message);
            messages.Add(messageViewCtrl);
            await AsyncScrollToBottom(token);
        }

        public async Awaitable AsyncAppendPlayerBusyMessage(CancellationToken token)
        {
            Instantiate(playerBusyMessagePrefab, listRoot);
            await Awaitable.EndOfFrameAsync(token);
            await AsyncScrollToBottom(token);
        }

        public async Awaitable AsyncAppendNpcMessage(CancellationToken token, string message)
        {
            var go = Instantiate(npcMessagePrefab, listRoot);
            var messageViewCtrl = go.GetComponent<MessageViewCtrl>();
            messageViewCtrl.SetContent(message);
            messages.Add(messageViewCtrl);
            await AsyncScrollToBottom(token);
        }

        public async Awaitable AsyncMatchMessagePair(CancellationToken token, bool enableTransparency)
        {
            var m1 = messages[^2];
            var m2 = messages[^1];
            var indicator = InstantiateMismatchIndicator(m1, m2);

            var m1Clone = Instantiate(m1.gameObject, listRoot, true).GetComponent<MessageViewCtrl>();
            var m2Clone = Instantiate(m2.gameObject, listRoot, true).GetComponent<MessageViewCtrl>();
            m1Clone.GetCanvasGroup().alpha = 0.0f;
            m2Clone.GetCanvasGroup().alpha = 0.0f;
            await AsyncPlayMatchAnimation(token, m1, m2, indicator, enableTransparency);
            Destroy(m1Clone.gameObject);
            Destroy(m2Clone.gameObject);
        }

        private MismatchIndicatorViewCtrl InstantiateMismatchIndicator(MessageViewCtrl m1, MessageViewCtrl m2)
        {
            var m1TopRight = ConvertFromMessageLocalPositionToListLocalPosition(
                m1.GetContent(), m1.GetContent().rect.max, listRoot);
            var m2BottomLeft = ConvertFromMessageLocalPositionToListLocalPosition(
                m2.GetContent(), m2.GetContent().rect.min, listRoot);
            var center = (m1TopRight + m2BottomLeft) * 0.5f;
            var size = m1TopRight - m2BottomLeft;
            size = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
            var go = Instantiate(mismatchIndicatorPrefab, listRoot);
            var viewCtrl = go.GetComponent<MismatchIndicatorViewCtrl>();
            viewCtrl.ResizeRectTransform(center, size);
            return viewCtrl;
        }

        private async Awaitable AsyncPlayMatchAnimation(
            CancellationToken token,
            MessageViewCtrl m1, MessageViewCtrl m2, MismatchIndicatorViewCtrl indicator,
            bool makeM1SemiTransparent)
        {
            m2.SetLayoutEnabled(false);
            m1.SetLayoutEnabled(false);

            var m1Transform = m1.GetComponent<RectTransform>();
            var m2Transform = m2.GetComponent<RectTransform>();
            var m1OriginalY = m1Transform.localPosition.y;
            var m2OriginalY = m2Transform.localPosition.y;

            var targetY = (m1OriginalY + m2OriginalY) * 0.5f;
            m1Transform.DOLocalMoveY(targetY, 1.0f).Play();
            m2Transform.DOLocalMoveY(targetY, 1.0f).Play();
            indicator.GetCanvasGroup().alpha = 0.0f;
            if (makeM1SemiTransparent)
                m1.GetCanvasGroup().DOFade(0.5f, 1.0f).Play();
            await Awaitable.WaitForSecondsAsync(1.0f, token);

            indicator.GetCanvasGroup().DOFade(1.0f, 0.5f).Play();
            await Awaitable.WaitForSecondsAsync(1.0f, token);

            m1Transform.DOLocalMoveY(m1OriginalY, 1.0f).Play();
            m2Transform.DOLocalMoveY(m2OriginalY, 1.0f).Play();
            if (makeM1SemiTransparent)
                m1.GetCanvasGroup().DOFade(1.0f, 1.0f).Play();
            await Awaitable.WaitForSecondsAsync(1.0f, token);

            m1.SetLayoutEnabled(true);
            m2.SetLayoutEnabled(true);
        }

        public async Awaitable AsyncAppendRating(CancellationToken token, Rating rating)
        {
            var content =
                $"width_match={rating.WidthMatchResult.ToString()}, score={rating.WidthMatchScore}, emotion={rating.NpcEmotion.ToString()}, emotion_match={rating.IsEmotionMatched}, bonus={rating.EmotionMatchScore}";
            await AsyncAppendNpcMessage(token, content);
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
        private void Clear()
        {
            foreach (var message in messages)
                Destroy(message.gameObject);
            messages.Clear();

            for (var i = listRoot.childCount - 1; i >= 0; --i)
                Destroy(listRoot.GetChild(i).gameObject);
        }
    }
}