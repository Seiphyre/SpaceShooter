using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    [SerializeField]
    private Sprite _tabIdle;
    [SerializeField]
    private Sprite _tabHover;
    [SerializeField]
    private Sprite _tabActive;

    [SerializeField]
    private List<TabButton> _tabButtons;

    [SerializeField]
    private List<GameObject> _pages;

    private TabButton _selectedTab;

    private void Start()
    {
        // Set default tab
        if (_tabButtons != null)
        {
            int index = _tabButtons.FindIndex(button => button.IsDefaultTab == true);
            if (index != -1)
                OnTabSelected(_tabButtons[index]);
        }
    }

    public void Suscribe(TabButton button)
    {
        if (_tabButtons == null)
        {
            _tabButtons = new List<TabButton>();
        }

        _tabButtons.Add(button);
    }

    public void OnTabEnter(TabButton button)
    {
        ResetTabs();

        if (_selectedTab == null || _selectedTab != button)
        {
            button.Background.sprite = _tabHover;
        }
    }

    public void OnTabExit(TabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton button)
    {
        _selectedTab = button;

        ResetTabs();
        button.Background.sprite = _tabActive;

        int index = _pages.FindIndex(page => page.Equals(button.Content));
        for (int i = 0; i < _pages.Count; i++)
        {
            if (i == index)
                _pages[i].SetActive(true);
            else
                _pages[i].SetActive(false);
        }
    }

    private void ResetTabs()
    {
        foreach (TabButton button in _tabButtons)
        {
            if (button != null && button == _selectedTab) continue;

            button.Background.sprite = _tabIdle;
        }
    }
}
