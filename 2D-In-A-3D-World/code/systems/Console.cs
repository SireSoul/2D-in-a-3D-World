using Raylib_cs;
using System.Numerics;
using System;

namespace MyGame;

public static class GameConsole
{
    private static bool open = false;
    private static string input = "";

    public static bool IsOpen => open;
    public static List<string> Inputs = [];
    private static int inputIndex = 0;

    public static void Toggle()
    {
        open = !open;
        input = "";
        inputIndex = -1;
    }

    public static void Update(Player player)
    {
        if (!open) return;

        // --- CAPTURE TEXT INPUT ---
        int key = Raylib.GetCharPressed();

        while (key > 0)
        {
            // Accept visible ASCII characters
            if (key >= 32 && key <= 126)
            {
                input += (char)key;
            }

            key = Raylib.GetCharPressed();
        }

        // --- Handle Backspace ---
        if (Raylib.IsKeyPressed(KeyboardKey.Backspace) && input.Length > 0)
            input = input[..^1];

        // --- UP: go backward in history ---
        if (Raylib.IsKeyPressed(KeyboardKey.Up))
        {
            if (Inputs.Count > 0)
            {
                if (inputIndex == -1) {inputIndex = Inputs.Count - 1;}
                else if (inputIndex > 0) {inputIndex--;}
                input = Inputs[inputIndex];
            }
        }

        // --- DOWN: go forward in history ---
        if (Raylib.IsKeyPressed(KeyboardKey.Down))
        {
            if (Inputs.Count > 0 && inputIndex != -1)
            {
                if (inputIndex < Inputs.Count - 1)
                {
                    inputIndex++;
                    input = Inputs[inputIndex];
                }
                else
                {
                    inputIndex = -1;
                    input = "/";
                }
            }
        }

        // --- Execute Command ---
        if (Raylib.IsKeyPressed(KeyboardKey.Enter))
        {
            Execute(input, player);
            Inputs.Add(input);
            inputIndex = -1;
            input = "";
            open = false;
            return;
        }

        // --- Close console ---
        if (Raylib.IsKeyPressed(KeyboardKey.Escape))
        {
            input = "";
            open = false;
            inputIndex = 0;
            return;
        }
    }

    public static void Draw()
    {
        if (!open) return;

        Raylib.DrawRectangle(0, Raylib.GetScreenHeight() - 40, Raylib.GetScreenWidth(), 40, new Color(0, 0, 0, 180));
        Raylib.DrawText($"> {input}", 10, Raylib.GetScreenHeight() - 30, 20, Color.White);
    }

    private static void Execute(string cmd, Player player)
    {
        cmd = cmd.Trim();

        if (cmd.StartsWith("/give"))
            RunGive(cmd, player);
        else
            Console.WriteLine($"Unknown command: {cmd}");
    }

    private static void RunGive(string cmd, Player player)
    {
        // Format: /give itemName [amount]
        string[] parts = cmd.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 2)
        {
            Console.WriteLine("Usage: /give <item> [amount]");
            return;
        }

        string itemName = parts[1].ToLower();
        int amount = (parts.Length >= 3 && int.TryParse(parts[2], out int amt)) ? amt : 1;

        // Find item by name
        Item? template = ItemManager.FindByName(itemName);

        if (template == null)
        {
            Console.WriteLine($"Item '{itemName}' not found.");
            return;
        }

        player.playerInventory.AddItem(template, amount);

        Console.WriteLine($"Gave {amount}x {template.Name}");
    }
}