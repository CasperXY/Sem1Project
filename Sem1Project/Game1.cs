using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace Sem1Project
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Texture2D backgroundTxr, playersheetTxr, platformTxr, whiteBox, kiBlastTxr, enemysheetTxr, wallTxr, backgroundTxr1, backgroundTxr2, winScreen, gameOver; //Naming The Textures
        Song backingTrack; //Naming the backing track
        SoundEffect jumpSound, hitSound, blastSound, deathSound, advanceSound; //Naming the Sound Effects
        SpriteFont newFont; //Naming the sprite
        Point screenSize = new Point(800, 450); //Setting Scrren Size
        PlayerSprite playerSprite; //Giving Player Sprite class a variable
        bool kiBlast = false; //Setting up the starting blast value
        int levelNumber = 0; //Setting up the starting level number value
        public int lives = 3;//Setting up the starting value of the lives
        //Setting up Lists
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
            _graphics.PreferredBackBufferWidth = screenSize.X;
            _graphics.PreferredBackBufferHeight = screenSize.Y;
            _graphics.ApplyChanges();

            base.Initialize();
        }
        //Loading All Content
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            backgroundTxr = Content.Load<Texture2D>("nightBackground"); //Loading background Texture for level 1
            backgroundTxr1 = Content.Load<Texture2D>("background1"); //Loading background Texture for level 2
            backgroundTxr2 = Content.Load<Texture2D>("background2"); //Loading background Texture for level 3
            winScreen = Content.Load<Texture2D>("winScreen"); //Loading the win screen Texture
            gameOver = Content.Load<Texture2D>("gameOver"); //Loading the game over screen Texture
            playersheetTxr = Content.Load<Texture2D>("charSprite"); //Loading the player character sprite sheet
            platformTxr = Content.Load<Texture2D>("spriteSheet2"); //Loading the platform Texture
            wallTxr = Content.Load<Texture2D>("spriteSheet2.1"); //Loading the rotated platform Texture
            kiBlastTxr = Content.Load<Texture2D>("KiBlastSprite"); //Loading the Blast sprite sheet
            enemysheetTxr = Content.Load<Texture2D>("CharSprite2"); //Loading the enemy character sprite sheet
            backingTrack = Content.Load<Song>("backingTrack");//Loading the backing track .MP3
            jumpSound = Content.Load<SoundEffect>("jump"); //Loading the jump sound .WAV
            hitSound = Content.Load<SoundEffect>("hitSound");//Loading the hit sound .WAV
            deathSound = Content.Load<SoundEffect>("deathSound");//Loading the death sound .WAV
            advanceSound = Content.Load<SoundEffect>("advanceSound");//Loading the level advance sound .WAV
            blastSound = Content.Load<SoundEffect>("kiBlastSound"); //Loading the blast sound .WAV
            newFont = Content.Load<SpriteFont>("newFont"); //Loading the font style
            MediaPlayer.Play(backingTrack); //Starting the backing tack .MP3

            whiteBox = new Texture2D(GraphicsDevice, 1, 1);
            whiteBox.SetData(new[] { Color.White });

            playerSprite = new PlayerSprite(playersheetTxr, whiteBox, new Vector2(50, 430), hitSound, jumpSound); //giving player sprite variable a value

            BuildLevels(); //Building the Platforms
            EnemySpawn(); //Building the Enemies
            BuildWalls(); //Building the Rotated Platform
        }

        protected override void Update(GameTime gameTime)
        {
            playerSprite.spritePos = Vector2.Clamp(playerSprite.spritePos, new Vector2(50, 0), new Vector2(780, 1500)); //Stopping the Character sprite from being able to go off the screen
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit(); // when ESC is pressed the game will close
            KeyboardState keyboardState = Keyboard.GetState(); 
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            if (levelNumber == 4 && keyboardState.IsKeyDown(Keys.Enter))
            {
                levelNumber = 0;
                lives = 3;
                playerSprite = new PlayerSprite(playersheetTxr, whiteBox, new Vector2(50, 430), hitSound, jumpSound); //giving player sprite variable a value
                foreach (EnemySprite enemySprite in enemySprites[levelNumber])
                {
                    enemySprite.dead = false;
                    enemySprite.levelAdvance = false;
                }
                MediaPlayer.Play(backingTrack);
            }
            if (levelNumber == 3 && keyboardState.IsKeyDown(Keys.Enter))
            {
                levelNumber = 0;
                lives = 3;
                playerSprite = new PlayerSprite(playersheetTxr, whiteBox, new Vector2(50, 430), hitSound, jumpSound); //giving player sprite variable a value
                foreach (EnemySprite enemySprite in enemySprites[levelNumber])
                {
                    enemySprite.dead = false;
                    enemySprite.levelAdvance = false;
                }
                MediaPlayer.Play(backingTrack);
            }
            // if you are on any screen other than the game over screen then pressing space will create a blast corresponding to where the player character is positioned and whether or not the sprite is flipped
            if (levelNumber < 4)
            {
                if (keyboardState.IsKeyDown(Keys.Space) && !kiBlast && playerSprite.currentState != PlayerSprite.playerStates.kiBlast)
                {
                    if (playerSprite.flipped) blasts.Add(new KiBlastSprite(kiBlastTxr, whiteBox, new Vector2(playerSprite.spritePos.X - 25, playerSprite.spritePos.Y - 25), new Vector2(-300, 0)));
                    else blasts.Add(new KiBlastSprite(kiBlastTxr, whiteBox, new Vector2(playerSprite.spritePos.X + 25, playerSprite.spritePos.Y - 25), new Vector2(300, 0)));
                    kiBlast = true; 
                    blastSound.Play(); //Playing the sound of the blast
                }
                else if (!keyboardState.IsKeyDown(Keys.Space) && kiBlast) kiBlast = false; //declairing the value of the blast false when space bar is no longer pressed after using a blast
               //Updating the blasts
                foreach (KiBlastSprite kiBlast in blasts)
                {
                    kiBlast.Update(gameTime, enemySprites[levelNumber], playerSprite);
                }
            }

            foreach (EnemySprite enemySprite in enemySprites[levelNumber])
            {
                if (!enemySprite.dead) enemySprite.Update(gameTime, blasts, playerSprite);//if the enemy sprite is not dead then the enemy sprites update
                if (enemySprite.levelAdvance) 
                {
                    levelNumber ++;
                    playerSprite = new PlayerSprite(playersheetTxr, whiteBox, new Vector2(50, 430), hitSound, jumpSound);
                    break;
                }
                //Controlling the movement of the enemies on the 1st level
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
                //Controlling the enemies movement on the 2nd level
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
                //Controlling the enemies on the 3rd level
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
           
            if (!playerSprite.dead)playerSprite.Update(gameTime, levels[levelNumber], enemySprites[levelNumber], walls[levelNumber]); //Updating the PlayerSprite Class
            blasts.RemoveAll(kiBlast => kiBlast.dead); //Removing the blasts when they are declared dead
            //Resetting the players position on death as long as the lives are greater than 0
            if (playerSprite.dead && lives > 0)
            {
                playerSprite = new PlayerSprite(playersheetTxr, whiteBox, new Vector2(50, 430), hitSound, jumpSound);
                lives--;
            }
            //If your lives reach 0 it loads the gameover screen and stops the music
            if (lives <= 0)
            {
                levelNumber = 4;
                MediaPlayer.Stop();
            }
            base.Update(gameTime);
        }


        //Drawing the code
        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            if (levelNumber == 0)_spriteBatch.Draw(backgroundTxr, new Rectangle(0, 0, screenSize.X, screenSize.Y), Color.White); //Drawing background for level 1
            if (levelNumber == 1)_spriteBatch.Draw(backgroundTxr1, new Rectangle(0, 0, screenSize.X, screenSize.Y), Color.White);//Drawing background for level 2
            if (levelNumber == 2)_spriteBatch.Draw(backgroundTxr2, new Rectangle(0, 0, screenSize.X, screenSize.Y), Color.White);//Drawing background for level 3
            if (levelNumber == 3)_spriteBatch.Draw(winScreen, new Rectangle(0, 0, screenSize.X, screenSize.Y), Color.White);//Drawing the Win Screen
            if (levelNumber == 4)_spriteBatch.Draw(gameOver, new Rectangle(0, 0, screenSize.X, screenSize.Y), Color.White);//Drawing the Game Over Screen
            //Drawing the lives counter at the top left
            string livesString = "";
            if (lives == 3) livesString = "lives: " + "3";
            else if (lives == 2) livesString ="lives:" + "2";
            else if (lives == 1) livesString ="lives:" + "1";
            else livesString = "";
            _spriteBatch.DrawString(newFont, livesString, new Vector2(10, 7), Color.DarkCyan);
            if (lives > 0) playerSprite.Draw(_spriteBatch, gameTime);//Drawing the Player Sprite
            
            string playAgain = "";
            if (levelNumber == 4)
            {
                playAgain = "Press Enter To Play Again.";
                _spriteBatch.DrawString(newFont, playAgain, new Vector2(190, 400), Color.Crimson);
            }
            if (levelNumber == 3)
            {
                playAgain = "Press Enter To Play Again.";
                _spriteBatch.DrawString(newFont, playAgain, new Vector2(190, 7), Color.Black);
            }
            //Drawing the Blast Sprites
            foreach (KiBlastSprite kiBlast in blasts)
            {
                kiBlast.Draw(_spriteBatch, gameTime);
            }

            //Drawing the Enemy Sprites
            foreach (EnemySprite enemySprite in enemySprites[levelNumber])
            {
                if (!enemySprite.dead) enemySprite.Draw(_spriteBatch, gameTime);
            }

            //Drawing the Platform Sprites
            foreach (PlatformSprite platform in levels[levelNumber]) 
            {
                platform.Draw(_spriteBatch, gameTime);
            }

            //Drawing the Rotated Platform Sprite
            foreach (PlatformSpriteRotate rotate in walls[levelNumber]) 
            {
                rotate.Draw(_spriteBatch, gameTime);
            }
            _spriteBatch.End(); 
            base.Draw(gameTime);
        }
        void BuildLevels() //Building the Platforms on each level
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
            levels[4].Add(new PlatformSprite(platformTxr, whiteBox, new Vector2(50, 1000)));
        }
        
        void EnemySpawn() //Building the Enemy Spawns on each level
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

        void BuildWalls() //Building the Walls on the levels
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
