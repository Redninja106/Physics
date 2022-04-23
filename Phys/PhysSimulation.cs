using SimulationFramework;
using SimulationFramework.IMGUI;
using System;
using System.Numerics;
using Vector2 = System.Numerics.Vector2;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phys;

internal class PhysSimulation : Simulation
{
    List<Ball> balls;
    Ball mb = new(new(0, 0), .5f);
    static int steps = 8;

    public override void OnInitialize(AppConfig config)
    {
        balls = new();
        balls.Add(mb);
    }

    public override void OnRender(ICanvas canvas)
    {
        ImGui.SliderInt(nameof(steps), ref steps, 1, 16);
        Ball.Layout();

        canvas.Translate(canvas.Width / 2f, canvas.Height / 2f);
        canvas.Scale(canvas.Height / 17f, -canvas.Height / 17f);

        canvas.Transform.Invert(out var t);

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
                    
                    var axis = new Vector2(b1.Position.X - b2.Position.X, b1.Position.Y - b2.Position.Y);
                    var dist = (balls[i].Size + balls[j].Size);
                    if (axis.LengthSquared() < dist * dist)
                    {
                        var delta = dist - axis.Length();
                        var totalMass = balls[i].Mass + balls[j].Mass;
                        var fac1 = 1f - (balls[i].Mass / totalMass);
                        var fac2 = 1f - (balls[j].Mass / totalMass);
                        balls[i].Position += Vector2.Normalize(axis) * delta * fac1;
                        balls[j].Position -= Vector2.Normalize(axis) * delta * fac2;
                    }
                }
            }
        }
        canvas.Clear(Color.Black);
        canvas.DrawCircle((0, 0), 8f, Color.DarkGray);
        balls.ForEach(b => b.Render(canvas));
    }
}

