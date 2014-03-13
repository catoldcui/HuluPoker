using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

using System.IO.IsolatedStorage;

namespace Game1.Model
{
    public class Config
    {
        public static bool IsXmlExist = false;
        public static string Version = "v1.1.140304";
        // 每个关卡血量
        public static int[] MonstersBlood1 = {
                                        5, 20, 15, 200, 20, 25, 30, 50, 70, 3
                                    };
        public static int[] MonstersBlood = {
                                        50, 50, 100, 100, 200, 200, 250, 250, 300, 350, 150, 150, 200, 200, 200,200, 100
                                    };
        public static int LevelNum = 17;
        public static int CurLevel = 0;
        public static int FaceLevel;
        public static int HeroFullBlood = 100;
        public static int HeroStartBlood = 100;
        public static bool IsMusicOpen = true;
        public static bool IsSoundOpen = true;

        public static List<int> WinList = new List<int>();

        public int curlevel;
        public List<int> winlist;
        public bool ismusicopen;
        public bool issoundopen;
        public int heroStartBlood;

        public Config()
        {
            curlevel = 0;
            winlist = new List<int>();
            winlist.Add(0);
            ismusicopen = IsMusicOpen;
            issoundopen = IsSoundOpen;
            heroStartBlood = HeroFullBlood;
        }
        public static void setCurLevel(int num)
        {
            CurLevel = num;
        }

        public Config GenerateData()
        {
            winlist.Clear();
            WinList = WinList.Distinct().ToList();
            WinList.ForEach(
                delegate(int ele)
                {
                    winlist.Add(ele);
                });
            curlevel = CurLevel;
            heroStartBlood = HeroStartBlood;

            return this;
        }

        public static void setConfig(Config c){
            WinList.Clear();
            c.winlist = c.winlist.Distinct().ToList();
            c.winlist.ForEach(
                 delegate(int ele)
                 {
                     WinList.Add(ele);
                 });
            if (!WinList.Contains(0))
            {
                WinList.Add(0);
            }
            CurLevel = c.curlevel;
            IsMusicOpen = c.ismusicopen;
            IsSoundOpen = c.issoundopen;
            HeroStartBlood = c.heroStartBlood;
        }
    }
}
