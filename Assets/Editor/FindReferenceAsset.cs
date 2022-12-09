using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; // 必要

/// <summary>
/// ウィンドウを表示させるにはEditorWindowクラスを継承させる必要がある
/// </summary>
public class FindReferenceAsset : EditorWindow
{
    const string findType = @"t:scene t:prefab t:timelineAsset
                              t:animatorcontroller t:Material";

    // ファイルを右クリックしたときに出るメニューに表示させるには
    // 第2引数をfalseにする必要がある
    [MenuItem("Assets/参照を探す", false)]
    public static void FindAssets()
    {
        Debug.Log("参照を探し始めます");

        // 選択中のオブジェクトを取得する
        Object[] selects = Selection.objects;

        // 選択中のオブジェクトがなければ処理を抜ける
        if (selects == null || selects.Length == 0) return;

        // 選択中のオブジェクトに対しての処理
        foreach(Object selectItem in selects)
        {
            Debug.Log(selectItem.name);
        }

        // 指定したオブジェクトのguidを取得して配列に格納する
        string[] ids = AssetDatabase.FindAssets(findType);

        // 各オブジェクトのguidに対して処理をする
        foreach (string guid in ids)
        {
            // guidでパスを取得してくる
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // guidを使って依存関係にあるアセットを取得する
            string[] deps = AssetDatabase.GetDependencies(assetPath);
            foreach (string dep in deps)
            {
                Debug.Log(dep);
            }
        }

        // ウィンドウを表示させるメソッド
        GetWindow<FindReferenceAsset>();
    }
}
