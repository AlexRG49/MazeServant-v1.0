using System;
using System.Collections.Generic;
using System.Threading;
using MaestroDelJuego;
using Sirviente;
using Spectre.Console;

class Program
{
    static void Main(string[] args)
    {
        while (true) /* while para el menu en spectre*/
        {
            AnsiConsole.Clear();
            DisplayTitle();
            var choice = ShowMainMenu();

            switch (choice)
            {
                case "Nueva Partida":
                    StartNewGame();
                    break;

                case "Ayuda":
                    ShowHelp();
                    break;

                case "Creditos":
                    ShowCredits();
                    break;

                case "Salir":
                    return;
            }
        }



        static void DisplayTitle()
        {
            var title = new FigletText("Maze Servant")
            .Centered()
            .Color(Color.White);

            var rule = new Rule("[blue]Laberinto del Santo Grial[/]").RuleStyle(Style.Parse("blue dim"));

            AnsiConsole.Write(title);
            AnsiConsole.Write(rule);
            AnsiConsole.WriteLine();
        }

        static string ShowMainMenu()
        {
            return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("[yellow]Menu Principal[/]")
            .PageSize(5)
            .HighlightStyle(new Style(foreground: Color.Blue))
            .AddChoices(new[]{
                "Nueva Partida",
                "Ayuda",
                "Creditos",
                "Salir"
            }));
        }

        static void StartNewGame()
        {
            var servant = new List<Servant>
            {
                new Servant("Saber", 100, 3, 15, "Excalibuur: Ataque masivo de luz"),
                new Servant("Archer", 90, 4, 12, "Trace On: Disparo certero"),
                new Servant("Lancer", 95, 5, 18, "Gae Bolg: Ataque directo al corazon"),
                new Servant("Rider", 85, 6, 10, "Bellerophon: Montura voladora"),
                new Servant("Berserker", 120, 2, 20, "Locura que aumenta la cantidad de movimientos")
            };

            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[blue]Jugador 1: Elige tu Servant[/]"));
            var servant1 = AnsiConsole.Prompt(
                new SelectionPrompt<Servant>()
                .Title("Selecciona un Servant:")
                .PageSize(5)
                .UseConverter(p => $"{p.Name} ({p.ClassName}) - Hp:{p.Hp} |Attack {p.Attack}")
                .AddChoices(servant)
            );

            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[red]Jugador 2: Elige tu Servant[/]"));
            var servant2 = AnsiConsole.Prompt(
                new SelectionPrompt<Servant>()
                .Title("Selecciona un Servant:")
                .PageSize(5)
                    .UseConverter(p => $"{p.Name} ({p.ClassName}) - Hp:{p.Hp} |Attack {p.Attack}")
                .AddChoices(servant)
            );

             //Empezar el juego
            var game = new GameManager(10, servant1, servant2);
            game.StartGame();
            

        }

        static void ShowHelp()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[blue]Ayuda[/]").RuleStyle(Style.Parse("blue")));

            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("Simbolo")
                .AddColumn("Descripcion")
                .AddRow("[blue]►[/]", "Jugador 1")
                .AddRow("[red]◊[/]", "Jugador 2")
                .AddRow("[gold1]G[/]", "Santo Grial(Objetivo)")
                .AddRow("[red]X[/]", "Trampa de daño")
                .AddRow("[green]T[/]", "Trampa de teletransporte")
                .AddRow("[purple]S[/]", "Trampa de salto de turno");
                

            AnsiConsole.Write(table);

            AnsiConsole.MarkupLine("\nPresiona cualquier tecla para voler al inicio");
            Console.ReadKey();
        }

        static void ShowCredits()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[white]Creditos[/]").RuleStyle(Style.Parse("white")));

            var content = new Markup(
                        "[white]Desarrollado con esfuerzo pero sobre todo apuro[/]\n" +
                        "[white]Dedicarle unos agradecimientos especiales a:[/]\n" +
                        "[white] ~ El cafe que sin el nada de esto fuera posible[/]\n" +
                        "[white] ~ Los cigarros de la bodega[/]\n" +
                        "[white] ~ Toda la gente que me ayudo a sacar esto por hexagesima vez(gracias si lee esto profe)[/]\n" +
                        "[white] ~ DeepSeek(5mentarios)[/]"

                    );
                var panel = new Panel(Align.Center(content))
            .Border(BoxBorder.Rounded)
            .BorderStyle(Style.Parse("white"))
            .Padding(2, 1);

            AnsiConsole.Write(panel);

            AnsiConsole.MarkupLine("\n[white]Presiona cualquier tecla para volver al inicio[/]");
            Console.ReadKey();
        }
    }
}