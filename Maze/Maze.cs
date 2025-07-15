using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Spectre.Console;
using Dirreccion;

namespace Laberinto;

public class Cell //Clase para cada celda del laberinto
{
    // Booleanos para indicar si hay muro en cada direccion
    public bool Norte { get; set; } = true;
    public bool Sur { get; set; } = true;
    public bool Este { get; set; } = true;
    public bool Oeste { get; set; } = true;
    public bool Visitada { get; set; } = false; // Booleano para determinar se la celda fue ya visitada
}


public class Maze //Clase para la generacion del laberinto
{
    public Cell[,] Grid { get; private set; } //Matriz de las celdas del laberinto
    public int Rows { get; }
    public int Columns { get; }
    private Stack<(int x, int y)> stack = new Stack<(int x, int y)>(); //Pila para el algoritmo de backtrack

    public Maze(int rows, int columns) //Constructor para iniciar el laberinto con el tamaño requerido
    {
        Rows = rows;
        Columns = columns;
        Grid = new Cell[rows, columns]; //Crear la matriz de celdas
        InitializeGrid();

    }

    private void InitializeGrid() //Iniciar cada celda de la matriz
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                Grid[i, j] = new Cell();
            }
        }
    }

    public void Generate() //Generar el laberinto a traves de bactracking aleatorio
    {
        //Comenzar desde el centro
        int startX = Rows / 2;
        int startY = Columns / 2;
        stack.Push((startX, startY)); //
        Grid[startX, startY].Visitada = true;

        Random rand = new Random();

        while (stack.Count > 0)
        {
            (int x, int y) = stack.Peek(); //Toma la celda actual de la cima de la pila
            var neighbors = GetUnvisitedNeighbors(x, y);//busca vecinos no visitados

            if (neighbors.Count > 0)
            {
                //Selecciona un vecino al azar
                int index = rand.Next(neighbors.Count);
                var (dx, dy, wall) = neighbors[index];

                RemoveWall(x, y, wall);//Quita la pared entre la celda en que esta parado el algoritmo y el vecino elegido

                int newX = x + dx;
                int newY = y + dy;
                Grid[newX, newY].Visitada = true;
                stack.Push((newX, newY));
            }
            else
            {
                stack.Pop();//Retrocede si no hay vecinos no visitados
            }
        }
    }

    //Devuelve una lista de vecinos no visitados de la celda (x, y)
    private List<(int dx, int dy, Direction wall)>
    GetUnvisitedNeighbors(int x, int y)
    {
        var neighbors = new List<(int dx, int dy, Direction wall)>();

        if (x > 0 && !Grid[x - 1, y].Visitada) neighbors.Add((-1, 0, Direction.Norte));

        if (x < Rows - 1 && !Grid[x + 1, y].Visitada) neighbors.Add((1, 0, Direction.Sur));

        if (y > 0 && !Grid[x, y - 1].Visitada) neighbors.Add((0, -1, Direction.Oeste));

        if (y < Columns - 1 && !Grid[x, y + 1].Visitada) neighbors.Add((0, 1, Direction.Este));

        return neighbors;
    }

    //Usado para remover la pared entre la celda (x, y) y el vecino en la direccion indicada
    private void RemoveWall(int x, int y, Direction wall)
    {
        switch (wall)
        {
            case Direction.Norte:
                Grid[x, y].Norte = false;
                if (x > 0) Grid[x - 1, y].Sur = false;
                break;

            case Direction.Sur:
                Grid[x, y].Sur = false;
                if (x < Rows - 1) Grid[x + 1, y].Norte = false;
                break;


            case Direction.Este:
                Grid[x, y].Este = false;
                if (y < Columns - 1) Grid[x, y + 1].Oeste = false;
                break;

            case Direction.Oeste:
                Grid[x, y].Oeste = false;
                if (y > 0) Grid[x, y - 1].Este = false;
                break;

        }
    }

    //Metodo para verificar si es posible moverse de una celda a otra
public bool CanMove(int currentX, int currentY, int newX, int newY)
{
    // Comprueba que la nueva posición esté dentro de los límites
    if (newX < 0 || newX >= Rows || newY < 0 || newY >= Columns)
        return false;

    int diffX = newX - currentX;
    int diffY = newY - currentY;

    // Verifica si hay pared en la dirección del movimiento
    if (diffX == -1 && Grid[currentX, currentY].Norte) return false;
    if (diffX == 1 && Grid[currentX, currentY].Sur) return false;
    if (diffY == -1 && Grid[currentX, currentY].Oeste) return false;
    if (diffY == 1 && Grid[currentX, currentY].Este) return false;

    return true;
}
}