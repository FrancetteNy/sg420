using SG420UILibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class EncyclopediaController : MonoBehaviour
{

    private VisualElement _root;
    private Label _textView;
    private TreeView _treeView;
    private TextField _searchBar;    
    private List<IEntryOrCategory> _unlockedEntries;

    protected interface IEntryOrCategory
    {
        public string Name
        {
           get; 
        }

    }

    protected class Entry : IEntryOrCategory
    {
        public string Name
        {
           get; 
        }

        public Entry(string name)
        {
            this.Name = name;
        }
    }

    protected class Category : IEntryOrCategory
    {
        public string Name
        {
           get; 
        }
        
        public List<IEntryOrCategory> Entries
        {
           get; 
        }

        public Category(string name, List<IEntryOrCategory> entries) {
            this.Name = name;
            this.Entries = entries;
        }

    }
  
    protected static List<IEntryOrCategory> Categories = new List<IEntryOrCategory> {
        new Category("Anbau", new List<IEntryOrCategory>
        {
            new Entry("Alter"),
            new Entry("Bewässern"),
            new Entry("Geschlecht"),
            new Entry("Krankheiten"),
            new Entry("Licht"),
            new Entry("Nährstoffe"),
            new Entry("Strains"),
            new Category("Phasen", new List<IEntryOrCategory>
            {
                new Entry("Keimung"),
                new Entry("Wachstum"),
                new Entry("Blüte"),
            }),
        }),
        new Category("Ernteprozess", new List<IEntryOrCategory>
        {
            new Entry("Ernten"),
            new Entry("Trocknen"),
        }),
        new Category("Ausrüstung", new List<IEntryOrCategory>{
            new Entry("Lampen"),
            new Entry("Töpfe"),
        }),
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize(VisualElement root)
    {
        _root = root;

        _root.style.display = DisplayStyle.None;

        _root.Q<Button>("close-button").clicked += () => UIEvents.HideEncyclopedia.Invoke();
        _root.Q<Button>("close-button").clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");

        _textView = _root.Q<Label>("entry");
 
        _unlockedEntries = FilterLockedEntries(GameStateManagerSingleton.Instance.GameState.UnlockedEncyclopediaEntries.List, Categories);
        SetUpSearchBar();
        SetUpTreeView();

        _textView.text = Resources.Load<TextAsset>("EncyclopediaEntries/Home").text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadEntry(IEntryOrCategory entry)
    {
        string pathToContent = "EncyclopediaEntries/" + entry.Name;

        _textView.text = Resources.Load<TextAsset>(pathToContent).text;
    }

    void SetUpSearchBar() 
    {
        _searchBar = _root.Q<TextField>("search-bar");

        _searchBar.RegisterCallback<ChangeEvent<string>>((evt) =>
        {
            var search = evt.newValue;
            if (search == "") {
                _treeView.SetRootItems(GenerateTreeRoots(_unlockedEntries));
            }
            else 
            {
                _treeView.SetRootItems(GenerateTreeRoots(FilterEntries(search, _unlockedEntries)));
            }
            _treeView.Rebuild();
        });

    }


    private List<IEntryOrCategory> FilterEntries(string search, List<IEntryOrCategory> entriesOrCategories)
    {
        List<IEntryOrCategory> filteredList = new List<IEntryOrCategory>();
        foreach (var entryOrCategory in entriesOrCategories) 
        {
            if (entryOrCategory is Entry) 
            {
                if (entryOrCategory.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0) 
                {
                    filteredList.Add(entryOrCategory);
                }
            }
            else
            {
                Category category = entryOrCategory as Category;
                List<IEntryOrCategory> tmpList = FilterEntries(search, category.Entries);
                if (tmpList.Count > 0)
                {
                    filteredList.Add(new Category(category.Name, tmpList));
                }
            }
        }
        return filteredList;
    }

    private List<IEntryOrCategory> FilterLockedEntries(List<String> unlockedEntryNames, List<IEntryOrCategory> entriesOrCategories)
    {
        List<IEntryOrCategory> filteredList = new List<IEntryOrCategory>();
        foreach (var entryOrCategory in entriesOrCategories) 
        {
            if (entryOrCategory is Entry) 
            {
                if (unlockedEntryNames.Contains(entryOrCategory.Name)) 
                {
                    filteredList.Add(entryOrCategory);
                }
            }
            else
            {
                Category category = entryOrCategory as Category;
                List<IEntryOrCategory> tmpList = FilterLockedEntries(unlockedEntryNames, category.Entries);
                if (tmpList.Count > 0)
                {
                    filteredList.Add(new Category(category.Name, tmpList));
                }
            }
        }
        return filteredList;
    }

    public void ReloadEntries() 
    {
        _unlockedEntries = FilterLockedEntries(GameStateManagerSingleton.Instance.GameState.UnlockedEncyclopediaEntries.List, Categories);
        _treeView.SetRootItems(GenerateTreeRoots(_unlockedEntries));
        _treeView.Rebuild();

    }

    void SetUpTreeView() 
    {
        _treeView = _root.Query<TreeView>("tree-view");
        _treeView.SetRootItems(GenerateTreeRoots(_unlockedEntries));

        _treeView.makeItem = () => new Button();

        _treeView.bindItem = (VisualElement element, int index) =>
        {
            (element as Button).text = _treeView.GetItemDataForIndex<IEntryOrCategory>(index).Name;
            (element as Button).RegisterCallback<MouseUpEvent>((evt) => LoadEntry(_treeView.GetItemDataForIndex<IEntryOrCategory>(index)));
        };
    }


    static IList<TreeViewItemData<IEntryOrCategory>> GenerateTreeRoots(List<IEntryOrCategory> entriesOrCategories)
    {
        int id = 0;
        var roots = new List<TreeViewItemData<IEntryOrCategory>>(entriesOrCategories.Count);
        foreach (var entryOrCategory in entriesOrCategories)
        {
            if (entryOrCategory is Category)
            {
                var category = entryOrCategory as Category;
                var entriesInCategory = GenerateTreeRoots(category.Entries, id);
                id = entriesInCategory[entriesInCategory.Count - 1].id;
                id++;
                roots.Add(new TreeViewItemData<IEntryOrCategory>(id++, category, entriesInCategory));
                var lastAddedMember = entriesInCategory[^1];
            } 
            else
            {
                roots.Add(new TreeViewItemData<IEntryOrCategory>(id++, entryOrCategory as Entry));
            } 
        }
        return roots;
    }

    static List<TreeViewItemData<IEntryOrCategory>> GenerateTreeRoots(List<IEntryOrCategory> entriesOrCategories, int id)
    {
        var roots = new List<TreeViewItemData<IEntryOrCategory>>();
        foreach (var entryOrCategory in entriesOrCategories)
        {
            if (entryOrCategory is Category)
            {
                var category = entryOrCategory as Category;
                var entriesInCategory = GenerateTreeRoots(category.Entries, id);
                var lastAddedMember = entriesInCategory[^1];
                id = entriesInCategory[^1].id;
                id++;
                roots.Add(new TreeViewItemData<IEntryOrCategory>(id++, category, entriesInCategory));
            } 
            else
            {
                roots.Add(new TreeViewItemData<IEntryOrCategory>(id++, entryOrCategory as Entry));
            } 
        }
        return roots;
    }

}