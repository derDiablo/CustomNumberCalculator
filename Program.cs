using System;
using System.Collections.Generic;
using System.Text;

public class CustomNumber
{
    private List<int> digits;
    public int Base { get; }

    public CustomNumber(string value, int baseSystem)
    {
        if (!(baseSystem == 2 || baseSystem == 8 || baseSystem == 10 || baseSystem == 16))
            throw new ArgumentException("Основание должно быть 2, 8, 10 или 16");
        Base = baseSystem;
        digits = new List<int>();
        for (int i = value.Length - 1; i >= 0; i--)
        {
            char c = value[i];
            int digit = c >= '0' && c <= '9' ? c - '0' : (c & 0xDF) - 'A' + 10;
            if (digit >= Base) throw new ArgumentException($"Недопустимая цифра '{c}'");
            digits.Add(digit);
        }
        while (digits.Count > 1 && digits[digits.Count - 1] == 0) digits.RemoveAt(digits.Count - 1);
    }

    private int ToDecimal()
    {
        int res = 0, pow = 1;
        foreach (int d in digits) { res += d * pow; pow *= Base; }
        return res;
    }

    private static CustomNumber FromDecimal(int val, int targetBase)
    {
        if (val == 0) return new CustomNumber("0", targetBase);
        var list = new List<int>();
        while (val > 0) { list.Add(val % targetBase); val /= targetBase; }
        var num = new CustomNumber("0", targetBase);
        num.digits = list;
        return num;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int i = digits.Count - 1; i >= 0; i--)
            sb.Append(digits[i] < 10 ? (char)('0' + digits[i]) : (char)('A' + digits[i] - 10));
        return sb.ToString();
    }

    public static CustomNumber operator +(CustomNumber a, CustomNumber b) => FromDecimal(a.ToDecimal() + b.ToDecimal(), a.Base);
    public static CustomNumber operator -(CustomNumber a, CustomNumber b) => FromDecimal(a.ToDecimal() - b.ToDecimal(), a.Base);
    public static CustomNumber operator *(CustomNumber a, CustomNumber b) => FromDecimal(a.ToDecimal() * b.ToDecimal(), a.Base);
    public static CustomNumber operator /(CustomNumber a, CustomNumber b) => FromDecimal(a.ToDecimal() / b.ToDecimal(), a.Base);
    public static CustomNumber operator %(CustomNumber a, CustomNumber b) => FromDecimal(a.ToDecimal() % b.ToDecimal(), a.Base);

    public static bool operator ==(CustomNumber a, CustomNumber b) => a.ToDecimal() == b.ToDecimal();
    public static bool operator !=(CustomNumber a, CustomNumber b) => !(a == b);
    public static bool operator <(CustomNumber a, CustomNumber b) => a.ToDecimal() < b.ToDecimal();
    public static bool operator >(CustomNumber a, CustomNumber b) => a.ToDecimal() > b.ToDecimal();
    public static bool operator <=(CustomNumber a, CustomNumber b) => a.ToDecimal() <= b.ToDecimal();
    public static bool operator >=(CustomNumber a, CustomNumber b) => a.ToDecimal() >= b.ToDecimal();

    public CustomNumber ConvertTo(int newBase) => FromDecimal(ToDecimal(), newBase);

    public static void Main()
    {
        Console.WriteLine("=== Калькулятор чисел в системах счисления (2,8,10,16) ===\n");
        Console.WriteLine("Доступные системы: 2 (двоичная), 8 (восьмеричная), 10 (десятичная), 16 (шестнадцатеричная)");
        Console.Write("Выберите систему счисления для ввода: ");
        int b = int.Parse(Console.ReadLine());

        Console.Write("Введите первое число в этой системе: ");
        string s1 = Console.ReadLine();
        Console.Write("Введите второе число в этой системе: ");
        string s2 = Console.ReadLine();

        CustomNumber a = new CustomNumber(s1, b);
        CustomNumber bObj = new CustomNumber(s2, b);

        Console.WriteLine($"\nЧисло A = {a} (десятичное: {a.ToDecimal()})");
        Console.WriteLine($"Число B = {bObj} (десятичное: {bObj.ToDecimal()})");

        while (true)
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
            Console.Write("Ваш выбор: ");
            string choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1": Console.WriteLine($"Результат: {a} + {bObj} = {a + bObj}"); break;
                    case "2":
                        int diffDec = a.ToDecimal() - bObj.ToDecimal();
                        if (diffDec < 0)
                            Console.WriteLine("Ошибка: результат вычитания отрицательный (не поддерживается)");
                        else
                            Console.WriteLine($"Результат: {a} - {bObj} = {a - bObj}");
                        break;
                    case "3": Console.WriteLine($"Результат: {a} * {bObj} = {a * bObj}"); break;
                    case "4":
                        if (bObj.ToDecimal() == 0)
                            Console.WriteLine("Ошибка: деление на ноль");
                        else
                            Console.WriteLine($"Результат: {a} / {bObj} = {a / bObj}");
                        break;
                    case "5":
                        if (bObj.ToDecimal() == 0)
                            Console.WriteLine("Ошибка: остаток от деления на ноль");
                        else
                            Console.WriteLine($"Результат: {a} % {bObj} = {a % bObj}");
                        break;
                    case "6":
                        Console.WriteLine($"{a} > {bObj} : {a > bObj}");
                        Console.WriteLine($"{a} < {bObj} : {a < bObj}");
                        Console.WriteLine($"{a} == {bObj} : {a == bObj}");
                        break;
                    case "7":
                        Console.Write("Новое основание (2,8,10,16): ");
                        int nb = int.Parse(Console.ReadLine());
                        Console.WriteLine($"{a} в системе {nb} = {a.ConvertTo(nb)}");
                        break;
                    case "8":
                        Console.Write("Новое основание (2,8,10,16): ");
                        int nb2 = int.Parse(Console.ReadLine());
                        Console.WriteLine($"{bObj} в системе {nb2} = {bObj.ConvertTo(nb2)}");
                        break;
                    case "0":
                        Console.WriteLine("До свидания!");
                        return;
                    default:
                        Console.WriteLine("Неверный ввод, попробуйте снова.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}