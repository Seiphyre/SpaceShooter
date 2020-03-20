using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField]
    private TabGroup _tabGroup;

    [SerializeField]
    private GameObject _content;

    [SerializeField]
    private bool _isDefaultTab = false;

    [HideInInspector]
    public Image Background;

    public GameObject Content { get { return _content; } }
    public bool IsDefaultTab { get { return _isDefaultTab; } }

    private void Awake()
    {
        Background = GetComponent<Image>();
        _tabGroup.Suscribe(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _tabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tabGroup.OnTabExit(this);
    }
}
