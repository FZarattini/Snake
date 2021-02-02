using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    GameController _instance;
    public GameObject playerSelectionCanvas;
    public List<GameObject> possiblePlayers;
    LevelGrid levelGrid;
    List<Player> players;
    List<GameObject> instantiatedPlayers;

    //Score
    float score;

    private void Awake()
    {
        //Time.timeScale = 0f;
        _instance = this;
        //players.Add(GameObject.FindGameObjectWithTag("Player").GetComponent<Player>());
    }

    private void Start()
    {
        instantiatedPlayers = new List<GameObject>(0);
        playerSelectionCanvas.SetActive(true);
        players = new List<Player>();
    }

    //Starts the game
    public void StartGame(List<GameObject> chosenPlayers)
    {
        List<GameObject> chosenPlayersCopy = new List<GameObject>(chosenPlayers);

        levelGrid = new LevelGrid(20, 20);
        
        levelGrid.Setup(players);

        LevelGrid.OnBlockCaptured += IncreaseScore;

        playerSelectionCanvas.SetActive(false);
        
        for (int i = 0; i < players.Count; i++)
        {
            int index = chosenPlayersCopy[i].GetComponent<UISpriteManager>().spriteIndex;
            instantiatedPlayers.Add(GameObject.Instantiate(possiblePlayers[index]));
           
            instantiatedPlayers[i].GetComponent<Player>().LeftKey = players[i].LeftKey;
            instantiatedPlayers[i].GetComponent<Player>().RightKey = players[i].RightKey;
            instantiatedPlayers[i].GetComponent<Player>().SetGridPosition(players[i].GetGridPosition()) ;
        }

        UpdateScoreUI();
    }

    public void SetPlayers(List<Player> playersToSet)
    {
        this.players = new List<Player>(playersToSet);
    }

    void IncreaseScore(GameObject block, Player player)
    {
        int index;

        if ((index = players.IndexOf(player)) != -1) { 
            if (block.GetComponent<ENERGY_POWER>())
                players[index].score += 5;
            else if (block.GetComponent<TIME_TRAVEL>())
                players[index].score += 10;
            else if (block.GetComponent<BATTERING_RAM>())
                players[index].score += 15;
        }

        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        AssetReference._instance.scoreText.text = "Score: " + score.ToString();
    }
}
