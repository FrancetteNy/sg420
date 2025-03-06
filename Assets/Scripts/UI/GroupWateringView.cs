using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class GroupWateringView : UIView
{
    GroupWateringController _controller;
    public GroupWateringView(VisualElement root, UIManager manager) : base(root, manager) { }
    override protected void Initialize()
    {
        base.Initialize();
        Asset = Resources.Load<VisualTreeAsset>("UI Toolkit/UXML/GroupWatering");
        if (Asset == null)
        {
            Debug.LogError("Failed to load VisualTreeAsset: UI Toolkit/UXML/GroupWatering");
        }
        Asset.CloneTree(Root);
        View = Root.Q<VisualElement>("GroupWatering");

        _controller = Manager.gameObject.AddComponent<GroupWateringController>();
        _controller.Initialize(View, GameObject.FindAnyObjectByType<PlantManager>());


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
    }

    public override void OnCancelPerformed(InputAction.CallbackContext context)
    {
        if (!IsOpen)
            return;
        UIEvents.HideGroupWateringView();
    }
}
