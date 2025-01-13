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

    protected interface IEntryOrCategory
    {
        public string name
        {
           get; 
        }

        public string pathToContent
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

        public string pathToContent
        {
           get; 
        }

        public Entry(string name, string contentFileName)
        {
            this.name = name;
            this.pathToContent = "Assets/Resources/EncyclopediaEntries/" + contentFileName;
        }
    }

    protected class Category : IEntryOrCategory
    {
        public string name
        {
           get; 
        }
        
        public string pathToContent
        {
           get; 
        }
        
        public List<Entry> entries
        {
           get; 
        }

        public Category(string name, string contentFileName, List<Entry> entries) {
            this.name = name;
            this.entries = entries;
            this.pathToContent = "Assets/Resources/EncyclopediaEntries/" + contentFileName;
        }

    }
  
    protected static List<Category> categories = new List<Category> {
        new Category("Anbau", "Anbau.html",  new List<Entry>
        {
            new Entry("Bewässern", "Bewässern.html"),
            new Entry("Nährstoffe", "Nährstoffe.html"),
            new Entry("Feuchtigkeit", "Licht.html"),
            new Entry("Krankheiten", "Krankheiten.html"),
        }),
        new Category("Ernteprozess", "Ernteprozess.html", new List<Entry>
        {
            new Entry("Ernte", "Ernte.html"),
            new Entry("Trocknen", "Trocknen.html"),
        })
    };

    protected static List<Entry> entries
    {
        get
        {
            var retVal = new List<Entry>(6);
            foreach (var category in categories)
            {
                retVal.AddRange(category.entries);
            }
            return retVal;
        }
    }

    protected static IList<TreeViewItemData<IEntryOrCategory>> treeRoots
    {
        get
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


  

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize(VisualElement root)
    {
        _root = root;

        _root.style.display = DisplayStyle.None;

        _root.Q<Button>("close-button").clicked += () => UIEvents.HideEncyclopedia.Invoke();
        _root.Q<Button>("close-button").clicked += () => SoundManagerSingleton.Instance.PlaySound("Click");

        //var _encyclopediaEntryAsset = EditorGUIUtility.Load("Assets/Resources/UI%20Toolkit/EncyclopediaItemView.uxml") as VisualTreeAsset;

        _textView = _root.Q<Label>("entry");
 

        _treeView = _root.Query<TreeView>("tree-view");
        _treeView.SetRootItems(treeRoots);

        _treeView.makeItem = () => new Button();

        _treeView.bindItem = (VisualElement element, int index) =>
        {
            (element as Button).text = _treeView.GetItemDataForIndex<IEntryOrCategory>(index).name;
            (element as Button).RegisterCallback<MouseUpEvent>((evt) => LoadEntry(_treeView.GetItemDataForIndex<IEntryOrCategory>(index)));
        };


        try
        {
            using (StreamReader sr = new StreamReader("Assets/Resources/EncyclopediaEntries/Anbau.html")) 
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
        try
        {
            using (StreamReader sr = new StreamReader(entry.pathToContent)) 
            {
                _textView.text = sr.ReadToEnd();
            }
        }
        catch (Exception e)
        {
            Debug.Log("File Could not be read:" + entry.pathToContent);
            Debug.Log(e.Message);
        }
    }

}
