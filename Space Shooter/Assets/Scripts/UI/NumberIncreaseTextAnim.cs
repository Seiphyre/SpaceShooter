using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Text.RegularExpressions;

[RequireComponent(typeof(TextMeshProUGUI))]
public class NumberIncreaseTextAnim : MonoBehaviour
{
    // ----- [ Attributes ] -----------------------------------------------------

    [SerializeField]
    private float _duration = 1f;

    private TextMeshProUGUI _textMesh;

    private TextDigitFormat _textFormat;

    private int _amountAfterAnim;

    private Coroutine AnimCoroutine = null;

    // ----- [ Functions ] -----------------------------------------------------

    // --v-- Start/Awake --v--

    private void Awake()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
        _textFormat = GetComponent<TextDigitFormat>();

        _amountAfterAnim = 0;
    }

    // --v-- Animation Management --v--
    public void Play(int amount)
    {
        if (amount <= 0) return;

        int extraAmount = 0;
        if (AnimCoroutine != null)
        {
            int currentAmount = ParseNumber(_textMesh.text);
            extraAmount = _amountAfterAnim - currentAmount;

            StopCoroutine(AnimCoroutine);
            AnimCoroutine = null;
        }

        AnimCoroutine = StartCoroutine(PlayCoroutine(amount + extraAmount));
    }

    private IEnumerator PlayCoroutine(int newAmount)
    {
        float t = 0;

        int amountBeforeAnim = ParseNumber(_textMesh.text);
        _amountAfterAnim = amountBeforeAnim + newAmount;

        while (t <= 1)
        {
            float amountLerp = Mathf.Lerp(amountBeforeAnim, _amountAfterAnim, t);
            _textMesh.text = NumberToFormatString(Mathf.FloorToInt(amountLerp));

            t += Time.deltaTime / _duration;

            yield return null;
        }

        _textMesh.text = NumberToFormatString(Mathf.FloorToInt(_amountAfterAnim));

        AnimCoroutine = null;
        _amountAfterAnim = 0;
    }

    private string NumberToFormatString(int nbr)
    {
        if (_textFormat != null)
            return String.Format(_textFormat.Format, nbr);

        return nbr.ToString();
    }

    private int ParseNumber(string text)
    {
        string htmlRegex = "<.*?>";
        string textWithoutHTML = text;

        textWithoutHTML = Regex.Replace(text, htmlRegex, string.Empty);

        return Int32.Parse(textWithoutHTML);
    }

    // --v-- Destroy --v--
    private void OnDestroy()
    {
        if (AnimCoroutine != null)
            StopCoroutine(AnimCoroutine);
    }
}
