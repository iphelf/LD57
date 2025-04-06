using UnityEngine;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class MismatchIndicatorViewCtrl : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Vector2 extend = new(0.0f, 10.0f);

        public void ResizeRectTransform(Vector2 center, Vector2 size)
        {
            size += extend;
            var rectTransform = GetComponent<RectTransform>();
            var localPosition = center;
            localPosition.y += size.y * 0.5f;
            rectTransform.localPosition = localPosition;
            rectTransform.sizeDelta = size;
        }

        public CanvasGroup GetCanvasGroup()
        {
            return canvasGroup;
        }
    }
}