using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Класс, представляющий целое число в системе счисления 2,8,10,16.
/// Число хранится в виде массива разрядов (младший разряд — первый элемент).
/// </summary>
public class CustomNumber
{
    private List<int> digits;   // список цифр числа (0..15)
    public int Base { get; }    // основание системы счисления

    // Конструктор из строки и основания
    public CustomNumber(string value, int baseSystem)
    {
        // Проверка допустимости основания
        if (baseSystem != 2 && baseSystem != 8 && baseSystem != 10 && baseSystem != 16)
            throw new ArgumentException("Основание должно быть 2, 8, 10 или 16");

        // Проверка на пустую строку
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Число не может быть пустым");

        Base = baseSystem;
        digits = new List<int>();
        // Парсим строку справа налево (младший разряд — первый в списке)
        for (int i = value.Length - 1; i >= 0; i--)
        {
            char c = value[i];
            int digit;
            if (c >= '0' && c <= '9')
                digit = c - '0';
            else if (c >= 'A' && c <= 'F')
                digit = 10 + (c - 'A');
            else if (c >= 'a' && c <= 'f')
                digit = 10 + (c - 'a');
            else
                throw new ArgumentException($"Недопустимый символ '{c}'");

            if (digit >= Base)
                throw new ArgumentException($"Цифра '{c}' недопустима для системы {Base}");
            digits.Add(digit);
        }
        // Удаляем ведущие нули
        while (digits.Count > 1 && digits[digits.Count - 1] == 0)
            digits.RemoveAt(digits.Count - 1);
    }

    // Преобразование в десятичное число (используем long для предотвращения переполнения)
    private long ToDecimal()
    {
        long res = 0, pow = 1;
        foreach (int d in digits)
        {
            res += d * pow;
            pow *= Base;
        }
        return res;
    }

    // Создание объекта CustomNumber из десятичного числа long
    private static CustomNumber FromDecimal(long val, int targetBase)
    {
        if (val == 0)
            return new CustomNumber("0", targetBase);
        var list = new List<int>();
        long temp = val;
        while (temp > 0)
        {
            list.Add((int)(temp % targetBase));
            temp /= targetBase;
        }
        // Создаём временный объект и подменяем его список цифр
        var num = new CustomNumber("0", targetBase);
        num.digits = list;
        return num;
    }

