using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Models.Static
{
    public class CharacterAnimation
    {
        private readonly Dictionary<Facing, Dictionary<Action, List<Sprite>>> _directionToAnimation =
            new Dictionary<Facing, Dictionary<Action, List<Sprite>>>();
    
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

        public Sprite GetFrame(Facing facing, Action action, int frame)
        {
            return _directionToAnimation[facing][action][frame];
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