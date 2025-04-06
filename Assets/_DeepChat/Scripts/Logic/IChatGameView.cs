using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;

namespace _DeepChat.Scripts.Logic
{
    public interface IChatGameView
    {
        public Awaitable AsyncNpcSendMessage(CancellationToken token, Message message);
        public Awaitable<List<Emoticon>> AsyncWaitForPlayerAction(CancellationToken token, float maxWaitSeconds);
        public Awaitable AsyncPlayerSendMessage(CancellationToken token, [CanBeNull] string messageContent);
        public Awaitable AsyncPresentTurnResult(CancellationToken token, Rating rating, int newScore);
        public Awaitable AsyncRefreshPlayerEmoticons(CancellationToken token, List<Emoticon> emoticons, int newPower);

        public float GetMessageWidthDifference();
    }
}