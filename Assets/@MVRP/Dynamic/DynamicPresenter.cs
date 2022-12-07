using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// 動的に生成されたDynamicUnitに対してバインドする:Presenter
/// </summary>
public class DynamicPresenter : MonoBehaviour
{
    /// <summary>動的に生成されたModelとViewをバインドする</summary>
    public void OnCreate(DynamicUnit unit, DynamicView view)
    {
        view.SetName(unit.name);

        unit.HP.Subscribe(i =>
        {
            view.SetHP((float)i / unit.MaxHp);
        }).AddTo(this);
    }
}
