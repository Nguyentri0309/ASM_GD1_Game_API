using System.Windows.Forms;
using GameClient_WinForms.Models;
using GameClient_WinForms.Services;

namespace GameClient_WinForms.Forms;

public class FrmMain : Form
{
    private static readonly string[] Classes = ["Warrior", "Mage", "Archer", "Assassin", "Paladin"];

    private readonly ApiService _api;
    private Label _lblUser = null!;
    private Label _lblStatus = null!;
    private Label _lblCharacter = null!;
    private TextBox _txtNewName = null!;
    private ComboBox _cboNewClass = null!;
    private TextBox _txtId = null!;
    private TextBox _txtName = null!;
    private ComboBox _cboClass = null!;
    private NumericUpDown _numLevel = null!;
    private NumericUpDown _numHp = null!;
    private DataGridView _grid = null!;
    private Label _lblNotify = null!;
    private Button _btnCreate = null!;
    private Button _btnUpdate = null!;

    public FrmMain(ApiService api)
    {
        _api = api;
        BuildUi();
        _ = LoadCharactersAsync();
    }

    private void BuildUi()
    {
        Theme.StyleForm(this, 1160, 720);
        Text = "Game Adventure - Dashboard";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.Sizable;

        var top = new Panel
        {
            Dock = DockStyle.Top,
            Height = 62,
            BackColor = Color.FromArgb(18, 20, 38)
        };
        Controls.Add(top);
        var topLine = new Panel
        {
            Location = new Point(0, 60),
            Size = new Size(1160, 2),
            BackColor = Theme.Accent
        };
        top.Controls.Add(topLine);
        Theme.Emblem(top, 8, -2);
        Theme.Title(top, "GAME ADVENTURE", 78, 12, 240, 20);
        _lblUser = Theme.Label(top, "Người chơi: " + Session.Username, 300, 20, 240);
        _lblStatus = Theme.Label(top, "● API: " + _api.BaseUrl, 550, 20, 420);
        _lblStatus.ForeColor = Theme.Success;
        var btnLogout = Theme.Button(top, "Đăng xuất", 1030, 12, 110, 36, primary: false);
        btnLogout.Click += BtnLogout_Click;

        var info = Theme.Section(this, "THÔNG TIN NHÂN VẬT", 16, 76, 300, 160);
        _lblCharacter = new Label
        {
            Location = new Point(16, 46),
            Size = new Size(270, 100),
            ForeColor = Theme.Text,
            Font = Theme.Font(11),
            BackColor = Color.Transparent,
            Text = "Chưa có nhân vật. Tạo một nhân vật để bắt đầu."
        };
        info.Controls.Add(_lblCharacter);

        var create = Theme.Section(this, "TẠO NHÂN VẬT", 16, 250, 300, 200);
        Theme.Label(create, "Tên nhân vật", 16, 42, 260);
        _txtNewName = Theme.TextBox(create, 16, 66, 268);
        Theme.Label(create, "Class", 16, 104, 260);
        _cboNewClass = Theme.ComboBox(create, 16, 128, 268, Classes);
        _cboNewClass.SelectedIndex = 0;
        _btnCreate = Theme.Button(create, "Tạo nhân vật", 16, 152, 268, 36);
        _btnCreate.Click += BtnCreate_Click;

        var update = Theme.Section(this, "SỬA NHÂN VẬT", 16, 464, 300, 240);
        Theme.Label(update, "Character ID", 16, 40, 140);
        _txtId = Theme.TextBox(update, 16, 62, 120);
        Theme.Label(update, "Tên", 148, 40, 140);
        _txtName = Theme.TextBox(update, 148, 62, 136);
        Theme.Label(update, "Class", 16, 100, 140);
        _cboClass = Theme.ComboBox(update, 16, 122, 120, Classes);
        Theme.Label(update, "Level", 148, 100, 60);
        _numLevel = Theme.Numeric(update, 148, 122, 60, 1, 100, 1);
        Theme.Label(update, "HP", 222, 100, 60);
        _numHp = Theme.Numeric(update, 222, 122, 62, 0, 100000, 100);
        _btnUpdate = Theme.Button(update, "Cập nhật nhân vật", 16, 164, 268, 36);
        _btnUpdate.Click += BtnUpdate_Click;

        var list = Theme.Section(this, "DANH SÁCH NHÂN VẬT CỦA BẠN", 330, 76, 808, 490);
        _grid = new DataGridView
        {
            Location = new Point(16, 44),
            Size = new Size(776, 430),
            BackgroundColor = Theme.Panel,
            ForeColor = Theme.Text,
            GridColor = Color.FromArgb(55, 58, 82),
            BackColor = Color.FromArgb(30, 32, 54),
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            Font = Theme.Font(10)
        };
        _grid.Columns.AddRange(
            new DataGridViewTextBoxColumn { HeaderText = "Id", DataPropertyName = "Id", Width = 60 },
            new DataGridViewTextBoxColumn { HeaderText = "Name", DataPropertyName = "Name", Width = 220 },
            new DataGridViewTextBoxColumn { HeaderText = "Class", DataPropertyName = "Class", Width = 140 },
            new DataGridViewTextBoxColumn { HeaderText = "Level", DataPropertyName = "Level", Width = 90 },
            new DataGridViewTextBoxColumn { HeaderText = "Hp", DataPropertyName = "Hp", Width = 120 }
        );
        Theme.StyleGrid(_grid);
        _grid.SelectionChanged += Grid_SelectionChanged;
        list.Controls.Add(_grid);

        _lblNotify = new Label
        {
            Location = new Point(330, 580),
            Size = new Size(808, 60),
            ForeColor = Theme.Success,
            Font = Theme.Font(11),
            BackColor = Color.Transparent
        };
        Controls.Add(_lblNotify);
    }

