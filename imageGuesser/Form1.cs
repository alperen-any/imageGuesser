using System.Drawing;
using System.Collections.Generic;
using SolrNet.Utils;

namespace imageGuesser
{
    public partial class Form1 : Form
    {
        // deneme tekrar

        bool isBusy = false; //hýzlý týklamayý engellemek için yapay zeka önerdi
        int oy1skor = 0;
        int oy2skor = 0;
        int mevcutOy = 1;
        int kalanSure;

        PictureBox firstChoose;
        PictureBox secondChoose;

        Dictionary<int, Image> imageNumber = new Dictionary<int, Image>();

        int[] kartNum = [0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13, 14, 14, 15, 15, 16, 16, 17, 17, 18, 18, 19, 19];

        List<PictureBox> kartlar = new List<PictureBox>();

        public Form1()
        {
            InitializeComponent();
            imageNumLoad();
        }

        // kartlarda kullandýðým görsellerin sözlükte tutulmasý ve numaralandýrýlmasý
        private void imageNumLoad()
        {
            imageNumber.Add(0, Properties.Resources.apple);
            imageNumber.Add(1, Properties.Resources.bee);
            imageNumber.Add(2, Properties.Resources.carrot);
            imageNumber.Add(3, Properties.Resources.cat);
            imageNumber.Add(4, Properties.Resources.crab);
            imageNumber.Add(5, Properties.Resources.cucumber);
            imageNumber.Add(6, Properties.Resources.dolphin);
            imageNumber.Add(7, Properties.Resources.elephant);
            imageNumber.Add(8, Properties.Resources.fox);
            imageNumber.Add(9, Properties.Resources.frog);
            imageNumber.Add(10, Properties.Resources.hamster);
            imageNumber.Add(11, Properties.Resources.hippopotamus);
            imageNumber.Add(12, Properties.Resources.koala);
            imageNumber.Add(13, Properties.Resources.lemon);
            imageNumber.Add(14, Properties.Resources.monkey);
            imageNumber.Add(15, Properties.Resources.passionfruit);
            imageNumber.Add(16, Properties.Resources.penguin);
            imageNumber.Add(17, Properties.Resources.seal);
            imageNumber.Add(18, Properties.Resources.strawberry);
            imageNumber.Add(19, Properties.Resources.watermelon);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (PictureBox pb in tableLayoutPanel2.Controls.OfType<PictureBox>())
            {
                kartlar.Add(pb);
                pb.Click += Kart_Click;
            }
            timer1.Tick += timerTick;
            yanlisTimer.Tick += YanlisTimer_Tick;
            timer2.Tick += Timer2_Tick;
        }

