﻿using System.Collections.Generic;

namespace TileLands
{
    /// <summary>
    /// Scrolling background is called in Gamemanager
    /// </summary>
    public class ScrollingBackground : Component
    {
        #region Fields
        // if true, the object will always be moving.
        private bool _constantSpeed;
                
        private float _layer;
        public float Layer;

        //used in the _speed calculation
        private float _scrollingSpeed;
        private float _speed;

        private List<Assets> _sprites;
        #endregion Fields

        #region Constructors
        /// <summary>
        ///  constructor that uses the constructor underneath, to update appropiately, so you can pass in multiple sprites.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="scrollingSpeed"></param>
        /// <param name="constantSpeed"></param>

        public ScrollingBackground(Texture2D texture, float scrollingSpeed, bool constantSpeed = false)
            : this(new List<Texture2D>() { texture, texture }, scrollingSpeed, constantSpeed)
        {

        }

        /// <summary>
        ///  Cronstructer where you only need to pass in one sprite
        /// </summary>
        /// <param name="textures"></param>
        /// <param name="scrollingSpeed"></param>
        /// <param name="constantSpeed"></param>
        public ScrollingBackground(List<Texture2D> textures, float scrollingSpeed, bool constantSpeed = false)
        {
            _scrollingSpeed = scrollingSpeed;

            _constantSpeed = constantSpeed;

            _sprites = new List<Assets>();

            for (int i = 0; i < textures.Count; i++)
            {
                var texture = textures[i];
                _sprites.Add(new Assets(texture)
                {
                    Position = new Vector2((i * texture.Width) - 1, GameWorld.ScreenHeight - texture.Height) //Keeps the image locked on the bottom of the screen
                });
            }
        }
        #endregion Constructors

        #region Methods
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var sprite in _sprites)
                sprite.Draw(gameTime, spriteBatch);            
        }

        public override void Update(GameTime gameTime)
        {
            ApplySpeed(gameTime);
            CheckPosition();
        }

        /// <summary>
        /// applies the speed to the sprites
        /// </summary>
        /// <param name="gameTime"></param>
        private void ApplySpeed(GameTime gameTime)
        {
            _speed = (float)(_scrollingSpeed * gameTime.ElapsedGameTime.TotalSeconds); 

            foreach (var sprite in _sprites)
                sprite.Position.X -= _speed; 
        }

        /// <summary>
        /// fatter ikke helt
        /// </summary>
        private void CheckPosition() 
        {
            for (int i = 0; i < _sprites.Count; i++)
            {
                var sprite = _sprites[i];

                if (sprite.Rectangle.Right <= 0)
                {
                    var index = i - 1;

                    if (index < 0)
                        index = _sprites.Count - 1;

                    sprite.Position.X = _sprites[index].Rectangle.Right - (_speed * 2); //the (speed * 2) prevents white lines between the sprites.
                }
            }
        }
        #endregion Methods
    }
}