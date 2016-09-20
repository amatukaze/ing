using System;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public class ClampedValue : ModelBase, IEquatable<ClampedValue>
    {
        int r_Current;
        public int Current
        {
            get { return r_Current; }
            set
            {
                if (r_Current != value)
                {
                    r_Current = value;
                    OnPropertyChanged(nameof(Current));
                }
            }
        }

        int r_Before;
        public int Before
        {
            get { return r_Before; }
            set
            {
                if (r_Before != value)
                {
                    r_Before = value;
                    OnPropertyChanged(nameof(Before));
                }
            }
        }

        int r_Maximum;
        public int Maximum
        {
            get { return r_Maximum; }
            set
            {
                if (r_Maximum != value)
                {
                    r_Maximum = value;
                    OnPropertyChanged(nameof(Maximum));
                }
            }
        }

        public ClampedValue(int rpMaximum, int rpCurrent) : this(rpMaximum, rpCurrent, rpCurrent) { }
        public ClampedValue(int rpMaximum, int rpBefore, int rpCurrent)
        {
            Maximum = rpMaximum;
            Before = rpBefore;
            Current = rpCurrent;
        }

        public void Set(int rpMaximum, int rpCurrent)
        {
            r_Maximum = rpMaximum;
            r_Current = rpCurrent;

            OnPropertyChanged(nameof(Maximum));
            OnPropertyChanged(nameof(Current));
        }

        public bool Equals(ClampedValue rpOther)
        {
            if (ReferenceEquals(rpOther, null))
                return false;

            return Current == rpOther.Current && Before == rpOther.Before && Maximum == rpOther.Maximum;
        }
        public override bool Equals(object rpObject)
        {
            var rObject = rpObject as ClampedValue;
            if (rObject == null)
                return false;

            return Equals(rObject);
        }
        public override int GetHashCode() => (Maximum * 401) ^ (Before * 19) ^ Current;

        public static bool operator ==(ClampedValue x, ClampedValue y) => x.Equals(y);
        public static bool operator !=(ClampedValue x, ClampedValue y) => !x.Equals(y);

        public override string ToString() => $"{(Before == Current ? string.Empty : Before + "->")}{Current}/{Maximum}";
    }
}