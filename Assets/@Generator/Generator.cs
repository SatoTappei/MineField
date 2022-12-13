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

    public static readonly int Empty = 0;
    readonly int Grass = 1;
    readonly int Sea = 2;
    readonly int Beach = 3;
    readonly int Tree = 11;
    readonly int BeachTree = 12;
    readonly int GrassEdge = 50;

    [Header("生成するサイズ")]
    [SerializeField] int _terrainSize = 100;
    [Header("島の形のためのPNのデータ")]
    [SerializeField] float _p1Power = 10;
    [SerializeField] float _p1Range = 1.4f;
    [SerializeField] float _p2Power = 20;
    [SerializeField] float _p2Range = 1.3f;
    [SerializeField] float _p1Border = 1900;
    [SerializeField] float _p2Border = 2000;
    [Header("砂浜の設定")]
    [SerializeField] float _beachWidth = 150;
    [SerializeField] int _beachWoods = 5;
    [Header("草地に生成する森の木の量")]
    [SerializeField] float _forestWoods = 5;
    [Header("高低差用のPNのデータ")]
    [SerializeField] float _p3Power = 20;
    [SerializeField] float _p3Range = 1;
    [Header("生成するブロック")]
    [SerializeField] Block[] _blocks;

    Terrain terrain;
    Dictionary<int, Block> _dic = new Dictionary<int, Block>();

    void Awake()
    {
        _dic = _blocks.ToDictionary(b => b.Id, b => b);
    }

    void Start()
    {
        // シード値
        float seed = Random.Range(0.0f, 100f);
        // 地形クラスを生成
        terrain = new Terrain(_terrainSize, _terrainSize, seed, seed);
        // 各レイヤーを任意のブロックと高さで初期化
        terrain.Sea.Init(Sea, 0);
        terrain.Land.Init(Grass, 1);
        terrain.Prop.Init(Empty, 0);

        // 区域に対して操作するためにn*nの区域に分けてマスのリストを作成する
        Area[,] areas = new Area[10, 10];
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                areas[i, j] = new Area();

        // 草のマスにのみ操作をするためのリスト
        List<Mass> grassList = new List<Mass>();

        for (int x = 0; x < terrain.Width; x++)
            for (int z = 0; z < terrain.Depth; z++)
            {
                // 2つのパーリンノイズを組み合わせる
                // 円の半径にパーリンノイズの値をかけて滑らかにランダムにする
                float p1 = terrain.PerlinNoise(x, z, _p1Power, _p1Range);
                float p2 = terrain.PerlinNoise(x, z, _p2Power, _p2Range);

                // 中央からの長さ(円の半径)を求める
                float rx = x - terrain.Width / 2 - 1;
                float rz = z - terrain.Depth / 2 - 1;

                // 2つのパーリンノイズの範囲外にあるマスを消す
                if (CheckSqrt(rx, rz, _p1Border * p1) &&
                    CheckSqrt(rx, rz, _p2Border * p2))
                {
                    ToEmpty(x, z);
                }
                // 陸地なら一回り内側でも同じノイズで判定して砂浜を生成する
                else if (terrain.Land.Masses[x, z].Num == Grass &&
                         CheckSqrt(rx, rz, (_p1Border - _beachWidth) * p1) &&
                         CheckSqrt(rx, rz, (_p2Border - _beachWidth) * p2))
                {
                    SetBeach(x, z);
                }
                // 草地なら高低差を弄る
                else if (terrain.Land.Masses[x, z].Num == Grass)
                {
                    SetHeight(x, z);

                    // 後に操作するために草のマスのリストに追加する
                    grassList.Add(terrain.Land.Masses[x, z]);
                }

                // 区域ごとに操作するために区域のリストに追加する
                areas[x / 10, z / 10].List.Add(terrain.Land.Masses[x, z]);
            }

        SetEdge(grassList);

        // ランダムな箇所を森にする
        int rndX = Random.Range(3, 8);
        int rndZ = Random.Range(3, 8);

        // 左右のどちらか、上下のどちらかを選択する
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

    /// <summary>指定した陸地のマスのブロックを取り除く</summary>
    void ToEmpty(int x, int z) => terrain.Land.Masses[x, z].Num = Empty;

    /// <summary>指定した陸地の高さを変更する</summary>
    void SetHeight(int x, int z)
    {
        // 高さをパーリンノイズを使って変更する
        float p3 = terrain.PerlinNoise(x, z, _p3Power, _p3Range);
        // 高さ0だと海面と同じになってしまうので1以上にする
        terrain.Land.Masses[x, z].Height = Mathf.Max(1, Mathf.FloorToInt(p3 * 5));
    }

    /// <summary>指定した陸地のマスを砂浜にする</summary>
    void SetBeach(int x, int z)
    {
        terrain.Land.Masses[x, z].Num = Beach;

        // 5％の確率でそのマスに木を生成する
        if (Random.Range(0, 100) < _beachWoods)
        {
            terrain.Prop.Masses[x, z].Num = BeachTree;
            terrain.Prop.Masses[x, z].Height = 1;
        }
    }

    /// <summary>
    /// 高さを変更したときにそのマスの下が欠けないように
    /// 砂浜との境界線の草地を壁付に変更する
    /// </summary>
    void SetEdge(List<Mass> grassList)
    {
        // マスの高さを変えても見た目が欠けないように辺上のマスを壁付きに変更する
        List<Mass> temp = new List<Mass>(grassList.Where(m => terrain.Land.Masses[m.X - 1, m.Z].Num != Grass ||
                                                              terrain.Land.Masses[m.X + 1, m.Z].Num != Grass ||
                                                              terrain.Land.Masses[m.X, m.Z - 1].Num != Grass ||
                                                              terrain.Land.Masses[m.X, m.Z + 1].Num != Grass));
        temp.ForEach(m => terrain.Land.Masses[m.X, m.Z].Num = GrassEdge);
    }

    /// <summary>区域に森を生成する</summary>
    void SetForest(Area area)
    {
        // 確率で陸地のマスに木を生成する
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

    /// <summary>a^2 + b^2 > c を満たすかを返す</summary>
    bool CheckSqrt(float a, float b, float c)
    {
        float va = Mathf.Abs(a);
        float vb = Mathf.Abs(b);
        return va * va + vb * vb > c;
    }
}
