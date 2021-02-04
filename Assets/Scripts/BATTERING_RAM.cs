using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BATTERING_RAM : Block
{
    //The ability the block grants the player
    public override void Ability(Player player)
    {
        player.rams++;
        base.Ability(player);
    }
}
