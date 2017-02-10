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

namespace SensorIOT
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            int noOfRecords = 0;
            if (!string.IsNullOrEmpty(txtNoOfRecord.Text) && txtNoOfRecord.Text != "0")
            {
                int.TryParse(txtNoOfRecord.Text, out noOfRecords);
            }
            if (noOfRecords > 0)
            {

                for (int i = 0; i <= noOfRecords; i++)
                {
                    InsertData();
                }
            }
        }

        public async void InsertData()
        {
            string firstName = MockData.Person.FirstName();
            string surname = MockData.Person.Surname();
            string fullName = string.Concat(firstName, " ", surname);
            int age = MockData.RandomNumber.Next(99);
            string city = MockData.Address.City();
            string state = MockData.Address.State();
            int phoneNo = MockData.RandomNumber.Next(100000000, 999999999);

            await ExecuteData(firstName, surname, fullName, age, city, state, phoneNo);
        }

        public Task<int> ExecuteData(string firstName, string surname, string fullName, int age, string city, string state, int phoneNo)
        {
            Stopwatch innerWatch = new Stopwatch();
            innerWatch.Start();

            SqlConnection con = new SqlConnection("Data Source=192.168.35.116;Initial Catalog=BulkDB;User Id=sa;Password=mail_123");

            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();

                string command = "insert into EmployeeMaster(FirstName, LastName,FullName, Age, City, State, PhoneNo) Values(@firstName,@surname,@fullName,@age,@city,@state,@phoneNo)";

                SqlCommand sqlCommand = new SqlCommand(command, con);
                sqlCommand.Parameters.Add(new SqlParameter("@firstName", firstName));
                sqlCommand.Parameters.Add(new SqlParameter("@surname", surname));
                sqlCommand.Parameters.Add(new SqlParameter("@fullName", fullName));
                sqlCommand.Parameters.Add(new SqlParameter("@age", age));
                sqlCommand.Parameters.Add(new SqlParameter("@city", city));
                sqlCommand.Parameters.Add(new SqlParameter("@state", state));
                sqlCommand.Parameters.Add(new SqlParameter("@phoneNo", phoneNo));

                sqlCommand.ExecuteNonQuery();
                innerWatch.Stop();
                AppLogger.LogTimer(innerWatch);
            }
            catch (Exception ex)
            {
                innerWatch.Stop();
                AppLogger.LogError(ex);
                return new Task<int>(1);
            }
            finally
            {
                if (con.State != ConnectionState.Open)
                    con.Close();
                innerWatch.Stop();
            }
            return new Task<int>(0);
        }
    }
}
