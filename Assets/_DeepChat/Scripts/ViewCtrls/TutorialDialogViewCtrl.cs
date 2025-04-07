using _DeepChat.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class TutorialDialogViewCtrl : MonoBehaviour
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text text;
        [SerializeField] private Transform pageDotsRoot;
        [SerializeField] private TutorialViewConfig config;

        private int _index;

        private void OnEnable()
        {
            Time.timeScale = 0;
            closeButton.onClick.AddListener(OnCloseButtonClicked);
            nextButton.onClick.AddListener(OnNextButtonClicked);

            SetPage(0);
        }

        private void OnDisable()
        {
            closeButton.onClick.RemoveListener(OnCloseButtonClicked);
            nextButton.onClick.RemoveListener(OnNextButtonClicked);
            Time.timeScale = 1;
        }

        private void OnCloseButtonClicked()
        {
            gameObject.SetActive(false);
        }

        private void OnNextButtonClicked()
        {
            SetPage((_index + 1) % config.pages.Count);
        }

        private void SetPage(int index)
        {
            _index = index;
            var content = config.pages[_index];
            image.sprite = content.image;
            text.text = content.text;
            RefreshPageDots();
        }

        private void RefreshPageDots()
        {
        }
    }
}