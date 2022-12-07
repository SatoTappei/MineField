using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// ���ʂ̃f�[�^�̎���:Model
/// </summary>
public class SoundVolume : MonoBehaviour
{
    public readonly int MaxValue = 100;

    readonly IntReactiveProperty _value = new IntReactiveProperty(0);

    /// <summary>�ǂݎ���p�Ƃ��ĊO���Ɍ��J</summary>
    public IReadOnlyReactiveProperty<int> Value => _value;

    public void SetValue(int value)
    {
        // �ő剹�ʂ͂��̃N���X�������Ă���̂ł����ŕ␳������H
        // Presenter�ɂ͏�Ԃ̕ێ����������Ȃ�
        value = Mathf.Clamp(value, 0, MaxValue);
        _value.Value = value;
    }

    void OnDestroy()
    {
        _value.Dispose();
    }
}
