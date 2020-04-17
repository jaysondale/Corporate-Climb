using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// External Resource View Controller
// This handles the screen where you hire consultants.
public class ExternalResourceViewController : ComputerViewController
{

    // variables for text
    private string[] _consultantText = new string[]
    {
        "Message From Henry: Hey, I miss you! It's always fun workin' with ya. You know the drill. " +
        "I'm an enigneer and I get the job done. Not always the most exciting work, " +
        "but it'll do. It's against policy, but I know what you want. " +
        "I don't mind doing your work for you... for the right price. $200 and we'll get started." +
        "\nLow Risk, Low Reward.",

        "Message From Jim: Hey! Remember me? As an astrophysist, I offer quality work that's rarely ever (maybe sometimes) " +
        "a little eccentric. Your job will either love me or hate me, but I don't really care. " +
        "I would offer you the standard free consult, but I won't even humour you. $250 and we can give it a go." +
        "\nModerate risk, Moderate reward.",

        "Message From Piper: Hi! Yay! I get to work with you again. I'm young but I'm smart - " +
        "plus I like to think I'm cute. Am I cute enough for $300 this time? I'll do your work better than everyone else!" +
        "\nHigh risk, High reward.",

        "Welcome to QuantBit's External Resource system. You have three new messages from your contacts. " +
        "Hover over a button to view, and click to get a consult. REMINDER: QuantBit does NOT condone the use " +
        "of paid consults. But, in all honesty, we don't really know how to enforce that. It's 100% your money, " + 
        "and 100% not my problem. "

    };

    private string _messageString;

    // UI Element
    public Text MessageText;
    public Text titleText;

    public GameObject henryImage;
    public GameObject jimImage;
    public GameObject piperImage;

    public Button cancelButton;
    public Button henryButton;
    public Button jimButton;
    public Button piperButton;

    // Set the message to the default. Init the game manager.
    protected override void Awake()
    {
        base.Awake();
        _messageString = _consultantText[3];
    }
    // Keep the text updated
    protected override void Update()
    {
        base.Update();
        MessageText.text = _messageString;
    }

    // Set the appropriate text when hovering over the contractor buttoms
    public void HenryText()
    {
        _messageString = _consultantText[0];
    }

    public void JimText()
    {
        _messageString = _consultantText[1];
    }

    public void PiperText()
    {
        _messageString = _consultantText[2];
    }

    public void DefaultText() {
        _messageString = _consultantText[3];
    }

    // Hire the correct consultant on click.
    public void HireHenry()
    {
        _gameManager.GoToGameScene();
        if (_gameManager.GetCash() >= 200)
        {
            // deduct fees and create an outcome.
            _gameManager.IncrementCash(-200);

            float chances = Random.Range(0.0f, 1.0f);

            if (chances < .15)
            {
                _gameManager.IncrementInfluence(20);
                _gameManager.IncrementMultiplierByFactor(1.3f);
                _gameManager.GlobalAlert("Wow, impressive! Henry's work really paid off for you.");
            }
            else if (chances < .85)
            {
                _gameManager.IncrementInfluence(10);
                _gameManager.IncrementMultiplierByFactor(1.2f);
                _gameManager.GlobalAlert("Henry performed as expected. Nice!");
            }
            else
            {
                _gameManager.IncrementInfluence(3);
                _gameManager.IncrementMultiplierByFactor(1.05f);
                _gameManager.GlobalAlert("Henry didn't do so well for you, but still better than nothing.");
            }

        }
        else // if you can't afford him, you lose some influence for even trying. 
        {
            _gameManager.GlobalAlert("You didn't have enough cash to hire Henry. He told you to buzz off until you have enough money.");
            _gameManager.IncrementInfluence(-1);
        }
    }

    public void HireJim()
    {
        _gameManager.GoToGameScene();
        // deduct fees and create an outcome.
        if (_gameManager.GetCash() >= 250)
        {
            _gameManager.IncrementCash(-250);

            float chances = Random.Range(0.0f, 1.0f);

            if (chances < .35)
            {
                _gameManager.IncrementInfluence(25);
                _gameManager.IncrementMultiplierByFactor(1.4f);
                _gameManager.GlobalAlert("Jim was a rockstar on this one. It wasn't rocket science, but Jim still crushed it!");
            }
            else if (chances < .65)
            {
                _gameManager.IncrementInfluence(12);
                _gameManager.IncrementMultiplierByFactor(1.2f);
                _gameManager.GlobalAlert("Jim performed as expected - pretty nice!");
            }
            else
            {
                _gameManager.IncrementInfluence(-5);
                _gameManager.IncrementMultiplierByFactor(.9f);
                _gameManager.GlobalAlert("Jim didn't do so well for you. His work was a little bit 'much'");
            }

        }
        else // if you can't afford him, you lose some influence for even trying. 
        {
            _gameManager.GlobalAlert("You didn't have enough cash to hire Jim. He told you to fly into the sun.");
            _gameManager.IncrementInfluence(-1);
        }
    }

    public void HirePiper()
    {
        _gameManager.GoToGameScene();// deduct fees and create an outcome.
        if (_gameManager.GetCash() >= 300)
        {
            _gameManager.IncrementCash(-300);

            float chances = Random.Range(0.0f, 1.0f);

            if (chances < .45)
            {
                _gameManager.IncrementInfluence(40);
                _gameManager.IncrementMultiplierByFactor(1.6f);
                _gameManager.GlobalAlert("Piper killed it! Love that wiz kid.");
            }
            else if (chances < .55)
            {
                _gameManager.IncrementInfluence(15);
                _gameManager.IncrementMultiplierByFactor(1.2f);
                _gameManager.GlobalAlert("Pretty rare, but Piper was pretty run of the mill. Not bad.");
            }
            else
            {
                _gameManager.IncrementInfluence(-15);
                _gameManager.IncrementMultiplierByFactor(.8f);
                _gameManager.GlobalAlert("The work really wasn't good. Maybe Piper isn't that smart after all.");
            }
        }
        else
        {
            _gameManager.GlobalAlert("You didn't have enough cash to hire Piper. He cried because you tempted him with candy money, then ripped it away.");// if you can't afford him, you lose some influence for even trying. 
            _gameManager.IncrementInfluence(-1);
        }
    }

    // resize when needed
    protected override void ResizeUIComponents()
    {
        base.ResizeUIComponents();

        ResizeText(titleText, _resolution);
        ResizeText(MessageText, _resolution);

        ResizeGameObject(henryImage, _resolution);
        ResizeGameObject(jimImage, _resolution);
        ResizeGameObject(piperImage, _resolution);

        ResizeButton(cancelButton, _resolution);
        ResizeButton(henryButton, _resolution);
        ResizeButton(jimButton, _resolution);
        ResizeButton(piperButton, _resolution);
    }



}
