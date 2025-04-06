using System.Collections.Generic;
using System.Threading;
using _DeepChat.Scripts.Common;
using _DeepChat.Scripts.Logic;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class MessageListViewCtrl : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Transform listRoot;
        [SerializeField] private GameObject playerMessagePrefab;
        [SerializeField] private GameObject playerBusyMessagePrefab;
        [SerializeField] private GameObject npcMessagePrefab;
        [SerializeField] private LayoutGroup targetReference;
        [SerializeField] private float targetWidthCorrection;
        [SerializeField] private string playerBusyStr = "The server is busy, please try again later...";

        [SerializeField, ReadOnly] private List<MessageViewCtrl> messages;

        [SerializeField] private bool clearOnAwake = true;

        private void Awake()
        {
            if (clearOnAwake)
                Clear();
        }

        public async Awaitable AsyncAppendPlayerMessage(CancellationToken token, string message)
        {
            var go = Instantiate(playerMessagePrefab, listRoot);
            var messageViewCtrl = go.GetComponent<MessageViewCtrl>();
            messageViewCtrl.SetContent(message ?? playerBusyStr);
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
            messageViewCtrl.SetContent(message ?? playerBusyStr);
            messages.Add(messageViewCtrl);
            await AsyncScrollToBottom(token);
        }

        public async Awaitable AsyncMatchMessagePair(CancellationToken token)
        {
            await Awaitable.WaitForSecondsAsync(1.0f, token);
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
            var width = m1.GetContentWidth() + m2.GetContentWidth();
            var targetWidth = (targetReference.transform as RectTransform)!.rect.width
                              - targetReference.padding.left
                              - targetReference.padding.right;
            targetWidth += targetWidthCorrection;
            var diff = width - targetWidth;
            return diff;
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