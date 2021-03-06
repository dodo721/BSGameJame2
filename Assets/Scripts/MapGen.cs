using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour
{

    public Transform focus;
    public GameObject[] buildings;
    public GameObject[] boats;
    public GameObject water;
    public GameObject[] smallObstacles;
    public GameObject shark;
    public GameObject food;

    public float sharkFoodRadius;
    public float buildingSeed;
    public float boatSeed;
    public float sharkSeed;
    public float smallObstacleSeed;
    public float foodSeed;
    public float buildingNoiseScale;
    public float boatNoiseScale;
    public float sharkNoiseScale;
    public float smallObstacleNoiseScale;
    public float foodNoiseScale;
    [Range(0,1)]
    public float buildingThreshold;
    [Range(0,1)]
    public float boatThreshold;
    [Range(0,1)]
    public float sharkThreshold;
    [Range(0,1)]
    public float smallObstacleThreshold;
    [Range(0,1)]
    public float foodThreshold;
    [Min(1)]
    public float buildingSpacing;
    public float chunkSize;
    public int chunkGenArea;
    public float chunkUnloadDistance;
    public struct Chunk {
        public int x;
        public int y;
        public GameObject gameObject;
        public Chunk(int x, int y, GameObject gameObject) {
            this.x = x;
            this.y = y;
            this.gameObject = gameObject;
        }
    }
    public List<Chunk> generatedChunks = new List<Chunk>();

    public static readonly float NOISE_SCALE_FAC = 1.432321748523f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        int chunkX = Mathf.RoundToInt(focus.position.x / chunkSize);
        int chunkY = Mathf.RoundToInt(focus.position.z / chunkSize);

        foreach (Chunk chunk in generatedChunks) {
            if (Vector3.Distance(chunk.gameObject.transform.position, focus.position) > chunkUnloadDistance) {
                chunk.gameObject.SetActive(false);
            } else {
                chunk.gameObject.SetActive(true);
            }
        }

        bool spawnGen = generatedChunks.Count == 0;

        //Debug.Log("ChunkX: " + chunkX + ", ChumkY: " + chunkY + " (" + focus.position.x + "," + focus.position.z + ")");
        for (int i = -chunkGenArea + chunkX; i < chunkGenArea + chunkX; i++) {
            for (int j = -chunkGenArea + chunkY; j < chunkGenArea + chunkY; j++) {
                if (!ChunkIsGenerated(i, j)) {
                    //Debug.Log("Generating chunk " + i + "," + j);
                    GenerateChunk(i, j, spawnGen);
                }
            }
        }
    }

    bool ChunkIsGenerated (int x, int y) {
        foreach(Chunk chunk in generatedChunks) {
            if (chunk.x == x && chunk.y == y) {
                return true;
            }
        }
        return false;
    }

    void GenerateChunk (int x, int y, bool spawnGen) {
        GameObject parentObject = new GameObject("Chunk " + x + "," + y);
        parentObject.transform.position = new Vector3(x * chunkSize, 0, y * chunkSize);
        generatedChunks.Add(new Chunk(x, y, parentObject));
        for (float i = 0; i < chunkSize; i += buildingSpacing) {
            for (float j = 0; j < chunkSize; j += buildingSpacing) {
                float worldX = (x * chunkSize) + i;
                float worldZ = (y * chunkSize) + j;
                //Debug.Log("NOISE COORDS: " + (((float)x * chunkSize) + i) * noiseScale + ","  + (((float)y * chunkSize) + j) * noiseScale);
                float buildingNoiseVal = NoiseVal(x, y, i, j, buildingNoiseScale, buildingSeed);
                if (buildingNoiseVal > buildingThreshold) {
                    //Debug.Log("Noise val: " + noiseVal);
                    GameObject building = buildings[Random.Range(0, buildings.Length)];
                    GameObject instance = Instantiate(building, new Vector3(worldX, 0, worldZ), Quaternion.Euler( -90, Random.Range( 0, 4 ) * 90, 0 ), parentObject.transform);
                } else {
                    float boatNoiseVal = NoiseVal(x, y, i, j, boatNoiseScale, boatSeed);
                    if (boatNoiseVal > boatThreshold) {
                        GameObject boat = boats[Random.Range(0, boats.Length)];
                        GameObject instance = Instantiate(boat, new Vector3(worldX, 0, worldZ), Quaternion.Euler( 0, Random.Range( 0, 359 ), 0 ), parentObject.transform);
                    } else {
                        float foodNoiseVal = NoiseVal(x, y, i, j, foodNoiseScale, foodSeed);
                        if (foodNoiseVal > foodThreshold) {
                            GameObject instance = Instantiate(food, new Vector3(worldX, 0, worldZ), Quaternion.Euler( 0, Random.Range( 0, 359 ), 0 ), parentObject.transform);
                            if (!spawnGen || Vector3.Distance(focus.position, new Vector3(worldX, 0, worldZ)) > sharkFoodRadius * 2) {
                                int numSharks = Random.Range((int)2, (int)6);
                                for (int k = 0; k < numSharks; k++) {
                                    float sharkX = Random.Range(worldX - sharkFoodRadius, worldX + sharkFoodRadius);
                                    float sharkZ = Random.Range(worldZ - sharkFoodRadius, worldZ + sharkFoodRadius);
                                    Vector3 sharkPos = new Vector3(sharkX, 0, sharkZ);
                                    GameObject sharkInstance = Instantiate(shark, sharkPos, Quaternion.Euler( 0, Random.Range( 0, 359 ), 0 ));
                                }
                            }
                        } else {
                            float smallObstacleNoiseVal = NoiseVal(x, y, i, j, smallObstacleNoiseScale, smallObstacleSeed);
                            if (smallObstacleNoiseVal > smallObstacleThreshold) {
                                GameObject smallObstacle = smallObstacles[Random.Range(0, smallObstacles.Length)];
                                GameObject instance = Instantiate(smallObstacle, new Vector3(worldX, -0.5f, worldZ), Quaternion.Euler( -90, Random.Range( 0, 359 ), 0 ), parentObject.transform);
                            }
                        }
                    }
                }
            }
        }
        Instantiate(water, new Vector3(x * chunkSize, 0, y * chunkSize), Quaternion.identity, parentObject.transform);
    }

    float NoiseVal (int x, int y, float i, float j, float noiseScale, float seed) {
        return Mathf.PerlinNoise(((float)(x * chunkSize) + i) * (noiseScale * NOISE_SCALE_FAC) + seed, (((float)y * chunkSize) + j) * (noiseScale * NOISE_SCALE_FAC) + seed);
    }
}
