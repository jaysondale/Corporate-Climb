using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// THis manages all of the game logic foe the pong game
public class PongManager : ResizeableView
{
    protected Vector2 _resolution; // Used for Ui resizing
    public Text scoreText; // text to hold the score

    // Objects that hold the paddles, walls, and ball.
    public GameObject paddlePrefab;

    private GameObject _playerPaddle;
    private GameObject _partnerPaddle;


    public GameObject wallOne;
    public GameObject wallTwo;
    public GameObject ball;

    // holds the score
    private int _partnerScore;
    private int _playerScore;

    // flags for game logic
    private bool _gameStarted;

    private bool _gamePaused;

    // vectors (x and y) for the balls.
    // These are 3D vectors since the "X" direction is actually the
    // camera X direction,  which may have some y and z components.
    private Vector3 _ballDirectionX;
    private Vector3 _ballDirectionY;


    // some parameters to control game speed
    public float ballSpeed = 0.3f;
    public float paddleSpeed = 1;
    public float partnerPaddleSpeed = 0.1f;
    public float yMultiplier = 10;

    // Start is called before the first frame update
    void Start()
    {
        HARDCODED_RESOLUTION = new Vector2(1510, 755); // resolution in which the scene is designed
        _resolution = HARDCODED_RESOLUTION;

        ResizeUIComponents();
        _resolution = new Vector2(Screen.width, Screen.height);

        scoreText.text = "";
        _gameStarted = false;
        _gamePaused = false;
        RotateRelativeToCamera(Camera.main, wallOne); // make sure the wall is properly rotated
        RotateRelativeToCamera(Camera.main, wallTwo);

    }

    // Update is called once per frame
    void Update()
    {
        
        if (_gameStarted)
        {
            MoveBallAndPartnerPaddle(); // move the ball and AI paddle
            // resize components if needed
            if (Screen.width != _resolution.x || Screen.height != _resolution.y)
            {
                ResizeUIComponents();
                _resolution = new Vector2(Screen.width, Screen.height);
            }

            scoreText.text = _playerScore + " - " + _partnerScore; // update score text

            // Move the paddle if getting input. 
            if (Input.GetKey(KeyCode.UpArrow))
            {
                MovePlayerPaddle(true);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                MovePlayerPaddle(false);
            }

            // rotate the paddles 
            RotateRelativeToCamera(Camera.main, _playerPaddle);
            RotateRelativeToCamera(Camera.main, _partnerPaddle);

            // check if you won and lost the point.
            if (ball.transform.position.x < (_playerPaddle.transform.position.x-.05) && !_gamePaused)
            {
                // Lost the point.
                PartnerPoint();
            } else if (ball.transform.position.x > (_partnerPaddle.transform.position.x + .05) && !_gamePaused)
            {
                PlayerPoint();
            }


        }



    }

    // Give point to the partner - pause ball and restart point
    private void PartnerPoint()
    {

        _gamePaused = true;
        _partnerScore += 1;
        _ballDirectionX = Vector3.zero;
        _ballDirectionY = Vector3.zero;


        Invoke("NewPoint", 2);

        
    }
    // Give point to the player - pause ball and restart point
    private void PlayerPoint()
    { 
        _gamePaused = true;
        _playerScore += 1;
        _ballDirectionX = Vector3.zero;
        _ballDirectionY = Vector3.zero;

        Invoke("NewPoint", 2);

    }
    // If no one has won, start the point with a random direction (in reason).
    // if someone has won, send message to VideoGameViewController
    private void NewPoint()
    {

        if (_playerScore < 7 && _partnerScore < 7)
        {
            _gamePaused = false;

            ball.GetComponent<Transform>().position = new Vector3(-5.19f, 3.52f, 34.89f);

            _ballDirectionX = (Random.Range(0, 2) == 1 ? -1 : 1) * Camera.main.transform.right;

            _ballDirectionY = Random.Range(-1f, 1f) * Camera.main.transform.up;
        } else
        {
            Camera.main.GetComponent<VideoGameViewController>().EndGame(_playerScore, _partnerScore);
        }
    }


