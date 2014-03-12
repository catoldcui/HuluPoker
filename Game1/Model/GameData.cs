using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

namespace Game1.Model
{
    class GameData
    {
        public static int CardNum = 54;
        public static int MaxBoardCardsNum = 10;
        public static int MaxHandCardsNum = 5;
        public static int HandCardsAtLastNum = 2;

        public int curRound = 0;//当前回合数 蛇精关卡用
        public List<int> leftCards = new List<int>(); //随机牌 存放卡片0-53
        public List<int> handCards = new List<int>(); //手中牌
        public List<int> handCardsTag = new List<int>(); // 标志手牌对应位置是否为空

        public List<int> boardCards = new List<int>(); //展示牌区
        public List<int> boardCardsTag = new List<int>(); // 标志选牌区对应位置是否为空

        public List<int> hurtCards = new List<int>(); // 计算出的伤害牌（带顺序）
        public List<int> hurtNums = new List<int>(); // 伤害值（带顺序）
        public List<int> tmpOfJokers = new List<int>(); 

        public List<int> showList = new List<int>(); //对应hurtcards和jokers
        public List<int> hurtHeroList = new List<int>(); // 伤害英雄牌
        public List<int> hurtHeroNumList = new List<int>(); // 伤害hero血量
        public string typeName;
        /* 0：一个最大  1：;一对儿 2：两对儿 3:五连 4:三个一样 5:三代二 6:四个一样 7：五个一样
         */

        public Person hero = new Person(Config.HeroFullBlood);

        public Person monster = new Person(Config.MonstersBlood[Config.FaceLevel]);

        public int result = -1; // -1:没结果  0:胜利   1:失败

        public int hurtType;
        public GameData()
        {
            shuffleCards();
        }

        public void initData()
        {
            hero.curBlood = Config.HeroStartBlood;

            handCards.Clear();
            boardCards.Clear();
            leftCards.Clear();
            handCardsTag.Clear();
            boardCardsTag.Clear();
            hurtCards.Clear();
            hurtNums.Clear();
            tmpOfJokers.Clear();
            showList.Clear();
            hurtHeroList.Clear();

            for (int i = 0; i < MaxHandCardsNum; i++)
            {
                handCardsTag.Add(i);
            }
            for (int i = 0; i < MaxBoardCardsNum; i++)
            {
                boardCardsTag.Add(i);
            }
        }
        /// <summary>
        /// 洗牌
        /// </summary>
        public void shuffleCards()
        {
            initData();

            for (int i = 0; i < CardNum; i++)
            {
                leftCards.Add(i);
            }
            // 每个项和随机的一个项交换位置，实现伪随机
            for (int i = 0; i < CardNum; i++)
            {
                int i1 = leftCards.ElementAt(i);
                int j = new Random((int)DateTime.Now.Ticks * i).Next(0, CardNum);
                //Debug.WriteLine("j:" + j);
                int j1 = leftCards.ElementAt(j);

                leftCards.RemoveAt(i);
                leftCards.Insert(i, j1);
                leftCards.RemoveAt(j);
                leftCards.Insert(j, i1);
            }

            //for (int i = 0; i < CardNum; i++)
            //{
            //    if (!leftCards.Contains(i))
            //    {
            //        Debug.WriteLine("wrong");
            //    }
            //}
        }

        /// <summary>
        /// 从leftCard接牌
        /// </summary>
        /// <param name="drawNum">接牌的数目</param>
        /// <returns></returns>
        public List<int> drawCard(int drawNum)
        {
            if (drawNum > leftCards.Count)
            {
                doCardOver();
                return null;
            }
            List<int> drawList = new List<int>();
            for (int i = 0; i < drawNum; i++)
            {
                int ele = leftCards.ElementAt(0);
                leftCards.RemoveAt(0);
                drawList.Add(ele);
            }
            return drawList;
        }

        /// <summary>
        /// 做一些牌不够的事...
        /// </summary>
        public void doCardOver()
        {
            Debug.WriteLine("card over");
        }

