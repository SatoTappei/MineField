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

    /// <summary>�p�[�����m�C�Y��p���č�����ݒ肷��</summary>
    public void SetHeightPN()
    {
        for (int x = 0; x < Width; x++)
            for (int z = 0; z < Depth; z++)
            {
                float vx = (x+0.008f) / 9;
                float vz = (z+0.008f) / 9;
                float h = Mathf.PerlinNoise(vx, vz) * 10;

                if (Mathf.RoundToInt(h) > 3)
                {
                    Masses[x, z].Height = 1;

                    float vx2 = (x + 43.008f) / 9;
                    float vz2 = (z + 43.008f) / 9;
                    float h2 = Mathf.PerlinNoise(vx2, vz2) * 10;

                    if (Mathf.RoundToInt(h2) > 6)
                    {
                        Masses[x, z].Height = -1000;
                    }
                }
                else
                {
                    Masses[x, z].Height = -1000;
                }

                //Masses[x, z].Height = Mathf.RoundToInt(h);
            }
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
}

/// <summary>
/// �֗��N���X
/// </summary>
static class Utility
{
    /// <summary>���̃}�X�ɉ����������Ƃ������ԍ�</summary>
    public const int Empty = 0;
}
