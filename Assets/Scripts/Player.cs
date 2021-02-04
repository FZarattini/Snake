using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    //Setup references
    LevelGrid levelGrid;
    SaveLoadState saveLoad;

    //The initial blocks of the player
    public List<GameObject> _initialBlocks;

    //Grid movement variables
    Vector2Int gridMoveDirection;
    Vector2Int gridPosition;
    float gridMoveTimer;
    float gridMoveTimerMax;
    float inputDelay;
    public float moveDelay;
    public float playerSpeed;
    public KeyCode LeftKey;
    public KeyCode RightKey;
    bool canMove;

    //Player specific variables
    bool aiControlled;
    public int playerID;
    int playerSize;
    bool canInput = true;
    public float score = 0f;
    public List<Vector2Int> playerPositionList;
    public List<GameObject> playerBodyBlocks;
    public float slowPlayerFactor;

    //Amount of BATTERING_RAMS the player can use on another player
    public float rams;
    
    // == 1 if player has unused TIME_TRAVEL block
    // == 0 if player has no TIME_TRAVEL block or already used the latest one
    public int continues = 0;

    //Event called everytime the player moves
    public delegate void PlayerMoved(Vector2Int gridPosition, Player player);
    public static event PlayerMoved OnPlayerMoved;

    public delegate void PlayerDied(Player player);
    public static event PlayerDied OnPlayerDeath;

    public bool AIControlled { get { return aiControlled; } set { aiControlled = value; } }

    public Vector2Int GridPosition { get { return gridPosition; } set { gridPosition = value; } }

    public List<GameObject> PlayerBodyBlocks { get { return playerBodyBlocks; } }

    public Vector2Int MoveGridDirection { get { return gridMoveDirection; } }


    //Player constructor
    public Player(KeyCode LeftKey, KeyCode RightKey, Vector2Int gridPosition, bool aiControlled)
    {
        this.LeftKey = LeftKey;
        this.RightKey = RightKey;
        this.gridPosition = gridPosition;
        this.aiControlled = aiControlled;
    }

    private void Awake()
    {
        //Initialization
        gridMoveDirection = new Vector2Int(0, 1);
        playerPositionList = new List<Vector2Int>();
        playerBodyBlocks = new List<GameObject>();
        saveLoad = gameObject.GetComponent<SaveLoadState>();
      
        gridMoveTimerMax = 1;
        gridMoveTimer = gridMoveTimerMax;
        inputDelay = moveDelay;
    }

    private void Start()
    {

        //Instantiate the initial blocks of the chosen player
        for(int i = 0; i < 3; i++)
        {
            _initialBlocks[i] = GameObject.Instantiate(_initialBlocks[i]);
            GrowPlayer(_initialBlocks[i], this);
        }

        LevelGrid.OnBlockCaptured += GrowPlayer;
        LevelGrid.OnBlockCaptured += SlowPlayer;

        SaveCheckpoint();
    }

    // Update is called once per frame
    void Update()
    {
        if(!aiControlled)
            HandleInput();

        MovePlayer();

        //Delay between inputs
        if (!canInput)
            inputDelay -= Time.deltaTime;

        if(inputDelay <= 0f)
        {
            canInput = true;
            inputDelay = moveDelay;
        }

    }

    //References the level grid
    public void Setup(LevelGrid lg)
    {
        this.levelGrid = lg;
    }

    //Listens to the input and calls the appropriate methods
    void HandleInput()
    {
        if (canInput)
        {
            if(Input.GetKeyDown(LeftKey))
            {
                UpdatePlayerDirection(-gridMoveDirection.y, gridMoveDirection.x);
                canInput = false;
            }

            if (Input.GetKeyDown(RightKey))
            {
                UpdatePlayerDirection(gridMoveDirection.y, -gridMoveDirection.x);
                canInput = false;
            }
        }
    }

    //Changes the moving direction of the player
    public void UpdatePlayerDirection(int xValue, int yValue)
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

            //Moves the player on the appropriate direction
            gridPosition += gridMoveDirection;

            //Wraps the player on the level grid
            gridPosition = levelGrid.ValidateGridPosition(gridPosition);

            //Removes from the end of the position list
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
                if((gridPosition.x == playerBodyBlocks[i].transform.position.x) && (gridPosition.y == playerBodyBlocks[i].transform.position.y)) 
                    Die();         
            }          
        }

        OnPlayerMoved(gridPosition, this);
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

    //Returns the full list of position occupied by the player (all blocks)
    public List<Vector2Int> GetFullPlayerGridPosition()
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { gridPosition };
        gridPositionList.AddRange(playerPositionList);
        return gridPositionList;
    }

    //Grows the player when a block is captured
    void GrowPlayer(GameObject block, Player player)
    {
        if(player == this)
        {
            playerBodyBlocks.Insert(0, block);
            playerPositionList.Insert(0, gridPosition);
            playerSize++;
            block.GetComponent<Block>().Ability(this);
        }
    }

    //Slows the player by a factor
    void SlowPlayer(GameObject block, Player player)
    {
        if(player == this)
            playerSpeed -= slowPlayerFactor;
    }

    //Kills the player handling TIME_TRAVEL situation
    public void Die()
    {
        //If player has continues
        if(continues > 0)
        {
            LoadLastCheckpoint();

            //playerPositionList.Insert(0, gridPosition);

            for(int i = 0; i < playerBodyBlocks.Count; i++)
            {
                playerBodyBlocks[i].transform.position = new Vector3(playerPositionList[i].x, playerPositionList[i].y, 0f);
            }

            continues = 0;
            
        }
        //if player has no continues, simply delete all of its parts
        else
        {
            for(int i = 0; i < playerBodyBlocks.Count; i++)
            {
                DestroyImmediate(playerBodyBlocks[i]);
            }
            playerBodyBlocks.Clear();
            playerPositionList.Clear();
            OnPlayerDeath(this);
            DestroyImmediate(this.gameObject);
        }
    }

    //Ram ability. If the player has rams available, it tries breaks the other player from the block it hits to the end and keeps on going
    public void Ram(Player otherPlayer, Vector2Int hitPosition)
    {
        if(rams == 0)
        {
            Die();
        }
        else
        {
            //Removes the blocks from the hit player starting from the hit position
            int index = otherPlayer.GetFullPlayerGridPosition().IndexOf(hitPosition);

            for(int i = otherPlayer.PlayerBodyBlocks.Count - 1; i >= index; i--)
            {
                GameObject blockToRemove;
                otherPlayer.GetFullPlayerGridPosition().RemoveAt(i);
                blockToRemove = otherPlayer.PlayerBodyBlocks[i];
                otherPlayer.PlayerBodyBlocks.RemoveAt(i);

                if (blockToRemove.GetComponent<BATTERING_RAM>())
                    otherPlayer.rams--;

                DestroyImmediate(blockToRemove);
                otherPlayer.playerSize--;
            } 

            rams--;
            return;
        }
    }

    //Saves the position of the player
    public void SaveCheckpoint()
    {
        saveLoad.SaveState(playerPositionList, gridPosition, playerBodyBlocks, gridMoveDirection);
        continues++;
    }

    //Loads the last checkpoint of the player
    void LoadLastCheckpoint()
    {
        List<GameObject> tempBlocks;
        GameObject removedObj;

        gridPosition = new Vector2Int(saveLoad.LoadGridPosition().x, saveLoad.LoadGridPosition().y);
        gridMoveDirection = new Vector2Int(saveLoad.LoadDirection().x, saveLoad.LoadDirection().y);
        tempBlocks = saveLoad.LoadBlocks();


        for (int i = 0; i < playerBodyBlocks.Count; i++)
        {
            if (tempBlocks.IndexOf(playerBodyBlocks[i]) == -1)
            {
                removedObj = playerBodyBlocks[i];
                playerBodyBlocks.Remove(playerBodyBlocks[i]);
                DestroyImmediate(removedObj);
            }
        }

        playerPositionList = new List<Vector2Int>(saveLoad.LoadPositions());
    }
}
