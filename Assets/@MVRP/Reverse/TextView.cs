using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Reverse
{
    public class TextView : MonoBehaviour, ITextPrinter
    {
        [SerializeField] Text _text;

        public string Text { get => _text.text; set => _text.text = value; }
    }
}
