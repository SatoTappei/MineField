using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// �N���X����Attribute�̑O�������̖��O�ɂȂ�
/// ���̏ꍇ��ReadOnly���������ɂȂ�
/// </summary>
public class ReadOnlyAttribute : PropertyAttribute
{

}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyAttributeDrawer : PropertyDrawer
{
    // �C���X�y�N�^�[�ŕ\�����邽�߂̃��\�b�h
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // �ҏW�ł��Ȃ���������(������true�ɂ���)
        EditorGUI.BeginDisabledGroup(true);
        // �C���X�y�N�^�[�ɕ\�����郁�\�b�h
        EditorGUI.PropertyField(position, property, label, true);
        // �ҏW�ł��Ȃ������܂�
        EditorGUI.EndDisabledGroup();
    }
}
#endif
