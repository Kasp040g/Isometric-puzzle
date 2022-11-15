﻿
namespace Isometric_test_1;

public class Tile
{
    private readonly Texture2D _texture;
    private readonly Vector2 _position;
    private bool _mouseSelected;

    public Tile(Texture2D texture, Vector2 position)
    {
        _texture = texture;
        _position = position;
    }

    public void MouseSelect()
    {
        _mouseSelected = true;
    }

    public void MouseDeselect()
    {
        _mouseSelected = false;
    }

    public void Draw()
    {
        var color = Color.White;
        if (_mouseSelected) color = Color.LightSlateGray;
        Globals.SpriteBatch.Draw(_texture, _position, color);
    }
}
