using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Drawing;

//画图相关
namespace WpfDS1
{
	//定义画图的各种元素常量和数学方法
	public class DrawCal
	{
		public static double ArrowHeadLenProp = 10;//箭头的线长度比例
		public static double ArrowHeadRotate = 30.0 / 180 * Math.PI;
		

		//返回旋转后的尾部点【固定箭头长度】
		public static Point RotateVec(Point tail, Point head, double RotateAngle)
		{
			double dx = tail.X - head.X, dy = tail.Y - head.Y;

			double Rdx = dx * Math.Cos(RotateAngle) + dy * Math.Sin(RotateAngle);
			double Rdy = -dx * Math.Sin(RotateAngle) + dy * Math.Cos(RotateAngle);

			//normalize
			double factor = 1 / Math.Sqrt(Rdx * Rdx + Rdy * Rdy);
			Rdx *= factor; Rdy *= factor;

			return new Point(head.X + ArrowHeadLenProp * Rdx, head.Y + ArrowHeadLenProp * Rdy);
		}

		//角度弧度转换
		public static double Rad2Deg(double radian)
		{
			return radian / Math.PI * 180;
		}
		public static double Deg2Rad(double degree)
		{
			return degree / 180 * Math.PI;
		}


		/*下面是具体画图坐标选择【函数参数通过0,1选择颜色】*/
		public static double DrawAdjListStartLeft =30; //画链表的起始X
		public static double DrawAdjListStartTop =50;  //画链表的起始Y
		public static double SquareSidelen = 50;//结点正方形边长
		public static double RowInv = 50;//行间距
		public static double ArrowLen = 50;//画邻接表默认箭头长度（都是水平的）
		public static SolidColorBrush DefaultBGColor = Brushes.LightGoldenrodYellow;//点、链表结点默认填充颜色
		public static SolidColorBrush DefaultLHeadColor = Brushes.Red;//链表头默认填充颜色（也是字体强调颜色和选中点的颜色）


		public static double DrawGraphStartLeft = 30; //画树和图的起始X
		public static double DrawGraphStartTop = 100;  //画树和图的起始Y

		public static double VexRadius = 50;//顶点半径
		public static SolidColorBrush DefaultArcColor = Brushes.DimGray;//默认边颜色
		public static SolidColorBrush StressArcColor = Brushes.Black;//强调（生成树或最短路径）边颜色

		public static SolidColorBrush DefaultArcWeakColor = Brushes.LightCyan;//最短路径虚化边颜色
		public static SolidColorBrush DefaultTextWeakColor = Brushes.DimGray;//最短路径虚化边颜色

		public static SolidColorBrush VexSelectedColor = Brushes.OrangeRed;//最短路径虚化边颜色
		public static SolidColorBrush TextBorderColor = Brushes.Black;//最短路径虚化边颜色
		public static SolidColorBrush HintTextColor = Brushes.Black;//最短路径虚化边颜色

		public static double DefaultGraphHintTop = 50;//最小生成树的文字说明及最短路径的文字说明的Y（X同画树和图的起始X）
		public static double GraphDisplayAreaSideLen = 480;//一个图的展示范围，同时也是多个图在画布上相对左侧图的整体X位移
	}

	//定义点
	public class Point
	{
		public double X { get; set; }
		public double Y { get; set; }
		public Point(double x = 0, double y = 0)
		{
			X = x; Y = y;
		}
	}


	public partial class MainWindow : Window
	{
		/*静态变量*/
		public static Point[] VexPoints;

		public static string HintShortestPath="";//最短路相关信息输出

		public static int VexSelected=-1;//当前选中的点
		public static int VexLast = -1;//上次选中的点
		public static int[] FullPath ;//路径
		public static Ellipse VexCircleLast =null;
		public static Ellipse VexCircleSelected = null;




