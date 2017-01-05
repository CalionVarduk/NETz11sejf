using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NETz11sejf
{
    public class Guard : GameEntity
    {
        public static float ShotInterval = 30;
        public static Font AsleepFont = new Font("Microsoft Sans Serif", 8);

        private int shotCooldown;
        private int currentBullet;
        private Bullet[] bullets;
        
        private int asleepCooldown;
        public bool IsAsleep { get { return (asleepCooldown > 0); } }

        private bool clearTargetPending;
        public GameEntity Target { get; private set; }
        public InteractionZone InteractionZone { get; private set; }

        private float sightArc;
        public float SightArc
        {
            get { return sightArc; }
            set
            {
                sightArc = value % 360;
                if (sightArc < 0) sightArc += 360;
            }
        }

        private float sightRange;
        public float SightRange
        {
            get { return sightRange; }
            set { sightRange = (value > 0) ? value : 0; }
        }

        public event EntityEventHandler OnStunned;
        public event EntityEventHandler OnTargetAquired;
        public event EntityEventHandler OnTargetLost;

        public Guard(float size, float sightArc, float sightRange, Color color, Safe safe) : base()
        {
            shotCooldown = 0;
            currentBullet = 0;
            int bulletCount = 100;
            bullets = new Bullet[bulletCount];
            for (int i = 0; i < bulletCount; ++i)
                bullets[i] = new Bullet(5, Color.OrangeRed);

            SightArc = sightArc;
            SightRange = sightRange;
            clearTargetPending = false;
            asleepCooldown = 0;
            Health = MaxHealth;
            Width = size;
            Height = size;
            MainColor = color;
            InteractionZone = new InteractionZone(this, 10, true);
            safe.OnFailedCrack += reactToFailedCrackAttempt;
        }

        public void spawn(float locX, float locY, Random rng)
        {
            clearTargetPending = false;
            asleepCooldown = 0;
            shotCooldown = 0;
            LocationX = locX;
            LocationY = locY;
            InteractionZone.spawn(this);
            IsActive = true;
            Target = null;
            pointInRandomDirection(rng);
            foreach (Bullet b in bullets) b.IsActive = false;
        }

        public void stun()
        {
            asleepCooldown = 900;
            clearTarget();
            if (OnStunned != null) OnStunned(this);
        }

        public void clearTarget()
        {
            if (Target != null)
            {
                clearTargetPending = true;
                Velocity = 0;
                if (OnTargetLost != null) OnTargetLost(this);
            }
        }

        public void deactivateBullets()
        {
            foreach (Bullet b in bullets) b.IsActive = false;
        }

        public bool isTargeting(GameEntity entity)
        {
            if (clearTargetPending) return false;
            return (Target == entity);
        }

        public bool canSeeEntity(GameEntity entity)
        {
            if (IsActive && entity.IsActive)
            {
                float px = entity.LocationX;
                float py = entity.LocationY;

                if (Collider.Circles(LocationX, LocationY, sightRange, px, py, entity.Width * 0.5f))
                {
                    float halfArc = sightArc * 0.5f;
                    float x = DirectionX;
                    float y = DirectionY;
                    Rotator.Point(-halfArc, ref x, ref y);
                    x = LocationX + (x * sightRange);
                    y = LocationY + (y * sightRange);

                    if(Collider.Orientation(px, py, LocationX, LocationY, x, y) >= 0)
                    {
                        x = DirectionX;
                        y = DirectionY;
                        Rotator.Point(halfArc, ref x, ref y);
                        x = LocationX + (x * sightRange);
                        y = LocationY + (y * sightRange);
                        return (Collider.Orientation(px, py, x, y, LocationX, LocationY) >= 0);
                    }
                }
            }
            return false;
        }

        public bool handleVisualTarget(GameEntity potentialTarget)
        {
            if(canSeeEntity(potentialTarget))
            {
                if (Target != potentialTarget)
                {
                    Target = potentialTarget;
                    Velocity = 0.8f;
                    pointInDirectionOf(Target);
                    if (OnTargetAquired != null) OnTargetAquired(this);
                }
                return true;
            }
            if (Target != null)
            {
                Player player = Target as Player;
                if (player == null || !player.IsPursued) clearTarget();
            }
            return false;
        }

        public override void update()
        {
            if (IsActive)
            {
                foreach (Bullet b in bullets) b.update();
                if (!IsAsleep) updateNotAsleep();
                else updateAsleep();
                collideBulletsWithTarget();
            }
        }

        private void updateAsleep()
        {
            --asleepCooldown;
        }

        private void updateNotAsleep()
        {
            if (!clearTargetPending && Target != null) updateWithTarget();
            else
            {
                float x = DirectionX;
                float y = DirectionY;
                Rotator.Point(0.3f, ref x, ref y);
                setDirection(x, y);
            }
        }

        private void updateWithTarget()
        {
            if (Target.IsAlive)
            {
                if (Target.IsMoving) pointInDirectionOf(Target);
                move();
                forceInsideScreen();
                InteractionZone.LocationX = LocationX;
                InteractionZone.LocationY = LocationY;
                tryShoot();
            }
            else clearTarget();
        }

        private void tryShoot()
        {
            if (++shotCooldown >= ShotInterval)
            {
                bullets[currentBullet].spawn(LocationX, LocationY, Target);
                if (++currentBullet >= bullets.Length) currentBullet = 0;
                shotCooldown = 0;
            }
        }

        private void collideBulletsWithTarget()
        {
            if (Target != null)
            {
                int activeBullets = 0;

                foreach (Bullet b in bullets)
                {
                    if (b.IsActive)
                    {
                        ++activeBullets;
                        if (Target.IsAlive && Collider.AsCircles(Target, b))
                        {
                            Target.Health -= Bullet.BulletDamage;
                            b.IsActive = false;
                        }
                    }
                }

                if (clearTargetPending && activeBullets == 0)
                {
                    clearTargetPending = false;
                    Target = null;
                }
            }
        }

        private void reactToFailedCrackAttempt(Safe sender, GameEntity offender)
        {
            Player player = offender as Player;
            if (player != null) player.IsPursued = true;

            asleepCooldown = 0;
            Target = offender;
            Velocity = 0.35f;
            pointInDirectionOf(Target);
            if (OnTargetAquired != null) OnTargetAquired(this);
        }

        protected override void drawObject(Graphics g)
        {
            g.FillEllipse(ObjectPen.Brush, Left, Top, Width, Height);
            InteractionZone.draw(g);

            if (IsAsleep)
            {
                ObjectPen.Color = Color.Black;
                g.DrawString("zzz...", AsleepFont, ObjectPen.Brush, Left, Top - 12);
            }
            else
            {
                ObjectPen.Color = Color.FromArgb((int)(0.11f * MainColor.A), MainColor);
                float x = LocationX - sightRange;
                float y = LocationY - sightRange;
                float d = sightRange + sightRange;

                float angle = (float)Math.Atan2(DirectionY, DirectionX) * 57.29578f;
                g.FillPie(ObjectPen.Brush, x, y, d, d, angle - (sightArc * 0.5f), sightArc);
            }

            foreach (Bullet b in bullets) b.draw(g);
        }
    }
}
