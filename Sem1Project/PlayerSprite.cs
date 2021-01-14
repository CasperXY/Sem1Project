using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Sem1Project
{
    class PlayerSprite : Sprite
    {
        bool jumpIsPressed, kickIsPressed, punchIsPressed, blastIsPressed;

         public playerStates currentState = playerStates.falling;
        SoundEffect hitSound, jumpSound;
        const float jumpSpeed = 5f;
        const float walkSpeed = 175f;
        public int lives = 3;
        public bool dead = false;
        public PlayerSprite(Texture2D newSpriteSheet, Texture2D newCollisionTxr, Vector2 newLocation, SoundEffect newHitSound, SoundEffect newJumpSound ) : base(newSpriteSheet, newCollisionTxr, newLocation)
        {
            hitSound = newHitSound;
            jumpSound = newJumpSound;
            spriteOrigin = new Vector2(0.5f, 1f);
            spriteScale = new Vector2(2, 2);
            isColliding = true;
            drawCollision = false;

            collisionInsetMin = new Vector2(0.25f, 0.3f);
            collisionInsetMax = new Vector2(0.25f, 0.03f);

            frameTime = 0.23f;

            // idle
            animations = new List<List<Rectangle>>();

            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(10, 5, 48, 50));
            animations[0].Add(new Rectangle(10, 5, 48, 51));


            // ki blast
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(710, 325, 48, 50));
            animations[1].Add(new Rectangle(710, 325, 48, 50));
            animations[1].Add(new Rectangle(710, 325, 48, 50));
            animations[1].Add(new Rectangle(710, 325, 48, 50));
            animations[1].Add(new Rectangle(710, 325, 48, 50));

            //walking
            animations.Add(new List<Rectangle>());
            animations[2].Add(new Rectangle(135, 65, 48, 50));
            
            //faling/jumping
            animations.Add(new List<Rectangle>());
            animations[3].Add(new Rectangle(10, 70, 48, 50));

            //kicking
            animations.Add(new List<Rectangle>());
            animations[4].Add(new Rectangle(398, 130, 48, 50));
            animations[4].Add(new Rectangle(462, 130, 48, 50));
            animations[4].Add(new Rectangle(590, 130, 48, 50));
            animations[4].Add(new Rectangle(782, 130, 48, 50));
            animations[4].Add(new Rectangle(846, 130, 48, 50));
            animations[4].Add(new Rectangle(846, 130, 48, 50));
            
            //punch
            animations.Add(new List<Rectangle>());
            animations[5].Add(new Rectangle(138, 130, 48, 50));
            animations[5].Add(new Rectangle(10, 130, 48, 50));
            animations[5].Add(new Rectangle(202, 130, 48, 50));
            animations[5].Add(new Rectangle(266, 130, 48, 50));
            animations[5].Add(new Rectangle(330, 130, 48, 50));
            animations[5].Add(new Rectangle(330, 130, 48, 50));





            jumpIsPressed = false;








        }
        


        public void Update(GameTime gameTime, List<PlatformSprite> platforms, List<EnemySprite>enemySprites, List<PlatformSpriteRotate>rotate)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            //Movement
            
            if (!jumpIsPressed && currentState == playerStates.idle && (keyboardState.IsKeyDown(Keys.W) || gamePadState.IsButtonDown(Buttons.A)))
            {
                jumpIsPressed = true;
                currentState = playerStates.jumping;
                spriteVelocity.Y -= jumpSpeed;
                jumpSound.Play();
            }
            else if (jumpIsPressed && currentState != playerStates.jumping && currentState != playerStates.falling && !(keyboardState.IsKeyDown(Keys.W) || gamePadState.IsButtonDown(Buttons.A)))
            {
                jumpIsPressed = false;
            }
            else if (currentState == playerStates.walking && keyboardState.IsKeyDown(Keys.W))
            {
                jumpIsPressed = true;
                currentState = playerStates.falling;
                spriteVelocity.Y += jumpSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if ((keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left) || gamePadState.IsButtonDown(Buttons.DPadLeft)) && currentState != playerStates.kiBlast )
            {
                currentState = playerStates.walking;
                spriteVelocity.X = -walkSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                flipped = true;


            }

            else if ((keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right) || gamePadState.IsButtonDown(Buttons.DPadRight)) && currentState != playerStates.kiBlast)
            {
                currentState = playerStates.walking;

                spriteVelocity.X = walkSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                flipped = false;



            }
            else if (currentState == playerStates.walking && keyboardState.IsKeyDown(Keys.W)) 
            {
                jumpIsPressed = true;
                currentState = playerStates.falling;
                spriteVelocity.Y += jumpSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                if (currentState == playerStates.walking) currentState = playerStates.idle;
                spriteVelocity.X = 0;

            }
            if (jumpIsPressed && currentState == playerStates.walking) 
            {
                spriteVelocity.Y += jumpSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (!jumpIsPressed && currentState == playerStates.walking)
            {
                spriteVelocity.Y += jumpSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (currentState == playerStates.idle && spriteVelocity.Y >= 1f) 
            {
                spriteVelocity.Y += jumpSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            }

            if (spritePos.Y > 810)
            {
                dead = true;
                hitSound.Play();
            }




            //Attacks
            //Blast
            if (!blastIsPressed && keyboardState.IsKeyDown(Keys.Space) || gamePadState.IsButtonDown(Buttons.RightTrigger))
            {
                currentState = playerStates.kiBlast;
                spriteVelocity.X = 0;
                blastIsPressed = true;


            }
            else if (blastIsPressed && !keyboardState.IsKeyDown(Keys.Space) && !gamePadState.IsButtonDown(Buttons.RightTrigger)) blastIsPressed = false;
                 
            //Kick
            if (!kickIsPressed && keyboardState.IsKeyDown(Keys.Q) || gamePadState.IsButtonDown(Buttons.X))
            {
                currentState = playerStates.kicking;
                kickIsPressed = true;
            }
            else if (kickIsPressed && !keyboardState.IsKeyDown(Keys.Q) && !gamePadState.IsButtonDown(Buttons.X)) kickIsPressed = false;
            //Punch
            if (!punchIsPressed && keyboardState.IsKeyDown(Keys.F) || gamePadState.IsButtonDown(Buttons.B))
            {
                currentState = playerStates.punching;
                punchIsPressed = true;
            }
            else if (punchIsPressed && !keyboardState.IsKeyDown(Keys.F) && !gamePadState.IsButtonDown(Buttons.B)) punchIsPressed = false;



            //Checking for collision
            bool hasCollided = false;

            foreach (PlatformSprite platform in platforms)
            {
                if (checkCollisionBelow(platform))
                {
                    hasCollided = true;
                    while (checkCollision(platform)) spritePos.Y--;
                    spriteVelocity.Y = 0;
                    if (currentState == playerStates.jumping || currentState == playerStates.falling) currentState = playerStates.idle;


                }
                else if (checkCollisionAbove(platform))
                {
                    hasCollided = true;
                    while (checkCollision(platform)) spritePos.Y++;
                    spriteVelocity.Y = 0;
                    currentState = playerStates.falling;

                }

                if (checkCollisionLeft(platform))
                {
                    hasCollided = true;
                    while (checkCollision(platform)) spritePos.X--;
                    spriteVelocity.X = 0;
                }
                else if (checkCollisionRight(platform))
                {
                    hasCollided = true;
                    while (checkCollision(platform)) spritePos.X++;
                    spriteVelocity.X = 0;
                }
            }

            foreach (PlatformSpriteRotate walls in rotate) 
            {
                if (checkCollisionLeft(walls))
                {
                    hasCollided = true;
                    while (checkCollision(walls)) 
                    {
                        spritePos.Y++;
                        spritePos.X--;
                    }
                    if (currentState == playerStates.jumping || currentState == playerStates.falling) currentState = playerStates.idle;
                    


                }
                if (checkCollisionRight(walls))
                {
                    hasCollided = true;
                    while (checkCollision(walls))
                    {
                        spritePos.X++;
                    }
                    if (currentState == playerStates.jumping || currentState == playerStates.falling) currentState = playerStates.idle;


                }




            }


            foreach (EnemySprite enemySprite in enemySprites) 
            {
                if(!enemySprite.dead && checkCollision(enemySprite)) 
                {

                    if (currentState == playerStates.kicking || currentState == playerStates.punching)
                    {
                        enemySprite.dead = true;
                        if (spritePos.Y < 200f) enemySprite.levelAdvance = true;
                    }
                    else
                    {
                        hasCollided = true;
                        dead = true;
                        spriteVelocity.X = 0;
                        hitSound.Play();
                    }

                }




            }




            if (currentState == playerStates.idle || currentState == playerStates.walking || currentState == playerStates.punching || currentState == playerStates.kicking) 
            {
                spritePos.Y += 5f;
                foreach (PlatformSprite platform in platforms)
                {
                    if (checkCollisionBelow(platform)) hasCollided = true;
                }
                spritePos.Y -= 5f;
            }


                if (!hasCollided && currentState != playerStates.falling && currentState != playerStates.kiBlast) currentState = playerStates.falling;
                if (currentState == playerStates.jumping && spriteVelocity.Y > 0) currentState = playerStates.falling;

                if ((currentState == playerStates.falling || currentState == playerStates.jumping || currentState == playerStates.kiBlast || currentState == playerStates.punching || currentState == playerStates.kicking || currentState == playerStates.walking) && spriteVelocity.Y < 500f) spriteVelocity.Y += 5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                spritePos += spriteVelocity;
                
                switch (currentState) 
                {
                   case playerStates.walking:
                        setAnim(2);
                        break;
                    case playerStates.jumping:
                        setAnim(3);
                        break;
                    case playerStates.falling:
                        setAnim(3);
                        break;
                    case playerStates.kiBlast:
                        setAnim(1);
                        break;
                    case playerStates.punching:
                        setAnim(5);
                        break;
                    case playerStates.kicking:
                        setAnim(4);
                        break;
                    case playerStates.idle:
                        setAnim(0);
                        break;
                    default:
                        setAnim(0);
                        break;
                }
            if (currentState == playerStates.punching || currentState == playerStates.kiBlast || currentState == playerStates.kicking)
            {
                if (currentFrame >= animations[currentAnim].Count - 1) currentState = playerStates.idle;
            }






        }

        public enum playerStates
        {
            idle, jumping, falling, walking, punching, kicking, kiBlast
        }
    }
}
