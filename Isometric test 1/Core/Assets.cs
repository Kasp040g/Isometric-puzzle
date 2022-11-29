﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using SharpDX.Direct3D9;
using Microsoft.Xna.Framework.Media;

namespace Isometric_test_1
{
    public class Assets : Component
    {
        protected float _layer { get; set; }

        protected Texture2D _texture;


        public readonly struct Sprites
        {
            // Tile struct members
            public static Texture2D tileGrassBlock1 = Globals.Content.Load<Texture2D>("tile0");
            public static Texture2D tileGrassBlock2 = Globals.Content.Load<Texture2D>("tile1");
            public static Texture2D tileGrassBlock3 = Globals.Content.Load<Texture2D>("tile2");
            public static Texture2D tileGrassBlock4 = Globals.Content.Load<Texture2D>("tile3");
            public static Texture2D tileEmpty = Globals.Content.Load<Texture2D>("tile5");
        }

        public readonly struct Audio
        {
            // Audio struct members
            public static SoundEffect MergeSound;
            public static SoundEffect WinSound;
            public static SoundEffect ResetSound;
          
            public static Song BackgroundMusic;

            


            // Method to load audio files and assign them to the struct members
            public static void LoadAudio()
            {
                MergeSound = Globals.Content.Load<SoundEffect>("Audio/Pop_sound_5");
                
                WinSound = Globals.Content.Load<SoundEffect>("Audio/WinSound");
                ResetSound = Globals.Content.Load<SoundEffect>("Audio/ResetSound");

                BackgroundMusic = Globals.Content.Load<Song>("Audio/lunar lounging_mp3");
               
            }
        }

        public float Layer
        {
            get { return _layer; }
            set
            {
                _layer = value;
            }
        }

        public Vector2 Position;

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
            }
        }
        public Assets(Texture2D texture)
        {
            _texture = texture;
        }
        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(_texture, Position, null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, Layer);
        }


    }
}
