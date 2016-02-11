using System;
using System.Reactive.Linq;
using System.Windows.Shell;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public class TaskbarProgressService : ModelBase
    {
        public static TaskbarProgressService Instance { get; } = new TaskbarProgressService();

        TaskbarItemProgressState r_State;
        public TaskbarItemProgressState State
        {
            get { return r_State; }
            set
            {
                if (r_State != value)
                {
                    r_State = value;
                    OnPropertyChanged(nameof(State));
                }
            }
        }

        double r_Value;
        public double Value
        {
            get { return r_Value; }
            set
            {
                if (r_Value != value)
                {
                    r_Value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        TaskbarProgressService() { }
    }
}
