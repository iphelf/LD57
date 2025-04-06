using System;
using System.Collections.Generic;
using System.Linq;
using _DeepChat.Scripts.Logic;
using NaughtyAttributes;
using UnityEngine;

namespace _DeepChat.Scripts.ViewCtrls
{
    public class EmoticonListViewCtrl : MonoBehaviour
    {
        [SerializeField] private GameObject emoticonPrefab;
        [SerializeField] private Transform listRoot;

        [SerializeField] private bool clearOnAwake;

        private readonly List<EmoticonViewCtrl> _emoticons = new();
        public event Action SelectionChanged;

        private void Awake()
        {
            if (clearOnAwake)
                Clear();
        }

        public IEnumerable<Emoticon> GetSelectedEmoticons()
        {
            return _emoticons.Where(v => v.IsChecked()).Select(v => v.Content);
        }

        public void SetEmoticons(IEnumerable<Emoticon> emoticons)
        {
            Clear();

            foreach (var emoticon in emoticons)
            {
                var go = Instantiate(emoticonPrefab, listRoot);
                var view = go.GetComponent<EmoticonViewCtrl>();
                view.SetContent(emoticon);
                view.CheckStateChanged += SelectionStateChangedByUser;
                _emoticons.Add(view);
            }
        }

        [Button]
        private void Clear()
        {
            foreach (var emoticon in _emoticons)
                Destroy(emoticon.gameObject);
            _emoticons.Clear();

            for (var i = listRoot.childCount - 1; i >= 0; --i)
                Destroy(listRoot.GetChild(i).gameObject);
        }

        private void SelectionStateChangedByUser()
        {
            SelectionChanged?.Invoke();
        }
    }
}