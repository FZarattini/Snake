using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    GameController _instance;
    LevelGrid levelGrid;
    Player player;
    float score;

    private void Awake()
    {
        _instance = this;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void Start()
    {
        levelGrid = new LevelGrid(20, 20);
        levelGrid.Setup(player);
        LevelGrid.OnBlockCaptured += IncreaseScore;
        UpdateScoreUI();
    }

    void IncreaseScore(GameObject block)
    {
        if (block.GetComponent<ENERGY_POWER>())
            score += 5;
        else if (block.GetComponent<TIME_TRAVEL>())
            score += 10;
        else if (block.GetComponent<BATTERING_RAM>())
            score += 15;

        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        AssetReference._instance.scoreText.text = "Score: " + score.ToString();
    }
}
