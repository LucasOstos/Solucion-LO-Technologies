<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaginaPrincipal.aspx.cs" Inherits="PaginaPrincipal" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Página Principal - LO Technologies</title>
</head>
<body>
    <form id="formPaginaPrincipal" runat="server">
        <div>
            <asp:Button ID="btnLogout" runat="server" Text="Cerrar Sesión" OnClick="btnLogout_Click" />
            <asp:Button ID="btnUsuarios" runat="server" Text="Usuarios" OnClick="btnUsuarios_Click" />
            <asp:Button ID="btnDigitos" runat="server" Text="Integridad del sistema" OnClick="btnDigitos_Click" />
            <asp:Button ID="btnEmpresas" runat="server" Text="Empresas" OnClick="btnEmpresas_Click" />
            <asp:Button ID="btnBackupRestore" runat="server" Text="Backup / Restore" OnClick="btnBackupRestore_Click" />
            <asp:Button ID="btnAuditoria" runat="server" Text="Auditoría / Bitácora" OnClick="btnAuditoria_Click" />
            <asp:Button ID="btnIdiomas" runat="server" Text="Idiomas" OnClick="btnIdiomas_Click" />
            <asp:Button ID="btnLocales" runat="server" Text="Locales" OnClick="btnLocales_Click" />
            <asp:Button ID="btnLayoutSectores" runat="server" Text="Layout y Sectores" OnClick="btnLayoutSectores_Click" />
            <asp:Button ID="btnProductos" runat="server" Text="Productos y Categorías" OnClick="btnProductos_Click" />
            <asp:Button ID="btnStockVentas" runat="server" Text="Stock y Ventas" OnClick="btnStockVentas_Click" />
            <asp:Button ID="btnEscenarios" runat="server" Text="Escenarios de Simulación" OnClick="btnEscenarios_Click" />
            <asp:Button ID="btnComparacionEscenarios" runat="server" Text="Comparación de Escenarios" OnClick="btnComparacionEscenarios_Click" />
            <asp:Button ID="btnIndicadores" runat="server" Text="Indicadores y Recomendaciones" OnClick="btnIndicadores_Click" />
            <asp:Button ID="btnAnalisisProductos" runat="server" Text="Análisis de Productos" OnClick="btnAnalisisProductos_Click" />
            <asp:Button ID="btnReportes" runat="server" Text="Reportes" OnClick="btnReportes_Click" />
        </div>
    </form>
</body>
</html>
