using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

/// <summary>
/// パーリンノイズから地形を生成するコンポーネント
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

    [Header("生成するブロック")]
    [SerializeField] Block[] _blocks;

    Dictionary<int, Block> _dic = new Dictionary<int, Block>();

    void Awake()
    {
        _dic = _blocks.ToDictionary(b => b.Id, b => b);
    }

    void Start()
    {
        // 海に島を立てるイメージ


        // 三層構造？にする
        // 各マスの構造体の二次元配列がほしい
        // 島には高低差があり、低いところには湖、高いところは山になる
        // 植物を生やす
        // 建物も立てたい

        // 100*100のサイズ
        Terrain terrain = new Terrain(100, 100, Random.Range(0.0f, 100.0f), Random.Range(0.0f, 100.0f));
        // 海レイヤーにサイズ分の海のマスを生成する
        terrain.sea.SetMassRange(0, 0, terrain.sea.Width - 1, terrain.sea.Depth - 1, new Mass(2,0));
        // 陸地を生成する
        terrain.land.SetMassRange(4, 4, 94, 94, new Mass(1, 1));

        // 2つのパーリンノイズを組み合わせる
        // 円の半径にパーリンノイズの値をかけて滑らかにランダムにする
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

        // 陸地と重なっている海のマスを削除する
        terrain.EraseOverlap(terrain.sea, terrain.land);

        // 生成する(仮)
        Generate(terrain.sea.Masses);
        Generate(terrain.land.Masses);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>二次元配列からブロックを生成して二次元配列として返す</summary>
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
                    Debug.Log("登録されていません:" + array[x, z].Num);
                }
            }

        return gos;
    }
}
