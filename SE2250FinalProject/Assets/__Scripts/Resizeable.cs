using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This class deals with UI elements that are supposed to scale with the screen. 
public abstract class ResizeableView: MonoBehaviour
{
    // this is the resolution in which the scene was designed.
    // the default is the default size of the scene on Macbook Pro 15",
    // but can be changed in any child class.
    protected Vector2 HARDCODED_RESOLUTION = new Vector2(1530f, 765f); 

    // Resizes any text objects. Resolution is the old resolution from which it was rendered.
    protected void ResizeText(Text obj, Vector2 resolution)
    {
        RectTransform oldTransform = obj.rectTransform;

        // change the position and scale based on old resolution and new resolution.
        oldTransform.anchoredPosition = new Vector3(oldTransform.anchoredPosition.x / resolution.x * Screen.width, oldTransform.anchoredPosition.y / resolution.y * Screen.height, oldTransform.position.z);
        oldTransform.localScale = new Vector3(Screen.width / HARDCODED_RESOLUTION.x, Screen.height / HARDCODED_RESOLUTION.y, 1f);

        obj.GetComponent<RectTransform>().anchoredPosition = oldTransform.anchoredPosition;
        obj.GetComponent<RectTransform>().localScale = oldTransform.localScale;
    }
    // Resizes any button objects. Resolution is the old resolution from which it was rendered.
    protected void ResizeButton(Button obj, Vector2 resolution)
    {
        RectTransform oldTransform = obj.GetComponent<RectTransform>();
        // change the position and scale based on old resolution and new resolution.
        oldTransform.anchoredPosition = new Vector3(oldTransform.anchoredPosition.x / resolution.x * Screen.width, oldTransform.anchoredPosition.y / resolution.y * Screen.height, oldTransform.position.z);
        oldTransform.localScale = new Vector3(Screen.width / HARDCODED_RESOLUTION.x, Screen.height / HARDCODED_RESOLUTION.y, 1f);

        obj.GetComponent<RectTransform>().anchoredPosition = oldTransform.anchoredPosition;
        obj.GetComponent<RectTransform>().localScale = oldTransform.localScale;
    }
    // Resizes any game objects. Resolution is the old resolution from which it was rendered.
    protected void ResizeGameObject(GameObject obj, Vector2 resolution)
    {
        RectTransform oldTransform = obj.GetComponent<RectTransform>();
        // change the position and scale based on old resolution and new resolution.
        oldTransform.anchoredPosition = new Vector3(oldTransform.anchoredPosition.x / resolution.x * Screen.width, oldTransform.anchoredPosition.y / resolution.y * Screen.height, oldTransform.position.z);
        oldTransform.localScale = new Vector3(Screen.width / HARDCODED_RESOLUTION.x, Screen.height / HARDCODED_RESOLUTION.y, 1f);

        obj.GetComponent<RectTransform>().anchoredPosition = oldTransform.anchoredPosition;
        obj.GetComponent<RectTransform>().localScale = oldTransform.localScale;
    }


}
