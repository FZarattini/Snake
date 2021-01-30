using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TIME_TRAVEL : Block
{
    public override void Ability(Player player)
    {
        // Saves the player data to reload it if he dies
        player.SaveCheckpoint();
        base.Ability(player);
    }
}
