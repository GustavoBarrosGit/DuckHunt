using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;

namespace DuckHunt
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D texBtnPlay, texBtnRestart, texPlayer, texPlayerArrow, texEnemyEgg;
        private List<Texture2D> texEnemies = new List<Texture2D>();
        public SoundEffect sndBtnDown, sndBtnOver, sndDeath, sndArrowEgg;
        private SpriteFont fontArial;

        enum GameState
        {
            MainMenu,
            Gameplay,
            GameOver
        }

        private GameState _gameState;

        private KeyboardState keyState = Keyboard.GetState();
        private MenuButton playButton;
        private MenuButton restartButton;

        private List<Enemy> enemies = new List<Enemy>();
        private List<EnemyEgg> enemyEggs = new List<EnemyEgg>();
        private List<PlayerArrow> playerArrows = new List<PlayerArrow>();
        private Player player = null;

        private int restartDelay = 60 * 2;
        private int restartTick = 0;
        private int spawnEnemyDelay = 40;
        private int spawnEnemyTick = 0;
        private int playerShootDelay = 10;
        private int playerShootTick = 0;

        #region Game1
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        #endregion


        #region Initialize
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 960;
            _graphics.PreferredBackBufferHeight = 1280;
            _graphics.ApplyChanges();

            base.Initialize();
        }
        #endregion


        #region LoadContent
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            texBtnPlay = Content.Load<Texture2D>("sprBtnPlay");
            texBtnRestart = Content.Load<Texture2D>("sprBtnRestart");
            texPlayer = Content.Load<Texture2D>("sprPlayer");
            texPlayerArrow = Content.Load<Texture2D>("sprArrowPlayer");
            texEnemyEgg = Content.Load<Texture2D>("sprEggEnemy");

            for (int i = 0; i < 3; i++)
            {
                texEnemies.Add(Content.Load<Texture2D>("sprDuck" + i));
            }

            sndBtnDown = Content.Load<SoundEffect>("sndBtnDown");
            sndBtnOver = Content.Load<SoundEffect>("sndBtnOver");
            sndDeath = Content.Load<SoundEffect>("sndExplode");
            sndArrowEgg = Content.Load<SoundEffect>("sndArrow");

            fontArial = Content.Load<SpriteFont>("Arial");

            playButton = new MenuButton(this, new Vector2(_graphics.PreferredBackBufferWidth * 0.5f - (int)(texBtnPlay.Width * 0.5), _graphics.PreferredBackBufferHeight * 0.5f), texBtnPlay);
            restartButton = new MenuButton(this, new Vector2(_graphics.PreferredBackBufferWidth * 0.5f - (int)(texBtnPlay.Width * 0.5), _graphics.PreferredBackBufferHeight * 0.5f), texBtnRestart);

            changeGameState(GameState.MainMenu);
        }
        #endregion


        #region Update
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            keyState = Keyboard.GetState();
            switch (_gameState)
            {
                case GameState.MainMenu:
                    {
                        UpdateMainMenu(gameTime);
                        break;
                    }

                case GameState.Gameplay:
                    {
                        UpdateGameplay(gameTime);
                        break;
                    }

                case GameState.GameOver:
                    {
                        UpdateGameOver(gameTime);
                        break;
                    }
            }

            base.Update(gameTime);
        }
        #endregion


        #region Update Methods
        private void UpdateMainMenu(GameTime gameTime)
        {
            if (playButton.isActive)
            {
                MouseState mouseState = Mouse.GetState();

                if (playButton.boundingBox.Contains(mouseState.Position))
                {
                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        playButton.SetDown(true);
                        playButton.SetHovered(false);
                    }
                    else
                    {
                        playButton.SetDown(false);
                        playButton.SetHovered(true);
                    }

                    if (mouseState.LeftButton == ButtonState.Released && playButton.lastIsDown)
                    {
                        changeGameState(GameState.Gameplay);
                    }
                }
                else
                {
                    playButton.SetDown(false);
                    playButton.SetHovered(false);
                }

                playButton.lastIsDown = mouseState.LeftButton == ButtonState.Pressed ? true : false;
            }
            else
            {
                playButton.isActive = true;
            }
        }

        private void UpdateGameplay(GameTime gameTime)
        {
            if (player == null)
            {
                player = new Player(texPlayer, new Vector2(_graphics.PreferredBackBufferWidth * 0.5f, _graphics.PreferredBackBufferHeight * 0.5f));
            }
            else
            {
                player.body.velocity = new Vector2(0, 0);

                if (player.isDead())
                {
                    if (restartTick < restartDelay)
                    {
                        restartTick++;
                    }
                    else
                    {
                        changeGameState(GameState.GameOver);
                        restartTick = 0;
                    }
                }
                else
                {
                    if (keyState.IsKeyDown(Keys.W))
                    {
                        player.MoveUp();
                    }
                    if (keyState.IsKeyDown(Keys.S))
                    {
                        player.MoveDown();
                    }
                    if (keyState.IsKeyDown(Keys.A))
                    {
                        player.MoveLeft();
                    }
                    if (keyState.IsKeyDown(Keys.D))
                    {
                        player.MoveRight();
                    }
                    if (keyState.IsKeyDown(Keys.Space))
                    {
                        if (playerShootTick < playerShootDelay)
                        {
                            playerShootTick++;
                        }
                        else
                        {
                            sndArrowEgg.Play();
                            PlayerArrow arrow = new PlayerArrow(texPlayerArrow, new Vector2(player.position.X + player.destOrigin.X, player.position.Y), new Vector2(0, -10));
                            playerArrows.Add(arrow);
                            playerShootTick = 0;
                        }
                    }
                }

                player.Update(gameTime);

                player.position.X = MathHelper.Clamp(player.position.X, 0, _graphics.PreferredBackBufferWidth - player.body.boundingBox.Width);
                player.position.Y = MathHelper.Clamp(player.position.Y, 0, _graphics.PreferredBackBufferHeight - player.body.boundingBox.Height);

                for (int i = 0; i < playerArrows.Count; i++)
                {
                    playerArrows[i].Update(gameTime);

                    if (playerArrows[i].position.Y < 0)
                    {
                        playerArrows.Remove(playerArrows[i]);
                        continue;
                    }
                }

                for (int i = 0; i < enemyEggs.Count; i++)
                {
                    enemyEggs[i].Update(gameTime);

                    if (player != null)
                    {
                        if (!player.isDead())
                        {
                            if (player.body.boundingBox.Intersects(enemyEggs[i].body.boundingBox))
                            {
                                sndDeath.Play();

                                player.setDead(true);
                            }
                        }
                    }

                    if (enemyEggs[i].position.Y > GraphicsDevice.Viewport.Height)
                    {
                        enemyEggs.Remove(enemyEggs[i]);
                    }

                }

                for (int i = 0; i < enemies.Count; i++)
                {
                    enemies[i].Update(gameTime);

                    if (player != null)
                    {
                        if (!player.isDead())
                        {
                            if (player.body.boundingBox.Intersects(enemies[i].body.boundingBox))
                            {
                                sndDeath.Play();

                                player.setDead(true);
                            }

                            if (enemies[i].GetType() == typeof(YellowDuck))
                            {
                                YellowDuck enemy = (YellowDuck)enemies[i];

                                if (enemy.canShoot)
                                {
                                    EnemyEgg egg = new EnemyEgg(texEnemyEgg, new Vector2(enemy.position.X, enemy.position.Y), new Vector2(0, 5));
                                    enemyEggs.Add(egg);

                                    enemy.resetCanShoot();
                                }
                            }
                            if (enemies[i].GetType() == typeof(RedDuck))
                            {
                                RedDuck enemy = (RedDuck)enemies[i];

                                if (Vector2.Distance(enemies[i].position, player.position + player.destOrigin) < 500)
                                {
                                    enemy.SetState(RedDuck.States.CHASE);
                                }

                                if (enemy.GetState() == RedDuck.States.CHASE)
                                {
                                    Vector2 direction = (player.position + player.destOrigin) - enemy.position;
                                    direction.Normalize();

                                    float speed = 5;
                                    enemy.body.velocity = direction * speed;


                                }
                            }
                        }
                    }

                    if (enemies[i].position.Y > GraphicsDevice.Viewport.Height)
                    {
                        enemies.Remove(enemies[i]);
                    }
                }
            }
            for (int i = 0; i < playerArrows.Count; i++)
            {
                bool shouldDestroyArrow = false;
                for (int j = 0; j < enemies.Count; j++)
                {
                    if (playerArrows[i].body.boundingBox.Intersects(enemies[j].body.boundingBox))
                    {
                        sndDeath.Play();

                        Console.WriteLine("Shot enemy.  Origin: " + enemies[j].destOrigin + ", pos: " + enemies[j].position);

                        enemies.Remove(enemies[j]);

                        shouldDestroyArrow = true;
                    }
                }

                if (shouldDestroyArrow)
                {
                    playerArrows.Remove(playerArrows[i]);
                }
            }
            if (spawnEnemyTick < spawnEnemyDelay)
            {
                spawnEnemyTick++;
            }
            else
            {
                Enemy enemy = null;

                if (randInt(0, 10) <= 3)
                {
                    Vector2 spawnPos = new Vector2(randFloat(0, _graphics.PreferredBackBufferWidth), -128);
                    enemy = new YellowDuck(texEnemies[0], spawnPos, new Vector2(0, randFloat(1, 3)));
                }
                else if (randInt(0, 10) >= 5)
                {
                    Vector2 spawnPos = new Vector2(randFloat(0, _graphics.PreferredBackBufferWidth), -128);
                    enemy = new RedDuck(texEnemies[1], spawnPos, new Vector2(0, randFloat(1, 3)));
                }
                else
                {
                    Vector2 spawnPos = new Vector2(randFloat(0, _graphics.PreferredBackBufferWidth), -128);
                    enemy = new GreenDuck(texEnemies[2], spawnPos, new Vector2(0, randFloat(1, 3)));
                }

                enemies.Add(enemy);

                spawnEnemyTick = 0;
            }

        }

        private void UpdateGameOver(GameTime gameTime)
        {
            if (restartButton.isActive)
            {
                MouseState mouseState = Mouse.GetState();

                if (restartButton.boundingBox.Contains(mouseState.Position))
                {
                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        restartButton.SetDown(true);
                        restartButton.SetHovered(false);
                    }
                    else
                    {
                        restartButton.SetDown(false);
                        restartButton.SetHovered(true);
                    }

                    if (mouseState.LeftButton == ButtonState.Released && restartButton.lastIsDown)
                    {
                        changeGameState(GameState.Gameplay);
                    }
                }
                else
                {
                    restartButton.SetDown(false);
                    restartButton.SetHovered(false);
                }

                restartButton.lastIsDown = mouseState.LeftButton == ButtonState.Pressed ? true : false;
            }
            else
            {
                restartButton.isActive = true;
            }

        }
        private void resetGameplay()
        {
            if (player != null)
            {
                player.setDead(false);
                player.position = new Vector2((int)(_graphics.PreferredBackBufferWidth * 0.5), (int)(_graphics.PreferredBackBufferHeight * 0.5));
            }
        }

        private void changeGameState(GameState gameState)
        {
            playButton.isActive = false;
            restartButton.isActive = false;
            enemies.Clear();
            playerArrows.Clear();
            enemyEggs.Clear();
            resetGameplay();

            _gameState = gameState;
        }
        #endregion


        #region Draw
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightSkyBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap);

            switch (_gameState)
            {
                case GameState.MainMenu:
                    {
                        DrawMainMenu(_spriteBatch);
                        break;
                    }

                case GameState.Gameplay:
                    {
                        DrawGameplay(_spriteBatch);
                        break;
                    }

                case GameState.GameOver:
                    {
                        DrawGameOver(_spriteBatch);
                        break;
                    }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion


        #region Draw Methods
        private void DrawMainMenu(SpriteBatch spriteBatch)
        {
            string title = "Duck Hunt";
            spriteBatch.DrawString(fontArial, title, new Vector2(_graphics.PreferredBackBufferWidth * 0.5f - (fontArial.MeasureString(title).X * 0.5f), _graphics.PreferredBackBufferHeight * 0.2f), Color.Black);

            playButton.Draw(spriteBatch);

        }

        private void DrawGameplay(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(spriteBatch);
            }

            for (int i = 0; i < playerArrows.Count; i++)
            {
                playerArrows[i].Draw(spriteBatch);
            }

            for (int i = 0; i < enemyEggs.Count; i++)
            {
                enemyEggs[i].Draw(spriteBatch);
            }

            if (player != null)
            {
                player.Draw(spriteBatch);
            }

        }

        private void DrawGameOver(SpriteBatch spriteBatch)
        {
            string title = "GAME OVER";
            spriteBatch.DrawString(fontArial, title, new Vector2(_graphics.PreferredBackBufferWidth * 0.5f - (fontArial.MeasureString(title).X * 0.5f), _graphics.PreferredBackBufferHeight * 0.2f), Color.Black);

            restartButton.Draw(spriteBatch);

        }

        public static int randInt(int minNumber, int maxNumber)
        {
            return new Random().Next(minNumber, maxNumber);
        }

        public static float randFloat(float minNumber, float maxNumber)
        {
            return (float)new Random().NextDouble() * (maxNumber - minNumber) + minNumber;
        }
        #endregion
    }
}
