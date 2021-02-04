using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    GameController _instance;
    public GameObject playerSelectionCanvas;
    public GameObject gameOverCanvas;
    public List<GameObject> possiblePlayers;
    LevelGrid levelGrid;
    List<Player> players;
    List<GameObject> instantiatedPlayers;
    bool gameStarted = false;

    public List<Player> Players { get { return players; } }

    public delegate void GameStarted();
    public static event GameStarted OnGameStarted;

    public delegate void ScoreIncreased();
    public static event ScoreIncreased OnIncreasedScore;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        instantiatedPlayers = new List<GameObject>();
        playerSelectionCanvas.SetActive(true);
        players = new List<Player>();
        Player.OnPlayerDeath += RemovePlayer;
    }

    private void Update()
    {
        //If all players are dead, ends the game
        if (players.Count == 0 && gameStarted)
        {
            GameOver();
        }
    }

    //Loads the level grid, the players and starts the game
    public void StartGame(List<GameObject> chosenPlayers)
    {
        List<GameObject> chosenPlayersCopy = new List<GameObject>(chosenPlayers);

        //Instantiates the level grid
        levelGrid = new LevelGrid(20 * chosenPlayersCopy.Count, 20 * chosenPlayersCopy.Count);

        //Assign the references
        levelGrid.Setup(players);
        
        //Assigns the events
        LevelGrid.OnBlockCaptured += IncreaseScore;

        //Turn off the player selection canvas
        playerSelectionCanvas.SetActive(false);

        //Sets the background and the camera in accordance with the amount of players on the game
        Vector3 backgroundScale = AssetReference._instance.background.transform.localScale;
        Vector3 backGroundPosition = AssetReference._instance.background.transform.position;

        AssetReference._instance.background.transform.localScale = new Vector3(backgroundScale.x * chosenPlayersCopy.Count, backgroundScale.y * chosenPlayersCopy.Count, 0f);
        AssetReference._instance.mainCamera.orthographicSize = 10 * chosenPlayersCopy.Count;
        AssetReference._instance.mainCamera.transform.position = new Vector3(10 * chosenPlayersCopy.Count, 10 * chosenPlayersCopy.Count, -10f);

        //Assign the variables of the instantiated objects
        for (int i = 0; i < players.Count; i++)
        {
            int index = chosenPlayersCopy[i].GetComponent<UISpriteManager>().spriteIndex;
            instantiatedPlayers.Add(GameObject.Instantiate(possiblePlayers[index]));
            Player instPlayer = instantiatedPlayers[i].GetComponent<Player>();

            instPlayer.LeftKey = players[i].LeftKey;
            instPlayer.RightKey = players[i].RightKey;
            instPlayer.GridPosition = players[i].GridPosition;
            instPlayer.AIControlled = (players[i].AIControlled);
            instPlayer.Setup(levelGrid);
            instPlayer.playerID = i + 1;

            if (instPlayer.AIControlled == true)
                instantiatedPlayers[i].AddComponent<AIPlayer>();
        }

        levelGrid.SpawnBlock();
        OnGameStarted();
        gameStarted = true;
    }

    //Sets the player that were chosen
    public void SetPlayers(List<Player> playersToSet)
    {
        this.players = new List<Player>(playersToSet);
    }

    //Increases the score when a block is caught
    void IncreaseScore(GameObject block, Player player)
    {
        if (block.GetComponent<ENERGY_POWER>())
            player.score += 5;
        else if (block.GetComponent<TIME_TRAVEL>())
            player.score += 10;
        else if (block.GetComponent<BATTERING_RAM>())
            player.score += 15;

        OnIncreasedScore();
    }

    //Removes a player from the game
    void RemovePlayer(Player player)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].LeftKey == player.LeftKey)
                players.RemoveAt(i);
        }

        for(int i = 0; i < instantiatedPlayers.Count; i++)
        {
            if (instantiatedPlayers[i].GetComponent<Player>() == player)
                instantiatedPlayers.RemoveAt(i);
        }
    }

    //Returns the list with the players instantiated in the game
    public List<GameObject> GetInstantiatedPlayers()
    {
        return instantiatedPlayers;
    }

    public LevelGrid getLevelGrid()
    {
        return levelGrid;
    }

    //Finishes the game
    void GameOver()
    {
        gameOverCanvas.SetActive(true);
    }
}
