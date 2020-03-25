using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public abstract class UpgradableValue<T>
{
    protected T _defaultValue;

    protected List<BuyableValue<T>> _upgradedValues;

    protected int _levelIndex;

    // -- Getters / Setters --------------------------------------------------

    public T Value
    {
        get
        {
            if (_levelIndex == -1)
                return _defaultValue;

            else if (_upgradedValues != null && _levelIndex < _upgradedValues.Count)
                return _upgradedValues[_levelIndex].Value;

            return _upgradedValues[_upgradedValues.Count - 1].Value;
        }
    }

    public int NextValuePrice
    {
        get
        {
            int nextLevelIndex = _levelIndex + 1;

            if (_upgradedValues != null && nextLevelIndex < _upgradedValues.Count)
                return _upgradedValues[nextLevelIndex].Price;

            return -1;
        }
    }

    public int LevelIndex { get { return _levelIndex; } }

    public ReadOnlyCollection<BuyableValue<T>> Values { get { return _upgradedValues.AsReadOnly(); } }

    // -- Functions ----------------------------------------------------------

    protected ReadOnlyUpgradableValue<T> CreateReadOnlyUpgradableValue(UpgradableValue<T> other)
    {
        ReadOnlyUpgradableValue<T> result = new ReadOnlyUpgradableValue<T>();

        result._defaultValue = other._defaultValue;
        result._upgradedValues = other._upgradedValues;
        result._levelIndex = other._levelIndex;

        return result;
    }
}

public sealed class ReadOnlyUpgradableValue<T> : UpgradableValue<T>
{
    public ReadOnlyUpgradableValue()
    {
        _defaultValue = default(T);
        _upgradedValues = null;
        _levelIndex = -1;
    }

    public ReadOnlyUpgradableValue(T value)
    {
        _defaultValue = value;
        _upgradedValues = null;
        _levelIndex = -1;
    }
    public ReadOnlyUpgradableValue(T defaultValue, List<BuyableValue<T>> upgradedValues, int level = 0)
    {
        _defaultValue = defaultValue;

        if (upgradedValues == null || upgradedValues.Count == 0)
        {
            _levelIndex = -1;
            _upgradedValues = null;
        }
        else
        {
            _upgradedValues = new List<BuyableValue<T>>(upgradedValues);
            _levelIndex = Mathf.Clamp(level - 1, -1, _upgradedValues.Count - 1);
        }
    }
}

public sealed class ModifiableUpgradableValue<T> : UpgradableValue<T>
{

    public new int LevelIndex { get { return base.LevelIndex; } set { _levelIndex = value; } }

    public ModifiableUpgradableValue(T value)
    {
        _defaultValue = value;
        _upgradedValues = null;
        _levelIndex = -1;
    }
    public ModifiableUpgradableValue(T defaultValue, List<BuyableValue<T>> upgradedValues, int level = 0)
    {
        _defaultValue = defaultValue;

        if (upgradedValues == null || upgradedValues.Count == 0)
        {
            _levelIndex = -1;
            _upgradedValues = null;
        }
        else
        {
            _upgradedValues = new List<BuyableValue<T>>(upgradedValues);
            _levelIndex = Mathf.Clamp(level - 1, -1, _upgradedValues.Count - 1);
        }
    }


    // --v-- Other --v--

    public bool Upgrade()
    {
        if (_levelIndex >= _upgradedValues.Count - 1) return false;

        LevelIndex += 1;
        return true;
    }

    public bool Downgrade()
    {
        if (_levelIndex <= 0) return false;

        LevelIndex -= 1;
        return true;
    }

    public void Reset()
    {
        LevelIndex = 0;
    }

    public ReadOnlyUpgradableValue<T> AsReadOnly()
    {
        return CreateReadOnlyUpgradableValue(this);
    }
}

public struct BuyableValue<T>
{
    public T Value;
    public int Price;

    public BuyableValue(T value, int price)
    {
        Value = value;
        Price = price;
    }
}