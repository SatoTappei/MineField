using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

/// <summary>
/// 音量をスライダーに反映する:Presenter
/// </summary>
public class VolumePresenter : MonoBehaviour
{
    [Header("UIをそのまま参照する")]
    [SerializeField] Slider _slider;
    [SerializeField] Text _text;

    [SerializeField] SoundVolume _volume;

    void Start()
    {
        // Modelのボリュームが変わったときにViewのスライダーとテキストに反映する
        _volume.Value.Subscribe(i => 
        {
            _slider.value = (float)i / _volume.MaxValue;
            _text.text = "音量: " + i;
        }).AddTo(this);

        // Viewのスライダーの値が変わったときにModelのボリュームに反映する
        _slider.OnValueChangedAsObservable().Subscribe(f =>
        {
            // スライダーの取りうる値は0~1なので補正する
            // 100をかけている理由はボリュームの最大値が100だから
            // TODO:この100は後に最大値が200になったときにこっちにも反映する必要がある
            int value = (int)(100 * f);
            _volume.SetValue(value);
        }).AddTo(this);
    }

    void Update()
    {
        
    }
}
