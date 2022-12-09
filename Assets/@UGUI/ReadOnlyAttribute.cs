using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// クラス名のAttributeの前が属性の名前になる
/// この場合はReadOnlyが属性名になる
/// </summary>
public class ReadOnlyAttribute : PropertyAttribute
{

}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyAttributeDrawer : PropertyDrawer
{
    // インスペクターで表示するためのメソッド
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 編集できないここから(引数をtrueにする)
        EditorGUI.BeginDisabledGroup(true);
        // インスペクターに表示するメソッド
        EditorGUI.PropertyField(position, property, label, true);
        // 編集できないここまで
        EditorGUI.EndDisabledGroup();
    }
}
#endif
