using System;
using UnityEngine;

public class chunkGeneration : MonoBehaviour
{
    [Header("Transform of the Chunk")]
    public Vector3 center;
    public int Size = 0;
    public int Height = 0;

    private float noiseScale = 0.9f;
    private GameObject Cube;

    private void Start()
    {
        //Load Cube Object

        Cube = Resources.Load("cube") as GameObject;
        
        //Plan here is to Instantiate cubes into a chunk

        for (int z = 0; z < Size; z++)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    if (Perlin3D(x * noiseScale, y * noiseScale, z * noiseScale) >= 0.5f)
                    {
                        GameObject tempCube = Instantiate(Cube, new Vector3(x, y, z), Quaternion.identity);
                    }
                }
            }
        }
        
    }

    private float Perlin3D(float x, float y, float z)
    {
        float xy = Mathf.PerlinNoise(x, y);
        float xz = Mathf.PerlinNoise(x, z);
        float yz = Mathf.PerlinNoise(y, z);
        
        float yx = Mathf.PerlinNoise(y, z);
        float zx = Mathf.PerlinNoise(z, x);
        float zy = Mathf.PerlinNoise(z, y);
        
        return (xy + xz + yz + yx + zx + zy )/6f;
    }
}
