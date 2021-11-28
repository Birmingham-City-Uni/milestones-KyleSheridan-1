using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TreeNode
{
    public abstract bool Execute();
}

public interface BehaviourTreeDelegates
{
    public delegate bool LeafNodeDelegate();
}
