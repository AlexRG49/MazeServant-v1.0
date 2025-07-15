using Sirviente;
using Laberinto;
using Trampa;
using Dirreccion;
using TipodeTrampa;
using Spectre.Console;
using System.Text;




namespace MaestroDelJuego;

public class GameManager
{
    private readonly Maze maze; //Laberinto donde se jugara
    private readonly Servant servant1; //Jugador 1
    private readonly Servant servant2; //Jugador 2
    private readonly int goalX; // posicion X (fila) del objetivo 
    private readonly int goalY; // posicion Y (columna) del objetivo 

    //Mascaras(arrays bidimensionales) para gestionar el juego (posicion del jugador y trampas)
    private readonly int[,] playerPositionMask;
    private readonly Trap[,] trapMask;

    private Servant currentServant; //Jugador que posee el turno
    private string StatusMessage = ""; //Mensaje de estado que se mostrara en la consola

    //Constructor que iniciara el juego con un tamanno n del laberinto
    public GameManager(int size, Servant p1, Servant p2)
    {
        maze = new Maze(size, size);
        maze.Generate();

        servant1 = p1;
        servant2 = p2;
        servant1.Position = (0, 0);
        servant2.Position = (size - 1, size - 1);


        //Ubicando la posicion de la meta en el centro
        goalX = size / 2;
        goalY = size / 2;


        //En estas 2 estoy dando inicio a las masccaras de jugadores y trampas respextivamente
        playerPositionMask = new int[size, size];
        trapMask = new Trap[size, size];

        InitializeTraps(15);
        UpdatePlayerMask();
    }

    //StartGame: Metdodo principal para iniciar y controlar el bucle del juego
    public void StartGame()
    {
        currentServant = servant1;
        bool gameOver = false;

        while (!gameOver)
        {
            if (currentServant.SkipNextTurn)
            {
                StatusMessage = $"[purple]{currentServant.Name} salta su turno[/]";

                currentServant.SkipNextTurn = false;

                //Cambiando el turno al proximo jugador
                currentServant = currentServant == servant1 ? servant2 : servant1;

                continue;
            }

            currentServant.Movement = 1;

            AnsiConsole.Clear();
            DrawGameState();

    
            while (currentServant.Movement > 0)
            {

                var direction = GetDirectionInput();
                bool moved = false;
                if (direction == Direction.Attack)
                    SetAttack(currentServant);

                else if (direction == Direction.Skill)
                    ActivateSkill(currentServant);

                else moved = MovePlayer(currentServant, direction);

                if (moved)
                {
                    currentServant.Movement--;
                }
                gameOver = CheckWinConditions();
                if (gameOver || currentServant.SkipNextTurn)
                    break;

            }

            if (currentServant.SkillCooldown > 0)
            {
                currentServant.SkillCooldown--;
            }




            //Cambiar el turno
            if (!gameOver)
            {
                currentServant = currentServant == servant1 ? servant2 : servant1;
            }
        }
    }

    private void InitializeTraps(int trapCount)
    {
        Random rand = new Random();

        for (int i = 0; i < trapCount; i++)
        {
            int x, y;
            do
            {
                x = rand.Next(maze.Rows);
                y = rand.Next(maze.Columns);
            } while (playerPositionMask[x, y] != 0 || x == goalX && y == goalY);

            TrapType type =
            //Elige un tipo de trampa aleatoria
            (TrapType)rand.Next(Enum.GetValues(typeof(TrapType)).Length);

            trapMask[x, y] = new Trap(type);
        }
    }

    //Actualiza la mascara de posicion con respecto a los jugadores
    private void UpdatePlayerMask()
    {
        Array.Clear(playerPositionMask, 0, playerPositionMask.Length); //Limpia la mascara

        playerPositionMask[servant1.Position.X, servant1.Position.Y] = 1; //Marca la posicion del jugador 1 como 1 

        playerPositionMask[servant2.Position.X, servant2.Position.Y] = 2; //Marca la posicion de ljugador 2 como 2

    }

