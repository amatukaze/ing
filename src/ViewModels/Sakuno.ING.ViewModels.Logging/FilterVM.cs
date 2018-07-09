using System;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.ING.ViewModels.Logging
{
    internal class FilterVM<T> : BindableObject, IFilterVM, IEqualityComparer<T>
    {
        private readonly Func<T, int> keySelector;
        private readonly Func<T, string> defaultText;
        private readonly Func<T, string[]> acceptingText;

        public FilterVM(string name, Func<T, int> keySelector, Func<T, string> defaultText, Func<T, string[]> acceptingText = null)
        {
            this.keySelector = keySelector;
            this.defaultText = defaultText;
            this.acceptingText = acceptingText;
            Name = name;
        }

        public string Name { get; }

        public bool Hits(T value)
        {
            if (defaultText(value).Contains(SelectedText)) return true;
            if (acceptingText == null) return false;
            foreach (string str in acceptingText(value))
                if (str.Contains(SelectedText)) return true;

            return false;
        }

        private string _selectedTest;
        public string SelectedText
        {
            get => _selectedTest;
            set
            {
                if (_selectedTest != value)
                {
                    _selectedTest = value;
                    NotifyPropertyChanged();
                    Updated?.Invoke();
                }
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    NotifyPropertyChanged();
                    Updated?.Invoke();
                }
            }
        }

        public event Action Updated;

        public IBindableCollection<string> Candidates { get; private set; }
        public void UpdateCandidates(IEnumerable<T> values)
        {
            Candidates = values.Distinct(this).OrderBy(keySelector)
                .Select(defaultText).Distinct().ToBindable();
            NotifyPropertyChanged(nameof(Candidates));
        }

        bool IEqualityComparer<T>.Equals(T x, T y) => keySelector(x) == keySelector(y);
        int IEqualityComparer<T>.GetHashCode(T obj) => keySelector(obj);
    }
}
