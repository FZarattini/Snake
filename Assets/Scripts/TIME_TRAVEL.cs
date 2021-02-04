using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TIME_TRAVEL : Block
{
    //The special ability the block grants the player
    public override void Ability(Player player)
    {
        // Saves the player data to reload it if he dies
        player.continues = 1;
        player.SaveCheckpoint();
        base.Ability(player);
    }
}
