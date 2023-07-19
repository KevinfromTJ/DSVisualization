using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfDS2
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


	//装配动画中变形、选中和颜色等的静态参数定义
	public class DynamicTransform
	{
		public static int GraphAreaHeight = 500;//Canvas一列的高度
		public static int GraghWidthExtend = 50;//每个图片的边长，也是相隔的距离

		public static int DefaultWidthLeft = 300;//动态装配时车的高度(只需要规定高度，宽度已经在grid固定了)
		public static int WidthLeftOnDiaplay = 500;//动态装配时车的高度(只需要规定高度，宽度已经在grid固定了)

		//public static int BorderThicknessSelected = 3;//被选中的零件边框厚度
		public static SolidColorBrush BorderColorSelected = Brushes.Red;//当前被选中的零件边框颜色
		public static SolidColorBrush BorderTextSelected = Brushes.OrangeRed;//当前被选中的零件边框颜色
		public static SolidColorBrush BorderColorSelectNext = Brushes.Orange;//接下来被选中的零件边框颜色
		public static string CompleteCarUri = "pack://application:,,,/final/car_multiple_9.png";
		public static string LocalImageUriPrefix = "pack://application:,,,";

		public static string[] ImageIndex = new string[MainWindow.MaxValidIdx+1];

		public static void Init()
        {
			ImageIndex[1 ]= "pack://application:,,,/final/carFrontBasic_single_1.png";
			ImageIndex[2 ]= "pack://application:,,,/final/carFrontBasicPlus_single_2.png";
			ImageIndex[3 ]= "pack://application:,,,/final/carEquip_single_3.png";
			ImageIndex[4 ]= "pack://application:,,,/final/carEquip_single_4.png";
			ImageIndex[5 ]= "pack://application:,,,/final/carEquip_single_5.png";
			ImageIndex[6 ]= "pack://application:,,,/final/carEquip_single_6.png";
			ImageIndex[7 ]= "pack://application:,,,/final/carEquip_single_7.png";
			ImageIndex[8 ]= "pack://application:,,,/final/carEquip_single_8.png";
			ImageIndex[9 ]= "pack://application:,,,/final/carEquip_single_9.png";
			ImageIndex[10] = "pack://application:,,,/final/carEquip_single_10.png";
			ImageIndex[11] = "pack://application:,,,/final/carEquip_single_11.png";
			ImageIndex[12] = "pack://application:,,,/final/carEquip_single_12.png";
			ImageIndex[13] = "pack://application:,,,/final/carEquip_single_13.png";
			ImageIndex[14] = "pack://application:,,,/final/carEquip_single_14.png";
			ImageIndex[15] = "pack://application:,,,/final/carEquip_single_15.png";
			ImageIndex[16] = "pack://application:,,,/final/carEquip_single_16.png";
			ImageIndex[17] = "pack://application:,,,/final/carEquip_single_17.png";
			ImageIndex[18] = "pack://application:,,,/final/carEquip_single_18.png";
			ImageIndex[19] = "pack://application:,,,/final/carEquip_single_19.png";
			ImageIndex[20] = "pack://application:,,,/final/carEquip_single_20.png";
			ImageIndex[21] = "pack://application:,,,/final/carBasicBottom_single_21.png";
			ImageIndex[22] = "pack://application:,,,/final/carBasicTop_single_22.png";
			ImageIndex[23] = "pack://application:,,,/final/carWheel_single_23.png";
			ImageIndex[24] = "pack://application:,,,/final/carWheel_single_24.png";
			ImageIndex[25] = "pack://application:,,,/final/carWheel_single_25.png";
		}
		
	}




	//定义数据处理
	public partial class MainWindow : Window
	{
		//在切换页面时需要强制重设图片
		public void ResetImageDisplay(int choice=0)
        {
			for(int i=MinValidIdx; i<=MaxValidIdx;i++)
            {
				TextBlock tb=(TextBlock)FindName("equipName_" + i.ToString());
				Border bd= (Border)FindName("equipBorder_" + i.ToString());
				Image image = (Image)bd.Child;

				tb.Foreground = Brushes.Black;
				bd.BorderThickness = new Thickness(0, 0, 0, 0);
				bd.BorderBrush =Brushes.Transparent;
				if (choice == 1 && TechRouteGraph.VexArray[i].VexIndex == 0)
					image.Visibility = Visibility.Hidden;
				else
					image.Visibility = Visibility.Visible;
			}
        }

		//后台读已经算好的路径数据并输出静态部分
		public void GetStaticRouteAndPrint()
        {
			int orderCnt = 0;
			for (int i = 0; i < DismantleOrder.Length; i++)
				DismantleOrder[i] = 0;


			string tbTmp = "装配路径：\n";
			bool isBegin = true;
			
			while (!TechRouteGraph.DismantleStack.IsEmpty())
			{
				if (!isBegin)
				{
					tbTmp += "-->";

				}
				
				isBegin = false;

				int curIdx = TechRouteGraph.DismantleStack.Top();
				tbTmp += curIdx.ToString();
				TechRouteGraph.EquipStack.Push(curIdx);
				
				TechRouteGraph.DismantleStack.Pop();

			}
			tbTmp += "\n\n拆卸路径：\n";
			isBegin = true;
			while (!TechRouteGraph.EquipStack.IsEmpty())
			{
				if (!isBegin)
				{
					tbTmp += "-->";

				}
				orderCnt++;
				isBegin = false;

				int curIdx = TechRouteGraph.EquipStack.Top();
				DismantleOrder[orderCnt] = curIdx;

				tbTmp += curIdx.ToString();
				TechRouteGraph.EquipStack.Pop();

			}

			dp_dynamic_tbx_route.Text = tbTmp;
		}



		//根据图结点中存储的坐标信息
		public string ImageIndexMapping(int curIdx)
		{
			string imageRoute;

			switch(curIdx)
			{
				case 12:
					if (IsOnDynamicDisplay == 2)
						imageRoute = DynamicTransform.CompleteCarUri;
					else
						imageRoute = DynamicTransform.LocalImageUriPrefix + "/final/car_multiple_7.png";
					break;
				case 9:
					if(IsOnDynamicDisplay==2)
						imageRoute = DynamicTransform.LocalImageUriPrefix+ "/final/car_multiple_7.png";
					else
						imageRoute = DynamicTransform.LocalImageUriPrefix + "/final/car_multiple_6.png";
					break;
				case 11:
					if (IsOnDynamicDisplay == 2)
						imageRoute = DynamicTransform.LocalImageUriPrefix + "/final/car_multiple_6.png";
					else
						imageRoute = DynamicTransform.LocalImageUriPrefix + "/final/car_multiple_5.png";
					break;
				//
				case 16:
					if (IsOnDynamicDisplay == 2)
						imageRoute = DynamicTransform.LocalImageUriPrefix + "/final/car_multiple_5.png";
					else
						imageRoute = DynamicTransform.LocalImageUriPrefix + "/final/car_multiple_5.png";
					break;
				case 17:
					if (IsOnDynamicDisplay == 2)
						imageRoute = DynamicTransform.LocalImageUriPrefix + "/final/car_multiple_5.png";
					else
						imageRoute = DynamicTransform.LocalImageUriPrefix + "/final/car_multiple_5.png";
					break;
				case 18:
					if (IsOnDynamicDisplay == 2)
						imageRoute = DynamicTransform.LocalImageUriPrefix + "/final/car_multiple_5.png";
					else
						imageRoute = DynamicTransform.LocalImageUriPrefix + "/final/car_multiple_5.png";
					break;
				//
				case 5:
					if (IsOnDynamicDisplay == 2)
						imageRoute = DynamicTransform.LocalImageUriPrefix + "/final/car_multiple_5.png";
					else
						imageRoute = DynamicTransform.LocalImageUriPrefix + "/final/car_multiple_4.png";
					break;
				case 8:
					if (IsOnDynamicDisplay == 2)
						imageRoute = DynamicTransform.LocalImageUriPrefix + "/final/car_multiple_4.png";
					else
						imageRoute = DynamicTransform.LocalImageUriPrefix + "/final/carBasicWithWheels_multiple_1.png";
					break;
				case 23:
					if (IsOnDynamicDisplay == 2)
						imageRoute = DynamicTransform.LocalImageUriPrefix + "/final/carBasicWithWheels_multiple_1.png";
					else
						imageRoute = DynamicTransform.LocalImageUriPrefix + "/final/car_multiple_8.png";
					break;
				case 24:
					if (IsOnDynamicDisplay == 2)
						imageRoute = DynamicTransform.LocalImageUriPrefix + "/final/car_multiple_8.png";
					else
						imageRoute = DynamicTransform.LocalImageUriPrefix + "/final/carBasicFull_multiple_2.png";
					break;
				case 25:
					if (IsOnDynamicDisplay == 2)
						imageRoute = DynamicTransform.LocalImageUriPrefix + "/final/carBasicFull_multiple_2.png";
					else
						imageRoute = DynamicTransform.LocalImageUriPrefix + "/final/carBasicBottom_single_21.png";
					break;
				case 21:
					if (IsOnDynamicDisplay == 2)
						imageRoute = DynamicTransform.LocalImageUriPrefix + "/final/carBasicBottom_single_21.png";
					else
						imageRoute = DynamicTransform.LocalImageUriPrefix + "/final/carFrontBasicPlus_single_2.png";
					break;
				case 22:
					if (IsOnDynamicDisplay == 2)
						imageRoute = DynamicTransform.LocalImageUriPrefix + "/final/carBasicTop_single_22.png";
					else
						imageRoute = DynamicTransform.LocalImageUriPrefix + "/final/carFrontBasic_single_1.png";
					break;
				case 2:
					imageRoute = DynamicTransform.LocalImageUriPrefix + "/final/carFrontBasicPlus_single_2.png";
					break;
				case 1:
					imageRoute = DynamicTransform.LocalImageUriPrefix + "/final/carFrontBasic_single_1.png";
					break;
				default:
					imageRoute = "";
					break;
			};
				




			return imageRoute;

		}

		//动态设置特定显示状态
		public void SetImageOnDisplay(int curCnt,int displayStage=1)
		{
			int curIdx = DismantleOrder[curCnt];
			int lastIdx = (displayStage==1)? DismantleOrder[curCnt- 1]: DismantleOrder[curCnt + 1];
			int nextIdx = (displayStage == 1) ? DismantleOrder[curCnt + 1]: DismantleOrder[curCnt - 1];
			string imageName ;
			string imageBorder ;
			Border bd;
			TextBlock tb;

			//去掉上一个（如果有）
			if (lastIdx!=0)
            {
				imageName = "equipName_" + lastIdx.ToString();
				imageBorder = "equipBorder_" + lastIdx.ToString();
				bd = (Border)FindName(imageBorder);
				tb = (TextBlock)FindName(imageName);

				bd.BorderThickness = new Thickness(0, 0, 0, 0);
				bd.BorderBrush = Brushes.Transparent;
				tb.Foreground = Brushes.Black;
			}


			//设置当前
			imageName = "equipName_" + curIdx.ToString();
			imageBorder = "equipBorder_" + curIdx.ToString();
			bd = (Border)FindName(imageBorder);
			tb = (TextBlock)FindName(imageName);

			bd.BorderThickness = new Thickness(2, 2, 2, 2);
			bd.CornerRadius = new CornerRadius(8,8,8,8);
			bd.BorderBrush = DynamicTransform.BorderColorSelected;
			tb.Foreground = DynamicTransform.BorderTextSelected;

			dp_dynamic_tb_cur.Text = ((displayStage == 1) ? "拆卸中，当前零件：" : "安装中，当前零件：" )+ tb.Text;

			//设置过程示意图
			
			ProcedureIamge.Source =new BitmapImage(new Uri(ImageIndexMapping(curIdx))) ;




			//画下一个（如果有）

			if (nextIdx != 0)
            {
				imageName = "equipName_" + nextIdx.ToString();
				imageBorder = "equipBorder_" + nextIdx.ToString();
				bd = (Border)FindName(imageBorder);
				tb = (TextBlock)FindName(imageName);

				bd.BorderThickness = new Thickness(2, 2, 2, 2);
				bd.CornerRadius = new CornerRadius(8, 8, 8, 8);
				bd.BorderBrush = DynamicTransform.BorderColorSelectNext;

				dp_dynamic_tb_next.Text = ((displayStage == 1) ? "拆卸中，下一个零件：" : "安装中，下一个零件：" )+ tb.Text;
				
			}
			else
				dp_dynamic_tb_next.Text = "完成！";





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

		//点的布局
		public void ArrageGrpahVex()
        {
			Queue queue = new Queue();
			
			ArrayList[] arrangeMat = new ArrayList[MaxValidIdx+1];
			for (int i = 0; i < MaxValidIdx + 1; i++)
				arrangeMat[i] = new ArrayList();
			bool[] visited = new bool[MaxValidIdx + 1];

			int k = 1; 
			for(int i=1; TechRouteGraph.StartVex[i]!=0;i++)
            {
				
				if(!visited[TechRouteGraph.StartVex[i]])
                {
					queue.Enqueue(TechRouteGraph.StartVex[i]);
					visited[TechRouteGraph.StartVex[i]] = true;
					arrangeMat[k].Add(TechRouteGraph.StartVex[i]);
				}
					
				
			}
			k++;
			int cnt = queue.Count;
			while(queue.Count!=0)
            {
				int curIdx=(int)queue.Peek();
				for(int i=1; i<TechRouteGraph.VexNum+1;i++)
                {
					if(TechRouteGraph.VexMatrix[curIdx,i]==true)
                    {
						
						if(!visited[i])
                        {
							queue.Enqueue(i);
							visited[i] = true;
							arrangeMat[k].Add(i);
						}
							
						//Console.WriteLine(curIdx + "  " + i);
					}
                }
				queue.Dequeue();
				cnt--;
				if(cnt==0)
                {
					k++;
					cnt = queue.Count;
				}
				
            }

			//画图
			for (int i = 1; i <k; i++)
			{
				double divDis = DynamicTransform.GraphAreaHeight/(arrangeMat[i].Count + 1);

				for(int j=0; j<arrangeMat[i].Count;j++)
                {
					Image image = new Image()
					{
						Source = new BitmapImage(new Uri(DynamicTransform.ImageIndex[(int)arrangeMat[i][j]])),
						Width = DynamicTransform.GraghWidthExtend,
						Uid=(2 * i * DynamicTransform.GraghWidthExtend).ToString()+"_" +((j+1)*divDis).ToString(),
						//Height= DynamicTransform.GraghWidthExtend,
						//Name ="Canvas_"+ arrangeMat[i][j].ToString(),
					};
					if(MainCanvas.FindName("Canvas_" + arrangeMat[i][j].ToString())==null)
						MainCanvas.RegisterName("Canvas_" + arrangeMat[i][j].ToString(),image);

					Console.WriteLine(image.Name);
					Canvas.SetTop(image,(j + 1) * divDis);
					Canvas.SetLeft(image, 2 * i * DynamicTransform.GraghWidthExtend);
					Canvas.SetZIndex(image,0);
					MainCanvas.Children.Add(image);
                }

			}




		}

		//画图
		public void DrawTechRouteGraph()
        {
			for(int i=1; i< TechRouteGraph.VexNum + 1; i++)
            {
				if (MainCanvas.FindName("Canvas_" + i.ToString()) != null)
					MainCanvas.UnregisterName("Canvas_" + i.ToString());

			}
			MainCanvas.Children.Clear();

			ArrageGrpahVex();
			//连边
			for(int i=1;i<TechRouteGraph.VexNum+1;i++)
            {
				for (int j = 1; j < TechRouteGraph.VexNum + 1; j++)
				{
					if(TechRouteGraph.VexMatrix[i,j]==true)
                    {
						Console.WriteLine(i + "  " + j);
						Image tailImage=MainCanvas.FindName("Canvas_"+i.ToString()) as Image;
						Image headImage = MainCanvas.FindName("Canvas_" + j.ToString()) as Image;

						
						string[] strTail = tailImage.Uid.Split('_');
						string[] strHead = headImage.Uid.Split('_');

						DrawArrow(new Point(double.Parse(strTail[0])+headImage.Width, double.Parse(strTail[1])+ headImage.Width/2), new Point(double.Parse(strHead[0]), double.Parse(strHead[1]) + headImage.Width / 2));
						
                    }
				}
			}


		}
	}




	//定义按键功能
	public partial class MainWindow : Window
    {
		private bool HasCalAll = false;
		private bool HasDrawRouteGraph = false;
		private int IsOnDynamicDisplay = 0;
		private int[] DismantleOrder = new int[MaxValidIdx*MaxValidIdx];
		//private bool HasDrawnAdjList = false;
		//private bool IsDrawingSinSrcSpath = false;//在单源最短路径页面
		//private bool IsDrawingAllSpath = false;//在多源最短路径页面

		private void BtnDataInit_Click(object sender, RoutedEventArgs e)
		{
			//进入到这个页面后当前后台数据和画的图不再有效
			HasCalAll = false;
			AnimeTimer.IsEnabled = false;//不需要动画效果
			HasDrawRouteGraph = false;
			IsOnDynamicDisplay = 0;
			ArcLines.Clear();
			for (int i = 0; i < DismantleOrder.Length; i++)
				DismantleOrder[i] = 0;


			//重设布局
			CanvasBoard.Visibility = Visibility.Collapsed;
			DataInputBoard.Visibility = Visibility.Visible;
			dp_dynamic.Visibility = Visibility.Collapsed;
			dp_static.Visibility =Visibility.Visible;
			WidthLeft.Width= new System.Windows.GridLength(DynamicTransform.DefaultWidthLeft);
			tb_bigImage.Text = "默认装配次序下完整的车辆预览图（仅供参考）";
			ProcedureIamge.Source =new BitmapImage(new Uri(DynamicTransform.CompleteCarUri)) ;
			
			ResetImageDisplay();
			
		}


		private void BtnShowEquipRoute_Click(object sender, RoutedEventArgs e)
		{
			IsOnDynamicDisplay = 0;
			AnimeTimer.IsEnabled = false;//不需要动画效果
		    //检查合法性与后台计算
			if (!HasCalAll)
            {
				bool validData = JudgeCalAndStore();
				if (!validData)
					return;

				HasCalAll = true;
				//TechRouteGraph.
				//HasCalAll = true;
			}


			if(!HasDrawRouteGraph)
            {
				DrawTechRouteGraph();
				HasDrawRouteGraph = true;
			}
			

			//重设布局
			DataInputBoard.Visibility= Visibility.Collapsed;
			CanvasBoard.Visibility = Visibility.Visible;



		}

		private void BtnDynamicEquip_Click(object sender, RoutedEventArgs e)
		{
			AnimeTimer.IsEnabled = false;//暂时不需要动画效果
			//检查合法性与后台计算
			if (!HasCalAll)
			{
				bool validData = JudgeCalAndStore();
				if (!validData)
					return;

				HasCalAll = true;
				
			}



			//重设布局
			CanvasBoard.Visibility = Visibility.Collapsed;
			DataInputBoard.Visibility = Visibility.Visible;
			dp_dynamic.Visibility = Visibility.Visible;
			dp_static.Visibility = Visibility.Collapsed;
			WidthLeft.Width = new System.Windows.GridLength(DynamicTransform.WidthLeftOnDiaplay);
			tb_bigImage.Text = "车辆当前装配情况（仅供参考）";
			dp_dynamic_tbx_route.Text = "拆卸路径：\n\n装配路径：";
			dp_dynamic_tb_cur.Text = "点击右侧零件对应的图片区域";
			dp_dynamic_tb_next.Text = "确认需要拆卸维修的零件";
			ResetImageDisplay(1);

		}


		//点击图片选择
		private void SelectElement_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (IsOnDynamicDisplay!=0||!HasCalAll)
				return;

			UIElement targetElement = Mouse.DirectlyOver as UIElement;
			Console.WriteLine(targetElement.GetType().ToString());//Debug


			
			
            if (targetElement != null && targetElement.GetType() == typeof(Image))
            {


				Image curImage = (Image)targetElement;
				if (curImage.Name == "ProcedureIamge")
					return;

				Border curBorder = (Border)curImage.Parent;
				int curIdx = int.Parse(curBorder.Name.Split('_')[1]);

				TextBlock tb = (TextBlock)FindName("equipName_" + curIdx.ToString());

				MessageBoxResult selectRes= MessageBox.Show("要拆卸维修的零件是:\n"+curIdx.ToString()+"号  "+tb.Text, "确认您的选择", MessageBoxButton.YesNo, MessageBoxImage.Question);
				//需要确定是否选择这一个
				if (selectRes == MessageBoxResult.No)
				{
					return ;
				}

				//去掉上一次画的和上一次计算信息
				ResetImageDisplay(1);
				//计算
				//EquipRepaired = curIdx;
				TechRouteGraph.GetTechOrder(curIdx);
				//输出静态的路径
				GetStaticRouteAndPrint();

				//动态调用
				TimerCnt = 1;
				IsOnDynamicDisplay = 1;
				AnimeTimer.IsEnabled = true; ;

			}
            
        }



		private void BtnMenuReset_Click(object sender, RoutedEventArgs e)
		{
			HasCalAll = false;
			AnimeTimer.IsEnabled = false;//不需要动画效果
			IsOnDynamicDisplay = 0;
			HasDrawRouteGraph = false;
			ArcLines.Clear();
			for (int i = 0; i < DismantleOrder.Length; i++)
				DismantleOrder[i] = 0;

			//重设布局
			CanvasBoard.Visibility = Visibility.Collapsed;
			DataInputBoard.Visibility = Visibility.Visible;
			dp_dynamic.Visibility = Visibility.Collapsed;
			dp_static.Visibility = Visibility.Visible;
			WidthLeft.Width = new System.Windows.GridLength(DynamicTransform.DefaultWidthLeft);
			tb_bigImage.Text = "默认装配次序下完整的车辆预览图（参考）";
			ProcedureIamge.Source = new BitmapImage(new Uri(DynamicTransform.CompleteCarUri));
			
			ResetImageDisplay();

			mainViewModel.Initialize();



		}


		private void BtnMenuExit_Click(object sender, RoutedEventArgs e)
		{
			Environment.Exit(0);
		}


		private void BtnMenuInstrution_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("1.默认或点击\"装配路径定义\" 在输入框中以图的边的形式输入零件的装配依赖\n" +
				"2.点击\"装配路径可视化\" 显示零件装配顺序的AOE图\n" +
				"3.点击\"零件拆卸与装配\" 选择零件的图片即可动态显示拆卸至该零件维修再逆向装配的过程\n", "使用说明", MessageBoxButton.OK);
		}

		private void BtnMenuProgramInfo_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("自走车拆卸组装教学系统\n版本：v1.0\n平台：.NET Framework 4.7.2（WPF）\n作者：陈君涛\n指导教师：柳先辉", "程序信息", MessageBoxButton.OK);
		}











		
    }


}
