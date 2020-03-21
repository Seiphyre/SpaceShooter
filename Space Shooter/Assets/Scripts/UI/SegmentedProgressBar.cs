using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SegmentedProgressBar : MonoBehaviour
{

    // -- Attributes  --------------------------------------------------------

    [SerializeField]
    [Tooltip("This value should be equal to Segment raw image rectUV.W")]
    private int _nbrOfSegments;

    [SerializeField]
    private Image _segmentMask;

    [SerializeField]
    private Image _segmentFilter;

    private int _value;

    private int _maxValue;



    // -- Getters / Setters  --------------------------------------------------

    public int Value { get { return _value; } }
    public int MaxValue { get { return _maxValue; } }



    // -- Functions -----------------------------------------------------------

    private void Awake()
    {
        Init(_nbrOfSegments);
        SetValue(0);
    }

    public void Init(int maxValue)
    {
        maxValue = Mathf.Clamp(maxValue, 0, _nbrOfSegments);
        _maxValue = maxValue;

        _segmentMask.fillAmount = (float)_maxValue / (float)_nbrOfSegments;
    }

    public void SetValue(int value)
    {
        value = Mathf.Clamp(value, 0, _maxValue);
        _value = value;

        _segmentFilter.fillAmount = 1f - _segmentMask.fillAmount + ((float)(_maxValue - _value) / (float)_nbrOfSegments);
    }

    public void AddValue(int amount = 1)
    {
        SetValue(_value + amount);
    }

    public void SubstractValue(int amount = 1)
    {
        SetValue(_value - amount);
    }
}
