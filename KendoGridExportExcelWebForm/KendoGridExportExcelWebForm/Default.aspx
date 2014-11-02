<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="KendoGridExportExcelWebForm.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Kendo UI Web Grid Export</title>
    <link href="http://cdn.kendostatic.com/2012.3.1114/styles/kendo.common.min.css" rel="stylesheet" />
    <link href="http://cdn.kendostatic.com/2012.3.1114/styles/kendo.default.min.css" rel="stylesheet" />
    <link href="Content/Site.css" rel="stylesheet" />

    <script src="http://code.jquery.com/jquery-1.8.2.min.js"></script>
    <script src="http://cdn.kendostatic.com/2012.3.1114/js/kendo.web.min.js"></script>
    <script src="http://demos.kendoui.com/content/shared/js/people.js"></script>
    <script src="Scripts/kendoExcelGrid.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Export Grid to Excel WebForms Example</h2>

        <div id="grid" style="width:700px;"></div>

        <script type="text/javascript" src="Scripts/Index.js"></script>
    </form>
</body>
</html>