		/*函数*/
		//给静态变量HintShortestPath赋值 choice=1为单源
		public void SetSPathHintText(int vex1,int vex2,int choice= 1)
		{


			int[]path = new int[Graph.VexNum+2];
			HintShortestPath = "";

			if (choice==3)
            {
				for(int t=1; t<Graph.VexNum+1;t++)
                {
					string tmp = "点：" + t.ToString()+"\n";
					for (int i = 1; i < Graph.VexNum + 1; i++)
					{
						//清空上一次
						for (int j = 0; j < Graph.VexNum + 2; j++)
							path[j] = 0;
						bool hasPath = Graph.TraceShortestPath(t, i, ref path);

						string vexDis = "到 " + i.ToString() + " 的最短距离：";

						string vexPath = "路径：";
						if (hasPath)
						{
							vexDis += Graph.SPathLen[t, i].ToString();
							vexPath += t.ToString();
							for (int k = 1; path[k + 1] != 0; k++)
							{
								vexPath += " --> " + path[k + 1];
							}
						}
						else
							vexDis += "不存在";

						tmp +=  vexDis + "  " + vexPath+'\n';
					}
					HintShortestPath += tmp;
				}
            }


			//单源路
			//【格式】"选择的点：2\n到6最短距离：100 路径：2 --> 4 --> 6\n到2最短距离：0 路径： "
			if (choice == 1)
            {
				
				HintShortestPath = "选择的点：" + vex1.ToString();
				for (int i=1; i<Graph.VexNum+1;i++)
                {
					//清空上一次
					for (int j = 0; j < Graph.VexNum + 2; j++)
						path[j] = 0;
					bool hasPath = Graph.TraceShortestPath(vex1,i, ref path);
					
					string vexDis = "到 "+i.ToString() + " 的最短距离：";

					string vexPath = "路径：";
					if (hasPath)
					{
						vexDis += Graph.SPathLen[vex1, i].ToString();
						vexPath += vex1.ToString();
						for (int k = 1; path[k + 1] != 0; k++)
						{
							vexPath += " --> " + path[k + 1];
						}
					}
					else
						vexDis += "不存在";

					HintShortestPath +=  "\n" + vexDis+"  " + vexPath;
				}
                
            }

			//多源
			//【格式】"选择的点：2 6\n最短距离：100\n路径：2 --> 4 --> 6 "
			if (choice == 2)
			{
				//清空上一次
				for (int j = 0; j < Graph.VexNum + 2; j++)
					path[j] = 0;
				bool hasPath = Graph.TraceShortestPath(vex1, vex2,ref path);
				string vexSel = "选择的点：" + vex1.ToString() + " " + vex2.ToString();
				string vexDis = "最短距离：";
				
				string vexPath = "路径：";
				if (hasPath)
				{
					vexDis += Graph.SPathLen[vex1, vex2].ToString();
					vexPath += vex1.ToString();
					for (int i = 1; path[i + 1] != 0; i++)
					{
						vexPath += " --> " + path[i + 1];
					}
				}
				else
					vexDis +="不存在";

				HintShortestPath = vexSel + "\n" + vexDis+"\n"+vexPath;


			}

			//把字加到正确的textblock上
			foreach (UIElement el in MainCanvas.Children)
			{
				Console.WriteLine(el.GetType());
				if (el != null && el.GetType() == typeof(Border))
				{
					Border bd = (Border)el;
					TextBlock tb = (TextBlock)bd.Child;

					
					string[] key = tb.Name.Split('_');
					if (key[1] == "dynamic")
					{


						tb.Text = HintShortestPath;
						return;

					}
				}

			}

		}

