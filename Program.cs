using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

public class Employee
{
    // Fields
    private string id;
    private string name;
    private string address;
    private string phone;
    private long sin;
    private string dob;
    private string dept;

    // Constructors
    public Employee() { }
    public Employee(string id, string name, string address, string phone, long sin, string dob, string dept)
    {
        this.id = id;
        this.name = name;
        this.address = address;
        this.phone = phone;
        this.sin = sin;
        this.dob = dob;
        this.dept = dept;
    }

    // Properties
    public string Id { get { return id; } set { id = value; } }
    public string Name { get { return name; } set { name = value; } }
    public string Address { get { return address; } set { address = value; } }
    public string Phone { get { return phone; } set { phone = value; } }
    public long SIN { get { return sin; } set { sin = value; } }
    public string DOB { get { return dob; } set { dob = value; } }
    public string Dept { get { return dept; } set { dept = value; } }

    // Method to calculate weekly pay (to be overridden)
    public virtual double GetPay() { return 0.0; }

    // ToString method
    public override string ToString()
    {
        return $"ID: {Id}\nName: {Name}\nAddress: {Address}\nPhone: {Phone}\nSIN: {SIN}\nDOB: {DOB}\nDept: {Dept}";
    }
}

public class Salaried : Employee
{
    // Fields
    private double salary;

    // Constructors
    public Salaried() { }
    public Salaried(string id, string name, string address, string phone, long sin, string dob, string dept, double salary)
        : base(id, name, address, phone, sin, dob, dept)
    {
        this.salary = salary;
    }

    // Property
    public double Salary { get { return salary; } set { salary = value; } }

    // Method to calculate weekly pay
    public override double GetPay()
    {
        return salary;
    }

    // ToString method
    public override string ToString()
    {
        return base.ToString() + $"\nSalary: {Salary:C}";
    }
}

public class PartTime : Employee
{
    // Fields
    private double rate;
    private double hours;

    // Constructors
    public PartTime() { }
    public PartTime(string id, string name, string address, string phone, long sin, string dob, string dept, double rate, double hours)
        : base(id, name, address, phone, sin, dob, dept)
    {
        this.rate = rate;
        this.hours = hours;
    }

    // Properties
    public double Rate { get { return rate; } set { rate = value; } }
    public double Hours { get { return hours; } set { hours = value; } }

    // Method to calculate weekly pay
    public override double GetPay()
    {
        return rate * hours;
    }

    // ToString method
    public override string ToString()
    {
        return base.ToString() + $"\nRate: {Rate:C}\nHours: {Hours}";
    }
}

public class Wages : Employee
{
    // Fields
    private double rate;
    private double hours;

    // Constructors
    public Wages() { }
    public Wages(string id, string name, string address, string phone, long sin, string dob, string dept, double rate, double hours)
        : base(id, name, address, phone, sin, dob, dept)
    {
        this.rate = rate;
        this.hours = hours;
    }

    // Properties
    public double Rate { get { return rate; } set { rate = value; } }
    public double Hours { get { return hours; } set { hours = value; } }

    // Method to calculate weekly pay
    public override double GetPay()
    {
        if (hours <= 40)
            return rate * hours;
        else
            return (40 * rate) + ((hours - 40) * rate * 1.5);
    }

    // ToString method
    public override string ToString()
    {
        return base.ToString() + $"\nRate: {Rate:C}\nHours: {Hours}";
    }
}

public class Program
{
    static void Main(string[] args)
    {
        // Step 2: Read data from file
        List<Employee> employees = ReadEmployeesFromFile("C:\\Users\\yukur\\OneDrive\\Desktop\\ConsoleApp3\\employees.txt");

        // Step 3: Calculate and print required statistics
        Console.WriteLine($"Average Weekly Pay: {CalculateAverageWeeklyPay(employees):C}");
        Console.WriteLine($"Highest Weekly Pay (Wages): {GetHighestWeeklyPay(employees)}");
        Console.WriteLine($"Lowest Salary (Salaried): {GetLowestSalary(employees)}");
        CalculateEmployeeCategoryPercentage(employees);
    }

    // Step 4a: Fill a list with objects based on the supplied data file
    static List<Employee> ReadEmployeesFromFile(string filePath)
    {
        List<Employee> employees = new List<Employee>();

        try
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                string id = parts[0];
                string name = parts[1];
                string address = parts[2];
                string phone = parts[3];
                long sin = long.Parse(parts[4]);
                string dob = parts[5];
                string dept = parts[6];

                if (id[0] >= '0' && id[0] <= '4')
                {
                    double salary = double.Parse(parts[7]);
                    employees.Add(new Salaried(id, name, address, phone, sin, dob, dept, salary));
                }
                else if (id[0] >= '5' && id[0] <= '7')
                {
                    double rate = double.Parse(parts[7]);
                    double hours = double.Parse(parts[8]);
                    employees.Add(new Wages(id, name, address, phone, sin, dob, dept, rate, hours));
                }
                else if (id[0] >= '8' && id[0] <= '9')
                {
                    double rate = double.Parse(parts[7]);
                    double hours = double.Parse(parts[8]);
                    employees.Add(new PartTime(id, name, address, phone, sin, dob, dept, rate, hours));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading from file");
        }

        return employees;
    }


    // Step 4b: Calculate and return the average weekly pay for all employees
    static double CalculateAverageWeeklyPay(List<Employee> employees)
    {
        double totalPay = 0.0;
        foreach (Employee emp in employees)
        {
            totalPay += emp.GetPay();
        }
        return totalPay / employees.Count;
    }

    // Step 4c: Calculate and return the highest weekly pay for the wage employees, including the name of the employee
    static string GetHighestWeeklyPay(List<Employee> employees)
    {
        var wageEmployees = employees.OfType<Wages>();
        if (wageEmployees.Any())
        {
            var highestPay = wageEmployees.Max(emp => emp.GetPay());
            var highestPaidEmployee = wageEmployees.First(emp => emp.GetPay() == highestPay);
            return $"{highestPaidEmployee.Name} - {highestPay:C}";
        }
        else
        {
            return "No wage employees found.";
        }
    }

    // Step 4d: Calculate and return the lowest salary for the salaried employees, including the name of the employee
    static string GetLowestSalary(List<Employee> employees)
    {
        var salariedEmployees = employees.OfType<Salaried>();
        if (salariedEmployees.Any())
        {
            var lowestSalary = salariedEmployees.Min(emp => emp.GetPay());
            var lowestPaidEmployee = salariedEmployees.First(emp => emp.GetPay() == lowestSalary);
            return $"{lowestPaidEmployee.Name} - {lowestSalary:C}";
        }
        else
        {
            return "No salaried employees found.";
        }
    }

    // Step 4e: What percentage of the company’s employees fall into each employee category?
    static void CalculateEmployeeCategoryPercentage(List<Employee> employees)
    {
        int totalEmployees = employees.Count;
        int salariedCount = employees.Count(emp => emp is Salaried);
        int wagesCount = employees.Count(emp => emp is Wages);
        int partTimeCount = employees.Count(emp => emp is PartTime);

        double salariedPercentage = (double)salariedCount / totalEmployees * 100;
        double wagesPercentage = (double)wagesCount / totalEmployees * 100;
        double partTimePercentage = (double)partTimeCount / totalEmployees * 100;

        Console.WriteLine($"Salaried Employees: {salariedPercentage:F2}%");
        Console.WriteLine($"Wages Employees: {wagesPercentage:F2}%");
        Console.WriteLine($"Part-Time Employees: {partTimePercentage:F2}%");
    }
}
