using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// This script belons to the pong ball in the video game function, and handles collisions.
public class PongBallCollisionHandler : MonoBehaviour
{
    // pong manager instance
    private GameObject _pongManager;
    // flags for collisions
    private bool _wallColliding = false;
    private bool _paddleColliding = false;

    // check collisions and call functions in PongManager as needed
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("paddle") && !_paddleColliding)
        {
            _paddleColliding = true;
            _pongManager.GetComponent<PongManager>().PaddleCollision();
        }
        else if (other.CompareTag("wall") && !_wallColliding)
        {
            Debug.Log("Collision");
            _wallColliding = true;
            _pongManager.GetComponent<PongManager>().WallCollision();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("paddle"))
        {
            _paddleColliding = false;
        }
        else if (other.CompareTag("wall"))
        {
            _wallColliding = false;
        }
    }
    // Set the manager to an instance of pong manager.
    public void SetManager(GameObject pm)
    {
        _pongManager = pm;
    }


}

