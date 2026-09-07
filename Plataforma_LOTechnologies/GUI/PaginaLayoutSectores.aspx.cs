using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BE;
using BLL;
using SERVICIO.Logica;

public partial class PaginaLayoutSectores : System.Web.UI.Page
{
    private LocalBLL localBLL = new LocalBLL();
    private LayoutBLL layoutBLL = new LayoutBLL();
    private SectorBLL sectorBLL = new SectorBLL();
    private int CodigoLocalActual
    {
        get { return (int)(ViewState["CodigoLocalActual"] ?? 0); }
        set { ViewState["CodigoLocalActual"] = value; }
    }
    private static readonly string[] extensionesValidas = { ".png", ".jpg", ".jpeg", ".pdf" };
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Sesion.Instancia.IsLogueado())
        {
            Response.Redirect("PaginaLogin.aspx");
            return;
        }

        if (!IsPostBack)
        {
            CargarDropdownLocales();
            int codigoLocalQuery;
            if (int.TryParse(Request.QueryString["local"], out codigoLocalQuery))
            {
                ddlLocal.SelectedValue = codigoLocalQuery.ToString();
            }
            CodigoLocalActual = int.Parse(ddlLocal.SelectedValue);
            CargarLayouts();
            CargarSectores();
        }
    }

    #region Funciones
    private void CargarDropdownLocales()
    {
        List<LocalListado> locales = localBLL.ObtenerLocalesListado("", null, "Activo");
        ddlLocal.Items.Clear();
        foreach (LocalListado local in locales)
        {
            ddlLocal.Items.Add(new ListItem(local.Nombre, local.CodigoLocal.ToString()));
        }
    }
    private void CargarLayouts()
    {
        List<LayoutBE> layouts = layoutBLL.ObtenerLayoutsPorLocal(CodigoLocalActual);
        if (layouts.Count == 0)
        {
            rptLayouts.Visible = false;
            lblSinLayouts.Visible = true;
        }
        else
        {
            rptLayouts.Visible = true;
            lblSinLayouts.Visible = false;
            rptLayouts.DataSource = layouts;
            rptLayouts.DataBind();
        }
    }
    private string FormatearTamanio(int bytes)
    {
        double kb = bytes / 1024.0;
        if (kb < 1024) return $"{kb:0} KB";
        return $"{kb / 1024:0.0} MB";
    }
    private void CargarSectores()
    {
        List<SectorListado> sectores = sectorBLL.ObtenerSectoresPorLocal(CodigoLocalActual);
        if (sectores.Count == 0)
        {
            rptSectores.Visible = false;
            lblSinSectores.Visible = true;
        }
        else
        {
            rptSectores.Visible = true;
            lblSinSectores.Visible = false;
            rptSectores.DataSource = sectores;
            rptSectores.DataBind();
        }
    }
    public string ObtenerClaseEstadoLayout(string estado)
    {
        switch (estado)
        {
            case "Activo": return "etiqueta-activo";
            case "Inactivo": return "etiqueta-inactivo";
            case "Borrador": return "etiqueta-info";
            default: return "";
        }
    }
    private void MostrarMensaje(string texto, bool esExito)
    {
        lblMensaje.Text = texto;
        lblMensaje.CssClass = esExito ? "mensaje" : "mensaje mensaje-error";
        lblMensaje.Visible = true;
    }
    private void MostrarDetalleLayout(int codigoLayout)
    {
        LayoutBE layout = layoutBLL.ObtenerLayoutPorCodigo(codigoLayout);

        litNombreDetalle.Text = layout.Nombre;
        litFechaDetalle.Text = layout.Fecha.ToString("dd/MM/yyyy");
        litEstadoDetalle.Text = layout.Estado;
        litDescripcionDetalle.Text = layout.Descripcion;

        phPreview.Controls.Clear();
        string ruta = $"~/Subidas/Layouts/{layout.NombreArchivo}";

        if (layout.NombreArchivo.ToLower().EndsWith(".pdf"))
        {
            phPreview.Controls.Add(new LiteralControl(
                $"<a href='{ResolveUrl(ruta)}' target='_blank'>Abrir PDF en una pestaña nueva</a>"));
        }
        else
        {
            phPreview.Controls.Add(new Image
            {
                ImageUrl = ResolveUrl(ruta),
                CssClass = "et-preview-imagen"
            });
        }

        pnlDetalleLayout.Style["display"] = "flex";
    }
    private string GenerarHtmlTreemap(List<SectorListado> sectores)
    {
        decimal total = sectores.Sum(s => s.Circulacion ?? 5m);
        if (total <= 0) total = 1;
        var sb = new System.Text.StringBuilder();
        sb.Append("<div class='sec-treemap'>");
        string[] paleta = { "#2f6bff", "#1e7a37", "#b3691e", "#b3261e", "#6a4bd6", "#0d9488", "#c2185b", "#5d4037" };
        int i = 0;
        foreach (SectorListado sector in sectores)
        {
            decimal peso = sector.Circulacion ?? 5m;
            decimal porcentaje = Math.Round((peso / total) * 100, 1);
            string color = paleta[i % paleta.Length];
            sb.Append("<div class='sec-treemap-bloque' style='flex-basis:")
              .Append(porcentaje.ToString(System.Globalization.CultureInfo.InvariantCulture))
              .Append("%; background-color:").Append(color).Append(";'>")
              .Append("<span class='sec-treemap-nombre'>").Append(HttpUtility.HtmlEncode(sector.Nombre)).Append("</span>")
              .Append("<span class='sec-treemap-valor'>").Append(porcentaje.ToString(System.Globalization.CultureInfo.InvariantCulture)).Append("%</span>")
              .Append("</div>");
            i++;
        }
        sb.Append("</div>");
        return sb.ToString();
    }
    #endregion

    protected void ddlLocal_SelectedIndexChanged(object sender, EventArgs e)
    {
        CodigoLocalActual = int.Parse(ddlLocal.SelectedValue);
        CargarLayouts();
        CargarSectores();
    }
    protected void btnCargarLayout_Click(object sender, EventArgs e)
    {
        tbNombreLayout.Text = "";
        tbFechaLayout.Text = "";
        ddlEstadoLayout.SelectedIndex = 0;
        tbDescripcionLayout.Text = "";
        pnlModalLayout.Style["display"] = "flex";
    }
    protected void btnCancelarLayout_Click(object sender, EventArgs e)
    {
        pnlModalLayout.Style["display"] = "none";
    }
    protected void btnConfirmarLayout_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid)
        {
            pnlModalLayout.Style["display"] = "flex";
            return;
        }
        if (!fuArchivoLayout.HasFile)
        {
            MostrarMensaje("Adjunte un archivo de imagen o plano.", esExito: false);
            pnlModalLayout.Style["display"] = "flex";
            return;
        }

        string extension = Path.GetExtension(fuArchivoLayout.FileName).ToLower();
        if (!extensionesValidas.Contains(extension))
        {
            MostrarMensaje("El archivo adjunto no tiene formato válido. Formatos aceptados: PNG, JPG, PDF.", esExito: false);
            pnlModalLayout.Style["display"] = "flex";
            return;
        }
        int tamanioBytes = fuArchivoLayout.PostedFile.ContentLength;
        if (tamanioBytes > 5 * 1024 * 1024) //5MB MAXIMO
        {
            MostrarMensaje("Tamaño de la imagen o plano muy grande. El máximo permitido es 5 MB.", esExito: false);
            pnlModalLayout.Style["display"] = "flex";
            return;
        }
        string nombreArchivoGuardado = Guid.NewGuid().ToString() + extension;
        string carpetaDestino = Server.MapPath("~/Subidas/Layouts/");
        if (!Directory.Exists(carpetaDestino))
        {
            Directory.CreateDirectory(carpetaDestino);
        }
        fuArchivoLayout.SaveAs(Path.Combine(carpetaDestino, nombreArchivoGuardado));
        DateTime fechaLayout;
        if (!DateTime.TryParse(tbFechaLayout.Text, out fechaLayout))
        {
            MostrarMensaje("Complete los datos.", esExito: false);
            pnlModalLayout.Style["display"] = "flex";
            return;
        }
        var layout = new LayoutBE
        {
            CodigoLocal = CodigoLocalActual,
            Nombre = tbNombreLayout.Text.Trim(),
            Descripcion = tbDescripcionLayout.Text.Trim(),
            NombreArchivo = nombreArchivoGuardado,
            TamanioArchivo = FormatearTamanio(tamanioBytes),
            Fecha = fechaLayout,
            Estado = ddlEstadoLayout.SelectedValue
        };
        layoutBLL.CrearLayout(layout);
        pnlModalLayout.Style["display"] = "none";
        MostrarMensaje("Layout registrado correctamente.", esExito: true);
        CargarLayouts();
    }
    protected void btnNuevoSector_Click(object sender, EventArgs e)
    {
        hfCodigoSector.Value = "";
        hfEsEdicionSector.Value = "false";
        litTituloModalSector.Text = "Nuevo Sector";
        btnGuardarSector.Text = "Crear Sector";
        tbNombreSector.Text = "";
        tbCirculacionSector.Text = "";
        ddlUbicacionSector.SelectedIndex = 0;
        tbDescripcionSector.Text = "";
        pnlModalSector.Style["display"] = "flex";
    }
    protected void btnCancelarSector_Click(object sender, EventArgs e)
    {
        pnlModalSector.Style["display"] = "none";
    }
    protected void btnGuardarSector_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid)
        {
            pnlModalSector.Style["display"] = "flex";
            return;
        }
        decimal? circulacion = null;
        decimal circulacionParsed;
        if (decimal.TryParse(tbCirculacionSector.Text.Trim(), out circulacionParsed))
        {
            circulacion = circulacionParsed;
        }
        bool esEdicion = hfEsEdicionSector.Value == "true";
        SectorBE sector = new SectorBE(esEdicion ? int.Parse(hfCodigoSector.Value) : 0, CodigoLocalActual, tbNombreSector.Text.Trim(), ddlTipoSector.SelectedValue, ddlUbicacionSector.SelectedValue, circulacion, tbDescripcionSector.Text.Trim(), true);       
        if (esEdicion) { sectorBLL.ModificarSector(sector); }
        else { sectorBLL.CrearSector(sector); }
        pnlModalSector.Style["display"] = "none";
        MostrarMensaje(esEdicion ? "Sector modificado correctamente." : "Sector creado correctamente.", esExito: true);
        CargarSectores();
    }
    protected void rptLayouts_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int codigoLayout = int.Parse(e.CommandArgument.ToString());
        switch (e.CommandName)
        {
            case "VerDetalle":
                MostrarDetalleLayout(codigoLayout);
                break;
            case "EliminarLayout":
                layoutBLL.EliminarLayout(codigoLayout);
                MostrarMensaje("Layout eliminado.", esExito: true);
                CargarLayouts();
                break;
        }
    }
    protected void rptSectores_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "EditarSector")
        {
            int codigoSector = int.Parse(e.CommandArgument.ToString());
            SectorBE sector = sectorBLL.ObtenerSectorPorCodigo(codigoSector);
            hfEsEdicionSector.Value = "true";
            hfCodigoSector.Value = sector.CodigoSector.ToString();
            litTituloModalSector.Text = "Editar Sector";
            btnGuardarSector.Text = "Guardar Cambios";
            tbNombreSector.Text = sector.Nombre;
            ddlUbicacionSector.SelectedValue = sector.Ubicacion;
            tbCirculacionSector.Text = sector.Circulacion?.ToString() ?? "";
            tbDescripcionSector.Text = sector.Descripcion;
            pnlModalSector.Style["display"] = "flex";
        }
    }
}