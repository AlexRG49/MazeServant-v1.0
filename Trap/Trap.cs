using Laberinto;
using Sirviente;
using TipodeTrampa;

namespace Trampa;

public class Trap
{
    public TrapType Type { get; }
    public bool Activated { get; set; }

    public Trap(TrapType type)
    {
        Type = type;
        Activated = false;
    }

    public void Activate(Servant servant, Maze maze)
    {
        Random rand = new Random();

        switch (Type)
        {

            //Para este caso la trampa activada restara 20 puntos de vida del servant que la pise
            case TrapType.Damage:
                servant.Hp -= 20;
                break;

            case TrapType.SkipTurn:
                servant.SkipNextTurn = true;
                break;

            //Genera una posicion aleatoria en el tablero, el cual se repite hasta que sea posible alcanzar la casilla, tras esto hace un cambio automatico de la posicion del jugador en dicha posicion
            case TrapType.Teleport:
                int newX, newY;
                do
                {
                    newX = rand.Next(0, maze.Rows);
                    newY = rand.Next(0, maze.Columns);
                } while (!maze.CanMove(0, 0, newX, newY));

                servant.Position = (newX, newY);
                break;
        }
    }
}