using UnityEngine;
using UnityEngine.UIElements;

class DetailView : UIView
{
    DetailViewController _controller;
    HighlightController _highlightController;
    public DetailView(VisualElement root, UIManager manager) : base(root, manager) { }
    override protected void Initialize()
    {
        base.Initialize();
        Asset = Resources.Load<VisualTreeAsset>("UI Toolkit/UXML/DetailView");
        if (Asset == null)
        {
            Debug.LogError("Failed to load VisualTreeAsset: UI Toolkit/UXML/DetailView");
        }
        Asset.CloneTree(Root);
        View = Root.Q<VisualElement>("DetailView");

        _controller = Manager.gameObject.AddComponent<DetailViewController>();
        _controller.Initialize(GameObject.Find("DetailView Camera").GetComponent<Camera>(), GameObject.FindAnyObjectByType<PlantManager>());
        _controller.enabled = false;

        _highlightController = GameObject.FindAnyObjectByType<HighlightController>();
    }
    public override void Dispose()
    {
        Root.Remove(View);
        GameObject.Destroy(_controller);
    }

    public override void Hide()
    {
        View.style.display = DisplayStyle.None;
        _controller.TriggerDisabling();
        _highlightController.enabled = true;
    }

    public void Show(int index)
    {
        _controller.ActivateView(index, () => { });
        ShowView();
    }

    public override void Show()
    {
        _controller.ActivateView(0, () => { });
        ShowView();
    }

    private void ShowView()
    {
        _controller.enabled = true;
        View.style.display = DisplayStyle.Flex;
        _highlightController.enabled = false;

    }
}
