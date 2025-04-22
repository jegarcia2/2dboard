class DrawingCanvas : Panel
{
    public Point centerPoint;
    private bool isDragging = false;
    private bool isErasing = false;
    private Point lastMousePosition;
    public String selectedMouse = "Select";
    public new Point MousePosition = new Point(0, 0);
    private Point? tempStartPoint = null;
    public List<(Point Start, Point End, Color color)> lines = new List<(Point, Point, Color)>();
    public object selectedLine;
    public int selectedLineIndex = -1; 

public event Action<SidePanelData>? OnUpdateSidePanel;


    public Color selectedColor = Color.White;

    public DrawingCanvas()
    {
        this.DoubleBuffered = true; // Prevent flickering
        this.centerPoint = new Point(Width / 2, Height / 2); // Start at center
        this.MouseDown += Canvas_MouseDown;
        this.MouseMove += Canvas_MouseMove;
        this.MouseUp += Canvas_MouseUp;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;

        Pen axisPen = new Pen(Color.White, 2);

        g.DrawLine(axisPen, new Point(0, centerPoint.Y), new Point(this.Width, centerPoint.Y));
        g.DrawLine(axisPen, new Point(centerPoint.X, 0), new Point(centerPoint.X, this.Height));

        foreach (var line in lines)
        {
            Pen LinePen = new Pen(line.color, 2);
            Point p1 = new Point(centerPoint.X + line.Start.X, centerPoint.Y - line.Start.Y);
            Point p2 = new Point(centerPoint.X + line.End.X, centerPoint.Y - line.End.Y);
            g.DrawLine(LinePen, p1, p2);
            LinePen.Dispose();
        }

        // Optional: draw preview line from tempStartPoint to current mouse
        if (selectedMouse == "Line" && tempStartPoint.HasValue)
        {
            Pen TempLinePen = new Pen(selectedColor, 1);
            Point currentPos = GetRelativeCoordinates(this.PointToClient(Cursor.Position));

            // Convert back to canvas coordinates for drawing
            Point drawStart = new Point(tempStartPoint.Value.X + centerPoint.X, centerPoint.Y - tempStartPoint.Value.Y);
            Point drawEnd = new Point(currentPos.X + centerPoint.X, centerPoint.Y - currentPos.Y);

            g.DrawLine(TempLinePen, drawStart, drawEnd);
            TempLinePen.Dispose();
        }
    }

    private void Canvas_MouseDown(object sender, MouseEventArgs e)
    {
        Point clickedPoint = GetRelativeCoordinates(e.Location);

        switch (selectedMouse)
        {
            case "Select":
                SelectElement(e.Location);
                break;

            case "Move":
                isDragging = true;
                break;

            case "Line":
                if (tempStartPoint == null)
                {
                    // First click - save start point
                    tempStartPoint = clickedPoint;
                }
                else
                {
                    // Second click - save line and reset
                    lines.Add((tempStartPoint.Value, clickedPoint, selectedColor));
                    tempStartPoint = null;
                    this.Invalidate(); // Trigger redraw
                }
                break;

            case "Eraser":
                TryEraseAt(e.Location);
                isErasing = true;
                break;
        }

        lastMousePosition = e.Location;
    }

    private void Canvas_MouseUp(object sender, MouseEventArgs e)
    {
        isDragging = false;
        isErasing = false;
        if (selectedMouse == "Move")
        {
            Cursor = Cursors.Hand;
        }
        else
        {
            Cursor = Cursors.Default;
        }
    }

    private void Canvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (isDragging)
        {
            Cursor = Cursors.SizeAll;
            // Move the center point
            centerPoint.X += e.X - lastMousePosition.X;
            centerPoint.Y += e.Y - lastMousePosition.Y;
            lastMousePosition = e.Location;

            this.Invalidate(); // Redraw canvas
        }
        else if (selectedMouse == "Line" && tempStartPoint.HasValue)
        {
            this.Invalidate(); // Redraw canvas
        }
        else if (isErasing)
        {
            TryEraseAt(e.Location, true);
        }
    }

    private Point GetRelativeCoordinates(Point location)
    {
        return new Point(location.X - centerPoint.X, centerPoint.Y - location.Y);
    }

    private void TryEraseAt(Point mousePos, bool Continuous = false)
    {
        Point relativePos = GetRelativeCoordinates(mousePos);

        for (int i = lines.Count - 1; i >= 0; i--)
        {
            var line = lines[i];

            if (IsPointNearLine(relativePos, line.Start, line.End))
            {
                lines.RemoveAt(i);
                Invalidate(); // Triggers a redraw
                if (!Continuous)
                {
                    break;
                }
            }
        }
    }

    private bool IsPointNearLine(Point p, Point a, Point b, int threshold = 5)
    {
        float distance = DistanceFromPointToLine(p, a, b);
        return distance <= threshold;
    }

    private float DistanceFromPointToLine(Point p, Point a, Point b)
    {
        float dx = b.X - a.X;
        float dy = b.Y - a.Y;

        if (dx == 0 && dy == 0)
            return Distance(p, a);

        float t = ((p.X - a.X) * dx + (p.Y - a.Y) * dy) / (dx * dx + dy * dy);
        t = Math.Max(0, Math.Min(1, t));

        float projX = a.X + t * dx;
        float projY = a.Y + t * dy;

        return Distance(p, new Point((int)projX, (int)projY));
    }

    private void SelectElement(Point mousePos)
    {
        Point relativePos = GetRelativeCoordinates(mousePos);
        for (int i = lines.Count - 1; i >= 0; i--)
        {
            var line = lines[i];

            if (IsPointNearLine(relativePos, line.Start, line.End))
            {
                selectedLine = line; // Use the tuple directly
                selectedLineIndex = i;
                ThrowInformationToSidePanel(selectedLine); // Display the information
                Invalidate(); // Triggers a redraw
            }
        }
    }

    private float Distance(Point p1, Point p2)
    {
        return (float)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
    }

private void ThrowInformationToSidePanel(Object selectedLine)
{
    // Unbox the selected line from the object
    var line = (ValueTuple<Point, Point, Color>)selectedLine;

    // Calculate the length of the line
    double length = Distance(line.Item1, line.Item2);

    // Prepare side panel data
    SidePanelData panelData = new SidePanelData
    {
        Length = $"{length:F2}",
        Point1X = $"{line.Item1.X}",
        Point1Y = $"{line.Item1.Y}",
        Point2X = $"{line.Item2.X}",
        Point2Y = $"{line.Item2.Y}",
        Color = line.Item3.Name
    };

    // Trigger the event to update the side panel with editable fields
    OnUpdateSidePanel?.Invoke(panelData);
}

    public void cancelLine()
    {
        if (tempStartPoint.HasValue)
        {
            // Reset drawing state
            tempStartPoint = null;
            this.Invalidate(); // Trigger a redraw
        }
    }
}
