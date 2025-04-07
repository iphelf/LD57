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

        private AwaitableCompletionSource<List<int>> _waitingPlayerAction;

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
                var game = new ChatGame(gameRule, this);
                var goodResult = await game.Run(destroyCancellationToken);
                endingDialog.OpenEnding(goodResult);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void FinishPlayerAction(bool success)
        {
            countdown.StopCountdown();
            inputField.SetAsBlockingInput();

            var result = success ? emoticons.GetSelectedIndices().ToList() : null;
            if (success)
            {
                emoticons.RemoveSelectedEmoticons();
            }

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
            sendButton.interactable = selectedEmoticons.Length > 0;
        }

        public void Reset()
        {
            messages.Clear();
        }

        public async Awaitable AsyncNpcSendMessage(CancellationToken token, Message message)
        {
            await messages.AsyncAppendNpcMessage(token, message.content);
        }

        public async Awaitable<List<int>> AsyncWaitForPlayerAction(CancellationToken token, float maxWaitSeconds)
        {
            if (emoticons.HasAnySelectedEmoticons())
            {
                inputField.SetContent(emoticons.GetSelectedEmoticons().ToArray());
                sendButton.interactable = true;
            }
            else
            {
                inputField.SetAsHintingInput();
                sendButton.interactable = false;
            }

            _waitingPlayerAction = new AwaitableCompletionSource<List<int>>();
            countdown.StartCountdown(maxWaitSeconds, () => FinishPlayerAction(false));
            await messages.AsyncScrollToBottom(token);
            var selectedEmoticons = _waitingPlayerAction == null ? null : await _waitingPlayerAction.Awaitable;
            sendButton.interactable = false;
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

        public async Awaitable AsyncFillPlayerEmoticons(
            CancellationToken token, List<Emoticon> newEmoticons, int newPower)
        {
            emoticons.FillEmoticons(newEmoticons);
            power.SetValue(newPower);
        }

        public async Awaitable AsyncAppendPlayerEmoticons(
            CancellationToken token, List<Emoticon> newEmoticons, int newPower)
        {
            emoticons.AppendEmoticons(newEmoticons);
            power.SetValue(newPower);
        }

        public float GetMessageWidthDifference()
        {
            var diff = messages.GetDifferenceAgainstTarget();
            return diff;
        }
    }
}