		//画不带箭头的线
		public void DrawLine(Point tail, Point head, string text = "", int ArcColor = 0, string name = "")
		{
			//Point tail=new Point(350,500),head=new Point(410,522);

			//Grid grid = new Grid();

			//线上的字
			TextBlock textBlock = new TextBlock()
			{
				
				Text = text,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				TextAlignment = TextAlignment.Center,
				RenderTransform = new RotateTransform(DrawCal.Rad2Deg(Math.Atan((head.Y - tail.Y) / (head.X - tail.X)))),//让字与箭头指向平行
																														 //Foreground = Brushes.Red,

			};



			//画线和箭头
			Line lineCenter = new Line()
			{
				X1 = tail.X,
				Y1 = tail.Y,
				X2 = head.X,
				Y2 = head.Y,
				//Stroke = Brushes.Black,



			};
			if (ArcColor == 0)
			{
				textBlock.Foreground = DrawCal.DefaultTextWeakColor;
				lineCenter.Stroke = DrawCal.DefaultArcWeakColor;
				lineCenter.StrokeThickness = 2;
			}
			else
			{
				textBlock.Foreground = Brushes.Red;
				lineCenter.Stroke = Brushes.Black;
				lineCenter.StrokeThickness = 3;
			}

			if(name!="")
            {
				textBlock.Name = "Weight_" + name;
				lineCenter.Name = "Arc_" + name;
			}


			Canvas.SetLeft(textBlock, (tail.X + head.X) / 2 - text.Length * 4);//调出来比较正常的宽度
			Canvas.SetTop(textBlock, (tail.Y + head.Y) / 2);
			Canvas.SetZIndex(lineCenter, 0);
			Canvas.SetZIndex(textBlock, 5);
			if(text== "普里姆算法")
            {
				Console.WriteLine(((tail.X + head.X) / 2 - text.Length * 4)+"  "+ ((tail.Y + head.Y) / 2));
            }
			MainCanvas.Children.Add(lineCenter);
			MainCanvas.Children.Add(textBlock);

		}

		//画箭头
		public void DrawArrow(Point tail, Point head, string text = "")
		{
			//Point tail=new Point(350,500),head=new Point(410,522);

			//Grid grid = new Grid();

			//线上的字
			TextBlock textBlock = new TextBlock()
			{
				Text = text,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				TextAlignment = TextAlignment.Center,
				RenderTransform = new RotateTransform(DrawCal.Rad2Deg(Math.Atan((head.Y - tail.Y) / (head.X - tail.X)))),//让字与箭头指向平行
				Foreground = Brushes.Red,

			};

			Rectangle retc = new Rectangle()
			{

				Width = 100,
				Height = 200,
				Fill = Brushes.LightGoldenrodYellow,
				Stroke = Brushes.Black,
				StrokeThickness = 1,


			};


			//画线和箭头
			Line lineCenter = new Line()
			{
				X1 = tail.X,
				Y1 = tail.Y,
				X2 = head.X,
				Y2 = head.Y,
				Stroke = Brushes.Black,
				StrokeThickness = 2,


			};

			Point tailLeft = DrawCal.RotateVec(tail, head, DrawCal.ArrowHeadRotate);
			Line lineLeft = new Line()
			{
				X1 = tailLeft.X,
				Y1 = tailLeft.Y,
				X2 = head.X,
				Y2 = head.Y,
				Stroke = Brushes.Black,
				StrokeThickness = 2,
			};

			Point tailRight = DrawCal.RotateVec(tail, head, -DrawCal.ArrowHeadRotate);
			Line lineRight = new Line()
			{
				X1 = tailRight.X,
				Y1 = tailRight.Y,
				X2 = head.X,
				Y2 = head.Y,
				Stroke = Brushes.Black,
				StrokeThickness = 2,


			};


			Canvas.SetLeft(textBlock, (tail.X + head.X) / 2 - text.Length * 4);
			Canvas.SetTop(textBlock, (tail.Y + head.Y) / 2);
			Canvas.SetZIndex(textBlock, 0);

			MainCanvas.Children.Add(lineCenter);
			MainCanvas.Children.Add(lineLeft);
			MainCanvas.Children.Add(lineRight);
			MainCanvas.Children.Add(textBlock);
			//MainCanvas.Children.Add(grid);
		}

