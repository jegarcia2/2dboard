class DrawingCanvas : Panel
{
    public Point centerPoint;
    private bool isDragging = false;
    private Point lastMousePosition;
    public String selectedMouse = "Select";
    public new Point MousePosition = new Point(0,0);
    private Point? tempStartPoint = null;
    private List<(Point Start, Point End)> lines = new List<(Point, Point)>();

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
            Point p1 = new Point(centerPoint.X + line.Start.X, centerPoint.Y - line.Start.Y);
            Point p2 = new Point(centerPoint.X + line.End.X, centerPoint.Y - line.End.Y);
            g.DrawLine(Pens.White, p1, p2);
        }

        // Optional: draw preview line from tempStartPoint to current mouse
        if (selectedMouse == "Line" && tempStartPoint.HasValue)
        {
            Point currentPos = GetRelativeCoordinates(this.PointToClient(Cursor.Position));
            g.DrawLine(Pens.Gray, tempStartPoint.Value, currentPos);
        }
    }
    private void Canvas_MouseDown( object sender, MouseEventArgs e )
    {
        Point clickedPoint = GetRelativeCoordinates(e.Location);

        if (selectedMouse == "Move") {
            isDragging = true;
        } else if ( selectedMouse == "Line" ) {
            if (tempStartPoint == null)
            {
                // First click - save start point
                tempStartPoint = clickedPoint;
                drawLine();
            }
            else
            {
                // Second click - save line and reset
                lines.Add((tempStartPoint.Value, clickedPoint));
                tempStartPoint = null;
                this.Invalidate(); // Trigger redraw
            }
        }
        lastMousePosition = e.Location;
    }

    private void Canvas_MouseUp( object sender, MouseEventArgs e )
    {
        isDragging = false;
        if (selectedMouse == "Move") {
            Cursor = Cursors.Hand;
        } else {
            Cursor = Cursors.Default;
        }
    }

    private void Canvas_MouseMove( object sender, MouseEventArgs e )
    {
        if ( isDragging )
        {
            Cursor = Cursors.SizeAll;
            // Move the center point
            centerPoint.X += e.X - lastMousePosition.X;
            centerPoint.Y += e.Y - lastMousePosition.Y;
            lastMousePosition = e.Location;

            this.Invalidate(); // Redraw canvas
        }

    }

    private Point GetRelativeCoordinates(Point location)
    {
        return new Point(location.X - centerPoint.X, -(location.Y - centerPoint.Y));
    }

    private void drawLine() {

    }
}
