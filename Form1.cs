using System;
using System.Globalization;
using System.Resources;
using System.Windows.Forms;

namespace _2dboard
{
    public partial class Form1 : Form
    {
        ResourceManager rm;
        AppSettings settings = AppSettings.LoadSettings();
        private bool isDrawing = false;
        DrawingCanvas canvas;
        Panel sidePanel;
        string[] colors = new String[] { "White", "Red", "Green", "Blue", "Yellow" };
        Size BUTTON_SIZE = new Size(25, 25);
        public Form1()
        {
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            
            // Load form icon with error handling
            try
            {
                Icon = new Icon("Resources/icons/TEMP.ico");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Warning: Could not load form icon. " + ex.Message, "Icon Error");
            }

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

            //Icons List
            // Create an ImageList with the desired button size
            ImageList iconList = new ImageList();
            iconList.ImageSize = new Size(20, 20);  // Resize all icons to 25 x 25

            // Load the icons with error handling
            string[] iconPaths = new string[]
            {
                "Resources/icons/select.ico",
                "Resources/icons/move.ico",
                "Resources/icons/line.ico",
                "Resources/icons/circle.ico",
                "Resources/icons/erase.ico",
                "Resources/icons/grid-1.ico",
                "Resources/icons/center-canvas.ico",
                "Resources/icons/snap.ico"
            };

            foreach (var iconPath in iconPaths)
            {
                try
                {
                    iconList.Images.Add(Image.FromFile(iconPath));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Warning: Could not load icon '{iconPath}'. {ex.Message}", "Icon Error");
                    // Add a placeholder image instead of crashing
                    Bitmap placeholder = new Bitmap(20, 20);
                    iconList.Images.Add(placeholder);
                }
            }

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
                Text = $"{rm.GetString("Coordinates")}  X: 0 Y: 0",
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
                Size = BUTTON_SIZE,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                Image = iconList.Images[5],
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
                Size = BUTTON_SIZE,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                Image = iconList.Images[6],
            };

            centerButton.Click += (sender, e) =>
            {
                canvas.CenterCanvas(); // Animate the canvas to the center
            };

            footerButtonPanel.Controls.Add(centerButton);

            // Add button for toggling shape snapping
            Button snapToShapeButton = new Button
            {
                Size = BUTTON_SIZE,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                Image = iconList.Images[7],
            };

            snapToShapeButton.Click += (sender, e) =>
            {
                canvas.ToggleSnap();
            };

            // Add to footer
            footerButtonPanel.Controls.Add(snapToShapeButton);

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

            canvas.OnTempStartPointChanged += (tempStartPoint) =>
            {
                isDrawing = tempStartPoint.HasValue;
            };

            canvas.MouseMove += (sender, e) =>
            {
                if (isDrawing)
                {
                    footerLabel.Text = $"{rm.GetString("DrawingMode")} {rm.GetString("Coordinates")} - X: {e.Location.X - canvas.centerPoint.X} Y: {canvas.centerPoint.Y - e.Location.Y}";

                }
                else
                {
                    footerLabel.Text = $"{rm.GetString("Coordinates")} - X: {e.Location.X - canvas.centerPoint.X} Y: {canvas.centerPoint.Y - e.Location.Y}";
                }
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
            //Select Button

            Button selectButton = new Button
            {
                Size = BUTTON_SIZE,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                Image = iconList.Images[0],
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
                BackColor = Color.White,
                Image = iconList.Images[1],
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
                BackColor = Color.White,
                Image = iconList.Images[2],
            };

            lineButton.Click += (sender, e) =>
            {
                changeSelectedMouse(canvas, "Line");
            };

            menuPanel.Controls.Add(lineButton);

            // Circle Button
            Button circleButton = new Button
            {
                Size = BUTTON_SIZE,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                Image = iconList.Images[3],
            };

            circleButton.Click += (sender, e) =>
            {
                changeSelectedMouse(canvas, "Circle");
                canvas.Cursor = Cursors.Cross;  // Set cursor to crosshair for drawing circles
            };

            menuPanel.Controls.Add(circleButton);

            //Eraser Button
            Button eraserButton = new Button
            {
                Size = BUTTON_SIZE,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                Image = iconList.Images[4],
            };

            eraserButton.Click += (sender, e) =>
            {
                changeSelectedMouse(canvas, "Eraser");
            };

            menuPanel.Controls.Add(eraserButton);

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
                Text = panelData.Length ?? "",
                Dock = DockStyle.Top,
                ReadOnly = true
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
                Text = panelData.Point1X ?? "",
                Dock = DockStyle.Top,
                ReadOnly = true
            };

            Label point1YLabel = new Label
            {
                Text = "Point 1 Y:",
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft
            };
            TextBox point1YTextBox = new TextBox
            {
                Text = panelData.Point1Y ?? "",
                Dock = DockStyle.Top,
                ReadOnly = true
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
                Text = panelData.Point2X ?? "",
                Dock = DockStyle.Top,
                ReadOnly = true
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
                Text = panelData.Point2Y ?? "",
                Dock = DockStyle.Top,
                ReadOnly = true
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
                DataSource = colors,
            };

            // The flag prevents the handler from firing during the build phase.
            // DataSource binding can trigger SelectedIndexChanged with SelectedItem == null
            // when the control's handle is created inside Controls.Add on a visible panel.
            bool colorSetupComplete = false;

            colorComboBox.SelectedIndexChanged += (sender, e) =>
            {
                if (!colorSetupComplete || colorComboBox.SelectedItem == null) return;
                var selectedColor = Color.FromName(colorComboBox.SelectedItem.ToString());
                UpdateShapeColor(selectedColor);
                canvas.Invalidate();
            };

            // With DockStyle.Top, WinForms docks in reverse Z-order (last added = topmost),
            // so the add sequence below is intentionally reversed from the visual top-to-bottom order.
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

            // Set the selection after Controls.Add so the DataSource binding is fully
            // initialized before we apply the shape's current color.
            int colorIndex = Array.IndexOf(colors, panelData.Color);
            colorComboBox.SelectedIndex = (colorIndex >= 0) ? colorIndex : 0;
            colorSetupComplete = true;
        }

        private void UpdateShapeColor(Color newColor)
        {
            var shapes = canvas.shapes;  // List of all shapes
            int selectedShapeIndex = canvas.selectedIndex;  // Index of the selected shape

            if (selectedShapeIndex >= 0 && selectedShapeIndex < shapes.Count)
            {
                var selectedShape = shapes[selectedShapeIndex];

                selectedShape.Color = newColor;  // Update color of the Circle

                canvas.Invalidate();  // Trigger a redraw to reflect the color change
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
