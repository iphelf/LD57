using System.Collections;
using System.Collections.Generic;
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
        [Header("Components")] [SerializeField]
        private MessageListViewCtrl messages;

        [SerializeField] private EmoticonListViewCtrl emoticons;
        [SerializeField] private Button sendButton;
        [SerializeField] private ScoreViewCtrl score;

        [Header("Data")] [SerializeField] private GameRule gameRule;

        [SerializeField] private EmoticonBank playerEmoticonBank;

        // [SerializeField] private float npcMessageSpan = 5.0f;
        [SerializeField] private MessageBank npcMessageBank;

        [Header("Config")] [SerializeField] private bool runOnStart;

        private bool _isRunning;

        private int _score;
        private readonly List<Emoticon> _playerEmoticons = new();

        private bool _npcMessageTimeout;

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

            RefillPlayerEmoticons();

            while (_isRunning)
            {
                var index = Random.Range(0, npcMessageBank.messages.Count);
                var message = npcMessageBank.messages[index];
                messages.Append(ActorType.Npc, message.content);

                yield return new WaitUntil(() => _npcMessageTimeout);
                _npcMessageTimeout = false;
            }
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        [UsedImplicitly]
        private void NextNpcMessage()
        {
            _npcMessageTimeout = true;
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
            for (var i = _playerEmoticons.Count - 1; i >= 0; --i)
                if (selectedEmoticons.Contains(_playerEmoticons[i]))
                    _playerEmoticons.RemoveAt(i);

            var message = string.Concat(selectedEmoticons.Select(e => e.content));
            messages.Append(ActorType.Player, message);

            var diff = messages.GetDifferenceAgainstTarget();
            Debug.Log($"Difference against target width: {diff:F2}");
            var dist = Mathf.Abs(diff);
            var scoreChange = gameRule.GetScoreChange(dist);
            _score += scoreChange;
            score.UpdateScore(_score);

            RefillPlayerEmoticons();
        }

        private void RefillPlayerEmoticons()
        {
            while (_playerEmoticons.Count < gameRule.maxEmoticonCount)
            {
                var emoticon = gameRule.SampleEmoticon(playerEmoticonBank,
                    sizeType => _playerEmoticons.Any(e => e.size == sizeType));
                _playerEmoticons.Add(emoticon);
            }

            emoticons.SetEmoticons(_playerEmoticons);
        }
    }
}