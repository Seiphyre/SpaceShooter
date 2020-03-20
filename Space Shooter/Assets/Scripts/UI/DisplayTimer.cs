using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DisplayTimer : MonoBehaviour
{
    // ----- [ Attributes ] -----------------------------------------------------
    private TextMeshProUGUI _textMesh;

    private bool _isUpdateEnabled = false;



    // ----- [ Functions ] -----------------------------------------------------

    // --v-- Start/Awake --v-- 

    private void Awake()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        GameManager.Instance.OnGameStart += HandleOnGameStartEnd;
        GameManager.Instance.OnGameStop += HandleOnGameOverBegin;
    }

    // --v-- Update --v--

    private void Update()
    {
        if (_isUpdateEnabled)
        {
            float time = GameManager.Instance.GameTime;

            _textMesh.text = String.Format("{0:00}", Mathf.Floor(time / 3600f));
            _textMesh.text += ":";
            _textMesh.text += String.Format("{0:00}", Mathf.Floor(time / 60f));
            _textMesh.text += ":";
            _textMesh.text += String.Format("{0:00}", Mathf.Floor(time % 60));
        }
    }

    // --v-- Managing update --v--

    private void EnableUpdate()
    {
        _isUpdateEnabled = true;
    }

    private void DisableUpdate()
    {
        _isUpdateEnabled = false;
    }

    // --v-- Event Handler --v--

    private void HandleOnGameOverBegin()
    {
        DisableUpdate();
    }

    private void HandleOnGameStartEnd()
    {
        EnableUpdate();
    }

    // --v-- Destroy --v--
    private void OnDestroy()
    {
        // Clear Events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStart -= HandleOnGameStartEnd;
            GameManager.Instance.OnGameStop -= HandleOnGameOverBegin;
        }
    }
}
