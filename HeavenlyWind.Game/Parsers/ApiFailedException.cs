using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers
{
    class ApiFailedException : Exception
    {
        public int ResultCode { get; }

        public ApiFailedException(int rpResultCode)
        {
            ResultCode = rpResultCode;
        }
    }
}
