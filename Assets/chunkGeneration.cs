using System;
using System.Collections.Generic;
using UnityEngine;

public class chunkGeneration : MonoBehaviour
{
    [Header("Transform of the Chunk")]
    public Vector3 center;
    public int Size = 0;
    public int Height = 0;

    [Header("Noise Parameters")]
    public float noiseScale = 0.1f;

    //Private variables
    private GameObject Cube;
    
    
    //re-generating
    private Dictionary<Vector3, GameObject> objects;

    private void Start()
    {
        //Load Cube Object

        Cube = Resources.Load("cube") as GameObject;
        objects = new Dictionary<Vector3, GameObject>();
    }

    private void Update()
    {
        //Plan here is to Instantiate cubes into a chunk

        for (int z = 0; z < Size; z++)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    if (Noise3D(x * noiseScale, y * noiseScale, z * noiseScale) >= 0.5f && !objects.ContainsKey(new Vector3(x, y, z)))
                    {
                        GameObject tempCube = Instantiate(Cube, new Vector3(x, y, z), Quaternion.identity);
                        
                        objects.Add(tempCube.transform.position, tempCube);
                    }

                    if (Noise3D(x * noiseScale, y * noiseScale, z * noiseScale) >= 0.5f &&
                        !objects[new Vector3(x, y, z)].activeInHierarchy)
                    {
                        objects[new Vector3(x, y, z)].SetActive(true);
                    }
                    if (Noise3D(x * noiseScale, y * noiseScale, z * noiseScale) < 0.5f && objects.ContainsKey(new Vector3(x, y, z)))
                    {
                        objects[new Vector3(x, y, z)].SetActive(false);
                    }
                }
            }
        }
    }

    private float Noise3D(float x, float y, float z)
    {
        float xy = Noise2D(x, y);
        float xz = Noise2D(x, z);
        float yz = Noise2D(y, z);
        
        float yx = Noise2D(y, z);
        float zx = Noise2D(z, x);
        float zy = Noise2D(z, y);

        float xyz = xy + xz + yz + yx + zx + zy;
        
        return xyz/6f;
    }

    private float Noise2D(float x, float y)
    {
        float noise = 0f;

        noise = Mathf.PerlinNoise(x, y);
        
        return noise;
    }
}
