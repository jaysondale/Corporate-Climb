using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// This class handles all video game controls, aside form specific pong game mechanics. 

public class VideoGameViewController : ResizeableView
{
    // goal score for player and partner
    private int _playerGoalScore;
    private int _partnerGoalScore;
    // stores button to cancel the game
    public Button cancelButton;
    // public values to decide changes to influence and influence multiplier from game
    public float partnerPunishmentInfluenceMultiplier = -1;
    public int playerWinLossReward = 5;
    public int playerPointAccuracyAward = 10;
    public int playerPointAccuracyDeduction = 1;
    public float playerMultiplierFactor = .1f;

    public int partnerIncentive = 5;

    public int scoreToWin = 7; // how many points needed to win. Must be >= 2

    // name of the owner of the machine. 
    private string _partnerOwner;
    private GameManager _gameManager;

    private PongManager _pongManager;


    // Start is called before the first frame update
    void Start()
    {
        // set the game manager instance, add the listener to the cancel button
        _gameManager = GameManager.Instance;
        cancelButton.onClick.AddListener(OnClick);



        // make the target score for the player and display it
        CreateDisplayTargetScore();



        _pongManager = GameObject.Find("PongManager").GetComponent<PongManager>();

        _partnerOwner = _gameManager.GetManagerOwnerName(); // we get the owner of the arcade machine from the game manager
        // it's in the game manager because the scope of the arcade machine owner transcends a single scene.
        // it will be used for scoring the managers.

        // makes sure that score to win is above minimum for game objectives to work. 
        if (scoreToWin < 2)
        {
            scoreToWin = 2;
        }

    }


    // create the target score for the player
    private void CreateDisplayTargetScore()
    {
        bool _win = Random.Range(0, 2) == 1 ? true : false;

        if (_win)
        {
            _playerGoalScore = scoreToWin;
            _partnerGoalScore = Random.Range(0, scoreToWin);
        }
        else
        {
            _partnerGoalScore = 7;
            _playerGoalScore = Random.Range(0, scoreToWin);
        }

        _gameManager.GlobalAlert("Your managing partner has chosen to play Pong. To maximize how much they like you, aim to " + (_win ? "win" : "lose") + " with a score of 7 - " + (_win ? _partnerGoalScore : _playerGoalScore));
    }

    public void EndGame(int playerScore, int partnerScore)
    {
        bool _win = _playerGoalScore > _partnerGoalScore;
        bool _didWin = playerScore > partnerScore;

        int _influenceChange = 0;
        if (_win && _didWin || !_win && !_didWin)
        {
            _influenceChange += playerWinLossReward;
        }

        _influenceChange += playerPointAccuracyAward;

        _influenceChange -= (playerPointAccuracyDeduction * Mathf.Abs(playerScore - _playerGoalScore));
        _influenceChange -= (playerPointAccuracyDeduction * Mathf.Abs(partnerScore - _partnerGoalScore));

        _gameManager.IncrementInfluence(_influenceChange);

        _gameManager.PartnerInfluenceChange(_partnerOwner, partnerPunishmentInfluenceMultiplier);
        _gameManager.IncrementManagerInfluence(_partnerOwner, partnerIncentive);

        _gameManager.GlobalAlert(_partnerOwner + " is " + (_influenceChange > 5 ? "impressed" : "upset") + "!");

        _gameManager.GoToGameScene();


    }



    // If the alert is not displayed, and we aren't already playing the game, then start the game
    // Late update so this checks after Start is completed.
    void LateUpdate()
    {
        if (!_gameManager.alertDisplayed && !_pongManager.GetPlaying())
        {
            _pongManager.StartGame();
        }

    }


    // Once the cancel button is clicked, go back to the game manager. Abort everything. 
    void OnClick()
    {
        _gameManager.GoToGameScene();
    }
}
