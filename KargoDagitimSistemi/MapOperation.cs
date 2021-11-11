using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using System;

namespace KargoDagitimSistemi
{
    class MapOperation
    {
        static string ApiKey = @"AIzaSyDBaZ-hGMhnkHNn8XhzugkbduYBrOEf9Rk";

        public void InitialSettings(GMapControl gMap)
        {
            GMapProviders.GoogleMap.ApiKey = ApiKey;
            gMap.ShowCenter = false;

            gMap.MapProvider = GMapProviders.GoogleMap;
            gMap.Position = new PointLatLng(40.76525303008106, 29.93999147323718);

            gMap.MinZoom = 5;
            gMap.MaxZoom = 100;
            gMap.Zoom = 15;
        }

        public PointLatLng FindLatLng(string location,PointLatLng pointLatLng)
        {
            if (!location.Trim().Equals(""))
            {
                GeoCoderStatusCode statusCode;
                var point = GoogleMapProvider.Instance.GetPoint(location.Trim(), out statusCode);

                if (statusCode == GeoCoderStatusCode.OK)
                {
                    pointLatLng.Lat = Convert.ToDouble(point?.Lat);
                    pointLatLng.Lng = Convert.ToDouble(point?.Lng);
                }
            }

            return pointLatLng;
        }
    }
}
