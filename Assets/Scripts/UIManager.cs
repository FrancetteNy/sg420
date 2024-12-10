using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    VisualElement _root;
    List<UIView> _allUIViews = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        AddAllUIViews();
    }

    private void AddAllUIViews()
    {
        var detailView = new DetailView(_root, this);
        var hudView = new HUDView(_root, this);
        UIEvents.ShowDetailView += (_) => HideAllViews();
        UIEvents.ShowDetailView += (index) => detailView.Show(index);
        UIEvents.HideDetailView += detailView.Hide;
        UIEvents.HideDetailView += hudView.Show;

        UIEvents.ShowHUDView += HideAllViews;
        UIEvents.ShowHUDView += hudView.Show;
        UIEvents.HideHUDView += hudView.Hide;

        _allUIViews.Add(detailView);
        _allUIViews.Add(hudView);
    }

    private void HideAllViews()
    {
        foreach(var view in _allUIViews)
        {
                view.Hide();
        }
    }

}