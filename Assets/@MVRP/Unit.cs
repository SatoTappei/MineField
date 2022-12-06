using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// �v���C���[�����삷�郆�j�b�g:Model
/// </summary>
public class Unit : MonoBehaviour
{
    /// <summary>
    /// �̗͂Ƃ���Model�������Ă���
    /// Readonly�ɂ��邱�ƂŃN���X�����o�̂ݕύX�ł���
    /// </summary>
    readonly IntReactiveProperty _hp = new IntReactiveProperty(100);

    /// <summary>�Ď������邽�߂ɊO���ɓǂݎ���p�Ō��J����</summary>
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
