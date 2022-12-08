using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;

namespace MineGame
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;

        private Texture2D backgroundTexture;
        private Texture2D planeTexture;

        private Texture2D coinTexture;
        private Vector2 coinPos;
        private float difficulty;
        private string difficultyText;

        private Texture2D regMineTexture;
        private Texture2D advMineTexture;
        private Texture2D statMineTexture;
        private Texture2D sideMineTexture;
        private Texture2D sideMine2Texture;
        private Texture2D upMineTexture;
        private Texture2D blockTexture;
        private Texture2D bounceTexture;

        private Texture2D easyTexture;
        private Texture2D normalTexture;
        private Texture2D hardTexture;

        private Vector2 startPos;
        private Vector2 velocityPlayer;

        private List<MovingObject> mines;
        private List<BigBlock> blockList;
        private List<Button> buttonList;

        private float regMineTimer;
        private float advMineTimer;
        private float statMineTimer;
        private int gameScreenWidth;
        private int gameScreenHeight;

        private Random rnd;
        private bool isPlaying;
        private SpriteFont font;
        private float score;
        private float highscore;
        private float easyHighscore;
        private float normalHighscore;
        private float hardHighscore;
        private double rotation;

        private Player player;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            isPlaying = false;
            startPos = new Vector2(400, 400);
            velocityPlayer = Vector2.Zero;

            mines = new List<MovingObject>();
            blockList = new List<BigBlock>();
            buttonList = new List<Button>();
            rnd = new Random();

            gameScreenWidth = Window.ClientBounds.Width;
            gameScreenHeight = Window.ClientBounds.Height;

            score = 0;
            highscore = 0;
            easyHighscore = 0;
            normalHighscore = 0;
            hardHighscore = 0;
            difficulty = 1;
            difficultyText = "Easy";
            coinPos = new Vector2(-50, -50);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("font");

            coinTexture = Content.Load<Texture2D>("coin");
            planeTexture = Content.Load<Texture2D>("newFantasticPlane");
            regMineTexture = Content.Load<Texture2D>("regularMine");
            advMineTexture = Content.Load<Texture2D>("advancedMine");
            statMineTexture = Content.Load<Texture2D>("staticMine");
            sideMineTexture = Content.Load<Texture2D>("sideMine");
            sideMine2Texture = Content.Load<Texture2D>("sideMine2");
            upMineTexture = Content.Load<Texture2D>("upMine");
            bounceTexture = Content.Load<Texture2D>("bounceMine");
            blockTexture = Content.Load<Texture2D>("bigBlock");
            backgroundTexture = Content.Load<Texture2D>("background");

            easyTexture = Content.Load<Texture2D>("Easy");
            normalTexture = Content.Load<Texture2D>("Normal");
            hardTexture = Content.Load<Texture2D>("Hard");

            player = new Player(planeTexture, startPos);
            buttonList.Add(new Button(new Vector2(50, 150), easyTexture, 1));
            buttonList.Add(new Button(new Vector2(50, 200), normalTexture, 2));
            buttonList.Add(new Button(new Vector2(50, 250), hardTexture, 3));
        }
        // -----------------------------UPDATE------------------------------
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit(); //Stänger av spelet när man trycker på Escape

            KeyboardState state = Keyboard.GetState();
            if (!isPlaying)
            {

                // ----------------BUTTON COLLISION
                foreach (Button button in buttonList)
                {
                    if (button.MouseOnButton(Mouse.GetState()))
                    {
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                        {
                            difficulty = button.Difficulty;
                            switch (difficulty)
                            {
                                case 1:
                                    difficultyText = "Easy";
                                    highscore = easyHighscore;
                                    break;
                                case 2:
                                    difficultyText = "Normal";
                                    highscore = normalHighscore;
                                    break;
                                case 3:
                                    difficultyText = "Hard";
                                    highscore = hardHighscore;
                                    break;
                            }
                        }
                    }
                }

                // ----------------Restart;
                if (state.IsKeyDown(Keys.Tab)) Reset();
                else return;
            }

            // ----------------MOVEMENT
            rotation = player.PlayerUpdate(state);

            regMineTimer--;
            advMineTimer--;
            statMineTimer--;

            // ----------------COIN
            Rectangle playerBox = new((int)player.GetPosX(), (int)player.GetPosY(), player.Texture.Width, player.Texture.Height);
            Rectangle coinpos = new ((int)coinPos.X, (int)coinPos.Y, coinTexture.Width, coinTexture.Height);
            var kollision = Intersection(playerBox, coinpos);
            if (kollision.Width > 0 && kollision.Height > 0) //Kollar om spelaren kolliderar med pengen
            {
                Rectangle r1 = Normalize(playerBox, kollision);
                Rectangle r2 = Normalize(coinpos, kollision);
                if (TestCollision(planeTexture, r1, coinTexture, r2)) Newcoin();
            }

            // ----------------CREATE MINES & BLOCK
            if (regMineTimer <= 0)
            {
                regMineTimer = 180 / difficulty - score / 10; //bestämmer hur lång tid det ska ta innan nästa mina skapas baserat på svårighetsgraden och spelarens poäng

                int startPosX = rnd.Next(gameScreenWidth - regMineTexture.Width);
                int startPosY = rnd.Next(gameScreenHeight - regMineTexture.Height);
                mines.Add(new RegularMine(regMineTexture, new Vector2(startPosX, -32)));
                if (score >= 3) mines.Add(new SideMine2(sideMine2Texture, new Vector2(-32, startPosY)));
                if (score >= 9) mines.Add(new SideMine(sideMineTexture, new Vector2(gameScreenWidth + 32, startPosY)));
                if (score >= 12) mines.Add(new UpMine(upMineTexture, new Vector2(startPosX, gameScreenHeight + 32)));
            }
            if (advMineTimer <= 0)
            {
                advMineTimer = 1200 / difficulty - score / 10; //bestämmer hur lång tid det ska ta innan nästa mina skapas baserat på svårighetsgraden och spelarens poäng

                int startPosX = rnd.Next(gameScreenWidth - advMineTexture.Width);
                int startPosY = rnd.Next(gameScreenHeight - blockTexture.Height);
                float rotation = rnd.Next(4) * 90;
                if (score >= 4) 
                { 
                    switch (rotation) // Skapar avancerad mina med hastighet åt rätt håll
                    {
                        case 0:
                            mines.Add(new AdvancedMine(advMineTexture, new Vector2(startPosX, -32), rotation));
                            break;
                        case 90:
                            mines.Add(new AdvancedMine(advMineTexture, new Vector2(gameScreenWidth + 32, startPosY), rotation));
                            break;
                        case 180:
                            mines.Add(new AdvancedMine(advMineTexture, new Vector2(startPosX, gameScreenHeight + 32), rotation));
                            break;
                        case 270:
                            mines.Add(new AdvancedMine(advMineTexture, new Vector2(-32, startPosY), rotation));
                            break;
                    }
                }
                startPosX = rnd.Next(gameScreenWidth - blockTexture.Width);
                if (rnd.Next(0,2) <= 0 && score >= 16)
                {
                    if (rnd.Next(0,2) == 0)
                    {
                        blockList.Add(new BigBlock(blockTexture, new Vector2(startPosX, -128), new Vector2(0,1)));
                    }
                    else blockList.Add(new BigBlock(blockTexture, new Vector2(startPosX, gameScreenHeight + 128), new Vector2(0, -1)));
                }
                else if (score >= 16)
                {
                    if (rnd.Next(0,2) == 0)
                    {
                        blockList.Add(new BigBlock(blockTexture, new Vector2(-128, startPosY), new Vector2(1, 0)));
                    }
                    else blockList.Add(new BigBlock(blockTexture, new Vector2(gameScreenWidth + 128, startPosY), new Vector2(-1, 0)));
                }
            }
            if (statMineTimer <= 0)
            {
                statMineTimer = 360 / difficulty - score / 10; //bestämmer hur lång tid det ska ta innan nästa mina skapas baserat på svårighetsgraden och spelarens poäng

                int startPosX = rnd.Next(gameScreenWidth - statMineTexture.Width);
                int startPosY = rnd.Next(gameScreenHeight - statMineTexture.Height);
                if (score >= 7) mines.Add(new StatMine(statMineTexture, new Vector2(startPosX, startPosY), 75));
            }

            // ----------------UPDATE MINES & BLOCK
            foreach (MovingObject mine in mines)
            {
                if (mine is RegularMine)
                {
                    mine.Update(new Vector2(0, 1) * difficulty / 2 + new Vector2(0, 0.5f)); //Rör minan olika fort beroende på svårighetsgraden
                }
                else if (mine is UpMine)
                {
                    mine.Update(new Vector2(0, -1) * difficulty / 2 - new Vector2(0, 0.5f));
                }
                else if (mine is SideMine)
                {
                    mine.Update(new Vector2(-1, 0) * difficulty / 2 - new Vector2(0.5f, 0));
                }
                else if (mine is SideMine2)
                {
                    mine.Update(new Vector2(1, 0) * difficulty / 2 + new Vector2(0.5f, 0));
                }
                else if (mine is StatMine)
                {
                    StatMine statmine = mine as StatMine;
                    statmine.Timer -= 1;

                }
                else if (mine is AdvancedMine)
                {
                    AdvancedMine advmine = mine as AdvancedMine;
                    advmine.AdvancedUpdate(mine, player.GetPosX(), player.GetPosY(), difficulty);
                }
                else if (mine is BounceMine)
                {
                    BounceMine bounce = mine as BounceMine;
                    bounce.BounceUpdate(gameScreenWidth, gameScreenHeight);
                }
            }

            // ----------------REMOVE MINES & BLOCK
            for (int i = 0; i < mines.Count; i++)
            {
                if (mines[i].GetPosY() > gameScreenHeight + 32 || mines[i].GetPosX() > gameScreenWidth + 32 || mines[i].GetPosX() < -32 || mines[i].GetPosY() < -32) //Tar bort minor som är långt utanför spelområdet
                {
                    mines.RemoveAt(i);
                }
                else if (mines[i] is StatMine)
                {
                    StatMine statmine = mines[i] as StatMine;

                    if (statmine.Timer < -200 * difficulty)
                    {
                        mines.RemoveAt(i);
                    }
                }
            }
            foreach (BigBlock block in blockList) block.Update(block.Velocity);

            // ----------------MINES COLLISION
            foreach (MovingObject mine in mines)
            {
                if (Vector2.Distance(new Vector2(player.GetPosX(), player.GetPosY()), new Vector2(mine.GetPosX(), mine.GetPosY())) > 100) continue; //skippar kollisionshantering med minor som är långt ifrån spelaren
                if (mine is StatMine)
                {
                    StatMine statmine = mine as StatMine;
                    if (statmine.Timer >= 0) continue; //skippar statiska minor med en timer över 0;
                }

                Rectangle minepos = new ((int)mine.GetPosX(), (int)mine.GetPosY(), mine.Texture.Width, mine.Texture.Height);
                kollision = Intersection(playerBox, minepos);
                if (kollision.Width > 0 && kollision.Height > 0)
                {
                    Rectangle r1 = Normalize(playerBox, kollision);
                    Rectangle r2 = Normalize(minepos, kollision);
                    isPlaying = !TestCollision(planeTexture, r1, mine.Texture, r2);
                    if (!isPlaying && score > highscore) 
                    {
                        highscore = score;
                        if (difficulty == 1) easyHighscore = score;
                        else if (difficulty == 2) normalHighscore = score;
                        else hardHighscore = score;
                    }
                }
            }

            // ----------------BOX COLLISION
            foreach (BigBlock block in blockList)
            {
                Rectangle blockpos = new ((int)block.GetPosX(), (int)block.GetPosY(), block.Texture.Width, block.Texture.Height);
                kollision = Intersection(playerBox, blockpos);
                if (kollision.Width > 0 && kollision.Height > 0)
                {
                    if (playerBox.Left < kollision.Right)
                    {
                        blockpos = new ((int)block.GetPosX() + block.Texture.Width, (int)block.GetPosY(), block.Texture.Width, block.Texture.Height);
                        kollision = Intersection(playerBox, blockpos);
                        //kollar om spelarens position är höger om blocket
                        if (kollision.Width > 0 && kollision.Height > 0)
                        {
                            player.Update(new Vector2(3 - velocityPlayer.X, 0)); //flyttar spelaren åt höger
                        }
                    }
                    if (playerBox.Right > kollision.Left)
                    {
                        blockpos = new Rectangle((int)block.GetPosX() - block.Texture.Width, (int)block.GetPosY(), block.Texture.Width, block.Texture.Height);
                        kollision = Intersection(playerBox, blockpos);
                        //kollar om spelarens position är vänster om blocket
                        if (kollision.Width > 0 && kollision.Height > 0)
                        {
                            player.Update(new Vector2(-3 - velocityPlayer.X, 0)); //flyttar spelaren åt vänster
                        }
                    }
                    if (playerBox.Top > kollision.Bottom)
                    {
                        blockpos = new Rectangle((int)block.GetPosX(), (int)block.GetPosY() + block.Texture.Height, block.Texture.Width, block.Texture.Height);
                        kollision = Intersection(playerBox, blockpos);
                        //kollar om spelarens position är under blocket
                        if (kollision.Width > 0 && kollision.Height > 0)
                        {
                            player.Update(new Vector2(0, 3 - velocityPlayer.Y)); //flyttar spelaren neråt
                        }
                    }
                    if (playerBox.Bottom > kollision.Top)
                    {
                        blockpos = new Rectangle((int)block.GetPosX(), (int)block.GetPosY() - block.Texture.Height, block.Texture.Width, block.Texture.Height);
                        kollision = Intersection(playerBox, blockpos);
                        //kollar om spelarens position är över blocket
                        if (kollision.Width > 0 && kollision.Height > 0)
                        {
                            player.Update(new Vector2(0, -3 - velocityPlayer.Y)); //flyttar spelaren uppåt
                        }
                    }
                }
                
            }

            if (player.GetPosX() > gameScreenWidth - planeTexture.Width || player.GetPosX() < 0 || player.GetPosY() > gameScreenHeight - planeTexture.Height || player.GetPosY() < 0) 
            {
                isPlaying = false;
                if (score > highscore)
                {
                    highscore = score;
                    if (difficulty == 1) easyHighscore = score;
                    else if (difficulty == 2) normalHighscore = score;
                    else hardHighscore = score;
                }
            }

            base.Update(gameTime);
        }

        // ------------------------------DRAW-------------------------------
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkKhaki);
            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);

            foreach (BigBlock block in blockList) block.Draw(spriteBatch, Color.White);
            spriteBatch.Draw(planeTexture, new Rectangle((int)player.GetPosX() + planeTexture.Width / 2, (int)player.GetPosY() + planeTexture.Width / 2, planeTexture.Width, planeTexture.Height), null, Color.White, (float)rotation, new Vector2(planeTexture.Width / 2, planeTexture.Height / 2), SpriteEffects.None, 0f);
            //Ritar planet med rotation
            foreach (MovingObject mine in mines)
            { 
                if (mine is StatMine)
                {
                    StatMine statmine = mine as StatMine;
                    if (statmine.Timer < 1) mine.Draw(spriteBatch, Color.Black);
                    else mine.Draw(spriteBatch, Color.White);
                }
                else mine.Draw(spriteBatch, Color.White);
            }

            if (!isPlaying) foreach (Button button in buttonList) button.Draw(spriteBatch);
            spriteBatch.DrawString(font, score.ToString(), new Vector2(50, 50), Color.Yellow);
            spriteBatch.Draw(coinTexture, coinPos, Color.White);
            if (!isPlaying) 
            {
                spriteBatch.DrawString(font, "Current Difficulty: " + difficultyText, new Vector2(198, 52), Color.Black);
                spriteBatch.DrawString(font, "Current Difficulty: " + difficultyText, new Vector2(200, 50), Color.Red);
                spriteBatch.DrawString(font, "Press Tab to start!", new Vector2(250, 100), Color.White);
                spriteBatch.DrawString(font, "Highscore: " + highscore, new Vector2(300, 200), Color.White);

            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        // ------------------------------RESET------------------------------
        void Reset()
        {
            isPlaying = true;
            mines.Clear();
            blockList.Clear();
            player.Position = startPos;
            Newcoin();
            score = 0;
            regMineTimer = 1;
            advMineTimer = 1;
            statMineTimer = 1;
        }

        // -----------------------------NEWCOIN-----------------------------
        void Newcoin()
        {
            score++;
            if (score % 25 == 0)
            {
                float X = 0;
                while (X == 0)
                {
                    X = (float)rnd.NextDouble() - 0.5f; //Bestämmer X-värdet för den studsande minans Velocity
                }
                float Y = 0;
                while (Y == 0)
                {
                    Y = (float)rnd.NextDouble() - 0.5f; //Bestämmer Y-värdet för den studsande minans Velocity
                }
                mines.Add(new BounceMine(bounceTexture, new Vector2(0, gameScreenHeight - bounceTexture.Height), Vector2.Normalize(new Vector2(X, Y)) * (difficulty + 1))); //Skapar minan längst ner till vänster med Velocity en normalizerd vektor av X och Y som multipliceras med svårighetsgraden 
            }
            int startPosX = rnd.Next(gameScreenWidth - coinTexture.Width);
            int startPosY = rnd.Next(gameScreenHeight - coinTexture.Height);
            coinPos = new Vector2(startPosX, startPosY); //ändrar pengens position till en slumpmässig plats på skärmen
        }

        // ----------------------------COLLISION----------------------------
        public static Rectangle Intersection(Rectangle r1, Rectangle r2) //Jag har ingen aning om vad som pågår
        {
            int x1 = Math.Max(r1.Left, r2.Left);
            int y1 = Math.Max(r1.Top, r2.Top);
            int x2 = Math.Min(r1.Right, r2.Right);
            int y2 = Math.Min(r1.Bottom, r2.Bottom);

            if ((x2 >= x1) && (y2 >= y1))
            {
                return new Rectangle(x1, y1, x2 - x1, y2 - y1);
            }
            return Rectangle.Empty;
        }

        public static Rectangle Normalize(Rectangle reference, Rectangle overlap) //Jag har ingen aning om vad som pågår
        {
            return new Rectangle(overlap.X - reference.X, overlap.Y - reference.Y, overlap.Width, overlap.Height);
        }

        public static bool TestCollision(Texture2D t1, Rectangle r1, Texture2D t2, Rectangle r2) //Jag har ingen aning om vad som pågår
        {
            int pixelCount = r1.Width * r1.Height;
            uint[] texture1Pixels = new uint[pixelCount];
            uint[] texture2Pixels = new uint[pixelCount];

            t1.GetData(0, r1, texture1Pixels, 0, pixelCount);
            t2.GetData(0, r2, texture2Pixels, 0, pixelCount);

            for (int i = 0; i < pixelCount; ++i)
            {
                if (((texture1Pixels[i] & 0xff000000) > 0) && ((texture2Pixels[i] & 0xff000000) > 0))
                {
                    return true;
                }
            }
            return false;
        }
    }
}