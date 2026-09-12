using System;
using System.Data.SqlClient;
using System.Web.UI;
using skillExchange.App_Start;

namespace skillExchange
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e) { }
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            if (String.IsNullOrWhiteSpace(txtEmail.Text) || String.IsNullOrWhiteSpace(txtPassword.Text)) { lblMessage.Text = "Please enter your email and password."; return; }

            int userId = 0; string fullName = ""; string role = ""; string storedPassword = "";
            using (SqlConnection con = new SqlConnection(DB.connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT userId, fullName, role, password FROM users WHERE email = @email", con))
            {
                cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) { userId = Convert.ToInt32(reader["userId"]); fullName = reader["fullName"].ToString(); role = reader["role"].ToString(); storedPassword = reader["password"].ToString(); }
                }

                bool valid = PasswordHelper.Verify(txtPassword.Text, storedPassword);
                bool legacyPassword = PasswordHelper.IsLegacyPlainText(storedPassword) && storedPassword == txtPassword.Text;
                if (!valid && !legacyPassword) { lblMessage.Text = "Invalid email or password."; return; }

                if (legacyPassword)
                {
                    using (SqlCommand upgradeCmd = new SqlCommand("UPDATE users SET password = @password WHERE userId = @userId", con))
                    {
                        upgradeCmd.Parameters.AddWithValue("@password", PasswordHelper.Hash(txtPassword.Text));
                        upgradeCmd.Parameters.AddWithValue("@userId", userId);
                        upgradeCmd.ExecuteNonQuery();
                    }
                }
            }

            Session["userId"] = userId.ToString();
            Session["fullName"] = fullName;
            Session["role"] = role;
            Response.Redirect(role == "Admin" ? "Admin.aspx" : "Default.aspx");
        }
    }
}
