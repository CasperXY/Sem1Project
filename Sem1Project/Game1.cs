using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System;





namespace Sem1Project
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Texture2D backgroundTxr, playersheetTxr, platformTxr, whiteBox, kiBlastTxr, enemysheetTxr, wallTxr, backgroundTxr1, backgroundTxr2, winScreen, gameOver;
        Song backingTrack;
        SoundEffect jumpSound, hitSound, blastSound, deathSound, advanceSound, winSong;
        SpriteFont newFont;
        Point screenSize = new Point(800, 450);


        PlayerSprite playerSprite;

        bool kiBlast = false;
        int levelNumber = 0;
        public int lives = 3;




        List<List<PlatformSpriteRotate>> walls = new List<List<PlatformSpriteRotate>>();
        List<List<PlatformSprite>> levels = new List<List<PlatformSprite>>();
        List<KiBlastSprite> blasts = new List<KiBlastSprite>();
        List<List<EnemySprite>> enemySprites = new List<List<EnemySprite>>();
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = screenSize.X;
            _graphics.PreferredBackBufferHeight = screenSize.Y;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            backgroundTxr = Content.Load<Texture2D>("nightBackground");
            backgroundTxr1 = Content.Load<Texture2D>("background1");
            backgroundTxr2 = Content.Load<Texture2D>("background2");
            winScreen = Content.Load<Texture2D>("winScreen");
            gameOver = Content.Load<Texture2D>("gameOver");
            playersheetTxr = Content.Load<Texture2D>("charSprite");
            platformTxr = Content.Load<Texture2D>("spriteSheet2");
            wallTxr = Content.Load<Texture2D>("spriteSheet2.1");
            kiBlastTxr = Content.Load<Texture2D>("KiBlastSprite");
            enemysheetTxr = Content.Load<Texture2D>("CharSprite2");
            backingTrack = Content.Load<Song>("backingTrack");
            winSong = Content.Load<SoundEffect>("winSong");
            jumpSound = Content.Load<SoundEffect>("jump");
            hitSound = Content.Load<SoundEffect>("hitSound");
            deathSound = Content.Load<SoundEffect>("deathSound");
            advanceSound = Content.Load<SoundEffect>("advanceSound");
            blastSound = Content.Load<SoundEffect>("kiBlastSound");
            newFont = Content.Load<SpriteFont>("newFont");
            MediaPlayer.Play(backingTrack);




            whiteBox = new Texture2D(GraphicsDevice, 1, 1);
            whiteBox.SetData(new[] { Color.White });

            playerSprite = new PlayerSprite(playersheetTxr, whiteBox, new Vector2(50, 430), hitSound, jumpSound);

            BuildLevels();
            EnemySpawn();
            BuildWalls();

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            if (keyboardState.IsKeyDown(Keys.Space) && !kiBlast && playerSprite.currentState != PlayerSprite.playerStates.kiBlast) 
            {
                if (playerSprite.flipped) blasts.Add(new KiBlastSprite(kiBlastTxr, whiteBox, new Vector2(playerSprite.spritePos.X -25, playerSprite.spritePos.Y -25), new Vector2(-300, 0)));
                else blasts.Add(new KiBlastSprite(kiBlastTxr, whiteBox, new Vector2(playerSprite.spritePos.X +25 , playerSprite.spritePos.Y - 25), new Vector2(300, 0)));

                kiBlast = true;
                blastSound.Play();
                
            }
            else if (!keyboardState.IsKeyDown(Keys.Space) && kiBlast) kiBlast = false;
            foreach (KiBlastSprite kiBlast in blasts)
            {
                kiBlast.Update(gameTime, enemySprites[levelNumber], playerSprite);


            }

            if (lives > 0 && enemySprites[levelNumber].Count < 4)
            {
                EnemySpawn();
            }
            else if (lives <= 0) 
            {
                levelNumber = 0;
                playerSprite = new PlayerSprite(playersheetTxr, whiteBox, new Vector2(50, 430), hitSound, jumpSound);


            }
            foreach (EnemySprite enemySprite in enemySprites[levelNumber])
            {
                if (!enemySprite.dead) enemySprite.Update(gameTime, blasts, playerSprite);
                if (enemySprite.levelAdvance) 
                {
                    levelNumber ++;
                    playerSprite = new PlayerSprite(playersheetTxr, whiteBox, new Vector2(50, 430), hitSound, jumpSound);
                    break;
                }
                if (levelNumber == 0)
                {
                    if (enemySprite.spritePos.Y < 110f && enemySprite.spritePos.X > 748f && enemySprite.spritePos.X < 790f && !enemySprite.flipped)
                    {
                        enemySprite.spritePos.X++;
                        enemySprite.flipped = false;
                    }
                    else if (enemySprite.spritePos.Y < 110f && !enemySprite.flipped && enemySprite.spritePos.X == 790f)
                    {
                        enemySprite.flipped = true;
                    }
                    if (enemySprite.spritePos.Y < 110f && enemySprite.flipped && enemySprite.spritePos.X > 750f) enemySprite.spritePos.X--;
                    else if (enemySprite.spritePos.Y < 110f && enemySprite.flipped && enemySprite.spritePos.X <= 750f)
                    {
                        enemySprite.flipped = false;
                    }




                    if (enemySprite.spritePos.Y > 216f && enemySprite.spritePos.X > 200f && enemySprite.spritePos.X < 790f && !enemySprite.flipped)
                    {
                        enemySprite.spritePos.X += 2;
                        enemySprite.flipped = false;
                    }
                    else if (enemySprite.spritePos.Y > 216f && !enemySprite.flipped && enemySprite.spritePos.X == 790f)
                    {
                        enemySprite.flipped = true;
                    }
                    if (enemySprite.spritePos.Y > 216f && enemySprite.flipped && enemySprite.spritePos.X > 202f) enemySprite.spritePos.X -= 2;
                    else if (enemySprite.spritePos.Y > 216f && enemySprite.flipped && enemySprite.spritePos.X <= 202f)
                    {
                        enemySprite.flipped = false;
                    }











                }
                if (levelNumber == 1) 
                {
                    if (enemySprite.spritePos.Y < 200f && enemySprite.spritePos.X > 748f && enemySprite.spritePos.X < 790f && !enemySprite.flipped)
                    {
                        enemySprite.spritePos.X++;
                        enemySprite.flipped = false;
                    }
                    else if (enemySprite.spritePos.Y < 200f && !enemySprite.flipped && enemySprite.spritePos.X == 790f)
                    {
                        enemySprite.flipped = true;
                    }
                    if (enemySprite.spritePos.Y < 200f && enemySprite.flipped && enemySprite.spritePos.X > 750f) enemySprite.spritePos.X--;
                    else if (enemySprite.spritePos.Y < 200f && enemySprite.flipped && enemySprite.spritePos.X <= 750f)
                    {
                        enemySprite.flipped = false;
                    }



                    if (enemySprite.spritePos.Y > 301f && enemySprite.spritePos.X > 200f && enemySprite.spritePos.X < 790f && !enemySprite.flipped)
                    {
                        enemySprite.spritePos.X += 2;
                        enemySprite.flipped = false;
                    }
                    else if (enemySprite.spritePos.Y > 301f && !enemySprite.flipped && enemySprite.spritePos.X == 790f)
                    {
                        enemySprite.flipped = true;
                    }
                    if (enemySprite.spritePos.Y > 301f && enemySprite.flipped && enemySprite.spritePos.X > 499) enemySprite.spritePos.X -= 2;
                    else if (enemySprite.spritePos.Y > 301f && enemySprite.flipped && enemySprite.spritePos.X <= 500f)
                    {
                        enemySprite.flipped = false;
                    }



                }

                if (levelNumber == 2)
                {
                    if (enemySprite.spritePos.Y < 200f && enemySprite.spritePos.X > 748f && enemySprite.spritePos.X < 790f && !enemySprite.flipped)
                    {
                        enemySprite.spritePos.X++;
                        enemySprite.flipped = false;
                    }
                    else if (enemySprite.spritePos.Y < 200f && !enemySprite.flipped && enemySprite.spritePos.X == 790f)
                    {
                        enemySprite.flipped = true;
                    }
                    if (enemySprite.spritePos.Y < 200f && enemySprite.flipped && enemySprite.spritePos.X > 750f) enemySprite.spritePos.X--;
                    else if (enemySprite.spritePos.Y < 200f && enemySprite.flipped && enemySprite.spritePos.X <= 750f)
                    {
                        enemySprite.flipped = false;
                    }



                    if (enemySprite.spritePos.Y > 350f && enemySprite.spritePos.X > 200f && enemySprite.spritePos.X < 790f && !enemySprite.flipped)
                    {
                        enemySprite.spritePos.X += 2;
                        enemySprite.flipped = false;
                    }
                    else if (enemySprite.spritePos.Y > 350f && !enemySprite.flipped && enemySprite.spritePos.X == 790f)
                    {
                        enemySprite.flipped = true;
                    }
                    if (enemySprite.spritePos.Y > 350f && enemySprite.flipped && enemySprite.spritePos.X > 600) enemySprite.spritePos.X -= 2;
                    else if (enemySprite.spritePos.Y > 350f && enemySprite.flipped && enemySprite.spritePos.X <= 600f)
                    {
                        enemySprite.flipped = false;
                    }



                }




            }
            if (levelNumber == levels.Count) 
            {
                levelNumber = 0;
                foreach (List<EnemySprite> enemyLevels in enemySprites)
                {
                    foreach (EnemySprite enemy in enemyLevels)
                    {
                        enemy.dead = false;
                        enemy.levelAdvance = false;
                    }
                }
            }

            
            playerSprite.Update(gameTime, levels[levelNumber], enemySprites[levelNumber], walls[levelNumber]);
            blasts.RemoveAll(kiBlast => kiBlast.dead);
            
            if (playerSprite.dead && lives > 0)
            {
                playerSprite = new PlayerSprite(playersheetTxr, whiteBox, new Vector2(50, 430), hitSound, jumpSound);
                lives--;

            }
            
            if (lives <= 0)
            {
                levelNumber = 4;
            }



            
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            if (levelNumber == 0)_spriteBatch.Draw(backgroundTxr, new Rectangle(0, 0, screenSize.X, screenSize.Y), Color.White);
            if (levelNumber == 1)_spriteBatch.Draw(backgroundTxr1, new Rectangle(0, 0, screenSize.X, screenSize.Y), Color.White);
            if (levelNumber == 2)_spriteBatch.Draw(backgroundTxr2, new Rectangle(0, 0, screenSize.X, screenSize.Y), Color.White);
            if (levelNumber == 3)_spriteBatch.Draw(winScreen, new Rectangle(0, 0, screenSize.X, screenSize.Y), Color.White);
            if (levelNumber == 4)_spriteBatch.Draw(gameOver, new Rectangle(0, 0, screenSize.X, screenSize.Y), Color.White);

            string livesString = "";
            if (lives == 3) livesString = "lives: " + "3";
            else if (lives == 2) livesString ="lives:" + "2";
            else if (lives == 1) livesString ="lives:" + "1";
            else livesString = "";
            _spriteBatch.DrawString(newFont, livesString, new Vector2(10, 7), Color.DarkCyan);
            if (lives > 0) playerSprite.Draw(_spriteBatch, gameTime);



            foreach (KiBlastSprite kiBlast in blasts)
            {
                kiBlast.Draw(_spriteBatch, gameTime);
          
            }




            foreach (EnemySprite enemySprite in enemySprites[levelNumber])
            {
                if (!enemySprite.dead) enemySprite.Draw(_spriteBatch, gameTime);
            }


            foreach (PlatformSprite platform in levels[levelNumber]) 
            {
                platform.Draw(_spriteBatch, gameTime);
            }

            foreach (PlatformSpriteRotate rotate in walls[levelNumber]) 
            {
                rotate.Draw(_spriteBatch, gameTime);
            }






            _spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
        void BuildLevels()
        {

            levels.Add(new List<PlatformSprite>());
            levels[0].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(50, 425)));
            levels[0].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(145, 425)));
            levels[0].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(240, 425)));
            levels[0].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(335, 425)));
            levels[0].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(430, 425)));
            levels[0].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(525, 425)));
            levels[0].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(620, 425)));
            levels[0].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(715, 425)));
            levels[0].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(810, 425)));


            levels[0].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(65, 290)));
            levels[0].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(255, 210)));
            levels[0].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(780, 100)));

            levels.Add(new List<PlatformSprite>());
            levels[1].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(50, 425)));
            levels[1].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(158, 325)));
            levels[1].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(500, 425)));
            levels[1].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(590, 425)));
            levels[1].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(685, 425)));
            levels[1].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(780, 425)));
            
            
            levels[1].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(780, 300)));
            levels[1].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(600, 250)));
            levels[1].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(780, 100)));



            levels.Add(new List<PlatformSprite>());
            levels[2].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(50, 425)));
            levels[2].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(600, 425)));
            levels[2].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(695, 425)));
            levels[2].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(780, 425)));
            levels[2].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(780, 310)));
            levels[2].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(580, 250)));
            levels[2].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(30, 310)));
            levels[2].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(220, 310)));
            levels[2].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(410, 310)));
            levels[2].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(50, 100)));

            levels.Add(new List<PlatformSprite>());
            levels[3].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(50, 425)));
            levels[3].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(145, 425)));
            levels[3].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(240, 425)));
            levels[3].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(335, 425)));
            levels[3].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(430, 425)));
            levels[3].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(525, 425)));
            levels[3].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(620, 425)));
            levels[3].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(715, 425)));
            levels[3].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(810, 425)));

            levels.Add(new List<PlatformSprite>());
            levels[4].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(50, 500)));















        }
        
        void EnemySpawn() 
        {
            enemySprites.Add(new List<EnemySprite>());
            enemySprites[0].Add(new EnemySprite(enemysheetTxr, whiteBox, new Vector2(750, 100), advanceSound, deathSound));
            enemySprites[0].Add(new EnemySprite(enemysheetTxr, whiteBox, new Vector2(750, 430),advanceSound, deathSound));
            enemySprites[0].Add(new EnemySprite(enemysheetTxr, whiteBox, new Vector2(700, 430),advanceSound, deathSound));
            enemySprites[0].Add(new EnemySprite(enemysheetTxr, whiteBox, new Vector2(650, 430),advanceSound, deathSound));
            enemySprites[0].Add(new EnemySprite(enemysheetTxr, whiteBox, new Vector2(65, 295),advanceSound, deathSound));
            enemySprites[0].Add(new EnemySprite(enemysheetTxr, whiteBox, new Vector2(255, 215),advanceSound, deathSound));


            enemySprites.Add(new List<EnemySprite>());
            enemySprites[1].Add(new EnemySprite(enemysheetTxr, whiteBox, new Vector2(750, 100), advanceSound, deathSound));
            enemySprites[1].Add(new EnemySprite(enemysheetTxr, whiteBox, new Vector2(750, 430),advanceSound, deathSound));
            enemySprites[1].Add(new EnemySprite(enemysheetTxr, whiteBox, new Vector2(158, 325),advanceSound, deathSound));
            enemySprites[1].Add(new EnemySprite(enemysheetTxr, whiteBox, new Vector2(750, 300),advanceSound, deathSound));
            enemySprites[1].Add(new EnemySprite(enemysheetTxr, whiteBox, new Vector2(600, 250),advanceSound, deathSound));

            enemySprites.Add(new List<EnemySprite>());
            enemySprites[2].Add(new EnemySprite(enemysheetTxr, whiteBox, new Vector2(50, 100),advanceSound, deathSound));
            enemySprites[2].Add(new EnemySprite(enemysheetTxr, whiteBox, new Vector2(650, 430), advanceSound, deathSound));
            enemySprites[2].Add(new EnemySprite(enemysheetTxr, whiteBox, new Vector2(750, 430), advanceSound, deathSound));
            enemySprites[2].Add(new EnemySprite(enemysheetTxr, whiteBox, new Vector2(30, 310), advanceSound, deathSound));
            enemySprites[2].Add(new EnemySprite(enemysheetTxr, whiteBox, new Vector2(220, 310), advanceSound, deathSound));
            enemySprites[2].Add(new EnemySprite(enemysheetTxr, whiteBox, new Vector2(410, 310), advanceSound, deathSound));
            enemySprites[2].Add(new EnemySprite(enemysheetTxr, whiteBox, new Vector2(780, 310), advanceSound, deathSound));

            enemySprites.Add(new List<EnemySprite>());
            enemySprites[3].Add(new EnemySprite(enemysheetTxr, whiteBox, new Vector2(750, 430), advanceSound, deathSound));

            enemySprites.Add(new List<EnemySprite>());
            enemySprites[4].Add(new EnemySprite(enemysheetTxr, whiteBox, new Vector2(750, 650), advanceSound, deathSound));




        }

        void BuildWalls() 
        {
            walls.Add(new List<PlatformSpriteRotate>());
            walls[0].Add(new PlatformSpriteRotate(wallTxr, whiteBox, new Vector2(550, 500)));
            walls.Add(new List<PlatformSpriteRotate>());

            walls[1].Add(new PlatformSpriteRotate(wallTxr, whiteBox, new Vector2(550, 500)));
            walls.Add(new List<PlatformSpriteRotate>());

            walls[2].Add(new PlatformSpriteRotate(wallTxr, whiteBox, new Vector2(550, 150)));

            walls.Add(new List<PlatformSpriteRotate>());
            walls[3].Add(new PlatformSpriteRotate(wallTxr, whiteBox, new Vector2(550, 500)));

            walls.Add(new List<PlatformSpriteRotate>());
            walls[4].Add(new PlatformSpriteRotate(wallTxr, whiteBox, new Vector2(550, 500)));
        }
    }
}
