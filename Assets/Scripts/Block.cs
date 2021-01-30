using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    //Describes the special ability this block grants the player
    public virtual void Ability(Player player) { }
}
