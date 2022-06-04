using SimulationFramework;
using SimulationFramework.IMGUI;
using System;
using System.Numerics;
using Vector2 = System.Numerics.Vector2;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulationFramework.Drawing.Canvas;

namespace Phys;

internal class PhysSimulation : Simulation
{
    List<Ball> balls;
    Ball mb = new(new(0, 0), .5f);
    static int steps = 8;

    public override void OnInitialize(AppConfig config)
    {
        config.Title = "physics";
        config.Width = 1280;
        config.Height = 720;
        balls = new();
        balls.Add(mb);
    }

    public override void OnRender(ICanvas canvas)
    {
        ImGui.SliderInt(nameof(steps), ref steps, 1, 16);
        ImGui.Text(balls.Count);
        Ball.Layout();

        canvas.Translate(canvas.Width / 2f, canvas.Height / 2f);
        canvas.Scale(canvas.Height / 17f, -canvas.Height / 17f);

        Matrix3x2.Invert(canvas.State.Transform, out var t);

        if (Keyboard.IsKeyPressed(Key.A) || (Keyboard.IsKeyDown(Key.LShift) && Keyboard.IsKeyDown(Key.A)))
        {
            balls.Add(new Ball(Vector2.Transform(Mouse.Position, t), Random.Shared.NextSingle() * .0f + .05f));
        }

        if (Keyboard.IsKeyPressed(Key.C))
        {
            balls.Clear();
            balls.Add(mb);
        }


        for (int s = 0; s < steps; s++)
        {
            mb.Position = Vector2.Transform(Mouse.Position, t);
            mb.Size += Mouse.ScrollWheelDelta * 0.01f;
            balls.ForEach(b =>
            {
                b.Accelerate(new(0, -9.8f));
            });

            balls.ForEach(b => b.Update(Time.DeltaTime / steps));

            for (int i = 0; i < balls.Count; i++)
            {
                for (int j = i + 1; j < balls.Count; j++)
                {
                    var b1 = balls[i];
                    var b2 = balls[j];
                    
                    var axis = b1.Position - b2.Position;
                    var dist = b1.Size + b2.Size;
                    
                    if (axis.LengthSquared() < dist * dist)
                    {
                        var delta = dist - axis.Length();
                        var totalMass = b1.Mass + b2.Mass;
                        var fac1 = 1f - (b1.Mass / totalMass);
                        var fac2 = 1f - (b2.Mass / totalMass);
                        var amt = axis == Vector2.Zero ? Vector2.Zero : Vector2.Normalize(axis) * delta;
                        b1.Position += amt * fac1;
                        b2.Position -= amt * fac2;
                    }
                }
            }
        }
        canvas.Clear(Color.Black);
        canvas.Fill(Color.DarkGray);
        canvas.DrawCircle(0, 0, 8f);
        balls.ForEach(b => b.Render(canvas));
    }
}