		//画矩形
		public void DrawRectangle(Point leftTop, double width = 50, double height = 50, string text = "", int BGColorSelect = 0)
		{
			Grid grid = new Grid();
			//字
			TextBlock nodeText = new TextBlock()
			{
				Text = text,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				TextAlignment = TextAlignment.Center,
				//Parent =,//不能给它加父
			};



			Rectangle retc = new Rectangle()
			{

				Width = width,
				Height = height,


				Stroke = Brushes.Black,
				StrokeThickness = 1,

				//DataContext = "哈哈哈" //这个属性设置貌似没用
			};
			if (BGColorSelect == 0)//链表头
			{
				retc.Fill = DrawCal.DefaultLHeadColor;
			}
			else //其他时候
			{
				retc.Fill = DrawCal.DefaultBGColor;

			}


			Canvas.SetLeft(grid, leftTop.X);//注意要设置grid
			Canvas.SetTop(grid, leftTop.Y);

			//Canvas.SetLeft(nodeText, 300);
			//Canvas.SetTop(nodeText, 400);

			grid.Children.Add(retc);
			grid.Children.Add(nodeText);

			MainCanvas.Children.Add(grid);



		}

		//画圆形(点)
		public void DrawCircle(Point center, double radius = 25, string text = "", int BGColorSelect = 0,string name="")
		{
			Grid grid = new Grid();
			//字
			TextBlock nodeText = new TextBlock()
			{
				Text = text,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				TextAlignment = TextAlignment.Center,
				//Parent =,//不能给它加父
			};



			Ellipse ellipse = new Ellipse()
			{

				Width = radius,

				Height = radius,

				Fill = Brushes.Gray,

				Stroke = Brushes.Black,
				StrokeThickness = 1,

				//DataContext = "哈哈哈" //这个属性设置貌似没用
			};
			if (BGColorSelect == 0)
			{
				ellipse.Fill = DrawCal.DefaultBGColor;
			}
			else //正常或动画选中
			{
				ellipse.Fill = DrawCal.DefaultLHeadColor;
			}

			//靠名字来找
			if(name!="")
            {
				ellipse.Name = "Vex_"+name;

			}


			Canvas.SetLeft(ellipse, center.X);//额外设置，后面要用ellipse的位置
			Canvas.SetTop(ellipse, center.Y);
			Canvas.SetLeft(grid, center.X);//注意要设置grid
			Canvas.SetTop(grid, center.Y);
			Canvas.SetZIndex(grid, 3);

			//Canvas.SetLeft(nodeText, 300);
			//Canvas.SetTop(nodeText, 400);

			grid.Children.Add(ellipse);
			grid.Children.Add(nodeText);

			MainCanvas.Children.Add(grid);
		}

		//画邻接链表(已经判断数据合法再调用)
		public void DrawAdjList()
		{
			MainCanvas.Children.Clear();
			Point curLeftTop = new Point(DrawCal.DrawAdjListStartLeft, DrawCal.DrawAdjListStartTop); //正方形的左上角，箭头的尾部
			;
			for (int i = 1; i < Graph.VexArray.Length; i++)
			{
				//回到每行初始点
				curLeftTop.Y = DrawCal.DrawAdjListStartTop + i * DrawCal.RowInv - 40;
				curLeftTop.X = DrawCal.DrawAdjListStartLeft;

				DrawRectangle(curLeftTop, DrawCal.SquareSidelen, DrawCal.SquareSidelen, i.ToString(), 0);
				curLeftTop.X += DrawCal.SquareSidelen;
				curLeftTop.Y += DrawCal.SquareSidelen / 2;

				for (int j = 0; j < Graph.VexArray[i].ArclistIdx; j++)
				{
					int headIdx = Graph.VexArray[i].Arclist[j].HeadIndex;
					double weight = Graph.VexArray[i].Arclist[j].Weight;

					Console.WriteLine(i.ToString() + "   " + headIdx.ToString() + "   " + weight.ToString());


					DrawArrow(curLeftTop, new Point(curLeftTop.X + DrawCal.ArrowLen, curLeftTop.Y), weight.ToString());
					curLeftTop.X += DrawCal.ArrowLen;
					curLeftTop.Y -= DrawCal.SquareSidelen / 2;

					DrawRectangle(curLeftTop, DrawCal.SquareSidelen, DrawCal.SquareSidelen, headIdx.ToString(), 1);
					curLeftTop.X += DrawCal.SquareSidelen;
					curLeftTop.Y += DrawCal.SquareSidelen / 2;
				}

			}


			//在点击事件函数里就判断了
			//if(JudgeDataValidity()!=true)
			//         {
			//	;
			//         }



			HasDrawnAdjList = true;
		}