    //Metodo para implementar el ataque basico del servant
    private void SetAttack(Servant attacker) {
        Servant target = attacker == servant1 ? servant2 : servant1; //Indica que servant es el atacante

        int distance = Math.Abs(attacker.Position.X - target.Position.X) + Math.Abs(attacker.Position.Y - target.Position.Y);//Determina la distancia entre atacante y objetivo

        //Verifica si sera posible o no atacar
        if (distance <= 2)
        {
            target.Hp -= attacker.Attack;

            StatusMessage = $"[red]{attacker.Name} ha atacado a {target.Name}, causando {attacker.Attack} de danno[/]";
        }

        else
        {
            StatusMessage = $"[red]El objetivo esta fuera de rango[/]";
        }
    }





    //Metodo que da validez a la activazion de la habilidad del servant
    private void ActivateSkill(Servant servant)
    {
        if (!servant.CanUseSkill)
        {
            StatusMessage = $"[red]El servant {servant.Name} no puede usar su habilidad[/]";

            return;
        }

        Servant target = servant == servant1 ? servant2 : servant1;
        bool success = false; //variable de control para saber si se uso la habilidad

        switch (servant.ClassName)
        {
            case "Saber"://Ataque a enemigo cercano de 65 de danno
                if (IsAdjacent(servant.Position, target.Position))
                {
                    target.Hp -= 65;
                    success = true;
                }
                break;

            case "Archer"://Ataque a distancia de 40 de danno(3x3 casillas)
                if (IsInRange(servant.Position, target.Position, 3))
                {
                    target.Hp -= 40;
                    success = true;
                }
                break;

            case "Lancer"://Ataque en linea recta de 50 de danno(hasta 5 casillas)
                if (IsInLine(servant.Position, target.Position, 5))
                {
                    target.Hp -= 50;
                    success = true;
                }
                break;

            case "Rider"://Teletransporta a Rider en un rango de 3x2 casillas
                success = TeleportRider(servant, 3, 2);
                break;

            case "Berserker": //Berserker podra hacer 2 movimientos este turno
                servant.Movement += 2;
                success = true;
                StatusMessage = $"[green]{servant.Name} puede moverse 2 veces este turno[/]";
                break;
        }

        if (success)
        {
            servant.SkillCooldown = 4;

            StatusMessage = $"[gold1]{servant.Name} usa {servant.Skill}[/]";
        }

        else {
                StatusMessage = $"[red]No se puede usar la habilidad en este momento[/]";
            }

    }

    private void ReduceCooldownForPlayer(Servant movedServant)
    {
        if (movedServant.SkillCooldown > 0)
            movedServant.SkillCooldown--;
  

    }

    private bool IsAdjacent((int X, int Y) pos1, (int X, int Y) pos2)//booleano que valida el rango de posicion mediante el calculo de valor absoluto(maximo 2 casillas en cada direccion)
    {
        return Math.Abs(pos1.X - pos2.X) <= 2 && Math.Abs(pos1.Y - pos2.Y) <= 2;
    }

    private bool IsInRange((int X, int Y) pos1, (int X, int Y) pos2, int range)//retornara true si el objetivo esta dentro del rango de archer(cuadrado de amplitud range)
    {
        return Math.Abs(pos1.X - pos2.X) <= range && Math.Abs(pos1.Y - pos2.Y) <= range;
    }

