namespace DataStructure; 

public class SimpleBinaryTree {
    public class  TreeNode {
        public int Value { get; set; }
        public TreeNode? Left { get; set; }
        public TreeNode? Right { get; set; }

        public TreeNode(int val) {
            Value = val;
        }
    }
    public TreeNode? Root { get; private set; }

    public void Insert(int val) {
        TreeNode newNode = new TreeNode(val);
        if (Root == null) {
            Root = newNode;
        } else {
            TreeNode current = Root;
            TreeNode parent;
            while (true) {
                parent = current;
                if (val < current.Value) {
                    current = current.Left;
                    if (current == null) {
                        parent.Left = newNode;
                        break;
                    }
                } else if(val > current.Value) {
                    current = current.Right;
                    if (current == null) {
                        parent.Right = newNode;
                        break;
                    }
                }
            }
        }
    }
    
    public void TraverseInOrder(TreeNode node)
    {
        if (node != null)
        {
            TraverseInOrder(node.Left);
            Console.Write(node.Value + " ");
            TraverseInOrder(node.Right);
        }
    }

    public static void Run() {
        SimpleBinaryTree binaryTree = new SimpleBinaryTree();
        
        // 插入节点
        binaryTree.Insert(50);
        binaryTree.Insert(30);
        binaryTree.Insert(20);
        binaryTree.Insert(40);
        binaryTree.Insert(10);
        binaryTree.Insert(70);
        binaryTree.Insert(60);
        binaryTree.Insert(80);
        
        // 遍历二叉树（中序遍历）
        binaryTree.TraverseInOrder(binaryTree.Root);
    }
}