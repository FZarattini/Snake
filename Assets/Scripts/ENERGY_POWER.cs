using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ENERGY_POWER : Block
{
    //By how much the speed of the player grows when this block is captured
    public float speedFactor;

    public override void Ability(Player player)
    {
        player.playerSpeed += speedFactor;
        base.Ability(player);
    }
}
