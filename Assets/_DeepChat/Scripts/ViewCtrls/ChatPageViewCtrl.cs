using System.Collections.Generic;
using System.Linq;
using _DeepChat.Scripts.Common;
using _DeepChat.Scripts.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class ChatPageViewCtrl : MonoBehaviour
    {
        [Header("UI Components")] [SerializeField]
        private MessageListViewCtrl messages;

        [SerializeField] private EmoticonListViewCtrl emoticons;
        [SerializeField] private Button sendButton;
        [SerializeField] private ScoreViewCtrl score;
        [SerializeField] private InputFieldViewCtrl inputField;

        [Header("Logic Components")] [SerializeField]
        private ChatGameCtrl game;

        private void Awake()
        {
            sendButton.onClick.AddListener(OnPlayerSendButtonClicked);
            emoticons.SelectionChanged += OnPlayerEmoticonSelectionChanged;

            game.ScoreChanged += OnScoreChanged;
            game.PlayerTurnBegin += OnPlayerTurnBegin;
            game.PlayerEmoticonsChanged += OnPlayerEmoticonsChanged;
            game.PlayerTurnEnd += OnPlayerTurnEnd;
            game.NpcMessageSent += OnNpcMessageSent;
        }

        private void OnScoreChanged(int newScore)
        {
            score.UpdateScore(newScore);
        }

        private void OnPlayerEmoticonsChanged(List<Emoticon> newEmoticons)
        {
            emoticons.SetEmoticons(newEmoticons);
            inputField.SetAsHintingInput();
        }

        private void OnPlayerTurnBegin()
        {
            inputField.SetAsHintingInput();
        }

        private void OnPlayerSendButtonClicked()
        {
            var selectedEmoticons = emoticons.GetSelectedEmoticons().ToArray();
            var message = game.PlayerSendMessage(selectedEmoticons);
            messages.Append(ActorType.Player, message);

            var diff = messages.GetDifferenceAgainstTarget();
            Debug.Log($"Difference against target width: {diff:F2}");
            var dist = Mathf.Abs(diff);
            game.SettleTurn(dist);
        }

        private void OnPlayerTurnEnd()
        {
            inputField.SetAsBlockingInput();
        }

        private void OnNpcMessageSent(string content)
        {
            messages.Append(ActorType.Npc, content);
        }

        private void OnPlayerEmoticonSelectionChanged()
        {
            var selectedEmoticons = emoticons.GetSelectedEmoticons().ToArray();
            inputField.SetContent(selectedEmoticons);
        }
    }
}