using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class SpriteUtils
    {
        public static bool IsTransparent(this Sprite sprite)
        {
            for (var y = sprite.rect.y; y < sprite.rect.yMax; y++)
            {
                for (var x = sprite.rect.x; x < sprite.rect.xMax; x++)
                {
                    var alpha = sprite.texture.GetPixel((int) x, (int) y).a;
                    if (alpha > 0)
                        return false;
                }
            }

            return true;
        }

        public static Sprite Mirror(this Sprite sprite)
        {
            //TODO could possibly be replaced
            var width = (int)sprite.rect.width;
            var mirrored = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, sprite.texture.format, false);
            mirrored.filterMode = FilterMode.Point;
            for (var y = 0; y < sprite.rect.height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var color = sprite.texture.GetPixel((int) sprite.rect.x + x, (int) sprite.rect.y + y);
                    mirrored.SetPixel(width - x - 1, y, color);
                }
            }
            mirrored.Apply();

            var rect = new Rect(0, 0, sprite.rect.width, sprite.rect.height);
            var pivot = Vector2.right - sprite.pivot / sprite.rect.width;
            var mirroredSprite = Sprite.Create(mirrored, rect, pivot, 8);
            return mirroredSprite;
        }
        
        public static List<Sprite> CreateSprites(Texture2D texture, Rect targetRect, int imageWidth, int imageHeight)
        {
            List<Sprite> images = new List<Sprite>();
            for (var y = targetRect.y - imageHeight; y >= targetRect.y - targetRect.height; y -= imageHeight)
            {
                for (var x = targetRect.x; x < targetRect.x + targetRect.width; x += imageWidth)
                {
                    var rect = new Rect(x, y, imageWidth, imageHeight);
                    var pivot = new Vector2(0.5f, 0);
                    var sprite = Sprite.Create(texture, rect, pivot, 8);

                    images.Add(sprite);
                }
            }

            return images;
        }
        
        public static Sprite MergeSprites(Sprite first, Sprite second)
        {
            var rect = first.rect;
            rect.width += second.rect.width;
            var pivot = new Vector2(0.25f, 0);
            return Sprite.Create(first.texture, rect, pivot, 8);
        }
    }
}