using System;
using System.ComponentModel.DataAnnotations;

namespace Sakuno.ING.Game.Logger
{
    public abstract class EntityBase : ITimedEntity
    {
        [Key]
        public DateTimeOffset TimeStamp { get; set; }
        public string Source { get; set; }
    }
}
