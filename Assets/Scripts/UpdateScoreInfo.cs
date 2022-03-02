using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateScoreInfo : MonoBehaviour
{
    public GameObject playerScoreObj;

    public GameObject[] gemImages;
    public Texture collectedGem;

    private Text _playerScoreText;
    // Start is called before the first frame update
    void Start()
    {
        _playerScoreText = playerScoreObj.GetComponent<Text>();
        _playerScoreText.text = "Score: 0";
    }

    public void UpdateScoreImage(int score)
    {
        gemImages[score].GetComponent<RectTransform>().scale.x = 0.4;
        gemImages[score].GetComponent<RectTransform>().scale.y = 0.7;
        gemImages[score].GetComponent<RawImage>().texture = collectedGem;
    }

    public void UpdateScoreText(int score)
    {
        _playerScoreText.text = "Score: " + score;
    }
}
