using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using Game1.Model;

namespace Game1
{
    public partial class StartPage : PhoneApplicationPage
    {
        int helpNum = 0;
        public StartPage()
        {
            InitializeComponent();
        }

        public void initBtn()
        {
            if (Config.IsXmlExist)
            {
                resumeBtn.IsEnabled = true;
            }
            else
            {
                resumeBtn.IsEnabled = false;
            }
        }
        
        public void playBackGroundMusic()
        {
            DoDension.playBackGroundMusic("mainpageBGM.wav");
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string parameter = string.Empty;
            DoXml.LoadDataFromXml();
            initBtn();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Config.IsXmlExist)
            {
                DoDension.pauseDension();
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("检查到有存档，进入新的游戏会覆盖原来的记录，确定进入新游戏?", "提示", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    Config c = new Config();
                    Config.setConfig(c);
                    DoXml.SaveDataToXml();
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                }
                else
                {
                    DoDension.mainBGM.Resume();
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DoDension.stopDension();
            NavigationService.Navigate(new Uri("/MainPage.xaml?loc=" + Config.CurLevel, UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            playBackGroundMusic();
            versionText.Text = Config.Version;
        }

        private void backKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;    // Tell system we've handled it

            MessageBoxResult result = MessageBox.Show("是否要退出游戏?", "提示", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                DoXml.SaveDataToXml();
                Application.Current.Terminate();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (helpNum == 3)
            {
                return;
            }
            helpNum++;
            showHelpType();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (helpNum == 0)
            {
                return;
            }
            helpNum--;
            showHelpType();
        }

        private void showHelpType()
        {
            switch (helpNum)
            {
                case 0:
                    helpTypeImg0.Visibility = Visibility.Visible;
                    helpTypeImg1.Visibility = Visibility.Collapsed;
                    helpTypeImg2.Visibility = Visibility.Collapsed;
                    helpTypeImg3.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    helpTypeImg0.Visibility = Visibility.Collapsed;
                    helpTypeImg1.Visibility = Visibility.Visible;
                    helpTypeImg2.Visibility = Visibility.Collapsed;
                    helpTypeImg3.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    helpTypeImg0.Visibility = Visibility.Collapsed;
                    helpTypeImg1.Visibility = Visibility.Collapsed;
                    helpTypeImg2.Visibility = Visibility.Visible;
                    helpTypeImg3.Visibility = Visibility.Collapsed;
                    break;
                case 3:
                    helpTypeImg0.Visibility = Visibility.Collapsed;
                    helpTypeImg1.Visibility = Visibility.Collapsed;
                    helpTypeImg2.Visibility = Visibility.Collapsed;
                    helpTypeImg3.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            helpNum = 0;
            helpCanvas.Visibility = Visibility.Collapsed;
            startCanvas.Visibility = Visibility.Visible;
        }

        private void helpBtnClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            startCanvas.Visibility = Visibility.Collapsed;
            helpNum = 0;
            helpCanvas.Visibility = Visibility.Visible;
        }

        private void settingClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            startCanvas.Visibility = Visibility.Collapsed;
            helpNum = 0;
            settingCanvas.Visibility = Visibility.Visible;
            showSettingChecked();
        }

        public void showSettingChecked()
        {
            if (Config.IsMusicOpen)
            {
                musicOpen.IsChecked = true;
                musicClose.IsChecked = false;
            }
            else
            {
                musicOpen.IsChecked = false;
                musicClose.IsChecked = true;
            }

            if (Config.IsSoundOpen)
            {
                soundOpen.IsChecked = true;
                soundClose.IsChecked = false;
            }
            else
            {
                soundOpen.IsChecked = false;
                soundClose.IsChecked = true;
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            settingCanvas.Visibility = Visibility.Collapsed;
            startCanvas.Visibility = Visibility.Visible;
            DoXml.SaveDataToXml();
        }

        private void musicBtnClicked(object sender, RoutedEventArgs e)
        {
            if (musicOpen.IsChecked == true)
            {
                Config.IsMusicOpen = true;
                playBackGroundMusic();
            }
            else
            {
                Config.IsMusicOpen = false;
                DoDension.stopDension();
            }
        }

        private void soundBtnClicked(object sender, RoutedEventArgs e)
        {
            if (soundOpen.IsChecked == true)
            {
                Config.IsSoundOpen = true;
            }
            else
            {
                Config.IsSoundOpen = false;
            }
        }
    }
}