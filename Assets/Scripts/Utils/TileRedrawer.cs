using System.Collections.Generic;
using Game;
using Models.Static;
using UnityEngine;

namespace Utils
{
    public static class TileRedrawer
    {
        private static readonly Dictionary<object[], Sprite> _Cache = new Dictionary<object[], Sprite>();

        private static readonly Rect TextureRect = new Rect(0, 0, 8, 8);
        private static readonly RectInt Rect0 = new RectInt(0, 0, 4, 4);
        private static readonly RectInt Rect1 = new RectInt(4, 0, 4, 4);
        private static readonly RectInt Rect2 = new RectInt(0, 4, 4, 4);
        private static readonly RectInt Rect3 = new RectInt(4, 4, 4, 4);

        private static readonly Vector2Int Point0 = new Vector2Int(0, 0);
        private static readonly Vector2Int Point1 = new Vector2Int(4, 0);
        private static readonly Vector2Int Point2 = new Vector2Int(0, 4);
        private static readonly Vector2Int Point3 = new Vector2Int(4, 4);

        private static readonly List<List<List<Sprite>>> _MaskLists = GetMasks();
        private const int _INNER = 0;
        private const int _SIDE0 = 1;
        private const int _SIDE1 = 2;
        private const int _OUTER = 3;
        private const int _INNER_P1 = 4;
        private const int _INNER_P2 = 5;

        public static Sprite Redraw(Square square, bool originalBackground)
        {
            object[] sig = null;
            if (square.Type == 253)
            {
                sig = GetCompositeSig(square);
            }
            else if (square.Desc.HasEdge)
            {
                sig = GetEdgeSig(square);
            }
            else
            {
                sig = GetSig(square);
            }

            if (sig == null)
                return null;

            if (_Cache.ContainsKey(sig))
                return _Cache[sig];

            if (square.Type == 253)
            {
                var newSprite = BuildComposite(sig);
                _Cache[sig] = newSprite;
                return newSprite;
            }

            if (square.Desc.HasEdge)
            {
                var newSprite = DrawEdges(sig);
                _Cache[sig] = newSprite;
                return newSprite;
            }

            var redraw0 = false;
            var redraw1 = false;
            var redraw2 = false;
            var redraw3 = false;
            var s0 = (ushort) sig[0];
            var s1 = (ushort) sig[1];
            var s2 = (ushort) sig[2];
            var s3 = (ushort) sig[3];
            var s4 = (ushort) sig[4];
            var s5 = (ushort) sig[5];
            var s6 = (ushort) sig[6];
            var s7 = (ushort) sig[7];
            var s8 = (ushort) sig[8];
            if (s1 != s4)
            {
                redraw0 = true;
                redraw1 = true;
            }
            if (s3 != s4)
            {
                redraw0 = true;
                redraw2 = true;
            }
            if (s5 != s4)
            {
                redraw1 = true;
                redraw3 = true;
            }
            if (s7 != s4)
            {
                redraw2 = true;
                redraw3 = true;
            }
            if (!redraw0 && s0 != s4)
            {
                redraw0 = true;
            }
            if (!redraw1 && s2 != s4)
            {
                redraw1 = true;
            }
            if (!redraw2 && s6 != s4)
            {
                redraw2 = true;
            }
            if (!redraw3 && s8 != s4)
            {
                redraw3 = true;
            }
            if (!redraw0 && !redraw1 && !redraw2 && !redraw3)
            {
                _Cache[sig] = null;
                return null;
            }

            var orig = AssetLibrary.GetTileImage(square.Type);
            Texture2D texture;
            if (originalBackground)
            {
                texture = SpriteUtils.CreateTexture(orig);
            }
            else
            {
                texture = new Texture2D(8, 8);
                texture.filterMode = FilterMode.Point;
            }

            if (redraw0)
            {
                RedrawRect(texture, Rect0, _MaskLists[0], s4, s3, s0, s1);
            }
            if (redraw1)
            {
                RedrawRect(texture, Rect1, _MaskLists[1], s4, s1, s2, s5);
            }
            if (redraw2)
            {
                RedrawRect(texture, Rect2, _MaskLists[2], s4, s7, s6, s3);
            }
            if (redraw3)
            {
                RedrawRect(texture, Rect3, _MaskLists[3], s4, s5, s8, s7);
            }

            texture.Apply();
            var sprite = Sprite.Create(texture, TextureRect, SpriteUtils.Pivot, SpriteUtils.PIXELS_PER_UNIT);
            _Cache[sig] = sprite;
            return sprite;
        }

