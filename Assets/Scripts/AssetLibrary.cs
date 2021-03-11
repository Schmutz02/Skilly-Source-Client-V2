using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using Utils;

public static class AssetLibrary
{
    private static readonly Dictionary<string, List<CharacterAnimation>> _Animations =
        new Dictionary<string, List<CharacterAnimation>>();
    
    private static readonly Dictionary<string, List<Sprite>> _Images = new Dictionary<string, List<Sprite>>();
    
    public static void AddAnimations(Texture2D texture, SpriteSheetData data)
    {
        if (!_Animations.ContainsKey(data.Id))
            _Animations[data.Id] = new List<CharacterAnimation>();

        for (var y = texture.height; y >= 0; y -= data.AnimationHeight)
        {
            for (var x = 0; x < data.AnimationWidth; x += data.AnimationWidth)
            {
                var rect = new Rect(x, y, data.AnimationWidth, data.AnimationHeight);
                var animation = new CharacterAnimation(texture, rect, data.ImageWidth, data.ImageHeight, data.StartDirection);
                
                _Animations[data.Id].Add(animation);
            }
        }
    }

    public static void AddImages(Texture2D texture, SpriteSheetData data)
    {
        if (!_Images.ContainsKey(data.Id))
            _Images[data.Id] = new List<Sprite>();
        
        for (var y = texture.height - data.ImageHeight; y >= 0; y -= data.ImageHeight)
        {
            for (var x = 0; x < texture.width; x += data.ImageWidth)
            {
                var spriteRect = new Rect(x, y, data.ImageWidth, data.ImageHeight);
                var pivot = new Vector2(0.5f, 0);
                var sprite = Sprite.Create(texture, spriteRect, pivot, 8);
                
                _Images[data.Id].Add(sprite);
            }
        }
    }

    public static Sprite GetImage(string sheetName, int x, int y)
    {
        return _Images[sheetName][y * 16 + x];
    }

    public static Sprite GetAnimationFrame(string sheetName, int index, Direction direction, Action action, int frame)
    {
        return _Animations[sheetName][index].GetImage(direction, action, frame);
    }
}

public readonly struct SpriteSheetData
{
    public readonly string Id;
    public readonly string SheetName;
    public readonly int AnimationWidth;
    public readonly int AnimationHeight;
    public readonly Direction StartDirection;
    public readonly int ImageWidth;
    public readonly int ImageHeight;

    public SpriteSheetData(XElement xml)
    {
        Id = xml.ParseString("@id");
        SheetName = xml.ParseString("@name", Id);

        var animationSize = xml.ParseIntArray("AnimationSize", "x", new [] {0, 0});
        AnimationWidth = animationSize[0];
        AnimationHeight = animationSize[1];
        StartDirection = xml.ParseEnum("StartDirection", Direction.Right);

        var imageSize = xml.ParseIntArray("ImageSize", "x", new [] {0, 0});
        ImageWidth = imageSize[0];
        ImageHeight = imageSize[1];
    }

    public bool IsAnimation()
    {
        return AnimationWidth != 0 && AnimationHeight != 0;
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}