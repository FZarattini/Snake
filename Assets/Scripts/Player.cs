using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //The initial blocks of the player
    public List<GameObject> _initialBlocks;

    //Grid movement variables
    Vector2Int gridMoveDirection;
    Vector2Int gridPosition;
    float gridMoveTimer;
    float gridMoveTimerMax;
    public float playerSpeed;

    //Player specific variables
    int playerSize;
    bool canMove = true;
    List<Vector2Int> playerPositionList;
    List<GameObject> playerBodyBlocks;
    public bool canRam;
    public float slowPlayerFactor;
    public float moveDelay;
    // Equals the amount of TIME_TRAVEL blocks the player has
    int continues = 0;

    //Event called everytime the player moves
    public delegate void PlayerMoved(Vector2Int gridPosition);
    public static event PlayerMoved OnPlayerMoved;

    private void Awake()
    {
        //Initialization
        gridMoveDirection = new Vector2Int(1, 0);
        gridPosition = new Vector2Int(10, 10);
        playerPositionList = new List<Vector2Int>();
        playerBodyBlocks = new List<GameObject>();
      
        gridMoveTimerMax = 1;
        gridMoveTimer = gridMoveTimerMax;
    }

    private void Start()
    {
        //Instantiate the initial blocks of the chosen player
        for(int i = 0; i < 3; i++)
        {
            _initialBlocks[i] = GameObject.Instantiate(_initialBlocks[i]);
            GrowPlayer(_initialBlocks[i]);
        }

        //Insert the initial body blocks of the snake (except the head)
        playerPositionList.Insert(0, gridPosition);
        playerPositionList.Insert(0, gridPosition);

        LevelGrid.OnBlockCaptured += GrowPlayer;
        LevelGrid.OnBlockCaptured += SlowPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        MovePlayer();   
    }

    //Listens to the input and calls the appropriate methods
    void HandleInput()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow) && canMove)
        {
            UpdatePlayerDirection(-gridMoveDirection.y, gridMoveDirection.x);
            StartCoroutine("InputDelay");
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && canMove)
        {
            UpdatePlayerDirection(gridMoveDirection.y, -gridMoveDirection.x);
            StartCoroutine("InputDelay");
        }
    }

    //Changes the moving direction of the player
    void UpdatePlayerDirection(int xValue, int yValue)
    {
        gridMoveDirection.x = xValue;
        gridMoveDirection.y = yValue;
    }

    //Moves the player on the right direction
    void MovePlayer()
    {
        //Speed the player moves
        gridMoveTimer += Time.deltaTime * playerSpeed;

        if (gridMoveTimer >= gridMoveTimerMax)
        {
            gridMoveTimer -= gridMoveTimerMax;

            //Inserts a new position on the player's path
            playerPositionList.Insert(0, gridPosition);

            //Moves the player o the appropriate direction
            gridPosition += gridMoveDirection;

            //Makes the body follow the head
            if(playerPositionList.Count >= playerSize + 1)
            {
                playerPositionList.RemoveAt(playerPositionList.Count - 1);
            }

            //Moves the head and rotates it (In case of more complex sprites)
            transform.position = new Vector3(gridPosition.x, gridPosition.y);
            RotatePlayer(gridMoveDirection);

            //Moves the body
            for(int i = 0; i < playerBodyBlocks.Count; i++)
            {
                Vector3 playerBodyPosition = new Vector3(playerPositionList[i].x, playerPositionList[i].y);
                playerBodyBlocks[i].transform.position = playerBodyPosition;
            }

            //Kills the player if it touches itself
            for (int i = 0; i < playerBodyBlocks.Count; i++)
            {
                if((gridPosition.x == playerBodyBlocks[i].transform.position.x) && (gridPosition.y == playerBodyBlocks[i].transform.position.y)) { Die(); }
               
            }
        }
        OnPlayerMoved(gridPosition);
    }

    //Rotates the player
    void RotatePlayer(Vector2Int dir)
    {
        transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(dir) - 90);
    }

    //Returns the angle corresponding the direction the player is supposed to be facing
    private float GetAngleFromVector(Vector2Int dir)
    {
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) 
            n += 360;
        return n;
    }

    //Returns the grid position of the player
    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }

    //Returns the full list of position occupied by the player (all blocks)
    public List<Vector2Int> GetFullPlayerGridPosition()
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { gridPosition };
        gridPositionList.AddRange(playerPositionList);
        return playerPositionList;
    }

    //Grows the player when a block is captured
    void GrowPlayer(GameObject block)
    {
        playerBodyBlocks.Insert(0, block);
        playerSize++;
        block.GetComponent<Block>().Ability(this);
    }


    //Slows the player by a factor
    void SlowPlayer(GameObject block)
    {
        playerSpeed -= slowPlayerFactor;
    }


    //Kills the player
    void Die()
    {
        if(continues > 0)
        {
            Vector2Int auxPosition = new Vector2Int();
            LoadLastCheckpoint();
            int i;
            for(i = 0; i < playerBodyBlocks.Count; i++) {
                if (playerBodyBlocks[i].gameObject.name == "TIME_TRAVEL(Clone)")
                {
                    playerBodyBlocks.Remove(playerBodyBlocks[i]);
                    if(i != playerBodyBlocks.Count - 1)
                    {
                        auxPosition = playerPositionList[i];                      
                    }
                    playerPositionList.Remove(playerPositionList[i]);
                    break;
                }
            }

            playerPositionList[i] = auxPosition;

            for(i = i + 1; i < playerPositionList.Count; i++)
            {
                playerPositionList[i] = playerPositionList[i + 1];
            }
            continues--;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Ram()
    {

    }

    //Saves the position of the player
    public void SaveCheckpoint()
    {
        SaveLoadState.SaveState(playerPositionList, gridPosition);
        continues++;
    }

    //Loads the last position of the player
    void LoadLastCheckpoint()
    {

    }

    IEnumerator InputDelay()
    {
        canMove = false;
        yield return new WaitForSeconds(moveDelay * gridMoveTimerMax);
        canMove = true;
    }
}
