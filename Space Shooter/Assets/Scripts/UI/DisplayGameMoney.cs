using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Text.RegularExpressions;

public class DisplayGameMoney : MonoBehaviour
{
    private Player _player;

    private TextMeshProUGUI _textMesh;

    private int _amountOfMoneyAfterAnim;

    private int amountOfMoneyToAddInAnim;

    private Coroutine Anim = null;

    private void Awake()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        GameManager.Instance.OnGameStartBegin += Init;
        GameManager.Instance.OnGameOver += Reset;
    }

    private void Init()
    {
        Player player = GameManager.Instance.Player;

        if (player != null)
        {
            _player = player;

            UpdateDisplayedMoney();
            _player.OnGatherCrystals += UpdateDisplayedMoney;
        }
    }

    private void Reset()
    {
        Player player = GameManager.Instance.Player;

        if (player != null && player == _player)
            player.OnGatherCrystals -= UpdateDisplayedMoney;
    }

    private void UpdateDisplayedMoney(int amount = 0)
    {
        if (amount != 0)
        {
            //_textMesh.text = String.Format("{0:0000}", _player.Crystals);

            int extraAmountOfMoney = 0;
            if (Anim != null)
            {
                StopCoroutine (Anim);

                int currentAmountOfMoney = ParseNumber(_textMesh.text);
                extraAmountOfMoney = _amountOfMoneyAfterAnim - currentAmountOfMoney;
            }

            Anim = StartCoroutine(AddMoneyAnim((amount * Crystal.Value) + extraAmountOfMoney));
        }
        else
            _textMesh.text = String.Format("{0:0000}", _player.Crystals);
    }

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
}
