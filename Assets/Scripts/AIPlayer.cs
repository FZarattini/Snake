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
    float moveDelay;
    bool canMove = true;

    private void Awake()
    {
        player = gameObject.GetComponent<Player>();
        _gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void Start()
    {
        _lg = _gc.getLevelGrid();
        //Player.OnPlayerMoved += DecideDirection;
        moveDelay = player.moveDelay;
        if (_lg != null)
        {
            blockPosition = _lg.GetFoodGridPosition();
        }
        LevelGrid.OnBlockSpawned += AssignBlock;
        //inputDelay = player.moveDelay;
    }

    private void Update()
    {

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
    
    void AssignBlock()
    {
        blockPosition = _lg.GetFoodGridPosition();
    }

    void DecideDirection()
    {
        int rollChance;

        rollChance = Random.Range(1, 10);

        if (rollChance <= 5)
            SeekBlock();
        else if (rollChance == 6)
            EscapeBlock();
    }

    void SeekBlock()
    {
        Vector2Int posDiff = FindRelativePositionToBlock();
        Vector2Int moveDir = player.GetGridDirection();
        Vector2Int resultDir = new Vector2Int();

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

    void EscapeBlock()
    {
        Vector2Int posDiff = FindRelativePositionToBlock();
        Vector2Int moveDir = player.GetGridDirection();
        Vector2Int resultDir = new Vector2Int();

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

    Vector2Int FindRelativePositionToBlock()
    {
        Vector2Int posDiff = new Vector2Int(player.GetGridPosition().x - blockPosition.x, player.GetGridPosition().y - blockPosition.y) ;

        return posDiff;
    }
}
