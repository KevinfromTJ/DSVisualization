using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfDS2
{


    //自己实现的栈
    public partial class MyStack
    {
        int maxSize ;//最大容量
        int stackPointer ;//栈顶指针相对位置
        int[] stack = null; //栈基址
        public MyStack()
        {
            maxSize = TechRouteGraph.MaxStackSize;
            stack =new int[maxSize];
            stackPointer = -1;

        }
        public void expandDouble()
        {
            int[] newStack = new int[maxSize * 2];
            for(int i=0; i<maxSize;i++)
            {
                newStack[i] = stack[i];
            }
            maxSize *= 2;
            stack = newStack;

        }
        public int Top()
        {
            if (!IsEmpty())
                return stack[stackPointer];
            else
                return int.MinValue;

        }
        public int GetLength()
        {
            return stackPointer+1;
        }
        public bool IsFull()
        {
            if (stackPointer == maxSize - 1)
                return true;
            else
                return false;
        }
        public bool IsEmpty()
        {
            if (stackPointer == - 1)
                return true;
            else
                return false;
        }
        public void Push(int index)
        {
            if(IsFull())
            {
                Console.WriteLine("需要先扩容");
                expandDouble();
            }
            stack[++stackPointer]=index;
            
        }
        public void Pop()
        {
            if (IsEmpty())
            {
                Console.WriteLine("栈是空的！");
                return;
            }
            stackPointer--;
        }
        public void Clear()
        {
            stackPointer=-1;
        }
    }

    //工艺路径类
    public static partial class TechRouteGraph
    {
        public static int MaxStackSize = 10000;


        public static VexNode[] VexArray { get; set; } //按从1开始的编号依次储存点
        public static bool[,] VexMatrix { get; set; } //邻接矩阵(0行0列不用)(无需权重，以01表示相连或不相连)

        public static int VexNum { get; set; }
        public static int ArcNum { get; set; }

        public static bool OrderValidity =true;
        public static int[] StartVex; //入度为0的点的集合
        public static MyStack EquipStack;
        public static MyStack DismantleStack;


        //顶点，记录了零件以及（标准装配次序下）装配了零件后的状态图（可能不止一个，比如不同数量车轮，在主函数中确定）
        public class VexNode
        {

            //public int EquipmentImageIdx { get; set; }//索引
            public int CarStateImageIdx { get; set; } //索引
            public int VexIndex { get; set; }
            public VexNode(int vexIndex = 0, int equipmentImageIdx = 0, int carStateImageIdx = 0)
            {
                this.VexIndex = vexIndex;
                this.CarStateImageIdx = carStateImageIdx;
                //this.EquipmentImageIdx = equipmentImageIdx;

            }
        }


        public static void InitData()
        {
            OrderValidity = true;
            VexArray = new VexNode[VexNum + 1];//0号不用
            VexMatrix = new bool[VexNum + 1, VexNum + 1];//初始化邻接矩阵（C#默认为0)
            //EquipStack.Clear();
            //DismantleStack.Clear();
            StartVex = new int[VexNum + 1];
            EquipStack =new MyStack();
            DismantleStack= new MyStack();

            //栈自动初始化

            for (int i = 0; i < VexNum + 1; i++)
            {
                VexArray[i] = new VexNode();
                for (int j = 0; j < VexNum + 1; j++)
                {
                        VexMatrix[i, j] = false;
                }
            }


            

        }

        //从获取信息并初始化
        public static void GetFrontData(int vexNum, int arcNum, in int[,] arcVexPair)
        {


            VexNum = vexNum; ArcNum = arcNum;
            InitData();

            for (int i = 0; i < ArcNum; i++)
            {
                int tailIdx = arcVexPair[i, 0];
                int headIdx = arcVexPair[i, 1];
                VexArray[tailIdx].VexIndex = tailIdx;
                VexArray[headIdx].VexIndex = headIdx;

                if (VexMatrix[tailIdx,headIdx ] != false)//重复路径跳过
                    continue;

                //注意是有向图
                VexMatrix[tailIdx,headIdx ] = true;
            }

            int k = 1;
            for(int j=1;j<VexNum+1 ;j++)
            {
                if (VexArray[j].VexIndex == 0)
                    continue;
                bool isStart = true;
                for (int i = 1; i < VexNum + 1; i++)
                {
                    if(VexMatrix[i,j]==true)
                    {
                        isStart = false;
                        break;
                    }
                }
                if (isStart)
                    StartVex[k++]=j;

            }
            Console.WriteLine("入度为0的点有：" + (k - 1).ToString());
        }

        

        //检查整个图是否有环
        public static bool IsValidOrder()
        {
            //别偷懒，之前只检查入度为0的点显然不对！
            int [] visited= new int[VexNum + 1];
            for(int k=1; k<VexNum+1;k++)
            {
                for (int i = 0; i<visited.Length;i++)
                {
                    visited[i] = 0;
                }
                GetOrderDfs(k, visited);


            }
            return OrderValidity;
        }


        //给定指定零件，得到其装配路径和拆解路径。
        public static void GetTechOrder(int vex)
        {
            DismantleStack.Clear();
            EquipStack.Clear();
            int[] visited = new int[VexNum+1];
            GetOrderDfs(vex,visited); 
   
        }















    }

    //基本函数
    public static partial class TechRouteGraph
    {
        //判断是否有环的dfs
        static void GetOrderDfs(int curVex, int[] visited)
        {
            if (!OrderValidity)
                return;
            visited[curVex] = 1;        
            for (int i = 1; i < VexNum + 1; i++)
            {
                if (VexMatrix[curVex, i] != false)
                {
                    if(visited[i] == 0)
                    {
                        if (!OrderValidity)
                            return;
                        GetOrderDfs(i, visited);
                    }
                    else if (visited[i] == 1)//已经访问过了
                    {
                        OrderValidity = false;
                        Console.WriteLine(curVex + "  " + i);
                        return;
                    }          
                }
            }
            visited[curVex] = 2;
            DismantleStack.Push(curVex);
        }
    }
}
