using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid
{
    //events
    public delegate void SpawnedBlock();
    public static event SpawnedBlock OnBlockSpawned;

    public delegate void BlockCaptured(GameObject block, Player player);
    public static event BlockCaptured OnBlockCaptured;

    GameObject blockGO;
    private List<Player> players;

    Vector2Int foodGridPosition;
    int width;
    int height;

    //Constructor
    public LevelGrid(int width, int height)
    {
        this.width = width;
        this.height = height;

        Player.OnPlayerMoved += GetBlock;
    }

    //Sets up the references
    public void Setup(List<Player> player)
    {
        players = new List<Player>(player);

        SpawnBlock();
    }


    //Spawns a new block on the level in a position not occupied by the player
    void SpawnBlock()
    {    
        foodGridPosition = new Vector2Int(Random.Range(0, width), Random.Range(0, height));

        foreach(Player p in players)
        {            
            blockGO = GameObject.Instantiate(AssetReference._instance.possibleBlocks[Random.Range(0, AssetReference._instance.possibleBlocks.Length)]);
            blockGO.transform.position = new Vector3(foodGridPosition.x, foodGridPosition.y);
            break;
        }

    }

    //Destroys the block
    void DestroyBlock()
    {
        Object.DestroyImmediate(blockGO);
    }

    //Captures the block
    public void GetBlock(Vector2Int playerGridPosition, Player player)
    {   
        if(playerGridPosition == foodGridPosition)
        {
            OnBlockCaptured(blockGO, player);
            //Object.DestroyImmediate(blockGO);
            SpawnBlock();
        }
    }
   
}
