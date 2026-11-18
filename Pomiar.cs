namespace StacjePogodowe;

// Klasa opisująca jeden pomiar ze stacji pogodowej.
public class Pomiar
{
    public DateTime Data { get; set; }
    public string Stacja { get; set; } = "";
    public decimal Temperatura { get; set; }
    public int Wilgotnosc { get; set; }
    public decimal Cisnienie { get; set; }
}