        public bool isCardNotEnough()
        {
            return (leftCards.Count < handCardsTag.Count + boardCardsTag.Count);
        }

        /// <summary>
        /// 发牌到board区，包括了数目检测。
        /// </summary>
        public void addCardsToBoard()
        {
            curRound++;

            int length = boardCards.Count;
            if (length == MaxBoardCardsNum)
            {
                return;
            }
            List<int> drawList = drawCard(MaxBoardCardsNum - length);

            if (boardCardsTag.Count != drawList.Count)
            {
                Debug.WriteLine("addCardsToBoard错误，tag与接牌数目不同");
            }

            // 按顺序插入指定位置
            for (int i = 0; i < drawList.Count; i++)
            {
                int loc = boardCardsTag.ElementAt(0);
                boardCardsTag.RemoveAt(0);

                boardCards.Insert(loc, drawList.ElementAt(i));
            }
        }

        /// <summary>
        /// 发牌到hand区，包括了数目检测。
        /// </summary>
        public void addCardsToHand()
        {
            int length = handCards.Count;
            // 手牌小于2时，接牌
            if (length >= HandCardsAtLastNum)
            {
                return;
            }
            List<int> drawList = drawCard(HandCardsAtLastNum - length);

            // 按顺序插入指定位置
            handCardsTag.Sort();
            for (int i = 0; i < drawList.Count; i++)
            {
                int loc = handCardsTag.ElementAt(0);
                handCardsTag.RemoveAt(0);

                handCards.Insert(loc, drawList.ElementAt(i));
            }
        }

        /// <summary>
        /// 判断board区对应位置是否为空
        /// </summary>
        /// <param name="loc">board位置</param>
        /// <returns></returns>
        public bool isBoardLocEmpty(int loc)
        {
            return !boardCardsTag.Contains(loc);
        }

        /// <summary>
        /// 判断hand对应位置是否为空
        /// </summary>
        /// <param name="loc">hand位置</param>
        /// <returns></returns>
        public bool isHandLocEmpty(int loc)
        {
            return !handCardsTag.Contains(loc);
        }

        /// <summary>
        /// 从hand区向board接牌，非自动
        /// </summary>
        /// <param name="indexInHand">由controler传入</param>
        public void moveCardFromHandToBoard(int indexInHand)
        {
            if (boardCards.Count >= MaxBoardCardsNum ||
                handCards.Count <= HandCardsAtLastNum)
            {
                return;
            }
            int realIndex = getRealIndexInBoardOrHand(indexInHand, handCardsTag);
            int cardNum = handCards.ElementAt(realIndex);
            handCards.RemoveAt(realIndex);

            // 把hand位置加入空集范围
            handCardsTag.Add(indexInHand);
            handCardsTag.Sort();

            // 插入board的第一个空的位置
            int loc = boardCardsTag.ElementAt(0);
            boardCardsTag.RemoveAt(0);
            boardCards.Insert(loc, cardNum);
        }

        /// <summary>
        /// 从board区向hand接牌，非自动
        /// </summary>
        /// <param name="indexInBoard">所接牌，由controler传入</param>
        public void drawCardFromBoard(int indexInBoard)
        {

            if (boardCards.Count <= 0 ||
                handCards.Count >= MaxHandCardsNum)
            {
                return;
            }
            int realIndex = getRealIndexInBoardOrHand(indexInBoard, boardCardsTag);
            int cardNum = boardCards.ElementAt(realIndex);
            boardCards.RemoveAt(realIndex);

            // 把该位置加入空集范围
            boardCardsTag.Add(indexInBoard);
            boardCardsTag.Sort();

            // 插入手牌的第一个空的位置
            int loc = handCardsTag.ElementAt(0);
            handCardsTag.RemoveAt(0);
            handCards.Insert(loc, cardNum);
        }

        public int getRealIndexInBoardOrHand(int index, List<int> list)
        {
            int realIndex = index;
            for (int i = 0; i <= index; i++)
            {
                if (list.Contains(i))
                {
                    realIndex--;
                }
            }
            return realIndex;
        }

