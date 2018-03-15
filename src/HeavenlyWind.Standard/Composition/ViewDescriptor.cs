using System;

namespace Sakuno.KanColle.Amatsukaze.Composition
{
    public sealed class ViewDescriptor
    {
        public ViewDescriptor(Type viewType, Type viewModelType, string id, bool isFixedSize, bool singleWindowRecommended)
        {
            ViewType = viewType;
            Id = id;
            IsFixedSize = isFixedSize;
            SingleWindowRecommended = singleWindowRecommended;
        }

        public Type ViewType { get; }
        public string Id { get; }
        public bool IsFixedSize { get; }
        public bool SingleWindowRecommended { get; }
    }
}
