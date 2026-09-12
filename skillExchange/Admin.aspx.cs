using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using skillExchange.App_Start;

namespace skillExchange
{
    public partial class Admin : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (Session["role"] == null || Session["role"].ToString() != "Admin")
            {
                Response.Redirect("Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadStats();
                LoadUsers();
                LoadAllSkills();
            }
        }

        private void LoadStats()
        {
            using (SqlConnection sqlConnection = new SqlConnection(DB.connectionString))
            {
                sqlConnection.Open();
                litTotalUsers.Text = RunCount(sqlConnection, "SELECT COUNT(*) FROM users");
                litTotalSkills.Text = RunCount(sqlConnection, "SELECT COUNT(*) FROM skills");
                litTotalRequests.Text = RunCount(sqlConnection, "SELECT COUNT(*) FROM exchangeRequests");
                litTotalFeedback.Text = RunCount(sqlConnection, "SELECT COUNT(*) FROM feedbacks");
            }
        }

        private string RunCount(SqlConnection sqlConnection, string query)
        {
            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
            object result = sqlCommand.ExecuteScalar();
            return result != null ? result.ToString() : "0";
        }

        private void LoadUsers()
        {
            using (SqlConnection sqlConnection = new SqlConnection(DB.connectionString))
            {
                string query = @"SELECT userId, fullName, email, role, createdDate FROM users ORDER BY createdDate DESC";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);
                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);

                gvUsers.DataSource = dataTable;
                gvUsers.DataBind();

