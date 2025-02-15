using UnityEngine;
using UnityEngine.UIElements;

public class TabController : MonoBehaviour
{
    private VisualElement root;
    private Button tab1, tab2, tab3, tab4;
    private VisualElement content1, content2, content3;

    void Start()
    {
        
        root = GetComponent<UIDocument>().rootVisualElement;

       
        tab1 = root.Q<Button>("tab1");
        tab2 = root.Q<Button>("tab2");
        tab3 = root.Q<Button>("tab3");
        tab4 = root.Q<Button>("tab4");

        
        content1 = root.Q<VisualElement>("tab-content1");
        content2 = root.Q<VisualElement>("tab-content2");
        content3 = root.Q<VisualElement>("tab-content3");

        
        tab1.clicked += () => ShowTab(content1);
        tab2.clicked += () => ShowTab(content2);
        tab3.clicked += () => ShowTab(content3);

       
        ShowTab(content1);
    }

    void ShowTab(VisualElement activeTab)
    {
        
        content1.style.display = DisplayStyle.None;
        content2.style.display = DisplayStyle.None;
        content3.style.display = DisplayStyle.None;

       
        activeTab.style.display = DisplayStyle.Flex;
    }
}
