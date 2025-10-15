using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab;
    public int coinCount = 20;
    public float spawnRadius = 25f;

    void Start()
    {
        for (int i = 0; i < coinCount; i++)
        {
            Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
            Instantiate(coinPrefab, new Vector3(randomPos.x, randomPos.y, 0), Quaternion.identity);
        }
    }
}
