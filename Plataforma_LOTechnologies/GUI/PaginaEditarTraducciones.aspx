<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaginaEditarTraducciones.aspx.cs" Inherits="PaginaEditarTraducciones" %>

<%@ Register TagPrefix="uc" TagName="BotonVolverMenu" Src="~/Controles/BotonVolverMenu.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Editar traducciones - LO Technologies</title>
    <link rel="stylesheet" type="text/css" href="/Estilos/BotonVolverMenu.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/Comun.css" />
    <link rel="stylesheet" type="text/css" href="/Estilos/EstilosIdiomas.css" />
    <script type="text/javascript" src="/JS/EditarTraduccionesJS.js"></script>
</head>
<body>
    <form id="formEditarTraducciones" runat="server">
        <div class="pagina">
            <div class="contenedor">

                <uc:BotonVolverMenu ID="volverIdiomas" runat="server" UrlDestino="PaginaIdiomas.aspx" ClaveTexto="traducciones.volver" />

                <div class="encabezado">
                    <div>
                        <h1>
                            <asp:Literal ID="litTitulo" runat="server" /></h1>
                        <p class="subtitulo">
                            <asp:Literal ID="litResumen" runat="server" />
                        </p>
                    </div>
                </div>

                <asp:Label ID="lblAvisoBase" runat="server" CssClass="mensaje trd-aviso" Visible="false"
                    Text="Estás editando el idioma base. Estos textos se muestran cuando falta una traducción en otro idioma." Clave="traducciones.avisoBase" />
                <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje" Visible="false" EnableViewState="false" />

                <div class="filtros">
                    <asp:TextBox ID="tbBuscar" runat="server" CssClass="campo-buscar" placeholder="Buscar por clave o texto..." ClavePlaceholder="traducciones.buscarPlaceholder"
                        aria-label="Buscar por clave o texto" ClaveAria="traducciones.buscarPlaceholder" />
                    <asp:DropDownList ID="ddlModulo" runat="server" CssClass="selector" aria-label="Módulo" ClaveAria="traducciones.modulo" />
                    <span class="trd-filtro-pendientes" id="spnSoloPendientes" runat="server">
                        <asp:CheckBox ID="chkSoloPendientes" runat="server" Text="Solo pendientes" Clave="traducciones.soloPendientes" />
                    </span>
                    <asp:Button ID="btnBuscar" runat="server" CssClass="boton-buscar" Text="Filtrar" Clave="comun.filtrar" OnClick="btnBuscar_Click" CausesValidation="false" />
                </div>

                <div class="tarjeta">
                    <asp:Repeater ID="rptTraducciones" runat="server" OnItemDataBound="rptTraducciones_ItemDataBound">
                        <HeaderTemplate>
                            <table class="tabla trd-tabla">
                                <thead>
                                    <tr>
                                        <th scope="col"><%# Traducir("traducciones.colClave") %></th>
                                        <th scope="col" runat="server" visible='<%# !EsIdiomaBase %>'><%# NombreIdiomaBase %></th>
                                        <th scope="col"><%# Traducir(EsIdiomaBase ? "traducciones.colTexto" : "traducciones.colTraduccion") %></th>
                                        <th scope="col" runat="server" visible='<%# !EsIdiomaBase %>'><%# Traducir("comun.estado") %></th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr class='<%# (bool)Eval("Pendiente") ? "trd-fila-pendiente" : "" %>'>
                                <td class="trd-col-clave">
                                    <span class="trd-clave"><%#: Eval("Clave") %></span>
                                    <span class="texto-secundario"><%#: Eval("Modulo") %></span>
                                    <asp:HiddenField ID="hfCodigo" runat="server" Value='<%# Eval("CodigoTraduccion") %>' />
                                </td>
                                <td class="trd-col-base" runat="server" visible='<%# !EsIdiomaBase %>'><%#: Eval("TextoBase") %></td>
                                <td class="trd-col-texto">
                                    <asp:TextBox ID="tbTexto" runat="server" CssClass="trd-texto" TextMode="MultiLine" Rows="2"
                                        Text='<%# Eval("TextoTraducido") %>' />
                                </td>
                                <td class="trd-col-estado" runat="server" visible='<%# !EsIdiomaBase %>'>
                                    <span class="trd-revisada">
                                        <asp:CheckBox ID="chkRevisada" runat="server" Text="Revisada" Clave="traducciones.revisada" Checked='<%# !(bool)Eval("Pendiente") %>' />
                                    </span>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <asp:Label ID="lblSinResultados" runat="server" CssClass="sin-resultados" Visible="false"
                        Text="No hay traducciones que coincidan con el filtro." />
                </div>

                <div class="trd-barra-guardar">
                    <span class="texto-secundario" runat="server" clave="traducciones.ayudaGuardar">Al escribir en una traducción se marca como revisada. Los cambios se guardan juntos.</span>
                    <asp:Button ID="btnGuardar" runat="server" CssClass="boton-nuevo" Text="Guardar cambios" Clave="comun.guardarCambios"
                        OnClick="btnGuardar_Click" OnClientClick="trdGuardando();" CausesValidation="false" />
                </div>

            </div>
        </div>
    </form>
</body>
</html>
