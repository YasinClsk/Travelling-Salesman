using GMap.NET;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace KargoDagitimSistemi
{
    class DataAccess
    {
        public static bool merkezBelirlendiMi = false;

        MapOperation mapOperation = new MapOperation();
        public static List<Coordinates> coordinates = new List<Coordinates>();
        Random random = new Random();

        string sqlConnectionString = "Server=tcp:kargosql.database.windows.net,1433;Initial Catalog=KargoDagitimSistemi;Persist Security Info=False;User ID=atayasin;Password=123qweASD.;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public void ListItems(DataGridView dataGridView)
        {
            SqlConnection sqlConnection = new SqlConnection(sqlConnectionString);
            sqlConnection.Open();

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("Select * from Kargo_Tbl Order By KargoId", sqlConnection);
            DataTable dataTable = new DataTable();
            sqlDataAdapter.Fill(dataTable);

            dataGridView.DataSource = dataTable;

            sqlConnection.Close();
        }

        public void AddItems(string address,string customerName)
        {
            SqlConnection sqlConnection = new SqlConnection(sqlConnectionString);
            sqlConnection.Open();

            if (address != "" && customerName != "")
            {
                SqlCommand sqlCommand = new SqlCommand("Insert into Kargo_Tbl(MusteriAdi, MusteriLokasyon, TeslimDurumu) values(@p1, @p2,@p3) Select SCOPE_IDENTITY()", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@p1", customerName);
                sqlCommand.Parameters.AddWithValue("@p2", address);
                sqlCommand.Parameters.AddWithValue("@p3", false);

                int cargoId = Convert.ToInt32(sqlCommand.ExecuteScalar());
                
                PointLatLng point = new PointLatLng();
                point = mapOperation.FindLatLng(address, point);

                coordinates.Add(new Coordinates
                {
                    Point = point,
                    Color = new Pen(Color.FromArgb(random.Next(100, 256), random.Next(100, 256), random.Next(100, 256))),
                    CustomerName = customerName,
                    DeliveryStatus = false,
                    CargoId = cargoId,
                    Address = address
                });

                MessageBox.Show("Kargo bilgisi eklendi");
            }

            else
                MessageBox.Show("Adres veya isim bilgisi boş");
            sqlConnection.Close();
        }

        public void DeleteItems(int kargoId)
        {
            SqlConnection sqlConnection = new SqlConnection(sqlConnectionString);
            sqlConnection.Open();

            SqlCommand sqlCommand = new SqlCommand("Delete from Kargo_Tbl Where KargoId=@p1", sqlConnection);
            sqlCommand.Parameters.AddWithValue("@p1", kargoId);
            sqlCommand.ExecuteNonQuery();

            coordinates.RemoveAll(p => p.CargoId == kargoId);

            MessageBox.Show("Kayıt silinmiştir");
            sqlConnection.Close();
        }

        public void UpdateItems(int kargoId)
        {
            SqlConnection sqlConnection = new SqlConnection("Server=tcp:kargosql.database.windows.net,1433;Initial Catalog=KargoDagitimSistemi;Persist Security Info=False;User ID=atayasin;Password=123qweASD.;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            sqlConnection.Open();

            SqlCommand sqlCommand = new SqlCommand("Update Kargo_Tbl Set TeslimDurumu=@p1 where KargoId = @p2", sqlConnection);
            sqlCommand.Parameters.AddWithValue("@p1", true);
            sqlCommand.Parameters.AddWithValue("@p2", kargoId);
            sqlCommand.ExecuteNonQuery();

            foreach (var coordinate in coordinates)
            {
                if(coordinate.CargoId == kargoId)
                {
                    coordinate.DeliveryStatus = true;
                }
            }

            MessageBox.Show("Kargo teslim edilmiştir");
            sqlConnection.Close();
        }
        
        public void GetData()
        {
            
            SqlConnection sqlConnection = new SqlConnection(sqlConnectionString);
            sqlConnection.Open();

            SqlCommand sqlCommand = new SqlCommand("Select * from Kargo_Tbl Order By KargoId", sqlConnection);
            SqlDataReader dr;
            dr = sqlCommand.ExecuteReader();

            while (dr.Read())
            {
                string location = dr["MusteriLokasyon"].ToString();
                string customerName = dr["MusteriAdi"].ToString();
                int cargoId = Convert.ToInt32(dr["KargoId"]);
                bool deliveryStatus = Convert.ToBoolean(dr["TeslimDurumu"]);

                PointLatLng point = new PointLatLng();
                point = mapOperation.FindLatLng(location,point);

                coordinates.Add(new Coordinates
                {
                    Point = point,
                    Color = new Pen(Color.FromArgb(random.Next(100, 256), random.Next(100, 256), random.Next(100, 256))),
                    CustomerName = customerName,
                    DeliveryStatus = deliveryStatus,
                    CargoId = cargoId,
                    Address = location
                });

            }
        }
    }
}
