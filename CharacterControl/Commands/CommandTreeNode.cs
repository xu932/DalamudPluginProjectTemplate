using System;
using System.Collections.Generic;
using System.Linq;

namespace CottonCollector.CharacterControl.Commands
{
    [Serializable]
    internal class CommandTreeNode
    {
        // The following two shall be mutally exclusive.
        public Command command = null;
        public LinkedList<CommandTreeNode> children = new LinkedList<CommandTreeNode>();

        public CommandTreeNode() { }

        public CommandTreeNode(CommandTreeNode source)
        {
            command = source.command;
            children = source.children;
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
        }

        public void Add(CommandTreeNode child)
        {
            children.AddLast(child);
        }

        public bool IsCurrent()
        {
            if (IsLeaf())
            {
                return command != null && command.IsCurrent();
            }

            return children.Any(i => i.IsCurrent());
        }

        public Command PopCommand()
        {
            if (IsLeaf())
            {
                return command;
            }

            if (children.Count == 0)
            {
                return null;
            }

            var nextNode = children.First.ValueRef;

            if (nextNode.IsLeaf() || nextNode.children.Count == 0)
            {
                children.RemoveFirst();
            }

            return nextNode.command;
        }
    }
}
