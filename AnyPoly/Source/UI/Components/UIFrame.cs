﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace AnyPoly.UI;

/// <summary>
/// Represents a UI frame component.
/// </summary>
internal class UIFrame : UIComponent, IUIButtonContent<UIFrame>
{
    private const int NumberOfLines = 4;

    private readonly LineData[] lines = new LineData[NumberOfLines];
    private readonly UIContainer innerContainer;

    private int thickness;
    private Color color;

    private bool isRecalculationNeeded = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="UIFrame"/> class.
    /// </summary>
    /// <param name="color">The color of the frame lines.</param>
    public UIFrame(Color color, int thickness)
    {
        this.color = color;
        this.thickness = thickness;

        this.innerContainer = new UIContainer()
        {
            Parent = this,
            Transform = { Type = TransformType.Absolute },
        };

        this.Transform.Recalculated += (s, e) => this.Recalculate();

        this.isRecalculationNeeded = true;
    }

    /// <summary>
    /// Gets or sets the thickness of the frame lines.
    /// </summary>
    /// <remarks>
    /// The thickness is measured in pixels.
    /// </remarks>
    public int Thickness
    {
        get => this.thickness;
        set
        {
            if (this.thickness == value)
            {
                return;
            }

            this.thickness = value;
            this.isRecalculationNeeded = true;
        }
    }

    /// <summary>
    /// Gets or sets the color of the frame lines.
    /// </summary>
    public Color Color
    {
        get => this.color;
        set
        {
            if (this.color == value)
            {
                return;
            }

            this.color = value;
        }
    }

    /// <summary>
    /// Gets the read-only inner container of the frame.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The inner container serves as a designated rectangle within
    /// the frame where child UI elements can be positioned and organized.
    /// </para>
    /// <para>
    /// It is automatically created when <see cref="UIFrame"/>
    /// is initialized and is intended to provide a structured
    /// layout for nested components.
    /// </para>
    /// <para>
    /// Since it is automatically generated and managed by the
    /// <see cref="UIFrame"/> component, it is provided
    /// as a read-only property to avoid external modification.
    /// </para>
    /// </remarks>
    public IUIReadOnlyComponent InnerContainer => this.innerContainer;

    /// <inheritdoc/>
    public override void Update(GameTime gameTime)
    {
        if (this.isRecalculationNeeded)
        {
            this.Recalculate();
        }

        base.Update(gameTime);
    }

    /// <inheritdoc/>
    public override void Draw(GameTime gameTime)
    {
        if (!this.IsEnabled)
        {
            return;
        }

        foreach (LineData line in this.lines)
        {
            SpriteBatchController.SpriteBatch.Draw(
                texture: SpriteBatchController.WhitePixel,
                position: line.Start,
                sourceRectangle: null,
                color: this.color,
                rotation: line.Angle,
                origin: Vector2.Zero,
                scale: line.Scale,
                effects: SpriteEffects.None,
                layerDepth: 0.0f);
        }

        base.Draw(gameTime);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// The button content is hovered if the mouse
    /// cursor is within <see cref="InnerContainer"/>.
    /// </remarks>
    bool IUIButtonContent<UIFrame>.IsButtonContentHovered(Point mousePosition)
    {
        if (this.isRecalculationNeeded)
        {
            this.Recalculate();
        }

        return this.innerContainer
            .Transform
            .ScaledRectangle
            .Contains(mousePosition);
    }

    private void Recalculate()
    {
        this.UpdateLines();
        this.UpdateInnerRectangle();
        this.isRecalculationNeeded = false;
    }

    private void UpdateLines()
    {
        Rectangle rect = this.Transform.ScaledRectangle;
        int scaledThickness = this.GetScaledThickness();

        // Define pairs of start and end points for lines
        var lineEndpoints = new Vector2[]
        {
            new Vector2(rect.Left, rect.Top),
            new Vector2(rect.Right, rect.Top),
            new Vector2(rect.Right, rect.Bottom),
            new Vector2(rect.Left, rect.Bottom),
        };

        for (int i = 0; i < NumberOfLines; i++)
        {
            Vector2 start = lineEndpoints[i];
            Vector2 end = lineEndpoints[(i + 1) % NumberOfLines];

            float length = Vector2.Distance(start, end);
            float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);

            this.lines[i] = new LineData()
            {
                Start = start,
                Angle = angle,
                Scale = new Vector2(length, scaledThickness),
            };
        }
    }

    private void UpdateInnerRectangle()
    {
        Rectangle rect = this.Transform.ScaledRectangle;
        int scaledThickness = this.GetScaledThickness();

        var location = rect.Location += new Point(scaledThickness);
        var size = rect.Size -= new Point(2 * scaledThickness);

        this.innerContainer.Transform.ScaledLocation = location;
        this.innerContainer.Transform.ScaledSize = size;
        this.innerContainer.Transform.RelativePadding = this.Transform.RelativePadding;
    }

    private int GetScaledThickness()
    {
        return (int)(this.thickness * Math.Max(ScreenController.Scale.X, ScreenController.Scale.Y));
    }

    private struct LineData
    {
        public Vector2 Start;
        public float Angle;
        public Vector2 Scale;
    }
}
