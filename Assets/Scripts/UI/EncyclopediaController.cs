using MyUILibrary;
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
    protected interface IEntryOrCategory
    {
        public string name
        {
           get; 
        }

    }

    protected class Entry : IEntryOrCategory
    {
        public string name
        {
           get; 
        }

        public Entry(string name)
        {
            this.name = name;
        }
    }

    protected class Category : IEntryOrCategory
    {
        public string name
        {
           get; 
        }
        
        public List<Entry> entries
        {
           get; 
        }

        public Category(string name, List<Entry> entries) {
            this.name = name;
            this.entries = entries;
        }

    }
  
    protected static List<Category> _categories = new List<Category> {
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

    protected static List<Entry> entries
    {
        get
        {
            var retVal = new List<Entry>(6);
            foreach (var category in _categories)
            {
                retVal.AddRange(category.entries);
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
 
        SetUpSearchBar();
        SetUpTreeView();

        try
        {
            using (StreamReader sr = new StreamReader("Assets/Resources/EncyclopediaEntries/Home.html")) 
            {
                _textView.text = sr.ReadToEnd();
            }
        }
        catch (Exception e)
        {
            Debug.Log("File Could not be read: Home");
            Debug.Log(e.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadEntry(IEntryOrCategory entry)
    {
        string pathToContent = "Assets/Resources/EncyclopediaEntries/" + entry.name + ".html";
        try
        {
            using (StreamReader sr = new StreamReader(pathToContent)) 
            {
                _textView.text = sr.ReadToEnd();
            }
        }
        catch (Exception e)
        {
            Debug.Log("File Could not be read:" + pathToContent);
            Debug.Log(e.Message);
        }
    }

    void SetUpSearchBar() 
    {
        _searchBar = _root.Q<TextField>("search-bar");

        _searchBar.RegisterCallback<ChangeEvent<string>>((evt) =>
        {
            var search = evt.newValue;
            var tempList = new List<Category>();
            if (search == "") {
                _treeView.SetRootItems(GenerateTreeRoots(_categories));
            }
            else 
            {
                foreach (var category in _categories)
                {
                    if (category.name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        tempList.Add(category);
                    }
                    else
                    {
                        var tempEntryList = new List<Entry>();
                        int count = 0;
                        foreach (var entry in category.entries)
                        {
                            if (entry.name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                tempEntryList.Add(entry);
                                count++;
                            }
                        }
                        if (count > 0)
                        {
                            tempList.Add(new Category(category.name, tempEntryList));
                        }
                    }
                }
                _treeView.SetRootItems(GenerateTreeRoots(tempList));
            }

            _treeView.Rebuild();
        });

    }

    void SetUpTreeView() 
    {
        _treeView = _root.Query<TreeView>("tree-view");
        _treeView.SetRootItems(GenerateTreeRoots(_categories));

        _treeView.makeItem = () => new Button();

        _treeView.bindItem = (VisualElement element, int index) =>
        {
            (element as Button).text = _treeView.GetItemDataForIndex<IEntryOrCategory>(index).name;
            (element as Button).RegisterCallback<MouseUpEvent>((evt) => LoadEntry(_treeView.GetItemDataForIndex<IEntryOrCategory>(index)));
        };
    }


    static IList<TreeViewItemData<IEntryOrCategory>> GenerateTreeRoots(List<Category> categories)
    {
        int id = 0;
        var roots = new List<TreeViewItemData<IEntryOrCategory>>(categories.Count);
        foreach (var category in categories)
        {
            var entriesInCategory = new List<TreeViewItemData<IEntryOrCategory>>(category.entries.Count);
            foreach (var entry in category.entries)
            {
                entriesInCategory.Add(new TreeViewItemData<IEntryOrCategory>(id++, entry));
            }

            roots.Add(new TreeViewItemData<IEntryOrCategory>(id++, category, entriesInCategory));
        }
        return roots;
    }

}