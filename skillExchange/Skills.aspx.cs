using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using skillExchange.App_Start;

namespace skillExchange
{
    public partial class Skills : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userId"] == null) { Response.Redirect("Login.aspx"); return; }
            if (!IsPostBack) { LoadCategories(); LoadSkills(); }
        }

        private void LoadCategories()
        {
            using (SqlConnection con = new SqlConnection(DB.connectionString))
            {
                DataTable table = new DataTable(); new SqlDataAdapter("SELECT categoryId, categoryName FROM categories ORDER BY categoryName", con).Fill(table);
                ddlCategory.DataSource = table; ddlCategory.DataTextField = "categoryName"; ddlCategory.DataValueField = "categoryId"; ddlCategory.DataBind();
                ddlCategory.Items.Insert(0, new ListItem("-- Select Category --", ""));
            }
        }

        private DataTable GetSkills()
        {
            using (SqlConnection con = new SqlConnection(DB.connectionString))
            using (SqlCommand cmd = new SqlCommand(@"SELECT s.skillId, s.skillName, c.categoryName, s.skillLevels, s.availableDays, s.isAvailable FROM skills s INNER JOIN categories c ON s.categoryId = c.categoryId WHERE s.userId = @userId ORDER BY s.createdDate DESC", con))
            {
                cmd.Parameters.AddWithValue("@userId", Session["userId"]); DataTable table = new DataTable(); new SqlDataAdapter(cmd).Fill(table); return table;
            }
        }
        private void LoadSkills() { gvSkills.DataSource = GetSkills(); gvSkills.DataBind(); }

        protected void btnAddSkill_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            using (SqlConnection con = new SqlConnection(DB.connectionString))
            using (SqlCommand cmd = new SqlCommand(@"INSERT INTO skills (userId, categoryId, skillName, description, skillLevels, availableDays, createdDate, isAvailable) VALUES (@userId, @categoryId, @skillName, @description, @skillLevels, @availableDays, GETDATE(), @isAvailable)", con))
            {
                cmd.Parameters.AddWithValue("@userId", Session["userId"]); cmd.Parameters.AddWithValue("@categoryId", ddlCategory.SelectedValue); cmd.Parameters.AddWithValue("@skillName", txtSkillName.Text.Trim()); cmd.Parameters.AddWithValue("@description", txtDescription.Text.Trim()); cmd.Parameters.AddWithValue("@skillLevels", ddlSkillLevel.SelectedValue); cmd.Parameters.AddWithValue("@availableDays", txtAvailableDays.Text.Trim()); cmd.Parameters.AddWithValue("@isAvailable", chkIsAvailable.Checked);
                con.Open(); cmd.ExecuteNonQuery();
            }
            ShowMessage("Skill added successfully.", true); ClearFields(); LoadSkills();
        }
        protected void gvSkills_RowEditing(object sender, GridViewEditEventArgs e) { gvSkills.EditIndex = e.NewEditIndex; LoadSkills(); SetEditLevel(e.NewEditIndex); }
        protected void gvSkills_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e) { gvSkills.EditIndex = -1; LoadSkills(); }
        private void SetEditLevel(int rowIndex) { DropDownList ddl = (DropDownList)gvSkills.Rows[rowIndex].FindControl("ddlEditLevel"); string level = gvSkills.DataKeys[rowIndex].Values["skillLevels"].ToString(); if (ddl.Items.FindByValue(level) != null) ddl.SelectedValue = level; }
        protected void gvSkills_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = gvSkills.Rows[e.RowIndex]; string name = ((TextBox)row.FindControl("txtEditSkillName")).Text.Trim(); string days = ((TextBox)row.FindControl("txtEditDays")).Text.Trim();
            if (String.IsNullOrWhiteSpace(name) || String.IsNullOrWhiteSpace(days)) { ShowMessage("Skill name and available days are required.", false); return; }
            using (SqlConnection con = new SqlConnection(DB.connectionString)) using (SqlCommand cmd = new SqlCommand("UPDATE skills SET skillName=@name, skillLevels=@level, availableDays=@days, isAvailable=@available WHERE skillId=@skillId AND userId=@userId", con))
            {
                cmd.Parameters.AddWithValue("@name", name); cmd.Parameters.AddWithValue("@level", ((DropDownList)row.FindControl("ddlEditLevel")).SelectedValue); cmd.Parameters.AddWithValue("@days", days); cmd.Parameters.AddWithValue("@available", ((CheckBox)row.FindControl("chkEditAvailable")).Checked); cmd.Parameters.AddWithValue("@skillId", gvSkills.DataKeys[e.RowIndex].Value); cmd.Parameters.AddWithValue("@userId", Session["userId"]); con.Open(); cmd.ExecuteNonQuery();
            }
            gvSkills.EditIndex = -1; ShowMessage("Skill updated successfully.", true); LoadSkills();
        }
        protected void gvSkills_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            using (SqlConnection con = new SqlConnection(DB.connectionString)) using (SqlCommand cmd = new SqlCommand("DELETE FROM skills WHERE skillId=@skillId AND userId=@userId", con)) { cmd.Parameters.AddWithValue("@skillId", gvSkills.DataKeys[e.RowIndex].Value); cmd.Parameters.AddWithValue("@userId", Session["userId"]); con.Open(); cmd.ExecuteNonQuery(); }
            ShowMessage("Skill deleted successfully.", true); LoadSkills();
        }
        private void ClearFields() { ddlCategory.SelectedIndex = 0; txtSkillName.Text = txtDescription.Text = txtAvailableDays.Text = ""; ddlSkillLevel.SelectedIndex = 0; chkIsAvailable.Checked = true; }
        private void ShowMessage(string text, bool success) { lblMessage.ForeColor = success ? System.Drawing.Color.Green : System.Drawing.Color.Red; lblMessage.Text = text; }

    }
}
