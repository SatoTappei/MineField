using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 動的に生成されたUnitの情報を表示する:View
/// </summary>
public class DynamicView : MonoBehaviour
{
    [SerializeField] Text _nameFrame;
    [SerializeField] Transform _hPBar;

    public void SetName(string name) => _nameFrame.text = name;

    public void SetHP(float value)
    {
        Vector3 scale = _hPBar.localScale;
        scale.x = value;
        _hPBar.localScale = scale;
    }
        
}
