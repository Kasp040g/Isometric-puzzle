﻿namespace Isometric_test_1
{
    public class Map
    {
        //Setup basic tile and map information variables
        private Point _mapSize;
        private readonly Point _tileSize;
        private Vector2 _mapOffset = new(4.5f, 4f);
        private Tile[,] _tiles;
        private bool _shouldDrawMap = true;
        private bool _levelComplete = false;
        private bool _forest = false;


        //Mouse interaction variables
        private Tile _mouseHovered;                 //Null means none has been hovered, else stores a reference to hovered tile instance
        private Tile _mouseGrabbed;                 //Null means none has been grabbed, else stores a reference to grabbed tile instance

        //Keyboard
        private KeyboardState _currentKey;
        private KeyboardState _previousKey;

        //Text goals
        string _textGoal = "";              //The current goal as a string

        // Level States
        private Level _levels;

        public enum Level
        {
            Level1,
            Level2,
            Level3,
            Level4,
            Level5,
            Level6,
        }

        /// <summary>
        /// Map constructer to load, setup and create tiles on map
        /// </summary>
        public Map()
        {
            _levels = Level.Level1;

            //Update tile size variables
            _tileSize.X = Assets.Sprites.tileGrassBlock1.Width;
            _tileSize.Y = Assets.Sprites.tileGrassBlock1.Height / 2;
        }


        /// <summary>
        /// Converts map coordinates (the location of tiles in the map) to screen coordinates
        /// </summary>
        /// <param name="mapX"></param>
        /// <param name="mapY"></param>
        /// <returns></returns>
        public Vector2 MapToScreen(int mapX, int mapY)
        {
            var screenX = ((mapX - mapY) * _tileSize.X / 2) + (_mapOffset.X * _tileSize.X);
            var screenY = ((mapY + mapX) * _tileSize.Y / 2) + (_mapOffset.Y * _tileSize.Y);

            return new(screenX, screenY);
        }


        /// <summary>
        /// Converts a set of screen coordinates to the map coordinates (the location of tiles in the map)
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private Point ScreenToMap(Point point)
        {
            Vector2 vector = new(point.X - (int)(_mapOffset.X * _tileSize.X), point.Y - (int)(_mapOffset.Y * _tileSize.Y));

            var x = vector.X + (2 * vector.Y) - (_tileSize.X / 2);
            int mapX = (x < 0) ? -1 : (int)(x / _tileSize.X);
            var y = -vector.X + (2 * vector.Y) + (_tileSize.X / 2);
            int mapY = (y < 0) ? -1 : (int)(y / _tileSize.X);

            return new(mapX, mapY);
        }


        /// <summary>
        /// Is called every frame of the game and houses various functionalities of the map
        /// </summary>
        public void Update()
        {
            _previousKey = _currentKey;
            _currentKey = Keyboard.GetState();

            // Reset current level
            if (_currentKey.IsKeyDown(Keys.R) && _previousKey.IsKeyUp(Keys.R))
            {
                var _tempLevel = _levels;
                Assets.Audio.ResetSound.Play(0.1f, 0, 0);

                ClearLevel();
                _shouldDrawMap = true;
                _levels = _tempLevel;
            }

            // Skip current level  ***************for Debugging**************
            if (_currentKey.IsKeyDown(Keys.N) && _previousKey.IsKeyUp(Keys.N)) //  && Keyboard.GetState().IsKeyUp(Keys.N)) 
            {
                ClearLevel();
                _shouldDrawMap = true;
                _levels++;
            }

            // Skip current level  ***************for Debugging**************
            if (_currentKey.IsKeyDown(Keys.P) && _previousKey.IsKeyUp(Keys.P)) //  && Keyboard.GetState().IsKeyUp(Keys.N)) 
            {
                ClearLevel();
                _shouldDrawMap = true;
                _levels--;
            }

            if (_shouldDrawMap)
            {
                // Level state machine
                switch (_levels)
                {
                    case Level.Level1:
                        Level1();
                        break;
                    case Level.Level2:
                        Level2();
                        break;
                    case Level.Level3:
                        Level3();
                        break;
                    case Level.Level4:
                        Level4();
                        break;
                    case Level.Level5:
                        Level5();
                        break;
                    case Level.Level6:
                        Level6();
                        break;
                    default:
                        break;
                }

                _levelComplete = false;

                _shouldDrawMap = false;
            }

            #region Tile Merging

            //Checks if a tile is stored in mouse hovered and then calls for it to be unhovered if there is
            _mouseHovered?.MouseUnhovered();

            //Converts the current mouse position into map coordinates
            var mouseMap = ScreenToMap(InputManager.MousePosition);

            //Get mouse state for inputs and mouse screen coordinates
            var _mouseState = Mouse.GetState();
            var _mousePosition = new Point(_mouseState.X, _mouseState.Y);

            //Checks if the mouse coordinates are within bounds of the tile, thereby hovering it
            if (mouseMap.X >= 0 && mouseMap.Y >= 0 && mouseMap.X < _mapSize.X && mouseMap.Y < _mapSize.Y)
            {
                //Save the hovered tile
                _mouseHovered = _tiles[mouseMap.X, mouseMap.Y];

                //Tell the hovered tile that it is being hovered
                _mouseHovered.MouseHovered();
            }
            else
            {
                //No tile is being hovered
                _mouseHovered = null;
            }

            //If the player is hovering a tile and no other tile is being dragged, then they are able to drag the tile around
            if (_mouseHovered != null && _mouseGrabbed == null && _mouseState.LeftButton == ButtonState.Pressed)
            {
                //Transfers the hovered tile to grabbed
                _mouseGrabbed = _mouseHovered;

                //Tells the now grabbed tile that it is being grabbed
                _mouseGrabbed.MouseGrab();
            }

            //Release tile with mouse
            if (_mouseGrabbed != null && _mouseState.LeftButton == ButtonState.Released)
            {
                //Check if the tile is dropped on another tile
                if (_mouseHovered != null && _mouseHovered != _mouseGrabbed)
                {
                    //if (_mouseHovered._mapPosition.X < _mouseGrabbed._mapPosition.X + 1.05
                    //    && _mouseHovered._mapPosition.X > _mouseGrabbed._mapPosition.X - 1.05
                    //    && _mouseHovered._mapPosition.Y < _mouseGrabbed._mapPosition.Y + 1.05
                    //    && _mouseHovered._mapPosition.Y > _mouseGrabbed._mapPosition.Y - 1.05
                    //    )
                    var _hoveredTileVector = _mouseHovered._mapPosition.ToVector2();
                    var _grabbedTileVector = _mouseGrabbed._mapPosition.ToVector2();

                    if (Vector2.Distance(_hoveredTileVector, _grabbedTileVector) <= 1)
                    {
                        //_mouseGrabbed._texture = textures[5];
                        _mouseGrabbed.CheckTileMerge(_mouseHovered);
                    }
                }

                //Tell the grabbed tile that it is no longer grabbed
                _mouseGrabbed.MouseUngrab();

                //Reset grabbed variable
                _mouseGrabbed = null;
            }
            #endregion Tile merging 

            // check if a solution was found
            SolutionFound();
            DisplayForest();
        }


        /// <summary>
        /// Draw calls for tiles in map
        /// </summary>
        public void Draw()
        {
            //Loops through the map array and calls the individual tiles' draw method
            for (int y = 0; y < _mapSize.Y; y++)
            {
                for (int x = 0; x < _mapSize.X; x++)
                {
                    _tiles[x, y].Draw();
                }
            }
            if (_levelComplete)
            {
                //Setup text strings
                string _text1 = "Congratulations";
                string _text2 = "Press 'Space' for next level";

                //Measure the string both horizontal and vertical
                Vector2 _size1 = Globals.FontTest.MeasureString(_text1);
                Vector2 _size2 = Globals.FontTest.MeasureString(_text2);

                //Setup text shadow
                int _SO = 2;                                 //Shadow offset
                Color _SHA_COL = Color.DarkSlateBlue;        //Shadow color

                //Set text 1 positions
                Vector2 _textPosition1;
                _textPosition1.X = GameWorld.ScreenWidth / 2 - _size1.X / 2;
                _textPosition1.Y = (GameWorld.ScreenHeight / 4) * 3;

                //Set text 2 positions
                Vector2 _textPosition2;
                _textPosition2.X = GameWorld.ScreenWidth / 2 - _size2.X / 2;
                _textPosition2.Y = (GameWorld.ScreenHeight / 4) * 3;

                //Draw text shadow
                Globals.SpriteBatch.DrawString(Globals.FontTest, _text1, new Vector2(_textPosition1.X + _SO, _textPosition1.Y + _SO), _SHA_COL);
                Globals.SpriteBatch.DrawString(Globals.FontTest, _text2, new Vector2(_textPosition2.X + _SO, _textPosition2.Y + _SO + _size2.Y), _SHA_COL);

                //Draw text
                Color _TEXT_COL = Color.White;

                Globals.SpriteBatch.DrawString(Globals.FontTest, _text1, _textPosition1, _TEXT_COL);
                Globals.SpriteBatch.DrawString(Globals.FontTest, _text2, new Vector2(_textPosition2.X, _textPosition2.Y + _size2.Y), _TEXT_COL);
            }
            /*if(_forest)
            {
                Globals.SpriteBatch.DrawString(Globals.FontTest, $"      Congratulations \n you have created a forest", new Vector2(GameWorld.ScreenHeight / 2, GameWorld.ScreenWidth / 2), Color.White);
                _forest = false;
            }*/


            if (_textGoal != "")
            {
                //Text
                string _text = $"Goals:\n{_textGoal}";

                //Measure the string both horizontal and vertical
                Vector2 _size1 = Globals.FontTest.MeasureString(_textGoal);

                //Setup text shadow
                int _SO = 2;                                 //Shadow offset
                Color _SHA_COL = Color.DarkSlateBlue;        //Shadow color

                //Set text 1 positions
                Vector2 _textPosition1;
                _textPosition1.X = GameWorld.ScreenWidth / 20;
                _textPosition1.Y = GameWorld.ScreenHeight / 20;

                //Draw text shadow
                Globals.SpriteBatch.DrawString(Globals.FontTest, _text, new Vector2(_textPosition1.X + _SO, _textPosition1.Y + _SO), _SHA_COL);

                //Draw text
                Color _TEXT_COL = Color.White;

                Globals.SpriteBatch.DrawString(Globals.FontTest, _text, _textPosition1, _TEXT_COL);
            }
        }

        //Tile.WinCon = new EventHandler(solutio);

        private void SolutionFound()
        {
            // Update 
            UpdateGoalText();

            bool _spacePressed = false;

            // Check for SPACE press
            if (_currentKey.IsKeyDown(Keys.Space))
            {
                _spacePressed = true;
            }

            // Check if all goals of the current level is met
            switch (_levels)
            {
                // Level 1
                case Level.Level1:
                    if (TileTypeCount(Tile.TileTypes.tree) >= 1)
                    {
                        // Press Space to continue
                        _levelComplete = true;

                        // Go to next level
                        if (_spacePressed)
                        {     
                            _levels = Level.Level2;
                        }
                    }
                    break;

                // Level 2
                case Level.Level2:
                    if (TileTypeCount(Tile.TileTypes.tree) >= 4)
                    {
                        // Press Space to continue
                        _levelComplete = true;

                        // Go to next level
                        if (_spacePressed)
                        {
                            _levels = Level.Level3;
                        }
                    }
                    break;

                // Level 3
                case Level.Level3:
                    if (TileTypeCount(Tile.TileTypes.tree) >= 3)
                    {
                        // Press Space to continue
                        _levelComplete = true;

                        // Go to next level
                        if (_spacePressed)
                        {
                            _levels = Level.Level4;
                        }
                    }
                    break;

                // Level 4
                case Level.Level4:
                    if (TileTypeCount(Tile.TileTypes.tree) >= 5) // ****TEMP GOAL***
                    {
                        // Press Space to continue
                        _levelComplete = true;

                        // Go to next level
                        if (_spacePressed)
                        {
                            _levels = Level.Level5;
                        }
                    }
                    break;

                // Level 5
                case Level.Level5:
                    if (TileTypeCount(Tile.TileTypes.tree) >= 7) // ****TEMP GOAL***
                    {
                        // Press Space to continue
                        _levelComplete = true;

                        // Go to next level
                        if (_spacePressed)
                        {
                            _levels = Level.Level6;
                        }
                    }
                    break;

                // Level 6
                case Level.Level6:
                    if (TileTypeCount(Tile.TileTypes.tree) >= 20) // ****TEMP GOAL***
                    {
                        // Press Space to continue
                        _levelComplete = true;
                    }
                    break;

                // Default
                default:
                    break;
            }

            // Check if level is complete and the player can progress to the next scene
            if (_levelComplete == true)
            {
                // Check for SPACE key press
                if (_spacePressed)
                {
                    // Clears the level of tiles
                    ClearLevel();

                    // Play win sound
                    Assets.Audio.WinSound.Play();

                    //Now draw map again
                    _shouldDrawMap = true;
                }
            }
        }


        #region Levels

        //private void TempLevel()
        //{
        //    _tiles[0, 0] = new( new Point(0, 0), MapToScreen(0, 0), Tile.TileTypes.grass);
        //    _tiles[0, 1] = new( new Point(0, 1), MapToScreen(0, 1), Tile.TileTypes.grass);
        //    _tiles[0, 2] = new( new Point(0, 2), MapToScreen(0, 2), Tile.TileTypes.grass);
        //    _tiles[1, 0] = new( new Point(1, 0), MapToScreen(1, 0), Tile.TileTypes.grass);
        //    _tiles[1, 1] = new( new Point(1, 1), MapToScreen(1, 1), Tile.TileTypes.grass);
        //    _tiles[1, 2] = new( new Point(1, 2), MapToScreen(1, 2), Tile.TileTypes.grass);
        //    _tiles[2, 0] = new( new Point(2, 0), MapToScreen(2, 0), Tile.TileTypes.grass);
        //    _tiles[2, 1] = new( new Point(2, 1), MapToScreen(2, 1), Tile.TileTypes.grass);
        //    _tiles[2, 2] = new( new Point(2, 2), MapToScreen(2, 2), Tile.TileTypes.grass);
        //}

        private void ClearLevel()
        {

            _tiles[0, 0] = new(new Point(0, 0), Tile.TileTypes.empty);
            _tiles[0, 1] = new(new Point(0, 1), Tile.TileTypes.empty);
            _tiles[1, 0] = new(new Point(1, 0), Tile.TileTypes.empty);
            _tiles[1, 1] = new(new Point(1, 1), Tile.TileTypes.empty);
        }

        private void Level1()
        {
            _mapSize = new(2, 2);

            //Create tile array from map size
            _tiles = new Tile[_mapSize.X, _mapSize.Y];

            // Level 1
            _tiles[0, 0] = new(new Point(0, 0), Tile.TileTypes.bush);
            _tiles[0, 1] = new(new Point(0, 1), Tile.TileTypes.grass);

            _tiles[1, 0] = new(new Point(1, 0), Tile.TileTypes.empty);
            _tiles[1, 1] = new(new Point(1, 1), Tile.TileTypes.grass);

        }

        private void Level2()
        {
            _mapSize = new(3, 3);

            //Create tile array from map size
            _tiles = new Tile[_mapSize.X, _mapSize.Y];

            _tiles[0, 0] = new(new Point(0, 0), Tile.TileTypes.grass);
            _tiles[0, 1] = new(new Point(0, 1), Tile.TileTypes.grass);
            _tiles[0, 2] = new(new Point(0, 2), Tile.TileTypes.grass);

            _tiles[1, 0] = new(new Point(1, 0), Tile.TileTypes.grass);
            _tiles[1, 1] = new(new Point(1, 1), Tile.TileTypes.grass);
            _tiles[1, 2] = new(new Point(1, 2), Tile.TileTypes.grass);

            _tiles[2, 0] = new(new Point(2, 0), Tile.TileTypes.grass);
            _tiles[2, 1] = new(new Point(2, 1), Tile.TileTypes.grass);
            _tiles[2, 2] = new(new Point(2, 2), Tile.TileTypes.grass);
        }

        private void Level3() //Goal 10 træer
        {
            _mapSize = new(5, 5);
            _mapOffset = new(4.5f, 3f);

            //Create tile array from map size
            _tiles = new Tile[_mapSize.X, _mapSize.Y];

            _tiles[0, 0] = new(new Point(0, 0), Tile.TileTypes.empty);
            _tiles[0, 1] = new(new Point(0, 1), Tile.TileTypes.grass);
            _tiles[0, 2] = new(new Point(0, 2), Tile.TileTypes.grass);
            _tiles[0, 3] = new(new Point(0, 3), Tile.TileTypes.grass);
            _tiles[0, 4] = new(new Point(0, 4), Tile.TileTypes.empty);


            _tiles[1, 0] = new(new Point(1, 0), Tile.TileTypes.empty);
            _tiles[1, 1] = new(new Point(1, 1), Tile.TileTypes.grass);
            _tiles[1, 2] = new(new Point(1, 2), Tile.TileTypes.grass);
            _tiles[1, 3] = new(new Point(1, 3), Tile.TileTypes.grass);
            _tiles[1, 4] = new(new Point(1, 4), Tile.TileTypes.grass);


            _tiles[2, 0] = new(new Point(2, 0), Tile.TileTypes.grass);
            _tiles[2, 1] = new(new Point(2, 1), Tile.TileTypes.grass);
            _tiles[2, 2] = new(new Point(2, 2), Tile.TileTypes.grass);
            _tiles[2, 3] = new(new Point(2, 3), Tile.TileTypes.grass);
            _tiles[2, 4] = new(new Point(2, 4), Tile.TileTypes.grass);


            _tiles[3, 0] = new(new Point(3, 0), Tile.TileTypes.grass);
            _tiles[3, 1] = new(new Point(3, 1), Tile.TileTypes.grass);
            _tiles[3, 2] = new(new Point(3, 2), Tile.TileTypes.grass);
            _tiles[3, 3] = new(new Point(3, 3), Tile.TileTypes.grass);
            _tiles[3, 4] = new(new Point(3, 4), Tile.TileTypes.empty);


            _tiles[4, 0] = new(new Point(4, 0), Tile.TileTypes.empty);
            _tiles[4, 1] = new(new Point(4, 1), Tile.TileTypes.grass);
            _tiles[4, 2] = new(new Point(4, 2), Tile.TileTypes.grass);
            _tiles[4, 3] = new(new Point(4, 3), Tile.TileTypes.grass);
            _tiles[4, 4] = new(new Point(4, 4), Tile.TileTypes.empty);
        }

        private void Level4()
        {
            _mapSize = new(5, 2);
            _mapOffset = new(3.75f, 3f);

            //Create tile array from map size
            _tiles = new Tile[_mapSize.X, _mapSize.Y];

            _tiles[0, 0] = new(new Point(0, 0), Tile.TileTypes.empty);
            _tiles[0, 1] = new(new Point(0, 1), Tile.TileTypes.grass);

            _tiles[1, 0] = new(new Point(1, 0), Tile.TileTypes.grass);
            _tiles[1, 1] = new(new Point(1, 1), Tile.TileTypes.grass);

            _tiles[2, 0] = new(new Point(2, 0), Tile.TileTypes.tree);
            _tiles[2, 1] = new(new Point(2, 1), Tile.TileTypes.bush);

            _tiles[3, 0] = new(new Point(3, 0), Tile.TileTypes.grass);
            _tiles[3, 1] = new(new Point(3, 1), Tile.TileTypes.grass);

            _tiles[4, 0] = new(new Point(4, 0), Tile.TileTypes.empty);
            _tiles[4, 1] = new(new Point(4, 1), Tile.TileTypes.grass);

        }

        private void Level5()
        {
            _mapSize = new(4, 7);
            _mapOffset = new(5f, 3f);

            //Create tile array from map size
            _tiles = new Tile[_mapSize.X, _mapSize.Y];

            _tiles[0, 0] = new(new Point(0, 0), Tile.TileTypes.empty);
            _tiles[0, 1] = new(new Point(0, 1), Tile.TileTypes.empty);
            _tiles[0, 2] = new(new Point(0, 2), Tile.TileTypes.empty);
            _tiles[0, 3] = new(new Point(0, 3), Tile.TileTypes.empty);
            _tiles[0, 4] = new(new Point(0, 4), Tile.TileTypes.bush);
            _tiles[0, 5] = new(new Point(0, 5), Tile.TileTypes.bush);
            _tiles[0, 6] = new(new Point(0, 6), Tile.TileTypes.grass);

            _tiles[1, 0] = new(new Point(1, 0), Tile.TileTypes.bush);
            _tiles[1, 1] = new(new Point(1, 1), Tile.TileTypes.bush);
            _tiles[1, 2] = new(new Point(1, 2), Tile.TileTypes.empty);
            _tiles[1, 3] = new(new Point(1, 3), Tile.TileTypes.empty);
            _tiles[1, 4] = new(new Point(1, 4), Tile.TileTypes.empty);
            _tiles[1, 5] = new(new Point(1, 5), Tile.TileTypes.grass);
            _tiles[1, 6] = new(new Point(1, 6), Tile.TileTypes.grass);

            _tiles[2, 0] = new(new Point(2, 0), Tile.TileTypes.grass);
            _tiles[2, 1] = new(new Point(2, 1), Tile.TileTypes.tree);
            _tiles[2, 2] = new(new Point(2, 2), Tile.TileTypes.empty);
            _tiles[2, 3] = new(new Point(2, 3), Tile.TileTypes.empty);
            _tiles[2, 4] = new(new Point(2, 4), Tile.TileTypes.empty);
            _tiles[2, 5] = new(new Point(2, 5), Tile.TileTypes.empty);
            _tiles[2, 6] = new(new Point(2, 6), Tile.TileTypes.bush);

            _tiles[3, 0] = new(new Point(3, 0), Tile.TileTypes.grass);
            _tiles[3, 1] = new(new Point(3, 1), Tile.TileTypes.empty);
            _tiles[3, 2] = new(new Point(3, 2), Tile.TileTypes.empty);
            _tiles[3, 3] = new(new Point(3, 3), Tile.TileTypes.empty);
            _tiles[3, 4] = new(new Point(3, 4), Tile.TileTypes.empty);
            _tiles[3, 5] = new(new Point(3, 5), Tile.TileTypes.empty);
            _tiles[3, 6] = new(new Point(3, 6), Tile.TileTypes.empty);

        }



        /// <summary>
        /// Level 6 is a for loop that makes a 10x10 map, allowing the player to play around and see how many trees they can get.
        /// The reason that this is a loop, ane the previous levels arent, is because the other level is made of different tiletypes, such as empty, grass, bush and tree.
        /// whereas level 6 only consists of 1 type, which makes it easy to make in a loop.
        /// </summary>
        private void Level6()
        {

            _mapSize = new(10, 10);
            _tiles = new Tile[_mapSize.X, _mapSize.Y];
            _mapOffset = new(4.5f, 0.1f);

            for (int y = 0; y < _mapSize.Y; y++)
            {
                for (int x = 0; x < _mapSize.X; x++)
                {
                    _tiles[y, x] = new(new Point(y, x), Tile.TileTypes.grass);

                }
            }
        }
        #endregion Levels
        /// <summary>
        /// Loops through the tile map and returns the amount/count of how many of the provided tiletype were found
        /// </summary>
        /// <param name="tileType"></param>
        /// <returns></returns>
        public int TileTypeCount(Tile.TileTypes tileType)
        {
            //Keep track of how many of the specific tile type was found
            int _count = 0;

            //Loop through tile map and count how many of the specific tile type were found
            for (int x = 0; x < _tiles.GetLength(0); x++)
            {
                for (int y = 0; y < _tiles.GetLength(1); y++)
                {
                    if (_tiles[x, y]._tileType == tileType)
                    {
                        _count++;
                    }
                }
            }

            //Return the amount of tiles of the specific type that was found
            return _count;
        }

        public void DisplayForest()
        {
            if (TileTypeCount(Tile.TileTypes.tree) <= 4)
            {
                for (int x = 0; x < _tiles.GetLength(0) - 1; x++)
                {
                    for (int y = 0; y < _tiles.GetLength(1) - 1; y++)
                    {
                        if (_tiles[x, y]._tileType == Tile.TileTypes.tree)
                        {
                            if (_tiles[x, y + 1]._tileType == Tile.TileTypes.tree)
                            {
                                if (_tiles[x + 1, y]._tileType == Tile.TileTypes.tree)
                                {
                                    if (_tiles[x + 1, y + 1]._tileType == Tile.TileTypes.tree)
                                    {
                                        // CHANGE ABOVE TILE DISPLAY
                                        var _forestTile = _tiles[x + 1, y + 1];

                                        // Update tiles to forest type
                                        _forestTile._tileType = Tile.TileTypes.forest;
                                        _tiles[x, y + 1]._tileType = Tile.TileTypes.forest;
                                        _tiles[x + 1, y]._tileType = Tile.TileTypes.forest;
                                        _tiles[x, y]._tileType = Tile.TileTypes.forest;

                                        // Update tiles to forest sprite
                                        _forestTile._tileObjectSprite = Assets.Sprites.forest;
                                        _tiles[x, y + 1]._tileObjectSprite = null;
                                        _tiles[x + 1, y]._tileObjectSprite = null;
                                        _tiles[x, y]._tileObjectSprite = null;

                                        // update forest sprite offset
                                        _forestTile._tileObjectOffset.X = -40;
                                        _forestTile._tileObjectOffset.Y = -200;

                                        //START HOVERING BIRDS ANIMATION


                                        //forest = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Updates the text that is displayed to track level goals
        /// </summary>
        public void UpdateGoalText()
        {
            // Won't be updated if level is complete
            if (_levelComplete == false)
            {
                switch (_levels)
                {
                    // Level 1
                    case Level.Level1:
                        _textGoal = $"{TileTypeCount(Tile.TileTypes.tree)} / 1 tree";
                        break;

                    // Level 2
                    case Level.Level2:
                        _textGoal = $"{TileTypeCount(Tile.TileTypes.tree)} / 4 tree";
                        break;

                    // Default
                    default:
                        _textGoal = "";
                        break;
                }
            }
        }
    }
}