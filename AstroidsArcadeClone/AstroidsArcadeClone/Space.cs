using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace AstroidsArcadeClone
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Space : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private static Random r = new Random();
        private static List<SpriteObject> objects = new List<SpriteObject>();
        private static List<SpriteObject> removeObjects = new List<SpriteObject>();
        private static List<SpriteObject> addObjects = new List<SpriteObject>();
        private static ContentManager contentMan;
        private static GameWindow gamewindow;
        private static int score = 0;
        private SpriteFont sf;
        private float timer = 25;
        private Keys[] lastPressedKeys;
        private string playerName = string.Empty;
        private bool enterNameTime = true;
        private bool drawHighscore;

        private static SQLiteConnection dbcon = new SQLiteConnection("Data Source = highscore.db;Version=3");
        private static SQLiteCommand command = new SQLiteCommand("", dbcon);

        public static int Score
        {
            get { return score; }
            set { score = value; }
        }
        public static GameWindow Gamewindow
        {
            get { return gamewindow; }
            set { gamewindow = value; }
        }
        public static ContentManager ContentMan
        {
            get { return contentMan; }
            set { contentMan = value; }
        }
        public static List<SpriteObject> AddObjects
        {
            get { return addObjects; }
            set { addObjects = value; }
        }
        public static List<SpriteObject> Objects
        {
            get { return objects; }
            set { objects = value; }
        }
        public static List<SpriteObject> RemoveObjects
        {
            get { return removeObjects; }
            set { removeObjects = value; }
        }

        public Space()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            Window.Title = "AstroidClone by LaiHor Ent.";
            contentMan = Content;
            graphics.PreferredBackBufferWidth *= 2;
            graphics.PreferredBackBufferHeight *= 2;
            gamewindow = Window;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //Highscore
            dbcon.Open();
            //Laver highscore table
            command.CommandText = "CREATE TABLE IF NOT EXISTS Highscore (ID INTEGER , Name TEXT, Score INTEGER)";
            command.ExecuteNonQuery();

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            sf = Content.Load<SpriteFont>("MyFont");


            // TODO: use this.Content to load your game content here
            addObjects.Add(Player.Instance);
            Player.Instance.Position = new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            //Indtastning af navn
            if (Player.Instance.Lives == 0)
            {
                if (enterNameTime)
                {
                    GetKeys();
                }
            }

            //Respawn af astroids
            int tmpEnemyCount = 0;
            foreach (SpriteObject obj in objects)
            {
                if (obj is Enemy)
                {
                    if ((obj as Enemy).Type == EnemyType.AstroidBig || (obj as Enemy).Type == EnemyType.AstroidNormal || (obj as Enemy).Type == EnemyType.AstroidSmall)
                    {
                        tmpEnemyCount++;
                    }
                }
            }
            foreach (SpriteObject obj in addObjects)
            {
                if (obj is Enemy)
                {
                    if ((obj as Enemy).Type == EnemyType.AstroidBig || (obj as Enemy).Type == EnemyType.AstroidNormal || (obj as Enemy).Type == EnemyType.AstroidSmall)
                    {
                        tmpEnemyCount++;
                    }
                }
            }
            if (tmpEnemyCount == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    int x = r.Next(0, 2);
                    int y = r.Next(0, 2);
                    if (x == 1)
                    {
                        x = Window.ClientBounds.Width;
                    }
                    if (y == 1)
                    {
                        y = Window.ClientBounds.Height;
                    }
                    EnemyDirector director = new EnemyDirector(new AstroidBig(), Content, new Vector2(x, y));
                    director.BuildEnemy();
                    addObjects.Add(director.GetEnemy);
                }
            }

            //Random UFO spawn
            timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timer < 0)
            {
                timer = 25;
                int x = r.Next(0, 2);
                int y = r.Next(0, 2);
                if (x == 1)
                {
                    x = Window.ClientBounds.Width;
                }
                if (y == 1)
                {
                    y = Window.ClientBounds.Height;
                }
                switch (r.Next(0, 3))
                {
                    case 0:
                        EnemyDirector director = new EnemyDirector(new UFOSmall(), Content, new Vector2(x, y));
                        director.BuildEnemy();
                        addObjects.Add(director.GetEnemy);
                        break;

                    default:
                        EnemyDirector director2 = new EnemyDirector(new UFONormal(), Content, new Vector2(x, y));
                        director2.BuildEnemy();
                        addObjects.Add(director2.GetEnemy);
                        break;
                }
            }

            //Ordner liv
            foreach (SpriteObject obj in objects)
            {
                if (obj is Life)
                {
                    removeObjects.Add(obj);
                }
            }
            switch (Player.Instance.Lives)
            {
                case 3:
                    addObjects.Add(new Life(new Vector2(64, 64)));
                    addObjects.Add(new Life(new Vector2(64 + 128, 64)));
                    addObjects.Add(new Life(new Vector2(64 + 128 * 2, 64)));
                    break;

                case 2:
                    addObjects.Add(new Life(new Vector2(64, 64)));
                    addObjects.Add(new Life(new Vector2(64 + 128, 64)));
                    break;

                case 1:
                    addObjects.Add(new Life(new Vector2(64, 64)));
                    break;

                default:
                    break;
            }

            //Holder styr på listerne
            foreach (SpriteObject obj in removeObjects)
            {
                objects.Remove(obj);
            }
            foreach (SpriteObject obj in addObjects)
            {
                objects.Add(obj);
                obj.LoadContent(Content);
            }
            removeObjects.Clear();
            addObjects.Clear();
            foreach (SpriteObject obj in objects)
            {
                obj.Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);


            // TODO: Add your drawing code here
            spriteBatch.Begin();
            foreach (SpriteObject obj in objects)
            {
                obj.Draw(spriteBatch);
            }
            spriteBatch.DrawString(sf, "Score: " + score, new Vector2(0, 128), Color.White);
            #region død, highscore mm.
            if (Player.Instance.Lives == 0)
            {
                spriteBatch.DrawString(sf, "Game Over!", new Vector2(0, 64), Color.White);
                if (enterNameTime)
                {
                    spriteBatch.DrawString(sf, "Enter name:", new Vector2(500, Window.ClientBounds.Height / 2), Color.White);
                    spriteBatch.DrawString(sf, playerName, new Vector2(700, Window.ClientBounds.Height / 2), Color.White);
                }
            }
            if (drawHighscore)
            {
                #region highscore
                //Highscore
                command.CommandText = "select * from Highscore order by Score desc";
                command.ExecuteNonQuery();
                SQLiteDataReader reader = command.ExecuteReader();
                List<string> Names = new List<string>();
                List<string> Scores = new List<string>();
                while (reader.Read())
                {
                    Names.Add(reader["Name"].ToString());
                    Scores.Add(reader["Score"].ToString());
                }
                reader.Close();
                if (Names.Count < 10)
                {
                    int x = Names.Count;
                    for (int i = 0; i < 10 - x; i++)
                    {
                        Names.Add(" ");
                        Scores.Add(" ");
                    }
                }
                spriteBatch.DrawString(sf, "Highscore:", new Vector2(0, 170), Color.White);

                spriteBatch.DrawString(sf, "Name", new Vector2(0, 210), Color.White);
                spriteBatch.DrawString(sf, "Score", new Vector2(500, 210), Color.White);

                for (int i = 0; i < Names.Count; i++)
                {
                    spriteBatch.DrawString(sf, Names[i].ToString(), new Vector2(0, 230 + i * 20), Color.White);
                }
                for (int i = 0; i < Scores.Count; i++)
                {
                    spriteBatch.DrawString(sf, Scores[i].ToString(), new Vector2(500, 230 + i * 20), Color.White);
                }

                //Press r to restart
                spriteBatch.DrawString(sf, "Press r to restart", new Vector2(700, Window.ClientBounds.Height / 2 + 20), Color.White);
                pressRToRestart();
                #endregion
            }
            #endregion
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void GetKeys()
        {
            KeyboardState kbState = Keyboard.GetState();
            Keys[] pressedKeys = kbState.GetPressedKeys();

            //check  if any of the previous update's keys are no longer pressed
            try
            {
                foreach (Keys key in lastPressedKeys)
                {
                    if (!pressedKeys.Contains(key))
                    {
                        OnKeyUp(key);
                    }
                }
            }
            catch (Exception)
            {

            }

            //check if the currently pressed keys were already pressed
            try
            {
                foreach (Keys key in pressedKeys)
                {
                    if (!lastPressedKeys.Contains(key))
                    {
                        OnKeyDown(key);
                    }
                }
            }
            catch (Exception)
            {

            }

            //save the currently pressed keys so we can compare on the next update
            lastPressedKeys = pressedKeys;
        }
        private void OnKeyDown(Keys key)
        {
            if (key == Keys.Back && playerName.Length > 0)
            {
                playerName = playerName.Remove(playerName.Length - 1);
            }
            else if (key == Keys.Enter)
            {
                enterNameTime = false;
                drawHighscore = true;
                command.CommandText = "INSERT INTO Highscore(Name, Score) VALUES('" + playerName + "', " + score + ")";
                command.ExecuteNonQuery();
            }
            else
            {
                playerName += key.ToString();
            }
        }
        private void OnKeyUp(Keys key)
        {
            //do stuff
        }
        private void pressRToRestart()
        {
            KeyboardState kbState = Keyboard.GetState();
            Keys[] pressedKeys = kbState.GetPressedKeys();

            foreach (Keys key in pressedKeys)
            {
                if (key == Keys.R)
                {
                    
                }
            }
        }
    }
}
