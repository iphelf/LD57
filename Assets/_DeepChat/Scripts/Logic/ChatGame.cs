using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _DeepChat.Scripts.Common;
using _DeepChat.Scripts.Data;
using UnityEngine;

namespace _DeepChat.Scripts.Logic
{
    public class ChatGame
    {
        private readonly GameRule _rule;
        private readonly IChatGameView _view;

        private int _score;
        private readonly MessageSampler _npcMessageSampler;
        private readonly List<Emoticon> _playerEmoticons;
        private int _remainingEmoticons;

        public ChatGame(GameRule rule, IChatGameView view)
        {
            _rule = rule;
            _view = view;

            _npcMessageSampler = new MessageSampler(_rule.npcMessageBank);
            _playerEmoticons = new List<Emoticon>();
            _remainingEmoticons = rule.maxEmotionCountInDeck;
            _score = _rule.initialScore;
        }

        public async Awaitable<bool> Run(CancellationToken token)
        {
            _view.Reset();

            await _view.AsyncPresentNewScore(_score, _rule.minScoreForHappyEnd, true);
            await AsyncFillPlayerEmoticons(token);

            while (true)
            {
                var message = _npcMessageSampler.Sample();
                await _view.AsyncNpcSendMessage(token, message);

                var waitDuration = _rule.npcMaxWaitDuration - (1.0f * _score / _rule.minScoreForHappyEnd - 0.5f) * 1.3f;
                // var waitDuration = _rule.npcMaxWaitDuration;
                var selectedEmoticonIndices = await _view.AsyncWaitForPlayerAction(token, waitDuration);
                var selectedEmotion = selectedEmoticonIndices == null
                    ? EmotionType.None
                    : GetEmotion(selectedEmoticonIndices.Select(i => _playerEmoticons[i].emotion));
                if (selectedEmoticonIndices == null)
                {
                    await _view.AsyncPlayerSendMessage(token, null);
                }
                else
                {
                    var messageContent = string.Concat(
                        selectedEmoticonIndices.Select(i => _playerEmoticons[i].content));
                    foreach (var i in selectedEmoticonIndices)
                        _playerEmoticons[i] = null;
                    await _view.AsyncPlayerSendMessage(token, messageContent);
                }

                var turnActionResult = new TurnActionResult
                {
                    IsTimeout = selectedEmoticonIndices == null,
                    MatchWidthDiff = selectedEmoticonIndices == null ? 0.0f : _view.GetMessageWidthDifference(),
                    NpcEmotion = message.emotion,
                    PlayerEmotion = selectedEmotion,
                };
                if (!turnActionResult.IsTimeout)
                    Debug.Log($"Difference against target width: {turnActionResult.MatchWidthDiff:F2}");
                var rating = _rule.RateTurnActionResult(turnActionResult);
                _score += rating.WidthMatchScore + rating.EmotionMatchScore;
                await _view.AsyncPresentTurnResult(token, turnActionResult, rating);
                await _view.AsyncPresentNewScore(_score, _rule.minScoreForHappyEnd);

                if (_score >= _rule.minScoreForHappyEnd)
                    return true;

                if (_score <= _rule.maxScoreForBadEnd)
                    return false;

                await AsyncRefillPlayerEmoticons(token);

                if (_playerEmoticons.Count(e => e != null) + _remainingEmoticons <= 0)
                    return false;
            }
        }

        private async Awaitable AsyncFillPlayerEmoticons(CancellationToken token)
        {
            for (var i = 0; i < _rule.maxEmoticonCountInHand; ++i)
            {
                var emoticon = _rule.SampleEmoticon(
                    sizeType => _playerEmoticons.Any(e => e?.size == sizeType));
                _playerEmoticons.Add(emoticon);
            }

            await _view.AsyncFillPlayerEmoticons(token, _playerEmoticons, _remainingEmoticons);
        }

        private async Awaitable AsyncRefillPlayerEmoticons(CancellationToken token)
        {
            List<Emoticon> refilledEmoticons = new();
            for (var i = 0; _remainingEmoticons > 0 && i < _playerEmoticons.Count; ++i)
            {
                if (_playerEmoticons[i] != null)
                    continue;
                var emoticon = _rule.SampleEmoticon(
                    sizeType => _playerEmoticons.Any(e => e?.size == sizeType));
                --_remainingEmoticons;
                refilledEmoticons.Add(emoticon);
                _playerEmoticons[i] = emoticon;
            }

            await _view.AsyncAppendPlayerEmoticons(token, refilledEmoticons, _remainingEmoticons);
        }

        private static EmotionType GetEmotion(IEnumerable<EmotionType> emotionTypes)
        {
            var lastEmotion = EmotionType.None;
            foreach (var emotionType in emotionTypes)
            {
                if (lastEmotion == EmotionType.None)
                {
                    lastEmotion = emotionType;
                    continue;
                }

                if (emotionType != lastEmotion)
                {
                    return EmotionType.None;
                }
            }

            return lastEmotion;
        }
    }
}