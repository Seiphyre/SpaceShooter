using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class ExtraDecimalColorEffect : MonoBehaviour
{
    // ----- [ Attributes ] -----------------------------------------------------

    [SerializeField]
    private Color EffectColor = Color.gray;

    private TextMeshProUGUI _textMesh;



    // ----- [ Functions ] -----------------------------------------------------

    // --v-- Start/Awake --v-- 

    private void Awake()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
    }

    // --v-- Update/LateUpdate --v--
    private void LateUpdate()
    {
        // Removing the HTML from the text
        string htmlRegex = "<.*?>";
        string textWithoutHTML = _textMesh.text;

        textWithoutHTML = Regex.Replace(_textMesh.text, htmlRegex, string.Empty);

        // Apply the color effect to the text
        _textMesh.text = ApplyTextEffect(textWithoutHTML);
    }

    // --v-- Text modifying --v--

    private string ApplyTextEffect(string text)
    {
        string result = "";
        bool shouldApplyColorEffect = true;

        int i = 0;
        while (i < text.Length)
        {
            if (char.IsDigit(text, i) && !text[i].Equals('0') && shouldApplyColorEffect)
                shouldApplyColorEffect = false;

            if (shouldApplyColorEffect)
            {
                result += "<color=#" + ColorUtility.ToHtmlStringRGB(EffectColor) + ">";
                result += text[i];
                result += "</color>";
            }
            else
                result += text[i];

            i++;
        }

        return result;
    }
}
