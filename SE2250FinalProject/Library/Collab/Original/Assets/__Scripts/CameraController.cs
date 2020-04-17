using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Main controller for Main Game Scene.
// Contains a variety of methods and fields needed to operate the game.
public class CameraController : MonoBehaviour
{

    private GameObject _playerObject; // Player Game Object
    private GameObject _player; // field for instance
    private GameManager _gameManager; // Game Manager field
    public GameObject BotObject; // Bot Object Field
    public GameObject ManagerObject;
    // fields for movement
    public float turnSpeed;
    private Transform _playerTransform;
    private Vector3 _playerScale = new Vector3(3f, 3f, 2f);

    private bool _canInteract;
    private bool _canEnterComputer;
    private bool _canPlayVideoGame;

    // Text variables
    public Text cashText;
    public Text influenceText;
    public Text instructionsText;
    public Text salaryText;
    public Text multiplierText;
    public Text botText;
    public Text codeReviewTimerText;

    // Fields for camera
    public float distance = 10.0f;
    // the height we want the camera to be above the _playerTransform
    public float height = 5.0f;
    // How much we want height to be dampened, along with rotation.
    public float heightDamping = 2.0f;
    public float rotationDamping = 1.0f;

    // Text for the Text variables.
    private string _talkingInstructions = "Press 'T' to Talk";
    private string _computerInstructions = "Press 'C' to enter computer";
    private string _videoGameInstructions = "Press 'P' to play video game with manager";

    // Shader to fix shadow glitch in Unity.
    private Shader _standardShader;

    // Source: https://stackoverflow.com/questions/44104202/shadows-for-instantiated-game-object-bug
    void ChangeShader() // because shadow for assetbundle is not working properly (Unity Bug).
    {
        var renderers = FindObjectsOfType<Renderer>() as Renderer[];
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.shader = _standardShader;
        }
    }


    void Start()
    {
        _standardShader = Shader.Find("Standard"); // fix shader

        // Init flags to false
        _canInteract = false;
        _canEnterComputer = false;
        _canPlayVideoGame = false;

        // Instantiate player
        _gameManager = GameManager.Instance;
        _playerObject = _gameManager.GetCurrentPlayer();

        _player = Instantiate(_playerObject);
        _gameManager.SetPlayerObject(_player);
        ChangeShader();
        _playerTransform = _player.GetComponent<Transform>();
        _playerTransform.position = new Vector3(.02f, 0f, -1.135623f);
        _playerTransform.rotation = new Quaternion(0, 0, 0, 0);

        
        // Instantiate bots
        SortedDictionary<string, int> _bots = _gameManager.GetBots();
        _gameManager.Bots = new List<GameObject>();

        int i = 0;
        foreach (KeyValuePair<string, int> bot in _bots)
        {
            GameObject newBot = Instantiate(BotObject);
            BotController botController = newBot.GetComponent<BotController>();
            botController.SetCharacterName(bot.Key);
            botController.SetOfferSuccess(80);
            botController.SetComplimentSuccess(95);
            botController.SetInsultSuccess(100);
            newBot.transform.position = new Vector3(10 - 8 * i, 0, 0);
            newBot.transform.localScale = _playerScale;
            _gameManager.Bots.Add(newBot);
            i++;
        }

        // Introduce game with some alerts
        _gameManager.GameIntroductionAlerts();

        // Add dog if at level two
        if (_gameManager.GetLevel() == 2)
        {
            Vector3 xyz = new Vector3(0, 90, 0);
            Quaternion newRotation = Quaternion.Euler(xyz);
            Vector3 playerTransform = _gameManager.GetCurrentPlayer().transform.position;
            Instantiate(_gameManager.DogPrefab, new Vector3(playerTransform.x, 1, playerTransform.z - 2), newRotation);

            // Instantiate managers
            i = 0;
            foreach (KeyValuePair<string, int> manager in _gameManager.GetManagers())
            {
                GameObject newManager = Instantiate(ManagerObject);
                BotController botController = newManager.GetComponent<BotController>();
                botController.SetCharacterName(manager.Key);
                botController.SetOfferSuccess(80);
                botController.SetComplimentSuccess(95);
                botController.SetInsultSuccess(100);
                newManager.transform.localScale = _playerScale;
                newManager.transform.position = new Vector3(-2.27f, 0, 21 + 13*i);
                newManager.transform.rotation = Quaternion.Euler(new Vector3(0, 210,0));
                _gameManager.Managers.Add(newManager);
                i++;
            }
        }
        


    }

    void Update()
    {
        // Keep text updated, check for collision
        cashText.text = "Cash: $" + _gameManager.GetCash();
        influenceText.text = "Influence: " + _gameManager.GetInfluence();
        // alertText.text = _gameManager.GetGlobalAlert();
        salaryText.text = "Current Salary: $" + _gameManager.GetSalary();
        multiplierText.text = "Multiplier: " + _gameManager.GetMultiplier();

        

        // Get time remaining for code reveiw
        if (_gameManager.GetCodeReviewActive())
        {
            // Construct time
            int minutes = Mathf.FloorToInt(_gameManager.GetTimeToNextCodeReview() / 60);
            int seconds = Mathf.FloorToInt(_gameManager.GetTimeToNextCodeReview() - (minutes * 60));
            codeReviewTimerText.text = "Work Due In: " + minutes + ":" + seconds;
        } else
        {
            codeReviewTimerText.text = "";
        }
        
        botText.text = BotText();
        PlayerController playerController = _player.GetComponent<PlayerController>();
        _canInteract = playerController.GetColliding();

        _canEnterComputer = playerController.isSitting && playerController.GetCollidingWithDesk(); // to enter computer, must be squasting

        _canPlayVideoGame = playerController.GetCollidingVideoGame();
        UpdateInstructionsText();
    }

    // change text on lower left corner depending on where character is.
    void UpdateInstructionsText()
    {
        if (_canInteract)
        {
            instructionsText.text = _talkingInstructions;
        }
        else if (_canEnterComputer)
        {
            instructionsText.text = _computerInstructions;
        } else if (_canPlayVideoGame)
        {
            instructionsText.text = _videoGameInstructions;
        }
        else
        {
            instructionsText.text = "Current Position: " + _gameManager.GetPosition() + "\n" + (100 - _gameManager.GetInfluence()) + " influence to a promotion!";
        }
    }

    // Show the stats of the bots.
    public string BotText()
    {
        return _gameManager.GetBotStats();
    }
    void LateUpdate()
    {

        // Source: https://gamedev.stackexchange.com/questions/130621/turning-the-camera-when-the-player-turns

        if (!_playerTransform) return;

        // Calculate the current rotation angles
        float wantedRotationAngle = _playerTransform.eulerAngles.y;

        float currentRotationAngle = transform.eulerAngles.y;

        // Damp the rotation around the y-axis
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

        // Convert the angle into a rotation
        var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
        transform.position = _playerTransform.position;
        transform.position -= currentRotation * Vector3.forward * distance;

        // Set the height of the camera
        transform.position = new Vector3(transform.position.x, height, transform.position.z);

        // Always look at the _playerTransform
        transform.LookAt(_playerTransform);

    }
}
