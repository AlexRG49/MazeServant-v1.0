# Maze Servant: Laberinto del Santo Grial - README

Un juego de estrategia por turnos donde dos jugadores controlan "Servants" (siervos) en un laberinto mágico, compitiendo por alcanzar el Santo Grial primero o derrotar al oponente.

## 🎮 Características Principales
- **Sistema de combate estratégico** por turnos
- **5 Servants únicos** con habilidades especiales
- Laberintos generados proceduralmente
- **Trampas peligrosas** con efectos variados
- Interfaz de usuario en terminal con Spectre.Console
- Sistema de habilidades con cooldowns
- Modo de ataque a distancia

## 🚀 Requisitos del Sistema
- **.NET 5.0 SDK** o superior
- **Spectre.Console** (se instalará automáticamente)
- Sistema operativo: Windows, Linux o macOS
- Terminal con soporte para caracteres Unicode

## ⬇️ Instalación
1. Clona el repositorio:
```bash
git clone https://github.com/tu-usuario/maze-servant.git
```
2. Navega al directorio del proyecto:
```bash
cd maze-servant
```
3. Instala la dependencia de Spectre.Console:
```bash
dotnet add package Spectre.Console
```
4. Compila el proyecto:
```bash
dotnet build
```
5. Ejecuta el juego:
```bash
dotnet run
```

## 🕹️ Cómo Jugar
### Controles
- **Menú principal**: Usa las teclas ↑/↓ para navegar y Enter para seleccionar
- **Movimiento**: Selecciona dirección con ↑/↓/←/→
- **Acciones especiales**: 
  - `Ataque`: Ataca al oponente si está en rango
  - `Habilidad`: Usa la habilidad única de tu Servant

### Objetivo
- **Alcanza el Santo Grial** en el centro del laberinto
- **Derrota al oponente** reduciendo sus HP a 0

### Servants Disponibles
| Clase       | Nombre       | HP  | Ataque | Mov | Habilidad Especial               |
|-------------|--------------|-----|--------|-----|----------------------------------|
| Saber       | Arturia      | 100 | 15     | 3   | Excalibur (65 daño cercano)      |
| Archer      | Emiya        | 90  | 12     | 4   | Trace On (40 daño a distancia)   |
| Lancer      | Cu Chulainn  | 95  | 18     | 5   | Gae Bolg (50 daño en línea)      |
| Rider       | Medusa       | 85  | 10     | 6   | Bellerophon (teletransporte)     |
| Berserker   | Heracles     | 120 | 20     | 2   | Locura (+2 movimientos)          |

### Símbolos en el Laberinto
| Símbolo | Color   | Descripción                  |
|---------|---------|------------------------------|
| ►      | Azul    | Jugador 1                    |
| ◊      | Rojo    | Jugador 2                    |
| G      | Dorado  | Santo Grial (Objetivo)       |
| X      | Rojo    | Trampa de daño (-20 HP)      |
| T      | Verde   | Trampa de teletransporte     |
| S      | Púrpura | Trampa de salto de turno     |

## 🛠️ Tecnologías Utilizadas
- C# 9.0
- .NET 5.0
- Spectre.Console (para interfaz en terminal)
- Algoritmo de generación de laberintos (backtracking)

## 📂 Estructura del Proyecto
```
MazeServant/
├── Program.cs          - Punto de entrada principal
├── GameManager.cs      - Controlador del juego
├── Maze.cs             - Generador de laberintos
├── Servant.cs          - Implementación de los personajes
├── Trap.cs             - Sistema de trampas
├── Direction.cs        - Enumeración de direcciones
├── TrapType.cs         - Tipos de trampas
└── README.md           - Este archivo
```

## 👥 Créditos
- Desarrollado con esfuerzo y dedicación
- Agradecimientos especiales a:
  - El café que hizo esto posible ☕
  - Los cigarros de la bodega 🚬
  - DeepSeek por su apoyo técnico
  - Todos los que ayudaron en el desarrollo

¡Que disfrutes del juego y que la suerte esté siempre de tu lado en la búsqueda del Santo Grial!
