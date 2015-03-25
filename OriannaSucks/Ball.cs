using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Drawing;
using Color = System.Drawing.Color;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OriannaSucks
{
	public class Ball
	{
		private Vector3 Position;
		private bool DoDrawings = true;
		private Vector3 vector3;
		public Ball(Vector3 position)
		{
			Position = position;
			Drawing.OnDraw += Drawing_OnDraw;
		}

		void Drawing_OnDraw(EventArgs args)
		{
			if (DoDrawings)
			{
				Drawing.DrawCircle(Position, 250, Color.Blue);
				Drawing.DrawCircle(Position, 370, Color.Red);
			}
			
		}

		public Vector3 getPosition()
		{
			return Position;
		}

		public void setPosition(Vector3 position)
		{
			Position = position;
		}
		public void enableDrawings(bool state)
		{
			DoDrawings = state;
		}

	}
}
