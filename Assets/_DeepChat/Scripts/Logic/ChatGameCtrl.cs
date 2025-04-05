using System.Collections;
using System.Linq;
using _DeepChat.Scripts.Common;
using _DeepChat.Scripts.Data;
using _DeepChat.Scripts.ViewCtrls;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _DeepChat.Scripts.Logic
{
    public class ChatGameCtrl : MonoBehaviour
    {
        [SerializeField] private MessageListViewCtrl messages;
        [SerializeField] private EmoticonListViewCtrl emoticons;
        [SerializeField] private Button sendButton;

        [SerializeField] private EmoticonBank playerEmoticonBank;
        [SerializeField] private float npcMessageSpan = 5.0f;
        [SerializeField] private EmoticonBank npcEmoticonBank;
        [SerializeField] private bool runOnStart;

        private bool _isRunning;

        private void Awake()
        {
            sendButton.onClick.AddListener(PlayerSendMessage);
        }

        private void Start()
        {
            if (!runOnStart)
                return;
            StartCoroutine(Run());
        }

        private IEnumerator Run()
        {
            _isRunning = true;

            emoticons.SetEmoticons(playerEmoticonBank.emoticons);

            var wait = new WaitForSeconds(npcMessageSpan);
            while (_isRunning)
            {
                yield return wait;
                var index = Random.Range(0, npcEmoticonBank.emoticons.Count);
                var emoticon = npcEmoticonBank.emoticons[index];
                messages.Append(ActorType.Npc, emoticon.content);
            }
        }

        [Button, ShowIf("_isRunning")]
        [UsedImplicitly]
        private void Stop()
        {
            _isRunning = false;
        }

        private void PlayerSendMessage()
        {
            var selectedEmoticons = emoticons.GetSelectedEmoticons().ToArray();
            var message = string.Concat(selectedEmoticons.Select(e => e.content));
            messages.Append(ActorType.Player, message);
            emoticons.SetEmoticons(playerEmoticonBank.emoticons);
        }
    }
}