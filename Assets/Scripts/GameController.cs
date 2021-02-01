using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    GameController _instance;
    public GameObject playerSelectionCanvas;
    public GameObject[] possiblePlayers;
    LevelGrid levelGrid;
    List<Player> players;

    //Keys Control
    KeyCode leftKey;
    KeyCode rightKey;
    KeyCode kc;
    int keysPressed = 0;
    List<string> keysList = new List<string>();
    public bool longPressTriggered = false;
    bool checkInProgress = false;

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
        playerSelectionCanvas.SetActive(true);
        players = new List<Player>();
    }

    private void Update()
    {
        AnyKeyDown();
    }

    //Starts the game
    void StartGame()
    {
        playerSelectionCanvas.SetActive(false);

        levelGrid = new LevelGrid(20, 20);
        levelGrid.Setup(players);
        LevelGrid.OnBlockCaptured += IncreaseScore;
        UpdateScoreUI();
        Time.timeScale = 1f;
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

    void PlayerSelection()
    {
        //players.Add(new Player());
    }

    public void AnyKeyDown()
    {
        if (keysPressed == 2 && !checkInProgress)
        {
            StartCoroutine(CheckLongPress((i) => { longPressTriggered = i; }));
            checkInProgress = true;
        }

        if (longPressTriggered)
        {
            leftKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), keysList[0]);
            rightKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), keysList[1]);

            //INSTANCIAR JOGADOR NO PAINEL DE SELEÇÃO             
        }


        foreach (string key in AlphaNumKeys.keys)
        {
            kc = (KeyCode)System.Enum.Parse(typeof(KeyCode), key);


            if (Input.GetKeyDown(kc))
            {
                keysList.Add(key);
                keysPressed++;
            }

            if (Input.GetKeyUp(kc))
            {
                keysList.Remove(key);
                StopCoroutine("CheckLongPress");
                checkInProgress = false;
                keysPressed--;
            }
        }
    }

    IEnumerator CheckLongPress( System.Action<bool> callback)
    {
        yield return new WaitForSeconds(1f);
        callback(true);
        yield return null;
    }

}
