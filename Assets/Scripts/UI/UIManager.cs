using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    VisualElement _root;
    List<UIView> _allUIViews = new();

    DetailView _detailView;
    HUDView _hudView;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        AddAllUIViews();
    }

    private void AddAllUIViews()
    {
        _detailView = new DetailView(_root, this);
        _hudView = new HUDView(_root, this);
        UIEvents.ShowDetailView += (index) => { HideAllViews(); _detailView.Show(index); };
        UIEvents.HideDetailView += () => { _detailView.Hide(); _hudView.Show(); };

        UIEvents.ShowHUDView += () => { HideAllViews(); _hudView.Show(); };
        UIEvents.HideHUDView += _hudView.Hide;

        _allUIViews.Add(_detailView);
        _allUIViews.Add(_hudView);
    }
    private void OnDestroy()
    {
        UIEvents.ShowDetailView -= (index) => { HideAllViews(); _detailView.Show(index); };
        UIEvents.HideDetailView -= () => { _detailView.Hide(); _hudView.Show(); };
        _detailView.Dispose();
        UIEvents.ShowHUDView -= () => { HideAllViews(); _hudView.Show(); };
        UIEvents.HideHUDView -= _hudView.Hide;
        _hudView.Dispose();
    }

    private void HideAllViews()
    {
        foreach(var view in _allUIViews)
        {
                view.Hide();
        }
    }

}