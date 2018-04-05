using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sakuno.KanColle.Amatsukaze.Services;

namespace Sakuno.KanColle.Amatsukaze.UWP.Bridge
{
    internal class Provider : ITextStreamProvider
    {
        public bool Enabled { get; set; }

        public event Action<TextMessage> Received;
    }
}
