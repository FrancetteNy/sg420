using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ModalController : MonoBehaviour
{
    public static ModalController Instance;
    private VisualElement _root;

    private Label _title;
    private Label _description;

    Button _confirmBtn;

    private Action _confirmAction;
    private Action _cancelAction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
        _root = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("Modal");

        SetupButtons();
        SetupLabels();
    }
    private void SetupButtons()
    {
        _confirmBtn = _root.Q<Button>("Confirm");
        _confirmBtn.clicked += () =>
        {
            _confirmAction?.Invoke();
        };

        Button _cancelBtn = _root.Q<Button>("Cancel");
        _cancelBtn.clicked += () =>
        {
            _cancelAction?.Invoke();
            HideModal();
        };
        _cancelBtn.clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");

        _root.RegisterCallback<ClickEvent>(evt =>
        {
            if(evt.target == _root)
            {
                HideModal();
            }
        });
    }
    private void SetupLabels()
    {
        _title = _root.Q<Label>("Title");
        _description = _root.Q<Label>("Description");
    }

    public void ShowModal(string title, string description, Action onConfirmAction = null, Action onCancelAction = null)
    {
        _confirmAction = null;

        if (onConfirmAction == null)
        {
            _confirmBtn.AddToClassList("disabled");
        }
        else
        {
            _confirmBtn.RemoveFromClassList("disabled");
            _confirmAction += onConfirmAction;
            _confirmAction += () =>
            {
                SoundManagerSingleton.Instance.PlaySound("Click");
                HideModal();
            };
        }

        _title.text = title;
        _description.text = description;

        _cancelAction = onCancelAction;

        _root.RemoveFromClassList("hide");
    }
    private void HideModal()
    {
        _root.AddToClassList("hide");
    }
}
