using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.UI.Core;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Game1.Model;

namespace Game1
{
    public partial class GameType0 : PhoneApplicationPage
    {
        public static int XueCaoMaxWidth = 180;
        public static int CardWidth = 63;
        public static int CardHeight = 87;
        public static int CardShow = 40; // board区卡牌露出的长度
        Timer t; // 渐变Timer
        GameData g;

        bool lockMusic = false;
        bool isEnded = false;
        public GameType0()
        {
            InitializeComponent();
        }

        public void playBackGroundMusic(string sourceName)
        {
            if (lockMusic)
            {
                return;
            }
            if (sourceName != "gamePageBGM.wav")
            {
                lockMusic = true;
            }
            DoDension.playBackGroundMusic(sourceName);
        }

        public void playSound(string sourceName, bool isLoop)
        {
            if (lockMusic)
            {
                return;
            }
            DoDension.playSound(sourceName, false);
        }

        public void refreshData()
        {
            heroBloodText.Text = g.hero.curBlood.ToString();
            minusBlood(g.hero, 0);

            monsterBloodText.Text = g.monster.curBlood.ToString();

            cardNumText.Text = g.leftCards.Count.ToString();
            roundText.Text = g.curRound.ToString();
        }

        public void startGame()
        {
            g = new GameData();
            
            refreshHandCards();
            refreshBoardCards();
            refreshData();
            playBackGroundMusic("gamePageBGM.wav");
            startPlay();
        }

        public void startPlay()
        {
            doDrawingCard();
        }

        public void doDrawingCard()
        {
            gameCanvas.IsHitTestVisible = false;
            new Timer(
                new TimerCallback(
                    delegate(object ob){
                        Debug.WriteLine("1");
                        Dispatcher.BeginInvoke(() =>
                        {
                            if (isEnded)
                            {
                                return;
                            }
                            if (g.isCardNotEnough())
                            {
                                showResult(-1);
                                return;
                            }

                            new Timer(
                                new TimerCallback(
                                    delegate(object o1)
                                    {
                                        Debug.WriteLine("2");
                                        Dispatcher.BeginInvoke(() =>
                                        {
                                            if (isEnded)
                                            {
                                                return;
                                            }
                                            playSound("drawCardBGM.wav", false);

                                            g.addCardsToHand();
                                            refreshHandCards();

                                            g.addCardsToBoard();
                                            refreshBoardCards();

                                            refreshData();
                                            gameCanvas.IsHitTestVisible = true;
                                            if (g.isMonsterStep())
                                            {
                                                new Timer(
                                                new TimerCallback(
                                                    delegate(object o)
                                                    {
                                                        Debug.WriteLine("3");
                                                        Dispatcher.BeginInvoke(() =>
                                                        {
                                                            g.DoMonsterSkill();
                                                            if (isEnded)
                                                            {
                                                                return;
                                                            }
                                                            // 展示mention
                                                            gameCanvas.IsHitTestVisible = false;
                                                            gameCanvas.Opacity = 0.05;
                                                            mentionCanvas.Visibility = Visibility.Visible;
                                                            playSound("monsterSkillBGM.wav", false);

                                                            new Timer(
                                                                new TimerCallback(
                                                                    delegate(object o21)
                                                                    {
                                                                        Debug.WriteLine("4");
                                                                        Dispatcher.BeginInvoke(() =>
                                                                        {
                                                                            if (isEnded)
                                                                            {
                                                                                return;
                                                                            }
                                                                            // 去除mention
                                                                            mentionCanvas.Visibility = Visibility.Collapsed;
                                                                            gameCanvas.Opacity = 1;

                                                                            new Timer(
                                                                                new TimerCallback(
                                                                                    delegate(object o211)
                                                                                    {
                                                                                        Debug.WriteLine("5");
                                                                                        Dispatcher.BeginInvoke(() =>
                                                                                        {
                                                                                            if (isEnded)
                                                                                            {
                                                                                                return;
                                                                                            }
                                                                                            
                                                                                            playSound("drawCardBGM.wav", false);
                                                                                            refreshBoardCards();
                                                                                            gameCanvas.IsHitTestVisible = true;
                                                                                        });
                                                                                    }),
                                                                                null, 500, System.Threading.Timeout.Infinite);
                                                                          });
                                                                }),
                                                                null, 4000, System.Threading.Timeout.Infinite);
                                                        });
                                                    }),
                                                null, 500, System.Threading.Timeout.Infinite);
                                            }
                                        });
                                    }),
                                null, 300, System.Threading.Timeout.Infinite);

                        });
                    }), null, 300, System.Threading.Timeout.Infinite);
        }




