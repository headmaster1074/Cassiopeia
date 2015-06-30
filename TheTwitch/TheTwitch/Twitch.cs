﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using TheTwitch.Commons;
using TheTwitch.Commons.ComboSystem;

namespace TheTwitch
{
    class Twitch
    {
        private Orbwalking.Orbwalker _orbwalker;

        public void Load(EventArgs eArgs)
        {
            try
            {
                Load2(eArgs);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }
        }

        public void Load2(EventArgs eArgs)
        {
            if (ObjectManager.Player.ChampionName != "Twitch")
                return;

            var notification = new Notification("The Twitch loaded", 3) { TextColor = new SharpDX.ColorBGRA(0, 255, 0, 255), BorderColor = new SharpDX.ColorBGRA(144, 238, 144, 255) };
            Notifications.AddNotification(notification);

            var mainMenu = CreateMenu("The Twitch", true);
            var orbwalkerMenu = CreateMenu("Orbwalker", mainMenu);
            var targetSelectorMenu = CreateMenu("Targetselector", mainMenu);
            var comboMenu = CreateMenu("Combo", mainMenu);
            var harassMenu = CreateMenu("Harass", mainMenu);
            var laneclearMenu = CreateMenu("Laneclear", mainMenu);
            var manamanagerMenu = CreateMenu("Manamanager", mainMenu);
            var itemMenu = CreateMenu("Items", mainMenu);
            var antigapcloserMenu = CreateMenu("Anti gapcloser", mainMenu);
            var miscMenu = CreateMenu("Misc", mainMenu);
            // var healMenu = CreateMenu("Heal", mainMenu);
            var drawingMenu = CreateMenu("Drawing", mainMenu);


            _orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            TargetSelector.AddToMenu(targetSelectorMenu);

            var combo = new ComboProvider(1000, _orbwalker, new TwitchQ(new Spell(SpellSlot.Q)), new TwitchW(new Spell(SpellSlot.W)), new TwitchE(new Spell(SpellSlot.E)), new TwitchR(new Spell(SpellSlot.R)));

            combo.CreateBasicMenu(comboMenu, harassMenu, null, antigapcloserMenu, null, manamanagerMenu, null, itemMenu, interrupter: false, ignitemanager: false, laneclear: false);
            combo.CreateLaneclearMenu(laneclearMenu, true, SpellSlot.Q);

            comboMenu.AddMItem("Only R in teamfight", true, (sender, args) => combo.GetSkill<TwitchR>().OnlyInTeamfight = args.GetNewValue<bool>()).ProcStoredValueChanged<bool>();

            harassMenu.AddMItem("E after trade", true, (sender, args) => combo.GetSkill<TwitchE>().HarassActivateWhenLeaving = args.GetNewValue<bool>()).ProcStoredValueChanged<bool>();

            antigapcloserMenu.AddMItem("Uses W if enabled");

            laneclearMenu.AddMItem("Min W targets", new Slider(3, 1, 8), (sender, args) => combo.GetSkill<TwitchW>().MinFarmMinions = args.GetNewValue<Slider>().Value);
            laneclearMenu.AddMItem("Min E kills", new Slider(2, 1, 8), (sender, args) => combo.GetSkill<TwitchE>().MinFarmMinions = args.GetNewValue<Slider>().Value);
            laneclearMenu.AddMItem("Min E targets", new Slider(2, 1, 16), (sender, args) => combo.GetSkill<TwitchE>().MinFarmDamageMinions = args.GetNewValue<Slider>().Value);
            laneclearMenu.AddMItem("(Uses E if kills OR targets are here)");
            laneclearMenu.ProcStoredValueChanged<Slider>();

            miscMenu.AddMItem("E Killsteal", true, (sender, args) => combo.GetSkill<TwitchE>().Killsteal = args.GetNewValue<bool>());
            miscMenu.AddMItem("E farm assist", false, (sender, args) => combo.GetSkill<TwitchE>().FarmAssist = args.GetNewValue<bool>());
            miscMenu.AddMItem("W AOE prediction", true, (sender, args) => combo.GetSkill<TwitchW>().IsAreaOfEffect = args.GetNewValue<bool>());
            miscMenu.AddMItem("Stealh recall", true, (sender, args) => combo.GetSkill<TwitchQ>().StealthRecall = args.GetNewValue<bool>());
            miscMenu.ProcStoredValueChanged<bool>();

            drawingMenu.AddMItem("Draw Q Range", new Circle(true, Color.Gray), (sender, args) => combo.GetSkill<TwitchQ>().DrawRange = args.GetNewValue<Circle>());
            drawingMenu.AddMItem("Draw W Range", new Circle(false, Color.LightGreen), (sender, args) => combo.GetSkill<TwitchW>().DrawRange = args.GetNewValue<Circle>());
            drawingMenu.AddMItem("Draw E Range", new Circle(true, Color.DarkGreen), (sender, args) => combo.GetSkill<TwitchE>().DrawRange = args.GetNewValue<Circle>());
            drawingMenu.AddMItem("Draw R Range", new Circle(false, Color.Goldenrod), (sender, args) => combo.GetSkill<TwitchR>().DrawRange = args.GetNewValue<Circle>());
            drawingMenu.ProcStoredValueChanged<Circle>();

            combo.Initialize();
            mainMenu.AddToMainMenu();

        }


        private Menu CreateMenu(string name, Menu menu)
        {
            var newMenu = new Menu(name, name);
            menu.AddSubMenu(newMenu);
            return newMenu;
        }

        private Menu CreateMenu(string name, bool root = false)
        {
            return new Menu(name, name, root);
        }
    }
}
