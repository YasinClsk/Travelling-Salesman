using System;
using System.Data.SqlClient;
using System.Threading;
using System.Windows.Forms;

namespace KargoDagitimSistemi._1.GUI
{
    public partial class GirisFrm : Form
    {

        public static Form2 form2;
        public static GUI2 gui2;

        public GirisFrm()
        {
            InitializeComponent();
        }

        private void btnKayitOl_Click(object sender, EventArgs e)
        {
            string sqlConnectionString = "Server=tcp:kargosql.database.windows.net,1433;Initial Catalog=KargoDagitimSistemi;Persist Security Info=False;User ID=atayasin;Password=123qweASD.;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            SqlConnection sqlConnection = new SqlConnection(sqlConnectionString);
            sqlConnection.Open();

            if (tbxKayitAd.Text != "" && tbxKayitSifre.Text != "")
            {
                SqlCommand sqlCommand = new SqlCommand("Insert into Kullanici_Tbl(K_Adi, Sifre) values(@p1, @p2) ", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@p1", tbxKayitAd.Text);
                sqlCommand.Parameters.AddWithValue("@p2", tbxKayitSifre.Text);

                sqlCommand.ExecuteNonQuery();
                MessageBox.Show(tbxKayitAd.Text + " ismindeki kullanıcının kayıt olma işlemi gerçekleşmiştir");
            }

            else
            {
                MessageBox.Show("Ad ve şifre alanı boş bırakılamaz");
            }

            sqlConnection.Close();
        }

        private void btnGiris_Click(object sender, EventArgs e)
        {
            string sqlConnectionString = "Server=tcp:kargosql.database.windows.net,1433;Initial Catalog=KargoDagitimSistemi;Persist Security Info=False;User ID=atayasin;Password=123qweASD.;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            SqlConnection sqlConnection = new SqlConnection(sqlConnectionString);
            sqlConnection.Open();

            SqlCommand command = new SqlCommand("Select * from Kullanici_Tbl where K_Adi = @p1 and Sifre = @p2", sqlConnection);
            command.Parameters.AddWithValue("@p1", tbxKAdi.Text.Trim());
            command.Parameters.AddWithValue("@p2", tbxSifre.Text.Trim());
            SqlDataReader reader = command.ExecuteReader();

            if(reader.Read())
            {
                this.Close();

                DataAccess dataAccess = new DataAccess();
                dataAccess.GetData();
                form2 = new Form2();
                gui2 = new GUI2();

                Thread thread = new Thread(Deneme);
                thread.Start();


                Thread thread2 = new Thread(Deneme2);
                thread2.Start();
            }
            else
            {
                MessageBox.Show("Hatalı giriş");
            }
        }

        static void Deneme()
        {
            try
            {
                Application.Run(gui2);
            }
            catch (Exception)
            {}
        }

        static void Deneme2()
        {
            try
            {
                Application.Run(form2);
            }catch(Exception)
            { }
        }

        private void btnSifreUnuttum_Click(object sender, EventArgs e)
        {
            string sqlConnectionString = "Server=tcp:kargosql.database.windows.net,1433;Initial Catalog=KargoDagitimSistemi;Persist Security Info=False;User ID=atayasin;Password=123qweASD.;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            SqlConnection sqlConnection = new SqlConnection(sqlConnectionString);
            sqlConnection.Open();

            SqlCommand command = new SqlCommand("Select * from Kullanici_Tbl where K_Adi = @p1", sqlConnection);
            command.Parameters.AddWithValue("@p1", tbxYenilemeAd.Text.Trim());

            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                reader.Close();

                SqlCommand sqlCommand = new SqlCommand("update Kullanici_Tbl set Sifre=@p1 where K_Adi = @p2", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@p1", tbxYenilemeSifre.Text);
                sqlCommand.Parameters.AddWithValue("@p2", tbxYenilemeAd.Text);

                sqlCommand.ExecuteNonQuery();
                MessageBox.Show(tbxYenilemeAd.Text + " isimli kullanıcının şifresi güncellenmiştir");
            }

            else
            {
                MessageBox.Show("Kullanıcı adına ilişkin kayıt bulunamadı");
            }
        }

        private void GirisFrm_Load(object sender, EventArgs e)
        {

        }

        private void tbxKAdi_Click(object sender, EventArgs e)
        {
            ((TextBox)sender).Text = "";
        }

        private void tbxKAdi_Leave(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text == "")
                ((TextBox)sender).Text = "Kullanıcı Adı";
        }

        private void tbxSifre_Leave(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text == "")
            {
                ((TextBox)sender).PasswordChar = '\0';
                ((TextBox)sender).Text = "Şifre";
            }
                
        }

        private void tbxSifre_Enter(object sender, EventArgs e)
        {
            ((TextBox)sender).Text = "";
            ((TextBox)sender).PasswordChar = '*';
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            panel2.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel2.Show();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            panel2.Hide();
            panel3.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel2.Show();
            panel3.Show();
        }
    }
}
