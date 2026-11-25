using System.Windows.Forms;
using GameClient_WinForms.Models;
using GameClient_WinForms.Services;

namespace GameClient_WinForms.Forms;

public class FrmRegister : Form
{
    private readonly ApiService _api;
    private TextBox _username = null!;
    private TextBox _password = null!;
    private TextBox _confirm = null!;
    private Label _lblMessage = null!;
    private Button _btnSubmit = null!;

    public FrmRegister(ApiService api)
    {
        _api = api;
        BuildUi();
    }

    private void BuildUi()
    {
        Theme.StyleForm(this, 480, 430);
        Text = "Game Adventure - Đăng ký";

        Theme.Emblem(this, 32, 12);
        Theme.Title(this, "Tạo tài khoản", 104, 26, 340, 24);

        var card = new Panel
        {
            Location = new Point(32, 76),
            Size = new Size(416, 250),
            BackColor = Theme.Panel
        };
        Controls.Add(card);

        Theme.Label(card, "Tên đăng nhập (3-50 ký tự)", 22, 20, 300);
        _username = Theme.TextBox(card, 22, 44, 372);

        Theme.Label(card, "Mật khẩu (6-100 ký tự)", 22, 88, 300);
        _password = Theme.TextBox(card, 22, 112, 372, password: true);

        Theme.Label(card, "Xác nhận mật khẩu", 22, 156, 300);
        _confirm = Theme.TextBox(card, 22, 180, 372, password: true);

        _lblMessage = new Label
        {
            Location = new Point(34, 336),
            Size = new Size(410, 32),
            Font = Theme.Font(10),
            BackColor = Color.Transparent
        };
        Controls.Add(_lblMessage);

        _btnSubmit = Theme.Button(this, "Tạo tài khoản", 32, 368, 200);
        _btnSubmit.Click += BtnSubmit_Click;
        var btnBack = Theme.Button(this, "Quay lại đăng nhập", 248, 368, 200, primary: false);
        btnBack.Click += (_, _) => Close();
    }

    private async void BtnSubmit_Click(object? sender, EventArgs e)
    {
        string username = _username.Text.Trim();
        if (username.Length is < 3 or > 50)
        {
            _lblMessage.ForeColor = Theme.Danger;
            _lblMessage.Text = "Tên đăng nhập phải từ 3 đến 50 ký tự.";
            return;
        }
        if (_password.Text.Length is < 6 or > 100)
        {
            _lblMessage.ForeColor = Theme.Danger;
            _lblMessage.Text = "Mật khẩu phải từ 6 đến 100 ký tự.";
            return;
        }
        if (_password.Text != _confirm.Text)
        {
            _lblMessage.ForeColor = Theme.Danger;
            _lblMessage.Text = "Mật khẩu xác nhận không khớp.";
            return;
        }

        _btnSubmit.Enabled = false;
        _lblMessage.ForeColor = Theme.Text;
        _lblMessage.Text = "Đang tạo tài khoản...";

        var result = await _api.RegisterAsync(new RegisterRequest
        {
            Username = username,
            Password = _password.Text
        });

        _btnSubmit.Enabled = true;
        if (!result.Ok)
        {
            _lblMessage.ForeColor = Theme.Danger;
            _lblMessage.Text = result.Error;
            return;
        }

        MessageBox.Show("Đăng ký thành công. Bạn có thể đăng nhập ngay.", "Game Adventure",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        DialogResult = DialogResult.OK;
        Close();
    }
}
