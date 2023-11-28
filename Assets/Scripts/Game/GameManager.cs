using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public static GameManager Instance;

    // Add any other game-related variables here
    private int playerScore = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void IncreaseScore(int amount)
    {
        playerScore += amount;

        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        scoreText.text = playerScore.ToString();
    }
}
