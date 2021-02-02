using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISpriteManager : MonoBehaviour
{
    public List<Sprite> possibleIconSprites;
    public int spriteIndex = 0;

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<Image>().sprite = possibleIconSprites[spriteIndex];
    }

    public void SwapLeft()
    {
        if (spriteIndex == 0)
            spriteIndex = possibleIconSprites.Count - 1;
        else
            spriteIndex--;
    }

    public void SwapRight()
    {
        if (spriteIndex == possibleIconSprites.Count - 1)
            spriteIndex = 0;
        else
            spriteIndex++;
    }
}
