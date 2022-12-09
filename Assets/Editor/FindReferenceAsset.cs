using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; // �K�v

/// <summary>
/// �E�B���h�E��\��������ɂ�EditorWindow�N���X���p��������K�v������
/// </summary>
public class FindReferenceAsset : EditorWindow
{
    const string findType = @"t:scene t:prefab t:timelineAsset
                              t:animatorcontroller t:Material";

    // �t�@�C�����E�N���b�N�����Ƃ��ɏo�郁�j���[�ɕ\��������ɂ�
    // ��2������false�ɂ���K�v������
    [MenuItem("Assets/�Q�Ƃ�T��", false)]
    public static void FindAssets()
    {
        Debug.Log("�Q�Ƃ�T���n�߂܂�");

        // �I�𒆂̃I�u�W�F�N�g���擾����
        Object[] selects = Selection.objects;

        // �I�𒆂̃I�u�W�F�N�g���Ȃ���Ώ����𔲂���
        if (selects == null || selects.Length == 0) return;

        // �I�𒆂̃I�u�W�F�N�g�ɑ΂��Ă̏���
        foreach(Object selectItem in selects)
        {
            Debug.Log(selectItem.name);
        }

        // �w�肵���I�u�W�F�N�g��guid���擾���Ĕz��Ɋi�[����
        string[] ids = AssetDatabase.FindAssets(findType);

        // �e�I�u�W�F�N�g��guid�ɑ΂��ď���������
        foreach (string guid in ids)
        {
            // guid�Ńp�X���擾���Ă���
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // guid���g���Ĉˑ��֌W�ɂ���A�Z�b�g���擾����
            string[] deps = AssetDatabase.GetDependencies(assetPath);
            foreach (string dep in deps)
            {
                Debug.Log(dep);
            }
        }

        // �E�B���h�E��\�������郁�\�b�h
        GetWindow<FindReferenceAsset>();
    }
}
