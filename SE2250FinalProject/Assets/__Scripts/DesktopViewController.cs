using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// DesktopViewController
// Handles the computer menu screen./

public class DesktopViewController : ComputerViewController
{

    
    // Some description texts for the menu
    private string[] _descriptionTexts = new string[] {
        "Welcome to your terminal. Please select a function.",
        "Quantam Bit has a variety of external consultants you can access. " +
        "Feel free to chat with these resources and use them to your advantage!",
        "Access your desktop and all of your completed work. " +
        "Please note a numerical PIN number will be required."
    };

    //UI Fields
    public Text DescriptionText;

    public Text TitleText;

    public Button StealButton;

    public Button helpButton;

    public Button cancelButton;

    // Updates text and checks permissions.
    // Hans cannot steal as he has more speaking abilities (as required)
    protected override void Awake()
    {
        base.Awake();
        DefaultText();

        TitleText.text = _gameManager.GetTerminalOwnerName() + "'s Terminal";

        // If at level 2, steal button becomes active for hans too
        if (_gameManager.GetIsHans() && _gameManager.GetLevel() == 1)
        {
            StealButton.gameObject.SetActive(false);
        }
    }

    // Update text
    public void DesktopText()
    {
        Debug.Log("Desktop Text");
        DescriptionText.text = _descriptionTexts[2];
    }

    public void ExternalContactText()
    {
        DescriptionText.text = _descriptionTexts[1];
    }

    public void DefaultText()
    {
        DescriptionText.text = _descriptionTexts[0];
    }

    // resizes as needed
    protected override void ResizeUIComponents()
    {
        base.ResizeUIComponents();
        ResizeText(DescriptionText, _resolution);
        ResizeText(TitleText, _resolution);
        ResizeButton(StealButton, _resolution );
        ResizeButton(helpButton, _resolution);
        ResizeButton(cancelButton, _resolution);

    }



    // Go to Appropriate Scenes.
    public void ToDesktop()
    {
        _gameManager.GoToDesktopScene();
    }

    public void ToResources()
    {
        _gameManager.GoToExternalConnectionsScene();
    }

    
}
