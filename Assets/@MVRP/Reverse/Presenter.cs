using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Reverse
{
    public class Presenter : MonoBehaviour
    {
        // View�ւ̎Q�Ƃ��C���^�[�t�F�[�X�ōs��
        ITextPrinter _text;

        [SerializeField] Model _model;

        void Start()
        {
            _text = GetComponent<ITextPrinter>();

            _model.OnValueChanged += x =>
            {
                _text.Text = x.ToString();
            };
        }
    }
}
