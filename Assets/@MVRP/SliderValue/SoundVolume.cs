using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// 音量のデータの実体:Model
/// </summary>
public class SoundVolume : MonoBehaviour
{
    public readonly int MaxValue = 100;

    readonly IntReactiveProperty _value = new IntReactiveProperty(0);

    /// <summary>読み取り専用として外部に公開</summary>
    public IReadOnlyReactiveProperty<int> Value => _value;

    public void SetValue(int value)
    {
        // 最大音量はこのクラスが持っているのでここで補正をする？
        // Presenterには状態の保持をしたくない
        value = Mathf.Clamp(value, 0, MaxValue);
        _value.Value = value;
    }

    void OnDestroy()
    {
        _value.Dispose();
    }
}
