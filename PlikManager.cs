using System.Globalization;

namespace StacjePogodowe;

// Ta klasa odpowiada wyłącznie za odczyt i zapis pomiarów do pliku.
public static class PlikManager
{
    // Separator pól w pliku to średnik.
    private const char Separator = ';';

    public static List<Pomiar> Wczytaj(string sciezka)
    {
        var pomiary = new List<Pomiar>();

        foreach (string linia in File.ReadAllLines(sciezka))
        {
            // Pomijamy puste wiersze i ewentualny wiersz nagłówka.
            if (string.IsNullOrWhiteSpace(linia) || linia.StartsWith("Data;"))
                continue;

            string[] pola = linia.Split(Separator);

            // Każdy rekord musi zawierać dokładnie 5 pól.
            if (pola.Length != 5)
                continue;

            // InvariantCulture zapewnia ten sam format liczb w każdym komputerze.
            if (DateTime.TryParseExact(pola[0], "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime data) &&
                decimal.TryParse(pola[2], NumberStyles.Number, CultureInfo.InvariantCulture, out decimal temperatura) &&
                int.TryParse(pola[3], out int wilgotnosc) &&
                decimal.TryParse(pola[4], NumberStyles.Number, CultureInfo.InvariantCulture, out decimal cisnienie))
            {
                pomiary.Add(new Pomiar
                {
                    Data = data,
                    Stacja = pola[1].Trim(),
                    Temperatura = temperatura,
                    Wilgotnosc = wilgotnosc,
                    Cisnienie = cisnienie
                });
            }
        }

        return pomiary;
    }

    public static void Zapisz(string sciezka, List<Pomiar> pomiary)
    {
        var linie = new List<string>
        {
            "Data;Stacja;Temperatura;Wilgotnosc;Cisnienie"
        };

        foreach (Pomiar p in pomiary)
        {
            // Zapisujemy liczby w formacie niezależnym od ustawień regionalnych.
            linie.Add(string.Join(Separator,
                p.Data.ToString("yyyy-MM-dd"),
                p.Stacja.Replace(";", ","),
                p.Temperatura.ToString(CultureInfo.InvariantCulture),
                p.Wilgotnosc.ToString(),
                p.Cisnienie.ToString(CultureInfo.InvariantCulture)));
        }

        File.WriteAllLines(sciezka, linie);
    }
}
