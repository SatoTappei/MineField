using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

/// <summary>
/// �p�[�����m�C�Y����n�`�𐶐�����R���|�[�l���g
/// </summary>
public class Generator : MonoBehaviour
{
    [System.Serializable]
    struct Block
    {
        [SerializeField] int id;
        [SerializeField] GameObject go;

        public int Id { get => id; }
        public GameObject Go { get => go; }
    }

    [Header("��������u���b�N")]
    [SerializeField] Block[] _blocks;

    Dictionary<int, Block> _dic = new Dictionary<int, Block>();

    void Awake()
    {
        _dic = _blocks.ToDictionary(b => b.Id, b => b);
    }

    void Start()
    {
        // �C�ɓ��𗧂Ă�C���[�W


        // �O�w�\���H�ɂ���
        // �e�}�X�̍\���̂̓񎟌��z�񂪂ق���
        // ���ɂ͍��፷������A�Ⴂ�Ƃ���ɂ͌΁A�����Ƃ���͎R�ɂȂ�
        // �A���𐶂₷
        // ���������Ă���

        // 100*100�̃T�C�Y
        Terrain terrain = new Terrain(100, 100, Random.Range(0.0f, 100.0f), Random.Range(0.0f, 100.0f));
        // �C���C���[�ɃT�C�Y���̊C�̃}�X�𐶐�����
        terrain.sea.SetMassRange(0, 0, terrain.sea.Width - 1, terrain.sea.Depth - 1, new Mass(2,0));
        // ���n�𐶐�����
        terrain.land.SetMassRange(4, 4, 94, 94, new Mass(1, 1));

        // 2�̃p�[�����m�C�Y��g�ݍ��킹��
        // �~�̔��a�Ƀp�[�����m�C�Y�̒l�������Ċ��炩�Ƀ����_���ɂ���
        // TOOD:�e�\���̂̃N���X�Ɋi�[�ł��邩�ǂ�����������
        for(int x = 0; x < terrain.Width; x++)
        {
            for (int z = 0; z < terrain.Depth; z++)
            {
                int centerX = terrain.Width / 2-1;
                int centerZ = terrain.Depth / 2-1;

                float vx = (x + terrain.SeedX) / 10;
                float vz = (z + terrain.SeedZ) / 10;
                float p1 = Mathf.PerlinNoise(vx, vz) * 1.4f;

                float vx2 = (x + terrain.SeedX) / 20;
                float vz2 = (z + terrain.SeedZ) / 20;
                float p2 = Mathf.PerlinNoise(vz2, vx2) * 1.3f;

                if (Mathf.Abs(x - centerX) * Mathf.Abs(x - centerX) + Mathf.Abs(z - centerZ) * Mathf.Abs(z - centerZ) > 1900 * p1)
                {
                    if (Mathf.Abs(x - centerX) * Mathf.Abs(x - centerX) + Mathf.Abs(z - centerZ) * Mathf.Abs(z - centerZ) > 2000 * p2)
                    {
                        terrain.land.Masses[x, z].Num = 0;
                    }
                }
            }
        }

        //terrain.land.SetHeightPN();
        // ���n�̗֊s�̓p�[�����m�C�Y���g������
        // �������ɂ͍��������܂��Ă���K�v������ = ���l�̎��_�Ńp�[�����m�C�Y�ō������蓖�Ă�
        // ���n�Əd�Ȃ��Ă���C�̃}�X���폜����
        terrain.EraseOverlap(terrain.sea, terrain.land);

        // ��������(��)
        Generate(terrain.sea.Masses);
        Generate(terrain.land.Masses);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>�񎟌��z�񂩂�u���b�N�𐶐����ē񎟌��z��Ƃ��ĕԂ�</summary>
    GameObject[,] Generate(Mass[,] array)
    {
        int w = array.GetLength(0);
        int d = array.GetLength(1);

        GameObject[,] gos = new GameObject[w, d];

        for (int x = 0; x < w; x++)
            for (int z = 0; z < d; z++)
            {
                if (_dic.TryGetValue(array[x, z].Num, out Block value))
                {
                    gos[x, z] = Instantiate(value.Go, new Vector3(x, array[x, z].Height, z), Quaternion.identity);
                }
                else
                {
                    Debug.Log("�o�^����Ă��܂���:" + array[x, z].Num);
                }
            }

        return gos;
    }

    /// <summary>�񎟌��z�񂩂�u���b�N�𐶐����ē񎟌��z��Ƃ��ĕԂ�</summary>
    GameObject[,] Generate(int[,] array)
    {
        int d = array.GetLength(0);
        int w = array.GetLength(1);

        GameObject[,] gos = new GameObject[d, w];

        float seedX = Random.Range(0.0f, 100.0f);
        float seedZ = Random.Range(0.0f, 100.0f);

        for (int i = 0; i < d; i++)
            for (int j = 0; j < w; j++)
            {
                if (_dic.TryGetValue(array[i, j],out Block value))
                {
                    float x = (i + seedX)/15;
                    float z = (j + seedZ)/15;
                    float h = Mathf.PerlinNoise(x, z) * 10;
                    h = Mathf.Round(h);
                    Vector3 pos = new Vector3(i, h, j);

                    GameObject go = Instantiate(value.Go, pos, Quaternion.identity);

                    gos[i, j] = go;
                }
                else
                {
                    Debug.LogError("�o�^����Ă��܂���:" + array[i, j]);
                }
            }

        return gos;
    }

    float GetPerline()
    {
        return 0;
    }
}
