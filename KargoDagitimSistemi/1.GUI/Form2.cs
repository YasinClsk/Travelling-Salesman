using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using KargoDagitimSistemi._1.GUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace KargoDagitimSistemi
{
    public partial class Form2 : Form
    {
        DataAccess dataAccess;
        GMapMarker marker;
        GMapOverlay gMapOverlay;

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            dataAccess = new DataAccess();
            MapSettings();
            gMapOverlay = new GMapOverlay("markers");
            VeriTabaniIslemleri();
            Pointer();
        }


        void MapSettings()
        {
            MapOperation mapOperation = new MapOperation();
            mapOperation.InitialSettings(gMap);
        }

        void Pointer()
        {
            while (gMap.Overlays.Count > 0)
                gMap.Overlays.RemoveAt(0);

            gMap.Overlays.Clear();
            gMapOverlay.Markers.Clear();


            for (int i = 0; i < DataAccess.coordinates.Count; i++)
            {
                if(i==0 && DataAccess.merkezBelirlendiMi)
                    marker = new GMarkerGoogle(DataAccess.coordinates[0].Point, GMarkerGoogleType.blue);

                else if (DataAccess.coordinates[i].DeliveryStatus==false)
                    marker = new GMarkerGoogle(DataAccess.coordinates[i].Point, GMarkerGoogleType.red);
                else
                    marker = new GMarkerGoogle(DataAccess.coordinates[i].Point, GMarkerGoogleType.green);
                gMapOverlay.Markers.Add(marker);
                gMap.Overlays.Add(gMapOverlay);
            }
            gMap.Zoom = gMap.Zoom + 0.000001;
        }

        private void map_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var point = gMap.FromLocalToLatLng(e.X, e.Y);
                marker = new GMarkerGoogle(point, GMarkerGoogleType.red);

                List<Placemark> placemark = null;
                var stat = GMapProviders.GoogleMap.GetPlacemarks(point, out placemark);
                if (stat == GeoCoderStatusCode.OK)
                {
                    tbxAdres.Text = placemark[0].Address;
                }
            }
        }

        private void btnAdres_Click(object sender, EventArgs e)
        {
            dataAccess.AddItems(tbxAdres.Text, tbxMusteri.Text);
            VeriTabaniIslemleri();
            Pointer();

            if(DataAccess.merkezBelirlendiMi)
                GirisFrm.gui2.FindDistance();
        }

        void VeriTabaniIslemleri()
        {
            dataAccess.ListItems(dataGridView1);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            tbxAdres.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            tbxMusteri.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();

            PointLatLng pointLat = new PointLatLng();
            MapOperation mapOperation = new MapOperation();

            pointLat = mapOperation.FindLatLng(tbxAdres.Text, pointLat);

            gMap.Position = pointLat;
            gMap.Zoom = 15;

        }

        private void btnSil_Click(object sender, EventArgs e)
        {

            int kargoId = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);
            dataAccess.DeleteItems(kargoId);

            VeriTabaniIslemleri();
            Pointer();
            if (DataAccess.merkezBelirlendiMi)
                GirisFrm.gui2.FindDistance();
        }

        private void btnTeslim_Click(object sender, EventArgs e)
        {
            int kargoId = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value);

            int tut=0;
            for (int i = 0; i < DataAccess.coordinates.Count; i++)
            {
                if (DataAccess.coordinates[i].CargoId == kargoId)
                    tut = i;

            }
            dataAccess.UpdateItems(kargoId);

            DataAccess.coordinates[0].Point = DataAccess.coordinates[tut].Point;
            DataAccess.coordinates[0].Address = DataAccess.coordinates[tut].Address;

            VeriTabaniIslemleri();
            Pointer();

            if (DataAccess.merkezBelirlendiMi)
                GirisFrm.gui2.FindDistance();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MapOperation mapOperation = new MapOperation();
            PointLatLng pointLatLng = new PointLatLng();
            pointLatLng = mapOperation.FindLatLng(tbxAdres.Text, pointLatLng);

            Coordinates merkezNoktasi = new Coordinates()
            {
                Address = tbxAdres.Text,
                CargoId = 0,
                Color = new Pen(Color.Pink),
                CustomerName = "Atakan",
                DeliveryStatus = false,
                Point = pointLatLng
            };

            if (!DataAccess.merkezBelirlendiMi)
            {
                Coordinates coordinatesTut = new Coordinates();
                coordinatesTut = DataAccess.coordinates[0];
                DataAccess.coordinates[0] = merkezNoktasi;
                DataAccess.coordinates.Add(coordinatesTut);
            }
            else
            {
                DataAccess.coordinates[0] = merkezNoktasi;
            }

            DataAccess.merkezBelirlendiMi = true;

            marker = new GMarkerGoogle(pointLatLng, GMarkerGoogleType.blue);
            gMapOverlay.Markers.Add(marker);
            gMap.Overlays.Add(gMapOverlay);

            try
            {
                GirisFrm.gui2.FindDistance();
            }
            catch (Exception){}
            
        }
    }
}
