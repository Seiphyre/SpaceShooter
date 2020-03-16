using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ADamagableEntity : AEntity
{

    // ----- [ Attributes ] -------------------------------------------

    [Header("DamagableEntity")]

    [SerializeField]
    private int _maxLife = 1;

    [SerializeField]
    private Color _highLife = Color.green;
    [SerializeField]
    private Color _midLife = new Color(1, 0.65f, 0); // Orange
    [SerializeField]
    private Color _lowLife = Color.red;

    [SerializeField]
    private Transform _explosionPrefab;

    private int _currentLife;

    private MeshRenderer _mr;


    // --v-- Events --v--

    public event Action OnSelfDestroy;
    public event Action OnTakeDamage;

    // ----- [ Getter / Setters ] -------------------------------------

    public int Life { get { return (_currentLife); } }



    // ----- [ Functions ] ---------------------------------------------

    // --v-- Unity Messages --v--

    protected override void Awake()
    {
        base.Awake();

        _mr = GetComponent<MeshRenderer>();
        _currentLife = _maxLife;
    }

    protected virtual void Start()
    {

    }


    // --v-- AEntity Override --v--

    public override void Reset()
    {
        _currentLife = _maxLife;
    }

    // --v-- Damage Management --v--

    public void TakeDamage(int amountOfDamage = 1)
    {
        // Update life
        _currentLife -= amountOfDamage;
        _currentLife = Mathf.Max(0, _currentLife);

        OnTakeDamage?.Invoke();

        // Death
        if (_currentLife == 0)
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

            DestructionByOther();
        }
        else // Damage Animation
        {
            StartCoroutine(BlinkCoroutine(0.5f));
        }
    }

    protected virtual void SelfDestruction()
    {
        if (OnSelfDestroy != null)
            OnSelfDestroy.Invoke();

        Destroy(gameObject);
    }

    protected virtual void DestructionByOther()
    {
        SelfDestruction();
    }

    // protected void DamagePlayer(Player player)
    // {
    //     player.DamageSelf();
    //     DestroySelf();
    // }

    private IEnumerator BlinkCoroutine(float duration)
    {
        Color startColor = Color.white;
        Color blinkColor = Color.white;

        float step = _maxLife / 3f;

        if (Utils.IsValueInRange(_currentLife, (step * 2) + 0.1f, _maxLife) && _mr.material.color != _highLife)
            blinkColor = _highLife;
        else if (Utils.IsValueInRange(_currentLife, step + 0.1f, step * 2) && _mr.material.color != _midLife)
            blinkColor = _midLife;
        else if (Utils.IsValueInRange(_currentLife, 0f, step) && _mr.material.color != _lowLife)
            blinkColor = _lowLife;

        float t = 0;

        while (t <= 1)
        {
            _mr.material.color = Color.Lerp(startColor, blinkColor, t);
            t += Time.deltaTime / (duration / 2);

            yield return null;
        }

        t = 1;

        while (t >= 0)
        {
            _mr.material.color = Color.Lerp(startColor, blinkColor, t);
            t -= Time.deltaTime / (duration / 2);

            yield return null;
        }

    }
}
