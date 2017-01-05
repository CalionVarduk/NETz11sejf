using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NETz11sejf
{
    public class RecoveredCode : GameEntity
    {
        private char[] digits;
        public Font Font { get; set; }

        public RecoveredCode(float fontSize, Color color) : base()
        {
            Health = MaxHealth;
            Font = new Font("Microsoft Sans Serif", fontSize);
            digits = new char[3];
            MainColor = color;
        }

        public void spawn(float locX, float locY)
        {
            LocationX = locX;
            LocationY = locY;
            for (int i = 0; i < digits.Length; ++i) digits[i] = '_';
            IsActive = true;
        }

        public void useClue(Clue clue)
        {
            byte value, position;
            clue.extractDigit(out value, out position);
            position = (byte)(2 - position);
            digits[position] = value.ToString()[0];
        }

        public ushort getCode(Random rng)
        {
            ushort code = (ushort)((digits[2] != '_') ? digits[2] - '0' : rng.Next(0, 10));
            code += (ushort)(((digits[1] != '_') ? digits[1] - '0' : rng.Next(0, 10)) * 10);
            code += (ushort)(((digits[0] != '_') ? digits[0] - '0' : rng.Next(0, 10)) * 100);
            return code;
        }

        protected override void drawObject(Graphics g)
        {
            string code = "Code: " + new string(digits);
            g.DrawString(code, Font, ObjectPen.Brush, LocationX, LocationY);
        }
    }
}
