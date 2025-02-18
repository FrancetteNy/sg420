using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

class DetailView : UIView
{
    DetailViewController _controller;
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

    }
    public override void Dispose()
    {
        base.Dispose();
        Root.Remove(View);
        GameObject.Destroy(_controller);
    }

    public override void Hide()
    {
        base.Hide();
        _controller.TriggerDisabling();
    }

    public void Show(int index)
    {
        _controller.ActivateView(index, () => { });
        ShowView();
    }

    public override void Show()
    {
        _controller.ActivateView(-1, () => { });
        ShowView();
    }

    private void ShowView()
    {
        base.Show();
        _controller.enabled = true;

    }

    public override void OnCancelPerformed(InputAction.CallbackContext context)
    {
        if (!IsOpen)
            return;
        _controller.CloseView();
    }
}