		//画文字
		public void DrawTextBox(Point leftTop,string text="",string name="",double fontsize=18)
        {
            Border border = new Border()
            {
                BorderBrush = DrawCal.TextBorderColor,
                BorderThickness = new Thickness(2, 2, 2, 2),
            };



			TextBlock textBlock = new TextBlock()
			{

				Text = text,
				FontSize= fontsize,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				TextAlignment = TextAlignment.Left,
				Name = name,
				Foreground= DrawCal.HintTextColor,

			};
			
			
			//默认只有一个子项，所以要这么写
			border.Child =textBlock;

			Canvas.SetLeft(border,leftTop.X);
			Canvas.SetTop(border, leftTop.Y);


			
			MainCanvas.Children.Add(border);
		}

		//画出背景图（定位）
		public void DrawStaticGraph(int choice = 0)
		{
			MainCanvas.Children.Clear();
			//
			double blockOffset = 0;//定义点分块的偏移量
			int blockPerRow = 0;//一行（一列）几块

			//规划大概位置【待定】
			Random randSeed = new Random();


			VexPoints = new Point[Graph.VexNum + 1];//与点编号对应，0不用
			for (int i = 1; i < Graph.VexNum + 1; i++)
				VexPoints[i] = new Point();


			//根据多少将480x480分块
			if (Graph.VexNum <= 4)
				blockPerRow = 2;
			else if (Graph.VexNum <= 9)
				blockPerRow = 3;
			else if (Graph.VexNum <= 16)
				blockPerRow = 4;
			else if (Graph.VexNum <= 25)
				blockPerRow = 5;
			else
				blockPerRow = 10;

			blockOffset = DrawCal.GraphDisplayAreaSideLen / blockPerRow;

			double offsetX = DrawCal.GraphDisplayAreaSideLen;//Kruskal的图相对Prim整体向右偏

			//随机生成Graph.VexNum个坐标的相对
			int vexIdx = 1;
			for (int i = 0; i < 10000000; i++)
			{
				double ranCoordXY = randSeed.NextDouble() * blockOffset / 2;//防止重叠
				if (i % 100 == 0 && i % 200 != 0)
				{
					VexPoints[vexIdx].X = DrawCal.DrawGraphStartLeft + ranCoordXY + blockOffset * ((vexIdx - 1) % blockPerRow);

					DrawCircle(VexPoints[vexIdx], DrawCal.VexRadius, vexIdx.ToString(), 0,vexIdx.ToString());//画Prim的图和最短路径图上的点
					if (choice == 0)
						DrawCircle(new Point(offsetX + VexPoints[vexIdx].X, VexPoints[vexIdx].Y), DrawCal.VexRadius, vexIdx.ToString(), 0);//画Kruskal对应点

					vexIdx++;
				}
				else if (i % 200 == 0)
					VexPoints[vexIdx].Y = DrawCal.DrawGraphStartTop + ranCoordXY + blockOffset * ((vexIdx - 1) / blockPerRow);


				if (vexIdx > Graph.VexNum)
					break;
			}

			int cnt = 0;

			if(choice==0)
				DrawTextBox(new Point(3 * DrawCal.DrawGraphStartLeft, DrawCal.DefaultGraphHintTop), "普里姆算法", "Hint_static");
			else if(choice==1)
				DrawTextBox(new Point(3 * DrawCal.DrawGraphStartLeft, DrawCal.DefaultGraphHintTop), "点击黄色区域查看该源最短路径","Hint_static");
			else
				DrawTextBox(new Point(3 * DrawCal.DrawGraphStartLeft, DrawCal.DefaultGraphHintTop), "点击两个点显示它们之间的最短路径", "Hint_static");

			//画Prim的所有边
			for (int i = 1; i < Graph.VexNum + 1; i++)
			{
				for (int j = 1; j <= i; j++)
				{
					if (Graph.VexMatrix[i, j] != Graph.INF&&i!=j)
					{

						DrawLine(
							new Point(VexPoints[i].X + DrawCal.VexRadius / 2, VexPoints[i].Y + DrawCal.VexRadius / 2),
							new Point(VexPoints[j].X + DrawCal.VexRadius / 2, VexPoints[j].Y + DrawCal.VexRadius / 2),
							Graph.VexMatrix[i, j].ToString(), 0,i.ToString()+"_"+j.ToString());
						cnt++;
					}
				}
			}
			Console.WriteLine(cnt);


			
			if (choice == 0)
			{
				//画Kruskal的所有边
				for (int i = 1; i < Graph.VexNum + 1; i++)
				{
					for (int j = 1; j <= i; j++)
					{
						if (Graph.VexMatrix[i, j] != Graph.INF && i != j)
						{

							DrawLine(
								new Point(offsetX + VexPoints[i].X + DrawCal.VexRadius / 2, VexPoints[i].Y + DrawCal.VexRadius / 2),
								new Point(offsetX + VexPoints[j].X + DrawCal.VexRadius / 2, VexPoints[j].Y + DrawCal.VexRadius / 2),
								Graph.VexMatrix[i, j].ToString(), 0);
						}
					}
				}

				
				//写提示
				
				

			}


			if (choice == 0)
				DrawTextBox(new Point(3 * DrawCal.DrawGraphStartLeft + DrawCal.GraphDisplayAreaSideLen, DrawCal.DefaultGraphHintTop), "克鲁斯卡尔算法", "Hint_dynamic");
			else if (choice == 1)
				DrawTextBox(new Point(3 * DrawCal.DrawGraphStartLeft + DrawCal.GraphDisplayAreaSideLen, DrawCal.DefaultGraphHintTop), HintShortestPath, "Hint_dynamic");
			else
				DrawTextBox(new Point(3 * DrawCal.DrawGraphStartLeft + DrawCal.GraphDisplayAreaSideLen, DrawCal.DefaultGraphHintTop), HintShortestPath, "Hint_dynamic");

		}

