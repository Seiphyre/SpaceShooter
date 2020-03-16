using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Text.RegularExpressions;

public class DisplayGameMoney : MonoBehaviour
{
    // ----- [ Attributes ] -----------------------------------------------------

    private Player _player;

    private TextMeshProUGUI _textMesh;

    private int _amountOfMoneyAfterAnim;

    private int amountOfMoneyToAddInAnim;

    private Coroutine AnimCoroutine = null;



    // ----- [ Functions ] -----------------------------------------------------

    // --v-- Start/Awake --v--
    private void Awake()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        GameManager.Instance.OnGameStartBegin += StartDisplaying;
        GameManager.Instance.OnGameOver += StopDisplaying;
    }

    private void StartDisplaying()
    {
        Player player = GameManager.Instance.Player;

        if (player != null)
        {
            _player = player;

            UpdateAndDisplayMoney();
            _player.OnGatherCrystals += UpdateAndDisplayMoney;
        }
    }

    private void StopDisplaying()
    {
        Player player = GameManager.Instance.Player;

        if (player != null && player == _player)
            player.OnGatherCrystals -= UpdateAndDisplayMoney;
    }

    private void UpdateAndDisplayMoney(int amount = 0)
    {
        if (amount != 0)
        {
            int extraAmountOfMoney = 0;
            if (AnimCoroutine != null)
            {
                StopCoroutine(AnimCoroutine);

                int currentAmountOfMoney = ParseNumber(_textMesh.text);
                extraAmountOfMoney = _amountOfMoneyAfterAnim - currentAmountOfMoney;
            }

            AnimCoroutine = StartCoroutine(AddMoneyAnim((amount * Crystal.Value) + extraAmountOfMoney));
        }
        else
            _textMesh.text = String.Format("{0:0000}", _player.Crystals);
    }

    // --v-- Animation --v--

    private IEnumerator AddMoneyAnim(int amountOfMoney)
    {
        float t = 0;
        float duration = 1f;

        int amountOfMoneyBeforeAnim = ParseNumber(_textMesh.text);

        _amountOfMoneyAfterAnim = amountOfMoneyBeforeAnim + amountOfMoney;

        while (t <= 1)
        {
            float moneyLerp = Mathf.Lerp(amountOfMoneyBeforeAnim, _amountOfMoneyAfterAnim, t);
            _textMesh.text = String.Format("{0:0000}", Mathf.Floor(moneyLerp));

            t += Time.deltaTime / duration;

            yield return null;
        }

        _textMesh.text = String.Format("{0:0000}", Mathf.Floor(_amountOfMoneyAfterAnim));
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
        // Clear Events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStartBegin -= StartDisplaying;
            GameManager.Instance.OnGameOver -= StopDisplaying;
        }

        if (_player != null)
            _player.OnGatherCrystals -= UpdateAndDisplayMoney;
    }
}