        /// <summary>
        /// 判断时候手牌够数
        /// </summary>
        /// <returns></returns>
        public bool isHandCardsFull()
        {
            return (handCards.Count == MaxHandCardsNum);
        }


        public int getLeftNum()
        {
            return leftCards.Count;
        }


        /*****************************************************
         *下面是判断以及计算卡牌伤害的函数
         *****************************************************/
        public void calculateHandCards()
        {
            tmpOfJokers.Clear();
            hurtCards.Clear();
            hurtNums.Clear();
            showList.Clear();

            hurtType = -1;

            List<int> numOfCards = new List<int>();
            List<int> sortCards = new List<int>();
            // 算出card面值 比如24是7， 52、53是14是王
            for (int i = 0; i < MaxHandCardsNum; i++)
            {
                int num = handCards.ElementAt(i) / 4 + 1;
                numOfCards.Add(num);
                sortCards.Add(num);
            }
            sortCards.Sort();

            // 王的数目
            int numOfJokers = sortCards.Count(
                delegate(int num)
                {
                    return num == 14;
                });
            // 不重复牌的数量
            int distinctNum = sortCards.Distinct().ToList().Count;

            if (is4Same(sortCards))
            {
                Debug.WriteLine("四张");
                // 四张
                hurtType = 6;
                typeName = "四轰";
                if (numOfJokers == 1)
                {
                    Debug.WriteLine("一猫五张");
                    // 五张
                    hurtType = 7;
                    tmpOfJokers.Add(hurtCards.ElementAt(0));
                    typeName = "吊炸天";
                }
            }
            else if (is3Same(sortCards))
            {

                if (numOfJokers == 0)
                {
                    Debug.WriteLine("三张");
                    // 三张
                    hurtType = 4;
                    typeName = "炸弹";
                    if (is2Same(sortCards))
                    {
                        Debug.WriteLine("三代二");
                        // 三代二
                        hurtType = 5;
                        typeName = "炸弹连对儿"; ;
                    }
                }
                else if (numOfJokers == 1)
                {
                    Debug.WriteLine("一猫四张");
                    // 四张
                    hurtType = 6;
                    tmpOfJokers.Add(hurtCards.ElementAt(0));
                    typeName = "四轰";
                }
                else if (numOfJokers == 2)
                {
                    Debug.WriteLine("二猫五张");
                    // 五张
                    hurtType = 7;
                    tmpOfJokers.Add(hurtCards.ElementAt(0));
                    tmpOfJokers.Add(hurtCards.ElementAt(0));
                    typeName = "吊炸天";
                }
            }
            else if (is2Same(sortCards))
            {
                Debug.WriteLine("两张");
                // 两张，可能是两对
                hurtType = 2;
                if (hurtCards.Count == 4)
                {
                    typeName = "对对碰";
                }
                else
                {
                    typeName = "对儿";
                }
                if (numOfJokers == 1)
                {
                    Debug.WriteLine("一猫三张");
                    // 三张
                    hurtType = 5;

                    tmpOfJokers.Add(hurtCards.Max());

                    if (hurtCards.Count == 4)
                    {
                        typeName = "炸弹连对儿";
                    }
                    else
                    {
                        typeName = "炸弹";
                    }
                }
                else if (numOfJokers == 2)
                {
                    Debug.WriteLine("二猫四张");
                    // 四张
                    hurtType = 6;

                    tmpOfJokers.Add(hurtCards.ElementAt(0));
                    tmpOfJokers.Add(hurtCards.ElementAt(0));
                    typeName = "四轰";
                }
            }
            else if (isLine(sortCards))
            {
                Debug.WriteLine("五连");
                // 五连
                hurtType = 3;
                typeName = "拖拉机";
            }
            else
            {
                if (numOfJokers == 0)
                {
                    Debug.WriteLine("单张");
                    // 单张
                    hurtType = 0;

                    hurtCards.Add(sortCards.Max());
                    typeName = "单张";
                }
                else if (numOfJokers == 1)
                {
                    Debug.WriteLine("一猫两张");
                    // 一对
                    hurtType = 1;

                    hurtCards.Add(sortCards.ElementAt(3));
                    tmpOfJokers.Add(sortCards.ElementAt(3));
                    typeName = "对儿";
                }
                else if (numOfJokers == 2)
                {
                    Debug.WriteLine("二猫三张");
                    // 三张
                    hurtType = 4;

                    hurtCards.Add(sortCards.ElementAt(2));
                    tmpOfJokers.Add(sortCards.ElementAt(2));
                    tmpOfJokers.Add(sortCards.ElementAt(2));
                    typeName = "炸弹";
                }
            }

            removeCardsFromHand();
            sortHandCards();
            calculateHurtNum();
        }

