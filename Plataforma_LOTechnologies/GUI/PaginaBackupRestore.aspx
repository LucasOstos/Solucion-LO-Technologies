<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaginaBackupRestore.aspx.cs" Inherits="PaginaBackupRestore" %>
<%@ Register TagPrefix="uc" TagName="BotonVolverMenu" Src="~/Controles/BotonVolverMenu.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Backup / Restore - LO Technologies</title>
    <link rel="stylesheet" type="text/css" href="/Estilos/BotonVolverMenu.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/Comun.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/EstilosBackupRestore.css" />
    <script type="text/javascript" src="/JS/BackupRestoreJS.js"></script>
</head>
<body>
    <form id="formBackupRestore" runat="server">
        <div class="pagina">
            <div class="contenedor">

                <uc:BotonVolverMenu ID="volverMenu" runat="server" />

                <div class="encabezado">
                    <div>
                        <h1>Backup / Restore</h1>
                        <p class="subtitulo">CUS 013 — Copias de seguridad y restauración de base de datos</p>
                    </div>
                    <asp:Button ID="btnCrearBackup" runat="server" CssClass="boton-nuevo" Text="Crear Backup" OnClick="btnCrearBackup_Click" CausesValidation="false" />
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" />

                <div class="br-stats">
                    <div class="ap-stat-card">
                        <div class="texto-secundario">Último Backup</div>
                        <div class="ap-stat-valor"><asp:Literal ID="litUltimoBackup" runat="server" Text="—" /></div>
                    </div>
                    <div class="ap-stat-card">
                        <div class="texto-secundario">Frecuencia</div>
                        <div class="ap-stat-valor"><asp:Literal ID="litFrecuencia" runat="server" Text="Diaria" /></div>
                    </div>
                    <div class="ap-stat-card">
                        <div class="texto-secundario">Retención</div>
                        <div class="ap-stat-valor"><asp:Literal ID="litRetencion" runat="server" Text="30 días" /></div>
                    </div>
                </div>

                <div class="tarjeta">
                    <h2>Historial de Backups</h2>
                    <table class="tabla">
                        <thead>
                            <tr>
                                <th>Nombre</th>
                                <th>Fecha</th>
                                <th>Tamaño</th>
                                <th>Tipo</th>
                                <th>Estado</th>
                                <th>Acciones</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptBackups" runat="server" OnItemCommand="rptBackups_ItemCommand">
                                <ItemTemplate>
                                    <tr>
                                        <td class="texto-principal"><%# Eval("NombreArchivo") %></td>
                                        <td><%# Eval("Fecha", "{0:dd/MM/yyyy HH:mm}") %></td>
                                        <td><%# Eval("Tamanio") %></td>
                                        <td><span class="etiqueta etiqueta-info"><%# Eval("Tipo") %></span></td>
                                        <td>
                                            <span class='<%# (bool)Eval("Exitoso") ? "etiqueta etiqueta-activo" : "etiqueta etiqueta-inactivo" %>'>
                                                <%# (bool)Eval("Exitoso") ? "OK" : "Error" %>
                                            </span>
                                        </td>
                                        <td>
                                            <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Descargar"
                                                CommandName="Descargar" CommandArgument='<%# Eval("CodigoBackup") %>'>&#8595; Descargar</asp:LinkButton>
                                            <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Restaurar" Visible='<%# (bool)Eval("Exitoso") %>'
                                                CommandName="Restaurar" CommandArgument='<%# Eval("CodigoBackup") %>'>&#8593; Restaurar</asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                    <asp:Label ID="lblSinBackups" runat="server" Text="Todavía no se generó ningún backup." Visible="false" CssClass="sin-resultados" />
                </div>

            </div>
        </div>

        <asp:Panel ID="pnlModalRestaurar" runat="server" ClientIDMode="Static" CssClass="modal-fondo" Style="display:none;">
            <div class="modal-caja">
                <div class="modal-encabezado">
                    <h3>Confirmar Restauración</h3>
                    <span class="modal-cerrar" onclick="cerrarModal('pnlModalRestaurar')">&times;</span>
                </div>

                <div class="br-alerta">
                    &#9888; <strong>Acción destructiva.</strong> Al restaurar este backup, todos los datos actuales van a ser reemplazados por los datos del punto de restauración seleccionado. Esta acción no se puede deshacer.
                </div>

                <asp:HiddenField ID="hfCodigoBackup" runat="server" />

                <p><strong>Punto de restauración:</strong> <asp:Literal ID="litFechaRestaurar" runat="server" /></p>
                <p><strong>Tamaño:</strong> <asp:Literal ID="litTamanioRestaurar" runat="server" /></p>

                <div class="grupo-campo">
                    <asp:Label runat="server" Text='Escribí "CONFIRMAR" para proceder' AssociatedControlID="tbConfirmarRestauracion" />
                    <asp:TextBox ID="tbConfirmarRestauracion" runat="server" CssClass="modal-campo" placeholder="CONFIRMAR" />
                </div>

                <div class="modal-botones">
                    <asp:Button ID="btnCancelarRestaurar" runat="server" CssClass="modal-boton-cancelar"
                        Text="Cancelar" OnClick="btnCancelarRestaurar_Click" CausesValidation="false" />
                    <asp:Button ID="btnConfirmarRestaurar" runat="server" CssClass="boton-peligro"
                        Text="Restaurar Backup" OnClick="btnConfirmarRestaurar_Click" CausesValidation="false" />
                </div>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
