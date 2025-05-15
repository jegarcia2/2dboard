class DrawingCanvas : Panel
{
    public event Action<Point?> OnTempStartPointChanged;
    public Point centerPoint;
    private bool isDragging = false;
    private bool isErasing = false;
    private Point lastMousePosition;
    public String selectedMouse = "Select";
    public new Point MousePosition = new Point(0, 0);
    private Point? tempStartPoint = null;
    public List<IShape> shapes = new List<IShape>();
    public int selectedIndex = -1;
    public event Action<SidePanelData>? OnUpdateSidePanel;
    private bool showGrid = false;  // Track grid visibility
    private int gridDimension = 30;

    public Color selectedColor = Color.White;
    private bool snapEnabled = false;

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

        // Draw the axis (as before)
        Pen axisPen = new Pen(Color.White, 2);
        g.DrawLine(axisPen, new Point(0, centerPoint.Y), new Point(this.Width, centerPoint.Y));
        g.DrawLine(axisPen, new Point(centerPoint.X, 0), new Point(centerPoint.X, this.Height));

        // Draw the grid if it's enabled
        if (showGrid)
        {
            Pen gridPen = new Pen(Color.FromArgb(45, 45, 45), 0.5F);  // Grid line color

            // Vertical lines
            for (int x = centerPoint.X % gridDimension; x < this.Width; x += gridDimension)  // Adjust grid to be centered around centerPoint
            {
                g.DrawLine(gridPen, x, 0, x, this.Height);
            }

            // Horizontal lines
            for (int y = centerPoint.Y % gridDimension; y < this.Height; y += gridDimension)  // Adjust grid to be centered around centerPoint
            {
                g.DrawLine(gridPen, 0, y, this.Width, y);
            }
        }

        if (snapEnabled)
        {
            var currentMousePos = GetRelativeCoordinates(this.PointToClient(Cursor.Position));
            // Find the nearest snap point (for lines and circles, same snap points)
            Point? snapPoint = FindSnapPoint(currentMousePos);
            if (snapPoint.HasValue)
            {
                currentMousePos = snapPoint.Value;  // Snap to the nearest key point
                MousePosition = currentMousePos;

                drawPoint(snapPoint, g);
            }
        }

        // Draw all shapes (lines, circles, etc.)
        foreach (var shape in shapes)
        {
            if (shape is Line line)
            {
                Pen linePen = new Pen(line.Color, 2);
                Point p1 = new Point(centerPoint.X + line.Start.X, centerPoint.Y - line.Start.Y);
                Point p2 = new Point(centerPoint.X + line.End.X, centerPoint.Y - line.End.Y);
                g.DrawLine(linePen, p1, p2);
            }
            else if (shape is Circle circle)
            {
                Pen circlePen = new Pen(circle.Color, 2);
                Point center = new Point(centerPoint.X + circle.Center.X, centerPoint.Y - circle.Center.Y);
                g.DrawEllipse(circlePen, center.X - circle.Radius, center.Y - circle.Radius, 2 * circle.Radius, 2 * circle.Radius);
            }
        }

        if (tempStartPoint.HasValue)
        {
            Pen previewPen = new Pen(selectedColor, 1); // Use the selected color for preview line
            Point currentPos = GetRelativeCoordinates(this.PointToClient(Cursor.Position));
            // Preview line if the Line tool is selected
            if (selectedMouse == "Line")
            {
                // Draw the preview line from the start point to the current mouse position
                Point drawStart = new Point(tempStartPoint.Value.X + centerPoint.X, centerPoint.Y - tempStartPoint.Value.Y);
                Point drawEnd = new Point(currentPos.X + centerPoint.X, centerPoint.Y - currentPos.Y);

                g.DrawLine(previewPen, drawStart, drawEnd);
            }
            // Preview circle if the Circle tool is selected
            else if (selectedMouse == "Circle")
            {
                // Calculate the radius as the distance from tempStartPoint to the current mouse position
                float radius = Distance(tempStartPoint.Value, currentPos);

                // Draw the preview circle from tempStartPoint to current mouse position
                Point center = new Point(tempStartPoint.Value.X + centerPoint.X, centerPoint.Y - tempStartPoint.Value.Y);
                g.DrawEllipse(previewPen, center.X - radius, center.Y - radius, 2 * radius, 2 * radius);
            }
        }
    }

    private void Canvas_MouseDown(object sender, MouseEventArgs e)
    {
        Point clickedPoint = GetRelativeCoordinates(e.Location);

        switch (selectedMouse)
        {
            case "Select":
                Cursor = Cursors.Default;
                SelectElement(e.Location);
                break;

            case "Move":
                Cursor = Cursors.SizeAll;
                isDragging = true;
                break;

            case "Line":
                Cursor = Cursors.Cross;  // or Cursors.UpArrow
                if (tempStartPoint == null)
                {
                    // First click - save start point
                    tempStartPoint = clickedPoint;
                    OnTempStartPointChanged.Invoke(tempStartPoint);
                }
                else
                {
                    Line line = new Line
                    {
                        Start = tempStartPoint.Value,
                        End = clickedPoint,
                        Color = selectedColor,
                    };
                    // Second click - save line and reset
                    shapes.Add(line);
                    tempStartPoint = null;
                    OnTempStartPointChanged.Invoke(tempStartPoint);
                    this.Invalidate(); // Trigger redraw
                }
                break;

            case "Circle":
                Cursor = Cursors.Cross;
                if (tempStartPoint == null)
                {
                    // First click - save center point (using tempStartPoint)
                    tempStartPoint = clickedPoint;
                    OnTempStartPointChanged.Invoke(tempStartPoint);
                }
                else
                {
                    // Second click - calculate radius and create the circle
                    var radius = Distance(tempStartPoint.Value, clickedPoint); // Calculate radius
                    Circle newCircle = new Circle
                    {
                        Center = tempStartPoint.Value, // Center is the start point
                        Radius = radius,
                        Color = selectedColor // Assign the selected color
                    };
                    shapes.Add(newCircle);  // Add the new circle to the shapes list
                    tempStartPoint = null;  // Reset the center point
                    OnTempStartPointChanged.Invoke(tempStartPoint);
                    this.Invalidate(); // Trigger redraw
                }
                break;
            case "Eraser":
                Cursor = Cursors.No;  // or Cursors.Cross for eraser mode
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
        else if (tempStartPoint != null)
        {
            Cursor = Cursors.Cross;
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
            centerPoint.X += e.X - lastMousePosition.X;
            centerPoint.Y += e.Y - lastMousePosition.Y;
            lastMousePosition = e.Location;

        }
        else if (isErasing)
        {
            TryEraseAt(e.Location, true); // Continuous erasing
        }
        this.Invalidate(); // Redraw canvas
    }

    private Point GetRelativeCoordinates(Point location)
    {
        return new Point(location.X - centerPoint.X, centerPoint.Y - location.Y);
    }

    private void TryEraseAt(Point mousePos, bool continuous = false)
    {
        Point relativePos = GetRelativeCoordinates(mousePos);

        // Loop through all shapes in the list to check if they are close enough to be erased
        for (int i = shapes.Count - 1; i >= 0; i--)
        {
            var shape = shapes[i];

            if (shape is Line line)
            {
                if (IsPointNearLine(relativePos, line.Start, line.End))
                {
                    shapes.RemoveAt(i); // Remove the line if it's near the point
                    Invalidate(); // Trigger a redraw
                    if (!continuous)
                    {
                        break; // Stop after erasing one shape (if continuous is false)
                    }
                }
            }
            else if (shape is Circle circle)
            {
                if (IsPointNearCircle(relativePos, circle.Center, circle.Radius))
                {
                    shapes.RemoveAt(i); // Remove the circle if it's near the point
                    Invalidate(); // Trigger a redraw
                    if (!continuous)
                    {
                        break; // Stop after erasing one shape (if continuous is false)
                    }
                }
            }
        }
    }

    private bool IsPointNearLine(Point p, Point a, Point b, int threshold = 8)
    {
        float distance = DistanceFromPointToLine(p, a, b);
        return distance <= threshold;
    }

    private bool IsPointNearCircle(Point p, Point center, float radius, int threshold = 8)
    {
        // Calculate the distance from the point to the circumference of the circle
        float distance = DistanceFromPointToCircle(p, center, radius);
        return distance <= threshold;  // Check if the distance is within the threshold
    }

    private bool IsPointNearPoint(Point p1, Point p2, float threshold = 8)
    {
        return Distance(p1, p2) <= threshold;  // Return true if the distance is within the threshold
    }

    private float Distance(Point p1, Point p2)
    {
        return (float)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));  // Euclidean distance
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

    private float DistanceFromPointToCircle(Point p, Point center, float radius)
    {
        float distanceToCenter = Distance(p, center);  // Distance from point to the center of the circle
        return Math.Abs(distanceToCenter - radius);   // Return the absolute difference from the radius
    }

    private void SelectElement(Point mousePos)
    {
        Point relativePos = GetRelativeCoordinates(mousePos);

        // Loop through all shapes and check proximity with index tracking
        for (int i = 0; i < shapes.Count; i++)
        {
            var shape = shapes[i];

            if (shape is Line line && IsPointNearLine(relativePos, line.Start, line.End))
            {
                selectedIndex = i;
                ThrowInformationToSidePanel(line);
                Invalidate();
                return;
            }
            else if (shape is Circle circle && IsPointNearCircle(relativePos, circle.Center, circle.Radius))
            {
                selectedIndex = i;
                ThrowInformationToSidePanel(circle);
                Invalidate();
                return;
            }
        }
    }

    private void ThrowInformationToSidePanel(IShape selectedShape)
    {
        SidePanelData panelData = new SidePanelData
        {
            Color = selectedShape.Color.Name // No need to cast or check type
        };

        selectedShape.UpdateSidePanelData(panelData); // Update data specific to the shape

        // Trigger the event to update the side panel with editable fields
        OnUpdateSidePanel?.Invoke(panelData);
    }

    public void cancelLine()
    {
        if (tempStartPoint.HasValue)
        {
            // Reset drawing state
            tempStartPoint = null;
            OnTempStartPointChanged.Invoke(tempStartPoint);
            this.Invalidate(); // Trigger a redraw
        }
    }

    public void ToggleGrid()
    {
        showGrid = !showGrid;  // Toggle the boolean value
    }

    public void ToggleSnap()
    {
        snapEnabled = !snapEnabled;  // Toggle the boolean value
    }

    public void CenterCanvas()
    {
        // Start from the current centerPoint and animate toward the canvas center
        Point targetCenter = new Point(this.Width / 2, this.Height / 2);

        // Animating smoothly by moving towards the target center
        var animationTimer = new System.Windows.Forms.Timer();
        animationTimer.Interval = 10; // Set to adjust the speed of the movement
        animationTimer.Tick += (sender, e) =>
        {
            // Calculate delta (how much to move in each step)
            int deltaX = (targetCenter.X - centerPoint.X) / 10;
            int deltaY = (targetCenter.Y - centerPoint.Y) / 10;

            // Stop the animation when the center is close enough to the target
            if (Math.Abs(targetCenter.X - centerPoint.X) < 10 && Math.Abs(targetCenter.Y - centerPoint.Y) < 10)  // Tolerance of 2 pixels
            {
                centerPoint = targetCenter;  // Set centerPoint directly to the target to avoid overshooting
                animationTimer.Stop(); // Stop the animation timer
            }
            else
            {
                // Move towards the target
                centerPoint = new Point(centerPoint.X + deltaX, centerPoint.Y + deltaY);
                Invalidate(); // Redraw canvas to reflect the new position
            }
        };
        animationTimer.Start();  // Start the animation
    }

    private Point? FindSnapPoint(Point mousePos)
    {
        // Search all shapes for snap points
        foreach (var shape in shapes)
        {
            if (shape is Line line)
            {
                // Find key points for the line (Start, Midpoint, End)
                Point start = line.Start;
                Point end = line.End;
                Point midpoint = new Point((start.X + end.X) / 2, (start.Y + end.Y) / 2);

                if (IsPointNearPoint(mousePos, start)) return start;
                if (IsPointNearPoint(mousePos, midpoint)) return midpoint;
                if (IsPointNearPoint(mousePos, end)) return end;
            }
            else if (shape is Circle circle)
            {
                // Find key points for the circle (Center, Cardinal points)
                Point center = circle.Center;
                Point top = new Point(center.X, (int)(center.Y - circle.Radius));
                Point bottom = new Point(center.X, (int)(center.Y + circle.Radius));
                Point left = new Point((int)(center.X - circle.Radius), center.Y);
                Point right = new Point((int)(center.X + circle.Radius), center.Y);

                if (IsPointNearPoint(mousePos, center)) return center;
                if (IsPointNearPoint(mousePos, top)) return top;
                if (IsPointNearPoint(mousePos, bottom)) return bottom;
                if (IsPointNearPoint(mousePos, left)) return left;
                if (IsPointNearPoint(mousePos, right)) return right;
            }
        }

        return null;  // No snap point found
    }

    private void drawPoint(Point? point, Graphics g)
    {
        if (point.HasValue)
        {
            Point p = point.Value;

            Point screenPoint = new Point(
                centerPoint.X + p.X,
                centerPoint.Y - p.Y
            );

            int radius = 4;
            g.FillEllipse(Brushes.Red, screenPoint.X - radius, screenPoint.Y - radius, radius * 2, radius * 2);
        }
    }

}

