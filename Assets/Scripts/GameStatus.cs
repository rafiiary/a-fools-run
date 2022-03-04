using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameStatus : MonoBehaviour
{
    public GameObject timeRemainingObj;
    public GameObject gameStatObj;
    public GameObject gameOperObj;
    public GameObject playerObj;
    public ScoreManager scoreManager;

    private Text timeRemainingText;
    private Text gameStatText;
    private Text gameOperText;

    public float timeLeft = 90;
    public int requiredScoreToWin = 6;
    private float totalTime;
    public Slider slider;
    public bool winStat = false;
    private Rigidbody _rigidbody;

    private const string CollectMoreGemsMessage = "Collect more gems and come back!";

    // Start is called before the first frame update
    void Start() {
        timeRemainingText = timeRemainingObj.GetComponent<Text>();
        gameStatText = gameStatObj.GetComponent<Text>();
        gameOperText = gameOperObj.GetComponent<Text>();
        timeRemainingText.text = "Time Remaining: " + timeLeft;
        gameStatText.text = "";
        gameOperText.text = "";
        slider.value = 1f;
        totalTime = timeLeft;
    }

    // Update is called once per frame
    void Update() {
        // Check remaining time of this round
        if (timeLeft > 0) {
            timeLeft -= Time.deltaTime;
        } else {
            PauseGame("lose");
        }
        DisplayTime(timeLeft);

        // Check if player has reached end of the maze
        for (int i=0; i<playerObj.transform.childCount; i++) {
            GameObject childObj = playerObj.transform.GetChild(i).gameObject;
            if (PlayerCollision.hitFinishLine && scoreManager.GetScore() >= requiredScoreToWin)
            {
                winStat = true;
                }
                // else if (PlayerCollision.hitFinishLine)
                // {
                //     DisplayMessage(gameStatText, CollectMoreGemsMessage);
                //     StartCoroutine(ClearMessageAfterDelay(gameStatText, 2));
                //     
                // }
        }
        if (winStat == true) {
            PauseGame("win");
        }

        // Detect input about pause and resume
        if (Input.GetKeyDown(KeyCode.P)) {
            PauseGame("pause");
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            if (timeLeft > 0 & winStat == false) {
                ResumeGame();
            } else {
                RestartGame();
            }
        }
    }
    
    void DisplayTime(float time) {
        slider.value = timeLeft / totalTime;
    }

    public void PauseGame(string type) {
        Time.timeScale = 0;

        if (type == "pause") {
            DisplayMessage(gameStatText, "Game Paused");
            DisplayMessage(gameOperText, "Resume");
        } else if (type == "lose") {
            DisplayMessage(gameStatText, "Game Over!");
            DisplayMessage(gameOperText, "Restart");
        } else if (type == "win") {
            DisplayMessage(gameStatText, "You Win!");
            DisplayMessage(gameOperText, "Restart");
        }
    }

    public void ResumeGame() {
        Time.timeScale = 1;
        gameStatText.text = "";
        gameOperText.text = "";
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
        gameStatText.text = "";
        gameOperText.text = "";
        winStat = false;
    }
    
    public void DisplayMessage(Text textArea, string message) {
        // TODO: refactor this function to just check membership of an Enum of all the messages.
        if (message == "Game Over!") {
            textArea.text = "Game Over!";
        } else if (message == "Game Paused") {
            textArea.text = "Game Paused";
        } else if (message == "Resume") {
            textArea.text = "Resume";
        } else if (message == "You Win!") {
            textArea.text = "You Win!";
        } else if (message == "Restart") {
            textArea.text = "Restart";
        } else if (message == CollectMoreGemsMessage)
        {
            textArea.text = CollectMoreGemsMessage;
        }else {
            textArea.text = "ERROR: Unknown input!";
        }
    }

    public void ClearMessage(Text textArea)
    {
        textArea.text = "";
    }
    
    IEnumerator ClearMessageAfterDelay(Text textArea, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        ClearMessage(textArea);
    }
}
