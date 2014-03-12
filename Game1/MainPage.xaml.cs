using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Game1.Resources;

using Windows.UI.Core;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Game1.Model;

namespace Game1
{
    public partial class MainPage : PhoneApplicationPage
    {
        public int targetLoc = -1;
        int speed = 2000;
        Timer timer;
        int toBtn = -1;
        int fromBtn;
        bool lockMusic = false;

        List<int> moveLine = new List<int>();
        // 构造函数
        public MainPage()
        {
            InitializeComponent();

            initGameData();
            // 用于本地化 ApplicationBar 的示例代码
            //BuildLocalizedApplicationBar();
        }


        private void initGameData(){
            //Config.WinList.Add(0);
            //Config.WinList.Add(1);
            //Config.WinList.Add(2);
            //Config.WinList.Add(3);
            //Config.WinList.Add(4);
            //Config.WinList.Add(5);
            //Config.WinList.Add(6);
            //Config.WinList.Add(7);
            //Config.WinList.Add(8);
            //Config.WinList.Add(9);
            playBackGroundMusic();
        }

        public void initFlag()
        {
            for (int i = 0; i < Config.LevelNum; i++)
            {
                if (Config.WinList.Contains(i))
                {
                    continue;
                }
                Button b = getBtnByNum(i);
                ImageBrush ib = new ImageBrush();
                ib.ImageSource = new BitmapImage(new Uri("/Assets/Resources/flag1.png", UriKind.RelativeOrAbsolute));
                b.Background = ib;
            }

            Config.WinList.ForEach(
                delegate(int ele)
                {
                    Button b = getBtnByNum(ele);
                    ImageBrush i = new ImageBrush();
                    i.ImageSource = new BitmapImage(new Uri("/Assets/Resources/flag.png", UriKind.RelativeOrAbsolute));
                    b.Background = i;
                });
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            DoDension.stopDension();
            playBackGroundMusic();
            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }
            string parameter = string.Empty;
            if (NavigationContext.QueryString.TryGetValue("loc", out parameter))
            {
                fromBtn = int.Parse(parameter);
            }
            else
            {
                fromBtn = 0;
            }
            setHeroLoc(fromBtn);
            initFlag();
        }
        public void playBackGroundMusic()
        {
            DoDension.playBackGroundMusic("mainpageBGM.wav");
        }
        public void playSound(string sourceName , bool isLoop)
        {
            if (lockMusic)
            {
                return;
            }

            DoDension.playSound(sourceName, isLoop);
        }

        // 用于生成本地化 ApplicationBar 的示例代码
        //private void BuildLocalizedApplicationBar()
        //{
        //    // 将页面的 ApplicationBar 设置为 ApplicationBar 的新实例。
        //    ApplicationBar = new ApplicationBar();

