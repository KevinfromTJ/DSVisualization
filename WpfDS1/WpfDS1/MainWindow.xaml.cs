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
using System.ComponentModel;

namespace WpfDS1
{
	

	//双向绑定
	public class MainViewModel : INotifyPropertyChanged
	{
		//private string VexN1 = "8";
		//private string ArcN1 = "10";
		//private string ArcG1 = "1 2 1\n" +
		//		"2 3 2\n" +
		//		"3 4 1\n" +
		//		"4 5 2\n" +
		//		"5 6 1\n" +
		//		"6 7 2\n" +
		//		"7 8 1\n" +
		//		"8 1 2\n" +
		//		"3 5 1\n" +
		//		"6 7 2";
		public void Initialize()
		{
			VexNum = "6";
			ArcNum = "10";
			ArcGroup =
				"1 3 1\n" +
				"2 1 6\n" +
				"1 4 5\n" +
				"2 3 5\n" +
				"3 4 5\n" +
				"2 5 3\n" +
				"3 5 6\n" +
				"3 6 4\n" +
				"4 6 2\n" +
				"6 5 6";

			BottomHint = "请尽量在确认输入合理后，再点击左边按钮哦";
		}
		public MainViewModel()
		{
            VexNum = "6";
            ArcNum = "10";
            ArcGroup =
                "1 3 1\n" +
                "2 1 6\n" +
                "1 4 5\n" +
                "2 3 5\n" +
                "3 4 5\n" +
                "2 5 3\n" +
                "3 5 6\n" +
                "3 6 4\n" +
                "4 6 2\n" +
                "6 5 6";

            BottomHint = "请尽量在确认输入合理后，再点击左边按钮哦";
        }

		private string vexNum;

		private string arcNum;


		private string arcGroup;

		private string bottomHint;


		//private string vexNum { get; set; }
		public string VexNum { get {  return vexNum;  } set { vexNum = value; OnPropertyChanged("VexNum"); } }

		//private string arcNum { get; set; }
		public string ArcNum { get { return arcNum; } set { arcNum = value; OnPropertyChanged("ArcNum"); } }

		//private string arcGroup { get; set; }
		public string ArcGroup { get { return arcGroup; } set { arcGroup = value; OnPropertyChanged("ArcGroup"); } }

		//默认
		//private string bottomHint { get; set; }
		public string BottomHint { get { return bottomHint; } set { bottomHint = value; OnPropertyChanged("BottomHint"); } }



		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string properName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(properName));
			}
		}
	}
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	/// 



	public partial class MainWindow : Window
	{


		public static MainViewModel mainViewModel = new MainViewModel();

		public MainWindow()
		{			
			//创建窗体
			InitializeComponent();		
			this.DataContext = mainViewModel;			
			AnimeCtr();
			
		}

		






	}

	

	

}
