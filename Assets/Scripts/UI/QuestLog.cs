using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

class QuestLog : UIView
{
    QuestLogController _controller;
    public QuestLog(VisualElement root, UIManager manager) : base(root, manager) { }
    override protected void Initialize()
    {
        base.Initialize();
        Asset = Resources.Load<VisualTreeAsset>("UI Toolkit/UXML/QuestLog");
        if (Asset == null)
        {
            Debug.LogError("Failed to load VisualTreeAsset: UI Toolkit/UXML/QuestLog");
        }
        Asset.CloneTree(Root);
        View = Root.Q<VisualElement>("QuestLog");

        _controller = Manager.gameObject.AddComponent<QuestLogController>();
        _controller.Initialize(View);

        Hide();
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
        _controller.enabled = false;
    }


    public override void Show()
    {
        base.Show();
        _controller.enabled = true;
        _controller.RefreshView();
    }

    public override void OnCancelPerformed(InputAction.CallbackContext context)
    {
        if (!IsOpen)
            return;
        UIEvents.HideEncyclopedia();
    }
}
