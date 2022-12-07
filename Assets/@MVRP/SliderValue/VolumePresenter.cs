using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

/// <summary>
/// ���ʂ��X���C�_�[�ɔ��f����:Presenter
/// </summary>
public class VolumePresenter : MonoBehaviour
{
    [Header("UI�����̂܂܎Q�Ƃ���")]
    [SerializeField] Slider _slider;
    [SerializeField] Text _text;

    [SerializeField] SoundVolume _volume;

    void Start()
    {
        // Model�̃{�����[�����ς�����Ƃ���View�̃X���C�_�[�ƃe�L�X�g�ɔ��f����
        _volume.Value.Subscribe(i => 
        {
            _slider.value = (float)i / _volume.MaxValue;
            _text.text = "����: " + i;
        }).AddTo(this);

        // View�̃X���C�_�[�̒l���ς�����Ƃ���Model�̃{�����[���ɔ��f����
        _slider.OnValueChangedAsObservable().Subscribe(f =>
        {
            // �X���C�_�[�̎�肤��l��0~1�Ȃ̂ŕ␳����
            // 100�������Ă��闝�R�̓{�����[���̍ő�l��100������
            // TODO:����100�͌�ɍő�l��200�ɂȂ����Ƃ��ɂ������ɂ����f����K�v������
            int value = (int)(100 * f);
            _volume.SetValue(value);
        }).AddTo(this);
    }

    void Update()
    {
        
    }
}
