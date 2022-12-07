using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// DynamicManagerのイベントを検知して、Presenterにバインドする
/// </summary>
public class Dispatcher : MonoBehaviour
{
    [SerializeField] DynamicManager _manager;
    [SerializeField] DynamicPresenter _presenter;
    [SerializeField] DynamicView _viewPrefab;

    void Start()
    {
        // 今リストにあるやつをDispatch
        foreach (DynamicUnit u in _manager.Units)
        {
            Dispatch(u);
        }

        // リストに追加されるたびにSubscribeされる
        _manager.Units.ObserveAdd().Subscribe(u => Dispatch(u.Value)).AddTo(this);
    }

    void Dispatch(DynamicUnit unit)
    {
        // 第二引数が親、第三引数でワールド座標に設定している
        DynamicView view = Instantiate(_viewPrefab, unit.transform, true);
        // UIの位置調整
        view.transform.localPosition=Vector3.up * 1.5f;
        // Presenterの組み合わせ処理を呼ぶ
        _presenter.OnCreate(unit, view);
    }
}
