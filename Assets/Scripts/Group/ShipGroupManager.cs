using System;
using System.Collections.Generic;
using System.Linq;
using Players;
using ShipLogic;
using UI.Dialog.InfoAboutGroup;
using UnityEngine;
using Utils;

namespace Group
{
    public class ShipGroupManager : IDisposable
    {
        private const int MaxCountInGroup = 3;

        private readonly Dictionary<PlayerType, List<IShipGroup>> _groups = new();

        public ShipGroupManager()
        {
            _groups[PlayerType.Player1] = new List<IShipGroup>();
            _groups[PlayerType.Player2] = new List<IShipGroup>();
        }
        
        public void AddShipInGroup(PlayerType player, ISupportedGroup ship)
        {
            if (!_groups.ContainsKey(player))
            {
                Debug.LogError($"Not found player type: {player}");
                return;
            }
            
            var group = GetGroup(player, _groups[player]);
            ship.InitGroup(group);
            group.AddShipInGroup(ship);
        }

        private IShipGroup GetGroup(PlayerType player, List<IShipGroup> groups)
        {
            var freeGroup = groups.FirstOrDefault(group => group.CanAddShipInGroup);
            if (freeGroup != null)
            {
                return freeGroup;
            }
            
            var createdGroup = CreateNewGroup(player);
            groups.Add(createdGroup);
            return createdGroup;
        }

        private static ShipGroup CreateNewGroup(PlayerType player)
        {
            var group = new ShipGroup(MaxCountInGroup);
            var groupView = Main.Instance.DialogManager.GetNewLocationDialog<ShipGroupView>();
            var playerColor = Main.Instance.ColorStorage.GetColorForPlayer(player);
            groupView.Init(group, PlayerUtils.GetPlayerName(player), playerColor);

            return group;
        }

        public void Dispose()
        {
            foreach (var group in _groups.Values.SelectMany(groups => groups))
            {
                group.Dispose();
            }

            _groups.Clear();
        }
    }
}