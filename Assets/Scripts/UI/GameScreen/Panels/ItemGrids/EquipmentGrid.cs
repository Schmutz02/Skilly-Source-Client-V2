using System;
using Game.Entities;
using Models.Static;
using Networking.Packets.Incoming;
using UI.GameScreen.Panels.ItemGrids.ItemTiles;
using UnityEngine;

namespace UI.GameScreen.Panels.ItemGrids
{
    public class EquipmentGrid : ItemGrid
    {
        [SerializeField]
        private EquipmentTile[] _tiles;

        private void Awake()
        {
            Networking.Packets.Incoming.Update.OnMyPlayerJoined += OnMyPlayerJoined;
        }

        private void OnMyPlayerJoined(Player player)
        {
            Init(player, player.SlotTypes, player, 0);
        }

        private void Init(Entity owner, ItemType[] items, Player currentPlayer, int itemIndexOffset)
        {
            base.Init(owner, currentPlayer, itemIndexOffset);
            for (var i = 0; i < _tiles.Length; i++)
            {
                _tiles[i].Init(i, this);
                _tiles[i].SetType(items[i]);
            }
        }

        private void Update()
        {
            if (CurrentPlayer == null)
                return;

            SetItems(CurrentPlayer.Equipment, CurrentPlayer.ItemDatas, IndexOffset);
        }

        private void SetItems(int[] items, int[] itemDatas, int indexOffset)
        {
            var numItems = items.Length;
            for (var tileIndex = 0; tileIndex < _tiles.Length; tileIndex++)
            {
                var i = tileIndex + indexOffset;
                if (i < numItems)
                {
                    if (_tiles[tileIndex].SetItem(items[i], itemDatas[i]))
                    {
                        //TODO refresh tooltip
                    }
                }
                else
                {
                    if (_tiles[tileIndex].SetItem(-1, -1))
                    {
                        //TODO refresh tooltip
                    }
                }
            }
        }
    }
}