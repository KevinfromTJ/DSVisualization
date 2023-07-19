using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Shapes;

namespace WpfDS1
{
	
   


	//数据绑定+判断数据合法性+后台计算
	public partial class MainWindow : Window
	{
		public static int InputVexNum;
		public static int InputArcNum;
		public static int InputLineCnt;
		public static StringCollection ArcLines = new StringCollection();

		private bool HasCalAll = false;
		private bool HasDrawnAdjList = false;
        private bool IsDrawingSinSrcSpath = false;//在单源最短路径页面
		private bool IsDrawingAllSpath = false;//在多源最短路径页面


		public bool JudgeDataValidity()
        {
			if(InputArcNum!=InputLineCnt)
				return false;
			//待传入的弧信息
			int[,] arcVexPair = new int[InputArcNum, 2];
			double[] arcWeight = new double[InputArcNum];

			//Trace.WriteLine(arcLines);
			//Console.WriteLine("InputBox");
			//foreach (var i in ArcLines)
   //         {
			//	Console.WriteLine(i);
				
			//}
			//Console.WriteLine("\n\n***********\n\n");

			for (int i = 0; i < InputArcNum; i++)
			{
				if (ArcLines[i].IndexOf('\n')!=-1)
                {
					ArcLines[i]=ArcLines[i].Substring(0, ArcLines[i].IndexOf('\n'));

				}
				string[] arcInfo = ArcLines[i].Split(' ');
				//Console.WriteLine(arcInfo[0] + "**"+ arcInfo[1] + "**" + arcInfo[2]+"**"+ arcWeight[i]);
				bool vexIsValid = arcInfo.Length>=3
					&& int.TryParse(arcInfo[0], out arcVexPair[i, 0]) //尾
					&& int.TryParse(arcInfo[1], out arcVexPair[i, 1]) //头
					&& double.TryParse(arcInfo[2], out arcWeight[i]) //权重
					&& arcWeight[i] >= 0 //权重大于0
					&& arcVexPair[i, 0] >= 1 && arcVexPair[i, 0] <= InputVexNum
					&& arcVexPair[i, 1] >= 1 && arcVexPair[i, 1] <= InputVexNum;

				if (!vexIsValid)
					return false;
						
			}

			//送入后端
			Graph.GetFrontData(InputVexNum,InputArcNum,arcVexPair,arcWeight);


			return true;
        }

		//一次算完邻接链表+两种生成树数据+最短路径并存入
		public bool JudgeCalAndStore()
        {
			

			//读数据
			bool inputValid= int.TryParse(VexNumBox.Text,out InputVexNum)&& int.TryParse(ArcNumBox.Text, out InputArcNum)&&InputVexNum>0;
			 
			InputLineCnt = ArcGroupBox.LineCount;

			for (int line = InputLineCnt-1; line >=0; line--)
			{
				if (ArcGroupBox.GetLineText(line) == ""|| ArcGroupBox.GetLineText(line) == "\r\n" || ArcGroupBox.GetLineText(line) == " ")
				{
					InputLineCnt--;
				}
				Console.WriteLine(ArcGroupBox.GetLineText(line) + "***");
			}
			Console.WriteLine(InputLineCnt);

			for (int line = 0; line < InputLineCnt; line++)
			{
				ArcLines.Add(ArcGroupBox.GetLineText(line));
			}

			//输入不对就弹窗,否则送入数据到后台
			if (!inputValid||JudgeDataValidity() == false)
            {
				MessageBox.Show("输入不合法，请对照规则检查", "输入错误提示", MessageBoxButton.OK,MessageBoxImage.Error);
				ArcLines.Clear();
				return false;
			}


			//后台计算每个部分的结果
			Graph.MinSpanTreePRIM();
			Graph.MinSpanTreeKRU();
			//Graph.GetSinSrcShortestPath();
			Graph.GetAllShortestPath();

			return true;
			
        }

	}


	//放控件的事件
	public partial class MainWindow:Window
    {
		//第一个按钮
		private void BtnDataInit_Click(object sender, RoutedEventArgs e)
		{
			//进入到这个页面后当前后台数据和画的图不再有效		
			AnimeTimer.IsEnabled = false;

			HasCalAll = false;
			HasDrawnAdjList = false;
			//HasDrawnBasicGraph = false;
			IsDrawingAllSpath = false;
			IsDrawingSinSrcSpath = false;

			ArcLines.Clear();
			MainCanvas.Children.Clear();
			HintShortestPath = "";

			DataInputBoard.Visibility = Visibility.Visible;
			CanvasBoard.Visibility = Visibility.Collapsed;

		}


        //邻接链表
        private void BtnDrawAdjList_Click(object sender, RoutedEventArgs e)
		{
			AnimeTimer.IsEnabled = false;


            //检查合法性与后台计算
            if (!HasCalAll)
            {
                bool validData = JudgeCalAndStore();
                if (!validData)
                    return;

                HasCalAll = true;
            }


            if (HasDrawnAdjList)
            {
				return;
            }

			

			DrawAdjList();

			HasDrawnAdjList = true;
			//HasDrawnBasicGraph = false;
			IsDrawingAllSpath = false;
			IsDrawingSinSrcSpath = false;

			DataInputBoard.Visibility = Visibility.Collapsed;
			CanvasBoard.Visibility = Visibility.Visible;
		}

