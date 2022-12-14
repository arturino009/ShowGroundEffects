﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
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
                var effects = GameController.EntityListWrapper.ValidEntitiesByType[EntityType.Effect];
                //var monsters = GameController.EntityListWrapper.ValidEntitiesByType[EntityType.Monster];
                //var finalCollection = new ConcurrentBag<Entity>(effects.Union(monsters));
                foreach (var e in effects)
                {
                    if (e.Path == null) continue;
                    if (e.DistancePlayer > 75) continue;
                    if (e.Path.Contains("ground_effects")) // || e.Path.Contains("CrusaderArcaneRune"))
                    {
                        foreach (var bf in e.Buffs)
                        {
                            var positionedComponent = e?.GetComponent<Positioned>();
                            if (positionedComponent == null) continue;
                            Vector3 location = new Vector3(positionedComponent.WorldPos.X, positionedComponent.WorldPos.Y, GameController.Game.IngameState.Data.LocalPlayer.Pos.Z);
                            
                            switch (bf.Name)
                            {
                                case "ground_fire_burn": //burning ground
                                    DrawEllipseToWorld(location, positionedComponent.Size, Settings.Complexity, 1, Settings.FireColor);
                                    break;
                                case "affliction_demon_cold_degen": //frozen ground
                                    DrawEllipseToWorld(location, positionedComponent.Size, Settings.Complexity, 1, Settings.ColdColor);
                                    break;
                                case "ground_archnemesis_cold_snap": //cold snap
                                    DrawEllipseToWorld(location, positionedComponent.Size, Settings.Complexity, 1, Settings.ColdColor);
                                    break;
                                case "ground_maelstrom_chill":
                                    DrawEllipseToWorld(location, positionedComponent.Size, Settings.Complexity, 1, Settings.ColdColor);
                                    break;
                                case "ground_desecration": //desecrate
                                    DrawEllipseToWorld(location, positionedComponent.Size, Settings.Complexity, 1, Settings.ChaosColor);
                                    break;
                                case "caustic_cloud": //caustic ground
                                    DrawEllipseToWorld(location, positionedComponent.Size, Settings.Complexity, 1, Settings.ChaosColor);
                                    break;
                                case "elder_ground_spores": //eldritch decay
                                    DrawEllipseToWorld(location, positionedComponent.Size, Settings.Complexity, 1, Settings.PhysicalColor);
                                    break;
                                //case "atlas_exile_crusader_aura" when Settings.ShowLight.Value: //mana rune
                                //    DrawEllipseToWorld(location, positionedComponent.Size, Settings.Complexity, 1, Settings.LightningColor);
                                //    break;
                                //case "atlas_exile_crusader_aura_influence" when Settings.ShowLight.Value:
                                //    DrawEllipseToWorld(location, positionedComponent.Size, Settings.Complexity, 1, Settings.LightningColor);
                                //    break;
                                case "ground_vortex_lightning":
                                    DrawEllipseToWorld(location, positionedComponent.Size, Settings.Complexity, 1, Settings.LightningColor);
                                    break;
                                case "crimson_priest_boss_degen": //blood ritual
                                    DrawEllipseToWorld(location, positionedComponent.Size, Settings.Complexity, 1, Settings.PhysicalColor);
                                    break;
                                case "ground_devouring_darkness": //kitava
                                    DrawEllipseToWorld(location, positionedComponent.Size, Settings.Complexity, 1, Settings.PhysicalColor);
                                    break;
                                case "vomitous_ooze": //eater of worlds
                                    DrawEllipseToWorld(location, positionedComponent.Size, Settings.Complexity, 1, Settings.PhysicalColor);
                                    DrawEllipseToWorld(location, positionedComponent.Size, Settings.Complexity, 1, Settings.ChaosColor);
                                    break;
                                case "ground_ice_chill":
                                case "ground_brittle":
                                case "ground_consecration_enemy":
                                case "ground_consecration":
                                case "ground_lightning_shock":
                                case "generic_buff_aura":
                                case "ground_tar_slow":
                                    break;
                                default:
                                    if (Settings.DebugMode)
                                    {
                                        if (!list.Contains(bf.Name))
                                        {
                                            list.Add(bf.Name);
                                        }
                                        Graphics.DrawText(bf.Name, GameController.Game.IngameState.Camera.WorldToScreen(location));
                                        var background = new RectangleF(GameController.Game.IngameState.Camera.WorldToScreen(location).X, GameController.Game.IngameState.Camera.WorldToScreen(location).Y, 150, 20);
                                        Graphics.DrawBox(background, Color.Black);
                                    }
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
            if(radius>10000){
                DebugWindow.LogError("Entity radius offset is wrong! Ask for the HUD team to fix it!");
                return;
            }
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

            for (var i = 1; i < plottedCirclePoints.Count/2; i++)
            {
                //if (i >= plottedCirclePoints.Count - 1)
                //{
                //    var pointEnd1 = camera.WorldToScreen(plottedCirclePoints.Last());
                //    var pointEnd2 = camera.WorldToScreen(vector3Pos);
                //    Graphics.DrawLine(pointEnd1, pointEnd2, lineWidth, color);
                //    return;
                //}

                var point1 = camera.WorldToScreen(plottedCirclePoints[i]);
                var point2 = camera.WorldToScreen(plottedCirclePoints[plottedCirclePoints.Count - i]);
                Graphics.DrawLine(point1, point2, lineWidth, color);
            }
        }
    }
}
