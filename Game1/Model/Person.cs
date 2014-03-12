using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.Model
{
    public class Person
    {
        public int fullBlood = 0;
        public int curBlood = 0;
        public double percentOfFullBlood = 1;

        public Person(int fullBlood)
        {
            this.fullBlood = fullBlood;
            this.curBlood = fullBlood;
        }

        public void hurt(int hurtNum)
        {
            this.curBlood -= hurtNum;
            this.percentOfFullBlood = curBlood / (double)fullBlood;
        }

        public bool isDie()
        {
            return (curBlood <= 0);
        }
    }
}
