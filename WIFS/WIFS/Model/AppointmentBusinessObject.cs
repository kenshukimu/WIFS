using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIFS
{
    public class AppointmentBusinessObject : INotifyPropertyChanged
    {
        private string _subject = "";
        private string _body = "";
        private string _location = "";
        private string _properties = "";
        private DateTime _start = DateTime.Today;
        private DateTime _end = DateTime.Today + TimeSpan.FromDays(1);
        private Guid _id = Guid.NewGuid();
        private bool _isDeleted = false;

        private string _BOProperty1 = "";
        private string _BOProperty2 = "";

        public AppointmentBusinessObject()
        {
        }

        #region ** object model
        public string Subject
        {
            get { return _subject; }
            set
            {
                if (_subject != value)
                {
                    _subject = value;
                    OnPropertyChanged("Subject");
                }
            }
        }

        public DateTime Start
        {
            get { return _start; }
            set
            {
                if (_start != value)
                {
                    _start = value;
                    OnPropertyChanged("Start");
                }
            }
        }

        public DateTime End
        {
            get { return _end; }
            set
            {
                if (_end != value)
                {
                    _end = value;
                    OnPropertyChanged("End");
                }
            }
        }

        public string Body
        {
            get { return _body; }
            set
            {
                if (_body != value)
                {
                    _body = value;
                    OnPropertyChanged("Body");
                }
            }
        }

        public string Location
        {
            get { return _location; }
            set
            {
                if (_location != value)
                {
                    _location = value;
                    OnPropertyChanged("Location");
                }
            }
        }

        public string Properties
        {
            get { return _properties; }
            set
            {
                if (_properties != value)
                {
                    _properties = value;
                    OnPropertyChanged("Properties");
                }
            }
        }

        public Guid Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        public bool IsDeleted
        {
            get { return _isDeleted; }
            set
            {
                if (_isDeleted != value)
                {
                    _isDeleted = value;
                    OnPropertyChanged("IsDeleted");
                }
            }
        }

        public string BOProperty1
        {
            get { return _BOProperty1; }
            set
            {
                if (_BOProperty1 != value)
                {
                    _BOProperty1 = value;
                    OnPropertyChanged("BOProperty1");
                }
            }
        }
        public string BOProperty2
        {
            get { return _BOProperty2; }
            set
            {
                if (_BOProperty2 != value)
                {
                    _BOProperty2 = value;
                    OnPropertyChanged("BOProperty2");
                }
            }
        }

        #endregion

        #region ** INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        #endregion
    }

    /// <summary>
    /// The <see cref="AppointmentBOList"/> class is a collection of the <see cref="AppointmentBusinessObject"/>
    /// objects that supports data binding.
    /// Note: data binding support is inherited from the BindingList class. 
    /// </summary>
    public class AppointmentBOList : ObservableCollection<AppointmentBusinessObject>
    {
        protected override void RemoveItem(int index)
        {
            AppointmentBusinessObject item = this[index];
            if (item != null)
            {
                item.IsDeleted = true;
            }
            base.RemoveItem(index);
        }
    }
}
