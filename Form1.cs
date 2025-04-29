using System;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace _2dboard
{
    public partial class Form1 : Form
    {
        ResourceManager rm;
        AppSettings settings = AppSettings.LoadSettings();

        DrawingCanvas canvas;
        Panel sidePanel;
        string[] colors = new String[] { "White", "Red", "Green", "Blue", "Yellow" };

        public Form1()
        {
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            Icon = new Icon("Resources/TEMP.ico");  // Set the form icon
            //Creating Language Handler
            CultureInfo.CurrentUICulture = new CultureInfo(settings.Language);
            rm = new ResourceManager("_2dboard.Resources.Strings", typeof(Form1).Assembly);

            //Creating Table Menu Layout
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            SetupTableLayout(mainLayout);

            //Default base values
            Text = rm.GetString("AppTitle");
            Size = new Size(800, 500);

            //Menu and ToolStrip
            MenuStrip menuStrip = new MenuStrip();

            //Options Menu
            ToolStripMenuItem optionsMenu = new ToolStripMenuItem(rm.GetString("MenuOptions"));
            optionsMenu.DropDownItems.Add("Settings", null, (sender, e) => MessageBox.Show("Opening Settings..."));
            optionsMenu.DropDownItems.Add("Help", null, (sender, e) => MessageBox.Show("Showing Help..."));
            optionsMenu.DropDownItems.Add("Exit", null, (sender, e) => this.Close());
            menuStrip.Items.Add(optionsMenu);

            //Language Menu
            ToolStripMenuItem languageMenu = new ToolStripMenuItem(rm.GetString("MenuLanguage"));
            languageMenu.DropDownItems.Add("Español", null, (sender, e) => SetLanguage("es"));
            languageMenu.DropDownItems.Add("English", null, (sender, e) => SetLanguage("en"));
            menuStrip.Items.Add(languageMenu);

            MainMenuStrip = menuStrip;

            mainLayout.Controls.Add(menuStrip, 0, 0);

            // Footer Panel where the buttons will go
            Panel footerPanel = new Panel
            {
                Height = 30,
                Dock = DockStyle.Fill,
                BackColor = Color.LightGray
            };

            // Footer Label
            Label footerLabel = new Label
            {
                Text = $"{rm.GetString("Coordinates")} - X: 0 Y: 0",
                AutoSize = true,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            footerPanel.Controls.Add(footerLabel);

            // Add Corner Buttons (Grid On/Off and Center Canvas) to the right corner
            FlowLayoutPanel footerButtonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.LeftToRight,
                Width = 150  // Adjust the width for button space
            };

            // Grid Button
            Button gridButton = new Button
            {
                Size = new Size(25, 25),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Green,
                Text = "Grid",
                Font = new Font("Arial", 6)  // Small font size for the button
            };
            gridButton.Click += (sender, e) =>
            {
                canvas.ToggleGrid(); // Toggle grid visibility
                canvas.Invalidate(); // Redraw the canvas with or without grid
            };
            footerButtonPanel.Controls.Add(gridButton);

            // Center Canvas Button
            Button centerButton = new Button
            {
                Size = new Size(25, 25),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Blue,
                Text = "Center",
                Font = new Font("Arial", 6)  // Small font size for the button
            };
            centerButton.Click += (sender, e) =>
            {
                canvas.CenterCanvas(); // Animate the canvas to the center
            };
            footerButtonPanel.Controls.Add(centerButton);

            // Add the footer button panel to the footer
            footerPanel.Controls.Add(footerButtonPanel);
            mainLayout.Controls.Add(footerPanel, 0, 3);


            //SidePanel

            sidePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.LightGray
            };

            // Drawing Canvas

            Panel contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            mainLayout.Controls.Add(contentPanel, 0, 2);

            TableLayoutPanel contentLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2
            };
            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70)); // Canvas (Left)
            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30)); // Sidebar (Right)

            contentPanel.Controls.Add(contentLayout);

            canvas = new DrawingCanvas
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black
            };

            canvas.MouseMove += (sender, e) =>
            {
                footerLabel.Text = $"{rm.GetString("Coordinates")} - X: {e.Location.X - canvas.centerPoint.X} Y: {canvas.centerPoint.Y - e.Location.Y}";
            };

            canvas.OnUpdateSidePanel += UpdateSidePanel;

            contentLayout.Controls.Add(canvas, 0, 0);
            contentLayout.Controls.Add(sidePanel, 1, 0);

            //Flow Layout (Upper buttons)
            FlowLayoutPanel menuPanel = new FlowLayoutPanel
            {
                Height = 50,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
            };

            // Buttons
            Size BUTTON_SIZE = new Size(25, 25);
            //Select Button
            Button selectButton = new Button
            {
                Size = BUTTON_SIZE,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Red,
                Image = Image.FromFile("Resources/TEMP.ico"), // Set the custom icon
            };

            selectButton.Click += (sender, e) =>
            {
                changeSelectedMouse(canvas, "Select");
                canvas.Cursor = Cursors.Default;
            };

            menuPanel.Controls.Add(selectButton);

            //Move Button
            Button moveButton = new Button
            {
                Size = BUTTON_SIZE,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Black
            };

            moveButton.Click += (sender, e) =>
            {
                changeSelectedMouse(canvas, "Move");
                canvas.Cursor = Cursors.Hand;
            };

            menuPanel.Controls.Add(moveButton);

            //Line Button
            Button lineButton = new Button
            {
                Size = BUTTON_SIZE,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Blue
            };

            lineButton.Click += (sender, e) =>
            {
                changeSelectedMouse(canvas, "Line");
            };

            menuPanel.Controls.Add(lineButton);

            //Eraser Button
            Button eraserButton = new Button
            {
                Size = BUTTON_SIZE,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Purple
            };

            eraserButton.Click += (sender, e) =>
            {
                changeSelectedMouse(canvas, "Eraser");
            };

            menuPanel.Controls.Add(eraserButton);

            // Circle Button
            Button circleButton = new Button
            {
                Size = BUTTON_SIZE,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Orange  // Choose a color for the circle button
            };

            circleButton.Click += (sender, e) =>
            {
                changeSelectedMouse(canvas, "Circle");
                canvas.Cursor = Cursors.Cross;  // Set cursor to crosshair for drawing circles
            };

            menuPanel.Controls.Add(circleButton);

            //Color Selector

            ComboBox colorSelector = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 100
            };

            colorSelector.Items.AddRange(colors);
            colorSelector.SelectedIndex = 0; // Default color
            colorSelector.SelectedIndexChanged += (sender, e) =>
            {
                string selectedColorName = colorSelector.SelectedItem.ToString();
                canvas.selectedColor = Color.FromName(selectedColorName);
            };

            menuPanel.Controls.Add(colorSelector);

            mainLayout.Controls.Add(menuPanel, 0, 1);
        }

        void SetLanguage(string language)
        {
            DialogResult dialogResult = MessageBox.Show(rm.GetString("ChangeLanguageText"), rm.GetString("YesNoMenuTitle"), MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                settings.Language = language;
                settings.SaveSettings();
                Application.Restart();
                Environment.Exit(0);
            }
        }

        void SetupTableLayout(TableLayoutPanel mainLayout)
        {
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.RowCount = 4;
            mainLayout.ColumnCount = 1;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // MenuStrip
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Buttons Menu
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Main Content (Expands)
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Footer

            Controls.Add(mainLayout);
        }

        void changeSelectedMouse(DrawingCanvas canvas, String selectedMouse)
        {
            canvas.selectedMouse = selectedMouse;
        }

        // Event handler to update the side panel
        private void UpdateSidePanel(SidePanelData panelData)
        {
            // Clear previous controls
            sidePanel.Controls.Clear();

            // Create and add editable fields to the side panel
            // Length label and textbox
            Label lengthLabel = new Label
            {
                Text = "Length:",
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft
            };
            TextBox lengthTextBox = new TextBox
            {
                Text = panelData.Length,
                Dock = DockStyle.Top
            };

            // Point 1 X label and textbox
            Label point1XLabel = new Label
            {
                Text = "Point 1 X:",
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft
            };
            TextBox point1XTextBox = new TextBox
            {
                Text = panelData.Point1X,
                Dock = DockStyle.Top
            };

            Label point1YLabel = new Label
            {
                Text = "Point 1 Y:",
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft
            };
            TextBox point1YTextBox = new TextBox
            {
                Text = panelData.Point1Y,
                Dock = DockStyle.Top
            };

            // Point 2 X label and textbox
            Label point2XLabel = new Label
            {
                Text = "Point 2 X:",
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft
            };
            TextBox point2XTextBox = new TextBox
            {
                Text = panelData.Point2X,
                Dock = DockStyle.Top
            };

            // Point 2 Y label and textbox
            Label point2YLabel = new Label
            {
                Text = "Point 2 Y:",
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft
            };
            TextBox point2YTextBox = new TextBox
            {
                Text = panelData.Point2Y,
                Dock = DockStyle.Top
            };

            // Color label and textbox
            Label colorLabel = new Label
            {
                Text = "Color:",
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Create ComboBox for color selection
            ComboBox colorComboBox = new ComboBox
            {
                Dock = DockStyle.Top,
                DataSource = colors, // Bind to the color array
                Text = panelData.Color // Set the initial selected color
            };

            // Add event handler for color selection
            colorComboBox.SelectedIndexChanged += (sender, e) =>
            {
                // Update the line color when a new color is selected
                var selectedColor = Color.FromName(colorComboBox.SelectedItem.ToString());
                // Update the line's color with the selected color (you can add your logic for this)
                UpdateShapeColor(selectedColor); // Method that will handle the update (to be implemented)
                canvas.Invalidate(); // Trigger a redraw
            };

            // Optionally, add event handlers to update line data when values are changed
            // For example, update `drawingCanvas`'s line data when the user edits the fields
            // Add labels and textboxes to the panel in the correct order
            sidePanel.Controls.Add(lengthTextBox);
            sidePanel.Controls.Add(lengthLabel);

            sidePanel.Controls.Add(point2YTextBox);
            sidePanel.Controls.Add(point2YLabel);

            sidePanel.Controls.Add(point2XTextBox);
            sidePanel.Controls.Add(point2XLabel);

            sidePanel.Controls.Add(point1YTextBox);
            sidePanel.Controls.Add(point1YLabel);

            sidePanel.Controls.Add(point1XTextBox);
            sidePanel.Controls.Add(point1XLabel);

            sidePanel.Controls.Add(colorComboBox);
            sidePanel.Controls.Add(colorLabel);
        }

        private void UpdateShapeColor(Color newColor)
        {
            var shapes = canvas.shapes;  // List of all shapes
            int selectedShapeIndex = canvas.selectedLineIndex;  // Index of the selected shape

            if (selectedShapeIndex >= 0 && selectedShapeIndex < shapes.Count)
            {
                var selectedShape = shapes[selectedShapeIndex];

                // Check the type of shape and update the color
                if (selectedShape is Line line)
                {
                    line.Color = newColor;  // Update color of the Line
                }
                else if (selectedShape is Circle circle)
                {
                    circle.Color = newColor;  // Update color of the Circle
                }

                //Invalidate();  // Trigger a redraw to reflect the color change
            }
        }


        // Override ProcessCmdKey to capture Esc key press
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                // Call cancelLine in DrawingCanvas to cancel the line drawing
                canvas.cancelLine();
                return true; // Return true to indicate the key press was handled
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
