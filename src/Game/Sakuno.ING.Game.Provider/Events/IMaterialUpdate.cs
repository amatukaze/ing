﻿using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public interface IMaterialUpdate
    {
        void Apply(Materials materials);
    }
}
