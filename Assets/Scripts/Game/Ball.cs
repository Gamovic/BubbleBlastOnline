using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Ball : NetworkBehaviour
{
    public Vector2 startForce;

    public GameObject ball;
    public GameObject ball2;
    public GameObject ball3;
    public GameObject ball4;


    public GameObject nextBall;

    public Rigidbody2D rigidb;

    private void Initialize()
    {
        rigidb.AddForce(startForce, ForceMode2D.Impulse);
    }

    public override void OnNetworkSpawn()
    {
        Initialize();

        // Check if the object is spawned on the server
        /*if (IsServer)
        {
            SpawnBallOnClientsServerRpc();
        }*/
    }

    /*[ServerRpc]
    private void SpawnBallOnClientsServerRpc()
    {
        // Spawn the ball on all clients
        SpawnBallClientRpc();
    }*/

    /*[ClientRpc]
    private void SpawnBallClientRpc()
    {
        // Instantiate the ball on the clients
        GameObject ball = Instantiate(gameObject, rigidb.position, Quaternion.identity);

        // Ensure that the spawned ball is a network object
        ball.GetComponent<NetworkObject>().Spawn();
    }*/

    public void Split()
    {
        if (nextBall != null)
        {
            // Get the ball from the pool
        NetworkObjectPool networkObjectPool = NetworkObjectPool.Singleton;
        if (networkObjectPool != null)
        {
            NetworkObject originalBall = NetworkObject;

            GameObject ballGo1 = networkObjectPool.GetNetworkObject(nextBall, rigidb.position + Vector2.right / 4f, Quaternion.identity).gameObject;
            GameObject ballGo2 = networkObjectPool.GetNetworkObject(nextBall, rigidb.position + Vector2.left / 4f, Quaternion.identity).gameObject;
            GameObject ballGo3 = networkObjectPool.GetNetworkObject(nextBall, rigidb.position + Vector2.right / 4f, Quaternion.identity).gameObject;
            GameObject ballGo4 = networkObjectPool.GetNetworkObject(nextBall, rigidb.position + Vector2.left / 4f, Quaternion.identity).gameObject;

            ballGo1.name = nextBall.name;  // Set the name without (Clone)
            ballGo2.name = nextBall.name;  // Set the name without (Clone)
            ballGo3.name = nextBall.name;  // Set the name without (Clone)
            ballGo4.name = nextBall.name;  // Set the name without (Clone)

            ballGo1.GetComponent<Ball>().startForce = new Vector2(2f, 5f);
            ballGo2.GetComponent<Ball>().startForce = new Vector2(-2f, 5f);
            ballGo3.GetComponent<Ball>().startForce = new Vector2(-3f, 5f);
            ballGo4.GetComponent<Ball>().startForce = new Vector2(4f, 5f);

            // Return the original ball to the pool
            networkObjectPool.ReturnNetworkObject(originalBall, nextBall);
            /*networkObjectPool.ReturnNetworkObject(networkObjectPool, nextBall);
            networkObjectPool.ReturnNetworkObject(networkObjectPool, nextBall);
            networkObjectPool.ReturnNetworkObject(networkObjectPool, nextBall);*/


            // Optionally, despawn the original ball
            // NetworkObject.Despawn();
        }
        else
        {
            Debug.LogError("NetworkObjectPool.Singleton is null. Ensure that the pool is properly initialized.");
        }

            /*GameObject ballGo = Instantiate(nextBall, rigidb.position + Vector2.right / 4f, Quaternion.identity);
            GameObject ballGo2 = Instantiate(nextBall, rigidb.position + Vector2.left / 4f, Quaternion.identity);
            GameObject ballGo3 = Instantiate(nextBall, rigidb.position + Vector2.left / 4f, Quaternion.identity);
            GameObject ballGo4 = Instantiate(nextBall, rigidb.position + Vector2.left / 4f, Quaternion.identity);

            ballGo.GetComponent<Ball>().startForce = new Vector2(2f, 6f);
            ballGo2.GetComponent<Ball>().startForce = new Vector2(-2f, 6f);
            ballGo3.GetComponent<Ball>().startForce = new Vector2(-4f, 6f);
            ballGo4.GetComponent<Ball>().startForce = new Vector2(6f, 6f);

            NetworkObjectPool.Singleton.ReturnNetworkObject(NetworkObject, ball);
            NetworkObjectPool.Singleton.ReturnNetworkObject(NetworkObject, ball2);
            NetworkObjectPool.Singleton.ReturnNetworkObject(NetworkObject, ball3);
            NetworkObjectPool.Singleton.ReturnNetworkObject(NetworkObject, ball4);*/
        }


        //NetworkObject.Despawn();
        //Destroy(gameObject);
    }
}
