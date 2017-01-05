using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NETz11sejf
{
    public class Information : GameEntity
    {
        public static StringFormat StringFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

        public string Text { get; set; }
        public Font Font { get; set; }
    
        public Information(float fontSize, Color color) : base()
        {
            Health = MaxHealth;
            Font = new Font("Microsoft Sans Serif", fontSize);
            Text = "";
            MainColor = color;
        }

        public void spawn(float locX, float locY)
        {
            LocationX = locX;
            LocationY = locY;
            IsActive = true;
        }

        protected override void drawObject(Graphics g)
        {
            g.DrawString(Text, Font, ObjectPen.Brush, LocationX, LocationY, StringFormat);
        }
    }
}
