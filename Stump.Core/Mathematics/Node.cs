using System;
using System.Collections.Generic;
using System.Linq;

namespace Stump.Core.Mathematics
{
    public class Node<T>
    {
        private readonly List<Node<T>> m_children;
        private Node<T> m_parent;
        public Node<T> Parent
        {
            get
            {
                return this.m_parent;
            }
        }
        public IEnumerable<Node<T>> Nodes
        {
            get
            {
                return this.m_children;
            }
        }
        public Node<T> FirstNode
        {
            get
            {
                return this.m_children.First<Node<T>>();
            }
        }
        public Node<T> LastNode
        {
            get
            {
                return this.m_children.Last<Node<T>>();
            }
        }
        public Node<T> PrevNode
        {
            get
            {
                return this.m_parent.m_children[this.m_parent.m_children.IndexOf(this) - 1];
            }
        }
        public Node<T> NextNode
        {
            get
            {
                return this.m_parent.m_children[this.m_parent.m_children.IndexOf(this) + 1];
            }
        }
        public int Depth
        {
            get
            {
                int num = 0;
                Node<T> node = this;
                while (node != null)
                {
                    node = node.m_parent;
                    num++;
                }
                return num - 1;
            }
        }
        public int Branches
        {
            get
            {
                return this.m_children.Count;
            }
        }
        public T Value
        {
            get;
            set;
        }

        public Node()
        {
            this.m_children = new List<Node<T>>();
        }

        public Node(T value)
        {
            this.m_children = new List<Node<T>>();
            this.Value = value;
        }

        public void Add(Node<T> child)
        {
            this.m_children.Add(child);
            child.SetParent(this);
        }

        public void Add(params Node<T>[] nodes)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                Node<T> node = nodes[i];
                this.m_children.Add(node);
                node.SetParent(this);
            }
        }

        public Node<T> Add(T value)
        {
            Node<T> node = new Node<T>(value);
            this.m_children.Add(node);
            node.SetParent(this);
            return node;
        }

        public List<Node<T>> Add(params T[] values)
        {
            List<Node<T>> list = new List<Node<T>>(values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                T value = values[i];
                Node<T> node = new Node<T>(value);
                this.m_children.Add(node);
                node.SetParent(this);
                list.Add(node);
            }
            return list;
        }

        public void Remove(Node<T> child)
        {
            child.SetParent(null);
            this.m_children.Remove(child);
        }

        public void RemoveAt(int index)
        {
            Node<T> node = this.m_children[index];
            if (node != null)
            {
                node.SetParent(null);
                this.m_children.RemoveAt(index);
            }
        }

        public void RemoveAll(int index)
        {
            for (int i = 0; i < this.m_children.Count; i++)
            {
                this.m_children[i].SetParent(null);
                this.m_children.RemoveAt(i);
            }
        }

        private void SetParent(Node<T> parent)
        {
            this.m_parent = parent;
        }

        public int GetNodeCount(bool includeSubNodes)
        {
            int result;
            if (!includeSubNodes)
            {
                result = this.m_children.Count;
            }
            else
            {
                int num = this.m_children.Count;
                foreach (Node<T> current in this.m_children)
                {
                    num += current.GetNodeCount(true);
                }
                result = num;
            }
            return result;
        }

        public List<Node<T>> GetLeafNodes()
        {
            List<Node<T>> list = new List<Node<T>>();
            foreach (Node<T> current in this.m_children)
            {
                if (current.GetNodeCount(false) == 0)
                {
                    list.Add(current);
                }
                else
                {
                    list.AddRange(current.GetLeafNodes());
                }
            }
            return list;
        }

        public List<List<Node<T>>> GetLeafRoads(bool includeThis, bool includeLeaf)
        {
            List<List<Node<T>>> list = new List<List<Node<T>>>();
            List<Node<T>> leafNodes = this.GetLeafNodes();
            foreach (Node<T> current in leafNodes)
            {
                List<Node<T>> list2 = new List<Node<T>>();
                Node<T> node;
                if (includeLeaf)
                {
                    node = current;
                }
                else
                {
                    node = current.m_parent;
                }
                if (includeThis)
                {
                    while ((node != null && node.m_parent != null) || node == this)
                    {
                        list2.Add(node);
                        node = node.m_parent;
                    }
                }
                else
                {
                    while (node != null && node.m_parent != null && node != this)
                    {
                        list2.Add(node);
                        node = node.m_parent;
                    }
                }
                list2.Reverse();
                list.Add(list2);
            }
            return list;
        }

        public List<U> GetCastedBestRoad<U>(Func<Node<T>, decimal?> selector, Converter<Node<T>, U> converter)
        {
            List<List<Node<T>>> leafRoads = this.GetLeafRoads(false, true);
            List<Node<T>> list = (
                from road in leafRoads
                orderby road.Sum(selector)
                select road).Last<List<Node<T>>>();
            return list.ConvertAll<U>(converter);
        }

        public List<Node<T>> GetBestRoad(Func<Node<T>, decimal?> selector)
        {
            List<List<Node<T>>> leafRoads = this.GetLeafRoads(false, true);
            return (
                from road in leafRoads
                orderby road.Sum(selector)
                select road).Last<List<Node<T>>>();
        }

        public Node<T> GetBestChild(Func<Node<T>, decimal?> selector)
        {
            return this.m_children.OrderBy(selector).Last<Node<T>>();
        }

        public List<Node<T>> GetRootRoad(bool includeThis, bool includeRoot)
        {
            List<Node<T>> list = new List<Node<T>>();
            Node<T> node;
            if (includeThis)
            {
                node = this;
            }
            else
            {
                node = this.m_parent;
            }
            if (includeRoot)
            {
                while (node != null)
                {
                    list.Add(node);
                    node = node.m_parent;
                }
            }
            else
            {
                while (node != null && node.m_parent != null)
                {
                    list.Add(node);
                    node = node.m_parent;
                }
            }
            return list;
        }
    }
}