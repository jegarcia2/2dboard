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
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            SetupTableLayout(mainLayout);

            //Creating Language Handler
            CultureInfo.CurrentUICulture = new CultureInfo(settings.Language);
            rm = new ResourceManager("_2dboard.Resources.Strings", typeof(Form1).Assembly);

            Text = rm.GetString("AppTitle");
            Size = new Size(300, 200);

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

            //Flow Layout 

            FlowLayoutPanel menuPanel = new FlowLayoutPanel {
                //Dock = DockStyle.Top,
                Height = 50,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
            };

            for (int i = 0; i < 5; i++)
            {
                Button btn = new Button();
                btn.Size = new Size(10, 10); 
                btn.FlatStyle = FlatStyle.Flat;
                //btn.BackgroundImage = Image.FromFile($"icon{i}.png"); // Asegúrate de tener imágenes
                btn.BackgroundImageLayout = ImageLayout.Zoom;
                menuPanel.Controls.Add(btn); // Agrega el botón al panel
            }

            mainLayout.Controls.Add(menuPanel, 0, 1);


            //Centered Panel For Misc
            Panel contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            mainLayout.Controls.Add(contentPanel, 0, 2);

            Label label = new Label
            {
                Text = rm.GetString("WelcomeText"),
                AutoSize = true,
            };

            Button boton = new Button
            {
                Text = "Click me!",
            };

            Click += (sender, e) => MessageBox.Show(rm.GetString("HelloWorld"));

            contentPanel.Controls.Add(label);
            contentPanel.Controls.Add(boton);
        }

        void SetLanguage(String? language = null)
        {
            if (language != null) {
                settings.Language = language;
                settings.SaveSettings();
            } else {
                return;
            }

            Application.Restart();  
            Environment.Exit(0);  
        }
    
        void SetupTableLayout(TableLayoutPanel mainLayout) {
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.RowCount = 3; // MenuStrip, Button Panel, Main Content
            mainLayout.ColumnCount = 1;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // For MenuStrip
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // For Buttons Menu
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Remaining space for content
            Controls.Add(mainLayout);
        }
    
    }
}
