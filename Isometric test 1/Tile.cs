﻿
using Microsoft.VisualBasic;

namespace Isometric_test_1;

public class Tile
{
    //Tile visuals variables
    public Texture2D _texture;
    public readonly Point _mapPosition;
    public readonly Vector2 _coordinates;

    //Mouse interaction variables
    private bool _mouseHovered;
    private bool _mouseGrabbed;


    /// <summary>
    /// An enum state machine that stores the current type of the tile
    /// </summary>
    public enum TileTypes
    {
        grass,
        bush,
        tree,
    }

    private TileTypes _tileType;


    /// <summary>
    /// Tile constructer, used to instantiate a tile of a specific type
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="position"></param>
    /// <param name="coordinates"></param>
    /// <param name="tileType"></param>
    public Tile(Texture2D texture, Point position, Vector2 coordinates, TileTypes tileType)
    {
        _texture = texture;
        _mapPosition = position;
        _coordinates = coordinates;
        _tileType = tileType;
    }


    /// <summary>
    /// Tells the tile it is being hovered by the mouse
    /// </summary>
    public void MouseHovered()
    {
        _mouseHovered = true;
    }


    /// <summary>
    /// Tells the tile it is no longer hovered by the mouse
    /// </summary>
    public void MouseUnhovered()
    {
        _mouseHovered = false;
    }


    /// <summary>
    /// Tells the tile that it is now grabbed by the player
    /// </summary>
    public void MouseGrab()
    {
        _mouseGrabbed = true;
    }


    /// <summary>
    /// Tells the tile that it is no longer grabbed by the player
    /// </summary>
    public void MouseUngrab()
    {
        _mouseGrabbed = false;
    }


    /// <summary>
    /// Checks whether or not two tiles can be merged, if they can, give the resulting tile
    /// </summary>
    /// <param name="hoveredTile"></param>
    /// <param name="grabbedTile"></param>
    public void CheckTileMerge(Tile hoveredTile, Tile grabbedTile)
    {
        //First check if merge is possible
        //Return the index in array if one is found
        //Merge them


    }


    /// <summary>
    /// Draws the tile every frame depending on tile state
    /// </summary>
    public void Draw()
    {
        var color = Color.White;
        if (_mouseHovered) color = Color.LightSlateGray;
        if (_mouseGrabbed) color = Color.Red;
        Globals.SpriteBatch.Draw(_texture, _coordinates, color);
    }
}