using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NETz11sejf
{
    public delegate void EntityEventHandler(GameEntity sender);

    public abstract class GameEntity
    {
        protected static Pen ObjectPen = new Pen(new SolidBrush(Color.Black));
        protected static Pen HealthPen = new Pen(new SolidBrush(Color.Red));
        protected static Pen HealthBarPen = new Pen(new SolidBrush(Color.FromArgb(128, Color.Black)));

        public event EntityEventHandler OnDeath;
        public event EntityEventHandler OnHealthChanged;
        public event EntityEventHandler OnDirectionChanged;

        public float DirectionX { get; private set; }
        public float DirectionY { get; private set; }
        public float Velocity { get; set; }

        private float maxHealth;
        public float MaxHealth
        {
            get { return maxHealth; }
            set { maxHealth = (value < 1) ? 1 : value; }
        }

        private float health;
        public float Health
        {
            get { return health; }
            set
            {
                if (value > health)
                {
                    if (value > maxHealth) value = maxHealth;
                }
                else if (value <= 0)
                {
                    value = 0;
                    IsActive = false;
                    if (OnDeath != null) OnDeath(this);
                }
                health = value;
                if (OnHealthChanged != null) OnHealthChanged(this);
            }
        }

        private float width;
        public float Width
        {
            get { return width; }
            set { width = (value < 0) ? 0 : value; }
        }

        private float height;
        public float Height
        {
            get { return height; }
            set { height = (value < 0) ? 0 : value; }
        }

        public float LocationX { get; set; }
        public float LocationY { get; set; }

        public float Left
        {
            get { return (LocationX - (width * 0.5f)); }
            set { LocationX = value + (width * 0.5f); }
        }

        public float Right
        {
            get { return (LocationX + (width * 0.5f)); }
            set { LocationX = value - (width * 0.5f); }
        }

        public float Top
        {
            get { return (LocationY - (height * 0.5f)); }
            set { LocationY = value + (height * 0.5f); }
        }

        public float Bottom
        {
            get { return (LocationY + (height * 0.5f)); }
            set { LocationY = value - (height * 0.5f); }
        }

        public Color MainColor { get; set; }

        public bool IsActive { get; set; }
        public bool IsMoving { get { return (Velocity != 0); } }
        public bool IsAlive { get { return (health > 0); } }

        public bool CollidesWithScreenTop { get { return (Top < 0); } }
        public bool CollidesWithScreenBottom { get { return (Bottom >= GameResolution.Height); } }
        public bool CollidesWithScreenLeft { get { return (Left < 0); } }
        public bool CollidesWithScreenRight { get { return (Right >= GameResolution.Width); } }
        public bool CollidesWithScreen
        {
            get { return (CollidesWithScreenTop || CollidesWithScreenBottom || CollidesWithScreenLeft || CollidesWithScreenRight); }
        }

        public bool IsOutsideScreenTop { get { return (Bottom < 0); } }
        public bool IsOutsideScreenBottom { get { return (Top >= GameResolution.Height); } }
        public bool IsOutsideScreenLeft { get { return (Right < 0); } }
        public bool IsOutsideScreenRight { get { return (Left >= GameResolution.Width); } }
        public bool IsOutsideScreen
        {
            get { return (IsOutsideScreenTop || IsOutsideScreenBottom || IsOutsideScreenLeft || IsOutsideScreenRight); }
        }

        public void pointInDirectionOf(float tX, float tY)
        {
            float dX = tX - LocationX;
            float dY = tY - LocationY;
            setDirection(dX, dY);
        }

        public void pointInDirectionOf(GameEntity obj)
        {
            pointInDirectionOf(obj.LocationX, obj.LocationY);
        }

        public void pointInRandomDirection(Random rng)
        {
            float x = (float)rng.Next((int)LocationX - 1000, (int)LocationX + 1000);
            float y = (float)rng.Next((int)LocationY - 1000, (int)LocationY + 1000);
            pointInDirectionOf(x, y);
        }

        public void setDirection(float dX, float dY)
        {
            float length = (float)Math.Sqrt((dX * dX) + (dY * dY));

            if (length > 0)
            {
                DirectionX = dX / length;
                DirectionY = dY / length;
            }
            else
            {
                DirectionX = 1;
                DirectionY = 0;
            }
            if (OnDirectionChanged != null) OnDirectionChanged(this);
        }

        public void move()
        {
            LocationX += (Velocity * DirectionX);
            LocationY += (Velocity * DirectionY);
        }

        public void forceInsideScreen()
        {
            if (CollidesWithScreenTop) Top = 0;
            else if (CollidesWithScreenBottom) Bottom = GameResolution.Height - 1;
            if (CollidesWithScreenLeft) Left = 0;
            else if (CollidesWithScreenRight) Right = GameResolution.Width - 1;
        }

        public virtual void update() { }

        public void draw(Graphics g)
        {
            if (IsActive)
            {
                ObjectPen.Color = MainColor;
                drawObject(g);
            }
        }

        protected GameEntity()
        {
            DirectionX = 1;
            DirectionY = 0;
            maxHealth = 1;
            health = 0;
            width = 0;
            height = 0;
            Velocity = 0;
            LocationX = 0;
            LocationY = 0;
            MainColor = Color.Blue;
            IsActive = false;
        }

        protected void drawDefaultHealthBar(Graphics g)
        {
            float top = Bottom + 5;
            float left = Left - 10;
            float w = width + 20;
            float h = 5;

            g.FillRectangle(HealthBarPen.Brush, left, top, w, h);
            
            w *= (health / maxHealth);
            g.FillRectangle(HealthPen.Brush, left, top, w, h);
        }

        protected abstract void drawObject(Graphics g);
    }
}
