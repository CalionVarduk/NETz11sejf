using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NETz11sejf
{
    public class Player : GameEntity
    {
        public bool IsPursued { get; set; }
        public bool IsInSafeZone { get { return Collider.AsRectangles(this, SafeZone); } }
        public InteractionZone SafeZone { get; private set; }

        public event EntityEventHandler OnReturnedToSafeZone;

        public Player(float health, float size, Color color) : base()
        {
            MaxHealth = health;
            Width = size;
            Height = size;
            MainColor = color;
            IsPursued = false;
            SafeZone = new InteractionZone(this, 2, false);
            SafeZone.MainColor = MainColor;
        }

        public void spawn(float locX, float locY)
        {
            Health = MaxHealth;
            LocationX = locX;
            LocationY = locY;
            SafeZone.spawn(this);
            SafeZone.Width = Width + 85;
            SafeZone.Height = Height + 85;
            IsPursued = false;
            IsActive = true;
        }

        public override void update()
        {
            if (IsActive)
            {
                move();
                forceInsideScreen();

                if (IsPursued && IsInSafeZone)
                {
                    IsPursued = false;
                    if (OnReturnedToSafeZone != null) OnReturnedToSafeZone(this);
                }
            }
        }

        protected override void drawObject(Graphics g)
        {
            SafeZone.draw(g);
            g.FillEllipse(ObjectPen.Brush, Left, Top, Width, Height);
            drawDefaultHealthBar(g);
        }
    }
}
