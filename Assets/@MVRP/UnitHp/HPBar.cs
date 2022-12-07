using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プレイヤーの体力を反映するゲージ:View
/// </summary>
public class HPBar : MonoBehaviour
{
    [SerializeField] Transform _trans;

    void Start()
    {
        
    }

    void Update()
    {

    }

    /// <summary>
    /// 渡された値をそのまま反映する
    /// 値の加工はPresenterが引き受けるので行わない。
    /// </summary>
    public void SetValue(float value)
    {
        Vector3 scale = _trans.localScale;
        scale.x = value;

        _trans.localScale = scale;
    }
}
