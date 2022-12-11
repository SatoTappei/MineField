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
        // 100*100�̃T�C�Y
        Terrain terrain = new Terrain(100, 100, Random.Range(0.0f, 100.0f), Random.Range(0.0f, 100.0f));
        // �C���C���[�ɃT�C�Y���̊C�̃}�X�𐶐�����
        terrain.Sea.SetMassRange(0, 0, terrain.Sea.Width - 1, terrain.Sea.Depth - 1, 2, 0);
        // ���n�𐶐�����
        terrain.Land.SetMassRange(0, 0, terrain.Land.Width - 1, terrain.Land.Depth - 1, 1, 1);

        // TOOD:Layer�\���̂̒���Area�\���̂�����Ă������ŊǗ����邱�Ƃ𗯈ӂ���
        // 10*10�̋��ɕ������邽�߂̍��W�̃��X�g�̓񎟌��z��
        List<Mass>[,] area = new List<Mass>[10, 10];
        // ���W�̃��X�g�̓񎟌��z���������
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                area[i, j] = new List<Mass>();

        // ���̃}�X��"���W"�̃��X�g
        List<Mass> grassList = new List<Mass>();

        // 2�̃p�[�����m�C�Y��g�ݍ��킹��
        // �~�̔��a�Ƀp�[�����m�C�Y�̒l�������Ċ��炩�Ƀ����_���ɂ���
        for (int x = 0; x < terrain.Width; x++)
            for (int z = 0; z < terrain.Depth; z++)
            {
                float p1 = terrain.PerlinNoise(x, z, 10, 1.4f);
                float p2 = terrain.PerlinNoise(x, z, 20, 1.3f);

                int centerX = terrain.Width / 2 - 1;
                int centerZ = terrain.Depth / 2 - 1;

                float rx = x - centerX;
                float rz = z - centerZ;

                // 2�̃p�[�����m�C�Y�͈̔͊O�ɂ���}�X������
                if (Utility.CheckSqrt(rx, rz, 1900 * p1) &&
                    Utility.CheckSqrt(rx, rz, 2000 * p2))
                {
                    terrain.Land.Masses[x, z].Num = 0;
                }
                else if (terrain.Land.Masses[x, z].Num == 1)
                {
                    // ��������ł������m�C�Y�Ŕ��肵�č��l�𐶐�����
                    if (terrain.Land.Masses[x, z].Num == 1 &&
                        Utility.CheckSqrt(rx, rz, 1650 * p1) &&
                        Utility.CheckSqrt(rx, rz, 1750 * p2))
                    {
                        terrain.Land.Masses[x, z].Num = 3;
                    }
                    else
                    {
                        float p3 = terrain.PerlinNoise(x, z, 20, 1);
                        // ����0���ƊC�ʂƓ����ɂȂ��Ă��܂��̂�1�ȏ�ɂ���
                        terrain.Land.Masses[x, z].Height = Mathf.Max(1, Mathf.FloorToInt(p3 * 5));

                        // ��ɑ��삷�邽�߂ɑ��̃}�X�̃��X�g�ɒǉ�����
                        grassList.Add(terrain.Land.Masses[x, z]);
                    }
                }
                // ���W�̃��X�g�̓񎟌��z��ɒǉ�
                area[x / 10, z / 10].Add(terrain.Land.Masses[x, z]);
            }

        // �}�X�̍�����ς��Ă������ڂ������Ȃ��悤�ɕӏ�̃}�X��Ǖt���ɕύX����
        List<Mass> temp = new List<Mass>(grassList.Where(m => terrain.Land.Masses[m.X - 1, m.Z].Num != 1 ||
                                                              terrain.Land.Masses[m.X + 1, m.Z].Num != 1 ||
                                                              terrain.Land.Masses[m.X, m.Z - 1].Num != 1 ||
                                                              terrain.Land.Masses[m.X, m.Z + 1].Num != 1));
        temp.ForEach(m => terrain.Land.Masses[m.X, m.Z].Num = 50);

        // �_�C�A�����h�X�N�G�A�A���S���Y���Ƒg�ݍ��킹��
        // area�̑S���̃}�X���΂̃��X�g

        // 50,50�����S�A���W�����S�ɋ߂���
        foreach(Mass v in grassList)
        {
            //if (Mathf.Abs(50 - v.X) + Mathf.Abs(50 - v.Z) > 10) continue;
            float p2 = terrain.PerlinNoise(v.X, v.Z, 20, 2f);
            if (Utility.CheckSqrt(Mathf.Abs(50 - v.X), Mathf.Abs(50 - v.Z), 200 * p2)) continue;

            //v.Height = (Mathf.Abs(50 - v.X) +  Mathf.Abs(50 - v.Z)) + v.Height;
            v.Height += 20 - Mathf.Abs(50 - v.X) + 20 - Mathf.Abs(50 - v.Z);
        }

        //foreach (var v in area[6, 6])
        //{
        //    v.Num = 99;
        //    v.Height = 5;
        //}
        //foreach (var v in area[5, 5])
        //{
        //    v.Num = 99;
        //    v.Height = 5;
        //}
        //foreach (var v in area[4, 4])
        //{
        //    v.Num = 99;
        //    v.Height = 5;
        //}
        //foreach (var v in area[3, 3])
        //{
        //    v.Num = 99;
        //    v.Height = 5;
        //}

        // ��������(��)
        Generate(terrain.Land.Masses);
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
                    gos[x, z] = Instantiate(value.Go, new Vector3(x, array[x, z].Height, z), Quaternion.identity, transform);
                }
                else
                {
                    Debug.Log("�o�^����Ă��܂���:" + array[x, z].Num);
                }
            }

        return gos;
    }
}
