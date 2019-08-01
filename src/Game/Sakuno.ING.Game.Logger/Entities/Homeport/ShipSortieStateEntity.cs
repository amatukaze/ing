using System;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Logger.Entities.Homeport
{
    internal class ShipSortieStateEntity
    {
        public ShipId Id { get; set; }
        public DateTimeOffset LastSortie { get; set; }
    }
}
