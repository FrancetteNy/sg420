using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EncyclopediaController : MonoBehaviour
{

    private VisualElement _root;
    private Label _textView;
    private TreeView _treeView;
    private TextField _searchBar;
    private List<Category> _unlockedEntries;

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

        public List<Entry> Entries
        {
            get;
        }

        public Category(string name, List<Entry> entries)
        {
            this.Name = name;
            this.Entries = entries;
        }

    }

    protected static List<Category> Categories = new List<Category> {
        new Category("Anbau", new List<Entry>
        {
            new Entry("Bewässern"),
            new Entry("Nährstoffe"),
            new Entry("Krankheiten"),
            new Entry("Licht"),
        }),
        new Category("Ernteprozess", new List<Entry>
        {
            new Entry("Ernten"),
            new Entry("Trocknen"),
        })
    };

    protected static List<Entry> Entries
    {
        get
        {
            var retVal = new List<Entry>(6);
            foreach (var category in Categories)
            {
                retVal.AddRange(category.Entries);
            }
            return retVal;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize(VisualElement root)
    {
        _root = root;

        _root.style.display = DisplayStyle.None;

        _root.Q<Button>("close-button").clicked += () => UIEvents.HideEncyclopedia.Invoke();
        _root.Q<Button>("close-button").clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");

        _textView = _root.Q<Label>("entry");

        FilterLockedEntries();
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
            var tempList = new List<Category>();
            if (search == "")
            {
                _treeView.SetRootItems(GenerateTreeRoots(_unlockedEntries));
            }
            else
            {
                foreach (var category in _unlockedEntries)
                {
                    if (category.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        tempList.Add(category);
                    }
                    else
                    {
                        var tempEntryList = new List<Entry>();
                        int count = 0;
                        foreach (var entry in category.Entries)
                        {
                            if (entry.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                tempEntryList.Add(entry);
                                count++;
                            }
                        }
                        if (count > 0)
                        {
                            tempList.Add(new Category(category.Name, tempEntryList));
                        }
                    }
                }
                _treeView.SetRootItems(GenerateTreeRoots(tempList));
            }

            _treeView.Rebuild();
        });

    }

    private void FilterLockedEntries()
    {
        List<String> unlockedEntryNames = GameStateManagerSingleton.Instance.GameState.UnlockedEncyclopediaEntries.List;
        List<Category> filteredList = new List<Category>();
        foreach (var category in Categories)
        {
            List<Entry> entries = new List<Entry>();
            int count = 0;
            foreach (var entry in category.Entries)
            {
                if (unlockedEntryNames.Contains(entry.Name))
                {
                    entries.Add(entry);
                    count++;
                }
            }
            if (count > 0)
            {
                filteredList.Add(new Category(category.Name, entries));
            }
        }
        _unlockedEntries = filteredList;
    }

    public void ReloadEntries()
    {
        FilterLockedEntries();
        SetUpTreeView();
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


    static IList<TreeViewItemData<IEntryOrCategory>> GenerateTreeRoots(List<Category> categories)
    {
        int id = 0;
        var roots = new List<TreeViewItemData<IEntryOrCategory>>(categories.Count);
        foreach (var category in categories)
        {
            var entriesInCategory = new List<TreeViewItemData<IEntryOrCategory>>(category.Entries.Count);
            foreach (var entry in category.Entries)
            {
                entriesInCategory.Add(new TreeViewItemData<IEntryOrCategory>(id++, entry));
            }

            roots.Add(new TreeViewItemData<IEntryOrCategory>(id++, category, entriesInCategory));
        }
        return roots;
    }

}