using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using SharpDX;

namespace ShowGroundEffects
{
    public class Settings : ISettings
    {
        public ToggleNode Enable { get; set; } = new ToggleNode(true);
        [Menu("Fire damage")]
        public ToggleNode ShowFire { get; set; } = new ToggleNode(true);
        public ColorNode FireColor { get; set; } = new ColorNode(Color.Red);
        [Menu("Cold damage")]
        public ToggleNode ShowCold { get; set; } = new ToggleNode(true);
        public ColorNode ColdColor { get; set; } = new ColorNode(Color.Blue);
        [Menu("Lightning damage")]
        public ToggleNode ShowLight { get; set; } = new ToggleNode(true);
        public ColorNode LightningColor { get; set; } = new ColorNode(Color.Yellow);
        [Menu("Chaos damage")]
        public ToggleNode ShowChaos { get; set; } = new ToggleNode(true);
        public ColorNode ChaosColor { get; set; } = new ColorNode(Color.Purple);
        [Menu("Physical damage")]
        public ToggleNode ShowPhys { get; set; } = new ToggleNode(true);
        public ColorNode PhysicalColor { get; set; } = new ColorNode(Color.Brown);
    }
}