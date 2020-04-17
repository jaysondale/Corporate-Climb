using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// DogController
// Controller class used to manage the dog's ability to follow the player's position path
public class DogController : MonoBehaviour
{
    // GameManager instance
    private GameManager _gameManager;
    Animator _animator;
    public float Speed;
    public float DistanceFromPlayer;
    private bool _walking;
    private Vector3 _target;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameManager.Instance;
        _animator = GetComponent<Animator>();
        _walking = true;
        _target = _gameManager.PlayerPositions.Dequeue();
    }

    // Update is called once per frame
    void Update()
    {
        // Check distance to player
        Vector3 playerPosition = _gameManager.GetPlayerObject().transform.position;
        if (Vector3.Distance(new Vector3(playerPosition.x, 1, playerPosition.z), transform.position) <= DistanceFromPlayer)
        {
            _walking = false;
        }
        else
        {
            _walking = true;
        }

        // Once target is reached, move to next target
        Vector3 relativePos = new Vector3(_target.x, 1, _target.z) - transform.position;
        
        if (Vector3.Magnitude(relativePos) <= 0.1)
        {
            if (_gameManager.PlayerPositions.Count == 0)
            {
                _walking = false;
            }
            else
            {
                _target = _gameManager.PlayerPositions.Dequeue();
            }
        }
        
        if (_walking)
        {
            // Rotate 90 degrees to account for misaligned prefab
            Vector3 lookDirection = new Vector3(relativePos.z, relativePos.y, -relativePos.x);
            transform.localRotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lookDirection, Vector3.up), 150 * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(_target.x, 1, _target.z), Speed * Time.deltaTime);
        }
        _animator.SetBool("isWalking", _walking);

    }


    // Check for collision with player
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            _walking = false;
        }
    }

    // Check for exit of player collision
    private void OnTriggerExit(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            _walking = true;
        }
    }

    public void SetWalking(bool state)
    {
        _walking = state;
    }
}
