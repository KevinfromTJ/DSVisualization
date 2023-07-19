using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfDS1
{


    //无向图
    public static partial class Graph
    {
        public const double INF = double.MaxValue;
        public static int AdjListInitMaxLen = 100;


        public static VexNode[] VexArray { get; set; } //邻接表(0不用)
        public static double[,] VexMatrix { get; set; } //邻接矩阵(0行0列不用)
        public static int VexNum { get; set; }
        public static int ArcNum { get; set; }
        public static double[,] SPathLen { get; set; } //每对顶点最短路径长度储存(0行0列不用)
        public static int[,] PathTrace { get; set; } //任意两个点间最短路径经过点存储(0行0列不用)
        public static ArcNode[] ArcWgtList { get; set; }//用于Kruskal算法的边权重排序
        public static int[,] PrimOrder { get; set; } //记录Prim算法选边的顺序
        public static int[,] KruskalOrder { get; set; } //记录Kruskal算法选边的顺序


        //顶点
        public class VexNode
        {
            public ArcNode[] Arclist { get; set; }//邻接表
            public int ArclistIdx { get; set; }//索引
            public int VexData { get; set; } //顶点数据
            public int VexIndex { get; set; }
            public VexNode(int vexIndex=0, int vexData=0)
            {
                this.ArclistIdx = 0;
                this.VexIndex = vexIndex;
                this.VexData = vexData;
                this.Arclist = new ArcNode[AdjListInitMaxLen];//后面可以append,方便链表实现
                //for(int i=0; i< AdjListInitMaxLen; i++)
                //{
                //    this.Arclist[i] = new ArcNode();
                //}
                
            }
        }

        //无向弧(边)
        public class ArcNode
        {
            public int HeadIndex { get; set; }
            public int TailIndex { get; set; }

            private double weight;
            public double Weight
            {
                get
                {
                    return this.weight;
                }
                set
                {
                    if (value < 0)
                    {
                        Console.WriteLine("The weight of each arc(edge) in a graph can't be negative!!!");
                        System.Environment.Exit(-1);
                    }
                    else
                        this.weight = value;

                }
            }
            public ArcNode(int tailIndex=0, int headIndex=0, double weight = INF)
            {
                this.TailIndex = tailIndex;
                this.HeadIndex = headIndex;
                this.Weight = weight;

            }

        }

        //控制台输入信息并初始化
        public static void InitData()
        {

            AdjListInitMaxLen =ArcNum;
            VexArray = new VexNode[VexNum+1];//0号不用
            VexMatrix = new double[VexNum + 1, VexNum + 1];//初始化邻接矩阵（C#默认为0，非对角线要改无限大)
            SPathLen = new double[VexNum + 1, VexNum + 1];//初始化最短路径长度数组（C#默认为0，要改无限大)
            PathTrace = new int[VexNum + 1, VexNum + 1];//初始化最短路径结点数组
            ArcWgtList = new ArcNode[ArcNum];//初始化边权列表
            PrimOrder = new int[VexNum+1,2]; //2-VexNum :边形成的顺序
            KruskalOrder = new int[VexNum+1, 2]; //2-VexNum :边形成的顺序

            for (int i = 0; i < ArcNum; i++)
            {
                ArcWgtList[i] = new ArcNode();
            }
            for (int i = 0; i < VexNum + 1; i++)
            {
                VexArray[i] = new VexNode();
                for (int j = 0; j < VexNum + 1; j++)
                {
                    SPathLen[i, j] = INF;
                    if (i != j)
                        VexMatrix[i, j] = INF;

                }
            }


            /*string temp;

            for (int i = 0; i < ArcNum; i++)
            {
                //c#处理控制台输入只能使用读取字符串的方法，需要使用string
                temp = Console.ReadLine();
                string[] tempSplit = temp.Split(' ');
                int head = int.Parse(tempSplit[0]), tail = int.Parse(tempSplit[1]);
                double weight = double.Parse(tempSplit[2]);

                //读边
                ArcWgtList.Append(new ArcNode(head, tail, weight));


                //处理邻接表
                VexArray[head].Arclist.Append(new ArcNode(head, tail, weight));
                VexArray[tail].Arclist.Append(new ArcNode(tail, head, weight));

                //处理邻接矩阵
                VexMatrix[head, tail] = VexMatrix[tail, head] = weight;

            }*/

        }

        //从获取信息并初始化
        public static void GetFrontData(int vexNum,int arcNum, in int[,]arcVexPair,in double[] arcWeight)
        {
            

            VexNum = vexNum; ArcNum = arcNum;
            InitData();

            for (int i = 0; i < ArcNum; i++)
            {
                int tailIdx = arcVexPair[i, 0];
                int headIdx = arcVexPair[i, 1];

                if (VexMatrix[headIdx, tailIdx] != INF)
                    continue;


                //Console.WriteLine(tailIdx.ToString()+" "+ headIdx.ToString()+" "+ arcWeight[i].ToString());

                VexMatrix[headIdx, tailIdx] = VexMatrix[tailIdx, headIdx] = arcWeight[i];

                //加入边权列表，供Kruskal算法排序
                ArcWgtList[i] = new ArcNode(tailIdx, headIdx, arcWeight[i]);

                VexArray[headIdx].Arclist[VexArray[headIdx].ArclistIdx++]=new ArcNode(headIdx,tailIdx,arcWeight[i]);
                VexArray[tailIdx].Arclist[VexArray[tailIdx].ArclistIdx++] =new ArcNode(tailIdx, headIdx, arcWeight[i]);
            }
        }

        //判连通
        public static bool JudgeConnected()
        {
            if (VexNum == 1)
            {
                return true;
            }
            if (VexNum > ArcNum + 1)
            {
                return false;
            }
            //读入数据初始化后才能使用
            int[] visited = new int[VexNum + 1];
            Dfs(1, visited);

            for (int i = 1; i < VexNum + 1; i++)
            {
                if (visited[i] == 0)
                {
                    return false;
                }
            }
            return true;
        }




        //单源最短路径（dijkstra）
        public static void GetSinSrcShortestPath(int src)
        {

            //点从1-N编号，0单元未用
            //C#int数组默认初始化为0 
            int[] finished = new int[VexNum + 1];
            double[] curDis = new double[VexNum + 1];//src到其他点的当前求得最小距离

            finished[src] = 1;
            SPathLen[src, src] = 0; curDis[src] = 0;
            //初始化
            for (int i = 1; i < VexNum + 1; i++)
            {
                //finished[i] = 0;
                curDis[i] = VexMatrix[src, i];
                if (curDis[i] < INF)
                {
                    ;//PathTrace[src, i] = src;  PathTrace[src, i]= i;
                }

            }

            int curNearestVex = src;
            for (int i = 1; i < VexNum; i++)//遍历VexNum-1次
            {
                double curMin = INF;
                //选择起点
                for (int j = 1; j < VexNum + 1; j++)
                {
                    if (finished[j] == 0)
                    {
                        if (curDis[j] < curMin)
                        {
                            curMin = curDis[j];
                            curNearestVex = j;
                        }
                    }
                }
                finished[curNearestVex] = 1;

                //更新最短路径
                for (int k = 1; k < VexNum + 1; k++)
                {
                    if (finished[k] == 0 && VexMatrix[curNearestVex, k] != INF && curMin + VexMatrix[curNearestVex, k] < curDis[k])
                    {
                        curDis[k] = curMin + VexMatrix[curNearestVex, k];
                        //更新路径
                        for (int u = 1; u < VexNum + 1; u++)
                        {
                            ; //PathTrace[src, k] = PathTrace[src, curNearestVex], u];
                        }
                        //PathTrace[src, k, k] = 1;
                    }
                }
            }

        }


        //多源最短路(floyd)
        public static void GetAllShortestPath()
        {
            //初始化
            for (int i = 1; i < VexNum + 1; i++)
            {
                for (int j = 1; j < VexNum + 1; j++)
                {
                    SPathLen[i, j] = VexMatrix[i, j];
                    //省略三重遍历初始化路径记录
                    if (SPathLen[i, j] < INF)
                    {
                        PathTrace[i,j]=j;// PathTrace[j, i] = j;
                    }
                }

            }
            //
            for (int k = 1; k < VexNum + 1; k++)
            {
                for (int i = 1; i < VexNum + 1; i++)
                {
                    for (int j = 1; j < VexNum + 1; j++)
                    {
                        if (SPathLen[i, k] + SPathLen[k, j] < SPathLen[i, j])
                        {
                            SPathLen[i, j] = SPathLen[i, k] + SPathLen[k, j];
                            PathTrace[i, j] = PathTrace[i, k];
                        }
                    }
                }
            }

        }

        //最小生成树(Prim)
        public static void MinSpanTreePRIM()
        {
            bool[] treeVex = new bool[VexNum + 1];//已经在生成树中的点
            double[] minDis = new double[VexNum + 1];//到集合结点的最小权值
            int [] minDisTo= new int[VexNum + 1];//记录到集合中具体哪个结点
            double res = 0;//最小生成树权值

            treeVex[1] = true; 
            minDis[1] = 0; minDisTo[1]=1;
            for (int i = 2; i < VexNum + 1; i++)
            {
                minDis[i] = VexMatrix[1, i]; minDisTo[i] = 1;
            }

            //找其他点
            for (int i = 2; i < VexNum + 1; i++)
            {
                int tmp = -1;
                for (int j = 2; j < VexNum + 1; j++)
                {
                    //这点必须
                    if ((!treeVex[j]) && (tmp == -1 || (minDis[j] !=INF && minDis[j] < minDis[tmp])))
                        tmp = j;
                }

                treeVex[tmp] = true;

                //Console.WriteLine("tail: "+minDisTo[tmp].ToString()+"   head: "+tmp.ToString()+"  weight: "+ minDis[tmp].ToString());
                PrimOrder[i,0] = minDisTo[tmp]; PrimOrder[i, 1] = tmp;//记录顺序
                res += minDis[tmp];

                //更新最短距离
                for (int k = 1; k < VexNum + 1; k++)
                {
                    if ((!treeVex[k]) && VexMatrix[tmp, k] !=INF && VexMatrix[tmp, k] < minDis[k])
                    {
                        minDis[k] = VexMatrix[tmp, k]; minDisTo[k] = tmp;
                    }
                        
                }

            }

        }

        //最小生成树(Kruskal)
        public static void MinSpanTreeKRU()
        {
            UnionSet USet = new UnionSet(VexNum + 1);
            double res = 0;//最小生成树权值

            //排序边权重小到大
            for (int i = ArcWgtList.Length - 1; i >= 0; i--)
            {
                for (int j = 0; j < i; j++)
                {
                    if (ArcWgtList[j].Weight > ArcWgtList[j + 1].Weight)
                    {
                        int tmpHead = ArcWgtList[j].HeadIndex;
                        int tmpTail = ArcWgtList[j].TailIndex;
                        double tmpWeight = ArcWgtList[j].Weight;

                        ArcWgtList[j].HeadIndex = ArcWgtList[j + 1].HeadIndex;
                        ArcWgtList[j].TailIndex = ArcWgtList[j + 1].TailIndex;
                        ArcWgtList[j].Weight = ArcWgtList[j + 1].Weight;

                        ArcWgtList[j + 1].HeadIndex = tmpHead;
                        ArcWgtList[j + 1].TailIndex = tmpTail;
                        ArcWgtList[j + 1].Weight = tmpWeight;

                    }
                }
            }

            //选n-1个边
            int cnt = 0;//计数选够了n-1个边
            for (int i = 0; i < ArcWgtList.Length; i++)
            {
                //选不在同一连通分量的边
                if (USet.Find(ArcWgtList[i].HeadIndex) != USet.Find(ArcWgtList[i].TailIndex))
                {
                    USet.Union(ArcWgtList[i].HeadIndex, ArcWgtList[i].TailIndex);
                    KruskalOrder[cnt + 2, 0] = ArcWgtList[i].HeadIndex; KruskalOrder[cnt + 2, 1] = ArcWgtList[i].TailIndex;//加入结果中
                    res += ArcWgtList[i].Weight;
                    cnt++;
                }
                //够了n-1条边
                if (cnt >= VexNum - 1)
                    break;
            }

        }
    }


    public partial class Graph
    {


        //并查集
        class UnionSet
        {
            private int[] Vex { get; set; }
            public UnionSet(int n)
            {
                Vex = new int[n];
                for (int i = 0; i < n; i++)
                {
                    Vex[i] = i;
                }
            }

            //找最先的祖宗
            public int Find(int x)
            {
                return (Vex[x] == x) ? x : Find(Vex[x]);
            }

            //合并
            public void Union(int x, int y)
            {
                int fx = Find(x), fy = Find(y);
                if (fx != fy)
                    Vex[fx] = fy;
            }

        }

        static void Dfs(int curVex, int[] visited)
        {
            visited[curVex] = 1;
            for (int i = 1; i < VexNum + 1; i++)
            {
                if (VexMatrix[curVex, i] != INF && visited[i] == 0)
                {
                    Dfs(i, visited);
                }
            }
        }

        //public static void find(int x, int y)
        //{
        //    int cnt = 1;
        //    int z = PathTrace[x,y];
        //    Console.Write(x);
        //    while (z != y)
        //    {
        //        Console.Write("  "+z);
        //        z = PathTrace[z,y];
        //    }
        //    Console.Write("  " + y+" ");
        //}
        //
        public static bool TraceShortestPath(int src,int des,ref int[] path)
        {
            int cnt = 1;
            
            path[cnt++]=src  ;
            //Console.Write(path[cnt-1] + " ");
            int tmp = PathTrace[src, des];
            if (tmp==0)
                return false;
            while (tmp!=des)
            {
                
                path[cnt++] = tmp;
                //Console.Write(tmp + " ");
                tmp = PathTrace[tmp, des];
                
                
            }
            path[cnt] =des;

            return true;
        }


        //输出邻接表
        public static void PrintAdjList(int vexNum, int arcNum)
        {
            for (int i = 1; i <= vexNum; i++)
            {
                Console.Write("vex:{0:d}  ", i);
                for (int j = 0; j < VexArray[i].Arclist.Length; j++)
                    Console.WriteLine("| {0:d} , {1:d} | ", VexArray[i].Arclist[j].HeadIndex, VexArray[i].Arclist[j].Weight);

                Console.WriteLine();
            }
        }



    }


    public class Print1
    {
        //static void Main(string[] args)
        //{
        //    Print1.Print();

        //}
        public static void Print()
        {
            Console.WriteLine("Hello World");
        }

    }


}
