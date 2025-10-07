namespace CS_Intro;

class Program
{
    static void Main(string[] args)
    {
        // Variables & Datatypes
        // string holds text
        string myText = "Hello";
        // we can auto assign variables, using the 'var' keyword
        var myVariable = "Hello there!";
        var myNumberVariable = 64;
        Console.WriteLine("My text variable currently holds the value: " + myVariable);
        Console.WriteLine("My number variable, currently holds the value: " + myNumberVariable);
        // int holds whole numbers (integers)
        int myNumber = 25;
        // double holds decimals
        double myDecimalNumber = 25.5;
        // bool, holds a boolean value (true or false)
        bool shouldBeTrue = true;
        // Char, holds a single character (ASCII/UTF)
        char a = 'a';
        char b = 'b'; // a + b prints out 195 since it is character key-code (97)(a) + key-code(98)(b)
                      // byte, holds a byte type value, usually (8 bits) 1 byte, but we can assign larger values.
        byte sizeOfByte = 0xa;
        Console.WriteLine(myText + " World");
        Console.WriteLine(myNumber * myDecimalNumber);
        Console.WriteLine(shouldBeTrue);
        Console.WriteLine(a + b);
        Console.WriteLine(sizeOfByte);


        Console.WriteLine("@@@@@@@@@@@@\nEnd of variable segment\n@@@@@@@@@@@@");

        // Array of strings
        string[] shoppingList = ["Soda", "Snus", "Chicken", "Fruits", "Olive oil", "Pasta", "Salt", "Pepper"];

        // loop through the array
        // Using a standard for-loop
        // for (int i = 0; i < shoppingList.Length; i++)
        // {
        //     Console.WriteLine("Item in shopping list: " + shoppingList[i] + "\nIndex of our iterator: " + i);
        // }

        foreach (string items in shoppingList)
        {
            Console.WriteLine("Items in shopping list: " + items);
        }

        Console.WriteLine("@@@@@@@@@@@@@@@\nEnd of datastructure example\n@@@@@@@@@@@@@@@");

        /*

        // user input, conditionals & pattern matching
        Console.WriteLine("Hello, to enter this bar, you need to over 18, how old are you?");
        // We get the user input in the command line, by using the Console class & the ReadLine() method.
        string userInput = Console.ReadLine()!;
        // We convert out string input to an integer, by using the TryParse method.
        int.TryParse(userInput, out int result);

        // Conditionals, using if/else blocks
        if (result == 0)
        {
            Console.WriteLine("Please enter a number.");
        }

        if (result >= 18)
        {
            Console.WriteLine("Welcome to the bar.");
        }
        else if (result >= 1)
        {
            Console.WriteLine("You cannot enter the bar!");
        }

        Console.WriteLine("Who made C#?");
        string userInputTwo = Console.ReadLine()!;

        switch (userInputTwo.ToLower())
        {
            case "microsoft":
                Console.WriteLine("Correct, you win!");
                break;
            case "apple":
                Console.WriteLine("Incorrect, try again!");
                break;
            case "oracle":
                Console.WriteLine("Incorrect, try again!");
                break;
            default:
                Console.WriteLine("Sorry, that was not the correct answer..");
                break;
        }
        */

        Console.WriteLine("Enter a number:");
        string numberInputOne = Console.ReadLine()!;
        Console.WriteLine("Enter an operator: +,-, *, /");
        string _operator = Console.ReadLine()!;
        Console.WriteLine("Enter a second number:");
        string numberInputTwo = Console.ReadLine()!;

        double.TryParse(numberInputOne, out double num1);
        double.TryParse(numberInputTwo, out double num2);
        switch (_operator)
        {
            case "+":
                Console.WriteLine("Sum: " + Calculator.Add(num1, num2));
                break;
            case "-":
                Console.WriteLine("Sum: " + Calculator.Subtract(num1, num2));
                break;
            case "*":
                Console.WriteLine("Sum: " + Calculator.Multiply(num1, num2));
                break;
            case "/":
                Console.WriteLine("Sum: " + Calculator.Divide(num1, num2));
                break;
            default:
                Console.WriteLine("To use the calculator, please enter a valid operator: Either + or - and valid number.");
                return;
        }
    }
}