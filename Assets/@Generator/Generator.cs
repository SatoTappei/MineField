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
        // 100*100のサイズ
        Terrain terrain = new Terrain(100, 100, Random.Range(0.0f, 100.0f), Random.Range(0.0f, 100.0f));
        // 海レイヤーにサイズ分の海のマスを生成する
        terrain.Sea.SetMassRange(0, 0, terrain.Sea.Width - 1, terrain.Sea.Depth - 1, 2, 0);
        // 陸地を生成する
        terrain.Land.SetMassRange(0, 0, terrain.Land.Width - 1, terrain.Land.Depth - 1, 1, 1);

        // TOOD:Layer構造体の中にArea構造体を作ってそっちで管理することを留意する
        // 10*10の区域に分割するための座標のリストの二次元配列
        List<(int x, int z)>[,] area = new List<(int x, int z)>[10, 10];
        // 座標のリストの二次元配列を初期化
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                area[i, j] = new List<(int x, int z)>();

        // ★TODO
        // landを複製する
        // 1段下にずらす
        // 4方向にブロックがある場合は削除する
        // 残ったブロックを海抜まで複製していく
        //Layer dupe = new Layer(100, 100);
        //dupe.SetMassRange(4, 4, 94, 94, new Mass(0, 0));

        // 2つのパーリンノイズを組み合わせる
        // 円の半径にパーリンノイズの値をかけて滑らかにランダムにする
        for(int x = 0; x < terrain.Width; x++)
            for (int z = 0; z < terrain.Depth; z++)
            {
                Debug.Log(terrain.Land.Masses[x, z].Num);
                float p1 = terrain.PerlinNoise(x, z, 10, 1.4f);
                float p2 = terrain.PerlinNoise(x, z, 20, 1.3f);

                int centerX = terrain.Width / 2 - 1;
                int centerZ = terrain.Depth / 2 - 1;

                float rx = x - centerX;
                float rz = z - centerZ;

                // 2つのパーリンノイズの範囲外にあるマスを消す
                if (Utility.CheckSqrt(rx, rz, 1900 * p1) &&
                    Utility.CheckSqrt(rx, rz, 2000 * p2))
                {
                    terrain.Land.Masses[x, z].Num = 0;
                }
                else if (terrain.Land.Masses[x, z].Num == 1)
                {
                    // 一回り内側でも同じノイズで判定して砂浜を生成する
                    if (terrain.Land.Masses[x, z].Num == 1 &&
                        Utility.CheckSqrt(rx, rz, 1650 * p1) &&
                        Utility.CheckSqrt(rx, rz, 1750 * p2))
                    {
                        terrain.Land.Masses[x, z].Num = 3;
                    }
                    else
                    {
                        float p3 = terrain.PerlinNoise(x, z, 20, 1);
                        // 高さ0だと海面と同じになってしまうので1以上にする
                        terrain.Land.Masses[x, z].Height = Mathf.Max(1, Mathf.FloorToInt(p3 * 5));
                    }


                    // 周囲4マスを確認、砂浜もしくは海があればリストに追加
                    //if (terrain.land.Masses[x - 1, z].Num != 1 ||
                    //    terrain.land.Masses[x + 1, z].Num != 1 ||
                    //    terrain.land.Masses[x, z - 1].Num != 1 ||
                    //    terrain.land.Masses[x, z + 1].Num != 1)
                    //{
                    //    // 真下に生成 テスト
                    //    dupe.Masses[x, z].Num = 99;
                    //    dupe.Masses[x, z].Height = terrain.land.Masses[x, z].Height - 1;
                    //}
                    
                    // 3回複製すればいい
                }

                // 座標のリストの二次元配列に追加
                area[x / 10, z / 10].Add((x, z));
            }

        // エリア単位で弄る
        //foreach ((int x, int z) pair in area[9, 9])
        //{
        //    terrain.land.Masses[pair.x, pair.z].Num = 99;
        //}

        // 陸地と重なっている海のマスを削除する
        //terrain.EraseOverlap(terrain.sea, terrain.land);

        // 生成する(仮)
        //Generate(terrain.sea.Masses);
        Generate(terrain.Land.Masses);
        //Generate(dupe.Masses);
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
                    gos[x, z] = Instantiate(value.Go, new Vector3(x, array[x, z].Height, z), Quaternion.identity, transform);
                }
                else
                {
                    Debug.Log("登録されていません:" + array[x, z].Num);
                }
            }

        return gos;
    }
}