        public void sortHandCards()
        {
            handCardsTag.Clear();
            for (int i = handCards.Count; i < MaxHandCardsNum; i++)
            {
                handCardsTag.Add(i);
            }
        }


        /// <summary>
        /// 算出对怪物伤害
        /// </summary>
        public void hurtMonster()
        {
            hurtNums.ForEach(
                delegate(int ele)
                {
                    monster.hurt(ele);
                });
            Debug.WriteLine("Monster:" + monster.curBlood);
        }

        /// <summary>
        /// 是否对自身有伤害
        /// </summary>
        /// <returns></returns>
        public bool isHandCardsAbove2()
        {
            return (handCards.Count > 2);
        }

        /// <summary>
        /// 先用isHandCardsAbove2检测，再算出对英雄伤害
        /// </summary>
        public void hurtHero()
        {
            hurtHeroList.Clear();
            hurtHeroNumList.Clear();
            if (handCards.Count <= 2)
            {
                return;
            }

            while (handCards.Count > 2)
            {
                int hurtNum = (handCards.Last() / 4 + 1) / 2 + handCards.Count;
                hero.hurt(hurtNum);

                hurtHeroNumList.Add(hurtNum);
                hurtHeroList.Add(handCards.Last());
                handCards.RemoveAt(handCards.Count - 1);
            }
            Debug.WriteLine("Hero:" + hero.curBlood);
            handCardsTag.Clear();
            handCardsTag.Add(2);
            handCardsTag.Add(3);
            handCardsTag.Add(4);
        }

        /// <summary>
        /// 获得结果
        /// </summary>
        /// <returns>-1：没结果  0：hero die  1：monster die</returns>
        public int getResult()
        {
            if (monster.isDie())
            {
                result = 1;
            }
            else if (hero.isDie())
            {
                result = 0;
            }
            else
            {
                result = -1;
            }

            return result;
        }

        /// <summary>
        /// 从手牌移除伤害牌
        /// </summary>
        public void removeCardsFromHand()
        {
            // 移除大小王
            List<int> locList = new List<int>();
            if (handCards.Contains(52))
            {
                locList.Add(handCards.IndexOf(52));
            }
            if (handCards.Contains(53))
            {
                locList.Add(handCards.IndexOf(53));
            }

            // 移除hurtCards中牌
            for (int i = 0; i < hurtCards.Count; i++)
            {
                for (int j = 0; j < handCards.Count; j++)
                {
                    int ele = handCards.ElementAt(j) / 4 + 1;
                    if (hurtCards.ElementAt(i) == ele)
                    {
                        if (locList.Contains(j))
                        {
                            continue;
                        }
                        locList.Add(j);
                        break;
                    }
                }
            }
            locList.Sort();
            for (int i = locList.Count - 1; i >= 0; i--)
            {
                int index = locList.ElementAt(i);

                handCardsTag.Add(index);
                showList.Add(handCards.ElementAt(index));
                handCards.RemoveAt(index);
            }
            handCardsTag.Sort();

            showList.Sort(
                delegate(int a, int b)
                {
                    int a1 = a / 4 + 1;
                    int b1 = b / 4 + 1;
                    return (a1.CompareTo(b1));
                });
            showList.ForEach(
                delegate(int ele)
                {
                    Debug.WriteLine("ele:" + ele + "("+ ( ele / 4 + 1 ) +")");
                });
        }

