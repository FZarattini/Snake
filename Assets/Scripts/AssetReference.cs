using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetReference : MonoBehaviour
{
    public static AssetReference _instance;

    private void Awake()
    {
        _instance = this;
    }

    public GameObject[] possibleBlocks;

    public Sprite snakeHeadSprite;
    public Text scoreText;
    public GameObject background;
    public Camera mainCamera;
}
