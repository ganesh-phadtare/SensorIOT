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
using System.Threading;

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
            Reset();
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
            IntitateProcess(noOfRecords);
        }

        private void btl_TimeWiseStart_Click(object sender, EventArgs e)
        {
            int noOfRecords = 0;

            if (!string.IsNullOrEmpty(txtTimeWise.Text) && txtTimeWise.Text != "0")
            {
                int.TryParse(txtTimeWise.Text, out noOfRecords);
            }
            IntitateTimeWiseProcess(noOfRecords);
        }

        void Reset()
        {
            label3.Text = string.Empty;
            label7.Text = string.Empty;
            label10.Text = string.Empty;
        }

        private void IntitateProcess(int noOfRecords)
        {
            try
            {
                if (noOfRecords > 0)
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();

                    var tasks = new List<Task>();
                    for (int i = 0; i <= noOfRecords - 1; i++)
                    {
                        Task.Run(() =>
                            {
                                InsertData();
                            });

                        //Unused code  required for testing.
                        //tasks.Add(
                        //   Task.Run(() =>
                        //     {
                        //         InsertData();
                        //     })
                        //     );

                        //if (i != 0 && ((i % 5000) == 0))
                        //{
                        //    //Task.WaitAll(tasks.ToArray());
                        //    Thread.Sleep(2000);
                        //    //tasks = new List<Task>();
                        //}
                    }

                    //Task.WaitAll(tasks.ToArray());

                    watch.Stop();
                    SetTimer(watch);
                }

            }
            catch (Exception ex)
            {
                AppLogger.LogError(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Log timers time to label
        /// </summary>
        /// <param name="watch"></param>
        private void SetTimer(Stopwatch watch)
        {
            double timeTaken = watch.ElapsedMilliseconds;

            label3.Text = timeTaken.ToString();
            label7.Text = Convert.ToString(Math.Round(timeTaken / 1000));
            label10.Text = Convert.ToString(Math.Round(timeTaken / 60000));
        }

        private long CovertToMillis(int noOfMinutes)
        {
            return noOfMinutes * 60000;
        }

        private void IntitateTimeWiseProcess(int noOfMinutes)
        {
            try
            {
                Stopwatch watch = new Stopwatch();

                Stopwatch loopWatch = new Stopwatch();

                loopWatch.Start();

                int noOfRecords = 0;

                var tasks = new List<Task>();

                while (loopWatch.ElapsedMilliseconds <= CovertToMillis(noOfMinutes))
                {
                    Task.Run(() =>
                        {
                            InsertData();
                        });
                    noOfRecords += 1;
                }
                loopWatch.Stop();

                watch.Stop();

                label4.Text = noOfRecords.ToString();
            }
            catch (Exception ex)
            {
                AppLogger.LogError(ex);
                throw ex;
            }
        }

        private void InsertData()
        {
            try
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

                proxy.Fetch(GetCommandType(), dict);
            }
            catch (Exception ex)
            {
                AppLogger.LogError(ex);
                throw ex;
            }
        }

        private int GetCommandType()
        {
            if (rbInline.Checked == true)
                return 1;
            else if (rbSp.Checked == true)
                return 2;

            return 0;
        }
    }
}
