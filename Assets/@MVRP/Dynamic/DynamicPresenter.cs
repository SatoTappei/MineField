using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// ���I�ɐ������ꂽDynamicUnit�ɑ΂��ăo�C���h����:Presenter
/// </summary>
public class DynamicPresenter : MonoBehaviour
{
    /// <summary>���I�ɐ������ꂽModel��View���o�C���h����</summary>
    public void OnCreate(DynamicUnit unit, DynamicView view)
    {
        view.SetName(unit.name);

        unit.HP.Subscribe(i =>
        {
            view.SetHP((float)i / unit.MaxHp);
        }).AddTo(this);
    }
}
