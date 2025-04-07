using UnityEngine;
using UnityEngine.UI;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class SpriteSelectableViewCtrl : MonoBehaviour
    {
        [SerializeField] private Selectable selectable;

        [SerializeField] private Sprite overrideDisabledStateSprite;

        private Sprite _originalSprite;

        private bool _wasInteractable = true;

        private void Awake()
        {
            _originalSprite = selectable.image.sprite;
        }

        private void Update()
        {
            if (selectable.interactable == _wasInteractable)
            {
                return;
            }

            _wasInteractable = selectable.interactable;
            if (overrideDisabledStateSprite)
                selectable.image.sprite = _wasInteractable ? _originalSprite : overrideDisabledStateSprite;
        }
    }
}