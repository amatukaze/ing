using System;

namespace Sakuno.KanColle.Amatsukaze
{
    public struct DataStoreItem
    {
        public string Name { get; }
        public byte[] Content { get; }

        public DateTimeOffset? Timestamp { get; }

        internal DataStoreItem(string rpName, byte[] rpContent, long? rpTimestamp)
        {
            Name = rpName;
            Content = rpContent;

            if (rpTimestamp.HasValue)
                Timestamp = DateTimeUtil.FromUnixTime(rpTimestamp.Value);
            else
                Timestamp = null;
        }
    }
}
