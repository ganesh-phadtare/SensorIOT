using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using SensorIOT.Helpers;
using System.Diagnostics;
using SensorIOT.Communication;

namespace SensorIOT
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void OnFrameChanged(object o, EventArgs e)
        {
            //Force a call to the Paint event handler.
            this.Invalidate();
        }

        SensorApiProxy proxy = new SensorApiProxy();

        Bitmap animatedImage = new Bitmap(@"D:/load1.gif");

        private void btnStart_Click(object sender, EventArgs e)
        {
            //Begin the animation.
            ImageAnimator.Animate(animatedImage, OnFrameChanged);

            //Get the next frame ready for rendering.
            ImageAnimator.UpdateFrames();

            //Draw the next frame in the animation.
            this.CreateGraphics().DrawImage(this.animatedImage, new Point(0, 0));

            int noOfRecords = 0;

            if (!string.IsNullOrEmpty(txtNoOfRecord.Text) && txtNoOfRecord.Text != "0")
            {
                int.TryParse(txtNoOfRecord.Text, out noOfRecords);
            }
            IntitalProcess(noOfRecords);
        }


        private void btl_TimeWiseStart_Click(object sender, EventArgs e)
        {
            int noOfRecords = 0;

            if (!string.IsNullOrEmpty(txtTimeWise.Text) && txtTimeWise.Text != "0")
            {
                int.TryParse(txtTimeWise.Text, out noOfRecords);
            }
            IntitalTimeWiseProcess(noOfRecords);
        }

        private async void IntitalProcess(int noOfRecords)
        {
            Stopwatch watch = new Stopwatch();


            if (noOfRecords > 0)
            {
                for (int i = 0; i <= noOfRecords - 1; i++)
                {
                    await InsertData(watch);
                }
            }

            double timeTaken = watch.ElapsedMilliseconds;

            label3.Text = timeTaken.ToString();
            label7.Text = Convert.ToString(Math.Round(timeTaken / 600));
            label10.Text = Convert.ToString(Math.Round(timeTaken / 60000));
        }

        private long CovertToMillis(int noOfMinutes)
        {
            return noOfMinutes * 60000;
        }

        private async void IntitalTimeWiseProcess(int noOfMinutes)
        {
            Stopwatch watch = new Stopwatch();

            Stopwatch loopWatch = new Stopwatch();

            loopWatch.Start();
            int noOfRecords = 0;
            while (loopWatch.ElapsedMilliseconds <= CovertToMillis(noOfMinutes))
            {
                await InsertData(watch);
                noOfRecords += 1;
            }
            loopWatch.Stop();

            watch.Stop();

            label4.Text = noOfRecords.ToString();
        }

        public async Task InsertData(Stopwatch watch)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string firstName = MockData.Person.FirstName();
            string surname = MockData.Person.Surname();

            dict.Add("@firstName", firstName);
            dict.Add("@surname", surname);
            dict.Add("@fullName", string.Concat(firstName, " ", surname));
            dict.Add("@age", MockData.RandomNumber.Next(99).ToString());
            dict.Add("@city", MockData.Address.City());
            dict.Add("@state", MockData.Address.State());
            dict.Add("@phoneNo", MockData.RandomNumber.Next(100000000, 999999999).ToString());

            watch.Start();
            await proxy.Fetch(1, dict);
            watch.Stop();
        }
    }
}
