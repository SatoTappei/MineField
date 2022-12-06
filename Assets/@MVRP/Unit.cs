using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// プレイヤーが操作するユニット:Model
/// </summary>
public class Unit : MonoBehaviour
{
    /// <summary>
    /// 体力というModelを持っている
    /// Readonlyにすることでクラスメンバのみ変更できる
    /// </summary>
    readonly IntReactiveProperty _hp = new IntReactiveProperty(100);

    /// <summary>監視させるために外部に読み取り専用で公開する</summary>
    public IReadOnlyReactiveProperty<int> HP => _hp;
    public int MaxHp { get; private set; }

    void Awake()
    {
        MaxHp = _hp.Value;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _hp.Value -= 20;
        }
    }

    private void OnDestroy()
    {
        _hp.Dispose();
    }
}