        /// <summary>
        /// 显示结果(未写具体函数)
        /// </summary>
        /// <param name="type">0:英雄死亡  1：蛇妖死亡 -1：牌不够</param>
        public void showResult(int type)
        {
            if (gameCanvas.Visibility == Visibility.Collapsed)
            {
                return;
            }
            if (isEnded)
            {
                return;
            }
            isEnded = true;
            switch (type)
            {

                case -1:
                case 0:
                    Debug.WriteLine("游戏失败");
                    gameCanvas.IsHitTestVisible = false;
                    new Timer(
                        new TimerCallback(
                            delegate(object o)
                            {
                                Debug.WriteLine("6");
                                Dispatcher.BeginInvoke(() =>
                                {
                                    playBackGroundMusic("lostBGM.wav");
                                    gameCanvas.Visibility = Visibility.Collapsed;
                                    lockMusic = true;
                                    gameCanvas.IsHitTestVisible = true;
                                    lostCanvas.Visibility = Visibility.Visible;
                                });
                            }), null, 500, System.Threading.Timeout.Infinite);
                    break;
                case 1:
                    Debug.WriteLine("游戏胜利");
                    gameCanvas.IsHitTestVisible = false;
                    new Timer(
                        new TimerCallback(
                            delegate(object o)
                            {
                                Debug.WriteLine("7");
                                Dispatcher.BeginInvoke(() =>
                                {
                                    DoDension.stopDension();
                                    playSound("winBGM.wav", false);
                                    gameCanvas.Visibility = Visibility.Collapsed;
                                    lockMusic = true;
                                    winCanvas.Visibility = Visibility.Visible;
                                    Config.WinList.Add(Config.FaceLevel);
                                    
                                    Config.CurLevel = Config.FaceLevel;
                                    new Timer(
                                        new TimerCallback(
                                            delegate(object o1)
                                            {
                                                Debug.WriteLine("8");
                                                Dispatcher.BeginInvoke(() =>
                                                {
                                                    Config.HeroStartBlood = g.hero.curBlood;
                                                    gameCanvas.IsHitTestVisible = true;
                                                    winCanvas.Visibility = Visibility.Collapsed;
                                                    DoDension.stopDension();
                                                    NavigationService.Navigate(new Uri("/MainPage.xaml?loc=" + Config.FaceLevel, UriKind.RelativeOrAbsolute));
                                                });
                                            }), null, 3000, System.Threading.Timeout.Infinite);
                                });
                            }), null, 1000, System.Threading.Timeout.Infinite);

                    break;
            }
        }

        public void refreshLeftNum()
        {
            cardNumText.Text = g.leftCards.Count.ToString();
        }

        /// <summary>
        /// 刷新hand区卡牌
        /// </summary>
        public void refreshHandCards()
        {
            
            for (int i = 0; i < GameData.MaxHandCardsNum; i++)
            {
                string btnName = "h_" + i;
                Image btn = (Image)getObjectByName(btnName); // hand位置
                btn.Source = null;
            }

            int indexOfBtn = -1;
            for (int i = 0; i < g.handCards.Count; i++)
            {
                string item;
                Image btn;
                do{
                    indexOfBtn++;
                    if (indexOfBtn > 4)
                    {
                        return;
                    }
                    item = "h_" + indexOfBtn;
                    btn = (Image)getObjectByName(item); // hand位置
                    btn.Source = null;
                } while (g.handCardsTag.Contains(indexOfBtn));
                
                int num = g.handCards.ElementAt(i); // 牌号

                btn.Source = new BitmapImage(new Uri("Assets/Resources/cards/" + num + ".png", UriKind.RelativeOrAbsolute));
            }
        }

