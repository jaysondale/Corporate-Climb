using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
// Dialogue Manager
// This file handles actions related to speaking combat.
public class DialogueManager : InfoBoxController
{
    // List to contain option buttons
    private List<GameObject> _buttons;
    private List<string> _options;
   
    private DialogueScripts _dialogueScripts;
    private Dialogue[] _dialogue;
    private string _dialogueType;

    public GameObject DialogueButton;

    // game manager
    private GameManager _gameManager;

    // variables for the bots involved in dialogue
    private string _bot;

    private string _insultedBot;

    private bool _isManager;

    // store game manager instance
    private void Awake()
    {
        _gameManager = GameManager.Instance;


    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // Initialize button list
        _buttons = new List<GameObject>();

        // Get DialogueScripts
        _dialogueScripts = gameObject.GetComponent<DialogueScripts>();

    }

    // Displays the dialogue box and prompts user to select dialogue options.
    public void InitializeDialogue(string interactingBot, bool isManager)
    {
        // Set bot
        _bot = interactingBot;
        _isManager = isManager;

        // Initialize options list
        _options = new List<string>();

        _options.Add("Compliment");
        if (_isManager)
        {
            _options.Add("Bribe");
        }
        else
        {
            _options.Add("Offer");
        }
        _options.Add("Insult Other");

        // Display box
        ShowBox(false);

        // Reset insulted bot
        _insultedBot = null;

        // Get dialogue type from user
        SetText("What do you want to say to " + _bot + "?");

        // If at level 2, talking becomes available for both
        if (_gameManager.GetIsHans() || _gameManager.GetLevel() == 2)
        {
            CreateButtonArray(_options, PresentDialogues);
            _dialogueType = null;
        }
        else
        {
            _dialogueType = "Compliment";
            PresentDialogues();
        }



    }

    // Presents user with dialogue options associated with the category selection
    void PresentDialogues()
    {
        // Get selected dialogue type
        if (_dialogueType == null)
        {
            GameObject buttonpressed = EventSystem.current.currentSelectedGameObject;
            _dialogueType = buttonpressed.GetComponentInChildren<Text>().text;
        }

        // Get dialogue options
        switch (_dialogueType)
        {
            case "Offer":
                _dialogue = _dialogueScripts.offerDialogues;
                SetText("What do you want to offer?");
                GetComment();
                break;
            case "Bribe":
                _dialogue = _dialogueScripts.bribleDialogues;
                SetText("What do you want to bribe?");
                GetComment();
                break;
            case "Compliment":
                _dialogue = _dialogueScripts.complimentDialogues;
                SetText("What do you want to compliment?");
                GetComment();
                break;
            case "Insult Other":
                _dialogue = _dialogueScripts.insultDialogues;

                // Prompt user to select bot to insult
                SetText("Who do you want to insult?");
                CreateButtonArray(new List<string>(_gameManager.GetBots().Keys), GetInsultedBot);
                break;
            default:
                Debug.LogError("No dialogue initialized");
                return;
        }

        
    }

    // Determines which bot the user chose to insult and assigns it to _insultedBot
    void GetInsultedBot()
    {
        // Get selected dialogue from button
        GameObject buttonpressed = EventSystem.current.currentSelectedGameObject;
        _insultedBot = buttonpressed.GetComponentInChildren<Text>().text;
        SetText("What do you want to insult");
        GetComment();
    }

    // Prompts user to select their specific dialogue choice
    void GetComment()
    {
        List<string> dialogueNames = new List<string>();

        foreach (Dialogue d in _dialogue)
        {
            if (d.Price <= _gameManager.GetCash())
                dialogueNames.Add(d.DialogueName);
        }

        // Create array of button options
        CreateButtonArray(dialogueNames, ActivateDialogue);
    }

    // Creates a series of selection buttons at the bottom the dialogue window
    void CreateButtonArray(List<string> buttonLabels, UnityEngine.Events.UnityAction call)
    {
        // Clear any existing buttons
        ClearButtons();

        // 10 units of button spacing
        int buttonSpacing = Mathf.RoundToInt(DialogueButton.GetComponent<RectTransform>().rect.width) + 40;
        int buttonCount = 0;

        // Get position of buttons
        RectTransform dbRect = _infoBox.GetComponent<RectTransform>();
        int boxHeight = Mathf.RoundToInt(dbRect.rect.height / 4);
        int boxWidth = Mathf.RoundToInt(dbRect.rect.width / 8);

        foreach (string label in buttonLabels)
        {
            // Instantiate new button
            GameObject newButton = Instantiate(DialogueButton);
            newButton.GetComponent<Button>().onClick.AddListener(call);
            Text buttonText = newButton.GetComponentInChildren<Text>();
            buttonText.text = label;
            buttonText.fontSize = 12;
            newButton.transform.SetParent(_infoBox.transform);

            // Position button
            newButton.transform.localPosition = new Vector3(-boxWidth/2 + buttonSpacing * buttonCount++, -boxHeight/2, 0);

            // Add to button list
            _buttons.Add(newButton);
        }

    }