    // Преобразование в строку в текущей системе счисления
    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int i = digits.Count - 1; i >= 0; i--)
        {
            int d = digits[i];
            sb.Append(d < 10 ? (char)('0' + d) : (char)('A' + d - 10));
        }
        return sb.ToString();
    }

    // Арифметические операторы (используют long)
    public static CustomNumber operator +(CustomNumber a, CustomNumber b) =>
        FromDecimal(a.ToDecimal() + b.ToDecimal(), a.Base);
    public static CustomNumber operator -(CustomNumber a, CustomNumber b) =>
        FromDecimal(a.ToDecimal() - b.ToDecimal(), a.Base);
    public static CustomNumber operator *(CustomNumber a, CustomNumber b) =>
        FromDecimal(a.ToDecimal() * b.ToDecimal(), a.Base);
    public static CustomNumber operator /(CustomNumber a, CustomNumber b) =>
        FromDecimal(a.ToDecimal() / b.ToDecimal(), a.Base);
    public static CustomNumber operator %(CustomNumber a, CustomNumber b) =>
        FromDecimal(a.ToDecimal() % b.ToDecimal(), a.Base);

    // Операторы сравнения
    public static bool operator ==(CustomNumber a, CustomNumber b) => a.ToDecimal() == b.ToDecimal();
    public static bool operator !=(CustomNumber a, CustomNumber b) => !(a == b);
    public static bool operator <(CustomNumber a, CustomNumber b) => a.ToDecimal() < b.ToDecimal();
    public static bool operator >(CustomNumber a, CustomNumber b) => a.ToDecimal() > b.ToDecimal();
    public static bool operator <=(CustomNumber a, CustomNumber b) => a.ToDecimal() <= b.ToDecimal();
    public static bool operator >=(CustomNumber a, CustomNumber b) => a.ToDecimal() >= b.ToDecimal();

    // Перевод в другую систему счисления
    public CustomNumber ConvertTo(int newBase) => FromDecimal(ToDecimal(), newBase);

    // Вспомогательные методы для ввода и вывода (рефакторинг Main)
    private static int ReadSystem(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (!int.TryParse(Console.ReadLine(), out int sys))
            {
                Console.WriteLine("Ошибка: введите число.");
                continue;
            }
            if (sys == 2 || sys == 8 || sys == 10 || sys == 16)
                return sys;
            Console.WriteLine("Ошибка: основание должно быть 2, 8, 10 или 16.");
        }
    }

    private static string ReadNumber(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Ошибка: введите число.");
                continue;
            }
            return input;
        }
    }

    private static void DisplayMenu()
    {
        Console.WriteLine("\nВыберите операцию:");
        Console.WriteLine("1 - А + В");
        Console.WriteLine("2 - А - В");
        Console.WriteLine("3 - А * В");
        Console.WriteLine("4 - А / В");
        Console.WriteLine("5 - А % В (остаток)");
        Console.WriteLine("6 - Сравнить А и В");
        Console.WriteLine("7 - Перевести число А в другую систему");
        Console.WriteLine("8 - Перевести число В в другую систему");
        Console.WriteLine("0 - Выход");
    }

    private static bool ProcessChoice(string choice, CustomNumber a, CustomNumber b)
    {
        switch (choice)
        {
            case "1":
                Console.WriteLine($"Результат: {a} + {b} = {a + b}");
                break;
            case "2":
                long diffDec = a.ToDecimal() - b.ToDecimal();
                if (diffDec < 0)
                    Console.WriteLine("Ошибка: результат вычитания отрицательный (не поддерживается)");
                else
                    Console.WriteLine($"Результат: {a} - {b} = {a - b}");
                break;
            case "3":
                Console.WriteLine($"Результат: {a} * {b} = {a * b}");
                break;
            case "4":
                if (b.ToDecimal() == 0)
                    Console.WriteLine("Ошибка: деление на ноль");
                else
                    Console.WriteLine($"Результат: {a} / {b} = {a / b}");
                break;
            case "5":
                if (b.ToDecimal() == 0)
                    Console.WriteLine("Ошибка: остаток от деления на ноль");
                else
                    Console.WriteLine($"Результат: {a} % {b} = {a % b}");
                break;
            case "6":
                Console.WriteLine($"{a} > {b} : {a > b}");
                Console.WriteLine($"{a} < {b} : {a < b}");
                Console.WriteLine($"{a} == {b} : {a == b}");
                break;
            case "7":
                int nb = ReadSystem("Новое основание (2,8,10,16): ");
                Console.WriteLine($"{a} в системе {nb} = {a.ConvertTo(nb)}");
                break;
            case "8":
                int nb2 = ReadSystem("Новое основание (2,8,10,16): ");
                Console.WriteLine($"{b} в системе {nb2} = {b.ConvertTo(nb2)}");
                break;
            case "0":
                Console.WriteLine("До свидания!");
                return false;
            default:
                Console.WriteLine("Неверный ввод, попробуйте снова.");
                break;
        }
        return true;
    }

    public static void Main()
    {
        Console.WriteLine("=== Калькулятор чисел в системах счисления (2,8,10,16) ===\n");
        Console.WriteLine("Доступные системы: 2 (двоичная), 8 (восьмеричная), 10 (десятичная), 16 (шестнадцатеричная)");

        int systemBase = ReadSystem("Выберите систему счисления для ввода: ");
        string str1 = ReadNumber("Введите первое число в этой системе: ");
        string str2 = ReadNumber("Введите второе число в этой системе: ");

        CustomNumber a = new CustomNumber(str1, systemBase);
        CustomNumber b = new CustomNumber(str2, systemBase);

        Console.WriteLine($"\nЧисло A = {a} (десятичное: {a.ToDecimal()})");
        Console.WriteLine($"Число B = {b} (десятичное: {b.ToDecimal()})");

        bool running = true;
        while (running)
        {
            DisplayMenu();
            Console.Write("Ваш выбор: ");
            string choice = Console.ReadLine();
            try
            {
                running = ProcessChoice(choice, a, b);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}