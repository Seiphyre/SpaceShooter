using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Text.RegularExpressions;

[RequireComponent(typeof(NumberIncreaseTextAnim))]
[RequireComponent(typeof(TextDigitFormat))]
[RequireComponent(typeof(ExtraDecimalColorEffect))]
public class DisplayGameMoney : MonoBehaviour
{
    // ----- [ Attributes ] -----------------------------------------------------

    private Player _player;

    private NumberIncreaseTextAnim _textAnim;

    private TextMeshProUGUI _textMesh;

    private TextDigitFormat _textFormat;

    // ----- [ Functions ] -----------------------------------------------------

    // --v-- Start/Awake --v--
    private void Awake()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
        _textAnim = GetComponent<NumberIncreaseTextAnim>();
        _textFormat = GetComponent<TextDigitFormat>();
    }

    private void Start()
    {
        GameManager.Instance.OnGameStartBegin += StartDisplaying;
        GameManager.Instance.OnGameOverBegin += StopDisplaying;
        GameManager.Instance.OnGameRestart += StopDisplaying;
    }

    private void StartDisplaying()
    {
        Player player = GameManager.Instance.Player;

        if (player != null)
        {
            _player = player;

            _textMesh.text = String.Format(_textFormat.Format, _player.Crystals);
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
        _textAnim.Play(amount);
    }

    // --v-- Destroy --v--
    private void OnDestroy()
    {
        // Clear Events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStartBegin -= StartDisplaying;
            GameManager.Instance.OnGameOverBegin -= StopDisplaying;
            GameManager.Instance.OnGameRestart -= StopDisplaying;
        }

        if (_player != null)
            _player.OnGatherCrystals -= UpdateAndDisplayMoney;
    }
}