    // Removes all the selection buttons from the bottom the dialogue window
    void ClearButtons()
    {
        foreach (GameObject b in _buttons)
        {
            Destroy(b);
        }
        _buttons.Clear();
    }

    // Presents the desired dialogue to the user and adds/deducts from player resources accordingly
    void ActivateDialogue()
    {
        // Get selected dialogue from button
        GameObject buttonpressed = EventSystem.current.currentSelectedGameObject;
        string buttonName = buttonpressed.GetComponentInChildren<Text>().text;

        Dialogue selectedDialogue = null;

        // Find dialogue
        foreach (Dialogue d in _dialogue)
        {
            if (d.DialogueName == buttonName)
            {
                selectedDialogue = d;
            }
        }

        if (selectedDialogue == null)
        {
            Debug.LogError("No dialogue found that matched button selection!");
            return;
        }

        // Get bot probability of success for selected category
        BotController botController = null;
        if (_isManager)
        {
            foreach (GameObject manager in _gameManager.Managers)
            {
                if (manager.GetComponent<BotController>().GetCharacterName() == _bot)
                {
                    botController = manager.GetComponent<BotController>();
                    break;
                }
            }
        }
        else
        {
            foreach (GameObject bot in _gameManager.Bots)
            {
                if (bot.GetComponent<BotController>().GetCharacterName() == _bot)
                {
                    botController = bot.GetComponent<BotController>();
                    break;
                }
            }
        }

        if (botController == null)
        {
            Debug.LogError("Couldn't find bot object with name: " + _bot);
            return;
        }

        int probOfSuccess = botController.GetProbabilityOfSuccess(_dialogueType);
        if (probOfSuccess < 0)
        {
            Debug.LogError("Invalid type key: " + _dialogueType);
            return;
        }

        // Spend money on selection
        _gameManager.SpendMoney(selectedDialogue.Price);

        // Insult player and/or bot accordingly
        if (_insultedBot != null && _insultedBot == _bot)
        {
            DisplayDialogue(_insultedBot + " " + selectedDialogue.PlayerMessage, selectedDialogue.ErrorInsultResponse);
            _gameManager.IncrementInfluence(-10);
        }
        else if (IsSuccessful(probOfSuccess))
        {
            if (_insultedBot != null)
            {
                _gameManager.IncrementBotInfluence(_insultedBot, -1 * selectedDialogue.BotInfluenceDamage);
                DisplayDialogue(_insultedBot + " " + selectedDialogue.PlayerMessage, selectedDialogue.SuccessResponse);
            } else
            {
                DisplayDialogue(selectedDialogue.PlayerMessage, selectedDialogue.SuccessResponse);
            }
            
            _gameManager.IncrementInfluence(selectedDialogue.InfluenceChange);
            _gameManager.IncrementMultiplier(selectedDialogue.MultiplierChange);
            
        }
        else
        {
            if (_insultedBot != null)
            {
                _gameManager.IncrementBotInfluence(_insultedBot, selectedDialogue.BotInfluenceDamage);
                DisplayDialogue(_insultedBot + " " + selectedDialogue.PlayerMessage, selectedDialogue.FailureResponse);
                _gameManager.IncrementInfluence(-2 * selectedDialogue.InfluenceChange);
            }
            else
            {
                DisplayDialogue(selectedDialogue.PlayerMessage, selectedDialogue.FailureResponse);
                _gameManager.IncrementInfluence(-1 * selectedDialogue.InfluenceChange);
                _gameManager.IncrementMultiplier(-1 * selectedDialogue.MultiplierChange);
            }

            
        }


    }

    // Displayes dialogue text on the screen and shows the exit button after a two-second delay
    void DisplayDialogue(string playerMessage, string botMessage)
    {
        string message = _gameManager.PlayerName + ": " + playerMessage + "\n\n" + _bot + ": " + botMessage;
        ClearButtons();
        SetText(message);
        Invoke("DisplayExitButton", 2);
    }

    // Shows the exit button
    void DisplayExitButton()
    {
        _exitButtonObject.SetActive(true);
    }

    // Returns true/false depending on the propability of success
    bool IsSuccessful(int probSuccess)
    {
        int randInt;
        // If on level 2, prob of success for kira is inversely proportional to the player's influence
        if (_gameManager.GetLevel() == 2 && !_gameManager.GetIsHans())
        {
            randInt = Mathf.RoundToInt(Random.Range(0, 100) / Mathf.Min(0.5f, _gameManager.GetInfluence() / 10));
        }
        else
        {
            randInt = Mathf.RoundToInt(Random.Range(0, 100));
        }
        return randInt <= probSuccess;
    }
}