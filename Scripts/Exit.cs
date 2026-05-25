using Godot;
using System;

public partial class Exit : Area2D
{
    private void _on_body_entered(Node2D body)
    {
        GD.Print($"Cuerpo entró en el área de salida: {body.Name} ({body.GetType().Name})");

        if (body is Player player)
        {
            GetTree().CallDeferred(
                SceneTree.MethodName.ChangeSceneToFile,
                "res://Scenes/win_panel.tscn"
            );
        }
    }
}