        /// <summary>
        /// 刷新board区卡牌
        /// </summary>
        public void refreshBoardCards()
        {
            for (int i = 0; i < GameData.MaxBoardCardsNum; i++)
            {
                string btnName = "c_" + i;
                Image btn = (Image)getObjectByName(btnName); // hand位置
                btn.Source = null;
            }

            int indexOfBtn = -1;
            for (int i = 0; i < g.boardCards.Count; i++)
            {
                string item;
                Image btn;
                do
                {
                    indexOfBtn++;
                    if (indexOfBtn > 9)
                    {
                        return;
                    }
                    item = "c_" + indexOfBtn;
                    btn = (Image)getObjectByName(item); // board位置
                    btn.Source = null;
                } while (g.boardCardsTag.Contains(indexOfBtn));

                int num = g.boardCards.ElementAt(i); // 牌号
                btn.Source = new BitmapImage(new Uri("Assets/Resources/cards/" + num + ".png", UriKind.RelativeOrAbsolute));
            }
        }

        public Object getObjectByName(string name)
        {
            switch (name)
            {
                case "c_0":
                    return c_0;
                case "c_1":
                    return c_1;
                case "c_2":
                    return c_2;
                case "c_3":
                    return c_3;
                case "c_4":
                    return c_4;
                case "c_5":
                    return c_5;
                case "c_6":
                    return c_6;
                case "c_7":
                    return c_7;
                case "c_8":
                    return c_8;
                case "c_9":
                    return c_9;
                case "h_0":
                    return h_0;
                case "h_1":
                    return h_1;
                case "h_2":
                    return h_2;
                case "h_3":
                    return h_3;
                case "h_4":
                    return h_4;
                default:
                    return null;
            }
        }

        private void doBoardCardHold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image btn = (Image) sender;
            int loc = int.Parse(btn.Name.ElementAt(2).ToString());
            if (g.boardCardsTag.Contains(loc))
            {
                // 当前位置为空
                return;
            }
            g.drawCardFromBoard(loc);
            playSound("drawCard.wav", false);
            refreshBoardCards();
            refreshHandCards();

            if (g.isHandCardsFull())
            {
                gameCanvas.IsHitTestVisible = false;
                g.calculateHandCards();
                new Timer(doHandCardsFull, null, 500, System.Threading.Timeout.Infinite);
            }
        }
        private void doHandCardsFull(object source)
        {
            Debug.WriteLine("9");
            Dispatcher.BeginInvoke(() =>
            {
                if (isEnded)
                {
                    return;
                }
                showCardsType();
            });
        }

        public void showCardsType()
        {
            playSound("PKBGM.wav", false);
            img_0.Source = null;
            img_1.Source = null;
            img_2.Source = null;
            img_3.Source = null;
            img_4.Source = null;

            typeNameText.Text = g.typeName;

            int num = g.getReplaceNum();// 替换的数量
            string imgSourceUri = "Assets/Resources/cards/img.png";
            switch (num)
            {
                case 1:
                    img_2.Source = new BitmapImage(
                        new Uri(imgSourceUri.Replace("img", g.showList.ElementAt(0).ToString()),
                        UriKind.RelativeOrAbsolute));
                    break;
                case 2:
                    img_1.Source = new BitmapImage(
                        new Uri(imgSourceUri.Replace("img", g.showList.ElementAt(0).ToString()),
                        UriKind.RelativeOrAbsolute));
                    img_2.Source = new BitmapImage(
                        new Uri(imgSourceUri.Replace("img", g.showList.ElementAt(1).ToString()),
                        UriKind.RelativeOrAbsolute));
                    break;
                case 3:
                    img_1.Source = new BitmapImage(
                        new Uri(imgSourceUri.Replace("img", g.showList.ElementAt(0).ToString()),
                        UriKind.RelativeOrAbsolute));
                    img_2.Source = new BitmapImage(
                        new Uri(imgSourceUri.Replace("img", g.showList.ElementAt(1).ToString()),
                        UriKind.RelativeOrAbsolute));
                    img_3.Source = new BitmapImage(
                        new Uri(imgSourceUri.Replace("img", g.showList.ElementAt(2).ToString()),
                        UriKind.RelativeOrAbsolute));
                    break;
                case 4:
                    img_0.Source = new BitmapImage(
                        new Uri(imgSourceUri.Replace("img", g.showList.ElementAt(0).ToString()),
                        UriKind.RelativeOrAbsolute));
                    img_1.Source = new BitmapImage(
                        new Uri(imgSourceUri.Replace("img", g.showList.ElementAt(1).ToString()),
                        UriKind.RelativeOrAbsolute));
                    img_2.Source = new BitmapImage(
                        new Uri(imgSourceUri.Replace("img", g.showList.ElementAt(2).ToString()),
                        UriKind.RelativeOrAbsolute));
                    img_3.Source = new BitmapImage(
                        new Uri(imgSourceUri.Replace("img", g.showList.ElementAt(3).ToString()),
                        UriKind.RelativeOrAbsolute));
                    break;
                case 5:
                    img_0.Source = new BitmapImage(
                        new Uri(imgSourceUri.Replace("img", g.showList.ElementAt(0).ToString()),
                        UriKind.RelativeOrAbsolute));
                    img_1.Source = new BitmapImage(
                        new Uri(imgSourceUri.Replace("img", g.showList.ElementAt(1).ToString()),
                        UriKind.RelativeOrAbsolute));
                    img_2.Source = new BitmapImage(
                        new Uri(imgSourceUri.Replace("img", g.showList.ElementAt(2).ToString()),
                        UriKind.RelativeOrAbsolute));
                    img_3.Source = new BitmapImage(
                        new Uri(imgSourceUri.Replace("img", g.showList.ElementAt(3).ToString()),
                        UriKind.RelativeOrAbsolute));
                    img_4.Source = new BitmapImage(
                        new Uri(imgSourceUri.Replace("img", g.showList.ElementAt(4).ToString()),
                        UriKind.RelativeOrAbsolute));
                    break;
            }
            typeCanvas.Visibility = Visibility.Visible;
            gameCanvas.Opacity = 0.05;

            Timer t = new Timer(removeTypeCanvas, null, 2000, System.Threading.Timeout.Infinite);
        }

