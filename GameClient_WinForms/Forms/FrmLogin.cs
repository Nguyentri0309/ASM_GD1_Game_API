using System.Windows.Forms;
using GameClient_WinForms.Models;
using GameClient_WinForms.Services;

namespace GameClient_WinForms.Forms;

public class FrmLogin : Form
{
    private readonly ApiService _api = new();
    private TextBox _username = null!;
    private TextBox _password = null!;
    private CheckBox _showPassword = null!;
    private Button _btnLogin = null!;
    private Button _btnRegister = null!;
    private Label _lblMessage = null!;

    public FrmLogin()
    {
        BuildUi();
    }

    private void BuildUi()
    {
        Theme.StyleForm(this, 520, 440);
        Text = "Game Adventure - Đăng nhập";

        Theme.Emblem(this, 40, 16);
        Theme.Title(this, "GAME ADVENTURE", 112, 30, 360, 28);
        Theme.Label(this, "Nhập thông tin tài khoản để bắt đầu cuộc phiêu lưu", 42, 80, 440);

        var card = new Panel
        {
            Location = new Point(40, 110),
            Size = new Size(440, 250),
            BackColor = Theme.Panel
        };
        Controls.Add(card);

        Theme.Label(card, "Tên đăng nhập", 24, 24, 200);
        _username = Theme.TextBox(card, 24, 48, 392);

        Theme.Label(card, "Mật khẩu", 24, 92, 200);
        _password = Theme.TextBox(card, 24, 116, 392, password: true);

        _showPassword = new CheckBox
        {
            Text = "Hiện mật khẩu",
            Location = new Point(24, 154),
            Size = new Size(160, 24),
            ForeColor = Theme.Text,
            BackColor = Color.Transparent
        };
        _showPassword.CheckedChanged += (_, _) => _password.UseSystemPasswordChar = !_showPassword.Checked;
        card.Controls.Add(_showPassword);

        _btnLogin = Theme.Button(card, "Đăng nhập", 24, 190, 180);
        _btnLogin.Click += BtnLogin_Click;
        _btnRegister = Theme.Button(card, "Đăng ký", 236, 190, 180, primary: false);
        _btnRegister.Click += BtnRegister_Click;

        _lblMessage = new Label
        {
            Location = new Point(42, 372),
            Size = new Size(440, 40),
            ForeColor = Theme.Danger,
            Font = Theme.Font(10),
            BackColor = Color.Transparent
        };
        Controls.Add(_lblMessage);
    }

    private async void BtnLogin_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_username.Text) || string.IsNullOrWhiteSpace(_password.Text))
        {
            _lblMessage.ForeColor = Theme.Danger;
            _lblMessage.Text = "Nhập tên đăng nhập và mật khẩu.";
            return;
        }

        _btnLogin.Enabled = _btnRegister.Enabled = false;
        _lblMessage.ForeColor = Theme.Text;
        _lblMessage.Text = "Đang kết nối API...";

        var result = await _api.LoginAsync(new LoginRequest
        {
            Username = _username.Text.Trim(),
            Password = _password.Text
        });

        _btnLogin.Enabled = _btnRegister.Enabled = true;
        if (!result.Ok)
        {
            _lblMessage.ForeColor = Theme.Danger;
            _lblMessage.Text = result.Error == "Token hết hạn hoặc không có quyền truy cập (401)."
                ? "Sai tên đăng nhập hoặc mật khẩu."
                : result.Error;
            return;
        }

        _lblMessage.Text = string.Empty;
        var main = new FrmMain(_api);
        main.Show();
        Hide();
    }

    private void BtnRegister_Click(object? sender, EventArgs e)
    {
        using var register = new FrmRegister(_api);
        register.ShowDialog();
    }
}
