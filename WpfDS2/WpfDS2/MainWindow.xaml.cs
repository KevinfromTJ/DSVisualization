using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace WpfDS2
{


	//双向绑定
	public class MainViewModel : INotifyPropertyChanged
	{
		
		public void Initialize()
		{

			ArcGroup =
				"1 22 \n" +
				"2 21 \n" +
				"22 23 \n" +
				"21 23 \n" +
				"22 24 \n" +
				"21 24 \n" +
				"22 25 \n" +
				"21 25 \n" +
				"23 5 \n" +
				"24 5 \n" +
				"25 5 \n" +
				"23 8 \n" +
				"24 8 \n" +
				"25 8 \n" +
				"5 17 \n" +
				"5 18 \n" +
				"17 16 \n" +
				"18 16 \n" +
				"5 11 \n" +
				"8 11 \n" +
				"11 9 \n" +
				"9 12 ";

			BottomHint = "请尽量在确认输入的装配依赖关系合理且完整后，再点击左边按钮哦";
		}
		public MainViewModel()
		{
			
			ArcGroup =
				"1 22 \n" +
				"2 21 \n" +
				"22 23 \n" +
				"21 23 \n" +
				"22 24 \n" +
				"21 24 \n" +
				"22 25 \n" +
				"21 25 \n" +
				"23 5 \n" +
				"24 5 \n" +
				"25 5 \n" +
				"23 8 \n" +
				"24 8 \n" +
				"25 8 \n" +
				"5 17 \n" +
				"5 18 \n" +
				"17 16 \n" +
				"18 16 \n" +
				"5 11 \n" +
				"8 11 \n" +
				"11 9 \n" +
				"9 12 ";

			BottomHint = "请尽量在确认输入的装配依赖关系合理且完整后，再点击左边按钮哦";
		}

		



		private string arcGroup;

		private string bottomHint;

		
		public string ArcGroup { get { return arcGroup; } set { arcGroup = value; OnPropertyChanged("ArcGroup"); } }

		
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
	public partial class MainWindow : Window
    {
		public static MainViewModel mainViewModel = new MainViewModel();
		public static int imageIndex = 0;


		


        public MainWindow()
        {
            InitializeComponent();
			this.DataContext = mainViewModel;
			DynamicTransform.Init();
			AnimeCtr();
		}


		
        
    }



	
}
