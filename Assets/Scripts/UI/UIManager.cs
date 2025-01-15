using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    VisualElement _root;
    List<UIView> _allUIViews = new();

    DetailView _detailView;
    HUDView _hudView;
    LightOverview _lightOverview;
    Encyclopedia _encyclopedia;

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
        _lightOverview = new LightOverview(_root, this);
        _encyclopedia = new Encyclopedia(_root, this);

        UIEvents.ShowDetailView += (index) => { HideAllViews(); _detailView.Show(index); };
        UIEvents.HideDetailView += () => ResetToHUD(_detailView);

        UIEvents.ShowHUDView += () => ShowView(_hudView);
        UIEvents.HideHUDView += _hudView.Hide;

        UIEvents.ShowLightOverview += () => ShowView(_lightOverview);
        UIEvents.HideLightOverview += () => ResetToHUD(_lightOverview);

        UIEvents.ShowEncyclopedia += () => ShowView(_encyclopedia);
        UIEvents.HideEncyclopedia += () => ResetToHUD(_encyclopedia);



        _allUIViews.Add(_detailView);
        _allUIViews.Add(_hudView);
        _allUIViews.Add(_lightOverview);
        _allUIViews.Add(_encyclopedia);
    }
    private void OnDestroy()
    {
        UIEvents.ShowDetailView -= (index) => { HideAllViews(); _detailView.Show(index); };
        UIEvents.HideDetailView -= () => ResetToHUD(_detailView);
        _detailView.Dispose();
        UIEvents.ShowHUDView -= () => ShowView(_hudView);
        UIEvents.HideHUDView -= _hudView.Hide;
        _hudView.Dispose();
        UIEvents.ShowLightOverview -= () => ShowView(_lightOverview);
        UIEvents.HideLightOverview -= () => ResetToHUD(_lightOverview);
        _lightOverview.Dispose();
        UIEvents.ShowEncyclopedia -= () => ShowView(_encyclopedia);
        UIEvents.ShowEncyclopedia -= () => ResetToHUD(_encyclopedia);
        _encyclopedia.Dispose();
    }

    private void ResetToHUD(UIView view)
    {
        view.Hide();
        _hudView.Show();
    }


    private void ShowView(UIView view)
    {
        HideAllViews();
        view.Show();
    }

    private void HideAllViews()
    {
        foreach(var view in _allUIViews)
        {
                view.Hide();
        }
    }

}