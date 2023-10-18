using ExileCore;
using SharpDX;
using System.Collections.Generic;
using System;
using ImGuiNET;
using ExileCore.PoEMemory.Components;
using ExileCore.Shared.Enums;
using ExileCore.PoEMemory.MemoryObjects;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

namespace ShowGroundEffects;

public class ShowGroundEffects : BaseSettingsPlugin<ShowGroundEffectsSettings>
{
    private Camera Camera => GameController.Game.IngameState.Camera;
    List<string> list = new List<string>();

    public override void DrawSettings()
    {
        base.DrawSettings();

        foreach (var str in list)
        {
            ImGui.Text(str);
        }
    }

    public override void Render()
    {
        try
        {
            if (!Settings.Enable
            || GameController.Area.CurrentArea == null
            || GameController.Area.CurrentArea.IsTown
            || GameController.Area.CurrentArea.IsHideout
            || GameController.IsLoading
            || !GameController.InGame
            || GameController.Game.IngameState.IngameUi.StashElement.IsVisibleLocal
            )
            {
                return;
            }
            var effects = GameController.EntityListWrapper.ValidEntitiesByType[EntityType.Effect];
            foreach (var e in effects)
            {
                if (e.Path == null || !e.IsHostile) continue;
                if (e.DistancePlayer > Settings.RenderDistance) continue;
                if (e.Path.Contains("ground_effects"))
                {
                    var positionedComponent = e?.GetComponent<Positioned>();
                    if (positionedComponent == null) continue;
                    var drawColor = Color.Transparent;
                    foreach (var bf in e.Buffs)
                    {
                        if (bf.Description.ToLower().Contains("fire") || bf.Description.ToLower().Contains("burning"))
                        {
                            drawColor = Settings.FireColor;
                        } else if (bf.Description.ToLower().Contains("cold")) 
                        {
                            drawColor = Settings.ColdColor;
                        }
                        else if (bf.Description.ToLower().Contains("lightning"))
                        {
                            drawColor = Settings.LightningColor;
                        }
                        else if (bf.Description.ToLower().Contains("chaos"))
                        {
                            drawColor = Settings.ChaosColor;
                        }
                        else if (bf.Description.ToLower().Contains("physical"))
                        {
                            drawColor = Settings.PhysicalColor;
                        } else
                        {
                            if (Settings.DebugMode)
                            {
                                if (!list.Contains(bf.Name))
                                {
                                    list.Add(bf.Name);
                                }
                                Graphics.DrawText(bf.Name, GameController.Game.IngameState.Camera.WorldToScreen(e.PosNum));
                                var background = new RectangleF(GameController.Game.IngameState.Camera.WorldToScreen(e.PosNum).X, GameController.Game.IngameState.Camera.WorldToScreen(e.PosNum).Y, 150, 20);
                                Graphics.DrawBox(background, Color.Black);
                            }
                        }
                        if (drawColor != Color.Transparent)
                        {
                            DrawFilledCircleInWorldPosition(GameController.IngameState.Data.ToWorldWithTerrainHeight(positionedComponent.GridPosition), positionedComponent.Size, 1, drawColor);
                        }
                    }
                }
            }
        }
        catch { }
    }

    /// <summary>
    /// Draws a filled circle at the specified world position with the given radius and color.
    /// </summary>
    /// <param name="position">The world position to draw the circle at.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="color">The color of the circle.</param>
    private void DrawFilledCircleInWorldPosition(Vector3 position, float radius, int thickness, Color color)
    {
        var circlePoints = new List<Vector2>();
        const int segments = 15;
        const float segmentAngle = 2f * MathF.PI / segments;

        for (var i = 0; i < segments; i++)
        {
            var angle = i * segmentAngle;
            var currentOffset = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * radius;
            var nextOffset = new Vector2(MathF.Cos(angle + segmentAngle), MathF.Sin(angle + segmentAngle)) * radius;

            var currentWorldPos = position + new Vector3(currentOffset, 0);
            var nextWorldPos = position + new Vector3(nextOffset, 0);

            circlePoints.Add(Camera.WorldToScreen(currentWorldPos));
            circlePoints.Add(Camera.WorldToScreen(nextWorldPos));
        }

        Graphics.DrawConvexPolyFilled(circlePoints.ToArray(), color);
    }
}