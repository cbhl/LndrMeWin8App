using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Net;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
//using Windows.UI.Xaml.Input;
using System.Windows.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using System.Collections.ObjectModel;
using Windows.Data.Json;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI.Core;
//using GalaSoft.MvvmLight.Command;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.Resources;

namespace LndrMeWin8App
{
    public abstract class CommonViewModel : LndrMeWin8App.Common.BindableBase
    {
    private static Uri _baseUri = new Uri("ms-appx:///");

        public CommonViewModel(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(CommonViewModel._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class ApplianceGroup : CommonViewModel
    {
        public ApplianceGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
        }

        private ObservableCollection<ApplianceViewModel> _items = new ObservableCollection<ApplianceViewModel>();
        public ObservableCollection<ApplianceViewModel> Items
        {
            get { return this._items; }
        }
    }

    public class MainViewModel : INotifyPropertyChanged
    {


        public CoreDispatcher _cd { get; set; }

        public MainViewModel()
        {
            this.ApplianceGroups = new ObservableCollection<ApplianceGroup>();
            this.ApplianceGroups.Add(new ApplianceGroup("ApplianceGroup-1", "All Appliances", "", "Assets/DarkGray.png", ""));
            this.ApplianceGroups.Add(new ApplianceGroup("ApplianceGroup-2", "Washers", "", "Assets/DarkGray.png", ""));
            this.ApplianceGroups.Add(new ApplianceGroup("ApplianceGroup-3", "Dryers", "", "Assets/DarkGray.png", ""));
        }

        public static string _(string key)
        {
            var rl = new ResourceLoader();
            return rl.GetString(key);
            //var context = new ResourceContext();
            //var resourceMap = ResourceManager.Current.MainResourceMap;
            //return resourceMap.GetValue(key, context).ToString();
        }

        public ObservableCollection<ApplianceGroup> ApplianceGroups { get; private set; }
        
        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<ApplianceViewModel> AllAppliances
        {
            get
            {
                foreach (ApplianceGroup ag in ApplianceGroups)
                {
                    if (ag.UniqueId.Equals("ApplianceGroup-1"))
                    {
                        return ag.Items;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<ApplianceViewModel> Washers
        {
            get
            {
                foreach (ApplianceGroup ag in ApplianceGroups)
                {
                    if (ag.UniqueId.Equals("ApplianceGroup-2"))
                    {
                        return ag.Items;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<ApplianceViewModel> Dryers
        {
            get
            {
                foreach (ApplianceGroup ag in ApplianceGroups)
                {
                    if (ag.UniqueId.Equals("ApplianceGroup-3"))
                    {
                        return ag.Items;
                    }
                }
                return null;
            }
        }

        private bool _isDataLoaded;
        public bool IsDataLoaded
        {
            get {
                return _isDataLoaded;
            }
            private set
            {
                if (value != _isDataLoaded)
                {
                    _isDataLoaded = value;
                    NotifyPropertyChanged("IsDataLoaded");
                }
            }
        }

        public void OnClaim(ApplianceViewModel avm)
        {
            UriBuilder fullUri = new UriBuilder("http://lndr.me/receive.json");
            fullUri.Query = String.Format("key={0}&id={1}&email={2}",_("ServerKey"),avm.Id,_("DefaultEMail"));

            // initialize a new WebRequest
            HttpWebRequest lndrRequest = (HttpWebRequest)WebRequest.Create(fullUri.Uri);

            // set up the state object for the async request
            LndrUpdateState lndrUpdateState = new LndrUpdateState();
            lndrUpdateState.AsyncRequest = lndrRequest;

            // start the asynchronous request
            lndrRequest.BeginGetResponse(new AsyncCallback(HandleUpdateResponse),
                lndrUpdateState);
        }

        private void AddAppliance(ApplianceViewModel avm)
        {
            avm.ClaimCommand = new ClaimApplianceCommand(this, avm);
            this.AllAppliances.Add(avm);
            switch (avm.Appliance)
            {
                case ApplianceViewModel.ApplianceType.DRYER:
                    this.Dryers.Add(avm);
                    break;
                case ApplianceViewModel.ApplianceType.WASHER:
                    this.Washers.Add(avm);
                    break;
            }
        }

        public void Clear()
        {
            this.AllAppliances.Clear();
            this.Dryers.Clear();
            this.Washers.Clear();
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            // Sample data; replace with real data
            //AddAppliance(new ApplianceViewModel() { Appliance = ApplianceViewModel.ApplianceType.WASHER, Id = 1, Name = "Washer 1", Busy = false, FreeAt = DateTime.Now });
            //AddAppliance(new ApplianceViewModel() { Appliance = ApplianceViewModel.ApplianceType.WASHER, Id = 2, Name = "Washer 2", Busy = true, FreeAt = DateTime.Now.AddHours(1.2) });
            //AddAppliance(new ApplianceViewModel() { Appliance = ApplianceViewModel.ApplianceType.DRYER, Id = 3, Name = "Dryer 1", Busy = true, FreeAt = DateTime.Now.AddMinutes(42) });
            //AddAppliance(new ApplianceViewModel() { Appliance = ApplianceViewModel.ApplianceType.DRYER, Id = 4, Name = "Dryer 2", Busy = false, FreeAt = DateTime.Now });

            this.IsDataLoaded = false;
            Clear();

            UriBuilder fullUri = new UriBuilder(_("ServerBaseURI") + _("ServerStatusEndpoint"));
            fullUri.Query = String.Format("key={0}", _("ServerKey"));

            // initialize a new WebRequest
            HttpWebRequest lndrRequest = (HttpWebRequest)WebRequest.Create(fullUri.Uri);

            // set up the state object for the async request
            LndrUpdateState lndrUpdateState = new LndrUpdateState();
            lndrUpdateState.AsyncRequest = lndrRequest;

            // start the asynchronous request
            lndrRequest.BeginGetResponse(new AsyncCallback(HandleIndexResponse),
                lndrUpdateState);
        }

        /// <summary>
        /// Handle the information returned from the async request
        /// </summary>
        /// <param name="asyncResult"></param>
        private void HandleUpdateResponse(IAsyncResult asyncResult)
        {
            LndrUpdateState lndrUpdateState = (LndrUpdateState)asyncResult.AsyncState;
            HttpWebRequest lndrRequest = (HttpWebRequest)lndrUpdateState.AsyncRequest;

            lndrUpdateState.AsyncResponse = (HttpWebResponse)lndrRequest.EndGetResponse(asyncResult);

            try {

                _cd.Invoke(CoreDispatcherPriority.Normal,
                    (s, a) =>
                    {
                        //LoadData();
                    }, this, null
                );

            }
            catch (FormatException)
            {
                // there was some kind of error processing the response from the web
                // additional error handling would normally be added here
                return;
            }
        }

        /// <summary>
        /// Handle the information returned from the async request
        /// </summary>
        /// <param name="asyncResult"></param>
        private void HandleIndexResponse(IAsyncResult asyncResult)
        {
            LndrUpdateState lndrUpdateState = (LndrUpdateState)asyncResult.AsyncState;
            HttpWebRequest lndrRequest = (HttpWebRequest)lndrUpdateState.AsyncRequest;

            lndrUpdateState.AsyncResponse = (HttpWebResponse)lndrRequest.EndGetResponse(asyncResult);

            try
            {
                Stream streamResult = lndrUpdateState.AsyncResponse.GetResponseStream();
                StreamReader sr = new StreamReader(streamResult);
                string json = sr.ReadToEnd();

                IList<ApplianceViewModel> appliances = new List<ApplianceViewModel>();

                JsonArray applianceArray = JsonArray.Parse(json);
                foreach (JsonValue jsonValue in applianceArray) {
                    JsonObject applianceObject = jsonValue.GetObject();
                    ApplianceViewModel avm = new ApplianceViewModel();
                    try
                    {
                        avm.Id =  (int) applianceObject.GetNamedNumber("id");
                        avm.Name = applianceObject.GetNamedString("name");
                        avm.Busy = applianceObject.GetNamedBoolean("busy");
                        avm.FreeAt = DateTime.Parse(applianceObject.GetNamedString("free_at")).ToUniversalTime();
                        appliances.Add(avm);
                    }
                    catch (Exception e)
                    {
                    }
                }

                //IList<ApplianceViewModel> appliances = JsonConvert.DeserializeObject<IList<ApplianceViewModel>>(json);

                Clear();
                foreach (ApplianceViewModel appliance in appliances)
                {
                    AddAppliance(appliance);
                }
                this.IsDataLoaded = true;

                //Window.Current.CoreWindow.Dispatcher.Invoke(CoreDispatcherPriority.Normal,
                //    (s, a) =>
                //    {
                        
                //    }, this, asyncResult
                //);
            }
            catch (FormatException)
            {
                // there was some kind of error processing the response from the web
                // additional error handling would normally be added here
                return;
            }
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

    /// <summary>
    /// State information for our BeginGetResponse async call
    /// </summary>
    public class LndrUpdateState
    {
        public HttpWebRequest AsyncRequest { get; set; }
        public HttpWebResponse AsyncResponse { get; set; }
    }
}