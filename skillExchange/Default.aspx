<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="skillExchange._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   
    <div class="jumbotron text-center">
        <h1>Skill Exchange Platform</h1>

        <p class="lead">
            Welcome,
            <asp:Label ID="lblWelcome" runat="server" Font-Bold="true"></asp:Label>
        </p>

        <p>Learn, teach, and connect with people through Skill Exchange.</p>

        <br />

        <a class="btn btn-default btn-lg" href="Skills.aspx">
            <span class="glyphicon glyphicon-search"></span> Browse Skills
        </a>

    </div>

    <div class="row">

        <div class="col-md-4">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title">Share Skills</h3>
                </div>

                <div class="panel-body">
                    Add your skills and help others learn.
                </div>
            </div>
        </div>

        <div class="col-md-4">
            <div class="panel panel-success">
                <div class="panel-heading">
                    <h3 class="panel-title">Exchange Requests</h3>
                </div>

                <div class="panel-body">
                    Send and receive learning requests.
                </div>
            </div>
        </div>

        <div class="col-md-4">
            <div class="panel panel-info">
                <div class="panel-heading">
                    <h3 class="panel-title">Community</h3>
                </div>

                <div class="panel-body">
                    Connect with learners around the world.
                </div>
            </div>
        </div>

    </div>

    <div class="skills-toolbar clearfix">
        <div class="pull-left"><h2>Latest Available Skills</h2><p class="text-muted">Discover skills shared by the community.</p></div>
        <div class="pull-right" style="margin-top:18px"><a class="btn btn-primary" href="ExchangeRequests.aspx">Request an Exchange</a></div>
    </div>
    <div class="row">
        <asp:Repeater ID="rptLatestSkills" runat="server">
            <ItemTemplate>
                <div class="col-md-4 col-sm-6"><div class="panel panel-default latest-skill-card"><div class="panel-body">
                    <span class="label label-primary"><%# Eval("categoryName") %></span>
                    <h3><%# Eval("skillName") %></h3>
                    <p class="text-muted"><span class="glyphicon glyphicon-user"></span> <%# Eval("ownerName") %> &middot; <%# Eval("skillLevels") %></p>
                    <p><%# Eval("description") %></p>
                    <p class="text-muted"><span class="glyphicon glyphicon-calendar"></span> <%# Eval("availableDays") %></p>
                </div></div></div>
            </ItemTemplate>
        </asp:Repeater>
        <asp:Panel ID="pnlNoSkills" runat="server" Visible="false" CssClass="col-md-12"><div class="alert alert-info">No available skills have been added yet.</div></asp:Panel>
    </div>

</asp:Content>

