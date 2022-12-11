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
    public void SetMassRange(int luX, int luZ, int rbX, int rbZ, int num, int height)
    {
        for (int x = luX; x <= rbX; x++)
            for (int z = luZ; z <= rbZ; z++)
                Masses[x, z] = new Mass(num, height, x, z);
    }

    //public Layer Clone()
    //{
    //    return MemberwiseClone() as Layer;
    //}

    /// <summary>
    /// 層をいくつかの区域に分けて管理するための構造体
    /// </summary>
    struct Area
    {
        // 未使用
    }
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

    // ランダム生成のためのシード値
    public readonly float SeedX;
    public readonly float SeedZ;

    public Terrain(int w, int d, float sx, float sz)
    {
        Width = w;
        Depth = d;
        Sea = new Layer(w,d);
        Land = new Layer(w, d);
        SeedX = sx;
        SeedZ = sz;
    }

    /// <summary>2つの層の重なっているマスを消去する</summary>
    public void EraseOverlap(Layer target, Layer overlap)
    {
        for (int x = 0; x < overlap.Width; x++)
            for (int z = 0; z < overlap.Depth; z++)
            {
                if (overlap.Masses[x, z].Num != Utility.Empty)
                    target.Masses[x, z].Num = Utility.Empty;
            }
    }

    /// <summary> パーリンノイズを用いて出した値を返す</summary>
    /// <param name="power">小さいほど強いノイズになる</param>
    /// <param name="range">小さいほど広範囲にノイズをかける</param>
    public float PerlinNoise(float x, float z, float power, float range)
        => Mathf.PerlinNoise((x + SeedX) / power, (z + SeedZ) / power) * range;
}

/// <summary>
/// 便利クラス
/// </summary>
static class Utility
{
    /// <summary>そのマスに何も無いことを示す番号</summary>
    public const int Empty = 0;

    /// <summary>a^2 + b^2 > c を満たすかを返す</summary>
    public static bool CheckSqrt(float a, float b, float c)
    {
        float va = Mathf.Abs(a);
        float vb = Mathf.Abs(b);
        return va * va + vb * vb > c;
    }
}
