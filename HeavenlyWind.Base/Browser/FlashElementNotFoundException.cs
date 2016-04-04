using System;

namespace Sakuno.KanColle.Amatsukaze.Browser
{
    public class FlashElementNotFoundException : Exception
    {
        public FlashElementNotFoundException() : this(StringResources.Instance.Main.Log_Screenshot_Failed_FlashElementNotFound) { }
        public FlashElementNotFoundException(string rpMessage) : base(rpMessage) { }
    }
}
