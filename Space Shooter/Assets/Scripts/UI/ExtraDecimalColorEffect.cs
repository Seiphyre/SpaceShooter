using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class ExtraDecimalColorEffect : MonoBehaviour
{
    [SerializeField]
    private Color EffectColor = Color.gray;

    private TextMeshProUGUI _textMesh;

    private void Awake()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        string htmlRegex = "<.*?>";
        string textWithoutHTML = _textMesh.text;

        textWithoutHTML = Regex.Replace(_textMesh.text, htmlRegex, string.Empty);

        _textMesh.text = ApplyColorEffect(textWithoutHTML);
    }

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