		private void BtnDrawMinSpanTree_Click(object sender, RoutedEventArgs e)
		{
			//检查合法性与后台计算
			if (!HasCalAll)
			{
				bool validData = JudgeCalAndStore();
				if (!validData)
					return;

				HasCalAll = true;
			}



			
			DrawStaticGraph(0);//0表示画生成树的两个图，1表示只画一个
			HasDrawnAdjList = false;
			//HasDrawnBasicGraph = false;
			IsDrawingAllSpath = false;
			IsDrawingSinSrcSpath = false;

			//没有最小生成树
			if (Graph.JudgeConnected() == false)
			{
				MessageBox.Show("不存在最小生成树", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}


			DataInputBoard.Visibility = Visibility.Collapsed;
			CanvasBoard.Visibility = Visibility.Visible;

			TimerCnt = 2;//初始化画图计数器
			CanvasRefresh = true;//允许刷新
			AnimeTimer.IsEnabled = true;//启动动画

		}

		private void BtnSinSourceShortestPath_Click(object sender, RoutedEventArgs e)
		{
			AnimeTimer.IsEnabled = false;//不需要动画效果
			//检查合法性与后台计算
			if (!HasCalAll)
			{
				bool validData = JudgeCalAndStore();
				if (!validData)
					return;

				HasCalAll = true;
			}

			//Console.WriteLine(HasDrawnBasicGraph.ToString()+"  "+HasCalAll.ToString());

			HintShortestPath = "";
			DrawStaticGraph(1);//0表示画生成树的两个图，1表示只画单源最短路径，2表示多源

			
			IsDrawingSinSrcSpath = true;
			IsDrawingAllSpath = false;
			HasDrawnAdjList = false;

			DataInputBoard.Visibility = Visibility.Collapsed;
			CanvasBoard.Visibility = Visibility.Visible;

		}

		private void BtnAllShortestPath_Click(object sender, RoutedEventArgs e)
		{
			AnimeTimer.IsEnabled = false;//不需要动画效果
			//检查合法性与后台计算
			if (!HasCalAll)
			{
				bool validData = JudgeCalAndStore();
				if (!validData)
					return;

				HasCalAll = true;
			}

			SetSPathHintText(0,0,3);//初始画出所有点之间的路径
			DrawStaticGraph(2);//0表示画生成树的两个图，1表示只画单源最短路径，2表示多源

			VexSelected = -1; VexLast = -1;
			VexCircleLast = null; VexCircleSelected = null;
			FullPath = new int[Graph.VexNum + 2];//最后留出一个0判断条件


			
			IsDrawingAllSpath= true;
			IsDrawingSinSrcSpath = false;
			HasDrawnAdjList = false;

			DataInputBoard.Visibility = Visibility.Collapsed;
			CanvasBoard.Visibility = Visibility.Visible;

		}

		private void SelectElement_MouseDown(object sender, MouseButtonEventArgs e)
		{
			
			//不是这两个页面的话点画布是没意义的
			if (!IsDrawingAllSpath && !IsDrawingSinSrcSpath)
				return;


			//初始化变量
			

			if (IsDrawingAllSpath)
            {
				DrawCoupleShortestPath();
				
            }

			if (IsDrawingSinSrcSpath)
			{
				DrawSinSrcShortestPath();
			}
		}

		private void BtnMenuReset_Click(object sender, RoutedEventArgs e)
		{
			

			
				
			AnimeTimer.IsEnabled = false;

			HasCalAll = false;
			HasDrawnAdjList = false;
			IsDrawingAllSpath = false;
			IsDrawingSinSrcSpath = false;

			ArcLines.Clear();
			MainCanvas.Children.Clear();
			HintShortestPath = "";

			DataInputBoard.Visibility = Visibility.Visible;
			CanvasBoard.Visibility = Visibility.Collapsed;

			//
			mainViewModel.Initialize();
			
		}

		private void BtnMenuExit_Click(object sender, RoutedEventArgs e)
		{
			Environment.Exit(0);
		}


		private void BtnMenuInstrution_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("1.默认或点击\"图数据输入\" 在输入框中输入【无向图】的顶点数、边数和边的信息\n" +
				"2.点击\"邻接链表\" 显示图的邻接链表\n" +
				"3.点击\"最小生成树(动态演示)\" 动态展示两种算法构造最小生成树的过程\n" +
				"4.点击\"单源最短路径\" 点击图中每一个点显示其到其他点的最短路径\n" +
				"5.点击\"任意最短路径\" 点击图中任意两个点显示它们之间的最短路径\n", "使用说明", MessageBoxButton.OK);
		}

		private void BtnMenuProgramInfo_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("图的相关数据结构与算法可视化\n版本：v1.0\n平台：.NET Framework 4.7.2（WPF）\n作者：陈君涛\n指导教师：柳先辉", "程序信息", MessageBoxButton.OK);
		}

	}

	//核心控制
    public partial class MainWindow : Window
    {
        public static bool CanvasRefresh = false;
		public static DispatcherTimer AnimeTimer = new DispatcherTimer();
		private static int TimerCnt = 2;

		//核心控制函数——根据当前状态决定是否刷新canvas，根据具体选项决定canvas上的内容
		private void AnimeTimer_Tick(object sender, EventArgs e)
		{
			if(!CanvasRefresh)
				return;
		
			DrawMinSpanTree(TimerCnt);//画最小生成树的一条边
			TimerCnt += 1;

			if(TimerCnt==Graph.VexNum+1)
				CanvasRefresh = false;

        }
        public void AnimeCtr()
        {
			//定义	
			AnimeTimer.Interval = TimeSpan.FromSeconds(1);
			AnimeTimer.Tick += AnimeTimer_Tick;
			//控制
			if (true)
				AnimeTimer.Start();	
		}


    }
}
