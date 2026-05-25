using Godot;
using System;

public partial class RedEnemy : CharacterBody2D
{
    [Export] public float Velocidad = 150.0f;
    [Export] public Path2D RutaPatrulla; // Arrastra el Path2D aquí desde el inspector

    private NavigationAgent2D _navigationAgent;
    private Vector2[] _puntosPatrulla;
    private int _indiceActual = 0;

    public override void _Ready()
    {
        _navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");

        Callable.From(InicializarPatrulla).CallDeferred();
    }

    private void InicializarPatrulla()
    {
        if (RutaPatrulla != null && RutaPatrulla.Curve != null)
        {
            Vector2[] puntosLocales = RutaPatrulla.Curve.GetBakedPoints();
            _puntosPatrulla = new Vector2[puntosLocales.Length];

            // Convertimos los puntos de la curva a posiciones globales
            for (int i = 0; i < puntosLocales.Length; i++)
            {
                _puntosPatrulla[i] = RutaPatrulla.ToGlobal(puntosLocales[i]);
            }

            ActualizarDestino();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        // Si el agente ya llegó al punto actual, pasamos al siguiente
        if (_navigationAgent.IsNavigationFinished())
        {
            CambiarSiguientePunto();
            return;
        }

        // Calcular movimiento hacia el siguiente punto del mapa de navegación
        Vector2 siguientePosicion = _navigationAgent.GetNextPathPosition();
        Vector2 direccion = GlobalPosition.DirectionTo(siguientePosicion);

        Velocity = direccion * Velocidad;
        MoveAndSlide();
    }

    private void ActualizarDestino()
    {
        _navigationAgent.TargetPosition = _puntosPatrulla[_indiceActual];
    }

    private void CambiarSiguientePunto()
    {
        // Avanza al siguiente índice. Si llega al final, vuelve a 0
        _indiceActual = (_indiceActual + 1) % _puntosPatrulla.Length;
        ActualizarDestino();
    }
}