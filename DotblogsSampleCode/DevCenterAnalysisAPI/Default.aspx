<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DevCenterAnalysisAPI.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <table>
        <tr>
            <td>App Id</td>
            <td><asp:TextBox runat="server" ID="txtAppId" /></td>
        </tr>
        <tr>
            <td>app acquisitions</td>
            <td><asp:Button runat="server" Text="Get app acquisitions" OnClick="OnGetAppAcquisitions_Click" /></td>
        </tr>
    </table>
    </div>
    </form>
</body>
</html>
