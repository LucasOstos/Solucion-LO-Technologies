<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaginaBackupRestore.aspx.cs" Inherits="PaginaBackupRestore" %>

<%@ Register TagPrefix="uc" TagName="BotonVolverMenu" Src="~/Controles/BotonVolverMenu.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
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
                        <h1 runat="server" clave="principal.backup.titulo">Backup / Restore</h1>
                    </div>
                    <asp:Button ID="btnCrearBackup" runat="server" CssClass="boton-nuevo" Text="Crear Backup" Clave="backup.btnCrear" OnClick="btnCrearBackup_Click" CausesValidation="false" />
                </div>

                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" EnableViewState="false" />

                <div class="br-stats">
                    <div class="ap-stat-card">
                        <div class="texto-secundario" runat="server" clave="backup.ultimo">Último Backup</div>
                        <div class="ap-stat-valor">
                            <asp:Literal ID="litUltimoBackup" runat="server" Text="—" />
                        </div>
                    </div>
                    <div class="ap-stat-card">
                        <div class="texto-secundario" runat="server" clave="backup.frecuencia">Frecuencia</div>
                        <div class="ap-stat-valor">
                            <span runat="server" clave="backup.semanal">Semanal</span>
                        </div>
                    </div>
                    <div class="ap-stat-card">
                        <div class="texto-secundario" runat="server" clave="backup.retencion">Retención</div>
                        <div class="ap-stat-valor">
                            <span runat="server" clave="backup.treintaDias">30 días</span>
                        </div>
                    </div>
                </div>

                <div class="tarjeta">
                    <h2 runat="server" clave="backup.historial">Historial de Backups</h2>
                    <table class="tabla">
                        <thead>
                            <tr>
                                <th runat="server" clave="backup.colNombre">Nombre</th>
                                <th runat="server" clave="backup.colFecha">Fecha</th>
                                <th runat="server" clave="backup.colTamanio">Tamaño</th>
                                <th runat="server" clave="backup.colTipo">Tipo</th>
                                <th runat="server" clave="comun.estado">Estado</th>
                                <th runat="server" clave="comun.acciones">Acciones</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptBackups" runat="server" OnItemCommand="rptBackups_ItemCommand">
                                <ItemTemplate>
                                    <tr>
                                        <td class="texto-principal"><%# Eval("NombreArchivo") %></td>
                                        <td><%# Eval("Fecha", "{0:dd/MM/yyyy HH:mm}") %></td>
                                        <td><%# Eval("Tamanio") %></td>
                                        <td><span class="etiqueta etiqueta-info"><%# Eval("Tipo").ToString() == "Manual" ? Traducir("backup.tipoManual") : Eval("Tipo") %></span></td>
                                        <td>
                                            <span class='<%# (bool)Eval("Exitoso") ? "etiqueta etiqueta-activo" : "etiqueta etiqueta-inactivo" %>'>
                                                <%# Traducir((bool)Eval("Exitoso") ? "backup.ok" : "backup.error") %>
                                            </span>
                                        </td>
                                        <td>
                                            <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Descargar" ClaveTitle="backup.descargar"
                                                CommandName="Descargar" CommandArgument='<%# Eval("NombreArchivo") %>' Text='<%# "\u2193 " + Traducir("backup.descargar") %>' />
                                            <asp:LinkButton runat="server" CssClass="boton-icono" ToolTip="Restaurar" ClaveTitle="backup.restaurar" Visible='<%# (bool)Eval("Exitoso") %>'
                                                CommandName="Restaurar" CommandArgument='<%# Eval("NombreArchivo") %>' Text='<%# "\u2191 " + Traducir("backup.restaurar") %>' />
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                    <asp:Label ID="lblSinBackups" runat="server" Text="Todavía no se generó ningún backup." Clave="backup.sinBackups" Visible="false" CssClass="sin-resultados" />
                </div>

            </div>
        </div>

        <asp:Panel ID="pnlModalRestaurar" runat="server" ClientIDMode="Static" CssClass="modal-fondo" Style="display: none;">
            <div class="modal-caja">
                <div class="modal-encabezado">
                    <h3 runat="server" clave="backup.confirmarTitulo">Confirmar Restauración</h3>
                    <span class="modal-cerrar" onclick="cerrarModal('pnlModalRestaurar')">&times;</span>
                </div>

                <div class="br-alerta">
                    &#9888; <strong runat="server" clave="backup.alertaTitulo">Acción destructiva.</strong>
                    <span runat="server" clave="backup.alertaTexto">Al restaurar este backup, todos los datos actuales van a ser reemplazados por los datos del punto de restauración seleccionado. Esta acción no se puede deshacer.</span>
                </div>

                <asp:HiddenField ID="hfNombreArchivo" runat="server" />

                <p>
                    <strong runat="server" clave="backup.puntoRestauracion">Punto de restauración:</strong>
                    <asp:Literal ID="litFechaRestaurar" runat="server" />
                </p>
                <p>
                    <strong runat="server" clave="backup.tamanio">Tamaño:</strong>
                    <asp:Literal ID="litTamanioRestaurar" runat="server" />
                </p>

                <div class="grupo-campo">
                    <asp:Label ID="lblConfirmarRestauracion" runat="server" Text='Escribí "CONFIRMAR" para proceder' AssociatedControlID="tbConfirmarRestauracion" />
                    <asp:TextBox ID="tbConfirmarRestauracion" runat="server" CssClass="modal-campo" placeholder="CONFIRMAR" />
                </div>

                <asp:Label ID="lblMensajeModal" runat="server" CssClass="mensaje mensaje-error" Visible="false" EnableViewState="false" />

                <div class="modal-botones">
                    <asp:Button ID="btnCancelarRestaurar" runat="server" CssClass="modal-boton-cancelar"
                        Text="Cancelar" Clave="comun.cancelar" OnClientClick="cerrarModal('pnlModalRestaurar'); return false;" CausesValidation="false" />
                    <asp:Button ID="btnConfirmarRestaurar" runat="server" CssClass="modal-boton-guardar br-boton-restaurar"
                        Text="Restaurar Backup" Clave="backup.btnRestaurar" OnClick="btnConfirmarRestaurar_Click" CausesValidation="false" />
                </div>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
