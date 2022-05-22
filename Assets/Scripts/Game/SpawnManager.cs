//Manages the Spawnpoints so the Player could spawn in!

using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    private SpawnPoint[] spawnPoints;

    private void Awake()
    {
        Instance = this;
        spawnPoints = GetComponentsInChildren<SpawnPoint>();
    }

    public Transform GetSpawnPoint()
    {
        int random = Random.Range(0, spawnPoints.Length);
        Transform t = spawnPoints[random].transform;
        return t;
    }
}
