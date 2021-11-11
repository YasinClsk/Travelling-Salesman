using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace KargoDagitimSistemi
{
    public partial class GUI2 : Form
    {
        GMapOverlay routes = new GMapOverlay("routes");
        List<Coordinates> points;
        public GUI2()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitialSettings();
        }

        void MapSettings()
        {
            MapOperation mapOperation = new MapOperation();
            mapOperation.InitialSettings(map);
        }

        void InitialSettings()
        {
            GMapProviders.GoogleMap.ApiKey = @"AIzaSyDBaZ-hGMhnkHNn8XhzugkbduYBrOEf9Rk";
            map.ShowCenter = false;

            map.MapProvider = GMapProviders.GoogleMap;
            map.Position = new PointLatLng(40.76525303008106, 29.93999147323718);

            map.MinZoom = 5;
            map.MaxZoom = 100;
            map.Zoom = 15;
        }

        public void FindDistance()
        {
            while (map.Overlays.Count > 0)
                map.Overlays.RemoveAt(0);

            map.Overlays.Clear();
            routes.Clear();
            routes = new GMapOverlay("routes");
            Pointer();

            points = new List<Coordinates>();
            for (int i = 0; i < DataAccess.coordinates.Count; i++)
            {
                if (DataAccess.coordinates[i].DeliveryStatus == false)
                {
                    points.Add(DataAccess.coordinates[i]);
                }
            }

            double[,] mesafe = new double[points.Count, points.Count];

            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 0; j < points.Count; j++)
                {
                    var route = GoogleMapProvider.Instance.GetRoute(points[i].Point, points[j].Point, false, false, 15);
                    mesafe[i, j] = route.Distance;
                }
            }

            gezginSaticiProblemi(mesafe, points.Count);
        }

        void PaintRoutes(List<int> shortestPath)
        {
            Random randomColor = new Random();
            for (int i = 0; i < shortestPath.Count - 1; i++)
            {
                var route = GoogleMapProvider.Instance.GetRoute(points[shortestPath[i]].Point, points[shortestPath[i + 1]].Point, false, false, 15);
                var r = new GMapRoute(route.Points, "My Route") { Stroke = new Pen(Color.FromArgb(randomColor.Next(50, 175), randomColor.Next(50, 175), randomColor.Next(50, 175)), 3) };
                routes.Routes.Add(r);
                map.Overlays.Add(routes);
            }
            try
            {
                map.Zoom = map.Zoom + 1;
            }
            catch (Exception) { }
        }

        void Pointer()
        {
            for (int i = 0; i < DataAccess.coordinates.Count; i++)
            {
                if (DataAccess.coordinates[i].DeliveryStatus == false)
                {
                    GMapMarker marker;
                    if (i == 0)
                    {
                        marker = new GMarkerGoogle(DataAccess.coordinates[0].Point, GMarkerGoogleType.blue);
                    }

                    else
                    {
                        marker = new GMarkerGoogle(DataAccess.coordinates[i].Point, GMarkerGoogleType.red);
                    }
                    GMapOverlay gMapOverlay = new GMapOverlay("markers");
                    gMapOverlay.Markers.Add(marker);
                    map.Overlays.Add(gMapOverlay);
                }
            }
        }

        void gezginSaticiProblemi(double[,] mesafeler, int kargoSayisi)
        {
            double[,] graph = mesafeler;
            int s = 0;

            List<int> vertex = new List<int>();
            List<int> enKisaYolList = new List<int>();

            for (int i = 0; i < kargoSayisi; i++)
                if (i != s)
                    vertex.Add(i);

            double enKisaYol = double.MaxValue;
            do
            {
                double mevcutYolMesafesi = 0;
                int k = s;
                for (int i = 0; i < vertex.Count; i++)
                {
                    mevcutYolMesafesi += graph[k, vertex[i]];
                    k = vertex[i];
                }

                if (enKisaYol > mevcutYolMesafesi)
                {
                    enKisaYolList.Clear();
                    enKisaYolList.Add(0);
                    foreach (var item in vertex)
                    {
                        enKisaYolList.Add(item);
                    }
                }
                enKisaYol = Math.Min(enKisaYol, mevcutYolMesafesi);

            } while (sonrakiIslemiBul(vertex));

            PaintRoutes(enKisaYolList);
        }

        List<int> degistir(List<int> veri, int sol, int sag)
        {
            int tut = veri[sol];
            veri[sol] = veri[sag];
            veri[sag] = tut;
            return veri;
        }

        List<int> tersineCevir(List<int> veri, int sol, int sag)
        {

            while (sol < sag)
            {
                int temp = veri[sol];

                veri[sol++] = veri[sag];
                veri[sag--] = temp;
            }
            return veri;
        }

        bool sonrakiIslemiBul(List<int> veri)
        {

            if (veri.Count <= 1)
                return false;

            int son = veri.Count - 2;
            while (son >= 0)
            {
                if (veri[son] < veri[son + 1])
                {
                    break;
                }
                son--;
            }

            if (son < 0)
                return false;

            int sonrakiBuyukEleman = veri.Count - 1;
            for (int i = veri.Count - 1; i > son; i--)
            {
                if (veri[i] > veri[son])
                {
                    sonrakiBuyukEleman = i;
                    break;
                }
            }
            veri = degistir(veri, sonrakiBuyukEleman, son);
            veri = tersineCevir(veri, son + 1, veri.Count - 1);
            return true;
        }
    }
}
