using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// ユニットの体力をGUIに反映する:Presenter
/// </summary>
public class UnitHpPresenter : MonoBehaviour
{
    /// <summary>ModelとViewへの参照を持つ</summary>
    [SerializeField] Unit _unit;
    [SerializeField] HPBar _hPBar;

    void Start()
    {
        // Presenterで値の加工を行いViewに反映させる
        // ここでSubscribeをすることによって、このコンポーネントが無くなったらDisposeされる
        _unit.HP.Subscribe(i =>
        {
            _hPBar.SetValue(Mathf.Clamp01((float)i / _unit.MaxHp));
        }).AddTo(this);
    }

    void Update()
    {
        
    }
}
