using System;
using System.Globalization;

namespace CoffeeApp
{
    public class Coffee
    {
        private string orderCode;
        private string name;
        private CoffeeType type;
        private double price;
        private DateTime roastDate;
        private bool isDecaf;
        private int volumeMl;
        private int temperatureC;

        private static int counter;
        private static int defaultTemperatureC;

        private static readonly DateTime MinRoastDate = new DateTime(2020, 1, 1);

        public string Currency { get; private set; } = "грн";

        public static int Counter
        {
            get { return counter; }
        }

        public static int DefaultTemperatureC
        {
            get { return defaultTemperatureC; }
            set
            {
                if (value < 20 || value > 95)
                    defaultTemperatureC = 75;
                else
                    defaultTemperatureC = value;
            }
        }

        static Coffee()
        {
            Console.WriteLine("  [Виклик статичного конструктора Coffee]");
            counter = 0;
            DefaultTemperatureC = 75;
        }

        public Coffee()
        {
            TemperatureC = DefaultTemperatureC;
        }

        public Coffee(string orderCode, string name)
            : this(orderCode, name, CoffeeType.Espresso)
        {
        }

        public Coffee(string orderCode, string name, CoffeeType type)
            : this(orderCode, name, type, 50.0)
        {
        }

        public Coffee(string orderCode, string name, CoffeeType type, double price)
            : this(orderCode, name, type, price, DateTime.Today, false, 200)
        {
        }

        public Coffee(string orderCode, string name, CoffeeType type, double price,
                      DateTime roastDate, bool isDecaf, int volumeMl) : this()
        {
            OrderCode = orderCode;
            Name = name;
            Type = type;
            Price = price;
            RoastDate = roastDate;
            IsDecaf = isDecaf;
            VolumeMl = volumeMl;
            counter++;
        }

        public string OrderCode
        {
            get { return orderCode; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException(null, "Error: відсутнє значення коду замовлення!");
                if (value.Length != 6)
                    throw new ArgumentException("Error: код має містити рівно 6 символів!");
                if (!IsLettersOrDigits(value))
                    throw new ArgumentException("Error: дозволені тільки літери та цифри!");
                orderCode = value;
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(null, "Error: відсутнє значення назви!");
                if (value.Length < 2 || value.Length > 30)
                    throw new ArgumentOutOfRangeException(null, "Error: довжина назви має бути від 2 до 30 символів!");
                if (value.Contains(","))
                    throw new ArgumentException("Error: назва не може містити коми!");
                name = value;
            }
        }

        public CoffeeType Type
        {
            get { return type; }
            set
            {
                if (!Enum.IsDefined(typeof(CoffeeType), value))
                    throw new ArgumentException("Error: невірний тип кави!");
                type = value;
            }
        }

        public double Price
        {
            get { return price; }
            set
            {
                if (value <= 0 || value > 500)
                    throw new ArgumentOutOfRangeException(null, "Error: ціна має бути в діапазоні (0; 500]!");
                price = value;
            }
        }

        public DateTime RoastDate
        {
            get { return roastDate; }
            set
            {
                if (value < MinRoastDate || value > DateTime.Now)
                    throw new ArgumentOutOfRangeException(null,
                        $"Error: дата має бути в діапазоні від 01.01.2020 до {DateTime.Now:dd.MM.yyyy}!");
                roastDate = value;
            }
        }

        public bool IsDecaf
        {
            get { return isDecaf; }
            set { isDecaf = value; }
        }

        public int VolumeMl
        {
            get { return volumeMl; }
            set
            {
                if (value < 30 || value > 1000)
                    throw new ArgumentOutOfRangeException(null, "Error: об'єм має бути в діапазоні [30; 1000]!");
                volumeMl = value;
            }
        }

        public int TemperatureC
        {
            get { return temperatureC; }
            private set
            {
                if (value < 20 || value > 95)
                    throw new ArgumentOutOfRangeException(null, "Error: температура має бути в діапазоні [20; 95]!");
                temperatureC = value;
            }
        }

        public double PricePer100Ml
        {
            get { return Price / VolumeMl * 100; }
        }

        public static bool IsLettersOrDigits(string text)
        {
            foreach (char ch in text)
                if (!char.IsLetterOrDigit(ch)) return false;
            return true;
        }

        public static bool IsValidOrderCode(string code)
        {
            return !string.IsNullOrEmpty(code) && code.Length == 6 && IsLettersOrDigits(code);
        }

        public static double CelsiusToFahrenheit(int celsius)
        {
            return celsius * 9.0 / 5.0 + 32;
        }

        public static double CalculatePrice(double pricePer100Ml, int volumeMl)
        {
            return pricePer100Ml * volumeMl / 100;
        }

        public static void DecreaseCounter(int count)
        {
            counter -= count;
            if (counter < 0) counter = 0;
        }

