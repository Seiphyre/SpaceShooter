using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EntityDestructionContext
{
    DESTROYED_BY_OTHER,
    SELF_DESTROYED,
    DESTROYED_BY_DAMAGE
}

public abstract class ADamagableEntity : AEntity
{

    // ----- [ Attributes ] -------------------------------------------

    [Header("DamagableEntity")]

    [SerializeField]
    private Color _highLife = Color.green;
    [SerializeField]
    private Color _midLife = new Color(1, 0.65f, 0); // Orange
    [SerializeField]
    private Color _lowLife = Color.red;

    [SerializeField]
    private Transform _explosionPrefab;

    protected ModifiableUpgradableValue<int> _maxLife;

    protected int _currentLife;

    private MeshRenderer _mr;


    // --v-- Events --v--

    public event Action<EntityDestructionContext> OnDestruction;
    public event Action OnTakeDamage;

    // ----- [ Getter / Setters ] -------------------------------------

    public int Life { get { return (_currentLife); } }

    public ReadOnlyUpgradableValue<int> MaxLife { get { return _maxLife.AsReadOnly(); } }

    // ----- [ Functions ] ---------------------------------------------

    // --v-- Unity Messages --v--

    protected override void Awake()
    {
        base.Awake();

        _mr = GetComponent<MeshRenderer>();
        _maxLife = new ModifiableUpgradableValue<int>(1);
        _currentLife = _maxLife.Value;
    }

    protected virtual void Start()
    {

    }

    // --v-- Max Life Upgrade --v--

    public void UpgradeMaxLife()
    {
        bool upgradeSucceeded;

        upgradeSucceeded = _maxLife.Upgrade();

        if (upgradeSucceeded)
            _currentLife++;
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

            DestructionByDamage();
        }
        else // Damage Animation
        {
            StartCoroutine(BlinkCoroutine(0.5f));
        }
    }

    protected virtual void SelfDestruction()
    {
        if (OnDestruction != null)
            OnDestruction.Invoke(EntityDestructionContext.SELF_DESTROYED);

        Destruct();
    }

    protected virtual void DestructionByDamage()
    {
        if (OnDestruction != null)
            OnDestruction.Invoke(EntityDestructionContext.DESTROYED_BY_DAMAGE);

        Destruct();
    }

    protected virtual void Destruct()
    {
        Destroy(gameObject);
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

        float step = _maxLife.Value / 3f;

        if (Utils.IsValueInRange(_currentLife, (step * 2) + 0.1f, _maxLife.Value) && _mr.material.color != _highLife)
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

    private void OnDestroy()
    {
        if (OnDestruction != null)
            OnDestruction.Invoke(EntityDestructionContext.DESTROYED_BY_OTHER);

        Destruct();
    }
}
