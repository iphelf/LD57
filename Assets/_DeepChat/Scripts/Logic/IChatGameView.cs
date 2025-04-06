using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace _DeepChat.Scripts.Logic
{
    public interface IChatGameView
    {
        public Awaitable AsyncNpcSendMessage(CancellationToken token, Message message);
        public Awaitable<List<Emoticon>> AsyncWaitForPlayerAction(CancellationToken token, float maxWaitSeconds);
        public Awaitable AsyncPlayerSendMessage(CancellationToken token, string messageContent);
        public Awaitable AsyncPresentTurnResult(CancellationToken token, Rating rating, int newScore);
        public Awaitable AsyncRefreshPlayerEmoticons(CancellationToken token, List<Emoticon> emoticons);

        public float GetMessageWidthDifference();
    }
}