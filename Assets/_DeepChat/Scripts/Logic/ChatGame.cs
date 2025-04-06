using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _DeepChat.Scripts.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _DeepChat.Scripts.Logic
{
    public class ChatGame
    {
        private GameRule _rule;
        private IChatGameView _view;

        private int _score;
        private readonly List<Emoticon> _playerEmoticons = new();

        public async Awaitable Run(CancellationToken token, GameRule rule, IChatGameView view)
        {
            _rule = rule;
            _view = view;

            try
            {
                await AsyncRefillPlayerEmoticons(token);

                while (!token.IsCancellationRequested)
                {
                    var index = Random.Range(0, _rule.npcMessageBank.messages.Count);
                    var message = _rule.npcMessageBank.messages[index];
                    await _view.AsyncNpcSendMessage(token, message);

                    var selectedEmoticons = await _view.AsyncWaitForPlayerAction(token, _rule.npcMaxWaitDuration);
                    for (var i = _playerEmoticons.Count - 1; i >= 0; --i)
                        if (selectedEmoticons.Contains(_playerEmoticons[i]))
                            _playerEmoticons.RemoveAt(i);
                    var messageContent = string.Concat(selectedEmoticons.Select(e => e.content));
                    await _view.AsyncPlayerSendMessage(token, messageContent);
                    var diff = _view.GetMessageWidthDifference();
                    Debug.Log($"Difference against target width: {diff:F2}");
                    var distance = Mathf.Abs(diff);
                    var scoreDelta = _rule.GetScoreChange(distance);
                    _score += scoreDelta;
                    await _view.AsyncPresentTurnResult(token, null, _score);
                    await AsyncRefillPlayerEmoticons(token);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async Awaitable AsyncRefillPlayerEmoticons(CancellationToken token)
        {
            while (_playerEmoticons.Count < _rule.maxEmoticonCount)
            {
                var emoticon = _rule.SampleEmoticon(
                    sizeType => _playerEmoticons.Any(e => e.size == sizeType));
                _playerEmoticons.Add(emoticon);
            }

            await _view.AsyncRefreshPlayerEmoticons(token, _playerEmoticons);
        }
    }
}