using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using testMaps.GeocodeService;
using testMaps.ImageryService;
using testMaps.RouteService;
using testMaps.SearchService;


namespace testMaps
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool removed = false;
        private Microsoft.Maps.MapControl.WPF.Pushpin pin;
        private double selectedLatitute;
        private double selectedLongitude;

        public MainWindow()
        {
            InitializeComponent();
            
            myMap.ZoomLevel = 10;
            
            //Delete the warning window
            myMap.LayoutUpdated += (sender, args) =>
            {
                if (!removed)
                {
                    RemoveOverlayTextBlock();
                }
            };

            //Search by address
            GeocodeAddress("Gemert, Domein 16");
  
            myMap.Center.Latitude = 0;
            myMap.Center.Longitude = 0;
        }

        private void RemoveOverlayTextBlock()
        {

            if (myMap.Children.Count > 1 && !removed) {
                myMap.Children.RemoveAt(1);
                removed = true;
            }
        }

        //Get the location by clicking on the map
        private void myMap_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            myMap.Children.RemoveAt(0);

            Point mousePosition = e.GetPosition(this);
            Microsoft.Maps.MapControl.WPF.Location pinLocation = myMap.ViewportPointToLocation(mousePosition);

            PlacePin(pinLocation.Latitude, pinLocation.Longitude);
        }

        //Add a pin to give visual feedback of the selected location
        private void PlacePin(double latitude, double longitude)
        {
            selectedLatitute = latitude;
            selectedLongitude = longitude;


            pin = new Microsoft.Maps.MapControl.WPF.Pushpin();
            pin.Location = new Microsoft.Maps.MapControl.WPF.Location(latitude, longitude);

            myMap.Children.Add(pin);
        }

        //Find the location by address
        private void GeocodeAddress(string address)
        {
            string results = "";
            string key = "AnYws_sa62sPmXR-zcq6XDp8P9Sbtvbv3Bi3FtXkKc4jvOjmNwD3SeaHz54pAxpg";
            GeocodeRequest geocodeRequest = new GeocodeRequest();

            // Set the credentials using a valid Bing Maps key
            geocodeRequest.Credentials = new Microsoft.Maps.MapControl.WPF.Credentials();
            geocodeRequest.Credentials.ApplicationId = key;

            // Set the full address query
            geocodeRequest.Query = address;

            // Set the options to only return high confidence results 
            ConfidenceFilter[] filters = new ConfidenceFilter[1];
            filters[0] = new ConfidenceFilter();
            filters[0].MinimumConfidence = GeocodeService.Confidence.High;

            // Add the filters to the options
            GeocodeOptions geocodeOptions = new GeocodeOptions();
            geocodeOptions.Filters = filters;
            geocodeRequest.Options = geocodeOptions;

            // Make the geocode request
            GeocodeServiceClient geocodeService = new GeocodeServiceClient("BasicHttpBinding_IGeocodeService");
            GeocodeResponse geocodeResponse = geocodeService.Geocode(geocodeRequest);

            if (geocodeResponse.Results.Length > 0)
            {
                results = String.Format("Latitude: {0}\nLongitude: {1}",
                  geocodeResponse.Results[0].Locations[0].Latitude,
                  geocodeResponse.Results[0].Locations[0].Longitude);
                PlacePin(geocodeResponse.Results[0].Locations[0].Latitude, geocodeResponse.Results[0].Locations[0].Longitude);
            }
            else
            {
                results = "No Results Found";
            }
        }
    }
}
