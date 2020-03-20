using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayPlayerLife : MonoBehaviour
{

    // ----- [ Attributes ] -----------------------------------------------------
    private Player _player;

    private TextMeshProUGUI _textMesh;



    // ----- [ Functions ] -----------------------------------------------------

    // --v-- Start/Awake --v-- 

    private void Awake()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        GameManager.Instance.OnGameInit += HandleOnGameStartBegin;
        GameManager.Instance.OnGameStop += HandleOnGameOverBegin;
    }

    private void StartDisplaying()
    {
        Player player = GameManager.Instance.Player;

        if (player != null)
        {
            _player = player;

            UpdateAndDisplayLife();
            _player.OnTakeDamage += UpdateAndDisplayLife;
        }
    }

    private void StopDisplaying()
    {
        Player player = GameManager.Instance.Player;

        if (player != null && player == _player)
            player.OnTakeDamage -= UpdateAndDisplayLife;
    }

    // --v-- Event Handler --v--

    private void HandleOnGameOverBegin()
    {
        StopDisplaying();
    }

    private void HandleOnGameStartBegin()
    {
        StartDisplaying();
    }

    // --v-- Display Life --v--

    private void UpdateAndDisplayLife()
    {
        _textMesh.text = _player.Life.ToString();
    }

    // --v-- Destroy --v--
    private void OnDestroy()
    {
        // Clear Events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameInit -= HandleOnGameStartBegin;
            GameManager.Instance.OnGameStop -= HandleOnGameOverBegin;
        }

        if (_player != null)
            _player.OnTakeDamage -= UpdateAndDisplayLife;
    }
}
