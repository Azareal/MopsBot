﻿using System;
using ChatterBotAPI;
using System.Web;
using System.Timers;
using Discord;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using Discord.Commands.Permissions.Visibility;
using Discord.Modules;
using MopsBot.Module.Data;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace MopsBot.Module
{
    internal class Game : IModule
    {
        private ModuleManager _manager;
        private DiscordClient _client;
        private Timer aTimer;
        private User victim, culprit;
        private Channel beenSearch;
        private Random ran = new Random();
        int status = 0, quest = 0, number, HP1, HP2;
        string[] wires, wiresUsed;
        public static Data.UserScore userScores;
        string wire;

        void IModule.Install(ModuleManager manager)
        {
            aTimer = new Timer();
            aTimer.Elapsed += ATimer_Elapsed;
            wires = new string[] { "salmon", "purple", "aquamarine", "emerald", "apricot", "cerulean", "peach", "blue", "red", "yellow", "black", "white", "green", "orange", "cyan", "beige", "grey", "gold", "buff", "monza", "rose", "tan", "brown", "flax", "pink" };

            Random random = new Random();
            userScores = new UserScore();

            ChatterBotFactory factory = new ChatterBotFactory();

            ChatterBot _Cleverbot = factory.Create(ChatterBotType.CLEVERBOT);
            ChatterBotSession Cleverbot1session = _Cleverbot.CreateSession();

            ChatterBot bot2 = factory.Create(ChatterBotType.PANDORABOTS, "b0dafd24ee35a477");
            ChatterBotSession bot2session = bot2.CreateSession();

            _manager = manager;
            _client = manager.Client;

            _client.MessageReceived += _client_MessageReceived;

            manager.CreateCommands("", group =>
            {
                group.PublicOnly();

                group.CreateCommand("kawush")
                .Description("Makes Kabooom")
                .Parameter("User")
                .Do(async e =>
                {
                    e.Args[0] = e.Args[0].Replace("!","");
                    if (victim == null && e.Server.FindUsers(e.Args[0], false).FirstOrDefault().Status.Value.Equals("online") && status == 0)
                    {
                        status = 1;

                        aTimer.Interval = random.Next(30000, 60000);
                        aTimer.Start();

                        int wiresrandom = random.Next(2, 11);
                        string output = "";
                        wiresUsed = new string[wiresrandom];

                        for (int i = 0; i < wiresrandom; i++)
                        {
                            int innerrandom = random.Next(0, wires.Length);
                            if (!wiresUsed.Contains(wires[innerrandom]))
                            {
                                wiresUsed[i] = wires[innerrandom];
                                output += $" {wiresUsed[i]} ";
                            }
                            else i--;
                        }
                        wire = wiresUsed[random.Next(0, wiresUsed.Length)];
                        victim = e.Server.FindUsers(e.Args[0]).FirstOrDefault();

                        if (victim.IsBot)
                        {
                            await e.Channel.SendMessage("How dare you attack a Bot like this? How about you taste your own medicine?");
                            victim = e.User;
                        }
                        if (victim != e.User) culprit = e.User;
                        beenSearch = e.Channel;
                        await e.Channel.SendMessage($"{victim.Name} is being bombed!\n" +
                                                    "Quick, find the right wire to cut!\n" +
                                                    $"({output})\n" +
                                                    $"You have got {(int)(aTimer.Interval / 1000)} seconds before the bomb explodes!");
                    }
                    else await e.Channel.SendMessage("Either the User is not online, or the Command is already in use!");
                });

                group.CreateCommand("rollDice")
                .Description("Rolls a random Number between Min and Max")
                .Parameter("Min")
                .Parameter("Max")
                .Do(async e =>
                {
                    await e.Channel.SendMessage($"[{random.Next(int.Parse(e.Args[0]), int.Parse(e.Args[1]) + 1)}]");
                });

                group.CreateCommand("Slotmachine")
                .Description("Costs 5$")
                .Do(e =>
               {
                   if (findDataUser(e.User).Score < 5)
                   {
                       e.Channel.SendMessage("Not enough money");
                       return;
                   }
                   Random rnd = new Random();
                   int[] num = { rnd.Next(0, 6), rnd.Next(0, 6), rnd.Next(0, 6) }; //0=Bomb, 1=Cherry, 2= Free, 3= cookie, 4= small, 5= big
                   bool won = true;
                   int amount = 0;

                   if (num[0] == num[1] && num[1] == num[2])
                   {
                       switch (num[0])
                       {
                           case 0:
                               won = false;
                               amount = 250;
                               break;
                           case 1:
                               amount = 40;
                               break;
                           case 2:
                               amount = 50;
                               break;
                           case 3:
                               amount = 100;
                               break;
                           case 4:
                               amount = 200;
                               break;
                           case 5:
                               amount = 500;
                               break;
                       }
                   }
                   else if ((num[0] == num[1] && num[0] == 1) || (num[0] == num[2] && num[0] == 1) || (num[1] == num[2] && num[1] == 1))
                   {
                       won = true;
                       amount = 20;
                   }
                   else if (num[0] == 1 || num[1] == 1 | num[2] == 1)
                   {
                       amount = 5;
                   }
                   if (won)
                   {
                       addToBase(e.User, amount - 5);
                   }
                   else {
                       addToBase(e.User, (5 + amount) * -1);
                   }
                   e.Channel.SendMessage("––––––––––––––––––––\n ¦   " + ((num[0] == 0) ? "💣" : ((num[0] == 1) ? "🆓" : ((num[0] == 2) ? "🍒" : ((num[0] == 3) ? "🍪" : ((num[0] == 4) ? "🔹" : "🔷"))))) + "   ¦  " + ((num[1] == 0) ? "💣" : ((num[1] == 1) ? "🆓" : ((num[1] == 2) ? "🍒" : ((num[1] == 3) ? "🍪" : ((num[1] == 4) ? "🔹" : "🔷"))))) + "   ¦  " + ((num[2] == 0) ? "💣" : ((num[2] == 1) ? "🆓" : ((num[2] == 2) ? "🍒" : ((num[2] == 3) ? "🍪" : ((num[2] == 4) ? "🔹" : "🔷"))))) + "  ¦\n ¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯\n 🆓=5$   🆓🆓=20$   🆓🆓🆓=40$\n 🍒🍒🍒=50$ 🍪🍪🍪=100$\n 🔹🔹🔹=200$ 🔷🔷🔷=500$\n 💣💣💣=-250$\n\n You " + ((won) ? "won " : "lost ") + amount + "$" +
                       $"\nYou now have {findDataUser(e.User).Score}$");
               });

                group.CreateCommand("1-")
                .Description("Let's you talk to Chomsky")
                .Parameter("Message", ParameterType.Unparsed)
                .Do(async e =>
                {
                    await e.Channel.SendMessage($"{bot2session.Think(e.Args[0])}");
                });

                group.CreateCommand("2-")
                .Description("Let's you talk to CleverBot")
                .Parameter("Message", ParameterType.Unparsed)
                .Do(async e =>
                {
                    await e.Channel.SendMessage($"{HttpUtility.HtmlDecode(Cleverbot1session.Think(e.Args[0]))}");
                });

                group.CreateCommand("cleverCeption")
               .Description("Bots talk to each other for a fixed amount of messages. Try not to abuse!")
               .Parameter("Message Count", ParameterType.Required)
               .Parameter("Starting Message", ParameterType.Unparsed)
               .Do(async e =>
               {
                   ChatterBot _Cleverbot2 = factory.Create(ChatterBotType.CLEVERBOT);
                   ChatterBotSession Cleverbot2session = _Cleverbot2.CreateSession();

                   string message = e.Args[1] != "" ? e.Args[1] : "Hello";
                   await e.Channel.SendMessage("A: " + message);
                                  
                   for(int count = 0; count < int.Parse(e.Args[0]); count++)
                   {
                       if(count % 2 != 0)
                       {
                           message = HttpUtility.HtmlDecode(Cleverbot1session.Think(message));
                           await e.Channel.SendMessage("A: " + message);
                       }
                       else if(count % 2 == 0)
                       {
                           message = HttpUtility.HtmlDecode(Cleverbot2session.Think(message));
                           await e.Channel.SendMessage("B: " + message);
                       }
                   }
               });       
            });

            manager.CreateCommands("ranking", group =>
            {
                group.PublicOnly();

                group.CreateCommand("Score")
                .Description("get the top scoreboard")
                .Parameter("(true) to get global ranking", ParameterType.Optional)
                .Do(async e =>
                {
                    List<Data.User.User> tempSort = userScores.users.OrderByDescending(u => u.Score).ToList();

                    string output = "";

                    if (e.Args[0] == "")
                    {
                        int count = 0;

                        foreach (Data.User.User curUser in tempSort)
                        {
                            if (count >= 10) break;
                            count++;
                            try
                            {
                                output += $"#{count} ``$ {curUser.Score} $`` by {e.Server.GetUser(curUser.ID).Name}\n";
                            }
                            catch (NullReferenceException ex)
                            {
                                count--;
                            }
                        }
                    }
                    else if (e.Args[0] == "true")
                    {
                        for (int i = 0; i < tempSort.Count; i++)
                        {
                            if (i >= 10) break;
                            try
                            {
                                output += $"#{i + 1} ``$ {tempSort[i].Score} $`` by {e.Server.GetUser(tempSort[i].ID).Name}\n";
                            }
                            catch (NullReferenceException ex)
                            {
                                output += $"#{i + 1} ``$ {tempSort[i].Score} $`` by **global** {findUser(tempSort[i].ID, _client)}\n";
                            }
                        }
                    }

                    await e.Channel.SendMessage(output);
                });

                group.CreateCommand("Experience")
                .Description("get the top expboard")
                .Parameter("(true) to get global ranking", ParameterType.Optional)
                .Do(async e =>
                {
                    List<Data.User.User> tempSort = userScores.users.OrderByDescending(u => u.Experience).ToList();

                    string output = "";

                    if (e.Args[0] == "")
                    {
                        int count = 0;

                        foreach (Data.User.User curUser in tempSort)
                        {
                            if (count >= 10) break;
                            count++;
                            try
                            {
                                output += $"#{count} ``{curUser.Experience} EXP`` by {e.Server.GetUser(curUser.ID).Name}\n";
                            }
                            catch (NullReferenceException ex)
                            {
                                count--;
                            }
                        }
                    }
                    else if (e.Args[0] == "true")
                    {
                        for (int i = 0; i < tempSort.Count; i++)
                        {
                            if (i >= 10) break;
                            try
                            {
                                output += $"#{i + 1} ``{tempSort[i].Experience} EXP`` by {e.Server.GetUser(tempSort[i].ID).Name}\n";
                            }
                            catch (NullReferenceException ex)
                            {
                                output += $"#{i + 1} ``{tempSort[i].Experience} EXP`` by **global** {findUser(tempSort[i].ID, _client)}\n";
                            }
                        }
                    }

                    await e.Channel.SendMessage(output);
                });

                group.CreateCommand("Level")
                .Description("get the top levelboard")
                .Parameter("(true) to get global ranking", ParameterType.Optional)
                .Do(async e =>
                {
                    List<Data.User.User> tempSort = userScores.users.OrderByDescending(u => u.Level).ToList();

                    string output = "";

                    if (e.Args[0] == "")
                    {
                        int count = 0;

                        foreach (Data.User.User curUser in tempSort)
                        {
                            if (count >= 10) break;
                            count++;
                            try
                            {
                                output += $"#{count} ``Level {curUser.Level}`` by {e.Server.GetUser(curUser.ID).Name}\n";
                            }
                            catch (NullReferenceException ex)
                            {
                                count--;
                            }
                        }
                    }
                    else if (e.Args[0] == "true")
                    {
                        for (int i = 0; i < tempSort.Count; i++)
                        {
                            try
                            {
                                output += $"#{i + 1} ``Level {tempSort[i].Level}`` by {e.Server.GetUser(tempSort[i].ID).Name}\n";
                            }
                            catch (NullReferenceException ex)
                            {
                                output += $"#{i + 1} ``Level {tempSort[i].Level}`` by **global** {findUser(tempSort[i].ID, _client)}\n";
                            }
                        }
                    }

                    await e.Channel.SendMessage(output);
                });

                //group.CreateCommand("getStats")
                //.Description("Returns your current Stats")
                //.Do(async e =>
                //{
                //    int curLevel = findDataUser(e.User).Level;
                //    await e.Channel.SendMessage($"${findDataUser(e.User).Score}\n" +
                //                                $"HP: {findDataUser(e.User).getHP()} \n" +
                //                                $"Level: {curLevel} (Experience Bar: {findDataUser(e.User).calcNextLevel()})\n" +
                //                                $"DMG: {findDataUser(e.User).getDmg() / 2} - {findDataUser(e.User).getDmg()}\n" +
                //                                $"EXP: {findDataUser(e.User).Experience}\n" +
                //                                $"Item: {new Data.Items().getItem(findDataUser(e.User).monster).itemInfo()}");
                //});
            });
            }

        private void _client_MessageReceived(object sender, MessageEventArgs e)
        {
            if (e.User.IsBot) return;

            try
            {
                int pre = -1;
                if (userScores.users.Any(x => x.ID == e.User.Id)) pre = findDataUser(e.User).Level;
                addToBase(e.User, 0, e.Message.RawText.Length);
                if (pre != -1 && pre < findDataUser(e.User).Level) e.Channel.SendMessage($"{e.User.Name} advanced from level {pre} to level {findDataUser(e.User).Level}!");

                if (e.User == victim)
                {
                    switch (status)
                    {
                        case 0:
                            break;
                        case 1:
                            if (wiresUsed.Contains(e.Message.Text) && !wire.Contains(e.Message.Text))
                            {
                                if (culprit == null && 1 == new Random().Next(1, 3)) e.Channel.SendMessage("That wire did nothing! Quick, try another one!\n");
                                else if (culprit != null && 1 == new Random().Next(1, 4)) e.Channel.SendMessage("That wire did nothing! Quick, try another one!\n");
                                else if (culprit != null && 2 == new Random().Next(1, 3))
                                {
                                    e.Channel.SendMessage($"The bomb somehow jumped over to {culprit.Mention}! Quick, cut a wire!\n");
                                    User vic = victim;
                                    victim = culprit;
                                    culprit = vic;
                                }
                                else
                                {
                                    e.Channel.SendMessage($"No! The right wire was {wire}\nWOOOOSH!\n...ehe, I don't really have an actual bomb.");
                                    if (culprit != null)
                                    {
                                        addToBase(culprit, wires.Length / wiresUsed.Length);
                                        addToBase(victim, wires.Length / wiresUsed.Length * -1);
                                        e.Channel.SendMessage($"Anyway, {culprit.Name} just took ``[$ {wires.Length / wiresUsed.Length} $]`` from you. {culprit.Name} now has ${findDataUser(culprit).Score}.\nYou now have ${findDataUser(victim).Score}.");
                                    }
                                    else
                                    {
                                        addToBase(victim, wires.Length / wiresUsed.Length * -1);
                                        e.Channel.SendMessage($"Also, during the imaginary explosion, ``[$ {wires.Length / wiresUsed.Length} $]`` disappeared from your pocket!\nYou now have ${findDataUser(victim).Score}.");
                                    }
                                    unDo();
                                }
                            }
                            else if (wiresUsed.Contains(e.Message.Text) && wire.Contains(e.Message.Text))
                            {
                                addToBase(e.User, wiresUsed.Length * 3);
                                e.Channel.SendMessage($"Wow! Good job o.o\n``[$ {wiresUsed.Length * 3} $]`` You now have ${findDataUser(e.User).Score} in total!");
                                unDo();
                            }
                            break;
                    }
                }
            }
            catch(ArgumentOutOfRangeException ex)
            {
                e.Channel.SendMessage(ex.Message);
            }
        }

        private void ATimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            switch (status)
            {
                case 1:
                    beenSearch.SendMessage($"WOOOOSH! The correct wire was {wire}\n...ehe, I don't really have an actual bomb.");
                    if (culprit != null)
                    {
                        addToBase(culprit, wires.Length / wiresUsed.Length);
                        addToBase(victim, wires.Length / wiresUsed.Length * -1);
                        beenSearch.SendMessage($"Anyway, {culprit.Name} just took ``[$ {wires.Length / wiresUsed.Length} $]`` from you. {culprit.Name} now has ${findDataUser(culprit).Score}.\nYou now have ${findDataUser(victim).Score}.");
                    }
                    else
                    {
                        addToBase(victim, wires.Length / wiresUsed.Length * -1);
                        beenSearch.SendMessage($"Also, during the imaginary explosion, ``[$ {wires.Length / wiresUsed.Length} $]`` disappeared from your pocket!\nYou now have ${findDataUser(victim).Score}.");
                    }
                    break;


            }
            unDo();
        }

        private void addToBase(User user, int score)
        {
            userScores = new Data.UserScore();
            for(int i = 0; i<userScores.users.Count; i++)
            {
                if(userScores.users[i].ID == user.Id)
                {
                    score += userScores.users[i].Score;
                    int exp = userScores.users[i].Experience;
                    int mip = userScores.users[i].monster;
                    userScores.users.RemoveAt(i);
                    userScores.users.Add(new Data.User.User(user.Id, score, exp, mip));
                    userScores.writeScore();
                    return;
                }
            }
            userScores.users.Add(new Data.User.User(user.Id, score, 0, 0));
            userScores.writeScore();
        }

        private void addToBase(User user, int score, int exp)
        {
            userScores = new Data.UserScore();
            for (int i = 0; i < userScores.users.Count; i++)
            {
                if (userScores.users[i].ID == user.Id)
                {
                    score += userScores.users[i].Score;
                    exp += userScores.users[i].Experience;
                    int mip = userScores.users[i].monster;
                    userScores.users.RemoveAt(i);
                    userScores.users.Add(new Data.User.User(user.Id, score, exp, mip));
                    userScores.writeScore();
                    return;
                }
            }
            userScores.users.Add(new Data.User.User(user.Id, score, exp, 0));
            userScores.writeScore();
        }
        
        private void addToBase(User user, int score, int exp, int MIP)
        {
            userScores = new Data.UserScore();
            for (int i = 0; i < userScores.users.Count; i++)
            {
                if (userScores.users[i].ID == user.Id)
                {
                    score += userScores.users[i].Score;
                    exp += userScores.users[i].Experience;
                    userScores.users.RemoveAt(i);
                    userScores.users.Add(new Data.User.User(user.Id, score, exp, MIP));
                    userScores.writeScore();
                    return;
                }
            }
            userScores.users.Add(new Data.User.User(user.Id, score, exp, MIP));
            userScores.writeScore();
        }

        public static Data.User.User findDataUser(User user)
        {
            userScores = new Data.UserScore();
            for (int i = 0; i < userScores.users.Count; i++)
            {
                if (userScores.users[i].ID == user.Id)
                {
                    return userScores.users[i];
                }
            }
            return null;
        }

        public static string findUser(ulong ID, DiscordClient _client)
        {
            string userName = ID.ToString();

            foreach(Server se in _client.Servers)
            {
                if (se.GetUser(ID) != null) userName = se.GetUser(ID).Name;
            }

            return userName;
        }

        private void unDo()
        {
            culprit = null;
            aTimer.Stop();
            status = 0;
            victim = null;
        }
    }
}