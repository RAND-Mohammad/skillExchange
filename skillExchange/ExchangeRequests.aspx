<%@ Page Title="Exchange Requests" Language="C#" MasterPageFile="~/Site.Master"
AutoEventWireup="true"
CodeBehind="ExchangeRequests.aspx.cs"
Inherits="skillExchange.ExchangeRequests" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<div class="container">
    <h2 class="text-primary">Exchange Requests</h2>
    <hr />

    <div class="panel panel-primary">
        <div class="panel-heading"><h3 class="panel-title">Send a New Request</h3></div>
        <div class="panel-body">
            <div class="form-group">
                <label>Search Skills</label>
                <div class="input-group"><asp:TextBox ID="txtSearchSkill" runat="server" CssClass="form-control" placeholder="Example: Photoshop, English, Programming"></asp:TextBox><span class="input-group-btn"><asp:Button ID="btnSearchSkill" runat="server" Text="Search" CssClass="btn btn-default" CausesValidation="false" OnClick="btnSearchSkill_Click" /></span></div>
            </div>
            <div class="form-group">
                <label>Skill You Want to Learn</label>
                <asp:DropDownList ID="ddlSkill" runat="server" CssClass="form-control"></asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvSkill" runat="server" ControlToValidate="ddlSkill" InitialValue="0"
                    ErrorMessage="Please choose a skill." ForeColor="Red" Display="Dynamic" ValidationGroup="RequestGroup"></asp:RequiredFieldValidator>
            </div>

            <div class="form-group">
                <label>Skill You Will Offer</label>
                <asp:DropDownList ID="ddlOfferedSkill" runat="server" CssClass="form-control"></asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvOfferedSkill" runat="server" ControlToValidate="ddlOfferedSkill" InitialValue="0"
                    ErrorMessage="Please choose the skill you will offer." ForeColor="Red" Display="Dynamic" ValidationGroup="RequestGroup"></asp:RequiredFieldValidator>
            </div>

            <div class="form-group">
                <label>Preferred Days (optional)</label><br />
                <asp:CheckBoxList ID="cblPreferredDays" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Text="Sunday" Value="Sunday" />
                    <asp:ListItem Text="Monday" Value="Monday" />
                    <asp:ListItem Text="Tuesday" Value="Tuesday" />
                    <asp:ListItem Text="Wednesday" Value="Wednesday" />
                    <asp:ListItem Text="Thursday" Value="Thursday" />
                    <asp:ListItem Text="Friday" Value="Friday" />
                    <asp:ListItem Text="Saturday" Value="Saturday" />
                </asp:CheckBoxList>
            </div>

            <div class="form-group">
                <label>Message (optional)</label>
                <asp:TextBox ID="txtMessage" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control"></asp:TextBox>
            </div>

            <asp:Button ID="btnSendRequest" runat="server" Text="Send Request" CssClass="btn btn-primary"
                OnClick="btnSendRequest_Click" ValidationGroup="RequestGroup" />
            <br /><br />
            <asp:Label ID="lblSendMessage" runat="server" Font-Bold="true"></asp:Label>
        </div>
    </div>

    <br />
    <h3>Requests Received (Pending)</h3>
    <asp:Label ID="lblReceivedMessage" runat="server" Font-Bold="true"></asp:Label>

    <asp:GridView ID="gvReceivedRequests" runat="server" CssClass="table table-bordered table-hover"
        AutoGenerateColumns="False" DataKeyNames="requestId" GridLines="None"
        EmptyDataText="No pending requests." OnRowCommand="gvReceivedRequests_RowCommand">
        <Columns>
            <asp:BoundField DataField="skillName" HeaderText="Skill" />
            <asp:BoundField DataField="senderName" HeaderText="From" />
            <asp:TemplateField HeaderText="Contact"><ItemTemplate><a href='mailto:<%# Eval("senderEmail") %>'><%# Eval("senderEmail") %></a></ItemTemplate></asp:TemplateField>
            <asp:BoundField DataField="message" HeaderText="Message" />
            <asp:BoundField DataField="requestDate" HeaderText="Date" DataFormatString="{0:dd/MM/yyyy}" />
            <asp:TemplateField HeaderText="Decision">
                <ItemTemplate>
                    <asp:RadioButtonList ID="rblDecision" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem Text="Accept" Value="Accepted" />
                        <asp:ListItem Text="Reject" Value="Rejected" />
                    </asp:RadioButtonList>
                    <asp:LinkButton ID="lnkUpdate" runat="server" Text="Update" CssClass="btn btn-sm btn-success"
                        CommandName="UpdateStatus" CommandArgument='<%# Eval("requestId") %>'></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <br />
    <h3>My Sent Requests</h3>

    <asp:GridView ID="gvSentRequests" runat="server" CssClass="table table-bordered table-hover"
        AutoGenerateColumns="False" GridLines="None" EmptyDataText="You have not sent any requests yet.">
        <Columns>
            <asp:BoundField DataField="skillName" HeaderText="Skill" />
            <asp:BoundField DataField="receiverName" HeaderText="Sent To" />
            <asp:TemplateField HeaderText="Contact"><ItemTemplate><asp:PlaceHolder ID="phOwnerEmail" runat="server" Visible='<%# Eval("status").ToString() == "Accepted" %>'><a href='mailto:<%# Eval("receiverEmail") %>'><%# Eval("receiverEmail") %></a></asp:PlaceHolder><asp:Label ID="lblPendingContact" runat="server" Visible='<%# Eval("status").ToString() != "Accepted" %>' Text="Available after acceptance" CssClass="text-muted" /></ItemTemplate></asp:TemplateField>
            <asp:BoundField DataField="message" HeaderText="Message" />
            <asp:BoundField DataField="status" HeaderText="Status" />
            <asp:BoundField DataField="requestDate" HeaderText="Sent On" DataFormatString="{0:dd/MM/yyyy}" />
            <asp:BoundField DataField="responseDate" HeaderText="Answered On" DataFormatString="{0:dd/MM/yyyy}" />
        </Columns>
    </asp:GridView>
</div>
</asp:Content>
