using System.ComponentModel;
using System.Globalization;

namespace StacjePogodowe;

public class Form1 : Form
{
    private readonly BindingList<Pomiar> pomiary = new();
    private readonly DataGridView tabela = new();
    private readonly ComboBox wyborStacji = new();
    private readonly DateTimePicker poleDaty = new();
    private readonly TextBox poleStacji = new();
    private readonly NumericUpDown poleTemperatury = new();
    private readonly NumericUpDown poleWilgotnosci = new();
    private readonly NumericUpDown poleCisnienia = new();
    private readonly Label etykietaMax = new();
    private readonly Label etykietaMin = new();
    private bool aktualizowanieFiltra = false;

    public Form1()
    {
        Text = "Stacje pogodowe";
        Width = 1050;
        Height = 700;
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(850, 550);

        ZbudujInterfejs();
        OdswiezWidok();
    }

    private void ZbudujInterfejs()
    {
        var glowny = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12),
            ColumnCount = 1,
            RowCount = 5
        };
        glowny.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        glowny.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        glowny.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        glowny.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        glowny.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        Controls.Add(glowny);

        var pasekPlikow = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
        var wczytaj = new Button { Text = "Wczytaj plik", AutoSize = true };
        var zapisz = new Button { Text = "Zapisz do pliku", AutoSize = true };
        wczytaj.Click += (_, _) => WczytajPlik();
        zapisz.Click += (_, _) => ZapiszPlik();
        pasekPlikow.Controls.AddRange(new Control[] { wczytaj, zapisz });
        glowny.Controls.Add(pasekPlikow, 0, 0);

        var filtr = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, Padding = new Padding(0, 8, 0, 8) };
        filtr.Controls.Add(new Label { Text = "Filtruj stację:", AutoSize = true, Padding = new Padding(0, 7, 0, 0) });
        wyborStacji.DropDownStyle = ComboBoxStyle.DropDownList;
        wyborStacji.Width = 220;
        wyborStacji.SelectedIndexChanged += (_, _) =>
        {
            if (!aktualizowanieFiltra)
                OdswiezWidok();
        };
        filtr.Controls.Add(wyborStacji);
        glowny.Controls.Add(filtr, 0, 1);

        // Tabela wyświetla wszystkie najważniejsze dane pomiaru.
        tabela.Dock = DockStyle.Fill;
        tabela.ReadOnly = true;
        tabela.AllowUserToAddRows = false;
        tabela.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        tabela.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        tabela.MultiSelect = false;
        tabela.RowHeadersVisible = false;
        tabela.BackgroundColor = Color.White;
        tabela.DataSource = new BindingList<Pomiar>();
        glowny.Controls.Add(tabela, 0, 2);

        var statystyki = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, Padding = new Padding(0, 8, 0, 8) };
        etykietaMax.AutoSize = true;
        etykietaMax.ForeColor = Color.DarkGreen;
        etykietaMax.Font = new Font(Font, FontStyle.Bold);
        etykietaMin.AutoSize = true;
        etykietaMin.ForeColor = Color.DarkBlue;
        etykietaMin.Font = new Font(Font, FontStyle.Bold);
        statystyki.Controls.Add(etykietaMax);
        statystyki.Controls.Add(new Label { Text = "     " });
        statystyki.Controls.Add(etykietaMin);
        glowny.Controls.Add(statystyki, 0, 3);

        var dodawanie = new GroupBox { Text = "Dodaj nowy pomiar", Dock = DockStyle.Fill, AutoSize = true, Padding = new Padding(8) };
        var pola = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
        poleDaty.Format = DateTimePickerFormat.Short;
        poleStacji.Width = 110;
        poleStacji.PlaceholderText = "Nazwa stacji";
        poleTemperatury.DecimalPlaces = 1;
        poleTemperatury.Minimum = -80;
        poleTemperatury.Maximum = 60;
        poleTemperatury.Width = 75;
        poleWilgotnosci.Minimum = 0;
        poleWilgotnosci.Maximum = 100;
        poleWilgotnosci.Width = 65;
        poleCisnienia.DecimalPlaces = 1;
        poleCisnienia.Minimum = 800;
        poleCisnienia.Maximum = 1200;
        poleCisnienia.Value = 1013;
        poleCisnienia.Width = 80;

        pola.Controls.Add(new Label { Text = "Data", AutoSize = true, Padding = new Padding(0, 7, 0, 0) });
        pola.Controls.Add(poleDaty);
        pola.Controls.Add(new Label { Text = "Stacja", AutoSize = true, Padding = new Padding(0, 7, 0, 0) });
        pola.Controls.Add(poleStacji);
        pola.Controls.Add(new Label { Text = "Temp. °C", AutoSize = true, Padding = new Padding(0, 7, 0, 0) });
        pola.Controls.Add(poleTemperatury);
        pola.Controls.Add(new Label { Text = "Wilgotność %", AutoSize = true, Padding = new Padding(0, 7, 0, 0) });
        pola.Controls.Add(poleWilgotnosci);
        pola.Controls.Add(new Label { Text = "Ciśnienie hPa", AutoSize = true, Padding = new Padding(0, 7, 0, 0) });
        pola.Controls.Add(poleCisnienia);

        var dodaj = new Button { Text = "Dodaj pomiar", AutoSize = true };
        dodaj.Click += (_, _) => DodajPomiar();
        pola.Controls.Add(dodaj);
        dodawanie.Controls.Add(pola);
        glowny.Controls.Add(dodawanie, 0, 4);
    }

    private List<Pomiar> WidocznePomiary()
    {
        string stacja = wyborStacji.SelectedItem?.ToString() ?? "Wszystkie stacje";

        if (stacja == "Wszystkie stacje")
            return pomiary.ToList();

        return pomiary.Where(p => p.Stacja == stacja).ToList();
    }

    private void OdswiezWidok()
    {
        // Zapamiętujemy wybraną stację, aby nie resetować filtra przy odświeżaniu.
        string? poprzedniWybor = wyborStacji.SelectedItem?.ToString();
        aktualizowanieFiltra = true;
        var stacje = pomiary.Select(p => p.Stacja).Distinct().OrderBy(s => s).ToList();

        wyborStacji.Items.Clear();
        wyborStacji.Items.Add("Wszystkie stacje");
        foreach (string stacja in stacje)
            wyborStacji.Items.Add(stacja);

        int indeks = poprzedniWybor == null ? 0 : wyborStacji.Items.IndexOf(poprzedniWybor);
        wyborStacji.SelectedIndex = indeks >= 0 ? indeks : 0;
        aktualizowanieFiltra = false;

        // Ustawiamy dane tabeli zgodnie z wybranym filtrem.
        var widoczne = WidocznePomiary();
        tabela.DataSource = new BindingList<Pomiar>(widoczne);

        // Po każdej zmianie danych ponownie szukamy temperatury MAX i MIN.
        foreach (DataGridViewRow wiersz in tabela.Rows)
        {
            wiersz.DefaultCellStyle.BackColor = Color.White;
            if (wiersz.DataBoundItem is Pomiar p)
            {
                if (widoczne.Count > 0 && p.Temperatura == widoczne.Max(x => x.Temperatura))
                    wiersz.DefaultCellStyle.BackColor = Color.LightGreen;
                else if (widoczne.Count > 0 && p.Temperatura == widoczne.Min(x => x.Temperatura))
                    wiersz.DefaultCellStyle.BackColor = Color.LightBlue;
            }
        }

        if (widoczne.Count == 0)
        {
            etykietaMax.Text = "MAX: brak danych";
            etykietaMin.Text = "MIN: brak danych";
        }
        else
        {
            etykietaMax.Text = $"MAX: {widoczne.Max(p => p.Temperatura):0.0} °C";
            etykietaMin.Text = $"MIN: {widoczne.Min(p => p.Temperatura):0.0} °C";
        }
    }

    private void DodajPomiar()
    {
        string nazwa = poleStacji.Text.Trim();

        if (string.IsNullOrWhiteSpace(nazwa))
        {
            MessageBox.Show("Wpisz nazwę stacji.");
            return;
        }

        // Dodajemy nowy obiekt pomiaru do listy.
        pomiary.Add(new Pomiar
        {
            Data = poleDaty.Value.Date,
            Stacja = nazwa,
            Temperatura = poleTemperatury.Value,
            Wilgotnosc = (int)poleWilgotnosci.Value,
            Cisnienie = poleCisnienia.Value
        });

        poleStacji.Clear();
        OdswiezWidok();
    }

    private void WczytajPlik()
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Pliki tekstowe (*.txt)|*.txt|Wszystkie pliki (*.*)|*.*",
            InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dane")
        };

        if (dialog.ShowDialog() != DialogResult.OK)
            return;

        try
        {
            pomiary.Clear();
            foreach (Pomiar p in PlikManager.Wczytaj(dialog.FileName))
                pomiary.Add(p);

            OdswiezWidok();
            MessageBox.Show($"Wczytano pomiarów: {pomiary.Count}");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Nie udało się wczytać pliku: " + ex.Message);
        }
    }

    private void ZapiszPlik()
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "Pliki tekstowe (*.txt)|*.txt",
            FileName = "pomiary.txt",
            InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dane")
        };

        if (dialog.ShowDialog() != DialogResult.OK)
            return;

        try
        {
            PlikManager.Zapisz(dialog.FileName, pomiary.ToList());
            MessageBox.Show("Dane zostały zapisane.");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Nie udało się zapisać pliku: " + ex.Message);
        }
    }
}
