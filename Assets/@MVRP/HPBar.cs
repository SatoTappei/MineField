using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �v���C���[�̗̑͂𔽉f����Q�[�W:View
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
    /// �n���ꂽ�l�����̂܂ܔ��f����
    /// �l�̉��H��Presenter�������󂯂�̂ōs��Ȃ��B
    /// </summary>
    public void SetValue(float value)
    {
        Vector3 scale = _trans.localScale;
        scale.x = value;

        _trans.localScale = scale;
    }
}
