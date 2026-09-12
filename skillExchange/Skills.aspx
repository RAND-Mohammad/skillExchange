<%@ Page Title="My Skills" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Skills.aspx.cs" Inherits="skillExchange.Skills" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<div class="container">
    <h2 class="text-primary">Skills Management</h2>
    <p class="text-muted">Add, edit, delete, and export the skills in your profile.</p>
    <asp:ValidationSummary ID="vsSkills" runat="server" CssClass="alert alert-danger" ValidationGroup="SkillGroup" />

    <div class="panel panel-primary">
        <div class="panel-heading"><h3 class="panel-title">Add New Skill</h3></div>
        <div class="panel-body">
            <div class="row">
                <div class="col-md-6 form-group">
                    <label>Category</label>
                    <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control"></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvCategory" runat="server" ControlToValidate="ddlCategory" InitialValue="" ErrorMessage="Please select a category." ForeColor="Red" Display="Dynamic" ValidationGroup="SkillGroup" />
                </div>
                <div class="col-md-6 form-group">
                    <label>Skill Name</label>
                    <asp:TextBox ID="txtSkillName" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSkillName" runat="server" ControlToValidate="txtSkillName" ErrorMessage="Please enter the skill name." ForeColor="Red" Display="Dynamic" ValidationGroup="SkillGroup" />
                </div>
            </div>
            <div class="form-group">
                <label>Description</label>
                <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="4" CssClass="form-control" MaxLength="1000"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription" ErrorMessage="Please enter a description." ForeColor="Red" Display="Dynamic" ValidationGroup="SkillGroup" />
            </div>
            <div class="row">
                <div class="col-md-6 form-group">
                    <label>Skill Level</label>
                    <asp:DropDownList ID="ddlSkillLevel" runat="server" CssClass="form-control">
                        <asp:ListItem Value="Beginner">Beginner</asp:ListItem><asp:ListItem Value="Intermediate">Intermediate</asp:ListItem><asp:ListItem Value="Advanced">Advanced</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-md-6 form-group">
                    <label>Available Days</label>
                    <asp:TextBox ID="txtAvailableDays" runat="server" CssClass="form-control" placeholder="Example: Sunday, Tuesday" MaxLength="150"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvDays" runat="server" ControlToValidate="txtAvailableDays" ErrorMessage="Please enter available days." ForeColor="Red" Display="Dynamic" ValidationGroup="SkillGroup" />
                </div>
            </div>
            <div class="checkbox"><label><asp:CheckBox ID="chkIsAvailable" runat="server" Checked="true" /> Available for exchange</label></div>
            <asp:Button ID="btnAddSkill" runat="server" Text="Add Skill" CssClass="btn btn-primary" OnClick="btnAddSkill_Click" ValidationGroup="SkillGroup" />
            <br /><br /><asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label>
        </div>
    </div>

    <div class="skills-toolbar"><h3>My Skills</h3><p class="text-muted">Manage your saved skills. Reports are available in the Admin Dashboard.</p></div>
    <asp:GridView ID="gvSkills" runat="server" CssClass="table table-bordered table-hover" AutoGenerateColumns="False" DataKeyNames="skillId,skillLevels" GridLines="None" EmptyDataText="You have not added any skills yet." OnRowEditing="gvSkills_RowEditing" OnRowCancelingEdit="gvSkills_RowCancelingEdit" OnRowUpdating="gvSkills_RowUpdating" OnRowDeleting="gvSkills_RowDeleting">
        <Columns>
            <asp:TemplateField HeaderText="Skill"><ItemTemplate><%# Eval("skillName") %></ItemTemplate><EditItemTemplate><asp:TextBox ID="txtEditSkillName" runat="server" CssClass="form-control" Text='<%# Bind("skillName") %>' MaxLength="100" /></EditItemTemplate></asp:TemplateField>
            <asp:BoundField DataField="categoryName" HeaderText="Category" ReadOnly="True" />
            <asp:TemplateField HeaderText="Level"><ItemTemplate><%# Eval("skillLevels") %></ItemTemplate><EditItemTemplate><asp:DropDownList ID="ddlEditLevel" runat="server" CssClass="form-control"><asp:ListItem>Beginner</asp:ListItem><asp:ListItem>Intermediate</asp:ListItem><asp:ListItem>Advanced</asp:ListItem></asp:DropDownList></EditItemTemplate></asp:TemplateField>
            <asp:TemplateField HeaderText="Available Days"><ItemTemplate><%# Eval("availableDays") %></ItemTemplate><EditItemTemplate><asp:TextBox ID="txtEditDays" runat="server" CssClass="form-control" Text='<%# Bind("availableDays") %>' MaxLength="150" /></EditItemTemplate></asp:TemplateField>
            <asp:TemplateField HeaderText="Available"><ItemTemplate><asp:CheckBox ID="chkAvailable" runat="server" Checked='<%# Convert.ToBoolean(Eval("isAvailable")) %>' Enabled="false" /></ItemTemplate><EditItemTemplate><asp:CheckBox ID="chkEditAvailable" runat="server" Checked='<%# Bind("isAvailable") %>' /></EditItemTemplate></asp:TemplateField>
            <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>
                    <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" CssClass="btn btn-primary btn-sm"><span class="glyphicon glyphicon-pencil"></span> Edit</asp:LinkButton>
                    <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" CssClass="btn btn-danger btn-sm" OnClientClick="return confirm('Delete this skill?');"><span class="glyphicon glyphicon-trash"></span> Delete</asp:LinkButton>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:LinkButton ID="btnSave" runat="server" CommandName="Update" CssClass="btn btn-success btn-sm">Save</asp:LinkButton>
                    <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" CssClass="btn btn-default btn-sm">Cancel</asp:LinkButton>
                </EditItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</div>
</asp:Content>
