using System;
using System.Web.UI;

namespace skillExchange
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userId"] != null)
            {
                phUserMenu.Visible = true;
                litWelcome.Text = " Hi, " + Session["fullName"];

                if (Session["role"] != null && Session["role"].ToString() == "Admin")
                {
                    phAdminLink.Visible = true;
                }
            }
            else
            {
                phGuestMenu.Visible = true;
            }
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}