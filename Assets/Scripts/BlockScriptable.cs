using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Block", menuName = "New/Block", order = 1)]
public class BlockScriptable : ScriptableObject
{
    int id;
    Sprite icon;
}
