using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectionUI : MonoBehaviour
{
    GameController _gc;
    public List<Sprite> playerIcons;
    public Transform initialIconPosition;
    List<GameObject> players;

    private void Awake()
    {
        _gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    //Add a new player to the board
    public void AddPlayer()
    {
        GameObject.Instantiate(playerIcons[0], initialIconPosition);
    }

    public void CyclePlayer()
    {
        //Cycle through the icons of the possible players
    }

    public void ConfirmPlayer()
    {
        //Confirms the player
    }
}
