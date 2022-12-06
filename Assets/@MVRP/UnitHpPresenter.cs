using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// ���j�b�g�̗̑͂�GUI�ɔ��f����:Presenter
/// </summary>
public class UnitHpPresenter : MonoBehaviour
{
    /// <summary>Model��View�ւ̎Q�Ƃ�����</summary>
    [SerializeField] Unit _unit;
    [SerializeField] HPBar _hPBar;

    void Start()
    {
        // Presenter�Œl�̉��H���s��View�ɔ��f������
        // ������Subscribe�����邱�Ƃɂ���āA���̃R���|�[�l���g�������Ȃ�����Dispose�����
        _unit.HP.Subscribe(i =>
        {
            _hPBar.SetValue(Mathf.Clamp01((float)i / _unit.MaxHp));
        }).AddTo(this);
    }

    void Update()
    {
        
    }
}
