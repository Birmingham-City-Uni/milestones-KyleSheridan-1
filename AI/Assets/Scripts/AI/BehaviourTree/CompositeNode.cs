using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : TreeNode
{
    public List<TreeNode> children;

    public void AddChildNode(TreeNode child)
    {
        children.Add(child);
    }
}

public class SelectorNode : CompositeNode
{
    public override bool Execute()
    {
        foreach (TreeNode child in children)
        {
            if (child.Execute())
            {
                return true;
            }
        }

        return false;
    }
}

public class SequenceNode : CompositeNode
{
    public override bool Execute()
    {
        foreach (TreeNode child in children)
        {
            if (!child.Execute())
            {
                return false;
            }
        }

        return true;
    }
}
