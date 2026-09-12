using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using skillExchange.App_Start;

namespace skillExchange
{
    public partial class ExchangeRequests : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userId"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                LoadSkillsDropdown();
                LoadOfferedSkillsDropdown();
                LoadSentRequests();
                LoadReceivedRequests();
            }
        }

        private void LoadSkillsDropdown()
        {
            LoadSkillsDropdown(txtSearchSkill.Text.Trim());
        }

        private void LoadSkillsDropdown(string searchTerm)
        {
            using (SqlConnection sqlConnection = new SqlConnection(DB.connectionString))
            {
                string query = @"SELECT
                            skills.skillId,
                            skills.skillName + ' - by ' + users.fullName AS skillDisplay
                         FROM skills
                         INNER JOIN users ON skills.userId = users.userId
                         INNER JOIN categories ON skills.categoryId = categories.categoryId
                         WHERE skills.userId <> @currentUserId
                         AND skills.isAvailable = 1
                         AND (@searchTerm = '' OR skills.skillName LIKE '%' + @searchTerm + '%' OR categories.categoryName LIKE '%' + @searchTerm + '%')";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@currentUserId", Session["userId"]);
                sqlCommand.Parameters.AddWithValue("@searchTerm", searchTerm);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);

                ddlSkill.DataSource = dataTable;
                ddlSkill.DataTextField = "skillDisplay";
                ddlSkill.DataValueField = "skillId";
                ddlSkill.DataBind();
                ddlSkill.Items.Insert(0, new ListItem("-- Select a Skill --", "0"));
            }
        }

        protected void btnSearchSkill_Click(object sender, EventArgs e)
        {
            LoadSkillsDropdown();
            lblSendMessage.ForeColor = System.Drawing.Color.Blue;
            lblSendMessage.Text = ddlSkill.Items.Count > 1 ? "Matching skills loaded." : "No matching skills were found.";
        }

        private void LoadOfferedSkillsDropdown()
        {
            using (SqlConnection sqlConnection = new SqlConnection(DB.connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("SELECT skillId, skillName FROM skills WHERE userId = @userId AND isAvailable = 1 ORDER BY skillName", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@userId", Session["userId"]);
                DataTable table = new DataTable();
                new SqlDataAdapter(sqlCommand).Fill(table);
                ddlOfferedSkill.DataSource = table;
                ddlOfferedSkill.DataTextField = "skillName";
                ddlOfferedSkill.DataValueField = "skillId";
                ddlOfferedSkill.DataBind();
                ddlOfferedSkill.Items.Insert(0, new ListItem("-- Select Your Skill --", "0"));
            }
        }

        private void LoadSentRequests()
        {
            using (SqlConnection sqlConnection = new SqlConnection(DB.connectionString))
            {
                string query = @"SELECT
                            er.requestId, s.skillName, u.fullName AS receiverName, u.email AS receiverEmail,
                            er.message, er.status, er.requestDate, er.responseDate
                         FROM exchangeRequests er
                         INNER JOIN skills s ON er.skillId = s.skillId
                         INNER JOIN users u ON er.receiverId = u.userId
                         WHERE er.senderId = @userId
                         ORDER BY er.requestDate DESC";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@userId", Session["userId"]);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);

                gvSentRequests.DataSource = dataTable;
                gvSentRequests.DataBind();
            }
        }

        private void LoadReceivedRequests()
        {
            using (SqlConnection sqlConnection = new SqlConnection(DB.connectionString))
            {
                string query = @"SELECT
                            er.requestId, s.skillName, u.fullName AS senderName, u.email AS senderEmail,
                            er.message, er.requestDate
                         FROM exchangeRequests er
                         INNER JOIN skills s ON er.skillId = s.skillId
                         INNER JOIN users u ON er.senderId = u.userId
                         WHERE er.receiverId = @userId
                         AND er.status = 'Pending'
                         ORDER BY er.requestDate DESC";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@userId", Session["userId"]);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);

                gvReceivedRequests.DataSource = dataTable;
                gvReceivedRequests.DataBind();
            }
        }

        protected void btnSendRequest_Click(object sender, EventArgs e)
        {
            lblSendMessage.Text = "";

            if (ddlSkill.SelectedValue == "0")
            {
                lblSendMessage.ForeColor = System.Drawing.Color.Red;
                lblSendMessage.Text = "Please choose a skill.";
                return;
            }
            if (ddlOfferedSkill.SelectedValue == "0")
            {
                lblSendMessage.ForeColor = System.Drawing.Color.Red;
                lblSendMessage.Text = "Please choose the skill you will offer.";
                return;
            }
            int skillId = Convert.ToInt32(ddlSkill.SelectedValue);
            int senderId = Convert.ToInt32(Session["userId"]);

            using (SqlConnection sqlConnection = new SqlConnection(DB.connectionString))
            {
                sqlConnection.Open();

                string ownerQuery = "SELECT userId FROM skills WHERE skillId = @skillId";
                SqlCommand ownerCmd = new SqlCommand(ownerQuery, sqlConnection);
                ownerCmd.Parameters.AddWithValue("@skillId", skillId);
                object ownerResult = ownerCmd.ExecuteScalar();

                if (ownerResult == null)
                {
                    lblSendMessage.ForeColor = System.Drawing.Color.Red;
                    lblSendMessage.Text = "This skill no longer exists.";
                    return;
                }

                int receiverId = Convert.ToInt32(ownerResult);

                if (receiverId == senderId)
                {
                    lblSendMessage.ForeColor = System.Drawing.Color.Red;
                    lblSendMessage.Text = "You cannot send a request for your own skill.";
                    return;
                }

                string offeredSkillName = ddlOfferedSkill.SelectedItem.Text;
                string finalMessage = "Offering: " + offeredSkillName;
                if (!string.IsNullOrWhiteSpace(txtMessage.Text)) finalMessage += " | " + txtMessage.Text.Trim();
                string selectedDays = "";

                foreach (ListItem item in cblPreferredDays.Items)
                {
                    if (item.Selected)
                    {
                        selectedDays += (selectedDays == "" ? "" : ", ") + item.Value;
                    }
                }

                if (selectedDays != "")
                {
                    finalMessage += " | Preferred days: " + selectedDays;
                }

                string insertQuery = @"INSERT INTO exchangeRequests
                        (senderId, receiverId, skillId, message, status, requestDate)
                        VALUES
                        (@senderId, @receiverId, @skillId, @message, 'Pending', GETDATE())";

                SqlCommand insertCmd = new SqlCommand(insertQuery, sqlConnection);
                insertCmd.Parameters.AddWithValue("@senderId", senderId);
                insertCmd.Parameters.AddWithValue("@receiverId", receiverId);
                insertCmd.Parameters.AddWithValue("@skillId", skillId);
                insertCmd.Parameters.AddWithValue("@message", finalMessage);

                insertCmd.ExecuteNonQuery();

                string notificationError;
                bool notificationSent = false;
                using (SqlCommand emailCmd = new SqlCommand("SELECT email FROM users WHERE userId = @userId", sqlConnection))
                {
                    emailCmd.Parameters.AddWithValue("@userId", receiverId);
                    object result = emailCmd.ExecuteScalar();
                    if (result != null) notificationSent = EmailService.TrySend(result.ToString(), "New Skill Exchange request", "You have received a new Skill Exchange request. Please sign in to accept or reject it.", out notificationError);
                    else notificationError = "Receiver email was not found.";
                }

                lblSendMessage.ForeColor = System.Drawing.Color.Green;
                lblSendMessage.Text = notificationSent ? "Request sent successfully. The skill owner was notified by email." : "Request saved successfully. Email notification could not be sent: " + notificationError;

                txtMessage.Text = "";
                cblPreferredDays.ClearSelection();
                ddlSkill.SelectedIndex = 0;
                ddlOfferedSkill.SelectedIndex = 0;

                LoadSentRequests();
            }
        }

        protected void gvReceivedRequests_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "UpdateStatus") return;

            int requestId = Convert.ToInt32(e.CommandArgument);

            GridViewRow row = ((Control)e.CommandSource).NamingContainer as GridViewRow;
            RadioButtonList rblDecision = (RadioButtonList)row.FindControl("rblDecision");

            if (rblDecision.SelectedItem == null)
            {
                lblReceivedMessage.ForeColor = System.Drawing.Color.Red;
                lblReceivedMessage.Text = "Please choose Accept or Reject before updating.";
                return;
            }

            string newStatus = rblDecision.SelectedValue;
            int userId = Convert.ToInt32(Session["userId"]);
            bool notificationSent = false;

            using (SqlConnection sqlConnection = new SqlConnection(DB.connectionString))
            {
                string updateQuery = @"UPDATE exchangeRequests
                        SET status = @status, responseDate = GETDATE()
                        WHERE requestId = @requestId AND receiverId = @userId";

                SqlCommand sqlCommand = new SqlCommand(updateQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@status", newStatus);
                sqlCommand.Parameters.AddWithValue("@requestId", requestId);
                sqlCommand.Parameters.AddWithValue("@userId", userId);

                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();

                string notificationError;
                if (newStatus == "Accepted")
                {
                    using (SqlCommand emailCmd = new SqlCommand(@"SELECT sender.email AS senderEmail, receiver.email AS receiverEmail FROM exchangeRequests er INNER JOIN users sender ON er.senderId = sender.userId INNER JOIN users receiver ON er.receiverId = receiver.userId WHERE er.requestId = @requestId", sqlConnection))
                    {
                        emailCmd.Parameters.AddWithValue("@requestId", requestId);
                        using (SqlDataReader reader = emailCmd.ExecuteReader())
                        {
                            if (reader.Read()) notificationSent = EmailService.TrySend(reader["senderEmail"].ToString(), "Skill Exchange request accepted", "Your request has been accepted. You can now contact the skill owner at: " + reader["receiverEmail"], out notificationError);
                            else notificationError = "Sender email was not found.";
                        }
                    }
                }

            }

            lblReceivedMessage.ForeColor = System.Drawing.Color.Green;
            lblReceivedMessage.Text = notificationSent ? "Request accepted successfully. The sender was notified by email." : "Request " + newStatus.ToLower() + " successfully. Contact details are now available after acceptance.";

            LoadReceivedRequests();
            LoadSentRequests();
        }
    }
}
