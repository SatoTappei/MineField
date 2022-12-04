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
        // TOOD:各構造体のクラスに格納できるかどうか検討する
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
        // 陸地の輪郭はパーリンノイズを使いたい
        // 生成時には高さが決まっている必要がある = 数値の時点でパーリンノイズで高さ割り当てる
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

    /// <summary>二次元配列からブロックを生成して二次元配列として返す</summary>
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
                    Debug.LogError("登録されていません:" + array[i, j]);
                }
            }

        return gos;
    }

    float GetPerline()
    {
        return 0;
    }
}