		//画两种最小生成树(动态)
		public void DrawMinSpanTree(int no = 2)
		{
			//规划大概位置
			double offsetX = DrawCal.GraphDisplayAreaSideLen;//Kruskal的图相对Prim整体向右偏

			
			//画Prim的生成树
			int tailIdx = Graph.PrimOrder[no, 0];
			int headIdx = Graph.PrimOrder[no, 1];
			DrawLine(
						new Point(VexPoints[tailIdx].X + DrawCal.VexRadius / 2, VexPoints[tailIdx].Y + DrawCal.VexRadius / 2),
						new Point(VexPoints[headIdx].X + DrawCal.VexRadius / 2, VexPoints[headIdx].Y + DrawCal.VexRadius / 2),
						Graph.VexMatrix[tailIdx, headIdx].ToString(), 1);
			//画Kruskal的生成树
			tailIdx = Graph.KruskalOrder[no, 0];
			headIdx = Graph.KruskalOrder[no, 1];
			DrawLine(
						new Point(offsetX + VexPoints[tailIdx].X + DrawCal.VexRadius / 2, VexPoints[tailIdx].Y + DrawCal.VexRadius / 2),
						new Point(offsetX + VexPoints[headIdx].X + DrawCal.VexRadius / 2, VexPoints[headIdx].Y + DrawCal.VexRadius / 2),
						Graph.VexMatrix[tailIdx, headIdx].ToString(), 1);
		}

