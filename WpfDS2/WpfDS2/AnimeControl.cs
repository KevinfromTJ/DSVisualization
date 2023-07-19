using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace WpfDS2
{
	


	//动态与动画的控制器定义与基本启动
	public partial class MainWindow : Window
	{
		
		private static int TimerCnt = 1;
		public static DispatcherTimer AnimeTimer = new DispatcherTimer();

		private void AnimeTimer_Tick(object sender, EventArgs e)
		{
			if (IsOnDynamicDisplay==0)
			{
				return;
			}
			
			if(IsOnDynamicDisplay==1)
				SetImageOnDisplay(TimerCnt++,IsOnDynamicDisplay);
			else
				SetImageOnDisplay(TimerCnt--, IsOnDynamicDisplay);

			//自然结束演示
			if (DismantleOrder[TimerCnt]==0)
            {
				//if(TimerCnt)
    //            {
				//	IsOnDynamicDisplay

				//}
				if(IsOnDynamicDisplay==1)
                {
					MessageBox.Show("零件维修完成！准备进入下一阶段", "即将逆向安装", MessageBoxButton.OK,MessageBoxImage.Information);
					TimerCnt--;
					IsOnDynamicDisplay = 2;
				}
				else if(IsOnDynamicDisplay==2)
                {
					MessageBox.Show("逆向安装成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
					IsOnDynamicDisplay = 0;
				}

				
				return;
            }
		}

		public void AnimeCtr()
		{
			//定义

			AnimeTimer.Interval = TimeSpan.FromSeconds(2);
			AnimeTimer.Tick += AnimeTimer_Tick;

			//控制
			if (true)
				AnimeTimer.Start();


		}
	}
}
