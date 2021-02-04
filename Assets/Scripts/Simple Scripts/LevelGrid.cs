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

    GameController _gc;
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
        _gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        Player.OnPlayerMoved += GetBlock;
        Player.OnPlayerMoved += CheckPlayerCollision;
    }

    //Sets up the references
    public void Setup(List<Player> player)
    {
        players = new List<Player>(player);
    }

    //Spawns a new block on the level in a position not occupied by any players
    public void SpawnBlock()
    {
        foodGridPosition = new Vector2Int(Random.Range(0, width), Random.Range(0, height));

        for (int i = 0; i < _gc.GetInstantiatedPlayers().Count; i++)
        {
            Player ip = _gc.GetInstantiatedPlayers()[i].GetComponent<Player>();

            if (ip.GetFullPlayerGridPosition().IndexOf(foodGridPosition) != -1)
                SpawnBlock();
            else
            {
                blockGO = GameObject.Instantiate(AssetReference._instance.possibleBlocks[Random.Range(0, AssetReference._instance.possibleBlocks.Length)]);
                blockGO.transform.position = new Vector3(foodGridPosition.x, foodGridPosition.y);

                break;
            }
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
        if (playerGridPosition == foodGridPosition)
        {
            OnBlockCaptured(blockGO, player);
            SpawnBlock();
            if (OnBlockSpawned != null)
                OnBlockSpawned();
        }
    }

    //Validates the grid position so the player is wrapped around the level grid
    public Vector2Int ValidateGridPosition(Vector2Int gridPosition)
    {
        if (gridPosition.x < 0)
            gridPosition.x = width - 1;
        if (gridPosition.x > width - 1)
            gridPosition.x = 0;
        if (gridPosition.y > height - 1)
            gridPosition.y = 0;
        if (gridPosition.y < 0)
            gridPosition.y = height - 1;

        return gridPosition;
    }

    //Checks if a player collided with another
    //If the both players collided with their heads, both die
    //If a player collides with the midsection of another player, it dies, but not the one hit
    public void CheckPlayerCollision(Vector2Int playerGridPosition, Player player)
    {
        Player ip;
        Player jp;

        //Iterates every player
        for (int i = 0; i < _gc.GetInstantiatedPlayers().Count; i++)
        {
            if (_gc.GetInstantiatedPlayers()[i] != null)
                ip = _gc.GetInstantiatedPlayers()[i].GetComponent<Player>();
            else
                return;

            //Iterates every other player starting from the immediate next one
            for (int j = i + 1; j < _gc.GetInstantiatedPlayers().Count; j++)
            {
                if (_gc.GetInstantiatedPlayers()[j] != null)
                    jp = _gc.GetInstantiatedPlayers()[j].GetComponent<Player>();
                else
                    return;

                //Iterates all the positions of the second player
                for(int k = 0; k < jp.GetFullPlayerGridPosition().Count; k++)
                {
                    if (ip != null && jp != null)
                    {

                        if (ip.GetGridPosition() == jp.GetGridPosition())
                        {
                            //If both collide by the head, both dies
                            ip.Die();
                            jp.Die();
                            return;
                        }
                        else if (ip.GetGridPosition() == jp.GetFullPlayerGridPosition()[k])
                        {
                            //If the other hits the midsection of the first, tries to Ram
                            ip.Ram(jp);
                            return;
                        }
                    }                   
                }

                for (int l = 0; l < ip.GetFullPlayerGridPosition().Count; l++)
                {
                    if (jp.GetGridPosition() == ip.GetFullPlayerGridPosition()[l])
                    {
                        //If one hits the midsection of the other, tries to Ram
                        jp.Ram(ip);
                        return;
                    }
                }
            }
        }
    }

    public Vector2Int GetFoodGridPosition()
    {
        return foodGridPosition;
    }
}
