using UnityEngine;

public class TextGui : MonoBehaviour
{
    private static TextGui instance;

    public static TextGui Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TextGui>();
            }
            return instance;
        }
    }

    private string bottomLeftText = "Q - Previous\nE - Next\n\nMouse Click - Shoot!";
    private string topRightText = "Skill_01_Normal";
    private GUIStyle textStyle;

    private void Start()
    {
        // Initialize the text style
        textStyle = new GUIStyle();
        textStyle.fontSize = 20; // Set the font size to 20
    }

    private void OnGUI()
    {
        // Update the text style color to white
        textStyle.normal.textColor = Color.white;

        // Calculate the position for the bottom left text
        Vector2 bottomLeftTextSize = textStyle.CalcSize(new GUIContent(bottomLeftText));
        float bottomLeftX = 20;
        float bottomLeftY = Screen.height - bottomLeftTextSize.y - 20;

        // Calculate the position for the top right text
        Vector2 topRightTextSize = textStyle.CalcSize(new GUIContent(topRightText));
        float topRightX = Screen.width - topRightTextSize.x - 20;
        float topRightY = 20;

        // Set the text style
        GUI.Label(new Rect(bottomLeftX, bottomLeftY, bottomLeftTextSize.x, bottomLeftTextSize.y), bottomLeftText, textStyle);
        GUI.Label(new Rect(topRightX, topRightY, topRightTextSize.x, topRightTextSize.y), topRightText, textStyle);
    }

    public void SetTopRightText(string _newText)
    {
        topRightText = _newText;
    }
}