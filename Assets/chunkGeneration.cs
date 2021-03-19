using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class chunkGeneration : MonoBehaviour
{
    [Header("Transform of the Chunk")] 
    public Vector3 chunkPosition;
    public int Size = 0;
    public int startCave = 0;
    public int endCave = 0; //also = to startLandscape
    public int endLandscape = 0;


    [Header("Noise Parameters")]
    public float noise3dScale = 0.1f;
    public float noise2dScale = 0.1f;

    //Private variables
    private GameObject Cube;

    private void Start()
    {
        //Load Cube Object
        Cube = Resources.Load("cube") as GameObject;
        
        List<CombineInstance> combine = new List<CombineInstance>();
        
        for (int z = 0; z < Size; z++)
        {
            for (int y = 0; y < endLandscape-startCave; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    float landscapeNoise = (endLandscape - endCave) * Noise2D((x + chunkPosition.x) * noise2dScale, (z + chunkPosition.z) * noise2dScale) + endCave;
                    
                    if (y <= landscapeNoise)
                    {
                        GameObject tempCube = Instantiate(Cube, new Vector3(x, y, z) + chunkPosition, Quaternion.identity);

                        tempCube.transform.parent = transform;
                        // CombineInstance combineInstance = new CombineInstance();
                        // combineInstance.mesh = tempCube.GetComponent<MeshFilter>().mesh;
                        // combineInstance.transform = tempCube.transform.localToWorldMatrix;

                        //combine.Add(combineInstance);
                        //tempCube.SetActive(false);


                    }
                }
            }
        }

        //GetComponent<MeshFilter>().mesh = new Mesh();
        //GetComponent<MeshFilter>().mesh.CombineMeshes(combine.ToArray());
    }

    // private float Noise3D(float x, float y, float z)
    // {
    //     float xy = Noise2D(x, y);
    //     float xz = Noise2D(x, z);
    //     float yz = Noise2D(y, z);
    //     
    //     float yx = Noise2D(y, z);
    //     float zx = Noise2D(z, x);
    //     float zy = Noise2D(z, y);
    //
    //     float xyz = xy + xz + yz + yx + zx + zy;
    //     
    //     return xyz/6f;
    // }

    private float Noise2D(float x, float y)
    {
        float noise = 0f;

        noise = Mathf.PerlinNoise(x, y);
        
        return noise;
    }
}
