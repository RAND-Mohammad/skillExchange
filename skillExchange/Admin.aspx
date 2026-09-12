<%@ Page Title="Admin Dashboard" Language="C#" MasterPageFile="~/Site.Master"
AutoEventWireup="true"
CodeBehind="Admin.aspx.cs"
Inherits="skillExchange.Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<style>
    .stat-card { border-radius: 10px; color: #fff; padding: 22px; margin-bottom: 20px; box-shadow: 0 2px 8px rgba(0,0,0,.15); }
    .stat-card h2 { margin: 0; font-size: 34px; font-weight: bold; }
    .stat-card p { margin: 4px 0 0 0; opacity: .9; }
    .bg-blue   { background: linear-gradient(135deg, #4e73df, #224abe); }
    .bg-green  { background: linear-gradient(135deg, #1cc88a, #13855c); }
    .bg-orange { background: linear-gradient(135deg, #f6a935, #c47f0d); }
    .bg-purple { background: linear-gradient(135deg, #8e44ad, #5b2c6f); }
    .admin-panel { border-radius: 10px; overflow: hidden; margin-bottom: 30px; }
</style>

<div class="container">
    <h2 class="text-primary"><span class="glyphicon glyphicon-dashboard"></span> Admin Dashboard</h2>
    <hr />

    <div class="row">
        <div class="col-md-3 col-sm-6">
            <div class="stat-card bg-blue">
                <h2><asp:Literal ID="litTotalUsers" runat="server">0</asp:Literal></h2>
                <p>Total Users</p>
            </div>
        </div>
        <div class="col-md-3 col-sm-6">
            <div class="stat-card bg-green">
                <h2><asp:Literal ID="litTotalSkills" runat="server">0</asp:Literal></h2>
                <p>Total Skills</p>
            </div>
        </div>
        <div class="col-md-3 col-sm-6">
            <div class="stat-card bg-orange">
                <h2><asp:Literal ID="litTotalRequests" runat="server">0</asp:Literal></h2>
                <p>Exchange Requests</p>
            </div>
        </div>
        <div class="col-md-3 col-sm-6">
            <div class="stat-card bg-purple">
                <h2><asp:Literal ID="litTotalFeedback" runat="server">0</asp:Literal></h2>
                <p>Feedback Entries</p>
            </div>
        </div>
    </div>

    <asp:Label ID="lblAdminMessage" runat="server" Font-Bold="true"></asp:Label>

    <div class="panel panel-info admin-panel">
        <div class="panel-heading"><h3 class="panel-title">System Reports</h3></div>
        <div class="panel-body">
            <p class="text-muted">Download a complete report of all skills registered on the platform.</p>
            <asp:Button ID="btnExportExcel" runat="server" Text="Export All Skills to Excel" CssClass="btn btn-success" OnClick="btnExportExcel_Click" />
            <asp:Button ID="btnExportWord" runat="server" Text="Export All Skills to Word" CssClass="btn btn-info" OnClick="btnExportWord_Click" />
            <asp:Button ID="btnExportPdf" runat="server" Text="Export All Skills to PDF" CssClass="btn btn-danger" OnClick="btnExportPdf_Click" />
        </div>
    </div>

    <div class="panel panel-primary admin-panel">
        <div class="panel-heading"><h3 class="panel-title">Users Management</h3></div>
        <div class="panel-body">
            <asp:GridView ID="gvUsers" runat="server" CssClass="table table-striped table-hover"
                AutoGenerateColumns="False" DataKeyNames="userId" GridLines="None"
                EmptyDataText="No users found." OnRowCommand="gvUsers_RowCommand">
                <Columns>
                    <asp:BoundField DataField="fullName" HeaderText="Full Name" />
                    <asp:BoundField DataField="email" HeaderText="Email" />
                    <asp:BoundField DataField="createdDate" HeaderText="Joined" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:TemplateField HeaderText="Role">
                        <ItemTemplate>
                            <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-control">
                                <asp:ListItem Text="User" Value="User" />
                                <asp:ListItem Text="Admin" Value="Admin" />
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkUpdateRole" runat="server" Text="Save Role" CssClass="btn btn-sm btn-primary"
                                CommandName="UpdateRole" CommandArgument='<%# Eval("userId") %>'></asp:LinkButton>
                            <asp:LinkButton ID="lnkDeleteUser" runat="server" Text="Delete" CssClass="btn btn-sm btn-danger"
                                CommandName="DeleteUser" CommandArgument='<%# Eval("userId") %>'
                                OnClientClick="return confirm('Delete this user? This cannot be undone.');"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <div class="panel panel-success admin-panel">
        <div class="panel-heading"><h3 class="panel-title">Skills Management</h3></div>
        <div class="panel-body">
            <asp:GridView ID="gvAllSkills" runat="server" CssClass="table table-striped table-hover"
                AutoGenerateColumns="False" DataKeyNames="skillId" GridLines="None"
                EmptyDataText="No skills found." OnRowCommand="gvAllSkills_RowCommand">
                <Columns>
                    <asp:BoundField DataField="skillName" HeaderText="Skill" />
                    <asp:BoundField DataField="ownerName" HeaderText="Owner" />
                    <asp:BoundField DataField="categoryName" HeaderText="Category" />
                    <asp:BoundField DataField="skillLevels" HeaderText="Level" />
                    <asp:CheckBoxField DataField="isAvailable" HeaderText="Available" />
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkDeleteSkill" runat="server" Text="Delete" CssClass="btn btn-sm btn-danger"
                                CommandName="DeleteSkill" CommandArgument='<%# Eval("skillId") %>'
                                OnClientClick="return confirm('Delete this skill?');"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</div>
</asp:Content>
