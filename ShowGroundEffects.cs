using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.Shared.Enums;
using ImGuiNET;
using SharpDX;

namespace ShowGroundEffects
{
    public class Core : BaseSettingsPlugin<Settings>
    {
        List<string> list = new List<string>();
        public override bool Initialise()
        {
            return base.Initialise();
        }

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
                foreach (var e in GameController.EntityListWrapper.ValidEntitiesByType[EntityType.Effect])
                {
                    if (e.Path == null) continue;
                    if (e.DistancePlayer > 75) continue;
                    if (e.Path.Contains("ground_effects"))
                    {
                        foreach (var bf in e.Buffs)
                        {
                            if (!list.Contains(bf.Name))
                            {
                                list.Add(bf.Name);
                            }
                            var positionedComponent = e?.GetComponent<Positioned>();
                            if (positionedComponent == null) continue;
                            Vector3 location = new Vector3(positionedComponent.WorldPos.X, positionedComponent.WorldPos.Y, 0);
                            
                            switch (bf.Name)
                            {
                                case "ground_fire_burn" when Settings.ShowFire.Value: //burning ground
                                    DrawEllipseToWorld(location, positionedComponent.Size, Settings.Complexity, 1, Settings.FireColor);
                                    break;
                                case "affliction_demon_cold_degen" when Settings.ShowCold.Value: //frozen ground
                                    DrawEllipseToWorld(location, positionedComponent.Size, Settings.Complexity, 1, Settings.ColdColor);
                                    break;
                                case "ground_desecration" when Settings.ShowChaos.Value: //desecrate
                                    DrawEllipseToWorld(location, positionedComponent.Size, Settings.Complexity, 1, Settings.ChaosColor);
                                    break;
                                case "caustic_cloud" when Settings.ShowChaos.Value: //caustic ground
                                    DrawEllipseToWorld(location, positionedComponent.Size, Settings.Complexity, 1, Settings.ChaosColor);
                                    break;
                                case "elder_ground_spores" when Settings.ShowPhys.Value: //eldritch decay
                                    DrawEllipseToWorld(location, positionedComponent.Size, Settings.Complexity, 1, Settings.PhysicalColor);
                                    break;
                                case "atlas_exile_crusader_aura" when Settings.ShowLight.Value: //mana rune
                                case "atlas_exile_crusader_aura_influence" when Settings.ShowLight.Value:
                                    DrawEllipseToWorld(location, positionedComponent.Size, Settings.Complexity, 1, Settings.LightningColor);
                                    break;
                                case "crimson_priest_boss_degen" when Settings.ShowPhys.Value: //blood ritual
                                    DrawEllipseToWorld(location, positionedComponent.Size, Settings.Complexity, 1, Settings.PhysicalColor);
                                    break;
                            }
                        }
                    }
                }
            }
            catch { }
        }
        public void DrawEllipseToWorld(Vector3 vector3Pos, int radius, int points, int lineWidth, Color color)
        {
            var camera = GameController.Game.IngameState.Camera;

            var plottedCirclePoints = new List<Vector3>();
            var slice = 2 * Math.PI / points;
            for (var i = 0; i < points; i++)
            {
                var angle = slice * i;
                var x = (decimal)vector3Pos.X + decimal.Multiply((decimal)radius, (decimal)Math.Cos(angle));
                var y = (decimal)vector3Pos.Y + decimal.Multiply((decimal)radius, (decimal)Math.Sin(angle));
                plottedCirclePoints.Add(new Vector3((float)x, (float)y, vector3Pos.Z));
            }

            for (var i = 0; i < plottedCirclePoints.Count; i++)
            {
                if (i >= plottedCirclePoints.Count - 1)
                {
                    var pointEnd1 = camera.WorldToScreen(plottedCirclePoints.Last());
                    var pointEnd2 = camera.WorldToScreen(vector3Pos);
                    Graphics.DrawLine(pointEnd1, pointEnd2, lineWidth, color);
                    return;
                }

                var point1 = camera.WorldToScreen(plottedCirclePoints[i]);
                var point2 = camera.WorldToScreen(vector3Pos);
                Graphics.DrawLine(point1, point2, lineWidth, color);
            }
        }
    }
}
