using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _DeepChat.Scripts.Data;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _DeepChat.Scripts.Logic
{
    public class ChatGameCtrl : MonoBehaviour
    {
        [Header("Config")] [SerializeField] private GameRule gameRule;
        [SerializeField] private bool runOnStart;

        private bool _isRunning;

        private int _score;
        private readonly List<Emoticon> _playerEmoticons = new();

        private bool _npcMessageTimeout;

        public event Action<int> ScoreChanged;
        public event Action<List<Emoticon>> PlayerEmoticonsChanged;
        public event Action PlayerTurnBegin;
        public event Action PlayerTurnEnd;
        public event Action<string> NpcMessageSent;

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
                var index = Random.Range(0, gameRule.npcMessageBank.messages.Count);
                var message = gameRule.npcMessageBank.messages[index];
                NpcMessageSent?.Invoke(message.content);

                PlayerTurnBegin?.Invoke();

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

        public string PlayerSendMessage(Emoticon[] selectedEmoticons)
        {
            for (var i = _playerEmoticons.Count - 1; i >= 0; --i)
                if (selectedEmoticons.Contains(_playerEmoticons[i]))
                    _playerEmoticons.RemoveAt(i);

            var message = string.Concat(selectedEmoticons.Select(e => e.content));
            return message;
        }

        public void SettleTurn(float distance)
        {
            PlayerTurnEnd?.Invoke();

            var scoreChange = gameRule.GetScoreChange(distance);
            _score += scoreChange;
            ScoreChanged?.Invoke(_score);

            RefillPlayerEmoticons();
        }

        private void RefillPlayerEmoticons()
        {
            while (_playerEmoticons.Count < gameRule.maxEmoticonCount)
            {
                var emoticon = gameRule.SampleEmoticon(
                    sizeType => _playerEmoticons.Any(e => e.size == sizeType));
                _playerEmoticons.Add(emoticon);
            }

            PlayerEmoticonsChanged?.Invoke(_playerEmoticons);
        }
    }
}