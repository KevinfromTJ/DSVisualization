using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfDS2
{
	//数据绑定+判断数据合法性+后台计算
	public partial class MainWindow : Window
	{
		public static int MinValidIdx = 1,MaxValidIdx=25;//只有1-25的零件
		//public static ;


		//public static int InputVexNum;
		//public static int EquipRepaired=2;//要维修零件的索引号
		public static int InputLineCnt;
		public static StringCollection ArcLines = new StringCollection();

		


		//只检查不合法输入和单纯数据越界，成环之类的不判断，经过前端初步检验后调用后端
		public bool JudgeDataValidity()
		{
			
			//待传入的弧信息
			int[,] arcVexPair = new int[InputLineCnt, 2];
			


			for (int i = 0; i < InputLineCnt; i++)
			{
				if (ArcLines[i].IndexOf('\n') != -1)
				{
					ArcLines[i] = ArcLines[i].Substring(0, ArcLines[i].IndexOf('\n'));

				}
				string[] arcInfo = ArcLines[i].Split(' ');

				
				bool arcIsValid = arcInfo.Length>=2&&arcInfo[1]!=""
					&& int.TryParse(arcInfo[0], out arcVexPair[i, 0]) //尾
					&& int.TryParse(arcInfo[1], out arcVexPair[i, 1]) //头
					&& arcVexPair[i, 0] >= MinValidIdx && arcVexPair[i, 0] <= MaxValidIdx
					&& arcVexPair[i, 1] >= MinValidIdx && arcVexPair[i, 1] <= MaxValidIdx;
				//Console.WriteLine(arcInfo[0] + "**"+ arcInfo[1] + "**");

				if (!arcIsValid)
					return false;

			}


			/* 待完成*/
			TechRouteGraph.GetFrontData(MaxValidIdx, InputLineCnt, arcVexPair);






			return true;
		}

		//一次算完邻接链表+两种生成树数据+最短路径并存入
		public bool JudgeCalAndStore()
		{


			//读数据
			//bool IdxIsValid = EquipRepaired>=MinValidIdx&&EquipRepaired<=MaxValidIdx;//零件是对的
			//bool IdxIsValid =true;
			InputLineCnt = ArcGroupBox.LineCount;

			for (int line = InputLineCnt - 1; line >= 0; line--)
			{
				if (ArcGroupBox.GetLineText(line) == "" || ArcGroupBox.GetLineText(line) == "\r\n" || ArcGroupBox.GetLineText(line) == " ")
				{
					InputLineCnt--;
				}
				//Console.WriteLine(ArcGroupBox.GetLineText(line) + "***");
			}
			//Console.WriteLine(InputLineCnt);

			for (int line = 0; line < InputLineCnt; line++)
			{
				ArcLines.Add(ArcGroupBox.GetLineText(line));
			}

			//输入不对就弹窗,否则送入数据到后台【暂时没做完判断逻辑】
			if (JudgeDataValidity() == false)
			{
				MessageBox.Show("输入不合法——零件编号有误，请对照规则检查", "输入错误提示", MessageBoxButton.OK, MessageBoxImage.Error);
				ArcLines.Clear();//清空错误数据
				return false;
			}


			//后台计算每个部分的结果
			
			if(!TechRouteGraph.IsValidOrder())
            {
				MessageBox.Show("输入不合法——存在循环依赖，请对照规则检查", "输入错误提示", MessageBoxButton.OK, MessageBoxImage.Error);
				ArcLines.Clear();//清空错误数据
				return false;
			}

			return true;

		}

		

	}
}
