using System;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    partial class BindableSnapshotCollection<T>
    {
        public struct EditAction
        {
            public bool IsAdd;
            public int OriginalIndex;
            public T Item;
        }
        private class LinkNode
        {
            public int X { get; }
            public int Y { get; }
            public EditAction Action { get; }
            public LinkNode Previous { get; }
            public int Count { get; }
            public LinkNode(int x, int y)
            {
                X = x;
                Y = y;
            }
            public LinkNode(int x, int y, EditAction action, LinkNode previous)
            {
                X = x;
                Y = y;
                Action = action;
                Previous = previous;
                Count = previous.Count + 1;
            }
        }
        public static EditAction[] SequenceDiffer(T[] source, T[] target)
        {
            int GetLongestMatch(ArraySegment<T> _source, ArraySegment<T> _target)
            {
                int i;
                for (i = 0; i < _source.Count && i < _target.Count; i++)
                    if (!EqualityComparer<T>.Default.Equals(_source.Array[_source.Offset + i], _target.Array[_target.Offset + i]))
                        break;
                return i;
            }

            EditAction[] BuildResult(LinkNode node)
            {
                int c = node.Count;
                var result = new EditAction[c];
                while (c > 0)
                {
                    result[--c] = node.Action;
                    node = node.Previous;
                }
                return result;
            }

            int start = GetLongestMatch(new ArraySegment<T>(source), new ArraySegment<T>(target));
            if (start == source.Length && start == target.Length)
                return Array.Empty<EditAction>();

            var queue = new Queue<LinkNode>(source.Length + target.Length);
            queue.Enqueue(new LinkNode(start, start));

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (node.X < source.Length)
                {
                    var action = new EditAction { IsAdd = false, OriginalIndex = node.X, Item = source[node.X] };
                    int l = GetLongestMatch(new ArraySegment<T>(source, node.X + 1, source.Length - node.X - 1), new ArraySegment<T>(target, node.Y, target.Length - node.Y));
                    var newNode = new LinkNode(node.X + 1 + l, node.Y + l, action, node);
                    if (newNode.X == source.Length && newNode.Y == target.Length)
                        return BuildResult(newNode);
                    else
                        queue.Enqueue(newNode);
                }
                if (node.Y < target.Length)
                {
                    var action = new EditAction { IsAdd = true, OriginalIndex = node.X, Item = target[node.Y] };
                    int l = GetLongestMatch(new ArraySegment<T>(source, node.X, source.Length - node.X), new ArraySegment<T>(target, node.Y + 1, target.Length - node.Y - 1));
                    var newNode = new LinkNode(node.X + l, node.Y + 1 + l, action, node);
                    if (newNode.X == source.Length && newNode.Y == target.Length)
                        return BuildResult(newNode);
                    else
                        queue.Enqueue(newNode);
                }
            }
            throw new Exception("How do you get here?");
        }
    }
}
