# Stacje pogodowe – C# Windows Forms

## Wymagania
- Windows
- Visual Studio 2022 lub nowsze
- .NET 8 SDK

## Uruchomienie
1. Otwórz plik `StacjePogodowe.csproj` w Visual Studio.
2. Poczekaj na przywrócenie składników projektu.
3. Uruchom program klawiszem F5.
4. Kliknij **Wczytaj plik** i wybierz `Dane/pomiary.txt`.

## Format pliku
Każdy pomiar jest zapisany w osobnym wierszu. Pola są oddzielone średnikiem:
`Data;Stacja;Temperatura;Wilgotnosc;Cisnienie`

Przykład:
`2026-11-15;Sycow;8.5;72;1012.4`

## Funkcje
- odczyt pomiarów z pliku tekstowego,
- zapis pomiarów do osobnego pliku,
- dodawanie nowego pomiaru,
- filtrowanie pomiarów według nazwy stacji,
- automatyczne wyszukiwanie temperatury maksymalnej i minimalnej,
- podświetlanie rekordów MAX (zielony) i MIN (niebieski),
- komentarze w kodzie.

## Ważne
Po dodaniu lub wczytaniu danych program ponownie oblicza MAX i MIN. Wyniki są liczone dla aktualnie wybranej stacji (albo dla wszystkich stacji, jeśli wybrano „Wszystkie stacje”).
