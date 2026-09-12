<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="skillExchange.Register" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title>Create Account</title>

    <!-- Bootstrap -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
    <style>
        body{min-height:100vh;background:linear-gradient(135deg,#eef4ff,#eafbf8);font-family:Inter,"Segoe UI",Arial,sans-serif}.register-card{border:0;border-radius:22px;overflow:hidden;box-shadow:0 22px 60px rgba(30,64,175,.16)}.register-card .card-header{padding:28px;background:linear-gradient(120deg,#1e40af,#0f766e);color:#fff;border:0}.register-card .card-header h3{margin:0;font-weight:700}.register-card .card-body{padding:32px}.register-card label{font-weight:600;color:#334155}.register-card .form-control{border-radius:10px;padding:11px 13px;border-color:#d7deea}.register-card .form-control:focus{border-color:#2563eb;box-shadow:0 0 0 4px rgba(37,99,235,.12)}.register-card .btn-success{border:0;border-radius:10px;padding:12px;background:linear-gradient(90deg,#2563eb,#0f766e);font-weight:700}
    </style>

</head>

<body>

    <form id="form1" runat="server">

        <div class="container mt-5">

            <div class="row justify-content-center">

                <div class="col-md-6">

                    <div class="card register-card">

                        <div class="card-header text-center">
                            <h3>Create Account</h3>
                        </div>


                        <div class="card-body">


                            <!-- Full Name -->
                            <div class="mb-3">

                                <label>Full Name</label>

                                <asp:TextBox 
                                    ID="txtFullName" 
                                    runat="server"
                                    CssClass="form-control">
                                </asp:TextBox>

                                <asp:RequiredFieldValidator
                                    ID="rfvFullName"
                                    runat="server"
                                    ControlToValidate="txtFullName"
                                    ErrorMessage="Please enter your full name"
                                    ForeColor="Red">
                                </asp:RequiredFieldValidator>

                            </div>



                            <!-- Email -->
                            <div class="mb-3">

                                <label>Email</label>

                                <asp:TextBox 
                                    ID="txtEmail" 
                                    runat="server"
                                    TextMode="Email"
                                    CssClass="form-control">
                                </asp:TextBox>

                                <asp:RequiredFieldValidator
                                    ID="rfvEmail"
                                    runat="server"
                                    ControlToValidate="txtEmail"
                                    ErrorMessage="Please enter your email"
                                    ForeColor="Red">
                                </asp:RequiredFieldValidator>

                            </div>



                            <!-- Password -->
                            <div class="mb-3">

                                <label>Password</label>

                                <asp:TextBox 
                                    ID="txtPassword" 
                                    runat="server"
                                    TextMode="Password"
                                    CssClass="form-control">
                                </asp:TextBox>

                                <asp:RequiredFieldValidator
                                    ID="rfvPassword"
                                    runat="server"
                                    ControlToValidate="txtPassword"
                                    ErrorMessage="Please enter your password"
                                    ForeColor="Red">
                                </asp:RequiredFieldValidator>

                            </div>



                            <!-- Phone Number -->
                            <div class="mb-3">

                                <label>Phone Number</label>

                                <asp:TextBox 
                                    ID="txtPhoneNumber" 
                                    runat="server"
                                    CssClass="form-control">
                                </asp:TextBox>

                            </div>



                            <!-- Country -->
                            <div class="mb-3">

                                <label>Country</label>

                                <asp:TextBox 
                                    ID="txtCountry" 
                                    runat="server"
                                    CssClass="form-control">
                                </asp:TextBox>

                            </div>



                            <!-- Bio -->
                            <div class="mb-3">

                                <label>Bio</label>

                                <asp:TextBox 
                                    ID="txtBio" 
                                    runat="server"
                                    TextMode="MultiLine"
                                    Rows="3"
                                    CssClass="form-control">
                                </asp:TextBox>

                            </div>



                            <!-- Register Button -->
                            <asp:Button
                                ID="btnRegister"
                                runat="server"
                                Text="Register"
                                CssClass="btn btn-success w-100"
                                OnClick="btnRegister_Click" />


                            <br />
                            <br />


                            <!-- Message -->
                            <asp:Label
                                ID="lblMessage"
                                runat="server"
                                ForeColor="Red">
                            </asp:Label>


                        </div>

                    </div>

                </div>

            </div>

        </div>


    </form>


</body>

</html>
