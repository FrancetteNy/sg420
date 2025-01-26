using UnityEngine;
using UnityEngine.UIElements;

public class ChatView : UIView
{
    ChatViewController _controller;
    HighlightController _highlightController;
    public ChatView(VisualElement root, UIManager manager) : base(root, manager)
    {
        base.Initialize();
        Asset = Resources.Load<VisualTreeAsset>("UI Toolkit/UXML/ChatView");
        if (Asset == null)
        {
            Debug.LogError("Failed to load VisualTreeAsset: UI Toolkit/UXML/ChatView");
        }
        Asset.CloneTree(Root);
        View = Root.Q<VisualElement>("ChatView");

        _controller = Manager.gameObject.AddComponent<ChatViewController>();
        _controller.enabled = false;
        _controller.Initialize(root);
        _highlightController = GameObject.FindAnyObjectByType<HighlightController>();
        Hide();
    }

    public override void Dispose()
    {
        Root.Remove(View);
        GameObject.Destroy(_controller);
    }

    public override void Hide()
    {
        View.style.display = DisplayStyle.None;
        _controller.enabled=false;
        _highlightController.enabled=true;
    }

    public override void Show()
    {
        View.style.display = DisplayStyle.Flex;
        _controller.enabled = true;
        _highlightController.enabled=false;
    }
}
