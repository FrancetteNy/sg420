Quick Outline
=============

Developed by Chris Nolet (c) 2018


Instructions
------------

To add an outline to an object, drag-and-drop the Outline.cs
script onto the object. The outline materials will be loaded
at runtime.

You can also add outlines programmatically with:

    var outline = gameObject.AddComponent<Outline>();

    outline.OutlineMode = Outline.Mode.OutlineAll;
    outline.OutlineColor = Color.yellow;
    outline.OutlineWidth = 5f;

The outline script does a small amount of work in Awake().
For best results, use outline.enabled to toggle the outline.
Avoid removing and re-adding the component if possible.

For large meshes, you may also like to enable 'Precompute
Outline' in the editor. This will reduce the amount of work
performed in Awake().


Troubleshooting
---------------

If the outline appears off-center, please try the following:

1. Set 'Read/Write Enabled' on each model's import settings.
2. Disable 'Optimize Mesh Data' in the player settings.


## Verwendete Medien
- **Reispflanze Clipart** von Creazilla  
  Quelle: [https://creazilla.com/de/media/clipart/7768301/reispflanze](https://creazilla.com/de/media/clipart/7768301/reispflanze)  
  **Lizenz:** Frei für kommerzielle & nicht-kommerzielle Nutzung, keine Namensnennung erforderlich.
- **Goldener Stern (Sheriff-Stern)**  
  Quelle: [Pixabay](https://pixabay.com/de/vectors/sheriff-ist-sterne-stern-golden-151699/)  
  **Lizenz:** Frei für kommerzielle & nicht-kommerzielle Nutzung, keine Namensnennung erforderlich  
- **Herbstblatt**  
  Quelle: [Pixabay](https://pixabay.com/de/photos/herbst-herbstblatt-2887556/)  
  **Lizenz:** Frei für kommerzielle & nicht-kommerzielle Nutzung, keine Namensnennung erforderlich  

