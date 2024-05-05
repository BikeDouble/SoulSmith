using Godot;
using System.Collections.Generic;
using Godot.Collections;

//This class contains all the moves defined in MoveLibrary classes
public partial class TemplateLibrary<T> : Node
{
    private System.Collections.Generic.Dictionary<string, T> _unitTemplates = new System.Collections.Generic.Dictionary<string, T>(); //This list contains all moves 

    // Called when the node enters the scene tree for the first time.
    public void Initialize()
    {
        _unitTemplates = CreateTemplates();
    }

    public virtual System.Collections.Generic.Dictionary<string, T> CreateTemplates()
    {
        System.Collections.Generic.Dictionary<string, T> templates = new System.Collections.Generic.Dictionary<string, T>();

        Array<Node> children = GetChildren();

        // Collect all templates from all children, and concatenate them into the master array

        foreach (Node node in children)
        {
            System.Collections.Generic.Dictionary<string, T> childTemplates = ((TemplateLibrary<T>)node).CreateTemplates();
            templates = MergeDictionaries(templates, childTemplates);
        }

        return templates;
    }

    private System.Collections.Generic.Dictionary<string, T> MergeDictionaries(System.Collections.Generic.Dictionary<string, T> dict1, 
                                                                            System.Collections.Generic.Dictionary<string, T> dict2)
    {
        foreach (KeyValuePair<string, T> item in dict2)
        {
            if (dict1.ContainsKey(item.Key))
            {
                HandleDuplicateKey(item);
            }
            else
            {
                dict1.Add(item.Key, item.Value);
            }
        }
        return dict1;
    }

    private void HandleDuplicateKey(KeyValuePair<string, T> item)
    {
        GD.Print("Duplicate key: " + item.Key);
    }

}
