<%@ Page Title="Feedback" Language="C#" MasterPageFile="~/Site.Master"
AutoEventWireup="true"
CodeBehind="Feedback.aspx.cs"
Inherits="skillExchange.Feedback" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<div class="container">
    <h2 class="text-primary">Feedback</h2>
    <hr />

    <div class="panel panel-primary">
        <div class="panel-heading"><h3 class="panel-title">Leave Feedback</h3></div>
        <div class="panel-body">
            <div class="form-group">
                <label>Skill</label>
                <asp:DropDownList ID="ddlSkillFeedback" runat="server" CssClass="form-control"></asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvSkillFeedback" runat="server" ControlToValidate="ddlSkillFeedback" InitialValue="0"
                    ErrorMessage="Please choose a skill." ForeColor="Red" Display="Dynamic" ValidationGroup="FeedbackGroup"></asp:RequiredFieldValidator>
            </div>

            <div class="form-group">
                <label>Rating</label><br />
                <asp:RadioButtonList ID="rblRating" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Text="1" Value="1" />
                    <asp:ListItem Text="2" Value="2" />
                    <asp:ListItem Text="3" Value="3" />
                    <asp:ListItem Text="4" Value="4" />
                    <asp:ListItem Text="5" Value="5" />
                </asp:RadioButtonList>
                <asp:RequiredFieldValidator ID="rfvRating" runat="server" ControlToValidate="rblRating"
                    ErrorMessage="Please choose a rating." ForeColor="Red" Display="Dynamic" ValidationGroup="FeedbackGroup"></asp:RequiredFieldValidator>
            </div>

            <div class="form-group">
                <label>Comment</label>
                <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvComment" runat="server" ControlToValidate="txtComment"
                    ErrorMessage="Please write a comment." ForeColor="Red" Display="Dynamic" ValidationGroup="FeedbackGroup"></asp:RequiredFieldValidator>
            </div>

            <asp:Button ID="btnSubmitFeedback" runat="server" Text="Submit Feedback" CssClass="btn btn-primary"
                OnClick="btnSubmitFeedback_Click" ValidationGroup="FeedbackGroup" />
            <br /><br />
            <asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label>
        </div>
    </div>

    <br />
    <h3>All Feedback</h3>

    <asp:GridView ID="gvFeedback" runat="server" CssClass="table table-bordered table-hover"
        AutoGenerateColumns="False" GridLines="None" EmptyDataText="No feedback yet.">
        <Columns>
            <asp:BoundField DataField="skillName" HeaderText="Skill" />
            <asp:BoundField DataField="reviewerName" HeaderText="By" />
            <asp:BoundField DataField="rating" HeaderText="Rating" />
            <asp:BoundField DataField="comment" HeaderText="Comment" />
            <asp:BoundField DataField="createdDate" HeaderText="Date" DataFormatString="{0:dd/MM/yyyy}" />
        </Columns>
    </asp:GridView>
</div>
</asp:Content>