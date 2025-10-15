using UnityEngine;
using System.Collections.Generic;

public class InfiniteMapGenerator : MonoBehaviour
{
    public GameObject dirtPrefab;
    public GameObject grassPrefab;
    public GameObject waterPrefab;
    public GameObject coinPrefab;

    public int chunkSize = 16;
    public float noiseScale = 10f;
    public float tileSize = 1f;
    public Transform player;
    public int renderDistance = 3;
    public float coinSpawnChance = 0.1f;

    private Vector2Int currentChunk;
    private Dictionary<Vector2Int, GameObject> loadedChunks = new Dictionary<Vector2Int, GameObject>();

    private float seedX;
    private float seedY;

    void Start()
    {
        seedX = Random.Range(0f, 1000f);
        seedY = Random.Range(0f, 1000f);
        UpdateChunks();
    }

    void Update()
    {
        Vector2Int playerChunk = new Vector2Int(
            Mathf.FloorToInt(player.position.x / (chunkSize * tileSize)),
            Mathf.FloorToInt(player.position.y / (chunkSize * tileSize))
        );

        if (playerChunk != currentChunk)
        {
            currentChunk = playerChunk;
            UpdateChunks();
        }
    }

    void UpdateChunks()
    {
        for (int x = -renderDistance; x <= renderDistance; x++)
        {
            for (int y = -renderDistance; y <= renderDistance; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(currentChunk.x + x, currentChunk.y + y);
                if (!loadedChunks.ContainsKey(chunkCoord))
                {
                    loadedChunks[chunkCoord] = GenerateChunk(chunkCoord);
                }
            }
        }

        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (var chunk in loadedChunks)
        {
            if (Vector2Int.Distance(chunk.Key, currentChunk) > renderDistance + 1)
                toRemove.Add(chunk.Key);
        }

        foreach (var coord in toRemove)
        {
            Destroy(loadedChunks[coord]);
            loadedChunks.Remove(coord);
        }
    }

    GameObject GenerateChunk(Vector2Int chunkCoord)
    {
        GameObject chunkParent = new GameObject($"Chunk_{chunkCoord.x}_{chunkCoord.y}");

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                float worldX = (chunkCoord.x * chunkSize + x) * tileSize;
                float worldY = (chunkCoord.y * chunkSize + y) * tileSize;

                float noiseValue = Mathf.PerlinNoise((worldX + seedX) / noiseScale, (worldY + seedY) / noiseScale);

                GameObject prefabToSpawn;

                // Adjust thresholds to make water less frequent
                if (noiseValue < 0.3f) prefabToSpawn = waterPrefab;    // less water now
                else if (noiseValue < 0.6f) prefabToSpawn = grassPrefab;
                else prefabToSpawn = dirtPrefab;

                GameObject tile = Instantiate(prefabToSpawn, new Vector3(worldX, worldY, 1), Quaternion.identity);
                tile.transform.localScale = Vector3.one * tileSize;
                tile.transform.parent = chunkParent.transform;

                // Add BoxCollider2D to water tiles so player cannot walk on them
                if (prefabToSpawn == waterPrefab)
                {
                    BoxCollider2D collider = tile.GetComponent<BoxCollider2D>();
                    if (collider == null)
                        collider = tile.AddComponent<BoxCollider2D>();
                    collider.isTrigger = false; // solid
                }

                // Spawn coins only on grass tiles
                if (prefabToSpawn == grassPrefab && Random.value < coinSpawnChance)
                {
                    GameObject coin = Instantiate(coinPrefab, new Vector3(worldX, worldY, 0), Quaternion.identity);
                    coin.transform.parent = chunkParent.transform;
                }
            }
        }

        return chunkParent;
    }
}
