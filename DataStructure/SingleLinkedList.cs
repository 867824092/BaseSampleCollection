using System.Collections;

namespace DataStructure; 

public class SingleLinkedList<T> : IEnumerable {
    public sealed class Node {
        public T element;
        public Node? prev;

        public Node(T element) {
            this.element = element;
        }

        public void SetNode(Node node) {
            prev = node;
        }
    }
    private Node? head;
    private T _current;
    public int Count { get; private set; }
    public SingleLinkedList() {
        
    }
    public SingleLinkedList(IEnumerable<T> collection) {
        ArgumentNullException.ThrowIfNull(collection);
        foreach (var item in collection) {
            AddLast(item);
        }
    }
    public void AddLast(T value) {
        Node node = new Node(value);
        if (head != null) node.SetNode(head);
        head = node;
        Count++;
    }
    public Node? Find(T value) {
        Node? current = head;
        while (current != null) {
            if (current.element.Equals(value)) 
                return current;
            current = current.prev;
        }
        return null;
    }
    public Node AddBefore(Node node, T value) {
        Node newNode = new Node(value);
        if (node.prev != null) {
            newNode.SetNode(node.prev);
        }
        node.SetNode(newNode);
        Count++;
        return newNode;
    }
    public void Print() {
        Node? current = head;
        while (current != null) {
            Console.WriteLine(current.element);
            current = current.prev;
        }
    }
    public IEnumerator GetEnumerator() => new Enumerator(this);
    public static void Run() {
        var list = new SingleLinkedList<int>(new List<int>()
        {
        1,2,3
        });
        list.Print();
        Console.WriteLine("********************");
        var node = list.Find(3);
        node = list.AddBefore(node,4);
        list.AddBefore(node, 5);
        list.Print();
        Console.WriteLine("********************");
        foreach (int i in list) {
            Console.WriteLine(i);
        }
    }
    public class Enumerator : IEnumerator<T> {
        private SingleLinkedList<T> _list;
        private Node? _node;
        private int _count;
        public Enumerator(SingleLinkedList<T> list) {
            _list = list;
            _node = _list.head;
            _current = default;
        }
        public bool MoveNext() {
            if (_count == _list.Count)
                return false;
            _current = _node.element;
            _node = _node.prev;
            if (_node == _list.head) {
                _node = null;
            }

            _count++;
            return true;
        }

        public void Reset() {
            _count = 0;
            _node = _list.head;
        }

        private T _current;
        public T Current => _current;

        object IEnumerator.Current => Current;

        public void Dispose() { }
    }
}