        /// <summary>
        /// 按等级计算攻击伤害
        /// </summary>
        public void calculateHurtNum()
        {
            switch (hurtType)
            {
                case 0:
                    setAddHurtsNum(2);
                    break;
                case 1:
                case 2:
                    setAddHurtsNum(4);
                    break;
                case 3:
                case 4:
                case 5:
                    setAddHurtsNum(6);
                    break;
                case 6:
                    setAddHurtsNum(8);
                    break;
                case 7:
                    setAddHurtsNum(10);
                    break;
            }
        }

        /// <summary>
        /// 设置hurtNums的数值
        /// </summary>
        /// <param name="added">卡片伤害权重</param>
        public void setAddHurtsNum(int added)
        {
            for (int i = 0; i < hurtCards.Count; i++)
            {
                int hurt = hurtCards.ElementAt(i) / 2 + added;
                hurtNums.Add(hurt);
            }
            for (int i = 0; i < tmpOfJokers.Count; i++)
            {
                int hurt = tmpOfJokers.ElementAt(i) / 2 + added;
                hurtNums.Add(hurt);
            }
        }
        public bool is4Same(List<int> sortCards)
        {

            if (sortCards.ElementAt(0) == sortCards.ElementAt(1) &&
                sortCards.ElementAt(1) == sortCards.ElementAt(2) &&
                sortCards.ElementAt(2) == sortCards.ElementAt(3))
            {
                hurtCards.Add(sortCards.ElementAt(0));
                hurtCards.Add(sortCards.ElementAt(1));
                hurtCards.Add(sortCards.ElementAt(2));
                hurtCards.Add(sortCards.ElementAt(3));
                return true;
            }
            if (sortCards.ElementAt(4) == sortCards.ElementAt(1) &&
                sortCards.ElementAt(1) == sortCards.ElementAt(2) &&
                sortCards.ElementAt(2) == sortCards.ElementAt(3))
            {
                hurtCards.Add(sortCards.ElementAt(4));
                hurtCards.Add(sortCards.ElementAt(1));
                hurtCards.Add(sortCards.ElementAt(2));
                hurtCards.Add(sortCards.ElementAt(3));
                return true;
            }
            return false;
        }

        public bool is3Same(List<int> sortCards)
        {
            if (sortCards.ElementAt(0) == sortCards.ElementAt(1) &&
                sortCards.ElementAt(1) == sortCards.ElementAt(2) &&
                sortCards.ElementAt(2) != sortCards.ElementAt(3))
            {
                hurtCards.Add(sortCards.ElementAt(0));
                hurtCards.Add(sortCards.ElementAt(1));
                hurtCards.Add(sortCards.ElementAt(2));
                return true;
            }
            if (sortCards.ElementAt(4) != sortCards.ElementAt(3) &&
                sortCards.ElementAt(1) == sortCards.ElementAt(2) &&
                sortCards.ElementAt(2) == sortCards.ElementAt(3))
            {
                hurtCards.Add(sortCards.ElementAt(1));
                hurtCards.Add(sortCards.ElementAt(2));
                hurtCards.Add(sortCards.ElementAt(3));
                return true;
            }
            if (sortCards.ElementAt(4) == sortCards.ElementAt(3) &&
                sortCards.ElementAt(1) != sortCards.ElementAt(2) &&
                sortCards.ElementAt(2) == sortCards.ElementAt(3))
            {
                hurtCards.Add(sortCards.ElementAt(4));
                hurtCards.Add(sortCards.ElementAt(2));
                hurtCards.Add(sortCards.ElementAt(3));
                return true;
            }
            return false;
        }