        private static void RedrawRect(Texture2D texture, RectInt rect, List<List<Sprite>> masks, int b,
            int n0, int n1, int n2)
        {
            Sprite mask;
            Sprite blend;
            if (b == n0 && b == n2)
            {
                mask = masks[_OUTER].Random();
                blend = AssetLibrary.GetTileImage(n1);
            }
            else if (b != n0 && b != n2)
            {
                if (n0 != n2)
                {
                    var n0Image = SpriteUtils.CreateTexture(AssetLibrary.GetTileImage(n0));
                    var n2Image = SpriteUtils.CreateTexture(AssetLibrary.GetTileImage(n2));
                    texture.CopyPixels(n0Image, rect, masks[_INNER_P1].Random());
                    texture.CopyPixels(n2Image, rect, masks[_INNER_P2].Random());
                    return;
                }

                mask = masks[_INNER].Random();
                blend = AssetLibrary.GetTileImage(n0);
            }
            else if (b != n0)
            {
                mask = masks[_SIDE0].Random();
                blend = AssetLibrary.GetTileImage(n0);
            }
            else
            {
                mask = masks[_SIDE1].Random();
                blend = AssetLibrary.GetTileImage(n2);
            }

            var blendTex = SpriteUtils.CreateTexture(blend);
            texture.CopyPixels(blendTex, rect, mask);
        }

        private static List<List<List<Sprite>>> GetMasks()
        {
            var list = new List<List<List<Sprite>>>();
            AddMasks(list, AssetLibrary.GetImageSet("inner_mask"), AssetLibrary.GetImageSet("sides_mask"),
                AssetLibrary.GetImageSet("outer_mask"), AssetLibrary.GetImageSet("innerP1_mask"),
                AssetLibrary.GetImageSet("innerP2_mask"));
            return list;
        }

        private static void AddMasks(List<List<List<Sprite>>> list, List<Sprite> inner, List<Sprite> side, List<Sprite> outer,
            List<Sprite> innerP1, List<Sprite> innerP2)
        {
            foreach (var i in new[] { -1, 0, 2, 1 })
            {
                list.Add(new List<List<Sprite>>()
                {
                    RotateImageSet(inner, i - 1), RotateImageSet(side, i - 1), RotateImageSet(side, i),
                    RotateImageSet(outer, i - 1), RotateImageSet(innerP1, i), RotateImageSet(innerP2, i)
                });
            }
        }

        private static List<Sprite> RotateImageSet(List<Sprite> sprites, int clockwiseTurns)
        {
            var rotatedSprites = new List<Sprite>();
            foreach (var sprite in sprites)
            {
                rotatedSprites.Add(SpriteUtils.Rotate(sprite, clockwiseTurns));
            }

            return rotatedSprites;
        }

        private static Sprite DrawEdges(object[] sig)
        {
            var orig = AssetLibrary.GetTileImage((int) sig[4]);
            var texture = SpriteUtils.CreateTexture(orig);
            var desc = AssetLibrary.GetTileDesc((int) sig[4]);
            var edges = desc.GetEdges();
            var innerCorners = desc.GetInnerCorners();
            for (var i = 1; i < 8; i += 2)
            {
                if (!(bool) sig[i])
                {
                    texture.SetPixels32(edges[i].texture.GetPixels32());
                }
            }

            var s0 = (bool) sig[0];
            var s1 = (bool) sig[1];
            var s2 = (bool) sig[2];
            var s3 = (bool) sig[3];
            var s5 = (bool) sig[5];
            var s6 = (bool) sig[6];
            var s7 = (bool) sig[7];
            var s8 = (bool) sig[8];
            if (edges[0] != null)
            {
                if (s3 && s1 && !s0)
                {
                    texture.SetPixels32(edges[0].texture.GetPixels32());
                }
                if (s1 && s5 && !s2)
                {
                    texture.SetPixels32(edges[2].texture.GetPixels32());
                }
                if (s5 && s7 && !s8)
                {
                    texture.SetPixels32(edges[8].texture.GetPixels32());
                }
                if (s3 && s7 && !s6)
                {
                    texture.SetPixels32(edges[6].texture.GetPixels32());
                }
            }

            if (innerCorners != null)
            {
                if (!s3 && !s1)
                {
                    texture.SetPixels32(innerCorners[0].texture.GetPixels32());
                }
                if (!s1 && !s5)
                {
                    texture.SetPixels32(innerCorners[2].texture.GetPixels32());
                }
                if (!s5 && !s7)
                {
                    texture.SetPixels32(innerCorners[8].texture.GetPixels32());
                }
                if (!s7 && !s3)
                {
                    texture.SetPixels32(innerCorners[6].texture.GetPixels32());
                }
            }
            
            texture.Apply();
            return Sprite.Create(texture, TextureRect, SpriteUtils.Pivot, SpriteUtils.PIXELS_PER_UNIT);
        }