        /// <summary>
        /// 移除TypeCanvas，在showCardsType之后，最后调用hurtMonster
        /// </summary>
        /// <param name="o"></param>
        public void removeTypeCanvas(object o)
        {
            Debug.WriteLine("10");
            Dispatcher.BeginInvoke(() =>
            {
                if (isEnded)
                {
                    return;
                }
                typeCanvas.Visibility = Visibility.Collapsed;
                gameCanvas.Opacity = 1;

                new Timer(hurtMonster, null, 100, System.Threading.Timeout.Infinite);
            });
        }

        /// <summary>
        /// 伤害monster动画
        /// </summary>
        /// <param name="o"></param>
        public void hurtMonster(object o){
            Debug.WriteLine("11");
            Dispatcher.BeginInvoke(() =>
            {
                if (isEnded)
                {
                    return;
                }
                hurtCanvas.Visibility = Visibility.Visible;

                g.hurtMonster();
                int i = 0;
                new Timer(hurtMonsterShow, i, 0, System.Threading.Timeout.Infinite);

                //for (int i = 0; i < g.showList.Count; i++)
                //{
                //    // 设立伤害显示timer
                //    new Timer(
                //        new TimerCallback( 
                //            delegate(object o1){
                //                Debug.WriteLine("12");
                //                int i1 = (int)o1;
                //                int num = g.showList.ElementAt(i1);
                //                int hurtNum = g.hurtNums.ElementAt(i1);
                //                string imgSourceUri = "Assets/Resources/cards/" + num + ".png";
                //                Dispatcher.BeginInvoke(() =>
                //                {
                //                    if (isEnded)
                //                    {
                //                        return;
                //                    }
                //                    hurtMonsterCardImg.Source = new BitmapImage(new Uri(imgSourceUri, UriKind.RelativeOrAbsolute));
                //                    hurtMonsterNumText.Text = "-" + hurtNum;
                //                    minusBlood(g.monster, hurtNum);
                //                    playSound("hurtBGM.wav");
                //                });
                //            }), 
                //            i, 1000 * i, System.Threading.Timeout.Infinite);
                //}
                //// 伤害完后，刷新手牌
                //new Timer(new TimerCallback(
                //    delegate(object o5)
                //    {
                //        Debug.WriteLine("13");
                //        Dispatcher.BeginInvoke(() =>
                //        {
                //            if (isEnded)
                //            {
                //                return;
                //            }
                //            t.Dispose();
                //            hurtMonsterCardImg.Source = null;
                //            hurtMonsterNumText.Text = "";
                //            hurtCanvas.Visibility = Visibility.Collapsed;

                //            refreshHandCards();
                //            doHurtHero();
                //        });
                //    }), null, 1000 * g.hurtNums.Count, System.Threading.Timeout.Infinite);
                //Debug.WriteLine("13 time: " + 1000 * g.hurtNums.Count);

                if (g.hurtNums.Count != 0)
                {
                    //渐变效果
                    double n = 100 * g.showList.Count - 1;
                    t = new Timer(
                            new TimerCallback(
                                delegate(object o1)
                                {
                                    Debug.WriteLine("14");
                                    Dispatcher.BeginInvoke(() =>
                                    {
                                        if (isEnded)
                                        {
                                            t.Dispose();
                                            return;
                                        }
                                        if (n % 100 > 0)
                                        {
                                            n -= 10;
                                        }
                                        else if (n < 0)
                                        {
                                            t.Dispose();
                                            n = 100;
                                        }
                                        else
                                        {
                                            n -= 0;
                                        }
                                        //Debug.WriteLine(n);
                                        hurtMonsterNumText.Opacity = n % 100 / 100;
                                        hurtMonsterCardImg.Opacity = n % 100 / 100;
                                    });
                                }),
                                null, 0, 100);
                }
            });
        }

