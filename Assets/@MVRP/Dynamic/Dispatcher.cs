using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// DynamicManager�̃C�x���g�����m���āAPresenter�Ƀo�C���h����
/// </summary>
public class Dispatcher : MonoBehaviour
{
    [SerializeField] DynamicManager _manager;
    [SerializeField] DynamicPresenter _presenter;
    [SerializeField] DynamicView _viewPrefab;

    void Start()
    {
        // �����X�g�ɂ�����Dispatch
        foreach (DynamicUnit u in _manager.Units)
        {
            Dispatch(u);
        }

        // ���X�g�ɒǉ�����邽�т�Subscribe�����
        _manager.Units.ObserveAdd().Subscribe(u => Dispatch(u.Value)).AddTo(this);
    }

    void Dispatch(DynamicUnit unit)
    {
        // ���������e�A��O�����Ń��[���h���W�ɐݒ肵�Ă���
        DynamicView view = Instantiate(_viewPrefab, unit.transform, true);
        // UI�̈ʒu����
        view.transform.localPosition=Vector3.up * 1.5f;
        // Presenter�̑g�ݍ��킹�������Ă�
        _presenter.OnCreate(unit, view);
    }
}
