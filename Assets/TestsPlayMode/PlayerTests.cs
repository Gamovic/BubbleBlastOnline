using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class PlayerTests
{
    [UnityTest]
    public IEnumerator PlayerMovesLeftAndRight()
    {
        GameObject playerObject = new GameObject();
        Player player = playerObject.AddComponent<Player>();
        player.speed = 4.0f;

        // Test moving left
        float initialXPos = playerObject.transform.position.x;
        player.UpdateInputs(-1f, 0f); // Simulate input to the left
        player.Update(); // Simulate one frame of movement
        yield return new WaitForFixedUpdate(); // Wait for next frame
        float newXPosLeft = playerObject.transform.position.x;

        Assert.Less(newXPosLeft, initialXPos, "Player did not move left.");

        // Test moving right
        player.UpdateInputs(1f, 0f); // Simulate input to the right
        player.Update(); // Simulate one frame of movement
        yield return new WaitForFixedUpdate(); // Wait for next frame
        float newXPosRight = playerObject.transform.position.x;

        Assert.Greater(newXPosRight, newXPosLeft, "Player did not move right.");

        // Clean up
        GameObject.Destroy(playerObject);
    }

    private IEnumerator SimulatePlayerMovement(Player player, float horizontal, float vertical)
    {
        player.UpdateInputs(horizontal, vertical); // Simulate input
        player.Update(); // Simulate player update
        yield return new WaitForFixedUpdate(); // Wait for next frame
    }
}