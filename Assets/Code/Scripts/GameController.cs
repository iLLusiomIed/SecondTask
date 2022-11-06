using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameController : MonoBehaviour
{
    public static int Score { get; private set; }

    public static bool GameStarted { get; private set; }

    public Text score;
    public Text winText;

    private Field _field;
    [Inject]public void Constructor(Field field)
    {
        this._field = field;
    }

    private void Start()
    {
        StartGame();
    }
    public void StartGame()
    {
        winText.text = "";
        SetScore(0);
        GameStarted = true;
        _field.GenerateField();
    }

    public void Win()
    {
        GameStarted = false;
        winText.text = "You WIN!";
    }
    public void Lose()
    {
        GameStarted = false;
        winText.text = "You Lose...";
    }

    public void AddScore(int value)
    {
        SetScore(Score + value);
    }

    public void SetScore(int value)
    {
        Score = value;
        score.text = Score.ToString();
    }
}
