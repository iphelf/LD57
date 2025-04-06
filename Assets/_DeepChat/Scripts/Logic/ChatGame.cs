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
        private GameRule _rule;
        private IChatGameView _view;

        private int _score;
        private MessageSampler _npcMessageSampler;
        private readonly List<Emoticon> _playerEmoticons = new();
        private int _remainingEmoticons;

        public async Awaitable<bool> Run(CancellationToken token, GameRule rule, IChatGameView view)
        {
            _rule = rule;
            _view = view;

            _npcMessageSampler = new MessageSampler(_rule.npcMessageBank);
            _remainingEmoticons = rule.maxEmotionCountInDeck;

            _view.Reset();
            _score = _rule.initialScore;

            await _view.AsyncPresentNewScore(_score, _rule.minScoreForHappyEnd);
            await AsyncFillPlayerEmoticons(token);

            while (!token.IsCancellationRequested)
            {
                var message = _npcMessageSampler.Sample();
                await _view.AsyncNpcSendMessage(token, message);

                var selectedEmoticons = await _view.AsyncWaitForPlayerAction(token, _rule.npcMaxWaitDuration);
                if (selectedEmoticons == null)
                {
                    await _view.AsyncPlayerSendMessage(token, null);
                }
                else
                {
                    for (var i = _playerEmoticons.Count - 1; i >= 0; --i)
                        if (selectedEmoticons.Contains(_playerEmoticons[i]))
                            _playerEmoticons.RemoveAt(i);
                    var messageContent = string.Concat(selectedEmoticons.Select(e => e.content));
                    await _view.AsyncPlayerSendMessage(token, messageContent);
                }

                var turnActionResult = new TurnActionResult
                {
                    IsTimeout = selectedEmoticons == null,
                    MatchWidthDiff = selectedEmoticons == null ? 0.0f : _view.GetMessageWidthDifference(),
                    NpcEmotion = message.emotion,
                    PlayerEmotion = selectedEmoticons == null
                        ? EmotionType.None
                        : GetEmotion(selectedEmoticons.Select(e => e.emotion)),
                };
                if (!turnActionResult.IsTimeout)
                    Debug.Log($"Difference against target width: {turnActionResult.MatchWidthDiff:F2}");
                var rating = _rule.RateTurnActionResult(turnActionResult);
                _score += rating.WidthMatchScore + rating.EmotionMatchScore;
                await _view.AsyncPresentTurnResult(token, turnActionResult, rating);
                await _view.AsyncPresentNewScore(_score, _rule.minScoreForHappyEnd);

                if (_score >= _rule.minScoreForHappyEnd)
                {
                    return true;
                }

                if (_score <= _rule.maxScoreForBadEnd || _playerEmoticons.Count + _remainingEmoticons <= 0)
                {
                    return false;
                }

                await AsyncRefillPlayerEmoticons(token);
            }

            token.ThrowIfCancellationRequested();
            // not reachable
            return false;
        }

        private async Awaitable AsyncFillPlayerEmoticons(CancellationToken token)
        {
            for (var i = 0; i < _rule.maxEmoticonCountInHand; ++i)
            {
                var emoticon = _rule.SampleEmoticon(
                    sizeType => _playerEmoticons.Any(e => e.size == sizeType));
                _playerEmoticons.Add(emoticon);
            }

            await _view.AsyncRefreshPlayerEmoticons(token, _playerEmoticons, _remainingEmoticons);
        }

        private async Awaitable AsyncRefillPlayerEmoticons(CancellationToken token)
        {
            while (_remainingEmoticons > 0 && _playerEmoticons.Count < _rule.maxEmoticonCountInHand)
            {
                --_remainingEmoticons;
                var emoticon = _rule.SampleEmoticon(
                    sizeType => _playerEmoticons.Any(e => e.size == sizeType));
                _playerEmoticons.Add(emoticon);
            }

            await _view.AsyncRefreshPlayerEmoticons(token, _playerEmoticons, _remainingEmoticons);
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