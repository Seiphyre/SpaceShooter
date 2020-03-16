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
        _textMesh.text = ApplyColorEffect(textWithoutHTML);
    }

    // --v-- Text modifying --v--

    private string ApplyColorEffect(string nbr)
    {
        string result = "";
        bool shouldApplyColorEffect = true;

        int i = 0;
        while (i < nbr.Length)
        {
            if (char.IsDigit(nbr, i) && !nbr[i].Equals('0') && shouldApplyColorEffect)
                shouldApplyColorEffect = false;

            if (shouldApplyColorEffect)
            {
                result += "<color=#" + ColorUtility.ToHtmlStringRGB(EffectColor) + ">";
                result += nbr[i];
                result += "</color>";
            }
            else
                result += nbr[i];

            i++;
        }

        return result;
    }
}
