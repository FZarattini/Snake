using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
public class SaveLoadState:MonoBehaviour
{
    List<GameObject> playerBlocksSaved;
    List<Vector2Int> playerPositionsSaved;
    Vector2Int gridPositionSaved;
    Vector2Int gridMoveDirectionSaved;

    // Saves the player data in relation with the grid
    public void SaveState(List<Vector2Int> positionsToSave, Vector2Int gridPositionToSave, List<GameObject> blocksToSave, Vector2Int gridMoveDirectionToSave)
    {
        playerPositionsSaved = new List<Vector2Int>();
        playerBlocksSaved = new List<GameObject>();
       
        for(int i = 0; i < positionsToSave.Count; i++)
        {
            playerPositionsSaved.Add(positionsToSave[i]);
        }

        for (int i = 0; i < blocksToSave.Count; i++)
        {
            playerBlocksSaved.Add(blocksToSave[i]);
        }

        playerBlocksSaved = new List<GameObject>(blocksToSave);

        gridPositionSaved =  new Vector2Int(gridPositionToSave.x, gridPositionToSave.y);
        gridMoveDirectionSaved = new Vector2Int(gridMoveDirectionToSave.x, gridMoveDirectionToSave.y);

    }

    // Loads the player's grid position
    public Vector2Int LoadGridPosition()
    {
        return gridPositionSaved;
    }

    // Loads the positions of all the player's body parts
    public List<Vector2Int> LoadPositions()
    {
        return playerPositionsSaved;
    }

    // Loads all the blocks the player is made of
    public List<GameObject> LoadBlocks()
    {
        return playerBlocksSaved;
    }

    public Vector2Int LoadDirection()
    {
        return gridMoveDirectionSaved;
    }
}
