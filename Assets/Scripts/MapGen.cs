using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour
{

    public Transform focus;
    public GameObject[] buildings;
    public GameObject water;
    public GameObject[] signeage;
    public GameObject[] debris;

    public float seed;
    public float noiseScale;
    [Range(0,1)]
    public float buildingThreshold;
    [Min(1)]
    public float buildingSpacing;
    public float chunkSize;
    public int chunkGenArea;
    public struct ChunkCoord {
        public int x;
        public int y;
        public ChunkCoord(int x, int y) {
            this.x = x;
            this.y = y;
        }
    }
    public List<ChunkCoord> generatedChunks = new List<ChunkCoord>();

    // Start is called before the first frame update
    void Start()
    {
        /*
        for (int i = -chunkGenArea; i < chunkGenArea; i++) {
            for (int j = -chunkGenArea; j < chunkGenArea; j++) {
                if (!ChunkIsGenerated(i, j)) {
                    Debug.Log("Generating chunk " + i + "," + j);
                    GenerateChunk(i, j);
                }
            }
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        int chunkX = Mathf.RoundToInt(focus.transform.position.x % chunkSize);
        int chunkY = Mathf.RoundToInt(focus.transform.position.y % chunkSize);
        for (int i = -chunkGenArea + chunkX; i < chunkGenArea + chunkX; i++) {
            for (int j = -chunkGenArea + chunkY; j < chunkGenArea + chunkY; j++) {
                if (!ChunkIsGenerated(i, j)) {
                    Debug.Log("Generating chunk " + i + "," + j);
                    GenerateChunk(i, j);
                }
            }
        }
    }

    bool ChunkIsGenerated (int x, int y) {
        foreach(ChunkCoord chunk in generatedChunks) {
            if (chunk.x == x && chunk.y == y) {
                return true;
            }
        }
        return false;
    }

    void GenerateChunk (int x, int y) {
        generatedChunks.Add(new ChunkCoord(x, y));
        for (float i = 0; i < chunkSize; i += buildingSpacing) {
            for (float j = 0; j < chunkSize; j += buildingSpacing) {
                float worldX = (x * chunkSize) + i;
                float worldZ = (y * chunkSize) + j;
                //Debug.Log("NOISE COORDS: " + (((float)x * chunkSize) + i) * noiseScale + ","  + (((float)y * chunkSize) + j) * noiseScale);
                float noiseVal = Mathf.PerlinNoise(((float)(x * chunkSize) + i) * noiseScale, (((float)y * chunkSize) + j) * noiseScale);
                if (noiseVal > buildingThreshold) {
                    //Debug.Log("Noise val: " + noiseVal);
                    GameObject building = buildings[Random.Range(0, buildings.Length)];
                    GameObject instance = Instantiate(building, new Vector3(worldX, 0, worldZ), Quaternion.Euler( 0, Random.Range( 0, 4 ) * 90, 0 ));
                }
            }
        }
        Instantiate(water, new Vector3(x * chunkSize, 0, y * chunkSize), Quaternion.identity);
    }
}
