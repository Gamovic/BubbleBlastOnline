using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject ballPrefab;

    [SerializeField]
    private GameObject ballPrefab2;

    [SerializeField]
    private GameObject ballPrefab3;

    [SerializeField]
    private GameObject ballPrefab4;

    private const int MaxPrefabCount = 100;

    private bool isSpawning;

    void Start()
    {
        NetworkManager.Singleton.OnServerStarted += SpawnBallStart;
    }

    private void SpawnBallStart()
    {
        NetworkManager.Singleton.OnServerStarted -= SpawnBallStart;

        if (NetworkObjectPool.Singleton != null)
        {
            isSpawning = true;

            NetworkObjectPool.Singleton.OnNetworkSpawn();

            for (int i = 0; i < 7; ++i)
            {
                SpawnBall();
            }

            StartCoroutine(SpawnOverTime());
        }
        else
        {
            Debug.LogError("NetworkObjectPool.Singleton is null. Ensure that the pool is properly initialized.");
        }

        /*NetworkObjectPool.Singleton.OnNetworkSpawn();

        for (int i = 0; i < 7; ++i)
        {
            SpawnBall();
        }

        StartCoroutine(SpawnOverTime());*/
    }

    private void SpawnBall()
    {
        NetworkObject poolObj = NetworkObjectPool.Singleton.GetNetworkObject(ballPrefab, GetRandomPositionOnMap(), Quaternion.identity);
        NetworkObject poolObj2 = NetworkObjectPool.Singleton.GetNetworkObject(ballPrefab2, GetRandomPositionOnMap(), Quaternion.identity);
        NetworkObject poolObj3 = NetworkObjectPool.Singleton.GetNetworkObject(ballPrefab3, GetRandomPositionOnMap(), Quaternion.identity);
        NetworkObject poolObj4 = NetworkObjectPool.Singleton.GetNetworkObject(ballPrefab4, GetRandomPositionOnMap(), Quaternion.identity);

        if (poolObj != null)
        {
            poolObj.GetComponent<Ball>().ball = ballPrefab;

            poolObj.Spawn(true);
        }
        else if (poolObj2 != null)
        {
            poolObj.GetComponent<Ball>().ball2 = ballPrefab;
            poolObj2.Spawn(true);
        }
        else if (poolObj3 != null)
        {
            poolObj.GetComponent<Ball>().ball3 = ballPrefab;
            poolObj3.Spawn(true);
        }
        else if (poolObj4 != null)
        {
            poolObj.GetComponent<Ball>().ball4 = ballPrefab;
            poolObj4.Spawn(true);
        }

    }

    private Vector3 GetRandomPositionOnMap()
    {
        return new Vector3(Random.Range(-6f, 6f), Random.Range(-3f, 3f), 0f);
    }

    private IEnumerator SpawnOverTime()
    {
        while (NetworkManager.Singleton.ConnectedClients.Count > 0 && isSpawning)
        {
            yield return new WaitForSeconds(2f);

            //if (NetworkObjectPool.Singleton.GetCurrentPrefabCount(ballPrefab) < MaxPrefabCount)
                SpawnBall();
        }
    }
}
