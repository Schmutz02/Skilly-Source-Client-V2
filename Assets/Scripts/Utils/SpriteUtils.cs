using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class SpriteUtils
    {
        private static readonly Dictionary<Sprite, Dictionary<int, Sprite>> RedrawCache =
            new Dictionary<Sprite, Dictionary<int, Sprite>>();

        private static readonly Dictionary<string, Dictionary<int, Texture>> TextureCache =
            new Dictionary<string, Dictionary<int, Texture>>();
        
        public static Sprite Redraw(Sprite sprite, int size, float multiplier = 5)
        {
            var hash = GetHash(size, multiplier);
            if (IsCached(sprite, hash))
                return RedrawCache[sprite][hash];

            var scaledImage = TextureScaler.GetScaled(sprite, size, multiplier);
            Cache(sprite, hash, scaledImage);
            return scaledImage;
        }

        private static bool IsCached(Sprite sprite, int hash)
        {
            return RedrawCache.ContainsKey(sprite) && RedrawCache[sprite].ContainsKey(hash);
        }

        private static void Cache(Sprite original, int hash, Sprite modified)
        {
            if (!RedrawCache.ContainsKey(original))
                RedrawCache[original] = new Dictionary<int, Sprite>();

            RedrawCache[original][hash] = modified;
        }

        private static int GetHash(int size, float multiplier)
        {
            return (int)(size * multiplier);
        }

        public static Texture2D CreateTexture(Sprite sprite)
        {
            var smallTex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, sprite.texture.format, false);
            smallTex.filterMode = FilterMode.Point;
            for (var y = 0; y < sprite.rect.height; y++)
            {
                for (var x = 0; x < sprite.rect.width; x++)
                {
                    var color = sprite.texture.GetPixel((int) sprite.rect.x + x, (int) sprite.rect.y + y);
                    smallTex.SetPixel(x, y, color);
                }
            }
            smallTex.Apply();
            return smallTex;
        }
        
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
    
    public static class TextureScaler
    {
        /// <summary>
        /// Returns a scaled copy of given texture. 
        /// </summary>
        /// <param name="tex">Source texure to scale</param>
        /// <param name="width">Destination texture width</param>
        /// <param name="height">Destination texture height</param>
        /// <param name="mode">Filtering mode</param>
        public static Texture2D Scaled(Texture2D src, int width, int height, FilterMode mode = FilterMode.Trilinear)
        {
            var texR = new Rect(0, 0, width, height);
            GpuScale(src, width, height, mode);

            //Get rendered data back to a new texture
            var result = new Texture2D(width, height, TextureFormat.ARGB32, true);
            result.Resize(width, height);
            result.ReadPixels(texR, 0, 0, true);
            return result;
        }

        /// <summary>
        /// Scales the texture data of the given texture.
        /// </summary>
        /// <param name="tex">Texure to scale</param>
        /// <param name="width">New width</param>
        /// <param name="height">New height</param>
        /// <param name="mode">Filtering mode</param>
        public static void Scale(Texture2D tex, int width, int height, FilterMode mode = FilterMode.Trilinear)
        {
            var texR = new Rect(0, 0, width, height);
            GpuScale(tex, width, height, mode);

            // Update new texture
            tex.Resize(width, height);
            tex.ReadPixels(texR, 0, 0, true);
            tex.Apply(true); //Remove this if you hate us applying textures for you :)
        }

        public static Sprite GetScaled(Sprite sprite, int size, float multiplier)
        {
            var rawTexture = sprite.texture;
            var smallTexture = new Texture2D((int)sprite.rect.width + 2, (int)sprite.rect.height + 2, TextureFormat.ARGB32, true);
            var rawPixels = rawTexture.GetPixels((int)sprite.rect.x, (int)sprite.rect.y, (int)sprite.rect.width, (int)sprite.rect.height);
            
            for (var i = 0; i < smallTexture.height; i++)
            {
                for (var j = 0; j < smallTexture.width; j++)
                {
                    smallTexture.SetPixel(j, i, Color.clear);
                }
            }
            
            smallTexture.SetPixels(1, 1, (int)sprite.rect.width, (int)sprite.rect.height, rawPixels);
            smallTexture.Apply(true);
            
            var w = (int)(multiplier * size / 100 * smallTexture.width);
            var h = (int)(multiplier * size / 100 * smallTexture.height);
            var pivot = new Vector2(((sprite.pivot.x + 1) / smallTexture.width), multiplier / h);
            Scale(smallTexture, w, h, FilterMode.Point);

            var rect = new Rect(0, 0, w, h);
            var rescaledSprite = Sprite.Create(smallTexture, rect, pivot, 8 * 6.25f);
            return rescaledSprite;
        }

        // Internal utility that renders the source texture into the RTT - the scaling method itself.
        private static void GpuScale(Texture2D src, int width, int height, FilterMode fmode)
        {
            //We need the source texture in VRAM because we render with it
            src.filterMode = fmode;
            src.Apply(true);

            //Using RTT for best quality and performance. Thanks, Unity 5
            var rtt = new RenderTexture(width, height, 32);

            //Set the RTT in order to render to it
            Graphics.SetRenderTarget(rtt);

            //Setup 2D matrix in range 0..1, so nobody needs to care about sized
            GL.LoadPixelMatrix(0, 1, 1, 0);

            //Then clear & draw the texture to fill the entire RTT.
            GL.Clear(true, true, new Color(0, 0, 0, 0));
            Graphics.DrawTexture(new Rect(0, 0, 1, 1), src);
        }
    }
}