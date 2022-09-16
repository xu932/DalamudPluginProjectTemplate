using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dalamud.Logging;

namespace CottonCollector.CharacterControl.Commands
{
    [Serializable]
    internal class CommandTreeNode
    {
        public Command command = null;  // only available for leaf.

        public LinkedList<CommandTreeNode> children = new LinkedList<CommandTreeNode>();

        private bool mutable = false;

        public CommandTreeNode ExecutableCopy()
        {
            CommandTreeNode ret = new CommandTreeNode(true);
            ret.command = command; // this is suppose to be a ref.
            foreach (var child in children) {
                ret.children.AddLast(child.ExecutableCopy());
            }

            return ret;
        }

        public CommandTreeNode() { }

        public CommandTreeNode(bool mutable = false) {
            this.mutable = mutable;
        }

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
            if (!mutable) return null;

            if (IsLeaf())
            {
                return command;
            }

            if (children.Count > 0)
            {
                var nextNode = children.First.ValueRef;
                var ret = nextNode.PopCommand();
                if (nextNode.children.Count == 0)
                {
                    children.RemoveFirst();
                }
                return ret;
            }

            return null;
        }
    }
}
