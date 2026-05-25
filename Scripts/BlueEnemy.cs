using Godot;
using System.Linq;

public partial class BlueEnemy : CharacterBody2D
{
    [Export] public float Velocidad = 50.0f;
    [Export] private Marker2D _puntoInicial;

    private bool _perseguir = false;
    private NavigationAgent2D _navAgent;
    private Node2D _jugadorObjetivo = null;

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

        if (_puntoInicial != null)
        {
            _navAgent.TargetPosition = _puntoInicial.GlobalPosition;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_puntoInicial == null)
            return;

        // 1. Actualizar el destino del agente según estado
        if (_perseguir && GodotObject.IsInstanceValid(_jugadorObjetivo))
        {
            _navAgent.TargetPosition = _jugadorObjetivo.GlobalPosition;
        }

        // 2. Si llegó al punto de patrulla, avanzar al siguiente
        if (!_perseguir && _navAgent.IsTargetReached())
        {
            _navAgent.TargetPosition = _puntoInicial.GlobalPosition;
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


    // --- SEÑALES DEL AREA2D DE DETECCIÓN ---

    private void _on_detect_area_body_entered(Node2D body)
    {
        // Más idiomático en C# que GetType() == typeof(Player)
        if (body is Player player)
        {
            _jugadorObjetivo = player;
            _perseguir = true;
        }
    }

    private void _on_perseguir_area_body_exited(Node2D body)
    {
        if (body == _jugadorObjetivo)
        {
            _perseguir = false;
            _jugadorObjetivo = null;

            // Reasignamos explícitamente para que retome la patrulla de inmediato
            if (_puntoInicial != null)
            {
                _navAgent.TargetPosition = _puntoInicial.GlobalPosition;
            }
        }
    }
}