using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// CharacterCustomization
// File to handle Character Customization view. Allows you to choose
// Between two different players with different abilities, and between 5 outfits.
public class CharacterCustomization : MonoBehaviour
{
    // fields for game objects and game manager
    private GameObject _player;
    private GameObject _playerInstance;
    private GameManager _gameManager;

    // UI elements
    public Button ChangeCharacter;
    public Button ChangeOutfit;
    public Text Instructions;

    // set game manager on awake
    void Awake()
    {
        _gameManager = GameManager.Instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        // set the player fields. change the text accordingly.
        _player = _gameManager.GetCurrentPlayer();
        _playerInstance = Instantiate(_player);

        UpdateCharacterButtonText();
        UpdateOutfitButtonText();

        // begin with some alerts
        _gameManager.CharacterCustomizationIntroductionAlerts();

        
    }

    // Update is called once per frame
    void Update()
    {
        // Always update the text for character text.
        Instructions.text = GameManager.Instance.GetCharacterText();
    }

    // Change the text on the buttons depending on the current character and button
    void UpdateCharacterButtonText()
    {
        ChangeCharacter.transform.Find("CharacterText").GetComponent<Text>().text = _player.GetComponent<PlayerController>().Name;
        UpdateOutfitButtonText();
    }

    void UpdateOutfitButtonText()
    {
        ChangeOutfit.transform.Find("OutfitText").GetComponent<Text>().text = _player.GetComponent<PlayerController>().OutfitName;
    }

    // Change outfits.
    public void ToggleOutfit()
    {
        Destroy(_playerInstance);
        _player = GameManager.Instance.ToggleOutfit();

        UpdateOutfitButtonText();

        _playerInstance = Instantiate(_player);
    }

    // Change characters
    public void ToggleCharacter()
    {
        Destroy(_playerInstance);
        _player = GameManager.Instance.ToggleCharacters();

        UpdateCharacterButtonText();

        _playerInstance = Instantiate(_player);
    }

    // Confirm player choice and start game.
    public void ConfirmSelection()
    {
        _gameManager.GoToGameScene();
    }
}
