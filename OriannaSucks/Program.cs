﻿#region includes
using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Color = System.Drawing.Color;

#endregion
namespace OriannaSucks
{
	class Program
	{
		private static Menu MainMenu;
		private static Orbwalking.Orbwalker myOrb;
		private static string Championname = "Orianna";
		private static bool hasBall = false;
		private static bool draw;
		private static Ball userBall;
		private static int lastTick;
		private static Obj_AI_Hero Player;
		private static Spell Q, W, E, R;
		static void Main(string[] args)
		{
			Game.OnStart += onLoad;

		}
		static void onLoad(EventArgs args)
		{
			Player = ObjectManager.Player;
			if (Player.BaseSkinName != Championname)
				return;
			lastTick = Environment.TickCount;
			Drawing.OnDraw += Drawing_OnDraw;
			userBall = new Ball(Player.Position);
			Game.OnUpdate += onUpdate;
			Q = new Spell(SpellSlot.Q, 825, TargetSelector.DamageType.Magical);
			W = new Spell(SpellSlot.W, 250, TargetSelector.DamageType.Magical);
			E = new Spell(SpellSlot.E, 1100, TargetSelector.DamageType.Magical);
			R = new Spell(SpellSlot.R, 370, TargetSelector.DamageType.Magical);
			LoadSkillshots();

		}

		static void Drawing_OnDraw(EventArgs args)
		{
			if (draw)
			{
				Drawing.DrawCircle(Player.Position, Q.Range, Color.Green);
				Drawing.DrawCircle(Player.Position, E.Range, Color.Yellow);
			}
			
		}
		static void onUpdate(EventArgs args)
		{
			draw = MainMenu.Item("DoDrawings").GetValue<bool>();
			
			userBall.enableDrawings(draw);
			
			if (Player.IsDead)
				return;
			if(!hasBall && userBall.getPosition().Distance(Player.Position) > 1100){
				userBall.setPosition(Player.Position);
				UpdateSkillshots();
				hasBall = true;
			}
			if (hasBall &&  Environment.TickCount-lastTick >= 250 )
			{
				userBall.setPosition(Player.Position);
				lastTick = Environment.TickCount;
			}
			if (MainMenu.Item("ComboActive").GetValue<KeyBind>().Active)
			{
				var QTarget = Q.GetTarget();
				CastQ(QTarget);
				var WTarget = W.GetTarget();
				CastW(WTarget);
				var ETarget = E.GetTarget();
				CastE(Player, E.GetTarget());
				var RTarget = R.GetTarget();
				CastR(RTarget);
			}

		}
		static void constructMenu()
		{
			MainMenu = new Menu(Championname, Championname, true);
			TargetSelector.AddToMenu(MainMenu.SubMenu("Target Selector"));
			myOrb = new Orbwalking.Orbwalker(MainMenu.SubMenu("Orbwalking"));
			MainMenu.AddSubMenu(new Menu("Combo", "Combo"));
			MainMenu.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "Combo!").SetValue(new KeyBind(MainMenu.Item("Orbwalk").GetValue<KeyBind>().Key, KeyBindType.Press)));
			MainMenu.AddSubMenu(new Menu("Drawings", "Drawings"));
			MainMenu.SubMenu("Drawings").AddItem(new MenuItem("DoDrawings", "Draw Stuff :^)").SetValue(true));

		}
		static void LoadSkillshots()
		{
			Q.SetSkillshot(0.25f, 80, 1300, false, SkillshotType.SkillshotLine,userBall.getPosition());
			W.SetSkillshot(0f, 250, float.MaxValue, false, SkillshotType.SkillshotCircle, userBall.getPosition(),userBall.getPosition());
			E.SetSkillshot(0.25f, 145, 1700, false, SkillshotType.SkillshotLine, userBall.getPosition());
			R.SetSkillshot(0.60f, 370, float.MaxValue, false, SkillshotType.SkillshotCircle, userBall.getPosition(),userBall.getPosition());
		}
		static void UpdateSkillshots()
		{
			Q.UpdateSourcePosition(userBall.getPosition());
			W.UpdateSourcePosition(userBall.getPosition(), userBall.getPosition());
			E.UpdateSourcePosition(userBall.getPosition());
			R.UpdateSourcePosition(userBall.getPosition(),userBall.getPosition());
		}
		static void CastQ(Obj_AI_Base target)
		{
			if (Q.CanCast(target))
			{
				var output = Q.GetPrediction(target, true);
				if (output.Hitchance == HitChance.VeryHigh)
				{
					Q.Cast(output.CastPosition);
					userBall.setPosition(output.CastPosition);
					UpdateSkillshots();
					hasBall = false;
				}
				
			}
		}
		static void CastW(Obj_AI_Base target)
		{
			if (W.CanCast(target))
			{
				W.CastIfHitchanceEquals(target, HitChance.VeryHigh);
			}
		}
		static void CastE(Obj_AI_Base ShieldTarget, Obj_AI_Base DamageTarget)
		{
			if (E.CanCast(ShieldTarget))
			{
				float AngleA = ShieldTarget.Position.To2D().AngleBetween(userBall.getPosition().To2D());
				float AngleB = DamageTarget.Position.To2D().AngleBetween(userBall.getPosition().To2D());
				float HealthPercentage =  ShieldTarget.Health / ShieldTarget.MaxHealth;
				if (AngleA == AngleB || HealthPercentage <= 0.25f)
				{
					E.CastOnUnit(ShieldTarget);
					userBall.setPosition(ShieldTarget.Position);
					UpdateSkillshots();
					if (ShieldTarget == Player)
					{
						hasBall = true;
					}
					else
					{
						hasBall = false;
					}

				}
			}
		}
		static void CastR(Obj_AI_Base target)
		{
			if (R.IsReady())
			{
				var output = R.GetPrediction(target);
				if (output.AoeTargetsHitCount >= 2 && output.Hitchance == HitChance.VeryHigh)
				{
					R.Cast();
				}

				
			}
		}
	}
}