using System;
using System.Collections.Generic;

namespace SueldoBruto;

public partial class SueldoPage : ContentPage
{
    public SueldoPage()
    {
        InitializeComponent();
        ConfigurarInicial();
    }

    private void ConfigurarInicial()
    {
        for (int i = 20000; i <= 60000; i += 500) SueldoPicker.Items.Add(i.ToString());
        SueldoPicker.SelectedIndex = 10;

        ComunidadPicker.ItemsSource = new List<string> { "Galicia", "Madrid", "Andalucía", "Cataluña", "Otras" };
        ComunidadPicker.SelectedItem = "Galicia";

        PagasPicker.SelectedIndex = 0;
        LanguagePicker.SelectedIndex = 0;
    }

    private void OnLanguageChanged(object sender, EventArgs e)
    {
        if (LanguagePicker.SelectedIndex == -1) return;

        bool isGalego = LanguagePicker.SelectedItem.ToString() == "Galego";

        LblTitle.Text = isGalego ? "Calculadora soldo neto" : "Calculadora sueldo neto";
        LblSueldoBruto.Text = isGalego ? "SOLDO BRUTO ANUAL" : "SUELDO BRUTO ANUAL";
        LblComunidad.Text = isGalego ? "Comunidade" : "Comunidad";
        LblPagas.Text = isGalego ? "Pagas" : "Pagas";
        LblGastos.Text = isGalego ? "GASTOS MENSUAIS" : "GASTOS MENSUALES";

        AlquilerEntry.Placeholder = isGalego ? "Aluguer / Hipoteca (€)" : "Alquiler / Hipoteca (€)";
        TransporteEntry.Placeholder = isGalego ? "Combustible + AP-9 (€)" : "Combustible + AP-9 (€)";

        OnParametroCambiado(null, null);
    }

    private void OnParametroCambiado(object sender, EventArgs e)
    {
        if (SueldoPicker.SelectedIndex == -1 || LanguagePicker.SelectedIndex == -1 || PagasPicker.SelectedIndex == -1) return;

        bool isGalego = LanguagePicker.SelectedItem.ToString() == "Galego";
        double bruto = double.Parse(SueldoPicker.SelectedItem.ToString());

        double ss = bruto * 0.0645;
        double minimos = 5550 + 1200 + 1500;
        double baseLiquidable = Math.Max(0, bruto - ss - 2000 - minimos);

        double cuota = CalcularIRPF(baseLiquidable);
        double retencionPorcentaje = (cuota / bruto) * 100;
        double netoAnual = bruto - ss - cuota;

        int pagas = PagasPicker.SelectedIndex == 0 ? 12 : 14;
        double netoMensual = netoAnual / pagas;

        double.TryParse(AlquilerEntry.Text, out double alquiler);
        double.TryParse(TransporteEntry.Text, out double transporte);
        double disponible = netoMensual - alquiler - transporte;

        NetoLabel.Text = $"{netoMensual:N2} € / " + (isGalego ? "mes" : "mes");
        RetencionLabel.Text = (isGalego ? "Retención IRPF: " : "Retención IRPF: ") + $"{retencionPorcentaje:N2}%";
        DisponibleLabel.Text = (isGalego ? "Dispoñible: " : "Disponible: ") + $"{disponible:N2} €";
    }

    private double CalcularIRPF(double baseLiq)
    {
        if (baseLiq <= 12450) return baseLiq * 0.19;
        if (baseLiq <= 20200) return 2365 + (baseLiq - 12450) * 0.24;
        if (baseLiq <= 35200) return 4225 + (baseLiq - 20200) * 0.30;
        return 8725 + (baseLiq - 35200) * 0.37;
    }
}