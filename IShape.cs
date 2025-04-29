public interface IShape
{
    string GetTypeName();
    Color Color { get; set; }
    void UpdateSidePanelData(SidePanelData panelData);
}

public class Line : IShape
{
    public Point Start { get; set; }
    public Point End { get; set; }
    public Color Color { get; set; }

    public string GetTypeName() => "Line";

    public void UpdateSidePanelData(SidePanelData panelData)
    {
        panelData.Length = $"{Distance(Start, End):F2}";
        panelData.Point1X = $"{Start.X}";
        panelData.Point1Y = $"{Start.Y}";
        panelData.Point2X = $"{End.X}";
        panelData.Point2Y = $"{End.Y}";
    }

    private float Distance(Point p1, Point p2)
    {
        return (float)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
    }
}

public class Circle : IShape
{
    public Point Center { get; set; }
    public float Radius { get; set; }
    public Color Color { get; set; }

    public string GetTypeName() => "Circle";

    public void UpdateSidePanelData(SidePanelData panelData)
    {
        panelData.Diameter = $"{2 * Radius:F2}";
        panelData.Perimeter = $"{2 * Math.PI * Radius:F2}";
        panelData.Point1X = $"{Center.X}";
        panelData.Point1Y = $"{Center.Y}";
    }
}
