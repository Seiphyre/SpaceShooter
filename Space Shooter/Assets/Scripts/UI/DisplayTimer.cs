using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DisplayTimer : MonoBehaviour
{
    private TextMeshProUGUI _textMesh;

    private bool _canUpdate = false;

    private void Awake()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        GameManager.Instance.OnGameStartBegin += StartUpdate;
        GameManager.Instance.OnGameOver += StopUpdate;
    }

    private void StartUpdate()
    {
        _canUpdate = true;
    }

    private void StopUpdate()
    {
        _canUpdate = false;
    }

    private void Update()
    {
        if (_canUpdate)
        {
            float time = GameManager.Instance.GetGameTime();

            //_textMesh.SetText("Time : {0}:{1}:{2:2.2}", Time.time / 3600f, Time.time / 60f, Time.time % 60);
            _textMesh.text = String.Format("{0:00}", Mathf.Floor(time / 3600f));
            _textMesh.text += ":";
            _textMesh.text += String.Format("{0:00}", Mathf.Floor(time / 60f));
            _textMesh.text += ":";
            _textMesh.text += String.Format("{0:00}", Mathf.Floor(time % 60));

            //_textMesh.text = Grey(_textMesh.text);
        }
    }

    private string Grey(string nbr)
    {
        string result = "";
        bool isGrey = true;

        int i = 0;
        while (i < nbr.Length)
        {
            if (char.IsDigit(nbr, i) && !nbr[i].Equals('0') && isGrey) // if (!nbr[i].Equals('0') && isGrey)
                isGrey = false;

            if (isGrey)
            {
                result += "<color=#969696>";
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
