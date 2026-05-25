using Godot;
using System.Linq;

public partial class YellowEnemy : CharacterBody2D
{
    [Export] public float Velocidad = 50.0f;
    [Export] private Marker2D[] _puntosPatrulla;
    private Vector2[] _posicionesPatrulla;
    private int _indiceActual = 0;
    private bool _avanzando = true;

    private NavigationAgent2D _navAgent;


    public override void _Ready()
    {

        _navAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
        // Esperamos al primer frame de física antes de inicializar posiciones
        // y asignar el destino, para que GlobalPosition sea correcta.
        ActorSetup();
    }

    private async void ActorSetup()
    {
        // Espera al primer frame de física para que el mapa de navegación
        // y las posiciones globales de los Marker2D estén listas.
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
        InicializarPosiciones();

        if (_posicionesPatrulla != null && _posicionesPatrulla.Length > 0)
        {
            _navAgent.TargetPosition = _posicionesPatrulla[_indiceActual];
        }
    }

    private void InicializarPosiciones()
    {
        if (_puntosPatrulla != null && _puntosPatrulla.Length > 0)
        {
            _posicionesPatrulla = _puntosPatrulla
                .Where(m => m != null)
                .Select(m => m.GlobalPosition)
                .ToArray();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_posicionesPatrulla == null || _posicionesPatrulla.Length == 0)
            return;

        // 1. Actualizar el destino del agente según estado

        // 2. Si llegó al punto de patrulla, avanzar al siguiente
        if (_navAgent.IsTargetReached())
        {
            ActualizarSiguienteIndice();
            _navAgent.TargetPosition = _posicionesPatrulla[_indiceActual];
            return;
        }

        // 3. Calcular y aplicar movimiento
        Vector2 siguientePunto = _navAgent.GetNextPathPosition();
        Vector2 direccion = siguientePunto - GlobalPosition;

        // Evitar Normalized() sobre Vector2.Zero (causaría velocidad cero inesperada)
        if (direccion.LengthSquared() > 0.01f)
        {
            Velocity = direccion.Normalized() * Velocidad;
            MoveAndSlide();
        }
    }

    private void ActualizarSiguienteIndice()
    {
        if (_posicionesPatrulla.Length <= 1) return;

        if (_avanzando)
        {
            _indiceActual++;
            // Sin -1: así el enemigo SÍ llega al último punto antes de volver
            if (_indiceActual >= _posicionesPatrulla.Length)
            {
                _indiceActual = _posicionesPatrulla.Length - 1;
                _avanzando = false;
            }
        }
        else
        {
            _indiceActual--;
            if (_indiceActual <= 0)
            {
                _indiceActual = 0;
                _avanzando = true;
            }
        }
    }


}