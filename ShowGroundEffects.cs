using ExileCore;
using SharpDX;
using System.Collections.Generic;
using System;
using ImGuiNET;
using ExileCore.PoEMemory.Components;
using ExileCore.Shared.Enums;
using ExileCore.PoEMemory.MemoryObjects;

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
                    foreach (var bf in e.Buffs)
                    {
                        if (bf.Description.ToLower().Contains("fire") || bf.Description.ToLower().Contains("burning"))
                        {
                            DrawEllipseToWorld(e, positionedComponent.Size, Settings.Complexity, 1, Settings.FireColor);
                        } else if (bf.Description.ToLower().Contains("cold")) 
                        {
                            DrawEllipseToWorld(e, positionedComponent.Size, Settings.Complexity, 1, Settings.ColdColor);
                        }
                        else if (bf.Description.ToLower().Contains("lightning"))
                        {
                            DrawEllipseToWorld(e, positionedComponent.Size, Settings.Complexity, 1, Settings.LightningColor);
                        }
                        else if (bf.Description.ToLower().Contains("chaos"))
                        {
                            DrawEllipseToWorld(e, positionedComponent.Size, Settings.Complexity, 1, Settings.ChaosColor);
                        }
                        else if (bf.Description.ToLower().Contains("physical"))
                        {
                            DrawEllipseToWorld(e, positionedComponent.Size, Settings.Complexity, 1, Settings.PhysicalColor);
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
                    }
                }
            }
        }
        catch { }
    }

    public void DrawEllipseToWorld(Entity ent, int radius, int points, int lineWidth, Color color)
    {
        if (radius > 10000)
        {
            DebugWindow.LogError("Entity radius offset is wrong! Ask for the HUD team to fix it!");
            return;
        }

        var plottedCirclePoints = new List<Vector3>();
        var slice = 2 * Math.PI / points;
        for (var i = 0; i < points; i++)
        {
            var angle = slice * i;
            var x = (decimal)ent.PosNum.X + decimal.Multiply((decimal)radius, (decimal)Math.Cos(angle));
            var y = (decimal)ent.PosNum.Y + decimal.Multiply((decimal)radius, (decimal)Math.Sin(angle));
            //Probably have a mistake for height calculation but its close enough
            plottedCirclePoints.Add(new Vector3((float)x, (float)y, GameController.IngameState.Data.GetTerrainHeightAt(ent.GridPosNum)));
        }

        for (var i = 1; i < plottedCirclePoints.Count / 2; i++)
        {
            var point1 = Camera.WorldToScreen(plottedCirclePoints[i]);
            var point2 = Camera.WorldToScreen(plottedCirclePoints[plottedCirclePoints.Count - i]);
            Graphics.DrawLine(point1, point2, lineWidth, color);
        }
    }
}