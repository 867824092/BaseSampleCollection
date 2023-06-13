using System.Collections;

namespace DataStructure; 

public class DoubleLinkedList<T>  {
    public sealed class  Node {
        public T Element { get; set; }
        public Node? Prev { get; set; }
        public Node? Next { get; set; }

        public Node(T element) {
            Element = element;
        }
    }

    private int _count;
    private Node? _head;

    public void AddLast(T value) {
        Node newNode = new Node(value);
        if (_head == null) {
            newNode.Prev = newNode;
            newNode.Next = newNode;
            _head = newNode;
        } else {
            newNode.Next = _head;
            newNode.Prev = _head.Prev;
            _head.Prev!.Next = newNode;
            _head.Prev = newNode;
        }
        _count++;
    }

    public void Print() {
        Node? current = _head;
        int index = _count;
        while (index > 0) {
            Console.WriteLine("****************");
            Console.WriteLine($"上一节点：{current.Prev.Element}");
            Console.WriteLine($"当前节点：{current.Element}");
            Console.WriteLine($"下一节点：{current.Next.Element}");
            current = current.Next;
            index--;
        }
    }

    public static void Run() {
        DoubleLinkedList<int> list = new DoubleLinkedList<int>();
        list.AddLast(1);
        list.AddLast(2);
        list.AddLast(3);
        list.AddLast(4);
        list.Print();
    }

}