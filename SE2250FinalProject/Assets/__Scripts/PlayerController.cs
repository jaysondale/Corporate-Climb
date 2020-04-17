using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // Basic player properties
    public float rotateSpeed;
    public float speed;
    public bool isSitting;
    public string Name;
    public string OutfitName;

    // Will hold Animator instance and link scripted variables to animator variables
    private Animator animator;

    // Get GameManager instance
    private GameManager _gameManager = GameManager.Instance;

    // Flags for collision and actions
    private bool _socializingFlag;
    private bool _sittingFlag;
    private bool _isColliding;
    private bool _isCollidingWithDesk;
    private bool _isCollidingVideoGame;
    private bool _isInOffice;

    // Holds bot being collided with
    private GameObject _collidingBot;

    private string _gameOwnerName = "";

    // Time interval for recording path for dog
    private float _positionTimeInterval = 0.5f;



  

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        _socializingFlag = false;
        _sittingFlag = false;
        _isColliding = false;
        _isCollidingWithDesk = false;
        _isCollidingVideoGame = false;
        _isInOffice = false;

        UpdatePositionList();
    }

    // Update is called once per frame
    void Update()
    {
        
        float translation = Input.GetAxis("Vertical") * speed;
        animator.SetFloat("Speed", translation);
        animator.SetBool("isSitting", isSitting);
        float strafe = Input.GetAxis("Horizontal") * speed;
        translation *= Time.deltaTime;
        strafe *= Time.deltaTime;

        transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed, 0);
        transform.Translate(0, 0, translation);

        if (Input.GetKey(KeyCode.Space)) { ToggleSit(); }

        if (_isColliding)
        {
            if (Input.GetKey(KeyCode.T)) { Socialize(_collidingBot); }
        }

        if (_isCollidingWithDesk && isSitting)
        {
            if (Input.GetKey(KeyCode.C))
            {
                isSitting = false;
                _gameManager.GoToComputerView();
            }
        }

        if (_isCollidingVideoGame)
        {
            if (Input.GetKey(KeyCode.P))
            {
                _gameManager.GoToVideoGame(_gameOwnerName);
            }
        }

        // If on level two, record positions

        if (_gameManager.GetLevel() == 2 && translation > 0.0 && !_isInOffice)
        {
            _positionTimeInterval -= Time.deltaTime;
            if (_positionTimeInterval <= 0.0f)
            {
                UpdatePositionList();
                
                _positionTimeInterval = 0.5f; 
            }
        }

    }

    private void UpdatePositionList()
    {
        _gameManager.PlayerPositions.Enqueue(transform.position);
    }

    public bool GetColliding()
    {
        return _isColliding;
    }

    public bool GetCollidingWithDesk()
    {
        return _isCollidingWithDesk;
    }

    public bool GetCollidingVideoGame()
    {
        return _isCollidingVideoGame;
    }


    void ResetSocializingFlag()
    {
        _socializingFlag = false;
    }

    void ResetSittingFlag()
    {
        _sittingFlag = false;
    }

    // Toggle sitting variable
    void ToggleSit()
    {
        if (_sittingFlag == false)
        {
            isSitting = !isSitting;
            _sittingFlag = true;
            Invoke("ResetSittingFlag", 0.5f);
        }
        
    }

    // Check collision types
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("bot") || other.CompareTag("Manager"))
        {
            _isColliding = true;
            
            _collidingBot = other.gameObject;
        } else if (other.CompareTag("Desk"))
        {
            _isCollidingWithDesk = true;
        } else if (other.CompareTag("Arcade"))
        {
            _isCollidingVideoGame = true;
            _gameOwnerName = other.gameObject.name;
        } else if (other.CompareTag("Offices"))
        {
            _isInOffice = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("bot") || other.CompareTag("Manager"))
        {
            _isColliding = false;
        } else if (other.CompareTag("Desk"))
        {
            _isCollidingWithDesk = false;
        } else if (other.CompareTag("Offices"))
        {
            _isInOffice = false;
        }
    }

    // Activate dialogue manager if socialization is selected
    void Socialize(GameObject collidingBot)
    {
        if (_socializingFlag == false)
        {
            if (collidingBot == null)
            {
                Debug.Log("Attempting to talk to null bot");
            } else
            {
                DialogueManager dm = FindObjectOfType<DialogueManager>();
                dm.InitializeDialogue(collidingBot.GetComponent<BotController>().GetCharacterName(), collidingBot.CompareTag("Manager"));
            }
   
            _socializingFlag = true;

            Invoke("ResetSocializingFlag", 5);
        }

    }
}
