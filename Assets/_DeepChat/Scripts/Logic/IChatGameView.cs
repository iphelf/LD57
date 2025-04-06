using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;

namespace _DeepChat.Scripts.Logic
{
    public interface IChatGameView
    {
        public void Reset();
        public Awaitable AsyncNpcSendMessage(CancellationToken token, Message message);
        public Awaitable<List<Emoticon>> AsyncWaitForPlayerAction(CancellationToken token, float maxWaitSeconds);
        public Awaitable AsyncPlayerSendMessage(CancellationToken token, [CanBeNull] string messageContent);
        public Awaitable AsyncPresentTurnResult(CancellationToken token, TurnActionResult actionResult, Rating rating);
        public Awaitable AsyncPresentNewScore(int newScore, int maxScore);
        public Awaitable AsyncRefreshPlayerEmoticons(CancellationToken token, List<Emoticon> emoticons, int newPower);

        public float GetMessageWidthDifference();
    }
}