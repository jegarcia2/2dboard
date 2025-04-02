class DraggableCanvas : Panel
{
    public Point centerPoint;
    private bool isDragging = false;
    private Point lastMousePosition;
    public String selectedMouse = "Select";
    public new Point MousePosition = new Point(0,0);

    public DraggableCanvas()
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

        axisPen.Dispose();
    }
    private void Canvas_MouseDown( object sender, MouseEventArgs e )
    {
        if (selectedMouse == "Move") {
            isDragging = true;
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
        //getMouseRelativePoint();
    }

    private void getMouseRelativePoint() {
        Point screenMousePos = this.PointToClient(Cursor.Position); // Get mouse pos relative to the form
        this.MousePosition.X = screenMousePos.X - centerPoint.X;
        this.MousePosition.Y = centerPoint.Y - screenMousePos.Y;
    }
}
