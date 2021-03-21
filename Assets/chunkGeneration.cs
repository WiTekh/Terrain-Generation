using System;
using System.Collections.Generic;
using System.IO;
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

    public float caveNoiseLimit;

    //Private variables
    private GameObject Cube;
    private Dictionary<string, Material> materials;

    private void Awake()
    {
        Material stone = Resources.Load(Path.Combine("Rendering", "stoneMAT")) as Material;
        materials = new Dictionary<string, Material>
        {
            ["stone"] = Resources.Load(Path.Combine("Rendering", "stoneMAT")) as Material,
            ["grass"] = Resources.Load(Path.Combine("Rendering", "grassMAT")) as Material,
            ["dirt"] = Resources.Load(Path.Combine("Rendering", "dirtMAT")) as Material,
            ["clay"] = Resources.Load(Path.Combine("Rendering", "clayMAT")) as Material,
            ["sand"] = Resources.Load(Path.Combine("Rendering", "sandMAT")) as Material,
            ["proto"] = Resources.Load(Path.Combine("Rendering", "protoMAT")) as Material,
        };
        
        Debug.Log(stone);
    }

    private void Start()
    {
        //Load Cube Object
        Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        
        //List<CombineInstance> combine = new List<CombineInstance>();

        float maxCaveNoise = (endCave - startCave) * caveNoiseLimit + startCave;
        for (int z = 0; z < Size; z++)
        {
            for (int y = 0; y < endLandscape-startCave; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    Vector3 cubePosition = new Vector3(x, y, z);
                    float landscapeNoise = (endLandscape - endCave) * Noise2D((x + chunkPosition.x) * noise2dScale, (z + chunkPosition.z) * noise2dScale) + endCave;
                    float caveNoise = (endCave - startCave) * Noise3D((x + chunkPosition.x) * noise3dScale, (y + chunkPosition.y) * noise3dScale, (z + chunkPosition.z) * noise3dScale) + startCave;
                    float caveNoise1 = (endCave - startCave) * Noise3D((x + chunkPosition.x) * noise3dScale, (y + 1 + chunkPosition.y) * noise3dScale, (z + chunkPosition.z) * noise3dScale) + startCave;

                    if (y <= landscapeNoise && caveNoise <= maxCaveNoise)
                    {
                        GameObject tempCube = Instantiate(Cube, cubePosition + chunkPosition, Quaternion.identity);
                        
                        tempCube.GetComponent<MeshRenderer>().material = GetMaterial(y+1, landscapeNoise, caveNoise1);
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

    private Material GetMaterial(float y, float landscapeNoise, float caveNoise)
    {
        string key;
        float maxCaveNoise = (endCave - startCave) * caveNoiseLimit + startCave;

        if (y > landscapeNoise || caveNoise > maxCaveNoise)
        {
            key = "grass";
        }
        else
        {
            key = "dirt";
        }

        return materials[key];
    }
}
