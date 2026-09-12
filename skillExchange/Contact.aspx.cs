using System;
using System.Configuration;
using System.Web.UI;
using skillExchange.App_Start;

namespace skillExchange
{
    public partial class Contact : Page
    {
        protected void Page_Load(object sender, EventArgs e) { }
        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            string error;
            bool sent = EmailService.TrySend(ConfigurationManager.AppSettings["ContactRecipient"], "Skill Exchange contact message", "From: " + txtName.Text.Trim() + " <" + txtEmail.Text.Trim() + ">\n\n" + txtContactMessage.Text.Trim(), out error);
            lblContactMessage.ForeColor = sent ? System.Drawing.Color.Green : System.Drawing.Color.Red;
            lblContactMessage.Text = sent ? " Message sent successfully." : " Message was not sent. " + error;
            if (sent) txtName.Text = txtEmail.Text = txtContactMessage.Text = "";
        }
    }
}
