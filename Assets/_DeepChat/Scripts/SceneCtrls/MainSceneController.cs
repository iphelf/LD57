using _DeepChat.Scripts.ViewCtrls;
using UnityEngine;

namespace _DeepChat.Scripts.SceneCtrls
{
    public class MainSceneController : MonoBehaviour
    {
        [SerializeField] private Transform pageRoot;
        [SerializeField] private TitlePageViewCtrl titlePage;
        [SerializeField] private GameObject creditsPagePrefab;
        [SerializeField] private GameObject chatPagePrefab;

        private void Awake()
        {
            titlePage.OnStartButtonClicked += TransitionFromTitleToChat;
            titlePage.OnCreditsButtonClicked += TransitionFromTitleToCredits;
        }

        private void TransitionFromTitleToChat()
        {
            titlePage.gameObject.SetActive(false);
            var go = Instantiate(chatPagePrefab, pageRoot);
            var chatPage = go.GetComponent<ChatPageViewCtrl>();
            chatPage.OnClose += () => titlePage.gameObject.SetActive(true);
        }

        private void TransitionFromTitleToCredits()
        {
            titlePage.gameObject.SetActive(false);
            var go = Instantiate(creditsPagePrefab, pageRoot);
            var creditsPage = go.GetComponent<CreditsPageViewCtrl>();
            creditsPage.OnClose += () => titlePage.gameObject.SetActive(true);
        }
    }
}