        private static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private static bool ParseBool(string s)
        {
            string v = s.Trim().ToLower();
            if (v == "так" || v == "yes" || v == "true") return true;
            if (v == "ні" || v == "no" || v == "false") return false;
            throw new FormatException("Error: значення 'Без кофеїну' має бути 'Так' або 'Ні'!");
        }

        public static Coffee Parse(string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentNullException(null, "Error: рядок не може бути порожнім!");

            string[] parts = s.Split(',');

            if (parts.Length != 7)
                throw new FormatException("Error: невірний формат рядка, очікується 7 значень через кому: " +
                                          "код,назва,тип,ціна,дата,декаф,об'єм!");

            for (int i = 0; i < parts.Length; i++)
                parts[i] = parts[i].Trim();

            if (!Enum.TryParse(parts[2], true, out CoffeeType parsedType)
                || int.TryParse(parts[2], out _)
                || !Enum.IsDefined(typeof(CoffeeType), parsedType))
                throw new FormatException("Error: невірний тип кави (очікується: Espresso, Americano, " +
                                          "Cappuccino, Latte, Mocha, FlatWhite)!");

            if (!double.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out double parsedPrice))
                throw new FormatException("Error: ціна має бути числом (дробова частина через крапку)!");

            if (!DateTime.TryParseExact(parts[4], "dd.MM.yyyy", CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out DateTime parsedDate))
                throw new FormatException("Error: невірний формат дати! Використовуйте dd.MM.yyyy");

            bool parsedDecaf = ParseBool(parts[5]);

            if (!int.TryParse(parts[6], out int parsedVolume))
                throw new FormatException("Error: об'єм має бути цілим числом!");

            return new Coffee(parts[0], parts[1], parsedType, parsedPrice, parsedDate, parsedDecaf, parsedVolume);
        }

        public static bool TryParse(string s, out Coffee coffee)
        {
            coffee = null;
            bool valid = false;

            try
            {
                coffee = Parse(s);
                valid = true;
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TryParse: {ex.Message}");
            }

            return valid;
        }

        public void Reheat()
        {
            Console.WriteLine("  [Виклик Reheat()]");
            Reheat(10);
        }

        public void Reheat(int degrees)
        {
            Console.WriteLine("  [Виклик Reheat(int)]");
            TemperatureC = Clamp(TemperatureC + degrees, 20, 95);
            Console.WriteLine($"  -> {Name} підігріто на {degrees}°C. Поточна температура: {TemperatureC}°C");
        }

        public void Cool(int degrees)
        {
            TemperatureC = Clamp(TemperatureC - degrees, 20, 95);
            Console.WriteLine($"  -> {Name} охолоджено. Поточна температура: {TemperatureC}°C");
        }

        public void ApplyDiscount(double percent)
        {
            Console.WriteLine("  [Виклик ApplyDiscount(double)]");
            double oldPrice = Price;
            Price = Price - Price * percent / 100;
            Console.WriteLine($"  -> Знижка {percent}% застосована. Ціна: {oldPrice:F2} -> {Price:F2} {Currency}");
        }

        public void ApplyDiscount(double percent, double minPrice)
        {
            Console.WriteLine("  [Виклик ApplyDiscount(double, double)]");
            double oldPrice = Price;
            double newPrice = Price - Price * percent / 100;
            double floor = Math.Min(minPrice, oldPrice);
            if (newPrice < floor) newPrice = floor;
            Price = newPrice;
            Console.WriteLine($"  -> Знижка {percent}% (мін. ціна {minPrice:F2}). Ціна: {oldPrice:F2} -> {Price:F2} {Currency}");
        }

        public void AddVolume(int ml)
        {
            Console.WriteLine("  [Виклик AddVolume(int)]");
            VolumeMl = Clamp(VolumeMl + ml, 30, 1000);
            Console.WriteLine($"  -> Додано {ml} мл. Новий об'єм: {VolumeMl} мл");
        }

        public void AddVolume(int ml, bool increasePrice)
        {
            Console.WriteLine("  [Виклик AddVolume(int, bool)]");
            int oldVolume = VolumeMl;
            double oldPrice = Price;
            VolumeMl = Clamp(VolumeMl + ml, 30, 1000);
            if (increasePrice)
                Price = Math.Min(500, Price * VolumeMl / oldVolume);
            Console.WriteLine($"  -> Об'єм: {oldVolume} -> {VolumeMl} мл; ціна: {oldPrice:F2} -> {Price:F2} {Currency}");
        }

        public string ToTableRow()
        {
            return $"{OrderCode,-8}{Name,-20}{Type,-12}{Price,-10:F2}{RoastDate,-14:dd.MM.yyyy}" +
                   $"{(IsDecaf ? "Так" : "Ні"),-7}{VolumeMl,-8}{TemperatureC}";
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3:F2},{4:dd.MM.yyyy},{5},{6}",
                                 OrderCode, Name, Type, Price, RoastDate, IsDecaf ? "Так" : "Ні", VolumeMl);
        }
    }
}