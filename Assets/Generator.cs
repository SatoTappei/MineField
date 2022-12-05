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
        for(int x = 0; x < terrain.Width; x++)
            for (int z = 0; z < terrain.Depth; z++)
            {
                float p1 = terrain.PerlinNoise(x, z, 10, 1.4f);
                float p2 = terrain.PerlinNoise(x, z, 20, 1.3f);

                int centerX = terrain.Width / 2 - 1;
                int centerZ = terrain.Depth / 2 - 1;

                float rx = x - centerX;
                float rz = z - centerZ;
                if (Utility.CheckSqrt(rx, rz, 1900 * p1) && Utility.CheckSqrt(rx, rz, 2000 * p2))
                {
                    terrain.land.Masses[x, z].Num = 0;
                }
            }

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
}
