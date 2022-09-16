using System;
using System.Collections.Generic;
using System.Linq;

using Dalamud.Logging;

namespace CottonCollector.CharacterControl.Commands
{
    [Serializable]
    internal class CommandTreeNode
    {
        public Command command = null;  // only available for leaf.

        public LinkedList<CommandTreeNode> children = new LinkedList<CommandTreeNode>();
        private LinkedListNode<CommandTreeNode> curr = null;

        public CommandTreeNode() { }

        public CommandTreeNode(CommandTreeNode source)
        {
            command = source.command;
            children = source.children;
            curr = children.First;
        }

        public CommandTreeNode(Command command)
        {
            this.command = command;
        }

        public CommandTreeNode(LinkedList<CommandTreeNode> children)
        {
            this.children = children;
        }

        public bool IsLeaf()
        {
            return children.Count == 0;
        }

        public void Add(Command command)
        {
            children.AddLast(new CommandTreeNode(command));
            curr = children.First;
            children.Last().Reset();
        }

        public void Add(CommandTreeNode child)
        {
            children.AddLast(child);
            curr = children.First;
            children.Last().Reset();
        }

        public bool IsCurrent()
        {
            if (IsLeaf())
            {
                return command != null && command.IsCurrent();
            }

            return children.Any(i => i.IsCurrent());
        }

        public void Reset()
        {
            curr = children.First;
            foreach (var child in children) child.Reset();
        }

        public Command NextCommand()
        {
            var tmp = curr;
            if (curr != null && curr.ValueRef.IsLeaf())
            {
                curr = curr.Next;
                return tmp.ValueRef.command;
            }

            for (;curr != null; curr = curr.Next)
            {
                if (curr.ValueRef.curr != null)
                {
                    return curr.ValueRef.NextCommand();
                }
            }

            return null;
        }
    }
}