        public void hurtMonsterShow(object o)
        {
            Debug.WriteLine("12");

            int i = (int)o;
            if (i >= g.showList.Count)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    if (isEnded)
                    {

                        return;
                    }
                    t.Dispose();
                    hurtMonsterCardImg.Source = null;
                    hurtMonsterNumText.Text = "";
                    hurtCanvas.Visibility = Visibility.Collapsed;

                    refreshHandCards();
                    doHurtHero();
                });
            }
            else
            {
                int num = g.showList.ElementAt(i);
                int hurtNum = g.hurtNums.ElementAt(i);
                string imgSourceUri = "Assets/Resources/cards/" + num + ".png";
                Dispatcher.BeginInvoke(() =>
                {
                    if (isEnded)
                    {
                        return;
                    }
                    i++;
                    new Timer(hurtMonsterShow, i, 1000, System.Threading.Timeout.Infinite);
                    hurtMonsterCardImg.Source = new BitmapImage(new Uri(imgSourceUri, UriKind.RelativeOrAbsolute));
                    hurtMonsterNumText.Text = "-" + hurtNum;
                    minusBlood(g.monster, hurtNum);
                    playSound("hurtBGM.wav", false);
                });
            }
        }

        public void doHurtHero()
        {
            hurtCanvas.Visibility = Visibility.Visible;
            g.hurtHero();
            new Timer(hurtHeroShow, 0, 0, System.Threading.Timeout.Infinite);

            if (g.hurtHeroNumList.Count != 0)
            {
                double n = 100 * g.hurtHeroNumList.Count - 1;
                t = new Timer(
                    new TimerCallback(
                        delegate(object o1)
                        {
                            Dispatcher.BeginInvoke(() =>
                            {
                                if (isEnded)
                                {
                                    t.Dispose();
                                    return;
                                }
                                if (n % 100 > 0)
                                {
                                    n -= 10;
                                }
                                else if (n < 0)
                                {
                                    t.Dispose();
                                    n = 100;
                                }
                                else
                                {
                                    n -= 0;
                                }
                                hurtHeroNumText.Opacity = n % 100 / 100;
                                hurtHeroCardImg.Opacity = n % 100 / 100;
                            });
                        }),
                        null, 0, 100);
            }
        }

        public void hurtHeroShow(object o)
        {
            Debug.WriteLine("15");
            int i = (int)o;
            if (i >= g.hurtHeroNumList.Count)
            {
                Debug.WriteLine("15");
                Dispatcher.BeginInvoke(() =>
                {
                    if (isEnded)
                    {
                        return;
                    }
                    t.Dispose();
                    hurtHeroCardImg.Source = null;
                    hurtHeroNumText.Text = "";
                    hurtCanvas.Visibility = Visibility.Collapsed;
                    refreshHandCards();
                    doDrawingCard();
                });
                return;
            }

            Dispatcher.BeginInvoke(() =>
            {
                Debug.WriteLine("13");
                if (isEnded)
                {
                    return;
                }
                
                playSound("hurtBGM.wav", false);
                int num = g.hurtHeroList.ElementAt(i);
                int hurtNum = g.hurtHeroNumList.ElementAt(i);
                string imgSourceUri = "Assets/Resources/cards/" + num + ".png";
                hurtHeroCardImg.Source = new BitmapImage(new Uri(imgSourceUri, UriKind.RelativeOrAbsolute));
                hurtHeroNumText.Text = "-" + hurtNum;
                // 处理血条
                minusBlood(g.hero, hurtNum);
                i++;
                new Timer(hurtHeroShow, i, 1000, System.Threading.Timeout.Infinite);
            });
        }

        /// <summary>
        /// 处理血条显示
        /// </summary>
        /// <param name="p"></param>
        /// <param name="num"></param>
        public void minusBlood(Person p, int num)
        {
            int curShowBlood = 0; // 当前显示血量
            double percentOfFullBlood = 0; // 当前百分比
            if (p.Equals(g.monster))
            {
                curShowBlood = int.Parse(monsterBloodText.Text);
                
                if (curShowBlood - num > 0)
                {
                    curShowBlood -= num;
                    monsterBloodText.Text = curShowBlood.ToString();
                    percentOfFullBlood = curShowBlood / (double)p.fullBlood;
                    monsterBloodCao.Width = XueCaoMaxWidth * percentOfFullBlood;
                }
                else
                {
                    monsterBloodText.Text = "0";
                    showResult(1);
                    
                    monsterBloodCao.Width = 0;
                    hurtCanvas.Visibility = Visibility.Collapsed;
                    return;
                }
            }
            else if (p.Equals(g.hero))
            {
                curShowBlood = int.Parse(heroBloodText.Text);

                if (curShowBlood - num > 0)
                {
                    curShowBlood -= num;
                    heroBloodText.Text = curShowBlood.ToString();
                    percentOfFullBlood = curShowBlood / (double)p.fullBlood;
                    heroBloodCao.Width = XueCaoMaxWidth * percentOfFullBlood;
                    Canvas.SetLeft(heroBloodCao, XueCaoMaxWidth * (1 - percentOfFullBlood));
                }
                else
                {
                    heroBloodText.Text = "0";
                    showResult(0);
                    heroBloodCao.Width = 0;
                    return;
                }
            }
        }

        private void handCardHoldEventHandle(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image btn = (Image)sender;
            int loc = int.Parse(btn.Name.ElementAt(2).ToString());
            if (g.handCards.Contains(loc))
            {
                // 当前位置为空
                return;
            }
            g.moveCardFromHandToBoard(loc);
            playSound("drawCard.wav", false);
            refreshBoardCards();
            refreshHandCards();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            lockMusic = false;
            isEnded = false;

            lostCanvas.Visibility = Visibility.Collapsed;
            gameCanvas.Visibility = Visibility.Visible;
            mentionCanvas.Visibility = Visibility.Collapsed;

            this.InitializeComponent();
            this.startGame();
        }

        private void backToMapBtnClicked(object sender, RoutedEventArgs e)
        {
            DoDension.stopDension();
            NavigationService.Navigate(new Uri("/MainPage.xaml?loc=" + Config.CurLevel, UriKind.Relative));
        }

        private void backKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DoDension.pauseDension();
            
            e.Cancel = true;    // Tell system we've handled it

            MessageBoxResult result = MessageBox.Show("是否返回主地图?", "", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                DoDension.stopDension();
                NavigationService.Navigate(new Uri("/MainPage.xaml?loc=" + Config.CurLevel, UriKind.Relative));
            }
            else
            {
                DoDension.resumeDension();
            }
        }

        private void h_2_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image btn = (Image)sender;
            int loc = int.Parse(btn.Name.ElementAt(2).ToString());
            if (g.handCards.Contains(loc))
            {
                // 当前位置为空
                return;
            }
            g.moveCardFromHandToBoard(loc);
            playSound("drawCard.wav", false);
            refreshBoardCards();
            refreshHandCards();
        }

        private void c_0_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image btn = (Image)sender;
            int loc = int.Parse(btn.Name.ElementAt(2).ToString());
            if (g.boardCardsTag.Contains(loc))
            {
                // 当前位置为空
                return;
            }
            g.drawCardFromBoard(loc);
            playSound("drawCard.wav", false);
            refreshBoardCards();
            refreshHandCards();

            if (g.isHandCardsFull())
            {
                gameCanvas.IsHitTestVisible = false;
                g.calculateHandCards();
                new Timer(doHandCardsFull, null, 500, System.Threading.Timeout.Infinite);
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.startGame();
        }
    }
}