        public bool isLine(List<int> sortCards)
        {
            int numOfJokers = sortCards.Count(
                delegate(int num)
                {
                    return num == 14;
                });

            hurtCards.Add(sortCards.ElementAt(0));
            for (int i = 0; i < MaxHandCardsNum - 1; i++)
            {
                // 王跳出
                if (sortCards.ElementAt(i + 1) == 14)
                {
                    break;
                }
                // 如果连起来需要替补的数目
                int delta = sortCards.ElementAt(i + 1) - sortCards.ElementAt(i) - 1;
                if (delta > numOfJokers)
                {
                    // 补充牌不足
                    tmpOfJokers.Clear();
                    hurtCards.Clear();
                    return false;
                }

                numOfJokers -= delta;
                hurtCards.Add(sortCards.ElementAt(i + 1));
                // 算出替补的牌
                for (int j = 0; j < delta; j++)
                {
                    tmpOfJokers.Add(sortCards.ElementAt(0) + j);
                }
            }

            // joker替补全在中间或者全是数字
            if (tmpOfJokers.Count == numOfJokers)
            {
                return true;
            }

            // 剩余一张王
            if (numOfJokers - tmpOfJokers.Count == 1)
            {
                sortCards.Remove(14);
                int max = sortCards.Max();
                sortCards.Add(14);
                if (max == 13)
                {
                    tmpOfJokers.Add(9);
                }
                else
                {
                    tmpOfJokers.Add(max + 1);
                }
                return true;
            }

            // 剩余2张王
            if (numOfJokers - tmpOfJokers.Count == 2)
            {
                sortCards.Remove(14);
                sortCards.Remove(14);
                int max = sortCards.Max();
                sortCards.Add(14);
                sortCards.Add(14);
                if (max == 13)
                {
                    tmpOfJokers.Add(10);
                    tmpOfJokers.Add(9);
                }
                else if (max == 12)
                {
                    tmpOfJokers.Add(13);
                    tmpOfJokers.Add(9);
                }
                else
                {
                    tmpOfJokers.Add(max + 1);
                    tmpOfJokers.Add(max + 2);
                }
            }
            return true;
        }

        public bool is2Same(List<int> sortCards)
        {

            bool result = false;

            // 判断第一对
            if (sortCards.ElementAt(0) == sortCards.ElementAt(1) &&
                    sortCards.ElementAt(1) != sortCards.ElementAt(2))
            {
                result = true;

                hurtCards.Add(sortCards.ElementAt(0));
                hurtCards.Add(sortCards.ElementAt(1));
            }

            // 判断第2对
            if (sortCards.ElementAt(0) != sortCards.ElementAt(1) &&
                    sortCards.ElementAt(1) == sortCards.ElementAt(2) &&
                    sortCards.ElementAt(2) != sortCards.ElementAt(3))
            {
                result = true;

                hurtCards.Add(sortCards.ElementAt(1));
                hurtCards.Add(sortCards.ElementAt(2));
            }

            // 判断第3对
            if (sortCards.ElementAt(1) != sortCards.ElementAt(2) &&
                    sortCards.ElementAt(2) == sortCards.ElementAt(3) &&
                    sortCards.ElementAt(3) != sortCards.ElementAt(4))
            {
                result = true;

                hurtCards.Add(sortCards.ElementAt(3));
                hurtCards.Add(sortCards.ElementAt(2));
            }

            // 判断最后一对
            if (sortCards.ElementAt(2) != sortCards.ElementAt(3) &&
                    sortCards.ElementAt(3) == sortCards.ElementAt(4)
                    && sortCards.ElementAt(3) != 14)
            {
                result = true;

                hurtCards.Add(sortCards.ElementAt(3));
                hurtCards.Add(sortCards.ElementAt(4));
            }
            return result;
        }

        /// <summary>
        /// 获得伤害牌的数量
        /// </summary>
        /// <returns></returns>
        public int getReplaceNum()
        {
            return (tmpOfJokers.Count + hurtCards.Count);
        }

        /******释放技能区*****/
        public bool isMonsterStep()
        {
            if (curRound % 3 == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 实现monster的技能效果
        /// </summary>
        public void DoMonsterSkill()
        {
            int loc = new Random().Next(boardCards.Count);

            boardCards.RemoveAt(loc);
            boardCardsTag.Add(loc);
            boardCardsTag.Sort();
            Debug.WriteLine("Monster do the skill: steal the location " + loc + " in board");
        }
    }
}
