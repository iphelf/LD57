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
        [SerializeField] private PowerViewCtrl power;
        [SerializeField] private InputFieldViewCtrl inputField;
        [SerializeField] private CountdownViewCtrl countdown;

        private AwaitableCompletionSource<List<Emoticon>> _waitingPlayerAction;

        public event Action OnClose;

        private void Awake()
        {
            sendButton.onClick.AddListener(OnPlayerSendButtonClicked);
            emoticons.SelectionChanged += OnPlayerEmoticonSelectionChanged;
        }

        private async void Start()
        {
            try
            {
                if (!runOnStart)
                    return;
                var game = new ChatGame();
                await game.Run(destroyCancellationToken, gameRule, this);
            }
            catch (Exception e)
            {
                Debug.Log($"{nameof(ChatPageViewCtrl)} error: {e}");
            }
        }

        private void FinishPlayerAction(bool success)
        {
            countdown.StopCountdown();
            inputField.SetAsBlockingInput();

            var result = success ? emoticons.GetSelectedEmoticons().ToList() : null;

            var waitingPlayerAction = _waitingPlayerAction;
            _waitingPlayerAction = null;
            waitingPlayerAction.SetResult(result);
        }

        private void OnPlayerSendButtonClicked()
        {
            if (_waitingPlayerAction == null)
            {
                Debug.Log("不许动");
                return;
            }

            FinishPlayerAction(true);
        }

        private void OnPlayerEmoticonSelectionChanged()
        {
            var selectedEmoticons = emoticons.GetSelectedEmoticons().ToArray();
            inputField.SetContent(selectedEmoticons);
        }

        public async Awaitable AsyncNpcSendMessage(CancellationToken token, Message message)
        {
            await messages.AsyncAppendNpcMessage(token, message.content);
        }

        public Awaitable<List<Emoticon>> AsyncWaitForPlayerAction(CancellationToken token, float maxWaitSeconds)
        {
            inputField.SetAsHintingInput();
            _waitingPlayerAction = new AwaitableCompletionSource<List<Emoticon>>();
            countdown.StartCountdown(maxWaitSeconds, () => FinishPlayerAction(false));
            return _waitingPlayerAction.Awaitable;
        }

        public async Awaitable AsyncPlayerSendMessage(CancellationToken token, string messageContent)
        {
            if (messageContent == null)
                await messages.AsyncAppendPlayerBusyMessage(token);
            else
                await messages.AsyncAppendPlayerMessage(token, messageContent);
        }

        public async Awaitable AsyncPresentTurnResult(CancellationToken token, Rating rating, int newScore)
        {
            await messages.AsyncMatchMessagePair(token);
            await messages.AsyncAppendRating(token, rating);
            score.UpdateScore(newScore);
        }

        public async Awaitable AsyncRefreshPlayerEmoticons(
            CancellationToken token, List<Emoticon> newEmoticons, int newPower)
        {
            emoticons.SetEmoticons(newEmoticons);
            power.SetValue(newPower);
        }

        public float GetMessageWidthDifference()
        {
            var diff = messages.GetDifferenceAgainstTarget();
            return diff;
        }
    }
}