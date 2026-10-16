using System;
using System.Collections.Generic;

namespace CoffeeApp
{
    class Program
    {
        static List<Coffee> coffees = new List<Coffee>();
        static int maxCount;
        static Random rnd = new Random();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Програма обліку кавових напоїв ===");
            maxCount = ReadMaxCount();

            bool running = true;
            while (running)
            {
                PrintMenu();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": AddCoffee(); break;
                    case "2": ViewAll(); break;
                    case "3": FindCoffee(); break;
                    case "4": DemonstrateBehavior(); break;
                    case "5": DeleteCoffee(); break;
                    case "6": DemonstrateStatic(); break;
                    case "0":
                        running = false;
                        Console.WriteLine("Роботу програми завершено. До побачення!");
                        break;
                    default:
                        Console.WriteLine("Error: невірний пункт меню, спробуйте ще раз!");
                        break;
                }
                Console.WriteLine();
            }
        }

        static void PrintMenu()
        {
            Console.WriteLine("----- МЕНЮ -----");
            Console.WriteLine("1 - Додати об'єкт");
            Console.WriteLine("2 - Переглянути всі об'єкти");
            Console.WriteLine("3 - Знайти об'єкт");
            Console.WriteLine("4 - Продемонструвати поведінку");
            Console.WriteLine("5 - Видалити об'єкт");
            Console.WriteLine("6 - Продемонструвати static-методи");
            Console.WriteLine("0 - Вийти з програми");
            Console.Write("Ваш вибір: ");
        }

        static int ReadMaxCount()
        {
            while (true)
            {
                Console.Write("Введіть максимальну кількість об'єктів N (N > 0): ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out int n) && n > 0)
                    return n;
                Console.WriteLine("Error: N має бути цілим додатним числом!");
            }
        }

        static void Input(string prompt, Action<string> setter)
        {
            bool hasError;
            do
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                try
                {
                    setter(input);
                    hasError = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    hasError = true;
                }
            } while (hasError);
        }

        static bool ParseBool(string s)
        {
            string v = s?.Trim().ToLower();
            if (v == "так" || v == "yes" || v == "true") return true;
            if (v == "ні" || v == "no" || v == "false") return false;
            throw new FormatException("Error: введіть 'так' або 'ні'!");
        }

        static CoffeeType ParseType(string s)
        {
            if (!int.TryParse(s, out int idx) || !Enum.IsDefined(typeof(CoffeeType), idx))
                throw new FormatException("Error: невірний номер типу кави!");
            return (CoffeeType)idx;
        }

        static void PrintTypes()
        {
            Console.WriteLine("Оберіть тип кави:");
            foreach (CoffeeType t in Enum.GetValues(typeof(CoffeeType)))
                Console.WriteLine($"  {(int)t} - {t}");
        }

        static CoffeeType ReadCoffeeType()
        {
            PrintTypes();
            CoffeeType result = default;
            Input("Ваш вибір: ", v => result = ParseType(v));
            return result;
        }

        static bool ReadBool(string prompt)
        {
            bool result = false;
            Input(prompt, v => result = ParseBool(v));
            return result;
        }

        static void AddCoffee()
        {
            if (coffees.Count >= maxCount)
            {
                Console.WriteLine($"Error: досягнуто максимальної кількості об'єктів ({maxCount})!");
                return;
            }

            Console.Write("Режим додавання: 1 - через рядок, 2 - випадковий конструктор: ");
            string mode = Console.ReadLine();

            Coffee c = null;

            if (mode == "1")
            {
                Console.WriteLine("Формат: код,назва,тип,ціна,дата,декаф,об'єм");
                Console.WriteLine("Приклад: AB1234,Ranok Blend,Latte,85.50,15.03.2024,Ні,250");
                Console.Write("Введіть рядок: ");
                string line = Console.ReadLine();
                Coffee.TryParse(line, out c);
            }
            else if (mode == "2")
            {
                try
                {
                    c = GenerateRandomCoffee();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Error: невірний режим додавання!");
            }

            if (c != null)
            {
                coffees.Add(c);
                Console.WriteLine("Об'єкт успішно додано!");
            }
            else
            {
                Console.WriteLine("Object isn't created.");
            }
        }

        static Coffee GenerateRandomCoffee()
        {
            string code = rnd.Next(100000, 999999).ToString();
            string[] names = { "Ranok Blend", "Sunny Roast", "Dark Wave", "Milk Dream", "Velvet Cup" };
            string name = names[rnd.Next(names.Length)];
            CoffeeType type = (CoffeeType)rnd.Next(Enum.GetValues(typeof(CoffeeType)).Length);
            double price = Math.Round(rnd.NextDouble() * 480 + 10, 2);
            DateTime minDate = new DateTime(2020, 1, 1);
            DateTime roastDate = minDate.AddDays(rnd.Next((DateTime.Now - minDate).Days));
            bool isDecaf = rnd.Next(2) == 0;
            int volumeMl = rnd.Next(30, 1001);

            int variant = rnd.Next(5);
            switch (variant)
            {
                case 0:
                    Console.WriteLine(">>> Спрацював статичний метод: Coffee.Parse(string)");
                    string line = $"{code},{name},{type},{price.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}," +
                                  $"{roastDate:dd.MM.yyyy},{(isDecaf ? "Так" : "Ні")},{volumeMl}";
                    return Coffee.Parse(line);
                case 1:
                    Console.WriteLine(">>> Спрацював конструктор: Coffee(string orderCode, string name)");
                    return new Coffee(code, name);
                case 2:
                    Console.WriteLine(">>> Спрацював конструктор: Coffee(string orderCode, string name, CoffeeType type)");
                    return new Coffee(code, name, type);
                case 3:
                    Console.WriteLine(">>> Спрацював конструктор: Coffee(string, string, CoffeeType, double price)");
                    return new Coffee(code, name, type, price);
                default:
                    Console.WriteLine(">>> Спрацював конструктор: Coffee(string, string, CoffeeType, double, DateTime, bool, int) - повний");
                    return new Coffee(code, name, type, price, roastDate, isDecaf, volumeMl);
            }
        }

        static void ViewAll()
        {
            Console.WriteLine($"Кількість створених об'єктів (Coffee.Counter) = {Coffee.Counter}");
            if (coffees.Count == 0) { Console.WriteLine("Об'єктів немає."); return; }
            PrintTable(coffees);
        }

        static void PrintTable(List<Coffee> list)
        {
            Console.WriteLine($"{"№",-4}{"Код",-8}{"Назва",-20}{"Тип",-12}{"Ціна",-10}{"Дата обжарки",-14}{"Декаф",-7}{"Об'єм",-8}{"Темп."}");
            for (int i = 0; i < list.Count; i++)
                Console.WriteLine($"{i + 1,-4}{list[i].ToTableRow()}");
        }

        static void FindCoffee()
        {
            if (coffees.Count == 0) { Console.WriteLine("Об'єктів немає."); return; }

            Console.Write("Пошук за характеристикою: 1 - тип кави, 2 - без кофеїну (так/ні): ");
            string choice = Console.ReadLine();
            List<Coffee> result;

            if (choice == "1")
            {
                CoffeeType type = ReadCoffeeType();
                result = coffees.FindAll(c => c.Type == type);
            }
            else if (choice == "2")
            {
                bool isDecaf = ReadBool("Без кофеїну? (так/ні): ");
                result = coffees.FindAll(c => c.IsDecaf == isDecaf);
            }
            else
            {
                Console.WriteLine("Error: невірний вибір характеристики!");
                return;
            }

            if (result.Count == 0)
                Console.WriteLine("Об'єкт(и) із заданою характеристикою не знайдено.");
            else
                PrintTable(result);
        }

        static void DemonstrateBehavior()
        {
            if (coffees.Count == 0) { Console.WriteLine("Об'єктів немає."); return; }

            PrintTable(coffees);
            Console.Write("Введіть номер об'єкта для демонстрації поведінки: ");
            if (!int.TryParse(Console.ReadLine(), out int num) || num < 1 || num > coffees.Count)
            { Console.WriteLine("Error: невірний номер об'єкта!"); return; }

            Coffee c = coffees[num - 1];
            Console.WriteLine($"--- Поведінка об'єкта \"{c.Name}\" ---");

            Console.WriteLine("1) Перевантаження Reheat:");
            c.Reheat();
            c.Reheat(5);

            c.Cool(15);

            Console.WriteLine("2) Перевантаження ApplyDiscount:");
            c.ApplyDiscount(10);
            c.ApplyDiscount(20, 30);

            Console.WriteLine("3) Перевантаження AddVolume:");
            c.AddVolume(50);
            c.AddVolume(30, true);

            Console.WriteLine("Поточний стан об'єкта:");
            Console.WriteLine(c.ToString());
            Console.WriteLine($"Ціна за 100 мл: {c.PricePer100Ml:F2} {c.Currency}");
        }

        static void DeleteCoffee()
        {
            if (coffees.Count == 0) { Console.WriteLine("Об'єктів немає."); return; }

            Console.Write("Видалення: 1 - за порядковим номером, 2 - за характеристикою: ");
            string mode = Console.ReadLine();

            if (mode == "1")
            {
                PrintTable(coffees);
                Console.Write("Введіть номер об'єкта для видалення: ");
                if (!int.TryParse(Console.ReadLine(), out int num) || num < 1 || num > coffees.Count)
                { Console.WriteLine("Error: невірний номер об'єкта!"); return; }

                coffees.RemoveAt(num - 1);
                Coffee.DecreaseCounter(1);
                Console.WriteLine("Об'єкт успішно видалено!");
            }
            else if (mode == "2")
            {
                Console.Write("Видалення за характеристикою: 1 - тип кави, 2 - без кофеїну (так/ні): ");
                string choice = Console.ReadLine();
                int removed;

                if (choice == "1")
                {
                    CoffeeType type = ReadCoffeeType();
                    removed = coffees.RemoveAll(c => c.Type == type);
                }
                else if (choice == "2")
                {
                    bool isDecaf = ReadBool("Без кофеїну? (так/ні): ");
                    removed = coffees.RemoveAll(c => c.IsDecaf == isDecaf);
                }
                else
                {
                    Console.WriteLine("Error: невірний вибір характеристики!");
                    return;
                }

                if (removed == 0)
                    Console.WriteLine("Об'єктів для видалення із заданою характеристикою не знайдено.");
                else
                {
                    Coffee.DecreaseCounter(removed);
                    Console.WriteLine($"Видалено об'єктів: {removed}");
                }
            }
            else
            {
                Console.WriteLine("Error: невірний вибір режиму видалення!");
            }
        }

        static void DemonstrateStatic()
        {
            Console.WriteLine("Оберіть static-метод:");
            Console.WriteLine("1 - Coffee.CelsiusToFahrenheit");
            Console.WriteLine("2 - Coffee.CalculatePrice");
            Console.WriteLine("3 - Coffee.IsValidOrderCode");
            Console.WriteLine("4 - Coffee.DefaultTemperatureC і Coffee.Counter");
            Console.WriteLine("5 - Coffee.Parse / Coffee.TryParse");
            Console.Write("Ваш вибір: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    int celsius = 0;
                    Input("Введіть температуру в °C: ", v =>
                    {
                        if (!int.TryParse(v, out celsius))
                            throw new FormatException("Error: має бути цілим числом!");
                    });
                    Console.WriteLine($"{celsius}°C = {Coffee.CelsiusToFahrenheit(celsius):F1}°F");
                    break;

                case "2":
                    double per100 = 0;
                    int volume = 0;
                    Input("Введіть ціну за 100 мл: ", v =>
                    {
                        if (!double.TryParse(v.Replace(',', '.'), System.Globalization.NumberStyles.Float,
                                             System.Globalization.CultureInfo.InvariantCulture, out per100) || per100 <= 0)
                            throw new FormatException("Error: має бути додатним числом!");
                    });
                    Input("Введіть об'єм у мл: ", v =>
                    {
                        if (!int.TryParse(v, out volume) || volume <= 0)
                            throw new FormatException("Error: має бути додатним цілим числом!");
                    });
                    Console.WriteLine($"Ціна напою: {Coffee.CalculatePrice(per100, volume):F2} грн");
                    break;

                case "3":
                    Console.Write("Введіть код замовлення: ");
                    string code = Console.ReadLine();
                    Console.WriteLine(Coffee.IsValidOrderCode(code)
                        ? "Код коректний."
                        : "Код некоректний (потрібно рівно 6 літер/цифр).");
                    break;

                case "4":
                    Console.WriteLine($"Температура за замовчуванням: {Coffee.DefaultTemperatureC}°C");
                    Console.WriteLine($"Кількість створених об'єктів: {Coffee.Counter}");
                    Coffee.DefaultTemperatureC = 80;
                    Console.WriteLine($"Нова температура за замовчуванням: {Coffee.DefaultTemperatureC}°C");
                    Coffee.DefaultTemperatureC = 500;
                    Console.WriteLine($"Після некоректного значення (500): {Coffee.DefaultTemperatureC}°C");
                    Coffee.DefaultTemperatureC = 75;
                    break;

                case "5":
                    string[] samples =
                    {
                        "AB1234,Ranok Blend,Latte,85.50,15.03.2024,Ні,250",
                        "AB12,Ranok Blend,Latte,85.50,15.03.2024,Ні,250",
                        "AB1234,Ranok Blend,Latte,abc,15.03.2024,Ні,250",
                        "Espresso"
                    };
                    foreach (string sample in samples)
                    {
                        Console.WriteLine($"Рядок: {sample}");
                        if (Coffee.TryParse(sample, out Coffee parsed))
                        {
                            Console.WriteLine($"  Результат: {parsed}");
                            Coffee.DecreaseCounter(1);
                        }
                    }
                    break;

                default:
                    Console.WriteLine("Error: невірний вибір static-методу!");
                    break;
            }
        }
    }
}