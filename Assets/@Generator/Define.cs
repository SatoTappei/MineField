using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1�}�X
/// </summary>
struct Mass
{
    public int Num { get; set; }
    public int Height { get; set; }

    public Mass(int n, int h)
    {
        Num = n;
        Height = h;
    }
}

/// <summary>
/// �n�`���\������w
/// </summary>
struct Layer
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
    /// �w�肵���͈͂ɓ����}�X���Z�b�g����<br></br>
    /// �����͔z��̓Y�����Ŏw�肷�邱��
    /// </summary>
    public void SetMassRange(int luX, int luZ, int rbX, int rbZ, Mass m)
    {
        for (int x = luX; x <= rbX; x++)
            for (int z = luZ; z <= rbZ; z++)
                Masses[x, z] = m;
    }
}

/// <summary>
/// �n�`�S��
/// </summary>
struct Terrain
{
    public readonly int Width;
    public readonly int Depth;

    public Layer sea;
    public Layer land;

    // �����_�������̂��߂̃V�[�h�l
    public readonly float SeedX;
    public readonly float SeedZ;

    public Terrain(int w, int d, float sx, float sz)
    {
        Width = w;
        Depth = d;
        sea = new Layer(w,d);
        land = new Layer(w, d);
        SeedX = sx;
        SeedZ = sz;
    }

    /// <summary>2�̑w�̏d�Ȃ��Ă���}�X����������</summary>
    public void EraseOverlap(Layer target, Layer overlap)
    {
        for (int x = 0; x < overlap.Width; x++)
            for (int z = 0; z < overlap.Depth; z++)
            {
                if (overlap.Masses[x, z].Num != Utility.Empty)
                    target.Masses[x, z].Num = Utility.Empty;
            }
    }

    /// <summary> �p�[�����m�C�Y��p���ďo�����l��Ԃ�</summary>
    /// <param name="power">�������قǋ����m�C�Y�ɂȂ�</param>
    /// <param name="range">�������قǍL�͈͂Ƀm�C�Y��������</param>
    public float PerlinNoise(float x, float z, float power, float range)
        => Mathf.PerlinNoise((x + SeedX) / power, (z + SeedZ) / power) * range;
}

/// <summary>
/// �֗��N���X
/// </summary>
static class Utility
{
    /// <summary>���̃}�X�ɉ����������Ƃ������ԍ�</summary>
    public const int Empty = 0;

    /// <summary>a^2 + b^2 > c �𖞂�������Ԃ�</summary>
    public static bool CheckSqrt(float a, float b, float c)
    {
        float va = Mathf.Abs(a);
        float vb = Mathf.Abs(b);
        return va * va + vb * vb > c;
    }
}
