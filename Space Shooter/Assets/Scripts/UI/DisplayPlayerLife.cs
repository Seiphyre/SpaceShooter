using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayPlayerLife : MonoBehaviour
{

    private Player _player;

    private TextMeshProUGUI _textMesh;

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

            UpdateDisplayedLife();
            _player.OnTakeDamage += UpdateDisplayedLife;
        }
    }

    private void Reset()
    {
        Player player = GameManager.Instance.Player;

        if (player != null && player == _player)
            player.OnTakeDamage -= UpdateDisplayedLife;
    }

    private void UpdateDisplayedLife()
    {
        _textMesh.text = _player.Life.ToString();
    }

}
