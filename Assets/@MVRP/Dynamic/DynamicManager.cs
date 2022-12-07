using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using Cysharp.Threading.Tasks;

/// <summary>
/// DynamicUnitを生成するコンポーネント
/// </summary>
public class DynamicManager : MonoBehaviour
{
    readonly ReactiveCollection<DynamicUnit> _units = new ReactiveCollection<DynamicUnit>();

    [SerializeField] DynamicUnit _unitPrefab;

    public IReactiveCollection<DynamicUnit> Units => _units;

    async UniTaskVoid Start()
    {
        for (int i = 1; i <= 4; i++)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            _units.Add(Create(i));
        }
    }

    DynamicUnit Create(int id)
    {
        DynamicUnit unit = Instantiate(_unitPrefab, 
            Vector3.right * UnityEngine.Random.Range(-5f, 5f), Quaternion.identity);
        unit.name = "Unit" + id;

        return unit;
    }
}
