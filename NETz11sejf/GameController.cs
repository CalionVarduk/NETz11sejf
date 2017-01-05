using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace NETz11sejf
{
    public delegate void GameProcedureHandler();

    public static class GameController
    {
        private static Information RestartGame;
        private static Information EndGameMessage;
        private static Information SubMessage;
        private static Information AuthorInfo;

        private static RecoveredCode recoveredCode;
        private static Player player;
        private static Safe safe;
        private static List<Guard> guards;
        private static Random rng;
        private static Stopwatch watch;

        public static event GameProcedureHandler OnEscape;

        static GameController()
        {
            rng = new Random();
            watch = new Stopwatch();

            recoveredCode = new RecoveredCode(13, Color.Black);
            player = new Player(100, 20, Color.Blue);
            safe = new Safe(40, 24, Color.Gold);
            guards = new List<Guard>();

            EndGameMessage = new Information(15, Color.Black);
            SubMessage = new Information(12, Color.Black);
            RestartGame = new Information(12, Color.Black);
            AuthorInfo = new Information(10, Color.Black);

            EndGameMessage.spawn(GameResolution.Width * 0.5f, GameResolution.Height * 0.5f - 20);
            SubMessage.spawn(GameResolution.Width * 0.5f, GameResolution.Height * 0.5f + 10);
            RestartGame.spawn(GameResolution.Width * 0.5f, GameResolution.Height * 0.5f + 35);
            AuthorInfo.spawn(GameResolution.Width * 0.5f, GameResolution.Height - 20);
            EndGameMessage.IsActive = false;
            RestartGame.IsActive = false;
            SubMessage.Text = "Press any '1' to '9' key to choose the amount of guards";
            RestartGame.Text = "Press 'R' to restart the game";
            AuthorInfo.Text = "Author: Łukasz Furlepa" + Environment.NewLine + "University of Silesia (2016)";

            player.OnReturnedToSafeZone += playerReturnedToSafeZone;
            player.OnDeath += playerDeath;
            safe.OnSuccessfulCrack += playerVictory;
            PlayerControls.KeyPressed += keyPressed;
            PlayerControls.player = player;
        }

        public static void resetGame(int guardCount)
        {
            safe.spawn(GameResolution.Width * 0.5f, 30, rng);
            player.spawn(50, GameResolution.Height - 50);
            recoveredCode.spawn(10, 10);

            guards.Clear();
            guards.TrimExcess();

            for (int i = 0; i < guardCount; ++i)
            {
                guards.Add(new Guard(20, 65, 350, Color.Red, safe));
                float x = (float)rng.Next(20, GameResolution.Width - 20);
                float y = (float)rng.Next(20, GameResolution.Height - 20);
                guards[i].spawn(x, y, rng);
            }

            AuthorInfo.IsActive = false;
            SubMessage.IsActive = false;
            watch.Restart();
        }

        public static void drawGame(Graphics g)
        {
            safe.draw(g);
            
            int count = guards.Count;
            for (int i = 0; i < count; ++i)
                guards[i].draw(g);

            player.draw(g);
            recoveredCode.draw(g);

            EndGameMessage.draw(g);
            SubMessage.draw(g);
            RestartGame.draw(g);
            AuthorInfo.draw(g);
        }

        public static void updateGame()
        {
            safe.update();
            
            int count = guards.Count;
            for (int i = 0; i < count; ++i)
                guards[i].update();

            if (!SubMessage.IsActive)
            {
                if (!player.IsInSafeZone)
                {
                    for (int i = 0; i < count; ++i)
                        guards[i].handleVisualTarget(player);
                }
                else for (int i = 0; i < count; ++i)
                    guards[i].clearTarget();
            }

            player.update();
        }

        private static void keyPressed(Keys key)
        {
            if (key == Keys.Escape)
            {
                if (OnEscape != null) OnEscape();
            }
            else if (RestartGame.IsActive)
            {
                if (key == Keys.R)
                {
                    SubMessage.Text = "Press any '1' to '9' key to choose the amount of guards";
                    EndGameMessage.IsActive = false;
                    SubMessage.IsActive = true;
                    RestartGame.IsActive = false;
                    player.IsActive = false;
                    safe.IsActive = false;
                    recoveredCode.IsActive = false;
                    foreach (Guard g in guards) g.IsActive = false;
                }
            }
            else if (SubMessage.IsActive)
            {
                switch(key)
                {
                    case Keys.D1:
                    case Keys.NumPad1: resetGame(1); break;
                    case Keys.D2:
                    case Keys.NumPad2: resetGame(2); break;
                    case Keys.D3:
                    case Keys.NumPad3: resetGame(3); break;
                    case Keys.D4:
                    case Keys.NumPad4: resetGame(4); break;
                    case Keys.D5:
                    case Keys.NumPad5: resetGame(5); break;
                    case Keys.D6:
                    case Keys.NumPad6: resetGame(6); break;
                    case Keys.D7:
                    case Keys.NumPad7: resetGame(7); break;
                    case Keys.D8:
                    case Keys.NumPad8: resetGame(8); break;
                    case Keys.D9:
                    case Keys.NumPad9: resetGame(9); break;
                }
            }
            else if (key == Keys.Space) checkInteractionZones();
        }

        private static void checkInteractionZones()
        {
            if (safe.IsInteractible && !isAnyGuardAlarmed() && Collider.AsRectangles(safe.InteractionZone, player))
            {
                safe.tryToCrack(recoveredCode.getCode(rng), player);
            }
            else if (safe.FirstClue.IsActive && Collider.AsCircles(safe.FirstClue, player))
            {
                recoveredCode.useClue(safe.FirstClue);
            }
            else if (safe.SecondClue.IsActive && Collider.AsCircles(safe.SecondClue, player))
            {
                recoveredCode.useClue(safe.SecondClue);
            }
            else if (safe.ThirdClue.IsActive && Collider.AsCircles(safe.ThirdClue, player))
            {
                recoveredCode.useClue(safe.ThirdClue);
            }
            else
            {
                foreach (Guard g in guards)
                    if (Collider.AsCircles(g.InteractionZone, player))
                        g.stun();
            }
        }

        private static bool isAnyGuardAlarmed()
        {
            foreach (Guard g in guards)
                if (g.isTargeting(player))
                    return true;
            return false;
        }

        private static void playerReturnedToSafeZone(GameEntity sender)
        {
            int count = guards.Count;
            for (int i = 0; i < count; ++i)
            {
                guards[i].clearTarget();
                guards[i].pointInRandomDirection(rng);
            }
        }

        private static void playerDeath(GameEntity sender)
        {
            setEndGameInfo(false);
        }

        private static void playerVictory(Safe sender, GameEntity offender)
        {
            player.IsPursued = false;

            foreach (Guard g in guards)
            {
                g.deactivateBullets();
                g.clearTarget();
            }

            setEndGameInfo(true);
        }

        private static void setEndGameInfo(bool won)
        {
            EndGameMessage.Text = "Game over. You have " + ((won) ? "won!" : "lost!");
            SubMessage.Text = "The game took " + (watch.ElapsedMilliseconds / 1000f).ToString("F2") + " s";
            EndGameMessage.IsActive = true;
            SubMessage.IsActive = true;
            RestartGame.IsActive = true;
            AuthorInfo.IsActive = true;
        }
    }
}
