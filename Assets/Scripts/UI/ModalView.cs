using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ModalView : UIView
{
    UnityAction _confirmAction;
    Button _confirmButton;
    Button _closeButton;
    Label _titleLabel;
    Label _descriptionLabel;
    public ModalView(VisualElement root, UIManager manager) : base(root, manager) { }
    override protected void Initialize()
    {
        base.Initialize();
        Asset = Resources.Load<VisualTreeAsset>("UI Toolkit/UXML/ModalView");
        if (Asset == null)
        {
            Debug.LogError("Failed to load VisualTreeAsset: UI Toolkit/UXML/ModalView");
        }
        Asset.CloneTree(Root);
        View = Root.Q<VisualElement>("Modal");

        Hide();
        SetupButtons();
        SetupLabels();
    }

    private void SetupLabels()
    {
        _titleLabel = View.Q<Label>("Title");
        _descriptionLabel = View.Q<Label>("Description");
    }

    private void SetupButtons()
    {
        _confirmButton = View.Q<Button>("Confirm");
        _confirmButton.clicked += OnConfirmButtonClicked;
        _closeButton = View.Q<Button>("Cancel");
        _closeButton.clicked += OnCloseButtonClicked;
    }

    private void OnCloseButtonClicked()
    {
        SoundManagerSingleton.Instance.PlaySound("Click");
        _confirmAction = null;
        UIEvents.HideModalView?.Invoke();
    }

    private void OnConfirmButtonClicked()
    {
        SoundManagerSingleton.Instance.PlaySound("Click");
        _confirmAction?.Invoke();
        _confirmAction = null;
        UIEvents.HideModalView?.Invoke();
    }

    public override void Dispose()
    {
        base.Dispose();
        Root.Remove(View);
    }

    public override void Hide()
    {
        View.EnableInClassList("hide", true);
        IsOpen = false;
    }


    public override void Show()
    {
        View.EnableInClassList("hide", false);
        IsOpen = true;
        _confirmButton.SetEnabled(_confirmAction != null);
    }

    public void Show(string title, string description, UnityAction confirmAction)
    {
        _titleLabel.text = title;
        _descriptionLabel.text = description;
        _confirmAction = confirmAction;
        Show();
    }
    public override void OnCancelPerformed(InputAction.CallbackContext context)
    {
        if (!IsOpen)
            return;
        OnCloseButtonClicked();
    }
}
