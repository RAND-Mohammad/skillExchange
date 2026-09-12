using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.UI;
using skillExchange.App_Start;

namespace skillExchange
{
    public partial class Register : Page
    {
        protected void Page_Load(object sender, EventArgs e) { }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Please enter your full name.";
                return;
            }
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Please enter your email.";
                return;
            }
            if (!Regex.IsMatch(txtEmail.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Please enter a valid email address.";
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Please enter your password.";
                return;
            }
            if (txtPassword.Text.Trim().Length < 6)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Password must be at least 6 characters.";
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["SkillExchangeDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string checkQuery = "SELECT COUNT(*) FROM users WHERE email = @email";
                SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                checkCmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                int count = (int)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    lblMessage.Text = "Email already exists!";
                    return;
                }

                string insertQuery = @"INSERT INTO users
                    (fullName, email, password, phoneNumber, country, bio, createdDate, role)
                    VALUES
                    (@fullName, @email, @password, @phoneNumber, @country, @bio, GETDATE(), 'User')";

                SqlCommand cmd = new SqlCommand(insertQuery, con);
                cmd.Parameters.AddWithValue("@fullName", txtFullName.Text.Trim());
                cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@password", PasswordHelper.Hash(txtPassword.Text.Trim()));
                cmd.Parameters.AddWithValue("@phoneNumber", string.IsNullOrWhiteSpace(txtPhoneNumber.Text) ? (object)DBNull.Value : txtPhoneNumber.Text.Trim());
                cmd.Parameters.AddWithValue("@country", string.IsNullOrWhiteSpace(txtCountry.Text) ? (object)DBNull.Value : txtCountry.Text.Trim());
                cmd.Parameters.AddWithValue("@bio", string.IsNullOrWhiteSpace(txtBio.Text) ? (object)DBNull.Value : txtBio.Text.Trim());

                cmd.ExecuteNonQuery();

                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Text = "Account created successfully! Please login.";
                Response.AddHeader("REFRESH", "2;URL=Login.aspx");

                txtFullName.Text = "";
                txtEmail.Text = "";
                txtPassword.Text = "";
                txtPhoneNumber.Text = "";
                txtCountry.Text = "";
                txtBio.Text = "";
            }
        }
    }
}
