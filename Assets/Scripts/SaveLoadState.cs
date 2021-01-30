using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadState
{
    public static void SaveState(List<Vector2Int> positions, Vector2Int gridPosition)
    {
        string s = "";
        string s2 = "";

        s += string.Format("{0}:{1}", gridPosition.x, gridPosition.y);

        PlayerPrefs.SetString("GridPosition", s);

        foreach(Vector2Int v in positions)
        {
            s2 += string.Format("{0}:{1}:", v.x, v.y);
        }

        PlayerPrefs.SetString("Positions", s2);

    }

    public static void LoadGridState()
    {

    }

    

    public static void LoadState(List<Vector2Int> positions, Vector2Int gridPosition)
    {

    }
}