    private void MoveBallAndPartnerPaddle()
    {
        // Move the ball based on current direction
        Vector3 _normalizedTotalDirection = _ballDirectionX + _ballDirectionY;
        _normalizedTotalDirection.Normalize();
        ball.transform.Translate(_normalizedTotalDirection * ballSpeed * Time.deltaTime);

        // Move the paddle as needed.
        // If the paddle is above the ball, move it down and vice versa.
        Vector3 dir;
        if (ball.transform.position.y > (_partnerPaddle.transform.position.y + .02))
        {
            dir = Camera.main.transform.up;
        } else if (ball.transform.position.y < (_partnerPaddle.transform.position.y - .02))
        {
            dir = -Camera.main.transform.up;
        } else {
            dir = Vector3.zero;
        }

        if (_partnerPaddle.transform.position.y + dir.y * partnerPaddleSpeed * Time.deltaTime <= 3.733 && _partnerPaddle.transform.position.y + dir.y * partnerPaddleSpeed * Time.deltaTime >= 3.289)
        {
            dir.Normalize();
            _partnerPaddle.transform.Translate(dir * partnerPaddleSpeed * Time.deltaTime);
        }
        
    }



    // make sure the objects are looking at the camera, so the pong game appears 2D
    private void RotateRelativeToCamera(Camera cam, GameObject obj)
    {
        obj.transform.LookAt(cam.transform, cam.transform.up);
    }

    void MovePlayerPaddle(bool up)
    {
        // Move the paddle up or down, within limits, depending on inputs. 
        Vector3 dir;
        if (up)
        {
            dir = Camera.main.transform.up;

        }
        else
        {
            dir = -Camera.main.transform.up;

        }

        
        
        if (_playerPaddle.transform.position.y + dir.y * paddleSpeed * Time.deltaTime <= 3.736 && _playerPaddle.transform.position.y + dir.y * paddleSpeed * Time.deltaTime >= 3.24)
        {

            dir.Normalize();
            _playerPaddle.transform.Translate(dir * paddleSpeed * Time.deltaTime);
        } else if (_playerPaddle.transform.position.y + dir.y * paddleSpeed * Time.deltaTime > 3.736)
        {
            _playerPaddle.GetComponent<Transform>().position = new Vector3(-5.61225f, 3.735676f, 34.92545f);
        } else if (_playerPaddle.transform.position.y + dir.y * paddleSpeed * Time.deltaTime < 3.24) {
            _playerPaddle.GetComponent<Transform>().position = new Vector3(-5.669396f, 3.24f, 34.92545f);
        }


    }

    // resize the UI element as needed
    protected void ResizeUIComponents()
    {
        ResizeText(scoreText, _resolution);
    }

    public void StartGame()
    {
        // set all flags to default value, set the manager in the pong collision handler.
        _gameStarted = true;
        _partnerScore = 0;
        _playerScore = 0;

        ball.GetComponent<PongBallCollisionHandler>().SetManager(gameObject);

        // instantiate paddles
        _playerPaddle = Instantiate(paddlePrefab);
        _playerPaddle.GetComponent<Transform>().position = new Vector3(-5.62f, 3.53f, 34.89f);
        _playerPaddle.GetComponent<Transform>().rotation = Quaternion.Euler(new Vector3(14.442f, -19.831f, -4.328f));
        _playerPaddle.GetComponent<Transform>().localScale = new Vector3(0.02f, 0.1f, 0.5f);

        _partnerPaddle = Instantiate(paddlePrefab);
        _partnerPaddle.GetComponent<Transform>().position = new Vector3(-4.75f, 3.53f, 34.89f);
        _partnerPaddle.GetComponent<Transform>().rotation = Quaternion.Euler(new Vector3(14.442f, 24f, 6f));
        _partnerPaddle.GetComponent<Transform>().localScale = new Vector3(0.02f, 0.1f, 0.5f);
       
        // start a new point
        NewPoint();



    }

    public void PaddleCollision()
    {
        // this will flip the x component
        _ballDirectionX = -_ballDirectionX;

        // this will determine some y component as per Figure 1 in requirements.
        // Higher ball hits on paddle, more "up" it goes, and vice versa. 

        float _distance;
        if (ball.transform.position.x < -5.2f)
        {
            _distance = ball.transform.position.y - _playerPaddle.transform.position.y;
            Debug.Log("player");
        } else
        {
            _distance = ball.transform.position.y - _partnerPaddle.transform.position.y;
            Debug.Log("partner");
        }
        Debug.Log(_distance);
        _ballDirectionY += Camera.main.transform.up * _distance * yMultiplier;

        
    }

    public void WallCollision()
    {
        // flips y direction
        _ballDirectionY = -_ballDirectionY;

    }

    // returns if the game is playing
    public bool GetPlaying()
    {
        return _gameStarted;
    }
}

