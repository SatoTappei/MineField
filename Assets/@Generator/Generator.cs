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

    public static readonly int Empty = 0;
    readonly int Grass = 1;
    readonly int Sea = 2;
    readonly int Beach = 3;
    readonly int Tree = 11;
    readonly int BeachTree = 12;
    readonly int GrassEdge = 50;

    [Header("��������T�C�Y")]
    [SerializeField] int _terrainSize = 100;
    [Header("���̌`�̂��߂�PN�̃f�[�^")]
    [SerializeField] float _p1Power = 10;
    [SerializeField] float _p1Range = 1.4f;
    [SerializeField] float _p2Power = 20;
    [SerializeField] float _p2Range = 1.3f;
    [SerializeField] float _p1Border = 1900;
    [SerializeField] float _p2Border = 2000;
    [Header("���l�̐ݒ�")]
    [SerializeField] float _beachWidth = 150;
    [SerializeField] int _beachWoods = 5;
    [Header("���n�ɐ�������X�̖؂̗�")]
    [SerializeField] float _forestWoods = 5;
    [Header("���፷�p��PN�̃f�[�^")]
    [SerializeField] float _p3Power = 20;
    [SerializeField] float _p3Range = 1;
    [Header("��������u���b�N")]
    [SerializeField] Block[] _blocks;

    Terrain terrain;
    Dictionary<int, Block> _dic = new Dictionary<int, Block>();

    void Awake()
    {
        _dic = _blocks.ToDictionary(b => b.Id, b => b);
    }

    void Start()
    {
        // �V�[�h�l
        float seed = Random.Range(0.0f, 100f);
        // �n�`�N���X�𐶐�
        terrain = new Terrain(_terrainSize, _terrainSize, seed, seed);
        // �e���C���[��C�ӂ̃u���b�N�ƍ����ŏ�����
        terrain.Sea.Init(Sea, 0);
        terrain.Land.Init(Grass, 1);
        terrain.Prop.Init(Empty, 0);

        // ���ɑ΂��đ��삷�邽�߂�n*n�̋��ɕ����ă}�X�̃��X�g���쐬����
        Area[,] areas = new Area[10, 10];
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                areas[i, j] = new Area();

        // ���̃}�X�ɂ̂ݑ�������邽�߂̃��X�g
        List<Mass> grassList = new List<Mass>();

        for (int x = 0; x < terrain.Width; x++)
            for (int z = 0; z < terrain.Depth; z++)
            {
                // 2�̃p�[�����m�C�Y��g�ݍ��킹��
                // �~�̔��a�Ƀp�[�����m�C�Y�̒l�������Ċ��炩�Ƀ����_���ɂ���
                float p1 = terrain.PerlinNoise(x, z, _p1Power, _p1Range);
                float p2 = terrain.PerlinNoise(x, z, _p2Power, _p2Range);

                // ��������̒���(�~�̔��a)�����߂�
                float rx = x - terrain.Width / 2 - 1;
                float rz = z - terrain.Depth / 2 - 1;

                // 2�̃p�[�����m�C�Y�͈̔͊O�ɂ���}�X������
                if (CheckSqrt(rx, rz, _p1Border * p1) &&
                    CheckSqrt(rx, rz, _p2Border * p2))
                {
                    ToEmpty(x, z);
                }
                // ���n�Ȃ��������ł������m�C�Y�Ŕ��肵�č��l�𐶐�����
                else if (terrain.Land.Masses[x, z].Num == Grass &&
                         CheckSqrt(rx, rz, (_p1Border - _beachWidth) * p1) &&
                         CheckSqrt(rx, rz, (_p2Border - _beachWidth) * p2))
                {
                    SetBeach(x, z);
                }
                // ���n�Ȃ獂�፷��M��
                else if (terrain.Land.Masses[x, z].Num == Grass)
                {
                    SetHeight(x, z);

                    // ��ɑ��삷�邽�߂ɑ��̃}�X�̃��X�g�ɒǉ�����
                    grassList.Add(terrain.Land.Masses[x, z]);
                }

                // ��悲�Ƃɑ��삷�邽�߂ɋ��̃��X�g�ɒǉ�����
                areas[x / 10, z / 10].List.Add(terrain.Land.Masses[x, z]);
            }

        SetEdge(grassList);

        // �����_���ȉӏ���X�ɂ���
        int rndX = Random.Range(3, 8);
        int rndZ = Random.Range(3, 8);

        // ���E�̂ǂ��炩�A�㉺�̂ǂ��炩��I������
        int dx = (int)Mathf.Sign(Random.Range(-100, 100));
        int dz = (int)Mathf.Sign(Random.Range(-100, 100));

        SetForest(areas[rndX, rndZ]);
        SetForest(areas[rndX + dx, rndZ]);
        SetForest(areas[rndX, rndZ + dz]);
        SetForest(areas[rndX + dx, rndZ + dz]);

        for(int i = 0; i < 30; i++)
        {
            int gr = Random.Range(0, grassList.Count);
            int pr = Random.Range(21, 26);

            int gx = grassList[gr].X;
            int gz = grassList[gr].Z;

            terrain.Prop.Masses[gx, gz].Num = pr;
            terrain.Prop.Masses[gx, gz].Height = grassList[gr].Height + 1;
        }

        Generate(terrain.Land.Masses);
        Generate(terrain.Prop.Masses);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>�w�肵�����n�̃}�X�̃u���b�N����菜��</summary>
    void ToEmpty(int x, int z) => terrain.Land.Masses[x, z].Num = Empty;

    /// <summary>�w�肵�����n�̍�����ύX����</summary>
    void SetHeight(int x, int z)
    {
        // �������p�[�����m�C�Y���g���ĕύX����
        float p3 = terrain.PerlinNoise(x, z, _p3Power, _p3Range);
        // ����0���ƊC�ʂƓ����ɂȂ��Ă��܂��̂�1�ȏ�ɂ���
        terrain.Land.Masses[x, z].Height = Mathf.Max(1, Mathf.FloorToInt(p3 * 5));
    }

    /// <summary>�w�肵�����n�̃}�X�����l�ɂ���</summary>
    void SetBeach(int x, int z)
    {
        terrain.Land.Masses[x, z].Num = Beach;

        // 5���̊m���ł��̃}�X�ɖ؂𐶐�����
        if (Random.Range(0, 100) < _beachWoods)
        {
            terrain.Prop.Masses[x, z].Num = BeachTree;
            terrain.Prop.Masses[x, z].Height = 1;
        }
    }

    /// <summary>
    /// ������ύX�����Ƃ��ɂ��̃}�X�̉��������Ȃ��悤��
    /// ���l�Ƃ̋��E���̑��n��Ǖt�ɕύX����
    /// </summary>
    void SetEdge(List<Mass> grassList)
    {
        // �}�X�̍�����ς��Ă������ڂ������Ȃ��悤�ɕӏ�̃}�X��Ǖt���ɕύX����
        List<Mass> temp = new List<Mass>(grassList.Where(m => terrain.Land.Masses[m.X - 1, m.Z].Num != Grass ||
                                                              terrain.Land.Masses[m.X + 1, m.Z].Num != Grass ||
                                                              terrain.Land.Masses[m.X, m.Z - 1].Num != Grass ||
                                                              terrain.Land.Masses[m.X, m.Z + 1].Num != Grass));
        temp.ForEach(m => terrain.Land.Masses[m.X, m.Z].Num = GrassEdge);
    }

    /// <summary>���ɐX�𐶐�����</summary>
    void SetForest(Area area)
    {
        // �m���ŗ��n�̃}�X�ɖ؂𐶐�����
        area.List.ForEach(m =>
        {
            int x = terrain.Land.Masses[m.X, m.Z].X;
            int z = terrain.Land.Masses[m.X, m.Z].Z;

            if (Random.Range(0, 100) < _forestWoods && 
                terrain.Land.Masses[x, z].Num == 1)
            {
                terrain.Prop.Masses[x, z].Num = Tree;
                terrain.Prop.Masses[x, z].Height = terrain.Land.Masses[x, z].Height + 1;
            }
        });

        area.IsBuilding = true;
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

    /// <summary>a^2 + b^2 > c �𖞂�������Ԃ�</summary>
    bool CheckSqrt(float a, float b, float c)
    {
        float va = Mathf.Abs(a);
        float vb = Mathf.Abs(b);
        return va * va + vb * vb > c;
    }
}