        private static Sprite BuildComposite(object[] sig)
        {
            var texture = new Texture2D((int) TextureRect.width, (int) TextureRect.height);
            texture.filterMode = FilterMode.Point;
            var s0 = (int) sig[0];
            var s1 = (int) sig[1];
            var s2 = (int) sig[2];
            var s3 = (int) sig[3];
            if (s0 != 255)
            {
                var neighbor = AssetLibrary.GetTileImage(s0);
                var pixels = neighbor.texture.GetPixels(Point0.x, Point0.y, Rect0.width, Rect0.height);
                texture.SetPixels(Point0.x, Point0.y, Rect0.width, Rect0.height, pixels);
            }
            if (s1 != 255)
            {
                var neighbor = AssetLibrary.GetTileImage(s1);
                var pixels = neighbor.texture.GetPixels(Point1.x, Point1.y, Rect1.width, Rect1.height);
                texture.SetPixels(Point1.x, Point1.y, Rect1.width, Rect1.height, pixels);
            }
            if (s2 != 255)
            {
                var neighbor = AssetLibrary.GetTileImage(s2);
                var pixels = neighbor.texture.GetPixels(Point2.x, Point2.y, Rect2.width, Rect2.height);
                texture.SetPixels(Point2.x, Point2.y, Rect2.width, Rect2.height, pixels);
            }
            if (s3 != 255)
            {
                var neighbor = AssetLibrary.GetTileImage(s3);
                var pixels = neighbor.texture.GetPixels(Point3.x, Point3.y, Rect3.width, Rect3.height);
                texture.SetPixels(Point3.x, Point3.y, Rect3.width, Rect3.height, pixels);
            }
            texture.Apply();
            return Sprite.Create(texture, TextureRect, SpriteUtils.Pivot, SpriteUtils.PIXELS_PER_UNIT);
        }

        private static object[] GetSig(Square square)
        {
            var sig = new List<object>();
            var map = square.Map;
            for (var y = square.Position.y - 1; y <= square.Position.y + 1; y++)
            {
                for (var x = square.Position.x - 1; x <= square.Position.x + 1; x++)
                {
                    if (x < 0 || x >= map.Width || y < 0 || y >= map.Height ||
                        x == square.Position.x && y == square.Position.y)
                    {
                        sig.Add(square.Type);
                        continue;
                    }

                    var n = map.GetTile(x, y);
                    if (n == null || n.Desc.BlendPriority <= square.Desc.BlendPriority)
                    {
                        sig.Add(square.Type);
                        continue;
                    }
                    
                    sig.Add(n.Type);
                }
            }

            return sig.ToArray();
        }

        private static object[] GetEdgeSig(Square square)
        {
            var sig = new List<object>();
            var map = square.Map;
            var hasEdge = false;
            var sameTypeEdgeMode = square.Desc.SameTypeEdgeMode;
            for (var y = square.Position.y - 1; y <= square.Position.y + 1; y++)
            {
                for (var x = square.Position.x - 1; x <= square.Position.x + 1; x++)
                {
                    var n = map.GetTile(x, y);
                    if (x == square.Position.x && y == square.Position.y)
                    {
                        sig.Add(n.Type);
                        continue;
                    }

                    bool b;
                    if (sameTypeEdgeMode)
                    {
                        b = n == null || n.Type == square.Type;
                    }
                    else
                    {
                        b = n == null || n.Type != 255;
                    }
                    sig.Add(b);
                    hasEdge = hasEdge || !b;
                }
            }

            return hasEdge ? sig.ToArray() : null;
        }

        private static object[] GetCompositeSig(Square square)
        {
            var sig = new object[4];
            var map = square.Map;
            var x = square.Position.x;
            var y = square.Position.y;
            var n1 = map.GetTile(x, y - 1);
            var n2 = map.GetTile(x - 1, y);
            var n3 = map.GetTile(x + 1, y);
            var n4 = map.GetTile(x, y + 1);
            var p1 = n1 != null ? n1.Desc.CompositePriority : -1;
            var p2 = n2 != null ? n2.Desc.CompositePriority : -1;
            var p3 = n3 != null ? n3.Desc.CompositePriority : -1;
            var p4 = n4 != null ? n4.Desc.CompositePriority : -1;
            if (p1 < 0 && p2 < 0)
            { 
                var n0 = map.GetTile(x - 1, y - 1);
                sig[0] = n0 == null || n0.Desc.CompositePriority < 0 ? 255 : n0.Type;
            }
            else if (p1 < p2)
            {
                sig[0] = n2.Type;
            }
            else
            {
                sig[0] = n1.Type;
            }
            
            if (p1 < 0 && p3 < 0)
            { 
                var n0 = map.GetTile(x + 1, y - 1);
                sig[1] = n0 == null || n0.Desc.CompositePriority < 0 ? 255 : n0.Type;
            }
            else if (p1 < p3)
            {
                sig[1] = n3.Type;
            }
            else
            {
                sig[1] = n1.Type;
            }
            
            if (p2 < 0 && p4 < 0)
            { 
                var n0 = map.GetTile(x - 1, y + 1);
                sig[2] = n0 == null || n0.Desc.CompositePriority < 0 ? 255 : n0.Type;
            }
            else if (p2 < p4)
            {
                sig[2] = n4.Type;
            }
            else
            {
                sig[2] = n2.Type;
            }
            
            if (p3 < 0 && p4 < 0)
            { 
                var n0 = map.GetTile(x + 1, y + 1);
                sig[3] = n0 == null || n0.Desc.CompositePriority < 0 ? 255 : n0.Type;
            }
            else if (p3 < p4)
            {
                sig[3] = n4.Type;
            }
            else
            {
                sig[3] = n3.Type;
            }

            return sig;
        }
    }
}