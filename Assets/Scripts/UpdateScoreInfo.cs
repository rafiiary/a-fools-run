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
    }

    public void UpdateScoreImage(int score)
    {
        RectTransform rt;
        rt = gemImages[score].GetComponent<RectTransform>();
        rt.localScale = new Vector3 (0.4f, 0.7f, 1f);
        gemImages[score].GetComponent<RawImage>().texture = collectedGem;
    }

    public void UpdateScoreText(int score)
    {
        _playerScoreText.text = "Score: " + score;
    }
}