    private bool IsInLine((int X, int Y) start, (int X, int Y) end, int maxDistance)
    {
        //Booleano que comprobara que esten en la misma fila o columna y retornara true si hay una distancia de 5 o menos
        if (start.X != end.X && start.Y != end.Y) return false;

        int distance = Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y);
        return distance <= maxDistance && distance > 0;
    }

    private bool TeleportRider(Servant servant, int deltaX, int deltaY)
    {
        var choices = new List<(int X, int Y)>();//lista de todas las posibles posiciones a las que se puede mover

        //Genera todas las posiciones dentro del rango permitido
        for (int x = Math.Max(0,servant.Position.X - deltaX); x <= Math.Min(maze.Rows-1, servant.Position.X + deltaX); x++)
        {
            for (int y = Math.Max(0,servant.Position.Y - deltaY); y <= Math.Min(maze.Columns-1, servant.Position.Y + deltaY); y++)
            {
                if (x == servant.Position.X && y == servant.Position.Y)//Condicional para que no se transporte a su posicion actual
                    continue;

                if (x == goalX && y == goalY) //Condicional para que no se pueda transportar directamente al Santo Grial
                    continue;

                if (playerPositionMask[x, y] != 0) //Condicional para que no se teletransporte sobre otro jugador
                    continue;

                choices.Add((x, y));
            }
        }

        //Mostrar menu para poder trasladarse a la posicion deseada
        var newPos = AnsiConsole.Prompt(new SelectionPrompt<(int X, int Y)>()
        .Title($"[blue]Selecciona una casilla para que {servant.Name} vuele a ella[/]")
        .PageSize(10)
        .UseConverter(pos => $"Fila: {pos.X}, Columna: {pos.Y}")
        .AddChoices(choices));

        //Actualizar la posicion en el laberinto y la mascara
        servant.Position = newPos;
        UpdatePlayerMask();

        return true;
    }

    //Metodo que solicita al jugador que elija una direccion 
    private Direction GetDirectionInput()
    {
        return AnsiConsole.Prompt(new SelectionPrompt<Direction>()
        .Title($"[yellow]Turno de {currentServant.Name} ({currentServant.ClassName}) [/]")
        .PageSize(6)
        .UseConverter(d => d switch
        {
            Direction.Norte => "Arriba (↑)",
            Direction.Sur => "Abajo(↓)",
            Direction.Este => "Derecha(→)",
            Direction.Oeste => "Izquierda(←)",
            Direction.Attack => "Atacar",
            Direction.Skill => "Usar Habilidad",
            _ => d.ToString()
        })
        .AddChoices(Enum.GetValues<Direction>()));
    }

    //MovePlayer: Define al programa en que direccion se movera el jugador
    private bool MovePlayer(Servant servant, Direction direction)
    {
        var (x, y) = servant.Position;

        var newPos = direction switch
        {
            Direction.Norte => (x - 1, y),
            Direction.Sur => (x + 1, y),
            Direction.Este => (x, y + 1),
            Direction.Oeste => (x, y - 1),


            _ => (x, y)
        };

        //Condicional que verifica si se puede desplazar el jugador a dicha casilla
        if (!maze.CanMove(x, y, newPos.Item1, newPos.Item2))
        {
            StatusMessage = "[red]Movimiento invalido[/]";
            return false;
        }

        servant.Position = newPos; //Actualiza la posicion del servant
        UpdatePlayerMask(); //Actualiza la mascara de posiciones

        CheckTrap(servant);
        StatusMessage = $"[green]{servant.Name} se movio a ({newPos.Item1},{newPos.Item2}) [/]";
        return true; //movimiento exitoso
    }

    //CheckTrap: Verificando si el jugador cayo en una trampa
    private void CheckTrap(Servant servant)
    {
        var (x, y) = servant.Position;
        Trap trap = trapMask[x, y];

        if (trap != null && !trap.Activated)
        {
            trap.Activate(servant, maze); //Aplicando el efecto de la trampa
            trap.Activated = true;

            StatusMessage = trap.Type switch
            {
                TrapType.Damage => $"[red]Trampa de daño. {servant.Name} pierde 20 Hp[/]",


                TrapType.Teleport => $"[yellow]Teletransporte. {servant.Name} fue movido [/] ",


                TrapType.SkipTurn => $"[purple]Trampa de salto. {servant.Name} pierde su turno[/]",

                _ => StatusMessage

            };
        }
    }

    //Comprueba en cada turno si se cumple la condicion de victoria
    private bool CheckWinConditions()
    {
        //Los proximos if definen la condicion de victoria para cada servant de llegar a la meta
        if (servant1.Position.X == goalX && servant1.Position.Y == goalY)
        {
            ShowVictory(servant1, "Ha alcanzado el Santo Grial");
            return true;
        }

        if (servant2.Position.X == goalX && servant2.Position.Y == goalY)
        {
            ShowVictory(servant2, "Ha alcanzado el Santo Grial");
            return true;
        }

        //Los proximos 2, definen la condicion de victoria para cuando muera 1 de los 2 servant
        if (servant1.Hp <= 0)
        {
            ShowVictory(servant2, $"{servant1.Name} fue derrotado");
            return true;
        }

        if (servant2.Hp <= 0)
        {
            ShowVictory(servant1, $"{servant2.Name} fue derrotado");
            return true;
        }

        return false;
    }

    private void ShowVictory(Servant winner, string reason) //Metodo que depende de un integrante de la clase servant, y de un string reason que se respondera cuand osea usado en WinCondition 
    {
        AnsiConsole.Clear();

        var panel = new Panel($"[bold gold1]{winner.Name} ({winner.ClassName}) gana![/] \n[white]{reason}[/]")
        .Border(BoxBorder.Double)
        .Header("[gold1]Fin del Juego[/]")
        .Padding(2, 2);

        AnsiConsole.Write(panel);
        AnsiConsole.MarkupLine("[grey]Presiona cualquier tecla para continuar[/]");
        Console.ReadKey();
    }

       private void DrawGameState()
{
    // Construir el laberinto como texto
    var mazeText = new StringBuilder();
    
    // Dibujar laberinto con caracteres ASCII
    for (int i = 0; i < maze.Rows; i++)
    {
        // Parte superior de cada fila
        for (int j = 0; j < maze.Columns; j++)
        {
            Cell cell = maze.Grid[i, j];
            mazeText.Append("+");
            mazeText.Append(cell.Norte ? "---" : "   ");
        }
        mazeText.AppendLine("+");

        // Parte central de cada fila
        for (int j = 0; j < maze.Columns; j++)
        {
            Cell cell = maze.Grid[i, j];
            mazeText.Append(cell.Oeste ? "|" : " ");
            
            // Contenido de la celda
            if (playerPositionMask[i, j] == 1)
            {
                mazeText.Append("[blue] ► [/]");
            }
            else if (playerPositionMask[i, j] == 2)
            {
                mazeText.Append("[red] ◊ [/]");
            }
            else if (i == goalX && j == goalY)
            {
                mazeText.Append("[gold1] G [/]");
            }
            else if (trapMask[i, j] != null && !trapMask[i, j].Activated)
            {
                switch (trapMask[i, j].Type)
                {
                    case TrapType.Damage:
                        mazeText.Append("[red] X [/]");
                        break;
                    case TrapType.Teleport:
                        mazeText.Append("[yellow] T [/]");
                        break;
                    case TrapType.SkipTurn:
                        mazeText.Append("[purple] S [/]");
                        break;
                }
            }
            else
            {
                mazeText.Append("   ");
            }
        }
        mazeText.AppendLine(maze.Grid[i, maze.Columns - 1].Este ? "|" : " ");
    }

    // Parte inferior del laberinto
    for (int j = 0; j < maze.Columns; j++)
    {
        mazeText.Append("+---");
    }
    mazeText.AppendLine("+");

    // Mostrar el laberinto
    var mazePanel = new Panel(Align.Center(new Markup(mazeText.ToString())))
        .Border(BoxBorder.Rounded)
        .Header("[bold]Laberinto del Santo Grial[/]");

        //Muestra el laberinto en una variable creada llamada mazePanel, que utiliza la biblioteca del Spectre
        
        AnsiConsole.Write(mazePanel);

        var table = new Table()
        .Border(TableBorder.Simple)
        .AddColumn("Jugador")
        .AddColumn("Clase")
        .AddColumn("Hp")
        .AddColumn("Movimiento")
        .AddColumn("Ataque")
        .AddColumn("Habilidad")
        .AddColumn("Posicion")
        .Width(100)
        //.HideHeaders()

        .AddRow(
            servant1.Name.Substring(0, Math.Min(10, servant1.Name.Length)),
            servant1.ClassName,
            $"{servant1.Hp}",
            $"{servant1.Movement}",
            $"{servant1.Attack}",
            $"{servant1.Skill} (CD: {servant1.SkillCooldown})",
            $"({servant1.Position.X}, {servant1.Position.Y})"
        )

                .AddRow(
            servant2.Name.Substring(0, Math.Min(10, servant2.Name.Length)),
            servant2.ClassName,
            $"{servant2.Hp}",
            $"{servant2.Movement}",
            $"{servant2.Attack}",
            $"{servant2.Skill} (CD: {servant2.SkillCooldown})",
            $"({servant2.Position.X}, {servant2.Position.Y})"
        );

        AnsiConsole.Write(table);

        if (!string.IsNullOrEmpty(StatusMessage)) {
            AnsiConsole.MarkupLine(StatusMessage);  
        }
        
    }

    

}
