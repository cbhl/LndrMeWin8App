using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
//using Windows.UI.Xaml.Ink;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Windows.Data.Json;
using Windows.ApplicationModel.Resources.Core;
using System.Windows.Input;
//using Windows.UI.Xaml.Input;
//using GalaSoft.MvvmLight.Command;
using Windows.ApplicationModel.Resources;

namespace LndrMeWin8App
{
    public class ApplianceViewModel : INotifyPropertyChanged
    {
        public static string _(string key)
        {
            var rl = new ResourceLoader();
            return rl.GetString(key);
            //var context = new ResourceContext();
            //var resourceMap = ResourceManager.Current.MainResourceMap;
            //return resourceMap.GetValue(key, context).ToString();
        }

        public enum ApplianceType { WASHER, DRYER };

        public string Status
        {
            get
            {
                if (Busy)
                {
                    return string.Format(_("WillBeAvailableIn"), DateHelper.DistanceOfTimeInWords(DateTime.Now.ToUniversalTime(), FreeAt));
                }
                else
                {
                    return _("Available");
                }
            }
        }

        //private ApplianceType _appliance;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public ApplianceType Appliance
        {
            get
            {
                //return _appliance;
                return (Id <= 2 ? ApplianceViewModel.ApplianceType.WASHER : ApplianceViewModel.ApplianceType.DRYER);
            }
            //set
            //{
            //    if (value != _appliance)
            //    {
            //        _appliance = value;
            //        NotifyPropertyChanged("Appliance");
            //    }
            //}
        }

        private int _id;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (value != _id)
                {
                    _id = value;
                    NotifyPropertyChanged("Id");
                    NotifyPropertyChanged("Appliance");
                }
            }
        }

        private string _name;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        private bool _busy;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public bool Busy
        {
            get
            {
                return _busy;
            }
            set
            {
                if (value != _busy)
                {
                    _busy = value;
                    NotifyPropertyChanged("Busy");
                    NotifyPropertyChanged("Status");
                }
            }
        }

        private DateTime _freeAt;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public DateTime FreeAt
        {
            get
            {
                return _freeAt;
            }
            set
            {
                if (value != _freeAt)
                {
                    _freeAt = value;
                    NotifyPropertyChanged("FreeAt");
                }
            }
        }

        public ICommand ClaimCommand
        {
            get;
            set;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}