using System;

namespace Sakuno.KanColle.Amatsukaze
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ViewInfoAttribute : Attribute
    {
        public Type ViewType { get; }

        public ViewInfoAttribute(Type rpViewType)
        {
            ViewType = rpViewType;
        }
    }
}
