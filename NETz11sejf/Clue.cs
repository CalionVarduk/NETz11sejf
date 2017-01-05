using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NETz11sejf
{
    public class Clue : GameEntity
    {
        private byte digitValue;
        private byte digitPosition;

        public event EntityEventHandler OnDigitExtraction;

        public Clue(float size, byte position) : base()
        {
            Health = MaxHealth;
            Width = size;
            Height = size;
            MainColor = Color.GreenYellow;

            digitPosition = (byte)((position > 2) ? 2 : position);
            digitValue = 0;
        }

        public void spawn(float locX, float locY, ushort code)
        {
            byte pow10 = (byte)((digitPosition == 0) ? 1 : (digitPosition == 1) ? 10 : 100);
            digitValue = (byte)((code / pow10) % 10);
            LocationX = locX;
            LocationY = locY;
            IsActive = true;
        }

        public void extractDigit(out byte value, out byte position)
        {
            value = digitValue;
            position = digitPosition;
            IsActive = false;
            if (OnDigitExtraction != null) OnDigitExtraction(this);
        }

        protected override void drawObject(Graphics g)
        {
            ObjectPen.Width = 3;
            g.DrawEllipse(ObjectPen, Left, Top, Width, Height);
        }
    }
}
