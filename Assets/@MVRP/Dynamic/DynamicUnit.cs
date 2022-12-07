using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// 動的に生成されるユニット:Model
/// </summary>
public class DynamicUnit : MonoBehaviour
{
    public readonly int MaxHp = 100;
    readonly IntReactiveProperty _hp = new IntReactiveProperty(100);

    public IReadOnlyReactiveProperty<int> HP => _hp;

    void Awake()
    {
        _hp.Value = MaxHp;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            _hp.Value -= 10;
        }
    }

    void OnDestroy()
    {
        _hp.Dispose();
    }
}
