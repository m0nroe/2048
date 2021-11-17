using TMPro;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    public static int ScorePoints { get; private set; }
    public static bool isGameStarted { get; private set; }

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI gameResult;


    private void Start()
    {
        StartGame();
    }

    public void StartGame() 
    {
        gameResult.text = "";
        SetScorePoints(0);
        isGameStarted = true;

        Field.Instance.GenerateField();
    }

    public void Win() 
    {
        isGameStarted = false;
        gameResult.text = "You Win!";
    }
    public void Lose() 
    {
        isGameStarted = false;
        gameResult.text = "You Lose!";
    }

    public void AddScorePoints(int points) 
    {
        SetScorePoints(ScorePoints + points);
    }

    private void SetScorePoints(int points)
    {
        ScorePoints = points;
        scoreText.text = ScorePoints.ToString();
    }
}
