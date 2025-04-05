using System.Collections;
using _DeepChat.Scripts.Common;
using _DeepChat.Scripts.Data;
using _DeepChat.Scripts.ViewCtrls;
using NaughtyAttributes;
using UnityEngine;

namespace _DeepChat.Scripts.Logic
{
    public class ChatGameCtrl : MonoBehaviour
    {
        [SerializeField] private MessageListViewCtrl messages;
        [SerializeField] private float npcMessageSpan = 5.0f;
        [SerializeField] private EmoticonBank npcMessageBank;
        [SerializeField] private bool runOnStart;

        private bool _isRunning;

        private void Start()
        {
            if (!runOnStart)
                return;
            StartCoroutine(Run());
        }

        private IEnumerator Run()
        {
            _isRunning = true;
            var wait = new WaitForSeconds(npcMessageSpan);
            while (_isRunning)
            {
                yield return wait;
                var index = Random.Range(0, npcMessageBank.emoticons.Count);
                var emoticon = npcMessageBank.emoticons[index];
                messages.Append(ActorType.Npc, emoticon);
            }
        }

        [Button, ShowIf("_isRunning")]
        private void Stop()
        {
            _isRunning = false;
        }
    }
}