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

        public Form1()
        {
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
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

            //Footer
            Panel footerPanel = new Panel
            {
                Height = 30,
                Dock = DockStyle.Fill, 
                BackColor = Color.LightGray
            }; 

            Label footerLabel = new Label
            {
                Text = $"{rm.GetString("Coordinates")} - X: 0 Y: 0",
                AutoSize = true,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            footerPanel.Controls.Add(footerLabel);

            mainLayout.Controls.Add(footerLabel, 0, 3);


            //SidePanel

            Panel sidePanel = new Panel
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

            DrawingCanvas canvas = new DrawingCanvas
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black
            };

            canvas.MouseMove += (sender, e) => 
            {
                footerLabel.Text = $"{rm.GetString("Coordinates")} - X: {e.Location.X - canvas.centerPoint.X} Y: {canvas.centerPoint.Y - e.Location.Y}";
            };

            canvas.OnUpdateSidePanel += (text) => {
                sidePanel.Controls.Clear();
                sidePanel.Controls.Add(new Label { Text = text, AutoSize = true });
            };

            contentLayout.Controls.Add(canvas, 0, 0);
            contentLayout.Controls.Add(sidePanel, 1, 0);
            
            //Flow Layout (Upper buttons)
            FlowLayoutPanel menuPanel = new FlowLayoutPanel {
                Height = 50,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
            };

            // Buttons
            Size BUTTON_SIZE = new Size(15,15);
                //Select Button
            Button selectButton = new Button
            {
                Size = BUTTON_SIZE,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Red
            };

            selectButton.Click += (sender, e) => {
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

            moveButton.Click += (sender, e) => {
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

            lineButton.Click += (sender, e) => {
                changeSelectedMouse(canvas, "Line");
                canvas.Cursor = Cursors.UpArrow;
            };

            menuPanel.Controls.Add(lineButton);

                //Eraser Button
            Button eraserButton = new Button
            {
                Size = BUTTON_SIZE,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Purple
            };

            eraserButton.Click += (sender, e) => {
                changeSelectedMouse(canvas, "Eraser");
                canvas.Cursor = Cursors.UpArrow;
            };

            menuPanel.Controls.Add(eraserButton);

            //Temp iteration to visualize buttons location
            for (int i = 0; i < 3; i++)
            {
                Button btn = new Button();
                btn.Size = new Size(10, 10); 
                btn.FlatStyle = FlatStyle.Flat;
                btn.BackgroundImageLayout = ImageLayout.Zoom;
                menuPanel.Controls.Add(btn);
            }

            ComboBox colorSelector = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 100
            };

            colorSelector.Items.AddRange(new object[] { "Black", "Red", "Green", "Blue", "Yellow" });
            colorSelector.SelectedIndex = 0; // Default color
            colorSelector.SelectedIndexChanged  += (sender, e) => {
                string selectedColorName = colorSelector.SelectedItem.ToString();
                canvas.selectedColor = Color.FromName(selectedColorName);
            };

            menuPanel.Controls.Add(colorSelector);

            mainLayout.Controls.Add(menuPanel, 0, 1);

        }

        void SetLanguage(string language)
        {
            DialogResult dialogResult = MessageBox.Show(rm.GetString("ChangeLanguageText"), rm.GetString("YesNoMenuTitle"), MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes) {
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

        void changeSelectedMouse(DrawingCanvas canvas, String selectedMouse) {
            canvas.selectedMouse = selectedMouse;
        }
    }
}
