using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectionUI : MonoBehaviour
{
    GameController _gc;
    List<GameObject> instantiatedIcons;
    public List<Sprite> playerIcons;
    public GameObject playerIconPrefab;
    public Transform iconPosition;
    List<Player> players;
    bool gameStarted = false;


    //Keys Control
    public AlphaNumKeys ank;
    List<string> keys;
    KeyCode leftKey;
    KeyCode rightKey;
    KeyCode kc;
    int keysPressed = 0;
    List<string> keysList;
    List<string> usedKeys;
    bool longPressTriggered = false;
    float longPressTimer = 1f;
    bool runTimer = false;
    bool timerRan = false;

    private void Awake()
    {
        players = new List<Player>();
        instantiatedIcons = new List<GameObject>();
        keysList = new List<string>();
        keys = new List<string>(ank.keys);
        usedKeys = new List<string>();
        _gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void Update()
    {
        AnyKeyDown();
        ConfirmPlayer();
        CyclePlayer();

        if (runTimer)
            longPressTimer -= Time.deltaTime;
        else
        {
            longPressTriggered = false;
            longPressTimer = 1f;
        }

        if (longPressTimer <= 0)
        {
            timerRan = true;
            longPressTriggered = true;
            longPressTimer = 1f;
            runTimer = false;
        }
    }

    //Add a new player to the board
    public void AddPlayer()
    {
        Player newPlayer = new Player(leftKey, rightKey, new Vector2Int(10, 10* players.Count));
        players.Add(newPlayer);

        instantiatedIcons.Add(GameObject.Instantiate(playerIconPrefab, iconPosition));

        instantiatedIcons[instantiatedIcons.Count - 1].GetComponent<Image>().sprite = playerIcons[0];
        instantiatedIcons[instantiatedIcons.Count - 1].transform.GetChild(0).GetComponent<Text>().text = "Press "
            + keysList[0] + " or " + keysList[1] + " to change!"; 

    }

    public void CyclePlayer()
    {
        //Cycle through the icons of the possible players
        for(int i = 0; i < usedKeys.Count; i++)
        {
            kc = (KeyCode)System.Enum.Parse(typeof(KeyCode), usedKeys[i]);

            if (Input.GetKeyUp(kc))
            {
                if(i%2 == 0)
                {
                    instantiatedIcons[i / 2].GetComponent<UISpriteManager>().SwapLeft();
                }
                else
                {
                    instantiatedIcons[Mathf.FloorToInt(i / 2)].GetComponent<UISpriteManager>().SwapRight();
                }
            }

        }
    }
    public void ConfirmPlayer()
    {
        //Confirms the players and starts the game
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if((players != null) && (players.Count != 0) && (instantiatedIcons != null) && (instantiatedIcons.Count != 0) && !gameStarted)
            {
                _gc.SetPlayers(players);
                _gc.StartGame(instantiatedIcons);
                gameStarted = true;
            }
        }
    }

    void AnyKeyDown()
    {
        if (keysPressed == 0)
            timerRan = false;

        if (keysPressed == 2 && !timerRan)
            runTimer = true;
        else
            runTimer = false;

        if (longPressTriggered)
        {
            leftKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), keysList[0]);
            rightKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), keysList[1]);


            if(usedKeys.IndexOf(keysList[0]) == -1 && usedKeys.IndexOf(keysList[1]) == -1)
            {
                AddPlayer();
                usedKeys.Add(keysList[0]);
                usedKeys.Add(keysList[1]);
            }

            keysList.Clear();

            runTimer = false;
            longPressTriggered = false;
        }

        foreach (string key in keys)
        {
            kc = (KeyCode)System.Enum.Parse(typeof(KeyCode), key);

            if (Input.GetKeyDown(kc))
            {
                if(keysList.IndexOf(key) == -1)
                    keysList.Add(key);
                keysPressed++;
            }

            if (Input.GetKeyUp(kc))
            {
                keysList.Remove(key);
                keysPressed--;
            }
        }
    }
}