        private void Kart_Click(object? sender, EventArgs e)
        {
            if (isBusy) return;
            PictureBox tiklanan = sender as PictureBox;
            if (tiklanan.Enabled == false)
            {
                return;
            }

            if (firstChoose == null)
            {
                firstChoose = tiklanan;
                int id = (int)firstChoose.Tag;
                firstChoose.Image = imageNumber[id];
                firstChoose.Enabled = false;

                kalanSure = 5;
                lblGeri.Text = $"KALAN SÜRE: {kalanSure.ToString()}";
                lblGeri.BackColor = Color.Red;

                timer2.Interval = 1000;
                timer2.Start();
            }
            else if (secondChoose == null)
            {
                timer2.Stop();
                lblGeri.Text = "KALAN SÜRE: 5";
                lblGeri.BackColor = Color.Green;
                isBusy = true;
                secondChoose = tiklanan;
                int id = (int)secondChoose.Tag;
                secondChoose.Image = imageNumber[id];

                if ((int)firstChoose.Tag == (int)secondChoose.Tag)
                {
                    if (mevcutOy == 1)
                    {
                        oy1skor++;
                        label1.Text = $"Oyuncu 1: {oy1skor.ToString()} puan";
                    }
                    else
                    {
                        oy2skor++;
                        label2.Text = $"Oyuncu 2: {oy2skor.ToString()} puan";
                    }

                    firstChoose.Enabled = false;
                    secondChoose.Enabled = false;

                    firstChoose = null;
                    secondChoose = null;
                    
                    isBusy = false;
                    CheckForWinner();
                }
                else
                {
                    isBusy = true;
                    yanlisTimer.Interval = 1000;
                    yanlisTimer.Start();
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            oyuncuKontrol();
            button1.Enabled = false;
            button1.BackColor = Color.Gray;
            Random rnd = new Random();
            int[] karistir = kartNum.OrderBy(x => rnd.Next()).ToArray();

            for (int i = 0; i < kartlar.Count; i++)
            {
                kartlar[i].Enabled = false;
                kartlar[i].Tag = karistir[i];
            }

            foreach (var kart in kartlar)
            {
                int id = (int)kart.Tag;
                kart.Image = imageNumber[id];
            }

            timer1.Interval = 5000;
            timer1.Start();

        }

        // 2. kartýn seçim süresi ve seçilmemesinde olacaklarýn kontrolü
        private void Timer2_Tick(object? sender, EventArgs e)
        {
            kalanSure--;
            lblGeri.Text = $"KALAN SÜRE: {kalanSure.ToString()}";

            if (kalanSure == 0)
            {
                timer2.Stop();
                isBusy = true;

                firstChoose.Image = Properties.Resources.bg_pattern;
                firstChoose.Enabled = true;
                firstChoose = null;

                lblGeri.Text = "KALAN SÜRE: 5";
                oyuncuKontrol();

                mevcutOy = (mevcutOy == 1) ? 2 : 1;
                label3.Text = $"Mecut Oyuncu: {mevcutOy}";
                isBusy = false;
            }
        }

        private void timerTick(object? sender, EventArgs e)
        {
            timer1.Stop();
            foreach (var kart in kartlar)
            {
                kart.Image = Properties.Resources.bg_pattern;
                kart.Enabled = true;
            }
        }

        // Yanlýþ seçim yapýldýðýnda kartlarýn kapanmasý ve diðer oyuncuya geçiþ
        private void YanlisTimer_Tick(object? sender, EventArgs e)
        {
            yanlisTimer.Stop();
            firstChoose.Image = Properties.Resources.bg_pattern;
            secondChoose.Image = Properties.Resources.bg_pattern;

            firstChoose.Enabled = true;
            secondChoose.Enabled = true;

            firstChoose = null;
            secondChoose = null;

            if(mevcutOy == 1)
            {
                mevcutOy = 2;
            }
            else
            {
                mevcutOy = 1;
            }
            isBusy = false;
            label3.Text = $"Mevcut Oyuncu: {mevcutOy}";
            oyuncuKontrol();
        }

        // Sýradaki oyuncu için renk deðiþimi
        private void oyuncuKontrol()
        {
            if (mevcutOy == 1)
            {
                label1.ForeColor = Color.White;
                label1.BackColor = Color.Red;
                label2.BackColor = Color.Transparent;
                label2.ForeColor = Color.Black;
            }
            else
            {
                label2.ForeColor = Color.White;
                label2.BackColor = Color.Red;
                label1.BackColor = Color.Transparent;
                label1.ForeColor = Color.Black;
            }
        }

        // Oyunun bitmesi durumunda kazananýn belirlenmesi, ekrana skorun ve tebriðin yazýlmasý. Tekrar baþlatýldýðýnda skorlarýn sýfýrlanmasý
        private void CheckForWinner()
        {
            if (oy1skor == 11 || oy2skor == 11 || oy1skor + oy2skor == 20)
            {
                string kazanan = (oy1skor > oy2skor) ? "1. Oyuncu" : "2. Oyuncu";
                MessageBox.Show("Oyun Bitti! Kazanan: " + kazanan + " Tebrikler!!!\n" + $"Sonuç: {oy1skor} - {oy2skor}");
                button1.Text = "Yeniden Baþla";
                button1.Enabled = true;
                oy1skor = 0;
                oy2skor = 0;
                label1.Text = "Oyuncu 1: ";
                label2.Text = "Oyuncu 2: ";
            }
        }
    } 
}
