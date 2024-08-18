using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Etut_Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection baglan = new SqlConnection("Data Source=DESKTOP-LO1OC4N\\SQLEXPRESS01;Initial Catalog=EtutTest;Integrated Security=True");
        bool durum;
        void dersListele()
        {
            SqlDataAdapter da = new SqlDataAdapter("select * from tbldersler", baglan);
            DataTable dt = new DataTable();
            da.Fill(dt);
            cbbDers.ValueMember = "dersid";
            cbbDers.DisplayMember = "dersad";
            cbbDers.DataSource = dt;


            cbbOgrtDers.ValueMember = "dersid";
            cbbOgrtDers.DisplayMember = "dersad";
            cbbOgrtDers.DataSource = dt;

        }
        //etut listesi
        void etutListele()
        {

            SqlDataAdapter da2 = new SqlDataAdapter("select id,dersad,(tblogretmen.ad+' '+tblogretmen.soyad) as 'Ogretmen',tarih,saat,durum\r\nfrom tbletut\r\ninner join tbldersler\r\non\r\ntbletut.dersid=tbldersler.dersid\r\ninner join tblogretmen\r\non\r\ntbletut.ogretmenid=tblogretmen.ogrtid\r\n", baglan);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            dataGridView.DataSource = dt2;
        }
        void ogrenciListele()
        {

            SqlDataAdapter da2 = new SqlDataAdapter("select * from tblogrenci", baglan);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            dataGridView.DataSource = dt2;
        }
        void ogretmenListele()
        {

            SqlDataAdapter da2 = new SqlDataAdapter("select * from tblogretmen", baglan);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            dataGridView.DataSource = dt2;
        }
        void mukerrer()
        {
            baglan.Open();
            SqlCommand komut = new SqlCommand("select * from tbldersler where dersad=@dersad", baglan);
            komut.Parameters.AddWithValue("@dersad", txtDers.Text.ToLower());
            SqlDataReader dr = komut.ExecuteReader();
            if (dr.Read())
            {
                durum = false;
            }
            else
            {
                durum = true;
            }
            baglan.Close();
        }

        private void cbbDers_SelectedIndexChanged(object sender, EventArgs e)
        {
            SqlDataAdapter da2 = new SqlDataAdapter("select * from tblogretmen where bransid=" + cbbDers.SelectedValue, baglan);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            cbbOgretmen.ValueMember = "ogrtid";
            cbbOgretmen.DisplayMember = "Ad";
            cbbOgretmen.DataSource = dt2;
        }

        private void btnEtutOlustur_Click(object sender, EventArgs e)
        {
            //etut olustur
            baglan.Open();
            SqlCommand cmd = new SqlCommand("insert into tbletut (dersid,ogretmenid,tarih,saat) values  (@dersid,@ogretmenid,@tarih,@saat)", baglan);
            cmd.Parameters.AddWithValue("@dersid", cbbDers.SelectedValue);
            cmd.Parameters.AddWithValue("@ogretmenid", cbbOgretmen.SelectedValue);
            cmd.Parameters.AddWithValue("@tarih", mtbTarih.Text);
            cmd.Parameters.AddWithValue("@saat", mtbSaat.Text);
            cmd.ExecuteNonQuery();
            baglan.Close();
            MessageBox.Show("etut eklendi", "bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            etutListele();
        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int secilen = dataGridView.SelectedCells[0].RowIndex;
            txtEtutId.Text = dataGridView.Rows[secilen].Cells[0].Value.ToString();

        }

        private void btnEtutDetay_Click(object sender, EventArgs e)
        {
            //etut ver
            baglan.Open();
            SqlCommand komut = new SqlCommand("update  tbletut set ogrenciid=@ogrenciid,durum=@durum where id=@id", baglan);
            komut.Parameters.AddWithValue("@ogrenciid", txtOgrenci.Text);
            komut.Parameters.AddWithValue("@durum", "True");
            komut.Parameters.AddWithValue("@id", txtEtutId.Text);
            komut.ExecuteNonQuery();

            SqlDataAdapter da2 = new SqlDataAdapter("select id,dersad,(tblogretmen.ad+' '+tblogretmen.soyad) as 'Ogretmen',(tblogrenci.ad+' '+tblogrenci.soyad) as 'Ogrenci',tarih,saat,durum\r\nfrom tbletut\r\ninner join tbldersler\r\non\r\ntbletut.dersid=tbldersler.dersid\r\ninner join tblogretmen\r\non\r\ntbletut.ogretmenid=tblogretmen.ogrtid\r\n inner join tblogrenci on tbletut.ogrenciid=tblogrenci.ogrid", baglan);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            dataGridView.DataSource = dt2;
            baglan.Close();
            MessageBox.Show("etut ogrenciye verildi", "bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnFotograf_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            pictureBox1.ImageLocation = openFileDialog1.FileName;
        }

        private void btnOgrenciEkle_Click(object sender, EventArgs e)
        {
            baglan.Open();
            SqlCommand komut = new SqlCommand("insert into tblogrenci (ad,soyad,fotograf,sinif,telefon,mail) values" +
                " (@ad,@soyad,@fotograf,@sinif,@telefon,@mail)", baglan);
            komut.Parameters.AddWithValue("@ad", txtAd.Text);
            komut.Parameters.AddWithValue("@soyad", txtSoyad.Text);
            komut.Parameters.AddWithValue("@fotograf", pictureBox1.ImageLocation);
            komut.Parameters.AddWithValue("@sinif ", txtSinif.Text);
            komut.Parameters.AddWithValue("@telefon", mtbTelefon.Text);
            komut.Parameters.AddWithValue("@mail", txtMail.Text);
            komut.ExecuteNonQuery();
            baglan.Close();
            MessageBox.Show("Ogrenci basayirla eklendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ogrenciListele();
        }

        private void btnOgrtEkle_Click(object sender, EventArgs e)
        {
            baglan.Open();
            SqlCommand komut = new SqlCommand("insert into tblogretmen (ad,soyad,bransid) values (@ad,@soyad,@bransid) ", baglan);
            komut.Parameters.AddWithValue("@ad", txtOgrtAd.Text);
            komut.Parameters.AddWithValue("@soyad", txtOgrtSoyad.Text);
            komut.Parameters.AddWithValue("@bransid", cbbOgrtDers.SelectedValue);
            komut.ExecuteNonQuery();
            baglan.Close();
            ogretmenListele();
        }

        private void btnDersEkle_Click(object sender, EventArgs e)
        {
            mukerrer();
            if (durum == true)
            {
                baglan.Open();
                SqlCommand komut = new SqlCommand("insert into tbldersler (dersad) values (@dersad) ", baglan);
                komut.Parameters.AddWithValue("@dersad", txtDers.Text.ToLower());
                komut.ExecuteNonQuery();
                baglan.Close();
                MessageBox.Show("ders basariyla eklendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Bu ders zaten kayitli", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            SqlDataAdapter da = new SqlDataAdapter("select * from tbldersler", baglan);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView.DataSource = dt;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            etutListele();
            dersListele();
        }
    }
}
