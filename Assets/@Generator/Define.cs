using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1マス
/// </summary>
class Mass
{
    public int Num { get; set; }
    public int Height { get; set; }
    public int X { get; set; }
    public int Z { get; set; }

    public Mass(int n, int h, int x, int z)
    {
        Num = n;
        Height = h;
        X = x;
        Z = z;
    }
}

/// <summary>
/// 層をいくつかの区域に分けて管理するためのクラス
/// </summary>
class Area
{
    public List<Mass> List { get; set; }
    public bool IsBuilding { get; set; }

    public Area()
    {
        List = new List<Mass>();
        IsBuilding = false;
    }
}

/// <summary>
/// 地形を構成する層
/// </summary>
class Layer
{
    public readonly int Width;
    public readonly int Depth;

    public Mass[,] Masses { get; set; }

    public Layer(int w, int d)
    {
        Width = w;
        Depth = d;
        Masses = new Mass[w, d];
    }

    /// <summary>
    /// 指定した範囲に同じマスをセットする<br></br>
    /// 引数は配列の添え字で指定すること
    /// </summary>
    public void Init(int num, int height)
    {
        for (int x = 0; x < Width; x++)
            for (int z = 0; z < Depth; z++)
                Masses[x, z] = new Mass(num, height, x, z);
    }

    //public Layer Clone()
    //{
    //    return MemberwiseClone() as Layer;
    //}
}

/// <summary>
/// 地形全体
/// </summary>
class Terrain
{
    public readonly int Width;
    public readonly int Depth;

    public Layer Sea;
    public Layer Land;
    public Layer Prop;

    // ランダム生成のためのシード値
    public readonly float SeedX;
    public readonly float SeedZ;

    public Terrain(int w, int d, float sx, float sz)
    {
        Width = w;
        Depth = d;
        Sea = new Layer(w,d);
        Land = new Layer(w, d);
        Prop = new Layer(w, d);
        SeedX = sx;
        SeedZ = sz;
    }

    /// <summary>2つの層の重なっているマスを消去する</summary>
    public void EraseOverlap(Layer target, Layer overlap)
    {
        for (int x = 0; x < overlap.Width; x++)
            for (int z = 0; z < overlap.Depth; z++)
            {
                if (overlap.Masses[x, z].Num != Generator.Empty)
                    target.Masses[x, z].Num = Generator.Empty;
            }
    }

    /// <summary> パーリンノイズを用いて出した値を返す</summary>
    /// <param name="power">小さいほど強いノイズになる</param>
    /// <param name="range">小さいほど広範囲にノイズをかける</param>
    public float PerlinNoise(float x, float z, float power, float range)
        => Mathf.PerlinNoise((x + SeedX) / power, (z + SeedZ) / power) * range;
}