		public void DrawSinSrcShortestPath()
		{
			//获取鼠标点中的点
			UIElement targetElement = Mouse.DirectlyOver as UIElement;
			if (targetElement != null && targetElement.GetType() == typeof(Ellipse))
			{
				Ellipse curVexCircle = (Ellipse)targetElement;
				int curVex = int.Parse(curVexCircle.Name.Split('_')[1]);

				

				
				//先把上次的清除
				foreach (UIElement child in MainCanvas.Children)
				{

					if(child.GetType()== typeof(Grid))
                    {
						Grid gr = (Grid)child;
						//因为有字和圆圈，还要遍历。。。
						foreach(UIElement grChild in gr.Children)
                        {
							if (grChild.GetType() == typeof(Ellipse))
                            {
								Ellipse lastVexCircle = (Ellipse)grChild;
								lastVexCircle.Fill = DrawCal.DefaultBGColor;
							}
								
						}
						

					}
				}

				//设置这一次的点
				curVexCircle.Fill = DrawCal.DefaultLHeadColor;

				//输出路径
				SetSPathHintText(curVex, 0,1);


			}

			
		}

		//画任意两点的最短路径(传入的下一个点)
		public void DrawCoupleShortestPath()
		{
			
			

			//获取鼠标点中的点
			UIElement targetElement = Mouse.DirectlyOver as UIElement;
			//Console.WriteLine(targetElement.GetType().ToString());
			if (targetElement != null && targetElement.GetType() == typeof(Ellipse))
			{
				Ellipse curVexCircle = (Ellipse)targetElement;
				int curVex =int.Parse(curVexCircle.Name.Split('_')[1]);

				//Console.WriteLine("$$$$"+ VexLast+ "  "+ VexSelected + "  " + curVex);

				//第一次
				if (VexSelected==-1)
                {
					curVexCircle.SetValue(Ellipse.FillProperty, Brushes.OrangeRed);

					VexCircleSelected= curVexCircle;

					VexSelected = curVex;
					
					return;
				}

				//去掉上一次的边
				foreach(UIElement child in MainCanvas.Children)
                {
					if (child.GetType() == typeof(TextBlock))
					{
                        TextBlock tb = (TextBlock)child;
						string[] str = tb.Name.Split('_');
						for (int i = 1; FullPath[i+1] != 0; i++)
						{
							
							if ((int.Parse(str[1])== FullPath[i] && int.Parse(str[2]) == FullPath[i+1]) ||
								(int.Parse(str[2]) == FullPath[i] && int.Parse(str[1]) == FullPath[i + 1]))
                            {
								//Console.WriteLine(tb.Name);
								child.SetValue(TextBlock.ForegroundProperty, DrawCal.DefaultTextWeakColor);
							}
							
						}
						

					}
					else if (child.GetType() == typeof(Line))
					{
                        Line li = (Line)child;
						string[] str = li.Name.Split('_');
						//Console.WriteLine(li.Name);
						Console.WriteLine(FullPath.Length);
						for (int i = 1; FullPath[i + 1] != 0; i++)
						{
							
							if ((int.Parse(str[1]) == FullPath[i] && int.Parse(str[2]) == FullPath[i + 1]) ||
								(int.Parse(str[2]) == FullPath[i] && int.Parse(str[1]) == FullPath[i + 1]))
							{
								child.SetValue(Line.StrokeProperty, DrawCal.DefaultArcWeakColor);
							}
						}
							
							
					}
					
				}
				//并清空
				for (int i=0;i<FullPath.Length;i++)
                {
					FullPath[i] = 0;
                }

				//输出路径显示信息
				SetSPathHintText(VexSelected, curVex,2);

				if (VexSelected == curVex)
				{
					//虽说是同一个点，也还是要处理
					if(VexCircleLast!=null&& VexLast !=VexSelected)
                    {
						VexCircleLast.SetValue(Ellipse.FillProperty, DrawCal.DefaultBGColor);

						//VexCircleSelected 不变
						VexCircleLast = VexCircleSelected;

						//VexSelected 不变
						VexLast = VexSelected;
                    }
					return;
				}

				//Console.WriteLine(Graph.SPathLen[VexSelected, VexIdx]);
				//Console.WriteLine(VexSelected+"  "+VexIdx);
				
				bool hasPath=Graph.TraceShortestPath(VexSelected,curVex,ref FullPath);
				if(!hasPath)
                {
					MessageBox.Show("这两点不连通哦", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
					if (VexCircleLast != null && VexLast != VexSelected)
						VexCircleLast.SetValue(Ellipse.FillProperty, DrawCal.DefaultBGColor);

					curVexCircle.SetValue(Ellipse.FillProperty, DrawCal.VexSelectedColor);
					//VexCircleSelected.SetValue(Ellipse.FillProperty,DrawCal.VexSelectedColor);

					VexCircleLast = VexCircleSelected;
					VexCircleSelected = curVexCircle;

					VexLast = VexSelected;
					VexSelected = curVex;
					return;
                }

				for (int i = 0; i < FullPath.Length; i++)
					Console.Write(FullPath[i] + " ");
				Console.WriteLine();

				//return;

				//一般情况（不是第一次也不是重复）
				//画这一次遍历找到最短路径对应的边
				foreach (UIElement child in MainCanvas.Children)
				{
					if (child.GetType() == typeof(TextBlock))
					{
						TextBlock tb = (TextBlock)child;
						string[] str = tb.Name.Split('_');
						for (int i = 1; FullPath[i + 1] != 0; i++)
						{
							if ((int.Parse(str[1]) == FullPath[i] && int.Parse(str[2]) == FullPath[i + 1]) ||
								(int.Parse(str[2]) == FullPath[i] && int.Parse(str[1]) == FullPath[i + 1]))
							{
								//Console.WriteLine(tb.Name);
								child.SetValue(TextBlock.ForegroundProperty, DrawCal.DefaultLHeadColor);
							}

						}


					}
					else if (child.GetType() == typeof(Line))
					{
						Line li = (Line)child;
						string[] str = li.Name.Split('_');
						Console.WriteLine(li.Name);
						for (int i = 1; FullPath[i + 1] != 0; i++)
						{
							if ((int.Parse(str[1]) == FullPath[i] && int.Parse(str[2]) == FullPath[i + 1]) ||
								(int.Parse(str[2]) == FullPath[i] && int.Parse(str[1]) == FullPath[i + 1]))
							{
								child.SetValue(Line.StrokeProperty, DrawCal.StressArcColor);
							}
						}


					}

				}

				if(VexCircleLast!=null&&VexLast!=VexSelected)
					VexCircleLast.SetValue(Ellipse.FillProperty, DrawCal.DefaultBGColor);

				curVexCircle.SetValue(Ellipse.FillProperty, DrawCal.VexSelectedColor);
				//VexCircleSelected.SetValue(Ellipse.FillProperty,DrawCal.VexSelectedColor);

				VexCircleLast = VexCircleSelected;
				VexCircleSelected =curVexCircle;

				VexLast = VexSelected;
				VexSelected = curVex;
				

				

				Console.WriteLine(VexCircleSelected.Name);
			}

			

			

			
		}

	}
}
