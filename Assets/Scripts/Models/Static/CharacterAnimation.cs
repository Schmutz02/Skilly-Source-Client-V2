using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Models.Static
{
    public class CharacterAnimation
    {
        private readonly Dictionary<Facing, Dictionary<Action, List<Sprite>>> _directionToAnimation =
            new Dictionary<Facing, Dictionary<Action, List<Sprite>>>();
        
        private readonly List<List<Facing>> _sec2Dirs = new List<List<Facing>>
        {
            new List<Facing>
            {
                Facing.Left, Facing.Up, Facing.Down
            },
            new List<Facing>
            {
                Facing.Up, Facing.Left, Facing.Down
            },
            new List<Facing>
            {
                Facing.Up, Facing.Right, Facing.Down
            },
            new List<Facing>
            {
                Facing.Right, Facing.Up, Facing.Down
            },
            new List<Facing>
            {
                Facing.Right, Facing.Down
            },
            new List<Facing>
            {
                Facing.Down, Facing.Right
            },
            new List<Facing>
            {
                Facing.Down, Facing.Left
            },
            new List<Facing>
            {
                Facing.Left, Facing.Down
            }
        };
    
        public CharacterAnimation(List<Sprite> frames, Facing startFacing)
        {
            if (startFacing == Facing.Right)
            {
                _directionToAnimation[Facing.Right] = GetDirection(frames, 0, false);
                _directionToAnimation[Facing.Left] = GetDirection(frames, 0, true);
                if (frames.Count >= 14)
                {
                    _directionToAnimation[Facing.Down] = GetDirection(frames, 7, false);
                    if (frames.Count >= 21)
                    {
                        _directionToAnimation[Facing.Up] = GetDirection(frames, 14, false);
                    }
                }
            }
            else
            {
                _directionToAnimation[Facing.Down] = GetDirection(frames, 0, false);
                if (frames.Count >= 14)
                {
                    _directionToAnimation[Facing.Right] = GetDirection(frames, 7, false);
                    _directionToAnimation[Facing.Left] = GetDirection(frames, 7, true);
                    if (frames.Count >= 21)
                    {
                        _directionToAnimation[Facing.Up] = GetDirection(frames, 14, false);
                    }
                }
            }
        }

        public Sprite ImageFromDir(Facing facing, Action action, int frame)
        {
            var frames = _directionToAnimation[facing][action];
            frame %= frames.Count;
            return frames[frame];
        }

        public Sprite ImageFromAngle(float angle, Action action, float p)
        {
            var sec = (int) (angle / (Mathf.PI / 4) + 4) % 8;
            var dirs = _sec2Dirs[sec];
            if (!_directionToAnimation.TryGetValue(dirs[0], out var actions))
                if (!_directionToAnimation.TryGetValue(dirs[1], out actions))
                    actions = _directionToAnimation[dirs[2]];

            var images = actions[action];
            p = Mathf.Max(0, Mathf.Min(0.99999f, p));
            var i = (int)(p * images.Count);
            return images[i];
        }

        public Sprite ImageFromFacing(float facing, Action action, float p)
        {
            var cameraAngle = MathUtils.BoundToPI(facing - Settings.CameraAngle) * -1;
            return ImageFromAngle(cameraAngle, action, p);
        }

        private static Dictionary<Action, List<Sprite>> GetDirection(List<Sprite> frames, int offset, bool mirror)
        {
            var ret = new Dictionary<Action, List<Sprite>>();
        
            var stand = mirror ? frames[offset].Mirror() : frames[offset];
            var walk1 = mirror ? frames[offset + 1].Mirror() : frames[offset + 1];
            var walk2 = mirror ? frames[offset + 2].Mirror() : frames[offset + 2];
            var attack1 = mirror ? frames[offset + 4].Mirror() : frames[offset + 4];
            var attack2 = frames[offset + 5];
            var attackBit = frames[offset + 6];

            var standAnim = new List<Sprite>
            {
                stand
            };
            ret[Action.Stand] = standAnim;

            var walkAnim = new List<Sprite>
            {
                walk1, 
                walk2.IsTransparent() ? stand : walk2
            };
            ret[Action.Walk] = walkAnim;

            List<Sprite> attackAnim;
            if (attack1.IsTransparent() && attack2.IsTransparent())
            {
                attackAnim = walkAnim;
            }
            else 
            {
                attackAnim = new List<Sprite>
                {
                    attack1
                };
            
                if (!attackBit.IsTransparent())
                {
                    attack2 = SpriteUtils.MergeSprites(attack2, attackBit);
                }
            
                attackAnim.Add(mirror ? attack2.Mirror() : attack2);
            }
            ret[Action.Attack] = attackAnim;
        
            return ret;
        }
    }

    public enum Action
    {
        Stand,
        Walk,
        Attack
    }
}