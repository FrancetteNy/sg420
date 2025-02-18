using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ChatView : UIView
{
    ChatViewController _controller;
    public ChatView(VisualElement root, UIManager manager) : base(root, manager) { }
    override protected void Initialize()
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
        _controller.Initialize(Root);
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
        IsOpen = false;
        View.EnableInClassList("chatview--hidden", true);
        _controller.enabled = false;
        View.schedule.Execute(() =>
        {
            View.style.display = DisplayStyle.None;
        }).ExecuteLater(300);
    }


    public override void Show()
    {
        IsOpen = true;
        View.EnableInClassList("chatview--hidden", false);
        View.style.display = DisplayStyle.Flex;
        View.schedule.Execute(() =>
        {
            _controller.enabled = true;
        }).ExecuteLater(300);
    }

    public override void OnCancelPerformed(InputAction.CallbackContext context)
    {
        if (!IsOpen)
            return;
        UIEvents.HideChatView();
    }
}