        //    // 创建新按钮并将文本值设置为 AppResources 中的本地化字符串。
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // 使用 AppResources 中的本地化字符串创建新菜单项。
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}

        private void btnClick(object sender, RoutedEventArgs e)
        {
            toBtn = int.Parse(((Button)sender).Name.ElementAt(2).ToString());
            targetLoc = toBtn;
            //if (sender.Equals(g_0))
            //{
            //}
            //else if (sender.Equals(g_1) && Config.WinList.Contains(0))
            //{
            //}
            //else if (sender.Equals(g_2) && Config.WinList.Contains(1))
            //{
            //}
            //else if (sender.Equals(g_3) && Config.WinList.Contains(2))
            //{
            //}
            //else if (sender.Equals(g_4) && Config.WinList.Contains(2))
            //{
            //}
            //else if (sender.Equals(g_5) && Config.WinList.Contains(4))
            //{
            //}
            //else if (sender.Equals(g_6) && Config.WinList.Contains(5))
            //{
            //}
            //else if (sender.Equals(g_7) && Config.WinList.Contains(6))
            //{
            //}
            //else if (sender.Equals(g_8) && Config.WinList.Contains(4))
            //{
            //}
            //else if (sender.Equals(g_9) && Config.WinList.Contains(5))
            //{
            //}
            //else
            //{
            //    toBtn = -1;
            //    targetLoc = toBtn;
            //    return;
            //}
            //Debug.WriteLine(targetLoc);
            
            //Debug.WriteLine(targetLoc);

            if(toBtn == 7 && fromBtn == 8 ||
                toBtn == 8 && fromBtn == 7 ||
                toBtn == 4 && fromBtn == 3 ||
                toBtn == 3 && fromBtn == 4)
            {
                toBtn = -1;
                targetLoc = -1;
                return;
            }
            else if (Math.Abs(toBtn - fromBtn) == 1)
            {
                canvas.IsHitTestVisible = false;
                moveLine = findMoveLine(fromBtn, toBtn);
                MoveHero();
            }
            else if (toBtn == 2 && fromBtn == 4 ||
                  toBtn == 4 && fromBtn == 2 ||
                  toBtn == 4 && fromBtn == 8 ||
                  toBtn == 8 && fromBtn == 4)
            {
                canvas.IsHitTestVisible = false;
                moveLine = findMoveLine(fromBtn, toBtn);
                MoveHero();
            }
            else
            {
                toBtn = -1;
                targetLoc = -1;
                return;
            }
        }

        double[] m = new double[3];
        
        public void MoveHero()
        {

            //int from = moveLine.ElementAt(i);
            //int to = moveLine.ElementAt(i + 1);
            int from = fromBtn;
            int to = toBtn;
            Button fromb = getBtnByNum(from);
            Button tob = getBtnByNum(to);
            double l1 = Canvas.GetLeft(fromb);
            double t1 = Canvas.GetTop(fromb);
            double l2 = Canvas.GetLeft(tob);
            double t2 = Canvas.GetTop(tob);
            Debug.WriteLine("frombtn:" + from);
            Debug.WriteLine("tobtn:" + to);

            m[0] = l2 - l1;
            m[1] = t2 - t1;
            m[2] = 0;
            if (m[0] < 0)
            {
                heroScale.ScaleX = -1;
            }
            else
            {
                heroScale.ScaleX = 1;
            }
            playSound("stepBGM.wav", true);
            timer = new Timer(move, null, 0, Convert.ToInt64(speed / Math.Abs(m[1])));
            
        }

        public List<int> findMoveLine(int from, int to)
        {
            List<int> line = new List<int>();
            if(from == to){
                return line;
            }
            
            if(to == 3)
            {
                line.AddRange(findMoveLine(from, 2));
                line.Add(3);
            } 
            else if(from == 3)
            {
                line.Add(3);
                line.AddRange(findMoveLine(2, to));
            }
            else if(to == 8 && from != 9)
            {
                line.AddRange(findMoveLine(from, 4));
                line.Add(8);
            }
            else if (from == 8 && to != 9)
            {
                line.Add(8);
                line.AddRange(findMoveLine(4, to));
            }
            else if(Math.Abs(from - to) == 1)
            {
                line.Add(from);
                line.Add(to);
            }
            else if (from >= 5 && from <= 7 && to < 5 || to > 7)
            {
                line.Add(from);
                line.AddRange(findMoveLine(--from, to));
            }
            else if (to >= 5 && to <= 7 && from < 5 || from > 7)
            {
                line.AddRange(findMoveLine(from, to - 1));
                line.Add(to);
            }
            else if (from == 4 && to == 2 || from == 2 && to == 4)
            {
                line.Add(from);
                line.Add(to);
            }
            else if (to == 9)
            {
                line.AddRange(findMoveLine( from, 8));
                line.Add(9);
            } 
            else if(from == 9){
                line.Add(9);
                line.AddRange(findMoveLine(8, to));
            }
            else if (from <= 2 && to <= 2)
            {
                line.Add(from);
                line.Add(1);
                line.Add(to);
            }
            return line;
        }

        /// <summary>
        /// 显示移动效果
        /// </summary>
        /// <param name="source">长度为三的数组：第一个数为水平移动距离，
        /// 第二个数为竖直移动距离，第三个数为已经移动距离</param>
        private void move(object source)
        {
            Dispatcher.BeginInvoke(() =>
            {
                double l = Canvas.GetLeft(hero);
                double t = Canvas.GetTop(hero);
                double x = Canvas.GetLeft(canvas);
                double y = Canvas.GetTop(canvas);

                if (m[2] == Math.Abs(m[1]))
                {
                    timer.Dispose();

                    fromBtn = targetLoc;
                    toBtn = -1;

                    if ( DoDension.sei != null)
                    {
                        DoDension.sei.Stop();
                        DoDension.sei = null;
                    }
                    
                    canvas.IsHitTestVisible = true;
                    startGame();
                    return;
                }
                m[2]++;

                if(m[1] < 0){
                    Canvas.SetTop(hero, --t);
                    
                    if (y + 1 <= 0)
                    {
                        Canvas.SetTop(canvas, ++y);
                    }
                }
                else if(m[1] != 0)
                {
                    Canvas.SetTop(hero, ++t);
                    if (y - 1 >= -626)
                    {
                        Canvas.SetTop(canvas, --y);
                    }
                }

                double deltaL = Math.Abs(m[0] / m[1]);
                if (m[0] < 0)
                {
                    Canvas.SetLeft(hero, l - deltaL);
                    if (x + deltaL < 0)
                    {
                        Canvas.SetLeft(canvas, x + deltaL);
                    }
                }
                else if (m[0] != 0)
                {
                    Canvas.SetLeft(hero, l + deltaL);
                    
                    if (x - deltaL >= -400)
                    {
                        Canvas.SetLeft(canvas, x - deltaL);
                    }
                }
                
                //Debug.WriteLine("data[0]: " + data[0]);
                //Debug.WriteLine("data[2]: " + data[2]);
            });
        }

        public Button getBtnByNum(int num)
        {
            switch (num)
            {
                case 0:
                    return g_0;
                case 1:
                    return g_1;
                case 4:
                    return g_4;
                case 7:
                    return g_7;
                case 8:
                    return g_8;
                case 2:
                    return g_2;
                case 5:
                    return g_5;
                case 3:
                    return g_3;
                case 6:
                    return g_6;
                case 9:
                    return g_9;
                default:
                    return null;
            }
        }

        public void startGame()
        {
            if (targetLoc < 0)
            {
                return;
            }
            if(Config.WinList.Contains(targetLoc))
            {
                Config.CurLevel = targetLoc;
                return;
            }
            Config.FaceLevel = targetLoc;

            DoDension.stopDension();
            NavigationService.Navigate(new Uri("/GameType0.xaml", UriKind.Relative));
        }


        private void backKeyPress(object sender,System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;    // Tell system we've handled it
            if (timer != null)
            {
                timer.Dispose();
            }
            DoXml.SaveDataToXml();
            DoDension.stopDension();
            NavigationService.Navigate(new Uri("/StartPage.xaml?backFromGame=true", UriKind.Relative));

            //MessageBoxResult result = MessageBox.Show("是否返回主菜单?", "提示", MessageBoxButton.OKCancel);
            //if (result == MessageBoxResult.OK)
            //{
            //    stopMusic();
            //    NavigationService.Navigate(new Uri("/StartPage.xaml?backFromGame=true", UriKind.Relative));
            //}
            //else
            //{
            //    if (mainBGM != null)
            //    {
            //        mainBGM.Play();
            //    }
                
            //    if (sei != null)
            //    {
            //        sei.Play();
            //    }
            //}
        }

        public void setHeroLoc(int i)
        {
            Button b = getBtnByNum(i);
            double hx = Canvas.GetLeft(b);
            double hy = Canvas.GetTop(b);
            Canvas.SetLeft(hero, hx);
            Canvas.SetTop(hero, hy - 90);

            if (1200 - hx < 400)
            {
                Canvas.SetLeft(canvas, -400);
            }
            else if (hx < 400)
            {
                Canvas.SetLeft(canvas, 0);
            }
            else
            {
                Canvas.SetLeft(canvas, 400 - hx);
            }

            if (1200 - hy < 240)
            {
                Canvas.SetTop(canvas, -720);
            }
            else if (hy < 240)
            {
                Canvas.SetTop(canvas, 0);
            }
            else
            {
                Canvas.SetTop(canvas, 240 - hy);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DoDension.stopDension();
            if (timer != null)
            {
                timer.Dispose();
            }
            DoXml.SaveDataToXml();
            NavigationService.Navigate(new Uri("/StartPage.xaml?backFromGame=true", UriKind.Relative));
        }
    }
}