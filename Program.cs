using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

[DataContract]
public class Zadanie
{
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string Nazwa { get; set; }

    [DataMember]
    public string Opis { get; set; }

    [DataMember]
    public DateTime DataZakonczenia { get; set; }

    [DataMember]
    public bool CzyWykonane { get; set; }

    public Zadanie(int id, string nazwa, string opis, DateTime dataZakonczenia, bool czyWykonane)
    {
        Id = id;
        Nazwa = nazwa;
        Opis = opis;
        DataZakonczenia = dataZakonczenia;
        CzyWykonane = czyWykonane;
    }

    public override string ToString()
    {
        return $"Id: {Id}, Nazwa: {Nazwa}, Opis: {Opis}, Data zakończenia: {DataZakonczenia}, Wykonane: {(CzyWykonane ? "Tak" : "Nie")}";
    }
}

public class ManagerZadan
{
    private List<Zadanie> listaZadan = new List<Zadanie>();
    private int nextId = 1;

    public void DodajZadanie(Zadanie zadanie)
    {
        zadanie.Id = nextId++;
        listaZadan.Add(zadanie);
    }

    public void UsunZadanie(int id)
    {
        Zadanie zadanieDoUsuniecia = listaZadan.FirstOrDefault(z => z.Id == id);
        if (zadanieDoUsuniecia != null)
        {
            listaZadan.Remove(zadanieDoUsuniecia);
            Console.WriteLine("Zadanie usunięte.");
        }
        else
        {
            Console.WriteLine("Nie znaleziono zadania o podanym identyfikatorze.");
        }
    }

    public void WyswietlZadania()
    {
        if (listaZadan.Any())
        {
            foreach (var zadanie in listaZadan)
            {
                Console.WriteLine(zadanie);
            }
        }
        else
        {
            Console.WriteLine("Brak zadań do wyświetlenia.");
        }
    }

    public void ZapiszDoPliku(string nazwaPliku)
    {
        using (FileStream fs = new FileStream(nazwaPliku, FileMode.Create))
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<Zadanie>));
            ser.WriteObject(fs, listaZadan);
        }
        Console.WriteLine("Zadania zostały zapisane do pliku.");
    }

    public void WczytajZPliku(string nazwaPliku)
    {
        if (File.Exists(nazwaPliku))
        {
            using (FileStream fs = new FileStream(nazwaPliku, FileMode.Open))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<Zadanie>));
                listaZadan = (List<Zadanie>)ser.ReadObject(fs);
                nextId = listaZadan.Count + 1;
            }
            Console.WriteLine("Zadania zostały wczytane z pliku.");
        }
        else
        {
            Console.WriteLine("Plik nie istnieje.");
        }
    }

    public int GetNextId()
    {
        return nextId;
    }
}

class Program
{
    static void Main(string[] args)
    {
        ManagerZadan manager = new ManagerZadan();

        while (true)
        {
            Console.WriteLine("Wybierz opcję:");
            Console.WriteLine("1. Dodaj zadanie");
            Console.WriteLine("2. Usuń zadanie");
            Console.WriteLine("3. Wyświetl zadania");
            Console.WriteLine("4. Zapisz do pliku");
            Console.WriteLine("5. Wczytaj z pliku");
            Console.WriteLine("6. Wyjście");

            string opcja = Console.ReadLine();

            switch (opcja)
            {
                case "1":
                    DodajZadanie(manager);
                    break;
                case "2":
                    UsunZadanie(manager);
                    break;
                case "3":
                    manager.WyswietlZadania();
                    break;
                case "4":
                    manager.ZapiszDoPliku("zadania.json");
                    break;
                case "5":
                    manager.WczytajZPliku("zadania.json");
                    break;
                case "6":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Nieprawidłowa opcja. Wybierz ponownie.");
                    break;
            }
        }
    }

    static void DodajZadanie(ManagerZadan manager)
    {
        Console.WriteLine("Podaj nazwę zadania:");
        string nazwa = Console.ReadLine();

        Console.WriteLine("Podaj opis zadania:");
        string opis = Console.ReadLine();

        Console.WriteLine("Podaj datę zakończenia zadania (RRRR-MM-DD HH:MM:SS):");
        DateTime dataZakonczenia;
        while (!DateTime.TryParse(Console.ReadLine(), out dataZakonczenia))
        {
            Console.WriteLine("Nieprawidłowy format daty. Podaj ponownie.");
        }

        Zadanie noweZadanie = new Zadanie(manager.GetNextId(), nazwa, opis, dataZakonczenia, false);
        manager.DodajZadanie(noweZadanie);
        Console.WriteLine("Zadanie dodane.");
    }

    static void UsunZadanie(ManagerZadan manager)
    {
        Console.WriteLine("Podaj Id zadania do usunięcia:");
        int id;
        while (!int.TryParse(Console.ReadLine(), out id))
        {
            Console.WriteLine("Nieprawidłowy format Id. Podaj ponownie.");
        }

        manager.UsunZadanie(id);
    }
}
