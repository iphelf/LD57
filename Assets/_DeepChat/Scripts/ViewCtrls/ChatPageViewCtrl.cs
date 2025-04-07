using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _DeepChat.Scripts.Data;
using _DeepChat.Scripts.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class ChatPageViewCtrl : MonoBehaviour, IChatGameView
    {
        [SerializeField] private GameRule gameRule;

        [Header("UI Components")] [SerializeField]
        private Button closeButton;

        [SerializeField] private Button helpButton;
        [SerializeField] private MessageListViewCtrl messages;
        [SerializeField] private EmoticonListViewCtrl emoticons;
        [SerializeField] private Button sendButton;
        [SerializeField] private ScoreViewCtrl score;
        [SerializeField] private PowerViewCtrl power;
        [SerializeField] private InputFieldViewCtrl inputField;
        [SerializeField] private CountdownViewCtrl countdown;

        [Header("UI Dialogs")] [SerializeField]
        private EndingDialogViewCtrl endingDialog;

        private AwaitableCompletionSource<List<Emoticon>> _waitingPlayerAction;

        public event Action OnClose;

        private void Awake()
        {
            closeButton.onClick.AddListener(() =>
            {
                OnClose?.Invoke();
                Destroy(gameObject);
            });
            sendButton.onClick.AddListener(OnPlayerSendButtonClicked);
            emoticons.SelectionChanged += OnPlayerEmoticonSelectionChanged;

            endingDialog.OnRestartButtonClicked += () => StartCoroutine(LaunchGame());
            endingDialog.OnExitButtonClicked += () =>
            {
                OnClose?.Invoke();
                Destroy(gameObject);
            };
        }

        private void Start()
        {
            StartCoroutine(LaunchGame());
        }

        private async Awaitable LaunchGame()
        {
            try
            {
                var game = new ChatGame();
                var goodResult = await game.Run(destroyCancellationToken, gameRule, this);
                endingDialog.OpenEnding(goodResult);
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

        public void Reset()
        {
            messages.Clear();
        }

        public async Awaitable AsyncNpcSendMessage(CancellationToken token, Message message)
        {
            await messages.AsyncAppendNpcMessage(token, message.content);
        }

        public async Awaitable<List<Emoticon>> AsyncWaitForPlayerAction(CancellationToken token, float maxWaitSeconds)
        {
            inputField.SetAsHintingInput();
            _waitingPlayerAction = new AwaitableCompletionSource<List<Emoticon>>();
            countdown.StartCountdown(maxWaitSeconds, () => FinishPlayerAction(false));
            await messages.AsyncScrollToBottom(token);
            var selectedEmoticons = await _waitingPlayerAction.Awaitable;
            return selectedEmoticons;
        }

        public async Awaitable AsyncPlayerSendMessage(CancellationToken token, string messageContent)
        {
            if (messageContent == null)
                await messages.AsyncAppendPlayerBusyMessage(token);
            else
                await messages.AsyncAppendPlayerMessage(token, messageContent);
        }

        public async Awaitable AsyncPresentTurnResult(
            CancellationToken token, TurnActionResult actionResult, Rating rating)
        {
            if (!actionResult.IsTimeout)
                await messages.AsyncMatchMessagePair(token, rating.WidthMatchResult, actionResult.MatchWidthDiff > 0);
            await messages.AsyncAppendRating(token, rating);
        }

        public async Awaitable AsyncPresentNewScore(int newScore, int maxScore)
        {
            score.UpdateScore(newScore, maxScore);
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