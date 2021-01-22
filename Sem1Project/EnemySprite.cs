using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace Sem1Project
{
    class EnemySprite : Sprite
    {
        public bool hasCollided = false;
       public bool dead = false; //for when the enemy has been hit and declared as "dead"
        public bool levelAdvance = false; //Indicating the level advancement
        SoundEffect advanceSound, deathSound; //sounds for death/advancing
        public EnemySprite(Texture2D newSpriteSheet, Texture2D newCollisionTxr, Vector2 newLocation, SoundEffect newAdvanceSound, SoundEffect newDeathSound) : base(newSpriteSheet, newCollisionTxr, newLocation)
        {
            advanceSound = newAdvanceSound; // declaring and pulling the sound effect from Game1 
            deathSound = newDeathSound;  // declaring and pulling the sound effect from Game1 
            spriteOrigin = new Vector2(0.5f, 1f); //controlling starting point of what part of the image is being used
            spriteScale = new Vector2(2, 2); // controlling the size of the sprite
            isColliding = true; //collision has became true
            flipped = true; // controlling which way the sprite is facing
            //Setting collision values
            collisionInsetMin = new Vector2(0.25f, 0.3f);
            collisionInsetMax = new Vector2(0.25f, 0.03f);
            //controlling the animations for enemy/setting up the image/animation to be drawn
            animations = new List<List<Rectangle>>();

            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(10, 5, 48, 50));
        }

        public void Update(GameTime gameTime, List<KiBlastSprite> blastSprites, PlayerSprite playerSprite)
        {
            if (spritePos.Y > 120) spriteScale = new Vector2(1.5f, 1.5f); //controlling sprite size of enemies that are on the lower part of the screen 
            foreach (KiBlastSprite blastSprite in blastSprites)
            {
                if (!dead && checkCollision(blastSprite))       //checking for collision from another sprite
                {
                    hasCollided = true; //declaring collision
                    dead = true; //declaring death
                    deathSound.Play(); //playing the death sound effect
                }
                if (dead && spritePos.Y < 110f) //if death has been declared and the sprite is less than 110 frames down
                {
                    levelAdvance = true; //declaring the level has advanced
                    advanceSound.Play(); //playing the level advance sound effect
                }
            }
        }
    }
}
