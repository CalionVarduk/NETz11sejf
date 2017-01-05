using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NETz11sejf
{
    public delegate void SafeCrackingHandler(Safe sender, GameEntity offender);

    public class Safe : GameEntity
    {
        private ushort code;
        private int interactionCooldown;
        public bool IsInteractible { get { return (interactionCooldown == 0); } }

        public InteractionZone InteractionZone { get; private set; }

        public Clue FirstClue { get; private set; }
        public Clue SecondClue { get; private set; }
        public Clue ThirdClue { get; private set; }

        public event SafeCrackingHandler OnFailedCrack;
        public event SafeCrackingHandler OnSuccessfulCrack;

        public Safe(float width, float height, Color color) : base()
        {
            code = 0;
            interactionCooldown = 0;
            Health = MaxHealth;
            Width = width;
            Height = height;
            MainColor = color;
            InteractionZone = new InteractionZone(this, 30, false);
            FirstClue = new Clue(13, 0);
            SecondClue = new Clue(13, 1);
            ThirdClue = new Clue(13, 2);
        }

        public void spawn(float locX, float locY, Random rng)
        {
            code = (ushort)rng.Next(0, 1000);
            LocationX = locX;
            LocationY = locY;
            InteractionZone.spawn(this);

            float x = (float)rng.Next(20, GameResolution.Width - 20);
            float y = (float)rng.Next(20, GameResolution.Height - 20);
            FirstClue.spawn(x, y, code);

            x = (float)rng.Next(20, GameResolution.Width - 20);
            y = (float)rng.Next(20, GameResolution.Height - 20);
            SecondClue.spawn(x, y, code);

            x = (float)rng.Next(20, GameResolution.Width - 20);
            y = (float)rng.Next(20, GameResolution.Height - 20);
            ThirdClue.spawn(x, y, code);

            IsActive = true;
        }

        public void tryToCrack(ushort code, GameEntity cracker)
        {
            if (interactionCooldown == 0)
            {
                if (this.code == code)
                {
                    if (OnSuccessfulCrack != null) OnSuccessfulCrack(this, cracker);
                }
                else
                {
                    interactionCooldown = 600;
                    InteractionZone.MainColor = Color.Red;
                    if (OnFailedCrack != null) OnFailedCrack(this, cracker);
                }
            }
        }

        public override void update()
        {
            if (interactionCooldown > 0)
            {
                if (--interactionCooldown == 0) InteractionZone.MainColor = Color.ForestGreen;
            }
        }

        protected override void drawObject(Graphics g)
        {
            g.FillRectangle(ObjectPen.Brush, Left, Top, Width, Height);
            InteractionZone.draw(g);
            FirstClue.draw(g);
            SecondClue.draw(g);
            ThirdClue.draw(g);
        }
    }
}
