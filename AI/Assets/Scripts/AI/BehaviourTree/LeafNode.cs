using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafNode : TreeNode, BehaviourTreeDelegates
{
    private BehaviourTreeDelegates.LeafNodeDelegate functionallity;

    public override bool Execute()
    {
        if (functionallity != null)
            return functionallity();
        else
            return false;
    }

    public void SetFunctionallity(BehaviourTreeDelegates.LeafNodeDelegate newFunction)
    {
        functionallity = newFunction;
    }
}
