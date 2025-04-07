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

        public IEnumerable<int> GetSelectedIndices()
        {
            return Enumerable.Range(0, _emoticons.Count).Where(i => _emoticons[i].IsChecked());
        }

        public IEnumerable<Emoticon> GetSelectedEmoticons()
        {
            return _emoticons.Where(e => e.IsChecked()).Select(e => e.Content);
        }

        public bool HasAnySelectedEmoticons()
        {
            return _emoticons.Any(v => v.IsChecked());
        }

        public void RemoveSelectedEmoticons()
        {
            foreach (var emoticonViewCtrl in _emoticons.Where(v => v.IsChecked()))
            {
                emoticonViewCtrl.SetUnchecked();
                emoticonViewCtrl.SetVisible(false);
            }
        }

        public void ClearSelection()
        {
            foreach (var emoticonViewCtrl in _emoticons.Where(v => v.IsChecked()))
            {
                emoticonViewCtrl.SetUnchecked();
            }
        }

        public void FillEmoticons(IEnumerable<Emoticon> emoticons)
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

        public void AppendEmoticons(IEnumerable<Emoticon> emoticons)
        {
            var index = 0;
            foreach (var emoticon in emoticons)
            {
                while (index < _emoticons.Count && _emoticons[index].IsVisible)
                    ++index;
                Debug.Assert(index < _emoticons.Count);
                _emoticons[index].SetContent(emoticon);
                _emoticons[index].SetVisible(true);
            }

            for (; index < _emoticons.Count; ++index)
                if (!_emoticons[index].IsVisible)
                {
                    _emoticons[index].SetBusy();
                    _emoticons[index].SetVisible(true);
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