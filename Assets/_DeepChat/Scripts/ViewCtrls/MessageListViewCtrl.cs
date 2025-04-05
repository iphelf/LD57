using System.Collections.Generic;
using _DeepChat.Scripts.Common;
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

        public void Append(ActorType actorType, string message)
        {
            var messagePrefab = actorType == ActorType.Player ? playerMessagePrefab : npcMessagePrefab;
            var go = Instantiate(messagePrefab, listRoot);
            var messageViewCtrl = go.GetComponent<MessageViewCtrl>();
            messageViewCtrl.SetContent(message);
            messages.Add(messageViewCtrl);
        }

        [Button]
        private void DebugAppendPlayerMessage()
        {
            Append(ActorType.Player, "Message from player");
        }

        [Button]
        private void DebugAppendNpcMessage()
        {
            Append(ActorType.Npc, "Message from NPC");
        }

        [Button]
        private void DebugClear()
        {
            foreach (var message in messages)
                Destroy(message.gameObject);
            messages.Clear();

            for (var i = listRoot.childCount - 1; i >= 0; --i)
                Destroy(listRoot.GetChild(i).gameObject);
        }
    }
}