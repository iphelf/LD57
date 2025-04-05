using System.Collections.Generic;
using _DeepChat.Scripts.Common;
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
        [SerializeField] private GameObject npcMessagePrefab;

        [SerializeField, ReadOnly] private List<MessageViewCtrl> messages;

        [SerializeField] private bool clearOnAwake = true;

        private void Awake()
        {
            if (clearOnAwake)
                Clear();
        }

        public void Append(ActorType actorType, string message)
        {
            var messagePrefab = actorType == ActorType.Player ? playerMessagePrefab : npcMessagePrefab;
            var go = Instantiate(messagePrefab, listRoot);
            var messageViewCtrl = go.GetComponent<MessageViewCtrl>();
            messageViewCtrl.SetContent(message);
            messages.Add(messageViewCtrl);
            scrollRect.normalizedPosition = new Vector2(0, 0);
            Canvas.ForceUpdateCanvases();
        }

        [Button]
        public void Clear()
        {
            foreach (var message in messages)
                Destroy(message.gameObject);
            messages.Clear();

            for (var i = listRoot.childCount - 1; i >= 0; --i)
                Destroy(listRoot.GetChild(i).gameObject);
        }

        [SerializeField] private string dummyMessage = string.Empty;

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        [UsedImplicitly]
        private void DebugAppendPlayerMessage()
        {
            Append(ActorType.Player, string.IsNullOrEmpty(dummyMessage) ? "Message from player" : dummyMessage);
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        [UsedImplicitly]
        private void DebugAppendNpcMessage()
        {
            Append(ActorType.Npc, string.IsNullOrEmpty(dummyMessage) ? "Message from NPC" : dummyMessage);
        }
    }
}