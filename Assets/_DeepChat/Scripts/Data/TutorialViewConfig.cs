using System;
using System.Collections.Generic;
using UnityEngine;

namespace _DeepChat.Scripts.Data
{
    [CreateAssetMenu(fileName = "TutorialViewConfig", menuName = "SO/TutorialViewConfig")]
    public class TutorialViewConfig : ScriptableObject
    {
        [Serializable]
        public class Page
        {
            public Sprite image;
            public string text;
        }

        public List<Page> pages = new();
    }
}