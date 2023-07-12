using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using SharpDX;

namespace ShowGroundEffects;

public class ShowGroundEffectsSettings : ISettings
{
    public ToggleNode Enable { get; set; } = new ToggleNode(false);

    [Menu("Complexity of drawing")]
    public RangeNode<int> Complexity { get; set; } = new RangeNode<int>(300, 0, 1000);
    [Menu("Render distance")]
    public RangeNode<int> RenderDistance { get; set; } = new RangeNode<int>(80, 0, 100);
    [Menu("If you want to hide an element, set the color transparency to max", 100)]
    public EmptyNode Description { get; set; }
    [Menu("Fire damage", parentIndex = 100)]
    public ColorNode FireColor { get; set; } = new ColorNode(Color.Red);
    [Menu("Cold damage", parentIndex = 100)]
    public ColorNode ColdColor { get; set; } = new ColorNode(Color.Blue);
    [Menu("Lightning damage", parentIndex = 100)]
    public ColorNode LightningColor { get; set; } = new ColorNode(Color.Yellow);
    [Menu("Chaos damage", parentIndex = 100)]
    public ColorNode ChaosColor { get; set; } = new ColorNode(Color.Purple);
    [Menu("Physical damage", parentIndex = 100)]
    public ColorNode PhysicalColor { get; set; } = new ColorNode(Color.Brown);
    [Menu("Debug mode (shows encountered ground effects)")]
    public ToggleNode DebugMode { get; set; } = new ToggleNode(false);
}