﻿
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace TileLands
{
    public class GameManager
    {
        // Init States
        private State _state;

        // Instantiate DebugManager
        private DebugManager _debugManager = new();

        // Save/Load
        private ScoreManager _sm;
        private const string _savePath = "testScore.json";        
        private bool _saveFileCreated;

        //Background
        public static List<ScrollingBackground> _scrollingBackgrounds;



        //Animations
        //private Eagle _bird_ss = new(new(Globals.Bounds.Y / 2, 100));


        // Music bool
        private bool _musicIsPaused;
        

        public GameManager()
        {
            // Init
            StateManager.Init(this);

            // state
            ChangeState(ScreenStates.Splash);
        }

        public void Init()
        {
            //Load audio files
            Assets.Audio.LoadAudio();

            //Plays and repeats the background music
            MediaPlayer.Play(Assets.Audio.BackgroundMusic);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 1.2f;

            // sound effect not muted
            Globals._soundEffectsMuted = false;

            _sm = new()
            {                
                Level1Done = false,
                Level2Done = false,
                Level3Done = false,
                Level4Done = false,
                Level5Done = false,
                EndlessUnlocked = false,
                Score = 1000,
            };

            // load
            if(_saveFileCreated)
            {
                _sm = Load();
                Trace.WriteLine($"{_sm.Level1Done} {_sm.Level2Done} {_sm.Level3Done} {_sm.Level4Done} {_sm.Level5Done} {_sm.EndlessUnlocked} {_sm.Score}");
            }

            // save
            Save(_sm);


            //Loads the List of Scrolling backgrounds, and gives them their speed values and layer value
            _scrollingBackgrounds = new List<ScrollingBackground>()
            {
                new ScrollingBackground(Assets.Sprites.cloudsFast, 18f, true)
                {
                  Layer = 0.99f,
                },

                new ScrollingBackground(Assets.Sprites.cloudsSlow, 25f, true)
                {
                  Layer = 0.8f,
                }
            };
        }

        public void ChangeState(ScreenStates state)
        {
            _state = StateManager.States[state];
        }

        public void Update(GameTime gameTime)
        {
            InputManager.Update();
            _debugManager.Update(gameTime);            
            _state.Update(this);

            //loops the backgrounds
            foreach (var sb in _scrollingBackgrounds)
            {
                sb.Update(gameTime);
            }

        }

        public void Draw(GameTime gameTime)
        {
            foreach (var sb in _scrollingBackgrounds)
            {
                sb.Draw(gameTime, Globals.SpriteBatch);
            }

            _debugManager.Draw();
            _state.Draw(this);

           


        }

        #region Button Methods
        public void Play(object sender, EventArgs e)
        {
            ChangeState(ScreenStates.Game);
        }

        public void Quit(object sender, EventArgs e)
        {

        }

        public void ToggleMusic(object sender, EventArgs e)
        {            
            if(!_musicIsPaused)
            {
                //MediaPlayer.Play();
                
                MediaPlayer.Volume = 0f;
                _musicIsPaused = true;
            }
            else
            {
                //MediaPlayer.Pause();
                
                MediaPlayer.Volume = 1.2f;
                _musicIsPaused = false;
            }
        }

        public void ToggleSoundEffect(object sender, EventArgs e)
        {
            if(Globals._soundEffectsMuted)            
                Globals._soundEffectsMuted = false;            
            else            
                Globals._soundEffectsMuted = true;
            
            Console.WriteLine(Globals._soundEffectsMuted);
        }

        //public void ResetLevel(object sender, EventArgs e)
        //{

        //}
        #endregion Button Methods

        private void Save(ScoreManager sm)
        {
            string SaveThis = JsonSerializer.Serialize<ScoreManager>(sm);
            Trace.WriteLine(SaveThis);
            File.WriteAllText(_savePath, SaveThis);
            // File.AppendAllLines ?? for adding data??
            _saveFileCreated = true;
        }

        private ScoreManager Load()
        {
            var loadedData = File.ReadAllText(_savePath);  // read alllines output is a string array readall text is just a string
            return JsonSerializer.Deserialize<ScoreManager>(loadedData);
        }
    }
}

