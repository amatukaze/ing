namespace Sakuno.KanColle.Amatsukaze.ViewModels
{
    public abstract class TabItemViewModel : ModelBase
    {
        public abstract string Name { get; protected set; }

        public virtual bool IsClosable => false;
    }
}