    private void Notify(string text, bool error = false)
    {
        _lblNotify.ForeColor = error ? Theme.Danger : Theme.Success;
        _lblNotify.Text = (error ? "✖ " : "✔ ") + text;
    }

    private void ShowCharacter(CharacterResponse c)
    {
        _lblCharacter.Text = $"ID: {c.Id}\nTên: {c.Name}\nClass: {c.Class}\nLevel: {c.Level}\nHP: {c.Hp}";
    }

    private async Task LoadCharactersAsync()
    {
        SetBusy(true);
        var result = await _api.GetMyCharactersAsync();
        SetBusy(false);

        if (!result.Ok)
        {
            Notify(result.Error, true);
            return;
        }

        _grid.DataSource = result.Data;
        if (result.Data?.Count > 0)
        {
            ShowCharacter(result.Data[0]);
            Notify($"Đã tải {result.Data.Count} nhân vật từ API.");
        }
        else
        {
            Notify("Chưa có nhân vật. Tạo một nhân vật để bắt đầu.");
        }
    }

    private void SetBusy(bool busy)
    {
        _btnCreate.Enabled = _btnUpdate.Enabled = !busy;
        Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
    }

    private void Grid_SelectionChanged(object? sender, EventArgs e)
    {
        if (_grid.CurrentRow?.DataBoundItem is not CharacterResponse c) return;
        _txtId.Text = c.Id.ToString();
        _txtName.Text = c.Name;
        _cboClass.SelectedItem = c.Class;
        _numLevel.Value = Math.Min(Math.Max(c.Level, 1), 100);
        _numHp.Value = Math.Min(c.Hp, 100000);
        ShowCharacter(c);
    }

    private async void BtnCreate_Click(object? sender, EventArgs e)
    {
        string name = _txtNewName.Text.Trim();
        if (name.Length is < 2 or > 50)
        {
            Notify("Tên nhân vật phải từ 2 đến 50 ký tự.", true);
            return;
        }

        SetBusy(true);
        Notify("Đang tạo nhân vật...");
        var result = await _api.CreateCharacterAsync(new CreateCharacterRequest
        {
            Name = name,
            Class = _cboNewClass.SelectedItem?.ToString() ?? "Warrior"
        });
        SetBusy(false);

        if (!result.Ok || result.Data == null)
        {
            Notify(result.Error, true);
            return;
        }

        ShowCharacter(result.Data);
        _txtNewName.Clear();
        await LoadCharactersAsync();
        Notify($"Tạo nhân vật thành công: {result.Data.Name} | Level {result.Data.Level} | HP {result.Data.Hp}");
    }

    private async void BtnUpdate_Click(object? sender, EventArgs e)
    {
        if (!int.TryParse(_txtId.Text, out int id))
        {
            Notify("Nhập Character ID hợp lệ hoặc chọn một dòng trong danh sách.", true);
            return;
        }

        SetBusy(true);
        Notify("Đang cập nhật nhân vật...");
        var result = await _api.UpdateCharacterAsync(id, new UpdateCharacterRequest
        {
            Name = _txtName.Text.Trim(),
            Class = _cboClass.SelectedItem?.ToString() ?? "Warrior",
            Level = (int)_numLevel.Value,
            Hp = (int)_numHp.Value
        });
        SetBusy(false);

        if (!result.Ok || result.Data == null)
        {
            Notify(result.Error, true);
            return;
        }

        ShowCharacter(result.Data);
        await LoadCharactersAsync();
        Notify($"Cập nhật nhân vật thành công: {result.Data.Name} | Level {result.Data.Level} | HP {result.Data.Hp}");
    }

    private void BtnLogout_Click(object? sender, EventArgs e)
    {
        Session.Clear();
        new FrmLogin().Show();
        Close();
    }
}
