using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    Player player;
    GameController _gc;
    LevelGrid _lg;
    Vector2Int blockPosition;
    float inputDelay = .5f;
    bool canInput = true;

    private void Awake()
    {
        player = gameObject.GetComponent<Player>();
        _gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void Start()
    {
        _lg = _gc.getLevelGrid();

        if (_lg != null)
        {
            blockPosition = _lg.FoodGridPosition;
        }

        LevelGrid.OnBlockSpawned += AssignBlock;
    }

    private void Update()
    {
        //Decides a direction in a interval equals to the inputDelay variable
        if (canInput)
        {
            DecideDirection();
        }

        if (!canInput)
            inputDelay -= Time.deltaTime;

        if (inputDelay <= 0f)
        {
            canInput = true;
            inputDelay = .5f;
        }
    }
    
    //References the food block on the level grid
    void AssignBlock()
    {
        blockPosition = _lg.FoodGridPosition;
    }

    //Decides which direction the AI should go next
    void DecideDirection()
    {
        int rollChance;

        rollChance = Random.Range(1, 10);

        if (rollChance <= 5)
            SeekBlock();
        else if (rollChance == 6)
            EscapeBlock();
        //else it keeps going straight
    }

    //Makes a decision to try and approach the block (high chance)
    void SeekBlock()
    {
        Vector2Int posDiff = FindRelativePositionToBlock();
        Vector2Int moveDir = player.MoveGridDirection;
        Vector2Int resultDir = new Vector2Int();


        //Returns the direction in which the AI should turn based on the relative position between the AI and the food block and the direction the AI is currently going
        //Needs improvement
        if (((moveDir.x == 1 || moveDir.x == -1) && posDiff.y == 0) || ((moveDir.y == 1 || moveDir.y == -1) && posDiff.x == 0))
        {
            resultDir = moveDir;
        }
        else if ((moveDir.x == 1 && moveDir.y == 0) || (moveDir.x == -1 && moveDir.y == 0))
        {
            if ((posDiff.x < 0 && posDiff.y < 0) || (posDiff.x > 0 && posDiff.y < 0))
                resultDir = new Vector2Int(0, 1);
            else if ((posDiff.x < 0 && posDiff.y > 0) || (posDiff.x < 0 && posDiff.y > 0))
                resultDir = new Vector2Int(0, -1);
            else if ((posDiff.x == 0 && posDiff.y < 0))
                resultDir = new Vector2Int(0, 1);
            else if ((posDiff.x == 0 && posDiff.y > 0))
                resultDir = new Vector2Int(0, -1);

        }
        else if ((moveDir.x == 0 && moveDir.y == 1) || (moveDir.x == 0 && moveDir.y == -1))
        {
            if ((posDiff.x < 0 && posDiff.y < 0) || (posDiff.x > 0 && posDiff.y < 0))
                resultDir = new Vector2Int(1, 0);
            else if ((posDiff.x < 0 && posDiff.y > 0) || (posDiff.x < 0 && posDiff.y > 0))
                resultDir = new Vector2Int(-1, 0);
            else if ((posDiff.x < 0 && posDiff.y == 0))
                resultDir = new Vector2Int(1, 0);
            else if((posDiff.x > 0 && posDiff.y == 0))
                resultDir = new Vector2Int(-1, 0);
        }

        //Guarantees the AI won't turn on itself (Needs improvement)
        if(moveDir.x == 0 && resultDir.x == 0)
        {
            if (resultDir.y == (-1) * moveDir.y)
                resultDir = new Vector2Int(1,0);
        }else if (moveDir.y == 0 && resultDir.y == 0)
        {
            if (resultDir.x == (-1) * moveDir.x)
                resultDir = new Vector2Int(0,1);
        }

        if (resultDir.x == 0 && resultDir.y == 0)
            resultDir = moveDir;

        player.UpdatePlayerDirection(resultDir.x, resultDir.y);
        canInput = false;
    }


    //Makes a decision to try and escape from the food block (low chance)
    void EscapeBlock()
    {
        Vector2Int posDiff = FindRelativePositionToBlock();
        Vector2Int moveDir = player.MoveGridDirection;
        Vector2Int resultDir = new Vector2Int();

        //Returns the direction in which the AI should turn based on the relative position between the AI and the food block and the direction the AI is currently going
        //Needs improvement
        if (((moveDir.x == 1 || moveDir.x == -1) && posDiff.y == 0) || ((moveDir.y == 1 || moveDir.y == -1) && posDiff.x == 0))
        {
            resultDir = moveDir;
        }
        else if ((moveDir.x == 1 && moveDir.y == 0) || (moveDir.x == -1 && moveDir.y == 0))
        {
            if ((posDiff.x < 0 && posDiff.y < 0) || (posDiff.x > 0 && posDiff.y < 0))
                resultDir = new Vector2Int(0, -1);
            else if ((posDiff.x < 0 && posDiff.y > 0) || (posDiff.x < 0 && posDiff.y > 0))
                resultDir = new Vector2Int(0, 1);
            else if ((posDiff.x == 0 && posDiff.y < 0))
                resultDir = new Vector2Int(0, -1);
            else if ((posDiff.x == 0 && posDiff.y > 0))
                resultDir = new Vector2Int(0, 1);

        }
        else if ((moveDir.x == 0 && moveDir.y == 1) || (moveDir.x == 0 && moveDir.y == -1))
        {
            if ((posDiff.x < 0 && posDiff.y < 0) || (posDiff.x > 0 && posDiff.y < 0))
                resultDir = new Vector2Int(-1, 0);
            else if ((posDiff.x < 0 && posDiff.y > 0) || (posDiff.x < 0 && posDiff.y > 0))
                resultDir = new Vector2Int(1, 0);
            else if ((posDiff.x < 0 && posDiff.y == 0))
                resultDir = new Vector2Int(-1, 0);
            else if ((posDiff.x > 0 && posDiff.y == 0))
                resultDir = new Vector2Int(1, 0);
        }

        //Guarantees the AI won't turn on itself (Needs improvement)
        if (moveDir.x == 0 && resultDir.x == 0)
        {
            if (resultDir.y == (-1) * moveDir.y)
                resultDir = new Vector2Int(1, 0);
        }
        else if (moveDir.y == 0 && resultDir.y == 0)
        {
            if (resultDir.x == (-1) * moveDir.x)
                resultDir = new Vector2Int(0, 1);
        }

        if (resultDir.x == 0 && resultDir.y == 0)
            resultDir = moveDir;

        player.UpdatePlayerDirection(resultDir.x, resultDir.y);
        canInput = false;
    }

    //Gets the relative position between the AI and the food block on the level grid
    Vector2Int FindRelativePositionToBlock()
    {
        Vector2Int posDiff = new Vector2Int(player.GridPosition.x - blockPosition.x, player.GridPosition.y - blockPosition.y) ;

        return posDiff;
    }
}