                for (int i = 0; i < gvUsers.Rows.Count; i++)
                {
                    DropDownList ddlRole = (DropDownList)gvUsers.Rows[i].FindControl("ddlRole");
                    string currentRole = dataTable.Rows[i]["role"].ToString();
                    if (ddlRole.Items.FindByValue(currentRole) != null)
                    {
                        ddlRole.SelectedValue = currentRole;
                    }
                }
            }
        }

        private void LoadAllSkills()
        {
            using (SqlConnection sqlConnection = new SqlConnection(DB.connectionString))
            {
                string query = @"SELECT
                            s.skillId, s.skillName, u.fullName AS ownerName,
                            c.categoryName, s.skillLevels, s.isAvailable
                         FROM skills s
                         INNER JOIN users u ON s.userId = u.userId
                         INNER JOIN categories c ON s.categoryId = c.categoryId
                         ORDER BY s.createdDate DESC";

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);
                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);

                gvAllSkills.DataSource = dataTable;
                gvAllSkills.DataBind();
            }
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int targetUserId = Convert.ToInt32(e.CommandArgument);
            int currentUserId = Convert.ToInt32(Session["userId"]);

            if (e.CommandName == "UpdateRole")
            {
                GridViewRow row = ((Control)e.CommandSource).NamingContainer as GridViewRow;
                DropDownList ddlRole = (DropDownList)row.FindControl("ddlRole");

                using (SqlConnection sqlConnection = new SqlConnection(DB.connectionString))
                {
                    string updateQuery = "UPDATE users SET role = @role WHERE userId = @userId";
                    SqlCommand sqlCommand = new SqlCommand(updateQuery, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@role", ddlRole.SelectedValue);
                    sqlCommand.Parameters.AddWithValue("@userId", targetUserId);
                    sqlConnection.Open();
                    sqlCommand.ExecuteNonQuery();
                }

                lblAdminMessage.ForeColor = System.Drawing.Color.Green;
                lblAdminMessage.Text = "Role updated successfully.";
                LoadUsers();
            }
            else if (e.CommandName == "DeleteUser")
            {
                if (targetUserId == currentUserId)
                {
                    lblAdminMessage.ForeColor = System.Drawing.Color.Red;
                    lblAdminMessage.Text = "You cannot delete your own account.";
                    return;
                }

                using (SqlConnection sqlConnection = new SqlConnection(DB.connectionString))
                {
                    sqlConnection.Open();

                    try
                    {
                        using (SqlTransaction transaction = sqlConnection.BeginTransaction())
                        {
                            object targetRole = ExecuteScalar(sqlConnection, transaction, "SELECT role FROM users WHERE userId = @userId", targetUserId);
                            if (targetRole == null) throw new InvalidOperationException("The user no longer exists.");
                            if (targetRole.ToString() == "Admin" && Convert.ToInt32(ExecuteScalar(sqlConnection, transaction, "SELECT COUNT(*) FROM users WHERE role = 'Admin'", null)) <= 1)
                                throw new InvalidOperationException("At least one administrator account must remain.");

                            ExecuteDelete(sqlConnection, transaction, "DELETE FROM feedbacks WHERE userId = @userId OR skillId IN (SELECT skillId FROM skills WHERE userId = @userId)", targetUserId);
                            ExecuteDelete(sqlConnection, transaction, "DELETE FROM exchangeRequests WHERE senderId = @userId OR receiverId = @userId OR skillId IN (SELECT skillId FROM skills WHERE userId = @userId)", targetUserId);
                            ExecuteDelete(sqlConnection, transaction, "DELETE FROM skills WHERE userId = @userId", targetUserId);
                            ExecuteDelete(sqlConnection, transaction, "DELETE FROM users WHERE userId = @userId", targetUserId);
                            transaction.Commit();
                        }
                        lblAdminMessage.ForeColor = System.Drawing.Color.Green;
                        lblAdminMessage.Text = "User and related records deleted successfully.";
                    }
                    catch (Exception ex)
                    {
                        lblAdminMessage.ForeColor = System.Drawing.Color.Red;
                        lblAdminMessage.Text = "Could not delete user. " + ex.Message;
                    }
                }

                LoadUsers();
                LoadStats();
            }
        }

        protected void gvAllSkills_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteSkill")
            {
                int skillId = Convert.ToInt32(e.CommandArgument);

                using (SqlConnection sqlConnection = new SqlConnection(DB.connectionString))
                {
                    sqlConnection.Open();

                    try
                    {
                        using (SqlTransaction transaction = sqlConnection.BeginTransaction())
                        {
                            ExecuteDelete(sqlConnection, transaction, "DELETE FROM feedbacks WHERE skillId = @userId", skillId);
                            ExecuteDelete(sqlConnection, transaction, "DELETE FROM exchangeRequests WHERE skillId = @userId", skillId);
                            ExecuteDelete(sqlConnection, transaction, "DELETE FROM skills WHERE skillId = @userId", skillId);
                            transaction.Commit();
                        }
                        lblAdminMessage.ForeColor = System.Drawing.Color.Green;
                        lblAdminMessage.Text = "Skill and related records deleted successfully.";
                    }
                    catch (Exception ex)
                    {
                        lblAdminMessage.ForeColor = System.Drawing.Color.Red;
                        lblAdminMessage.Text = "Could not delete skill. " + ex.Message;
                    }
                }

                LoadAllSkills();
                LoadStats();
            }
        }

        private static void ExecuteDelete(SqlConnection connection, SqlTransaction transaction, string query, int userId)
        {
            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@userId", userId);
                command.ExecuteNonQuery();
            }
        }

        private static object ExecuteScalar(SqlConnection connection, SqlTransaction transaction, string query, int? userId)
        {
            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                if (userId.HasValue) command.Parameters.AddWithValue("@userId", userId.Value);
                return command.ExecuteScalar();
            }
        }

        private DataTable GetAllSkillsForExport()
        {
            using (SqlConnection con = new SqlConnection(DB.connectionString))
            {
                DataTable table = new DataTable();
                new SqlDataAdapter(@"SELECT s.skillName, u.fullName AS ownerName, u.email AS ownerEmail, c.categoryName, s.skillLevels, s.availableDays, s.isAvailable, s.createdDate FROM skills s INNER JOIN users u ON s.userId = u.userId INNER JOIN categories c ON s.categoryId = c.categoryId ORDER BY s.createdDate DESC", con).Fill(table);
                return table;
            }
        }
        protected void btnExportExcel_Click(object sender, EventArgs e) { ExportHtml("AllSkillsReport.xls", "application/vnd.ms-excel"); }
        protected void btnExportWord_Click(object sender, EventArgs e) { ExportHtml("AllSkillsReport.doc", "application/msword"); }
        private void ExportHtml(string fileName, string contentType)
        {
            DataTable data = GetAllSkillsForExport(); Response.Clear(); Response.Buffer = true; Response.ContentType = contentType; Response.AddHeader("content-disposition", "attachment;filename=" + fileName); Response.ContentEncoding = Encoding.UTF8;
            StringBuilder html = new StringBuilder("<html><head><meta charset='utf-8'></head><body><h2>Skill Exchange - All Skills Report</h2><table border='1'><tr><th>Skill</th><th>Owner</th><th>Email</th><th>Category</th><th>Level</th><th>Available Days</th><th>Available</th><th>Created</th></tr>");
            foreach (DataRow r in data.Rows) html.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7:dd/MM/yyyy}</td></tr>", HttpUtility.HtmlEncode(r["skillName"]), HttpUtility.HtmlEncode(r["ownerName"]), HttpUtility.HtmlEncode(r["ownerEmail"]), HttpUtility.HtmlEncode(r["categoryName"]), HttpUtility.HtmlEncode(r["skillLevels"]), HttpUtility.HtmlEncode(r["availableDays"]), Convert.ToBoolean(r["isAvailable"]) ? "Yes" : "No", r["createdDate"]);
            Response.Write(html.Append("</table></body></html>").ToString()); Response.End();
        }
        protected void btnExportPdf_Click(object sender, EventArgs e)
        {
            StringBuilder lines = new StringBuilder("Skill Exchange - All Skills Report\n\n"); foreach (DataRow r in GetAllSkillsForExport().Rows) lines.AppendFormat("{0} | {1} | {2} | {3}\n", r["skillName"], r["ownerName"], r["categoryName"], r["skillLevels"]);
            byte[] pdf = CreateSimplePdf(lines.ToString()); Response.Clear(); Response.ContentType = "application/pdf"; Response.AddHeader("content-disposition", "attachment;filename=AllSkillsReport.pdf"); Response.BinaryWrite(pdf); Response.End();
        }
        private static byte[] CreateSimplePdf(string text)
        {
            string safe = text.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)").Replace("\r", ""); string stream = "BT /F1 11 Tf 50 780 Td 14 TL "; foreach (string line in safe.Split('\n')) stream += "(" + line + ") Tj T* "; stream += "ET";
            StringBuilder pdf = new StringBuilder("%PDF-1.4\n"); int[] offsets = new int[5]; offsets[1] = pdf.Length; pdf.Append("1 0 obj<</Type/Catalog/Pages 2 0 R>>endobj\n"); offsets[2] = pdf.Length; pdf.Append("2 0 obj<</Type/Pages/Count 1/Kids[3 0 R]>>endobj\n"); offsets[3] = pdf.Length; pdf.Append("3 0 obj<</Type/Page/Parent 2 0 R/MediaBox[0 0 612 792]/Resources<</Font<</F1 4 0 R>>>>/Contents 5 0 R>>endobj\n"); offsets[4] = pdf.Length; pdf.Append("4 0 obj<</Type/Font/Subtype/Type1/BaseFont/Helvetica>>endobj\n"); int contentOffset = pdf.Length; pdf.Append("5 0 obj<</Length " + Encoding.ASCII.GetByteCount(stream) + ">>stream\n" + stream + "\nendstream endobj\n"); int xref = pdf.Length; pdf.Append("xref\n0 6\n0000000000 65535 f \n"); for (int i = 1; i < 5; i++) pdf.Append(offsets[i].ToString("D10") + " 00000 n \n"); pdf.Append(contentOffset.ToString("D10") + " 00000 n \ntrailer<</Size 6/Root 1 0 R>>\nstartxref\n" + xref + "\n%%EOF"); return Encoding.ASCII.GetBytes(pdf.ToString());
        }
    }
}
