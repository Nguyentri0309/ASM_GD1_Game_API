using System.Drawing;
using System.Windows.Forms;

namespace GameClient_WinForms.Forms;

// Mau nen toi, nhan vat RPG fantasy. Dung chung cho ca 3 form
internal static class Theme
{
    public static readonly Color Bg = Color.FromArgb(10, 12, 24);
    public static readonly Color Panel = Color.FromArgb(20, 23, 42);
    public static readonly Color Field = Color.FromArgb(34, 38, 64);
    public static readonly Color Accent = Color.FromArgb(96, 76, 170);
    public static readonly Color AccentLight = Color.FromArgb(186, 160, 255);
    public static readonly Color Gold = Color.FromArgb(240, 190, 100);
    public static readonly Color Text = Color.FromArgb(235, 235, 250);
    public static readonly Color Muted = Color.FromArgb(150, 155, 185);
    public static readonly Color Success = Color.FromArgb(90, 200, 140);
    public static readonly Color Danger = Color.FromArgb(235, 110, 120);

    public static Font Font(float size, bool bold = false)
        => new("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular);

    public static void StyleForm(Form form, int width, int height)
    {
        form.BackColor = Bg;
        form.ForeColor = Text;
        form.Font = Font(10);
        form.StartPosition = FormStartPosition.CenterScreen;
        form.FormBorderStyle = FormBorderStyle.FixedDialog;
        form.MaximizeBox = false;
        form.ClientSize = new Size(width, height);
    }

    public static Label Title(Control parent, string text, int x, int y, int width, float size = 26)
    {
        var label = new Label
        {
            Text = text,
            Location = new Point(x, y),
            Size = new Size(width, (int)(size * 1.6)),
            Font = Font(size, true),
            ForeColor = Gold,
            BackColor = Color.Transparent
        };
        parent.Controls.Add(label);
        return label;
    }

    public static Label Emblem(Control parent, int x, int y)
    {
        var label = new Label
        {
            Text = "⚔",
            Location = new Point(x, y),
            Size = new Size(64, 64),
            Font = Font(40, true),
            ForeColor = AccentLight,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleCenter
        };
        parent.Controls.Add(label);
        return label;
    }

    public static Label Label(Control parent, string text, int x, int y, int width)
    {
        var label = new Label
        {
            Text = text,
            Location = new Point(x, y),
            Size = new Size(width, 22),
            Font = Font(10),
            ForeColor = Muted,
            BackColor = Color.Transparent
        };
        parent.Controls.Add(label);
        return label;
    }

    public static TextBox TextBox(Control parent, int x, int y, int width, bool password = false)
    {
        var box = new TextBox
        {
            Location = new Point(x, y),
            Size = new Size(width, 30),
            BackColor = Field,
            ForeColor = Text,
            BorderStyle = BorderStyle.FixedSingle,
            Font = Font(10)
        };
        if (password)
        {
            box.UseSystemPasswordChar = true;
        }
        parent.Controls.Add(box);
        return box;
    }

    public static ComboBox ComboBox(Control parent, int x, int y, int width, string[] items)
    {
        var box = new ComboBox
        {
            Location = new Point(x, y),
            Size = new Size(width, 30),
            BackColor = Field,
            ForeColor = Text,
            FlatStyle = FlatStyle.Flat,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = Font(10)
        };
        box.Items.AddRange(items);
        parent.Controls.Add(box);
        return box;
    }

    public static NumericUpDown Numeric(Control parent, int x, int y, int width, int min, int max, int value)
    {
        var num = new NumericUpDown
        {
            Location = new Point(x, y),
            Size = new Size(width, 30),
            BackColor = Field,
            ForeColor = Text,
            Minimum = min,
            Maximum = max,
            Value = value,
            Font = Font(10)
        };
        parent.Controls.Add(num);
        return num;
    }

    public static Button Button(Control parent, string text, int x, int y, int width, int height = 40, bool primary = true)
    {
        var button = new Button
        {
            Text = text,
            Location = new Point(x, y),
            Size = new Size(width, height),
            FlatStyle = FlatStyle.Flat,
            BackColor = primary ? Accent : Field,
            ForeColor = Color.White,
            Font = Font(11, true),
            Cursor = Cursors.Hand
        };
        button.FlatAppearance.BorderSize = 0;
        Color normal = button.BackColor;
        button.MouseEnter += (_, _) => button.BackColor = primary ? AccentLight : Accent;
        button.MouseLeave += (_, _) => button.BackColor = normal;
        parent.Controls.Add(button);
        return button;
    }

    public static Panel Section(Control parent, string title, int x, int y, int width, int height)
    {
        var panel = new Panel
        {
            Location = new Point(x, y),
            Size = new Size(width, height),
            BackColor = Panel
        };
        Title(panel, title, 14, 10, width - 28, 12);
        var line = new Panel
        {
            Location = new Point(14, 34),
            Size = new Size(width - 28, 2),
            BackColor = Accent
        };
        panel.Controls.Add(line);
        parent.Controls.Add(panel);
        return panel;
    }

    public static void StyleGrid(DataGridView grid)
    {
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersDefaultCellStyle.BackColor = Accent;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.Font = Font(10, true);
        grid.ColumnHeadersHeight = 30;
        grid.DefaultCellStyle.BackColor = Panel;
        grid.DefaultCellStyle.ForeColor = Text;
        grid.DefaultCellStyle.SelectionBackColor = Accent;
        grid.DefaultCellStyle.SelectionForeColor = Color.White;
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(16, 19, 36);
        grid.RowTemplate.Height = 28;
        grid.RowHeadersVisible = false;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
    }
}
