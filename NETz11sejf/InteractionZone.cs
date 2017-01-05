using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NETz11sejf
{
    public class InteractionZone : GameEntity
    {
        public bool IsEllipse { get; set; }

        public InteractionZone(GameEntity parent, float sizeDiff, bool ellipse) : base()
        {
            if (sizeDiff < 2) sizeDiff = 2;
            Health = MaxHealth;
            Width = parent.Width + sizeDiff;
            Height = parent.Height + sizeDiff;
            MainColor = Color.ForestGreen;
            IsEllipse = ellipse;
        }

        public void spawn(GameEntity parent)
        {
            LocationX = parent.LocationX;
            LocationY = parent.LocationY;
            IsActive = true;
        }

        protected override void drawObject(Graphics g)
        {
            ObjectPen.Width = 1;
            if (IsEllipse) g.DrawEllipse(ObjectPen, Left, Top, Width, Height);
            else g.DrawRectangle(ObjectPen, Left, Top, Width, Height);
        }
    }
}
