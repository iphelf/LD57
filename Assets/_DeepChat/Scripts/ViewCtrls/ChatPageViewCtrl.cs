using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _DeepChat.Scripts.Common;
using _DeepChat.Scripts.Data;
using _DeepChat.Scripts.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class ChatPageViewCtrl : MonoBehaviour, IChatGameView
    {
        [SerializeField] private GameRule gameRule;
        [SerializeField] private bool runOnStart;

        [Header("UI Components")] [SerializeField]
        private MessageListViewCtrl messages;

        [SerializeField] private EmoticonListViewCtrl emoticons;
        [SerializeField] private Button sendButton;
        [SerializeField] private ScoreViewCtrl score;
        [SerializeField] private InputFieldViewCtrl inputField;
        [SerializeField] private CountdownViewCtrl countdown;

        private AwaitableCompletionSource<List<Emoticon>> _waitingPlayerAction;

        private void Awake()
        {
            sendButton.onClick.AddListener(OnPlayerSendButtonClicked);
            emoticons.SelectionChanged += OnPlayerEmoticonSelectionChanged;
        }

        private async void Start()
        {
            if (!runOnStart)
                return;
            var game = new ChatGame();
            await game.Run(destroyCancellationToken, gameRule, this);
        }

        private void OnPlayerSendButtonClicked()
        {
            if (_waitingPlayerAction == null)
            {
                Debug.Log("不许动");
                return;
            }

            var selectedEmoticons = emoticons.GetSelectedEmoticons().ToList();

            inputField.SetAsBlockingInput();

            var waitingPlayerAction = _waitingPlayerAction;
            _waitingPlayerAction = null;
            waitingPlayerAction.SetResult(selectedEmoticons);
        }

        private void OnPlayerEmoticonSelectionChanged()
        {
            var selectedEmoticons = emoticons.GetSelectedEmoticons().ToArray();
            inputField.SetContent(selectedEmoticons);
        }

        public async Awaitable AsyncNpcSendMessage(CancellationToken token, Message message)
        {
            messages.Append(ActorType.Npc, message.content);
        }

        public Awaitable<List<Emoticon>> AsyncWaitForPlayerAction(CancellationToken token, float maxWaitSeconds)
        {
            inputField.SetAsHintingInput();
            _waitingPlayerAction = new AwaitableCompletionSource<List<Emoticon>>();
            return _waitingPlayerAction.Awaitable;
        }

        public async Awaitable AsyncPlayerSendMessage(CancellationToken token, string messageContent)
        {
            messages.Append(ActorType.Player, messageContent);
        }

        public async Awaitable AsyncPresentTurnResult(CancellationToken token, Rating rating, int newScore)
        {
            // var content =
            //     $"width_match={rating.WidthMatchResult.ToString()}, score={rating.WidthMatchScore}, emotion={rating.NpcEmotion.ToString()}, emotion_match={rating.IsEmotionMatched}, bonus={rating.EmotionMatchScore}";
            // messages.Append(ActorType.Npc, content);
            score.UpdateScore(newScore);
        }

        public async Awaitable AsyncRefreshPlayerEmoticons(CancellationToken token, List<Emoticon> newEmoticons)
        {
            emoticons.SetEmoticons(newEmoticons);
        }

        public float GetMessageWidthDifference()
        {
            var diff = messages.GetDifferenceAgainstTarget();
            return diff;
        }
    }
}