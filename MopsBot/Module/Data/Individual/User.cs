﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MopsBot.Module.Data.User
{
    class User
    {
        public ulong ID;
        public int Score, Experience, Level, monster;

        public User(ulong userID, int userScore, int XP, int MIP)
        {
            ID = userID;
            Score = userScore;
            Experience = XP;
            Level = calcLevel();
            monster = MIP;
        }

        public User(ulong userID, int userScore)
        {
            ID = userID;
            Score = userScore;
        }

        internal delegate double del(int i);
        internal static del levelCalc = x => (200*(x*x));

        private int calcLevel()
        {
            int i = 0;
            while(Experience > levelCalc(i))
            {
                i++;
            }
            return (i - 1);
        }

        internal string calcNextLevel()
        {
            double expCurrentHold = Experience - levelCalc(Level);
            string output = "", TempOutput = "";
            double diffExperience = levelCalc(Level + 1) - levelCalc(Level);
            for (int i = 0; i < (expCurrentHold/(diffExperience/10)); i++)
            {
                output += "■";
            }
            for (int i = 0; i < 10-output.Length; i++)
            {
                TempOutput += "□";
            }
            return output + TempOutput;
        }

        //public int getHP()
        //{
        //    return (Level * Level + 3) + (new Data.Items().getItem(monster).itemHP);
        //}
    }
}