using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Dialogue
// An entity class that contains properties needed for Dialogue to appear.
// C# Properties as required.
[System.Serializable]
public class Dialogue
{
    // properties.
    public string DialogueName;
    public string PlayerMessage;
    public string SuccessResponse;
    public string ErrorInsultResponse;
    public string FailureResponse;
    public int InfluenceChange;
    public float MultiplierChange;
    public int Price;
    public int BotInfluenceDamage;
}
