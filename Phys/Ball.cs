using SimulationFramework;
using SimulationFramework.IMGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vector2 = System.Numerics.Vector2;

namespace Phys;
internal class Ball
{
    public Vector2 Position;
    public Vector2 LastPosition;
    public float Size;
    public Vector2 Velocity { get; private set; }
    public float Mass => Size * Size * MathF.PI;
    private Vector2 acceleration;
    private Color color;

    public static float MaxSpeed = 50;

    public static void Layout()
    {
        ImGui.DragFloat(nameof(MaxSpeed), ref MaxSpeed, 1, 0.1f, 100f);
    }

    public Ball(Vector2 position, float size)
    {
        LastPosition = Position = position;
        this.Size = size;
        color = RandColor();
    }

    Color RandColor()
    {
        Span<byte> col = stackalloc byte[3];
        Random.Shared.NextBytes(col);
        Color result = new(col[0], col[1], col[2]);

        if ((result.ToVector3().Normalized() - Color.DarkGray.ToVector3().Normalized()).Length() > Vector3.One.Length() - 0.1f)
        {
            return RandColor();
        }

        return result;
    }
    
    public void Update(float dt)
    {
        const float drag = 0.05f;

        var velocity = Position - LastPosition;
        this.Velocity = velocity / dt;
        if (MathF.Abs(velocity.Length()) > MaxSpeed * dt)
        {
            velocity = Vector2.Normalize(velocity) * MaxSpeed * dt;
        }

        LastPosition = Position;
        velocity *= MathF.Pow(1f - drag, dt);
        Position += velocity + acceleration * dt * dt;
        acceleration = Vector2.Zero;
        if (this.Position.Length() > 8 - Size)
        {
            this.Position = Vector2.Normalize(this.Position) * (8 - Size);
        }
    }

    public void Render(ICanvas canvas)
    {
        canvas.DrawCircle(this.Position, Size, Color.Black with { R = (byte)(255 * MathF.Cbrt(this.Velocity.Length() / MaxSpeed)) });
    }

    public void Accelerate(Vector2 acceleration)
    {
        this.acceleration += acceleration;
    }
}
