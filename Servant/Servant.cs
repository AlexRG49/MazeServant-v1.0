using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Laberinto;



namespace Sirviente;

public class Servant
{
    //ClassName: representa la clase del personaje
    public string ClassName { get; }

    // Name: almacena el nombre del personaje.
    public string Name { get; }

    // HP: representa la cantidad de puntos de salud del personaje.
    public int Hp { get; set; }

    // Movement: indica cuántos movimientos puede realizar el personaje.
    public int Movement { get; set; }

    // Attack: determina el poder ofensivo del personaje.
    public int Attack { get; set; }

    //Skill: le da nombre a la habilidad de dicho servant
    public string Skill{ get; set; }

    //Position: Indica la posicion en que se encuentra el servant en el tablero
    public (int X, int Y) Position { get; set; }

    //Propiedad para ayudarme con el metodo de la trampa SkipTurn
    public bool SkipNextTurn { get; set; }

    //SkillCooldown: entero que determinara la cantidad de turnos de cooldown de la skill del servant
    public int SkillCooldown { get; set; } = 0;

    //booleano del que se apoyara SkillCooldown para activarse
    public bool CanUseSkill => SkillCooldown == 0;

    public Servant(string className, int hp, int movement, int attack, string skill)
    {
        // Asigna el nombre recibido al campo ClassName
        ClassName = className;

        // Asigna el nombre recibido al campo Name.
        Name = GetServantName(className);

        // Asigna el valor recibido al campo HP.
        Hp = hp;

        // Asigna la cantidad de movimientos recibida al campo Movement.
        Movement = movement;

        // Asigna el valor de ataque recibido al campo Attack.
        Attack = attack;

        // Asigna el nombre a la habilidad
        Skill = skill;

        SkipNextTurn = false;
    }

    //Metodo para despues de seleccionada la clase del servant, este adquiera su nombre
    private static string GetServantName(string className)
    {
        return className switch
        {
            "Saber" => "Arturia",
            "Archer" => "Emiya",
            "Lancer" => "Cu Chulainn",
            "Rider" => "Medusa",
            "Caster" => "Heracles",
            _ => "Unknown Servant"
        };
    }
}