using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NETz11sejf
{
    public class Bullet : GameEntity
    {
        public static float BulletVelocity = 3f;
        public static float BulletDamage = 15;

        public Bullet(float size, Color color) : base()
        {
            Velocity = BulletVelocity;
            Health = MaxHealth;
            Width = size;
            Height = size;
            MainColor = color;
        }

        public void spawn(float locX, float locY, GameEntity target)
        {
            LocationX = locX;
            LocationY = locY;
            pointInDirectionOf(target);
            IsActive = true;
        }

        public override void update()
        {
            if (IsActive)
            {
                move();
                if (IsOutsideScreen) IsActive = false;
            }
        }

        protected override void drawObject(Graphics g)
        {
            g.FillEllipse(ObjectPen.Brush, Left, Top, Width, Height);
        }
    }
}
