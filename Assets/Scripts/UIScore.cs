using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScore : MonoBehaviour
{
    public GameController _gc;
    public GameObject scoreTextPrefab;
    List<GameObject> scoreTextObj;

    // Start is called before the first frame update
    void Start()
    {
        scoreTextObj = new List<GameObject>();
        GameController.OnGameStarted += InitializeScoreUI;
        GameController.OnIncreasedScore += UpdateScoreUI;
    }

    //Initializes the Score UI on the top left corner of the screen for each player
    void InitializeScoreUI()
    {

        for (int i = 0; i < _gc.Players.Count; i++)
        {
            scoreTextObj.Add(GameObject.Instantiate(scoreTextPrefab, this.transform));
            scoreTextObj[i].GetComponent<Text>().text = "Player " + (i + 1) + ": " + _gc.Players[i].score.ToString() ;
            scoreTextObj[i].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(-220f, 195f - 15 * i, 0f);
        }
    }

    // Updates the score text displayed on the screen for each player
    void UpdateScoreUI()
    {
        for(int  i = 0; i < _gc.GetInstantiatedPlayers().Count; i++)
        {
            Player player = _gc.GetInstantiatedPlayers()[i].GetComponent<Player>();
            scoreTextObj[player.playerID - 1].GetComponent<Text>().text = "Player " + (player.playerID) + ": " + player.score.ToString();
        }
    }
}
